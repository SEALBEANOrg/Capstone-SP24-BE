using AutoMapper;
using ExagenSharedProject;
using Repositories;
using Services.Interfaces.Subject;
using Services.ViewModels;

namespace Services.Services.Subject
{
    public class SubjectServices : ISubjectServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubjectServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SubjectViewModels>> GetAll(int? subjectEnum, int? grade)
        {
            try
            {
                var subjects = await _unitOfWork.SubjectRepo.GetAllAsync();

                if (subjects == null)
                {
                    return null;
                }

                if (subjectEnum != null)
                {
                    subjects = subjects.Where(subjects => EnumStatus.Subject[(int)subjectEnum].ToLower().Contains(subjects.Name.ToLower())).ToList();
                }

                if (grade != null)
                {
                    subjects = subjects.Where(subjects => subjects.Grade == grade).ToList();
                }

                return _mapper.Map<IEnumerable<SubjectViewModels>>(subjects);
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SubjectServices - GetAll: " + e.Message);
            }
        }
    }
}
