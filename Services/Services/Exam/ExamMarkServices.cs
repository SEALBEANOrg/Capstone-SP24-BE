using AutoMapper;
using Repositories;
using Services.Interfaces;
using Services.Interfaces.Exam;
using Services.Interfaces.Paper;
using Services.Interfaces.Storage;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Exam
{
    public class ExamMarkServices : IExamMarkServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaperServices _paperServices;
        private readonly IExamServices _examServices;
        private readonly IS3Services _s3Services;

        public ExamMarkServices(IUnitOfWork unitOfWork, IExamServices examServices, IMapper mapper, IS3Services s3Services, IPaperServices paperServices)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paperServices = paperServices;
            _s3Services = s3Services;
            _examServices = examServices;
        }

        public async Task<ExamInfo> CalculateAllMark(Guid examId, Guid currentUserId)
        {
            try
            {
                var examMark = await _unitOfWork.ExamMarkRepo.FindListByField(exam => exam.ExamId == examId);
                if (examMark == null)
                {
                    throw new Exception("Không tìm thấy kết quả thi");
                }

                var currentUser = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);
                if (examMark.Count > currentUser.Point)
                {
                    throw new Exception("Không đủ xu để chấm điểm");
                }

                foreach (var em in examMark)
                {
                    if (string.IsNullOrEmpty(em.AnswersSelected))
                    {
                        em.Mark = 0;
                        continue;
                    }

                    var paperSetId = (await _unitOfWork.ExamRepo.FindByField(exam => exam.ExamId == examId)).PaperSetId;
                    var answerSheetList = (await _unitOfWork.PaperRepo.FindByField(paper => paper.PaperCode == em.PaperCode && paper.PaperSetId == paperSetId)).PaperAnswer.Split('|').ToList();
                    List<string> correctAnswers = new List<string>();
                    foreach (var answer in answerSheetList)
                    {
                        correctAnswers.Add(answer.Substring(37));
                    }

                    List<string> selectedAnswers = em.AnswersSelected.Split('|').ToList();

                    decimal? mark = 0;
                    int maxAnswer = correctAnswers.Count < selectedAnswers.Count ? correctAnswers.Count : selectedAnswers.Count;
                    for (int i = 0; i < maxAnswer; i++)
                    {
                        if (correctAnswers[i] == selectedAnswers[i])
                        {
                            mark += 10 / (decimal)correctAnswers.Count;
                        }
                    }

                    em.Mark = mark;
                    em.ModifiedOn = DateTime.Now;
                }

                _unitOfWork.ExamMarkRepo.UpdateRange(examMark);
                var result = await _unitOfWork.SaveChangesAsync();

                if (result < examMark.Count)
                {
                    throw new Exception("Xuất điểm bị lỗi");
                }

                var exam = await _unitOfWork.ExamRepo.FindByField(exam => exam.ExamId == examId);
                exam.Status = 1;
                _unitOfWork.ExamRepo.UpdateOnlyStatus(exam);
                result = await _unitOfWork.SaveChangesAsync();

                if (result <= 0)
                {
                    throw new Exception("Lỗi lưu trạng thái bài thi");
                }

                currentUser.Point -= examMark.Count;
                Repositories.Models.Transaction transaction = new Repositories.Models.Transaction
                {
                    PointValue = -examMark.Count,
                    Type = 2, // chấm bài
                    UserId = currentUserId,
                    CreatedOn = DateTime.Now
                };

                _unitOfWork.UserRepo.Update(currentUser);
                _unitOfWork.TransactionRepo.AddAsync(transaction);

                result = await _unitOfWork.SaveChangesAsync();

                if (result < 2)
                {
                    throw new Exception("Lỗi lưu giao dịch");
                }

                var examInfo = await _examServices.GetExamInfo(examId, currentUserId);
                return examInfo;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SaveResultServices - SaveResult: " + e.Message);
            }
        }

    }
}
