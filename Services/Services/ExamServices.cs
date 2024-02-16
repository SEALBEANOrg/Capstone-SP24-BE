using AutoMapper;
using Microsoft.AspNetCore.Http.Features;
using Repositories;
using Repositories.Models;
using Services.Interfaces;
using Services.ViewModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Services.Services
{
    public class ExamServices : IExamServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExamServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
    }
}
