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
            CreateMap<UserViewModels, User>().ReverseMap();
            CreateMap<UserLogin, User>().ReverseMap();
            CreateMap<Request, User>().ReverseMap();
            //CreateMap<EmployeeWithNote, Employee>().ReverseMap();
            //CreateMap<Employee, ComboEmployee>().ReverseMap();

            #endregion

            #region StudentClass
            CreateMap<StudentClassViewModels, StudentClass>().ReverseMap();
            CreateMap<StudentClass, StudentClassCreate>().ReverseMap();
            CreateMap<StudentClass, StudentClassUpdate>().ReverseMap();

            #endregion

            #region School
            CreateMap<ComboSchool, School>().ReverseMap();

            #endregion

            #region Student
            CreateMap<StudentCreate, Student>().ReverseMap();
            CreateMap<Student, StudentViewModels>().ReverseMap();
            CreateMap<Student, StudentUpdate>().ReverseMap();
            CreateMap<Student, StudentMoveOut>().ReverseMap();

            #endregion

            #region Question
            CreateMap<QuestionRequest, Question>().ReverseMap();
            CreateMap<Question, QuestionViewModels>().ReverseMap();
            
            #endregion

            #region SubjectSection

            #endregion

        }

    }
}
