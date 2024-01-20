﻿using Services.ViewModels;

namespace Services.Interfaces
{
    public interface ISubjectSectionServices
    {
        Task<bool> AddSubjectSection(SubjectSectionCreate subjectSectionCreate, Guid currentUser);
        Task<bool> DeleteSubjectSection(Guid sectionId);
        Task<IEnumerable<SubjectSectionViewModels>> GetAllBySubjectIdAndGrade(int subjectId, int grade);
        Task<SubjectSectionViewModels> GetSectionBySectionId(Guid sectionId);
        Task<bool> UpdateSubjectSection(SubjectSectionUpdate subjectSectionUpdate, Guid currentUser);
    }
}
