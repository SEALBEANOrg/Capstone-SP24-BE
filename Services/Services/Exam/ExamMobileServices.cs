using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repositories;
using Services.Interfaces.Exam;
using Services.ViewModels;
using System.Text;

namespace Services.Services.Exam
{
    public class ExamMobileServices : IExamMobileServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;

        public ExamMobileServices(IUnitOfWork unitOfWork, IMapper mapper, HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpClient = httpClient;
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
                throw new Exception("Lỗi ở ExamMobileServices - CheckPermissionAccerssTest: " + e.Message);
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

                var infoClassInExam = new InfoClassInExam
                {
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

                    infoClassInExam.StudentInExam = studentInExam.OrderBy(o => o.No).ToList();
                }

                return infoClassInExam;

            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở ExamMobileServices - GetInfoOfClassInExam: " + e.Message);
            }
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
                throw new Exception("Lỗi ở ExamMobileServices - CallExternalApi: " + e.Message);
            }
        }

        public async Task<bool> SaveResult(ResultToSave resultToSave)
        {
            try
            {
                var examMark = await _unitOfWork.ExamMarkRepo.FindByField(exam => exam.ExamMarkId == resultToSave.ExamMarkId);
                if (examMark == null)
                {
                    throw new Exception("Không tìm thấy kết quả thi");
                }

                var exam = await _unitOfWork.ExamRepo.FindByField(exam => exam.ExamId == examMark.ExamId);

                //join exam and paper exam and paper and select paper content
                var paperExam = await _unitOfWork.PaperRepo.FindByField(paper => paper.PaperSetId == exam.PaperSetId && paper.PaperCode == resultToSave.PaperCode);
                if (paperExam == null)
                {
                    throw new Exception("Không tìm thấy đề thi");
                }

                examMark.AnswersSelected = resultToSave.AnswersSelected;
                examMark.PaperCode = resultToSave.PaperCode;
                examMark.ModifiedOn = DateTime.Now;
                _unitOfWork.ExamMarkRepo.Update(examMark);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở ExamMobileServices - SaveResult: " + e.Message);
            }
        }

        private string EliminateMultipleChoice(string answer)
        {
            var listAnswer = answer.Split('|').ToList();
            for (int i = 0; i < listAnswer.Count; i++)
            {
                if (listAnswer[i].Contains(","))
                {
                    listAnswer[i] = $"{i + 1}:";
                }
            }
            return string.Join("|", listAnswer);
        }

    }
}
