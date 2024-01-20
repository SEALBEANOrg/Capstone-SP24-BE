using AutoMapper;
using Repositories;
using Repositories.Models;
using Services.Interfaces;
using Services.ViewModels;

namespace Services.Services
{
    public class SubjectSectionServices : ISubjectSectionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubjectSectionServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> AddSubjectSection(SubjectSectionCreate subjectSectionCreate, Guid currentUser)
        {
            try
            {
                if (subjectSectionCreate.Grade < 0 || subjectSectionCreate.Grade > 12)
                {
                    throw new Exception("Khối không tồn tại");
                }

                if (subjectSectionCreate.Subject < 0 || subjectSectionCreate.Subject > 12)  
                {
                    throw new Exception("Môn học không tồn tại");
                }

                var subjectSection = _mapper.Map<SubjectSection>(subjectSectionCreate);
                subjectSection.CreatedOn = DateTime.Now;
                subjectSection.CreatedBy = currentUser;
                subjectSection.ModifiedOn = DateTime.Now;
                subjectSection.ModifiedBy = currentUser;

                _unitOfWork.SubjectSectionRepo.AddAsync(subjectSection);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SubjectSectionServices - AddSubjectSection: " + e.Message);
            }
        }

        public async Task<IEnumerable<SubjectSectionViewModels>> GetAllBySubjectIdAndGrade(int subjectId, int grade)
        {
            try
            {
                var subjectSections = await _unitOfWork.SubjectSectionRepo.FindListByField(subjectSection => subjectSection.Subject == subjectId && subjectSection.Grade == grade);

                if (subjectSections == null)
                {
                    return null;
                }

                var subjectSectionsViewModels = _mapper.Map<IEnumerable<SubjectSectionViewModels>>(subjectSections);

                return subjectSectionsViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SubjectSectionServices - GetAllBySubjectIdAndGrade: " + e.Message);
            }
        }

        public async Task<SubjectSectionViewModels> GetSectionBySectionId(Guid sectionId)
        {
            try
            {
                var section = await _unitOfWork.SubjectSectionRepo.FindByField(section => section.SectionId == sectionId);

                var sectionViewModels = _mapper.Map<SubjectSectionViewModels>(section);

                return sectionViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SubjectSectionServices - GetSectionBySectionId: " + e.Message);
            }
        }

        public async Task<bool> UpdateSubjectSection(SubjectSectionUpdate subjectSectionUpdate, Guid currentUser)
        {
            try
            {
                var subjectSection = await _unitOfWork.SubjectSectionRepo.FindByField(section => section.SectionId == subjectSectionUpdate.SectionId);

                if (subjectSectionUpdate.Grade < 0 || subjectSectionUpdate.Grade > 12)
                {
                    throw new Exception("Khối không tồn tại");
                }

                if (subjectSectionUpdate.Subject < 0 || subjectSectionUpdate.Subject > 12)
                {
                    throw new Exception("Môn học không tồn tại");
                }

                if (subjectSection == null)
                {
                    throw new Exception("Chương không tồn tại để chỉnh sửa");
                }

                var sectionNameExist = await _unitOfWork.SubjectSectionRepo.FindByField(section => section.Name == subjectSectionUpdate.Name && section.SectionId != subjectSectionUpdate.SectionId &&
                                                                                                    section.Subject == subjectSectionUpdate.Subject && section.Grade == subjectSectionUpdate.Grade);

                if (sectionNameExist != null)
                {
                    throw new Exception("Tên chương đã tồn tại ở môn học ở lớp này");
                }

                subjectSection.ModifiedOn = DateTime.Now;
                subjectSection.ModifiedBy = currentUser;
                subjectSection = _mapper.Map(subjectSectionUpdate, subjectSection);

                _unitOfWork.SubjectSectionRepo.Update(subjectSection);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SubjectSectionServices - UpdateSubjectSection: " + e.Message);
            }
        }

        public async Task<bool> DeleteSubjectSection(Guid sectionId)
        {
            try
            {
                var subjectSection = await _unitOfWork.SubjectSectionRepo.FindByField(section => section.SectionId == sectionId);

                if (subjectSection == null)
                {
                    throw new Exception("Chương không tồn tại để xóa");
                }

                var questions = await _unitOfWork.QuestionRepo.FindListByField(question => question.SectionId == sectionId);

                if (questions != null)
                {
                    throw new Exception("Chương này đang có câu hỏi, không thể xóa");
                }

                _unitOfWork.SubjectSectionRepo.Remove(subjectSection);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SubjectSectionServices - DeleteSubjectSection: " + e.Message);
            }
        }
    }
}
