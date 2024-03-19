using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using DucumentProcessing;
using ExagenSharedProject;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using Repositories;
using Repositories.Models;
using Services.Interfaces;
using Services.Utilities;
using Services.ViewModels;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace Services.Services
{
    public class ExamServices : IExamServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDocumentServices _documentServices;
        private readonly HttpClient _httpClient;

        public ExamServices(IUnitOfWork unitOfWork, IMapper mapper, IDocumentServices documentServices, HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _documentServices = documentServices;
            _httpClient = httpClient;
        }

        public async Task<Response> SendImage(ResultForScanViewModel Image)
        {
            try
            { 
                var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

                var url = configuration["AI_Services"];
                string jsonString = JsonConvert.SerializeObject(Image);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{url}/answer_base64", content);
                var apiContent = await response.Content.ReadAsStringAsync();
                var resp = JsonConvert.DeserializeObject<Response>(apiContent);
                resp.result = EliminateMultipleChoice(resp.result);

                return resp;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở ExamServices - CallExternalApi: " + e.Message);
            }
        }

        public async Task<Guid?> AddExamByMatrixIntoClass(ExamCreate examCreate, Guid currentUserId)
        {
            try
            {
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

                Guid templatePaperId = Guid.Parse("2fc0e6e6-edd7-ee11-90e0-90610ca5f0a3");
                // create paper set
                var paperSet = new PaperSet();
                paperSet.SubjectId = (await _unitOfWork.SubjectRepo.FindByField(subject => subject.Grade == examCreate.Grade && EnumStatus.Subject[examCreate.SubjectEnum].Contains(subject.Name))).SubjectId;
                paperSet.ShuffleQuestion = examCreate.ConfigArrange.ShuffleQuestions;
                paperSet.ShuffleAnswer = examCreate.ConfigArrange.ShuffleAnswers;
                paperSet.SortByDifficulty = examCreate.ConfigArrange.ArrangeDifficulty;
                paperSet.Grade = examCreate.Grade;
                paperSet.Name = examCreate.Name; // nhập tên bộ đề chỗ nào
                _unitOfWork.PaperSetRepo.AddAsync(paperSet);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    return null;
                }
                
                //add exam
                var exam = _mapper.Map<Exam>(examCreate);
                exam.Status = 0;  // mới tạo exam nên chưa chốt kq
                exam.CreatedOn = DateTime.Now;
                exam.CreatedBy = currentUserId;
                exam.ModifiedOn = DateTime.Now;
                exam.ModifiedBy = currentUserId;
                exam.PaperSetId = paperSet.PaperSetId;
                
                //add exam mark
                var students = await _unitOfWork.StudentRepo.FindListByField(student => student.ClassId == examCreate.ClassId);
                var examMarks = new List<ExamMark>();
                foreach (var student in students)
                {
                    var examMark = new ExamMark
                    {
                        StudentId = student.StudentId,
                        StudentNo = student.StudentNo,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                    };
                    examMarks.Add(examMark);
                }
                exam.ExamMarks = examMarks;
                _unitOfWork.ExamRepo.AddAsync(exam);
                result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    return null;
                }

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
                    sectionConfigs.Add(sectionConfig);
                }
                _unitOfWork.SectionPaperSetConfigRepo.AddRangeAsync(sectionConfigs);
                
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
                        var questions = (await _unitOfWork.QuestionRepo.FindListByField(question => question.SectionId == sectionUse.SectionId && question.Difficulty  == sectionUse.Difficulty,
                                                                                        includes => includes.QuestionSet));
                        Utils.Shuffle(questions);

                        if (sectionUse.CHCN > 0 && sectionUse.NHD > 0)
                        {
                            // CHCN là được createdBy currentUserId
                            questionIdsUse.AddRange(questions.Where(question => question.QuestionSet.CreatedBy == currentUserId).Select(question => question.QuestionId).Take(sectionUse.CHCN));
                            // NHD là được public và được share 
                            questionIdsUse.AddRange(questions.Where(question => (question.QuestionSet.CreatedBy != currentUserId && question.QuestionSet.Status == 2) ||
                                                                                (sharedQuestionSetId.Contains((Guid)question.QuestionSetId))).Select(question => question.QuestionId).Take(sectionUse.NHD));
                        }
                        else if (sectionUse.NHD > 0)
                        {
                            // NHD là được public và được share
                            questionIdsUse.AddRange(questions.Where(question => (question.QuestionSet.CreatedBy != currentUserId && question.QuestionSet.Status == 2) ||
                                                                                (sharedQuestionSetId.Contains((Guid)question.QuestionSetId))).Select(question => question.QuestionId).Take(sectionUse.NHD));
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
                    }
                }
                else
                {
                    var questionIdsUse = new List<Guid>();
                    var questionsFromSet = (await _unitOfWork.QuestionRepo.FindListByField(question => question.QuestionSetId == examCreate.QuestionSetIdUse)).Select(s => s.SectionId);
                    foreach (var sectionUse in examCreate.Sections)
                    {
                        var sharedQuestionSetId = (await _unitOfWork.ShareRepo.FindListByField(share => share.UserId == currentUserId, includes => includes.QuestionSet)).Select(share => share.QuestionSetId).ToList();
                        var questions = (await _unitOfWork.QuestionRepo.FindListByField(question => question.SectionId == sectionUse.SectionId && question.Difficulty == sectionUse.Difficulty,
                                                                                        includes => includes.QuestionSet));
                        Utils.Shuffle(questions);
                        if (sectionUse.CHCN > 0 && sectionUse.NHD > 0)
                        {
                            // CHCN là được createdBy currentUserId
                            if (questionsFromSet.Contains(sectionUse.SectionId))
                            {
                                questionIdsUse.AddRange(questions.Where(question => question.QuestionSetId == examCreate.QuestionSetIdUse).Select(question => question.QuestionId).OrderBy(o => new Guid()).Take(sectionUse.CHCN));
                            }
                            else
                            {
                                questionIdsUse.AddRange(questions.Where(question => question.QuestionSet.CreatedBy == currentUserId).Select(question => question.QuestionId).OrderBy(o => new Guid()).Take(sectionUse.CHCN));
                            }
                            
                            // NHD là được public và được share 
                            questionIdsUse.AddRange(questions.Where(question => (question.QuestionSet.CreatedBy != currentUserId && question.QuestionSet.Status == 2) ||
                                                                                (sharedQuestionSetId.Contains((Guid)question.QuestionSetId) && question.Difficulty == sectionUse.Difficulty)).Select(question => question.QuestionId).Take(sectionUse.NHD));
                        }
                        else if (sectionUse.NHD > 0)
                        {
                            // NHD là được public và được share
                            questionIdsUse.AddRange(questions.Where(question => (question.QuestionSet.CreatedBy != currentUserId && question.QuestionSet.Status == 2) ||
                                                                                (sharedQuestionSetId.Contains((Guid)question.QuestionSetId) && question.Difficulty == sectionUse.Difficulty)).Select(question => question.QuestionId).Take(sectionUse.NHD));
                        }
                        else if (sectionUse.CHCN > 0)
                        {
                            // CHCN là được createdBy currentUserId
                            if (questionsFromSet.Contains(sectionUse.SectionId))
                            {
                                questionIdsUse.AddRange(questions.Where(question => question.QuestionSetId == examCreate.QuestionSetIdUse).Select(question => question.QuestionId).Take(sectionUse.CHCN));
                            }
                            else
                            {
                                questionIdsUse.AddRange(questions.Where(question => question.QuestionSet.CreatedBy == currentUserId).Select(question => question.QuestionId).Take(sectionUse.CHCN));
                            
                            }
                        }

                        src.Add(new SourceUse
                        {
                            QuestionIds = questionIdsUse,
                            Difficulty = sectionUse.Difficulty,
                            Use = sectionUse.Use
                        });
                    }
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
                            if (s.Difficulty == 0)
                            {
                                nb.AddRange(s.QuestionIds.Take(s.Use));
                            }
                            else if (s.Difficulty == 1)
                            {
                                th.AddRange(s.QuestionIds.Take(s.Use));
                            }
                            else if (s.Difficulty == 2)
                            {
                                vdt.AddRange(s.QuestionIds.Take(s.Use));
                            }
                            else if (s.Difficulty == 3)
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
                        NameOfTest = examCreate.Name,
                        Grade = examCreate.Grade,
                        SubjectName = EnumStatus.Subject[examCreate.SubjectEnum]
                    };

                    // shuffle moi ma de thi
                    if (examCreate.ConfigArrange.ShuffleQuestions && !examCreate.ConfigArrange.ArrangeDifficulty)
                    {
                        for (int j = 0; j < examCreate.NumOfPaperCode; j++)
                        {
                            Utils.Shuffle(questionIdsInPaper);
                            detailOfPaper.QuestionIds = questionIdsInPaper;
                            Guid id = await _documentServices.CreateTestPaper(currentUserId, paperSet.PaperSetId, detailOfPaper, templatePaperId, examCreate.ConfigArrange.ShuffleAnswers);
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
                            Guid id = await _documentServices.CreateTestPaper(currentUserId, paperSet.PaperSetId, detailOfPaper, templatePaperId, examCreate.ConfigArrange.ShuffleAnswers);
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
                            Guid id = await _documentServices.CreateTestPaper(currentUserId, paperSet.PaperSetId, detailOfPaper, templatePaperId, examCreate.ConfigArrange.ShuffleAnswers);
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
                            Guid id = await _documentServices.CreateTestPaper(currentUserId, paperSet.PaperSetId, detailOfPaper, templatePaperId, examCreate.ConfigArrange.ShuffleAnswers);
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
                }
                
                _unitOfWork.QuestionInExamRepo.AddRangeAsync(questionInExams);
                result = await _unitOfWork.SaveChangesAsync();

                return result > 0 ? exam.ExamId : null;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở ExamServices - AddExamIntoClass: " + e.Message);
            }

            throw new NotImplementedException();
        }

        public async Task<bool> CheckPermissionAccessTest(string testCode, string email)
        {
            try
            {

                var user = await _unitOfWork.UserRepo.FindByField(user => user.Email == email);

                if (user == null)
                {
                    return false;
                }
                
                int testCodeInt = int.Parse(testCode);

                var testResult = await _unitOfWork.ExamRepo.FindByField(testResult => testResult.TestCode == testCodeInt && testResult.CreatedBy == user.UserId);

                if (testResult == null)
                {
                    return false;
                }

                return true;

            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở TestResultServices - CheckPermissionAccerssTest: " + e.Message);
            }
        }

        public async Task<ExamInfo> GetExamInfo(Guid examId, Guid currentUseId)
        {
            try
            {
                var exam = await _unitOfWork.ExamRepo.FindByField(exam => exam.ExamId == examId && exam.CreatedBy == currentUseId,includes => includes.Class);
                if (exam == null)
                {
                    throw new Exception("Không tìm thấy thông tin cuộc thi");
                }

                var examInfo = _mapper.Map<ExamInfo>(exam);
                examInfo.ClassName = exam.Class.Name;
                var examMark = await _unitOfWork.ExamMarkRepo.FindListByField(examMark => examMark.ExamId == exam.ExamId);
                var count = examMark.Count(examMark => examMark.Mark != null);
                examInfo.HasMark = count + "/" + examMark.Count;

                var studentInExam = await _unitOfWork.ExamMarkRepo.FindListByField(examMark => examMark.ExamId == examId, includes => includes.Student);

                var students = _mapper.Map<IEnumerable<ResultOfStudent>>(studentInExam);
                examInfo.Students = students;   
                return examInfo;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở TestResultServices - GetExamInfo: " + e.Message);
            }
        }

        public async Task<InfoClassInExam> GetInfoOfClassInExam(string testCode, string email)
        {
            try
            {

                var user = await _unitOfWork.UserRepo.FindByField(user => user.Email == email);

                if (user == null)
                {
                    throw new Exception("Không tìm thấy user");
                }

                int testCodeInt = int.Parse(testCode);

                List<ComboStudent> studentInExam = new List<ComboStudent>();
               
                var exam = await _unitOfWork.ExamRepo.FindByField(exams => exams.TestCode == testCodeInt && exams.CreatedBy == user.UserId, includes => includes.ExamMarks);

                if (exam == null)
                {
                    throw new Exception("Không tìm thấy kết quả thi");
                }

                var infoClassInExam = new InfoClassInExam { 
                    DescriptionOfTest = exam.Name,
                    TestCode = exam.TestCode,
                };
                
                if (exam.ExamMarks.Count > 0)
                {                    
                    foreach (var item in exam.ExamMarks)
                    {
                        var student = await _unitOfWork.StudentRepo.FindByField(student => student.StudentId == item.StudentId);
                        
                        var comboStudent = new ComboStudent
                        {
                            ExamMarkId = item.ExamMarkId,
                            StudentId = item.StudentId,
                            Name = student.FullName,
                            Mark = item.Mark,
                            No = student.StudentNo
                        };
                        studentInExam.Add(comboStudent);
                    }

                    infoClassInExam.StudentInExam = studentInExam;
                }

                return infoClassInExam; 

            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở TestResultServices - GetInfoOfClassInExam: " + e.Message);
            }
        }

        public async Task<IEnumerable<ExamViewModels>> GetOwnExam(Guid currentUserId, int? grade)
        {
            try
            {
                var exams = await _unitOfWork.ExamRepo.FindListByField(exam => exam.CreatedBy == currentUserId, includes => includes.Class);
                if (exams == null)
                {
                    return null;
                }

                if (grade != null)
                {
                    exams = exams.Where(exam => exam.Class.Grade == grade).ToList();
                }

                var examViewModels = _mapper.Map<IEnumerable<ExamViewModels>>(exams);
                foreach (var exam in examViewModels)
                {
                    var examMark = await _unitOfWork.ExamMarkRepo.FindListByField(examMark => examMark.ExamId == exam.ExamId);
                    var count = examMark.Count(examMark => examMark.Mark != null);
                    exam.HasMark = count + "/" + examMark.Count;
                }
                return examViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở TestResultServices - GetOwnExam: " + e.Message);
            }
        }

        public async Task<decimal?> SaveResult(ResultToSave resultToSave)
        {
            try
            {
                var examMark = await _unitOfWork.ExamMarkRepo.FindByField(exam => exam.ExamMarkId == resultToSave.ExamMarkId, includes => includes.Exam);
                if (examMark == null)
                {
                    throw new Exception("Không tìm thấy kết quả thi");
                }

                //join exam and paper exam and paper and select paper content
                var paperExam = await _unitOfWork.PaperRepo.FindByField(paper => paper.PaperSetId == examMark.Exam.PaperSetId && paper.PaperCode == resultToSave.PaperCode);
                if (paperExam == null)
                {
                    throw new Exception("Không tìm thấy đề thi");
                }

                var answerSheet = paperExam.PaperAnswer;

                List<string> correctAnswers = new List<string>();
                var answerSheetList = answerSheet.Split('|').ToList();
                foreach (var answer in answerSheetList)
                {
                    correctAnswers.Add(answer.Substring(37));
                }

                List<string> studentAnswers = resultToSave.AnswersSelected.Split('|').ToList();

                decimal? mark = 0;
                int maxAnswer = correctAnswers.Count < studentAnswers.Count ? correctAnswers.Count : studentAnswers.Count;
                for (int i = 0; i < maxAnswer; i++)
                {
                    if (correctAnswers[i] == studentAnswers[i])
                    {
                        mark += (decimal)10 / (decimal)correctAnswers.Count;
                    }
                }

                examMark.AnswersSelected = resultToSave.AnswersSelected;
                examMark.Mark = mark;
                examMark.PaperCode = resultToSave.PaperCode;
                examMark.ModifiedOn = DateTime.Now;
                _unitOfWork.ExamMarkRepo.Update(examMark);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0 ? mark : -1;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SaveResultServices - SaveResult: " + e.Message);
            }

            throw new NotImplementedException();
        }

        private string EliminateMultipleChoice(string answer)
        {
            var listAnswer = answer.Split('|').ToList();
            for (int i = 0; i < listAnswer.Count; i++)
            {
                if (listAnswer[i].Contains(","))
                {
                    listAnswer[i] = $"{i+1}:";
                }
            }
            return string.Join("|", listAnswer);
        }

        public async Task<ExamSourceViewModel> GetAllExamSource(Guid examId)
        {
            try
            {
                var exam = await _unitOfWork.ExamRepo.FindByField(exam => exam.ExamId == examId);
                if (exam == null)
                {
                    throw new Exception("Không tìm thấy đề thi");
                }
                var papers = await _unitOfWork.PaperRepo.FindListByField(paper => paper.PaperSetId == exam.PaperSetId);
                var anserSheet = await _unitOfWork.DocumentRepo.FindListByField(document => document.Type == 0);
                var result = new ExamSourceViewModel
                {
                    paperOfExams = _mapper.Map<List<PaperOfExam>>(papers),
                    anserSheets = _mapper.Map<List<AnserSheet>>(anserSheet)
                };
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở TestResultServices - GetAllExamSource: " + e.Message);
            }
        }

        public async Task<byte[]> GetPaperById(Guid paperId)
        {
            try 
            {                 
                var paper = await _unitOfWork.PaperRepo.FindByField(paper => paper.PaperId == paperId);
                if (paper == null)
                {
                    throw new Exception("Không tìm thấy đề thi");
                }
                return paper.PaperContent;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở TestResultServices - GetPaperById: " + e.Message);
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
                    worksheet.Cells[3, 1].Value = "Mã số sinh viên";
                    worksheet.Cells[3, 2].Value = "Họ và tên";
                    worksheet.Cells[3, 3].Value = "Mã đề";
                    worksheet.Cells[3, 4].Value = "Điểm";

                    // Add data to the worksheet
                    // Assuming you have a list of students with their details
                    int row = 4;
                    foreach (var em in examMark.OrderBy(o => o.StudentNo))
                    {
                        worksheet.Cells[row, 1].Value = em.StudentNo;
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
    }
}
