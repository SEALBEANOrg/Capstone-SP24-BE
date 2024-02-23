﻿using AutoMapper;
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
                var paperIds = (await _unitOfWork.PaperExamRepo.FindListByField(exam => exam.ExamId == examMark.ExamId, includes => includes.Paper)).Select(x => x.PaperId);
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
    }
}
