using AutoMapper;
using ExagenSharedProject;
using OfficeOpenXml;
using Repositories;
using Repositories.Models;
using Services.Interfaces;
using Services.Interfaces.Paper;
using Services.Interfaces.Storage;
using Services.Utilities;
using Services.ViewModels;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace Services.Services.Exam
{
    public class ExamServices : IExamServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaperServices _paperServices;
        private readonly IS3Services _s3Services;

        public ExamServices(IUnitOfWork unitOfWork, IMapper mapper, IS3Services s3Services, IPaperServices paperServices)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paperServices = paperServices;
            _s3Services = s3Services;
        }

        public async Task<Guid?> AddExamByMatrixIntoClass(ExamCreate examCreate, Guid currentUserId)
        {
            try
            {
                var user = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);
                
                this.ValidateExamCreate(examCreate, user);

                Guid templatePaperId = (await _unitOfWork.DocumentRepo.FindByField(template => template.Type == 1)).DocumentId;
                // create paper set
                var paperSet = new PaperSet();
                paperSet.SubjectId = (await _unitOfWork.SubjectRepo.FindByField(subject => subject.Grade == examCreate.Grade && EnumStatus.Subject[examCreate.SubjectEnum].Contains(subject.Name))).SubjectId;
                paperSet.ShuffleQuestion = examCreate.ConfigArrange.ShuffleQuestions;
                paperSet.ShuffleAnswer = examCreate.ConfigArrange.ShuffleAnswers;
                paperSet.SortByDifficulty = examCreate.ConfigArrange.ArrangeDifficulty;
                paperSet.Grade = examCreate.Grade;
                paperSet.Name = examCreate.Name; // nhập tên bộ đề chỗ nào
                paperSet.KeyS3 = "";
                _unitOfWork.PaperSetRepo.AddAsync(paperSet);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    return null;
                }

                //add exam
                var exam = _mapper.Map<Repositories.Models.Exam>(examCreate);
                exam.Status = 0;  // mới tạo exam nên chưa chốt kq
                exam.CreatedOn = DateTime.Now.AddHours(7);
                exam.CreatedBy = currentUserId;
                exam.ModifiedOn = DateTime.Now.AddHours(7);
                exam.ModifiedBy = currentUserId;
                exam.PaperSetId = paperSet.PaperSetId;
                exam.SubjectId = paperSet.SubjectId;

                //add exam mark
                var students = await _unitOfWork.StudentRepo.FindListByField(student => student.ClassId == examCreate.ClassId);
                var examMarks = new List<Repositories.Models.ExamMark>();
                foreach (var student in students)
                {
                    var examMark = new Repositories.Models.ExamMark
                    {
                        StudentId = student.StudentId,
                        CreatedOn = DateTime.Now.AddHours(7),
                        ModifiedOn = DateTime.Now.AddHours(7),
                    };
                    examMarks.Add(examMark);
                }
                exam.ExamMarks = examMarks;
                _unitOfWork.ExamRepo.AddAsync(exam);
                result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    _unitOfWork.PaperSetRepo.Remove(paperSet);
                    await _unitOfWork.SaveChangesAsync();
                    return null;
                }

                int totalUse = 0;
                //add section config
                var sectionConfigs = new List<SectionPaperSetConfig>();
                foreach (var section in examCreate.Sections)
                {
                    var sectionConfig = new SectionPaperSetConfig
                    {
                        PaperSetId = paperSet.PaperSetId,
                        Difficulty = section.Difficulty,
                        NumberOfUse = section.CHCN + section.NHD,
                        SectionId = section.SectionId,
                        NumInPaper = section.Use
                    };
                    totalUse += section.Use;
                    sectionConfigs.Add(sectionConfig);
                }
                _unitOfWork.SectionPaperSetConfigRepo.AddRangeAsync(sectionConfigs);
                result = await _unitOfWork.SaveChangesAsync();
                if (result < sectionConfigs.Count)
                {
                    _unitOfWork.ExamRepo.Remove(exam);
                    _unitOfWork.PaperSetRepo.Remove(paperSet);
                    await _unitOfWork.SaveChangesAsync();
                    return null;
                }

                // create paper
                List<SourceUse> src = new List<SourceUse>();
                int paperCode = 1;
                List<Guid> paperIds = new List<Guid>();

                // lay src chung cho moi section
                if (examCreate.QuestionSetIdUse == null)
                {
                    foreach (var sectionUse in examCreate.Sections)
                    {
                        var questionIdsUse = new List<Guid>();
                        var sharedQuestionSetId = (await _unitOfWork.ShareRepo.FindListByField(share => share.UserId == currentUserId, includes => includes.QuestionSet)).Select(share => share.QuestionSetId).ToList();
                        var questions = await _unitOfWork.QuestionRepo.FindListByField(question => question.SectionId == sectionUse.SectionId && question.Difficulty == sectionUse.Difficulty,
                                                                                        includes => includes.QuestionSet);
                        Utils.Shuffle(questions);

                        if (sectionUse.CHCN > 0 && sectionUse.NHD > 0)
                        {
                            // CHCN là được createdBy currentUserId
                            questionIdsUse.AddRange(questions.Where(question => question.QuestionSet.CreatedBy == currentUserId).Select(question => question.QuestionId).Take(sectionUse.CHCN));
                            // NHD là được public và được share 
                            questionIdsUse.AddRange(questions.Where(question => question.QuestionSet.CreatedBy != currentUserId && question.QuestionSet.Status == 2 ||
                                                                                sharedQuestionSetId.Contains((Guid)question.QuestionSetId)).Select(question => question.QuestionId).Take(sectionUse.NHD));
                        }
                        else if (sectionUse.NHD > 0)
                        {
                            // NHD là được public và được share
                            questionIdsUse.AddRange(questions.Where(question => question.QuestionSet.CreatedBy != currentUserId && question.QuestionSet.Status == 2 ||
                                                                                sharedQuestionSetId.Contains((Guid)question.QuestionSetId)).Select(question => question.QuestionId).Take(sectionUse.NHD));
                        }
                        else if (sectionUse.CHCN > 0)
                        {
                            // CHCN là được createdBy currentUserId
                            questionIdsUse.AddRange(questions.Where(question => question.QuestionSet.CreatedBy == currentUserId).Select(question => question.QuestionId).Take(sectionUse.CHCN));
                        }

                        src.Add(new SourceUse
                        {
                            QuestionIds = questionIdsUse,
                            Difficulty = sectionUse.Difficulty,
                            Use = sectionUse.Use
                        });
                        questionIdsUse = new List<Guid>();
                    }
                }
                else
                {
                    var questionIdsUse = new List<Guid>();
                    var questionsFromSet = (await _unitOfWork.QuestionRepo.FindListByField(question => question.QuestionSetId == examCreate.QuestionSetIdUse, includes => includes.QuestionSet));
                    foreach (var sectionUse in examCreate.Sections)
                    {
                        var questions = questionsFromSet.Where(question => question.SectionId == sectionUse.SectionId && question.Difficulty == sectionUse.Difficulty).ToList();
                        Utils.Shuffle(questions);
                        if (sectionUse.NHD > 0)
                        {
                            // NHD là được public và được share
                            questionIdsUse.AddRange(questions.Select(question => question.QuestionId).Take(sectionUse.NHD));
                        }
                        else if (sectionUse.CHCN > 0)
                        {
                            // CHCN là được createdBy currentUserId
                            questionIdsUse.AddRange(questions.Select(question => question.QuestionId).Take(sectionUse.CHCN));
                        }

                        src.Add(new SourceUse
                        {
                            QuestionIds = questionIdsUse,
                            Difficulty = sectionUse.Difficulty,
                            Use = sectionUse.Use
                        });
                        questionIdsUse = new List<Guid>();
                    }
                }

                string nameOfExam = Utils.FormatFileName(examCreate.Name + $"-{DateTime.Now.AddHours(7).Ticks}");
                // Tạo file Excel để lưu điểm 
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells[1, 1, 2, 1].Merge = true;
                worksheet.Cells[1, 1].Value = "Câu hỏi";
                worksheet.Cells[1, 1, totalUse + 1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1, totalUse + 2, examCreate.NumOfDiffPaper * examCreate.NumOfPaperCode + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1, totalUse + 2, examCreate.NumOfDiffPaper * examCreate.NumOfPaperCode + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                worksheet.Cells.Style.Font.Size = 12;
                worksheet.Cells.Style.Font.Name = "Arial";
                worksheet.Cells[1, 2, 1, examCreate.NumOfDiffPaper * examCreate.NumOfPaperCode + 1].Merge = true;
                worksheet.Cells[1, 2].Value = "Mã đề thi";
                worksheet.Cells[1, 2].Style.Font.Bold = true;

                for (int i = 1; i <= totalUse; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = i;
                    worksheet.Cells[i + 2, 1].Style.Font.Bold = true;
                }

                // moi de thi lay src rieng
                var questionIdsInPaper = new List<Guid>();
                var questionInExams = new List<QuestionInExam>();
                for (int i = 0; i < examCreate.NumOfDiffPaper; i++)
                {
                    List<Guid> nb = new List<Guid>();
                    List<Guid> th = new List<Guid>();
                    List<Guid> vdt = new List<Guid>();
                    List<Guid> vdc = new List<Guid>();

                    //lay cac cau se su dung
                    if (examCreate.ConfigArrange.ArrangeDifficulty)
                    {
                        foreach (var s in src)
                        {
                            Utils.Shuffle(s.QuestionIds);
                            if (s.Difficulty == OptionSet.Question.Difficulty.NB)
                            {
                                nb.AddRange(s.QuestionIds.Take(s.Use));
                            }
                            else if (s.Difficulty == OptionSet.Question.Difficulty.TH)
                            {
                                th.AddRange(s.QuestionIds.Take(s.Use));
                            }
                            else if (s.Difficulty == OptionSet.Question.Difficulty.VD)
                            {
                                vdt.AddRange(s.QuestionIds.Take(s.Use));
                            }
                            else if (s.Difficulty == OptionSet.Question.Difficulty.VDC)
                            {
                                vdc.AddRange(s.QuestionIds.Take(s.Use));
                            }
                        }
                    }
                    else
                    {
                        foreach (var s in src)
                        {
                            Utils.Shuffle(s.QuestionIds);
                            questionIdsInPaper.AddRange(s.QuestionIds.Take(s.Use));
                        }
                    }

                    var detailOfPaper = new DetailOfPaper
                    {
                        TimeOfTest = examCreate.Duration,
                        PaperCode = paperCode,
                        NameOfTest = nameOfExam,
                        Grade = examCreate.Grade,
                        SubjectName = EnumStatus.Subject[examCreate.SubjectEnum],
                        NameInTest = examCreate.Name,
                    };

                    // shuffle moi ma de thi
                    if (examCreate.ConfigArrange.ShuffleQuestions && !examCreate.ConfigArrange.ArrangeDifficulty)
                    {
                        for (int j = 0; j < examCreate.NumOfPaperCode; j++)
                        {
                            Utils.Shuffle(questionIdsInPaper);
                            detailOfPaper.QuestionIds = questionIdsInPaper;
                            Guid id = await _paperServices.CreateTestPaper(currentUserId, paperSet.PaperSetId, detailOfPaper, templatePaperId, examCreate.ConfigArrange.ShuffleAnswers, worksheet);
                            if (id.Equals(Guid.Empty))
                            {
                                var paper = await _unitOfWork.PaperRepo.FindListByField(paper => paper.PaperSetId == paperSet.PaperSetId);
                                if (paper != null)
                                {
                                    _unitOfWork.PaperRepo.RemoveRange(paper);
                                }
                                _unitOfWork.SectionPaperSetConfigRepo.RemoveRange(sectionConfigs);
                                _unitOfWork.ExamRepo.Remove(exam);
                                _unitOfWork.PaperSetRepo.Remove(paperSet);
                                await _unitOfWork.SaveChangesAsync();
                                return null;
                            }
                            paperIds.Add(id);
                            detailOfPaper.PaperCode = ++paperCode;
                        }
                    }
                    else if (examCreate.ConfigArrange.ShuffleQuestions && examCreate.ConfigArrange.ArrangeDifficulty)
                    {
                        for (int j = 0; j < examCreate.NumOfPaperCode; j++)
                        {
                            detailOfPaper.QuestionIds = new List<Guid>();
                            Utils.Shuffle(nb);
                            detailOfPaper.QuestionIds.AddRange(nb);
                            Utils.Shuffle(th);
                            detailOfPaper.QuestionIds.AddRange(th);
                            Utils.Shuffle(vdt);
                            detailOfPaper.QuestionIds.AddRange(vdt);
                            Utils.Shuffle(vdc);
                            detailOfPaper.QuestionIds.AddRange(vdc);
                            Guid id = await _paperServices.CreateTestPaper(currentUserId, paperSet.PaperSetId, detailOfPaper, templatePaperId, examCreate.ConfigArrange.ShuffleAnswers, worksheet);
                            if (id.Equals(Guid.Empty))
                            {
                                var paper = await _unitOfWork.PaperRepo.FindListByField(paper => paper.PaperSetId == paperSet.PaperSetId);
                                if (paper != null)
                                {
                                    _unitOfWork.PaperRepo.RemoveRange(paper);
                                }
                                _unitOfWork.SectionPaperSetConfigRepo.RemoveRange(sectionConfigs);
                                _unitOfWork.ExamRepo.Remove(exam);
                                _unitOfWork.PaperSetRepo.Remove(paperSet);
                                await _unitOfWork.SaveChangesAsync();
                                return null;
                            }
                            paperIds.Add(id);
                            detailOfPaper.PaperCode = ++paperCode;
                        }
                    }
                    else if (!examCreate.ConfigArrange.ShuffleQuestions && examCreate.ConfigArrange.ArrangeDifficulty)
                    {
                        detailOfPaper.QuestionIds = new List<Guid>();
                        Utils.Shuffle(nb);
                        detailOfPaper.QuestionIds.AddRange(nb);
                        Utils.Shuffle(th);
                        detailOfPaper.QuestionIds.AddRange(th);
                        Utils.Shuffle(vdt);
                        detailOfPaper.QuestionIds.AddRange(vdt);
                        Utils.Shuffle(vdc);
                        detailOfPaper.QuestionIds.AddRange(vdc);
                        for (int j = 0; j < examCreate.NumOfPaperCode; j++)
                        {
                            Guid id = await _paperServices.CreateTestPaper(currentUserId, paperSet.PaperSetId, detailOfPaper, templatePaperId, examCreate.ConfigArrange.ShuffleAnswers, worksheet);
                            if (id.Equals(Guid.Empty))
                            {
                                var paper = await _unitOfWork.PaperRepo.FindListByField(paper => paper.PaperSetId == paperSet.PaperSetId);
                                if (paper != null)
                                {
                                    _unitOfWork.PaperRepo.RemoveRange(paper);
                                }
                                _unitOfWork.SectionPaperSetConfigRepo.RemoveRange(sectionConfigs);
                                _unitOfWork.ExamRepo.Remove(exam);
                                _unitOfWork.PaperSetRepo.Remove(paperSet);
                                await _unitOfWork.SaveChangesAsync();
                                return null;
                            }
                            paperIds.Add(id);
                            detailOfPaper.PaperCode = ++paperCode;
                        }
                    }
                    else
                    {
                        Utils.Shuffle(questionIdsInPaper);
                        detailOfPaper.QuestionIds = questionIdsInPaper;
                        for (int j = 0; j < examCreate.NumOfPaperCode; j++)
                        {
                            Guid id = await _paperServices.CreateTestPaper(currentUserId, paperSet.PaperSetId, detailOfPaper, templatePaperId, examCreate.ConfigArrange.ShuffleAnswers, worksheet);
                            if (id.Equals(Guid.Empty))
                            {
                                var paper = await _unitOfWork.PaperRepo.FindListByField(paper => paper.PaperSetId == paperSet.PaperSetId);
                                if (paper != null)
                                {
                                    _unitOfWork.PaperRepo.RemoveRange(paper);
                                }
                                _unitOfWork.SectionPaperSetConfigRepo.RemoveRange(sectionConfigs);
                                _unitOfWork.ExamRepo.Remove(exam);
                                _unitOfWork.PaperSetRepo.Remove(paperSet);
                                await _unitOfWork.SaveChangesAsync();
                                return null;
                            }
                            paperIds.Add(id);
                            detailOfPaper.PaperCode = ++paperCode;
                        }
                    }

                    //add questioninexam
                    foreach (var questionId in detailOfPaper.QuestionIds)
                    {
                        if (questionInExams.Where(qie => qie.ExamId == exam.ExamId && qie.QuestionId == questionId).Count() == 0)
                        {
                            var qie = new QuestionInExam
                            {
                                ExamId = exam.ExamId,
                                QuestionId = questionId,
                                CorrectCount = 0,
                                UseCount = 0
                            };

                            questionInExams.Add(qie);
                        }
                    }

                    questionIdsInPaper = new List<Guid>();
                }
                package.Save();
                // convert package to memorystream 
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                _s3Services.UploadFileIntoS3Async(stream, $"papers/{currentUserId}/{nameOfExam}/DapAnTongHop.xlsx");
                paperSet.KeyS3 = $"papers/{currentUserId}/{nameOfExam}/DapAnTongHop.xlsx";

                //charge user when create paper
                user.Point -= examCreate.NumOfDiffPaper * examCreate.NumOfPaperCode * 10;

                var transaction = new Repositories.Models.Transaction
                {
                    Type = 1, // tạo đề
                    UserId = currentUserId,
                    PointValue = -(examCreate.NumOfDiffPaper * examCreate.NumOfPaperCode * 10),
                    CreatedOn = DateTime.Now.AddHours(7)
                };

                _unitOfWork.PaperSetRepo.Update(paperSet);
                _unitOfWork.QuestionInExamRepo.AddRangeAsync(questionInExams);
                _unitOfWork.UserRepo.Update(user);
                _unitOfWork.TransactionRepo.AddAsync(transaction);

                result = await _unitOfWork.SaveChangesAsync();

                if (result < questionInExams.Count + 3)
                {
                    var paper = await _unitOfWork.PaperRepo.FindListByField(paper => paper.PaperSetId == paperSet.PaperSetId);
                    _unitOfWork.PaperRepo.RemoveRange(paper);
                    _unitOfWork.SectionPaperSetConfigRepo.RemoveRange(sectionConfigs);
                    _unitOfWork.ExamRepo.Remove(exam);
                    _unitOfWork.PaperSetRepo.Remove(paperSet);
                    await _unitOfWork.SaveChangesAsync();
                    return null;
                }

                return exam.ExamId;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở ExamServices - AddExamIntoClass: " + e.Message);
            }
        }

        public async Task<IEnumerable<ExamViewModels>> GetOwnExam(Guid currentUserId, int? grade, string studyYear)
        {
            try
            {
                var exams = await _unitOfWork.ExamRepo.FindListByField(exam => exam.CreatedBy == currentUserId && exam.StudyYear == studyYear, includes => includes.Class, includes => includes.Subject);
                if (exams == null)
                {
                    return null;
                }

                if (grade != null)
                {
                    exams = exams.Where(exam => exam.Class.Grade == grade).ToList();
                }

                var examViewModels = _mapper.Map<IEnumerable<ExamViewModels>>(exams);
               
                return examViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở TestResultServices - GetOwnExam: " + e.Message);
            }
        }

        public async Task<ExamInfo> GetExamInfo(Guid examId, Guid currentUseId)
        {
            try
            {
                var exam = await _unitOfWork.ExamRepo.FindByField(exam => exam.ExamId == examId && exam.CreatedBy == currentUseId, includes => includes.Class, includes => includes.Subject);
                if (exam == null)
                {
                    throw new Exception("Không tìm thấy thông tin cuộc thi");
                }

                var examInfo = _mapper.Map<ExamInfo>(exam);
                examInfo.ClassName = exam.Class.Name;
                examInfo.SubjectName = exam.Subject.Name;
                
                var studentInExam = await _unitOfWork.ExamMarkRepo.FindListByField(examMark => examMark.ExamId == examId, includes => includes.Student);

                var students = _mapper.Map<IEnumerable<ResultOfStudent>>(studentInExam.OrderBy(o => o.StudentCode));
                examInfo.Students = students;
                return examInfo;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở TestResultServices - GetExamInfo: " + e.Message);
            }
        }

        public async Task<ExamSourceViewModel> GetAllExamSource(Guid examId)
        {
            try
            {
                var exam = await _unitOfWork.ExamRepo.FindByField(exam => exam.ExamId == examId, include => include.Class, includes => includes.PaperSet);
                if (exam == null)
                {
                    throw new Exception("Không tìm thấy đề thi");
                }
                var papers = await _unitOfWork.PaperRepo.FindListByField(paper => paper.PaperSetId == exam.PaperSetId);
                var anserSheet = await _unitOfWork.DocumentRepo.FindListByField(document => document.Type == 0);

                var papersToDownload = new List<PaperOfExam>();
                foreach (var paper in papers)
                {
                    var p = _mapper.Map<PaperOfExam>(paper);
                    p.S3Url = await _s3Services.GetObjectUrlAsync(paper.KeyS3);
                    papersToDownload.Add(p);
                }

                var answerSheetToDownload = new List<AnserSheet>();
                foreach (var answer in anserSheet)
                {
                    var a = _mapper.Map<AnserSheet>(answer);
                    a.S3Url = await _s3Services.GetObjectUrlAsync(answer.KeyS3);
                    answerSheetToDownload.Add(a);
                }

                var subjectName = (await _unitOfWork.SubjectRepo.FindByField(subject => subject.SubjectId == exam.SubjectId)).Name;

                var result = new ExamSourceViewModel
                {
                    ExamName = exam.Name + " - " + exam.Class.Name + " - Môn " + subjectName,
                    PaperOfExams = papersToDownload,
                    AnserSheets = answerSheetToDownload,
                    FileTotalAnswer = new FileTotalAnswer { S3Url = await _s3Services.GetObjectUrlAsync(exam.PaperSet.KeyS3), Name = exam.Name + " - " + exam.Class.Name + " - Môn " + subjectName + " Đáp án tổng hợp"}
                };
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở TestResultServices - GetAllExamSource: " + e.Message);
            }
        }

        public async Task<ExportResult> ExportResult(Guid examId)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var exam = await _unitOfWork.ExamRepo.FindByField(exam => exam.ExamId == examId, includes => includes.Class);
            var examMark = await _unitOfWork.ExamMarkRepo.FindListByField(exam => exam.ExamId == examId, includes => includes.Student);
            var fileName = "BaoCaoDiem-" + exam.TestCode + "-" + exam.Class.Name + ".xlsx";
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Create a new Excel package
                using (ExcelPackage package = new ExcelPackage(memoryStream))
                {
                    // Add a worksheet to the Excel package
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    // Set column names
                    worksheet.Cells[1, 1].Value = $"Kì thi: {exam.Name} - Mã cuộc thi: {exam.TestCode}";
                    worksheet.Cells[2, 1].Value = $"Lớp: {exam.Class.Name}";
                    worksheet.Cells[3, 1].Value = "Mã số học sinh";
                    worksheet.Cells[3, 2].Value = "Họ và tên";
                    worksheet.Cells[3, 3].Value = "Mã đề";
                    worksheet.Cells[3, 4].Value = "Điểm";

                    // Add data to the worksheet
                    // Assuming you have a list of students with their details
                    int row = 4;
                    foreach (var em in examMark.OrderBy(o => o.StudentCode))
                    {
                        worksheet.Cells[row, 1].Value = em.StudentCode;
                        worksheet.Cells[row, 2].Value = em.Student.FullName;
                        worksheet.Cells[row, 3].Value = em.PaperCode;
                        worksheet.Cells[row, 4].Value = em.Mark;
                        row++;
                    }

                    // Save the Excel package to the memory stream
                    package.Save();
                }

                // Return the byte array
                return new ExportResult { Bytes = memoryStream.ToArray(), FileName = fileName };
            }

        }

        private void ValidateExamCreate(ExamCreate examCreate, Repositories.Models.User user)
        {
            if (examCreate.NumOfDiffPaper * examCreate.NumOfPaperCode * 10 > user.Point)
            {
                throw new Exception("Không đủ điểm để phát sinh đề");
            }
            if (examCreate.Sections.Count == 0)
            {
                throw new Exception("Không có phần tử nào trong sections");
            }
            if (examCreate.ClassId == null)
            {
                throw new Exception("Không có lớp");
            }
            if (examCreate.NumOfDiffPaper == 0)
            {
                throw new Exception("Số lượng đề thi phải lớn hơn 0");
            }
            if (examCreate.NumOfPaperCode == 0)
            {
                throw new Exception("Số lượng mã đề thi phải lớn hơn 0");
            }
            foreach (var section in examCreate.Sections)
            {
                if (section.CHCN + section.NHD < section.Use)
                {
                    throw new Exception("Số lượng câu hỏi phát sinh it hơn số lượng câu hỏi sử dụng");
                }
            };
        }
    }
}
