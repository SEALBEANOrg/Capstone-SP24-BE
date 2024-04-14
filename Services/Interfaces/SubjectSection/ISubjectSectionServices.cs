using Services.ViewModels;

namespace Services.Interfaces.SubjectSection
{
    public interface ISubjectSectionServices
    {
        Task<bool> AddSubjectSection(SubjectSectionCreate subjectSectionCreate, Guid currentUser);

        Task<IEnumerable<SubjectSectionViewModels>> GetAllBySubjectId(Guid? subjectId);
        Task<IEnumerable<SubjectSectionNav>> GetAllBySubjectIdForNav(Guid? subjectId, Guid currentUserId);
        Task<SubjectSectionViewModel> GetSectionBySectionId(Guid sectionId);
        Task<IEnumerable<SubjectSectionNav>> GetAllByQuestionSet(Guid questionSetId, Guid currentUserId);

        Task<bool> UpdateSubjectSection(SubjectSectionUpdate subjectSectionUpdate, Guid currentUser);

        Task<bool> DeleteSubjectSection(Guid sectionId);
    }
}
