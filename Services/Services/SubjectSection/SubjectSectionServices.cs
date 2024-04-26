using AutoMapper;
using ExagenSharedProject;
using Repositories;
using Services.Interfaces.SubjectSection;
using Services.ViewModels;

namespace Services.Services.SubjectSection
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
                var subjectSection = _mapper.Map<Repositories.Models.SubjectSection>(subjectSectionCreate);
                //If the section number is not provided, it will be set to max of the current sections + 1
                if (subjectSection.SectionNo == 0)
                {
                    int maxSectionNo = (await _unitOfWork.SubjectSectionRepo.FindListByField(section => section.SubjectId == subjectSectionCreate.SubjectId))
                        .Select(section => section.SectionNo)
                        .DefaultIfEmpty()
                        .Max();
                    subjectSection.SectionNo = maxSectionNo + 1;
                } 
                subjectSection.CreatedOn = DateTime.Now.AddHours(7);
                subjectSection.CreatedBy = currentUser;
                subjectSection.ModifiedOn = DateTime.Now.AddHours(7);
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

                    var cn = await _unitOfWork.QuestionSetRepo.FindListByField(questionSet => questionSet.CreatedBy == currentUserId, includes => includes.Questions);
                    foreach (var item in cn)
                    {
                        nb.CHCN += item.Questions.Count(question => question.Difficulty == OptionSet.Question.Difficulty.NB && question.SectionId == section.SectionId);
                        th.CHCN += item.Questions.Count(question => question.Difficulty == OptionSet.Question.Difficulty.TH && question.SectionId == section.SectionId);
                        vdt.CHCN += item.Questions.Count(question => question.Difficulty == OptionSet.Question.Difficulty.VD && question.SectionId == section.SectionId);
                        vdc.CHCN += item.Questions.Count(question => question.Difficulty == OptionSet.Question.Difficulty.VDC && question.SectionId == section.SectionId);
                    }

                    var nh = await _unitOfWork.QuestionSetRepo.FindListByField(questionSet => questionSet.CreatedBy != currentUserId && questionSet.Status == 2 ||
                                                                    sharedQuestionSetId.Contains(questionSet.QuestionSetId), includes => includes.Questions);
                    foreach (var item in nh)
                    {
                        nb.NHD += item.Questions.Count(question => question.Difficulty == OptionSet.Question.Difficulty.NB && question.SectionId == section.SectionId);
                        th.NHD += item.Questions.Count(question => question.Difficulty == OptionSet.Question.Difficulty.TH && question.SectionId == section.SectionId);
                        vdt.NHD += item.Questions.Count(question => question.Difficulty == OptionSet.Question.Difficulty.VD && question.SectionId == section.SectionId);
                        vdc.NHD += item.Questions.Count(question => question.Difficulty == OptionSet.Question.Difficulty.VDC && question.SectionId == section.SectionId);
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

        public async Task<IEnumerable<SubjectSectionNav>> GetAllByQuestionSet(Guid questionSetId, Guid currentUserId)
        {
            try
            {
                var questionSet = await _unitOfWork.QuestionSetRepo.FindByField(item => item.QuestionSetId == questionSetId);
                var questions = await _unitOfWork.QuestionRepo.FindListByField(question => question.QuestionSetId == questionSetId, includes => includes.Section);
                var subjectSectionIds = questions.Select(question => question.SectionId).Distinct().ToList();
               
                List<SubjectSectionNav> result = new List<SubjectSectionNav>();
                foreach (var sectionId in subjectSectionIds)
                {
                    var nbCount = questions.Count(question => question.SectionId == sectionId && question.Difficulty == OptionSet.Question.Difficulty.NB);
                    var thCount = questions.Count(question => question.SectionId == sectionId && question.Difficulty == OptionSet.Question.Difficulty.TH);
                    var vdtCount = questions.Count(question => question.SectionId == sectionId && question.Difficulty == OptionSet.Question.Difficulty.VD);
                    var vdcCount = questions.Count(question => question.SectionId == sectionId && question.Difficulty == OptionSet.Question.Difficulty.VDC);

                    var nb = new NumOfEachDifficulty { Difficulty = 0, CHCN = 0, NHD = 0 };
                    var th = new NumOfEachDifficulty { Difficulty = 1, CHCN = 0, NHD = 0 };
                    var vdt = new NumOfEachDifficulty { Difficulty = 2, CHCN = 0, NHD = 0 };
                    var vdc = new NumOfEachDifficulty { Difficulty = 3, CHCN = 0, NHD = 0 }; 

                    if (currentUserId == questionSet.CreatedBy)
                    {
                        nb.CHCN = nbCount;
                        th.CHCN = thCount;
                        vdt.CHCN = vdtCount;
                        vdc.CHCN = vdcCount;
                    }
                    else
                    {
                        nb.NHD = nbCount;
                        th.NHD = thCount;
                        vdt.NHD = vdtCount;
                        vdc.NHD = vdcCount;
                    }

                    var sectio = await _unitOfWork.SubjectSectionRepo.FindByField(item => item.SectionId == sectionId);
                    SubjectSectionNav subjectSectionNav = new SubjectSectionNav
                    {
                        SectionId = (Guid)sectionId,
                        SectionNo = sectio.SectionNo,
                        Name = sectio.Name,
                        NumOfEachDifficulties = new List<NumOfEachDifficulty> { nb, th, vdt, vdc }
                    };

                    result.Add(subjectSectionNav);
                }

                return result;
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
                    subjectSections = subjectSections.Where(section => section.SubjectId == subjectId).OrderBy(section => section.SectionNo).ToList();
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

                subjectSection.ModifiedOn = DateTime.Now.AddHours(7);
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
