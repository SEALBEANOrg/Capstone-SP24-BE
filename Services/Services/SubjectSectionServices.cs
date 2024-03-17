using AutoMapper;
using ExagenSharedProject;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Models;
using Services.Interfaces;
using Services.ViewModels;
using System.Xml;

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
                var subjectSection = _mapper.Map<SubjectSection>(subjectSectionCreate);
                subjectSection.SectionNo = (await _unitOfWork.SubjectSectionRepo.FindListByField(section => section.SubjectId == subjectSectionCreate.SubjectId)).Count + 1;
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

        public async Task<IEnumerable<SubjectSectionNav>> GetAllBySubjectIdForNav(Guid? subjectId, Guid currentUserId)
        {
            try
            {
                var subjectSections = await _unitOfWork.SubjectSectionRepo.GetAllAsync();

                if (subjectSections == null)
                {
                    return null;
                }

                if (subjectId != null)
                {
                    subjectSections = subjectSections.Where(section => section.SubjectId == subjectId).ToList();
                }

                var sharedQuestionSetId = (await _unitOfWork.ShareRepo.FindListByField(share => share.UserId == currentUserId)).Select(share => share.QuestionSetId).ToList();
                var subjectSectionsViewModels = _mapper.Map<IEnumerable<SubjectSectionNav>>(subjectSections);
                foreach (var section in subjectSectionsViewModels)
                {
                    var nb = new NumOfEachDifficulty { Difficulty = 0, CHCN = 0, NHD = 0 };
                    var th = new NumOfEachDifficulty { Difficulty = 1, CHCN = 0, NHD = 0 };
                    var vdt = new NumOfEachDifficulty { Difficulty = 2, CHCN = 0, NHD = 0 };
                    var vdc = new NumOfEachDifficulty { Difficulty = 3, CHCN = 0, NHD = 0 };

                    var cn = (await _unitOfWork.QuestionSetRepo.FindListByField(questionSet => questionSet.CreatedBy == currentUserId, includes => includes.Questions));
                    foreach (var item in cn)
                    {
                        nb.CHCN += item.Questions.Count(question => question.Difficulty == 0 && question.SectionId == section.SectionId);
                        th.CHCN += item.Questions.Count(question => question.Difficulty == 1 && question.SectionId == section.SectionId);
                        vdt.CHCN += item.Questions.Count(question => question.Difficulty == 2 && question.SectionId == section.SectionId);
                        vdc.CHCN += item.Questions.Count(question => question.Difficulty == 3 && question.SectionId == section.SectionId);
                    }

                    var nh = (await _unitOfWork.QuestionSetRepo.FindListByField(questionSet => questionSet.CreatedBy != currentUserId && questionSet.Status == 2, includes => includes.Questions));
                    foreach (var item in nh)
                    {
                        nb.NHD += item.Questions.Count(question => question.Difficulty == 0 && question.SectionId == section.SectionId);
                        th.NHD += item.Questions.Count(question => question.Difficulty == 1 && question.SectionId == section.SectionId);
                        vdt.NHD += item.Questions.Count(question => question.Difficulty == 2 && question.SectionId == section.SectionId);
                        vdc.NHD += item.Questions.Count(question => question.Difficulty == 3 && question.SectionId == section.SectionId);
                    }

                    section.NumOfEachDifficulties = new List<NumOfEachDifficulty> { nb, th, vdt, vdc };
                }

                return subjectSectionsViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SubjectSectionServices - GetAllBySubjectIdAndGrade: " + e.Message);
            }
        }
        public async Task<IEnumerable<SubjectSectionViewModels>> GetAllBySubjectId(Guid? subjectId)
        {
            try
            {
                var subjectSections = await _unitOfWork.SubjectSectionRepo.GetAllAsync();

                if (subjectSections == null)
                {
                    return null;
                }

                if (subjectId != null)
                {
                    subjectSections = subjectSections.Where(section => section.SubjectId == subjectId).ToList();
                }

                var subjectSectionsViewModels = _mapper.Map<IEnumerable<SubjectSectionViewModels>>(subjectSections);

                return subjectSectionsViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SubjectSectionServices - GetAllBySubjectIdAndGrade: " + e.Message);
            }
        }

        public async Task<SubjectSectionViewModel> GetSectionBySectionId(Guid sectionId)
        {
            try
            {
                var section = await _unitOfWork.SubjectSectionRepo.FindByField(section => section.SectionId == sectionId, includes => includes.Subject);

                var sectionViewModel = _mapper.Map<SubjectSectionViewModel>(section);
                sectionViewModel.Grade = section.Subject.Grade;
                return sectionViewModel;
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

                if (subjectSection == null)
                {
                    throw new Exception("Chương không tồn tại để chỉnh sửa");
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
