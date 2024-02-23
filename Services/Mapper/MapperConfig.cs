using AutoMapper;
using Repositories.Models;
using Services.ViewModels;

namespace Services.Mapper
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            #region User
            CreateMap<UserInfo, User>().ReverseMap();
            CreateMap<UserSignUp, User>().ReverseMap();
            CreateMap<User, UserViewModels>().ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.School != null ? src.School.Name : null)).ReverseMap();
            CreateMap<UserLogin, User>().ReverseMap();
            CreateMap<Request, User>().ReverseMap();
            //CreateMap<EmployeeWithNote, Employee>().ReverseMap();
            //CreateMap<Employee, ComboEmployee>().ReverseMap();
            
            #endregion

            #region StudentClass
            CreateMap<StudentClassViewModels, StudentClass>().ReverseMap();
            CreateMap<StudentClass, StudentClassCreate>().ReverseMap();
            CreateMap<StudentClass, StudentClassUpdate>().ReverseMap();
            CreateMap<StudentClass, InfoClassInExam>().ReverseMap();
            CreateMap<StudentClass, ClassInfo>().ReverseMap();
            CreateMap<StudentClass, ClassModel>().ReverseMap();


            #endregion

            #region School
            CreateMap<ComboSchool, School>().ReverseMap();
            CreateMap<School, SchoolForCreateViewModel>().ReverseMap();
            CreateMap<School, SchoolViewModels>().ForMember(dest => dest.AdminEmail, opt => opt.MapFrom(src => src.Users.FirstOrDefault(u => u.UserId == src.AdminId).Email));
            CreateMap<School, SchoolForUpdateViewModel>().ReverseMap();
            CreateMap<SchoolList, School>().ReverseMap();

            #endregion

            #region Student
            CreateMap<StudentCreate, Student>().ReverseMap();
            CreateMap<Student, StudentViewModels>().ReverseMap();
            CreateMap<Student, StudentUpdate>().ReverseMap();
            CreateMap<Student, StudentMoveOut>().ReverseMap();
            CreateMap<Student, StudentInfo>().ReverseMap();

            #endregion

            #region Question
            CreateMap<QuestionCreate, Question>().ReverseMap();
            CreateMap<Question, QuestionViewModels>().ReverseMap();
            CreateMap<Question, QuestionUpdate>().ReverseMap();
            CreateMap<Question, QuestionSetViewModel>().ReverseMap();
            CreateMap<Question, QuestionJson>().ReverseMap();
            CreateMap<Question, QuestionViewModel>().ReverseMap();

            #endregion

            #region SubjectSection
            CreateMap<SubjectSection, SubjectSectionViewModels>().ReverseMap();
            CreateMap<SubjectSection, SubjectSectionCreate>().ReverseMap();
            CreateMap<SubjectSection, SubjectSectionUpdate>().ReverseMap();
            CreateMap<SubjectSection, SubjectSectionViewModel>().ReverseMap();

            #endregion

            #region Document
            CreateMap<Document, DocumentViewModels>().ReverseMap();
            CreateMap<Document, DocumentViewModel>().ReverseMap();
            CreateMap<Document, DocumentCreate>().ReverseMap();

            #endregion

            #region Exam
            CreateMap<Exam, ExamViewModels>().ReverseMap();

            #endregion



        }

    }
}
