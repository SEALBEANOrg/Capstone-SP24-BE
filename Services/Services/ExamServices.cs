using AutoMapper;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repositories;
using Repositories.Models;
using Services.Interfaces;
using Services.ViewModels;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

        public async Task<bool> AddExamByMatrixIntoClass(ExamCreate examCreate, Guid currentUserId)
        {
            try
            {
                Guid templatePaperId = Guid.Parse("2fc0e6e6-edd7-ee11-90e0-90610ca5f0a3");
                //add exam
                var exam = _mapper.Map<Exam>(examCreate);
                exam.CreatedOn = DateTime.Now;
                exam.CreatedBy = currentUserId;
                exam.Status = 0;  // mới tạo exam nên chưa chốt kq
                exam.ModifiedOn = DateTime.Now;
                exam.ModifiedBy = currentUserId;
                _unitOfWork.ExamRepo.AddAsync(exam);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    return false;
                }

                //add exam mark
                var students = await _unitOfWork.StudentRepo.FindListByField(student => student.ClassId == examCreate.ClassId);
                var examMarks = new List<ExamMark>();
                foreach (var student in students)
                {
                    var examMark = new ExamMark
                    {
                        ExamId = exam.ExamId,
                        StudentId = student.StudentId,
                        CreatedOn = DateTime.Now,
                        CreatedBy = currentUserId,
                        ModifiedOn = DateTime.Now,
                        ModifiedBy = currentUserId
                    };
                    examMarks.Add(examMark);
                }
                _unitOfWork.ExamMarkRepo.AddRangeAsync(examMarks);                           
                
                //get questions from question set
                List<List<Guid>> questionSets = new List<List<Guid>>();
                int paperCode = 1;
                List<Guid> paperIds = new List<Guid>();
                for (int i = 0; i < examCreate.NumOfDiffPaper; i++)
                {
                    var questionIdsUse = new List<Guid>();
                    foreach (var sectionUse in examCreate.Sections)
                    {
                        var questions = (await _unitOfWork.QuestionRepo.FindListByField(questionSet => questionSet.SectionId == sectionUse.SectionId)).OrderBy(order => Guid.NewGuid());

                        if (sectionUse.CHCN > 0)
                        {
                            questionIdsUse.AddRange(questions.Where(question => question.CreatedBy == currentUserId).Select(question => question.QuestionId).Take(sectionUse.CHCN));
                        }
                        if (sectionUse.NHD > 0)
                        {
                            questionIdsUse.AddRange(questions.Where(question => question.CreatedBy != currentUserId && question.IsPublic).Select(question => question.QuestionId).Take(sectionUse.NHD));
                        }
                    }

                    var detailOfPaper = new DetailOfPaper
                    {
                        QuestionIds = questionIdsUse,
                        TimeOfTest = examCreate.Duration,
                        PaperCode = paperCode,
                        NameOfTest = examCreate.NameOfExam
                    };

                    for (int j = 0; j < examCreate.NumOfPaperCode; j++)
                    {
                        Guid id = await _documentServices.CreateTestPaper(currentUserId, detailOfPaper, templatePaperId, Guid.Parse("00000000-0000-0000-0000-000000000001"));
                        paperIds.Add(id);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                // add paper exam
                var paperExams = new List<PaperExam>();
                foreach (var paperId in paperIds)
                {
                    var paperExam = new PaperExam
                    {
                        ExamId = exam.ExamId,
                        PaperId = paperId,
                    };
                    paperExams.Add(paperExam);
                }

                _unitOfWork.PaperExamRepo.AddRangeAsync(paperExams);
                result = await _unitOfWork.SaveChangesAsync();

                //if any save fail return false
                return result > 0 ? true : false;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở ExamServices - AddExamIntoClass: " + e.Message);
            }
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

                var studentInExam = await _unitOfWork.ExamMarkRepo.FindByField(examMark => examMark.ExamId == examId, includes => includes.Student);

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
                    DescriptionOfTest = exam.Description,
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
                var examMark = await _unitOfWork.ExamMarkRepo.FindByField(exam => exam.ExamMarkId == resultToSave.ExamMarkId);
                if (examMark == null)
                {
                    throw new Exception("Không tìm thấy kết quả thi");
                }

                //join exam and paper exam and paper and select paper content
                var paperIds = (await _unitOfWork.PaperExamRepo.FindListByField(exam => exam.ExamId == examMark.ExamId)).Select(x => x.PaperId);
                var paperExam = await _unitOfWork.PaperRepo.FindByField(paper => paperIds.Contains(paper.PaperId) && paper.PaperCode == resultToSave.PaperCode);
                if (paperExam == null)
                {
                    throw new Exception("Không tìm thấy đề thi");
                }

                var paperContentViewModel = JsonSerializer.Deserialize<PaperContentViewModel>(paperExam.PaperContent);
                var answerSheet = paperContentViewModel.Answer;

                List<string> correctAnswers = answerSheet.Split('|').ToList();
                List<string> studentAnswers = resultToSave.AnswersSelected.Split('|').ToList();
                
                decimal? mark = 0;
                int maxAnswer = correctAnswers.Count < studentAnswers.Count ? correctAnswers.Count : studentAnswers.Count;
                for (int i = 0; i < maxAnswer; i++)
                {
                    if (correctAnswers[i] == studentAnswers[i])
                    {
                        mark += (decimal)10/(decimal)correctAnswers.Count;
                    }
                }
                

                examMark.Answer = resultToSave.AnswersSelected;
                examMark.Mark = mark;
                examMark.ModifiedOn = DateTime.Now;
                _unitOfWork.ExamMarkRepo.Update(examMark);
                var result = await _unitOfWork.SaveChangesAsync();
                
                return result > 0 ? mark : -1;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SaveResultServices - SaveResult: " + e.Message);
            }
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
    }
}
