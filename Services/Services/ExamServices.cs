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
               
                var testResult = await _unitOfWork.ExamRepo.FindByField(testResult => testResult.TestCode == testCodeInt && testResult.CreatedBy == user.UserId);

                if (testResult == null)
                {
                    throw new Exception("Không tìm thấy kết quả thi");
                }

                var infoClassInExam = new InfoClassInExam { 
                    DescriptionOfTest = testResult.Description,
                    TestCode = testResult.TestCode,
                };

                if (testResult.Marks == null)
                {
                    var student = await _unitOfWork.StudentRepo.FindListByField(student => student.ClassId == testResult.ClassId);
                    
                    if (student == null)
                    {
                        return null;
                    }

                    foreach (var item in student)
                    {
                        var comboStudent = new ComboStudent
                        {
                            StudentId = item.StudentId,
                            Name = item.FullName,
                            Mark = null,
                            No = item.StudentNo
                        };
                        studentInExam.Add(comboStudent);
                    }

                    infoClassInExam.StudentInExam = studentInExam;
                }
                else 
                { 
                    var marks = JsonSerializer.Deserialize<List<string>>(testResult.Marks);
                    
                    foreach (var item in marks)
                    {
                        // StudentName - No | 4
                        string[] parts = item.Split(new[] { " | " }, StringSplitOptions.None);
                        var comboStudent = new ComboStudent
                        {
                            Name = parts[0],
                            StudentId = Guid.Parse(parts[1]),
                        };
                        if (parts.Length == 3)
                        {
                            comboStudent.Mark = int.Parse(parts[2]);
                        }
                        else
                        {
                            comboStudent.Mark = null;
                        }
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
