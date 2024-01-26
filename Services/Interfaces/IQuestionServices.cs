﻿using Services.ViewModels;
using static System.Collections.Specialized.BitVector32;
using System.Diagnostics;

namespace Services.Interfaces
{
    public interface IQuestionServices
    {
        Task<bool> AddQuestions(QuestionCreate questionCreate, Guid currentUser);
        Task<bool> DeleteQuestion(Guid questionId, Guid currentUser, int grade);
        Task<IEnumerable<QuestionViewModels>> GetAllMyQuestionByGrade(int grade, Guid currentUserId);
        Task<IEnumerable<QuestionViewModels>> GetAllValidQuestionByGradeForMe(int? subject, int grade, Guid? sectionId, Guid currentUserId);
        Task<QuestionViewModels> GetQuestionByQuestionIdAndGrade(Guid questionId, int grade, Guid currentUserId);
        Task<IEnumerable<QuestionViewModels>> GetQuestionBySectionIdAndGrade(Guid sectionId, int grade);
        Task<IEnumerable<QuestionViewModels>> GetQuestionBySubjectAndSectionAndGrade(int grade, int subject, Guid? sectionId);
        Task<bool> UpdateQuestion(QuestionUpdate questionUpdate, Guid currentUser, int grade);




    }
}
