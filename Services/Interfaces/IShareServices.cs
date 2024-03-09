﻿using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IShareServices
    {
        Task<IEnumerable<ShareViewModels>> GetRequestToShare(int? status, int? grade, int? subjectEnum, int? type, int year);
        Task<ShareViewModel> GetRequestToShareById(Guid id);
        Task<List<string>> GetUserEmailOfSharedQuestionSet(Guid questionSetId, Guid currentUserId, int? type);
        Task<bool> RequestToShare(ShareCreateRequest shareCreate, Guid currentUser);
        Task<bool> ResponseRequestShare(Guid id, ResponseRequest responseRequest, Guid currentUserId);
        Task<bool> ShareIndividual(ShareCreateForIndividual shareIndividual, Guid currentUser);
    }
}
