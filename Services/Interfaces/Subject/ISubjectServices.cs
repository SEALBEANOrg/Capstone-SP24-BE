using Services.ViewModels;

namespace Services.Interfaces.Subject
{
    public interface ISubjectServices
    {
        Task<IEnumerable<SubjectViewModels>> GetAll(int? subjectEnum, int? grade);
    }
}
