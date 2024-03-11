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
            CreateMap<User, UserViewModels>().ReverseMap();
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

            #region Student
            CreateMap<StudentCreate, Student>().ReverseMap();
            CreateMap<Student, StudentViewModels>().ReverseMap();
            CreateMap<Student, StudentUpdate>().ReverseMap();
            CreateMap<Student, StudentMoveOut>().ReverseMap();
            CreateMap<Student, StudentInfo>().ReverseMap();
            CreateMap<Student, ResultOfStudent>().ReverseMap();

            #endregion

            #region Question
            CreateMap<QuestionCreate, Question>().ReverseMap();
            CreateMap<Question, QuestionViewModels>().ReverseMap();
            CreateMap<Question, QuestionUpdate>().ReverseMap();
            CreateMap<Question, QuestionSetViewModel>().ReverseMap();
            CreateMap<Question, QuestionJson>().ReverseMap();
            CreateMap<Question, QuestionViewModel>().ReverseMap();
            CreateMap<QuestionSave, Question>().ReverseMap();

            #endregion

            #region QuestionSet
            CreateMap<QuestionSet, QuestionSetViewModels>().ReverseMap();
            CreateMap<QuestionSet, OwnQuestionSet>().ReverseMap();
            CreateMap<QuestionSet, QuestionSetViewModel>().ReverseMap();
            CreateMap<QuestionSet, QuestionReturn>().ReverseMap();
            CreateMap<ImportQuestionSet, QuestionReturn>().ReverseMap();
            CreateMap<QuestionSet, ImportQuestionSet>().ReverseMap();
            CreateMap<QuestionSet, QuestionSetSave>().ReverseMap();
            

            #endregion

            #region SubjectSection
            CreateMap<SubjectSection, SubjectSectionViewModels>().ReverseMap();
            CreateMap<SubjectSection, SubjectSectionCreate>().ReverseMap();
            CreateMap<SubjectSection, SubjectSectionUpdate>().ReverseMap();
            CreateMap<SubjectSection, SubjectSectionViewModel>().ReverseMap();
            CreateMap<SubjectViewModels, Subject>().ReverseMap();
            CreateMap<Subject, SubjectSectionNav>().ReverseMap();

            #endregion

            #region Document
            CreateMap<Document, DocumentViewModels>().ReverseMap();
            CreateMap<Document, DocumentViewModel>().ReverseMap();
            CreateMap<Document, DocumentCreate>().ReverseMap();

            #endregion

            #region Exam
            CreateMap<Exam, ExamViewModels>().ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class != null ? src.Class.Name : null)).ReverseMap();
            CreateMap<Exam, ExamInfo>().ReverseMap();
            CreateMap<Exam, ExamCreate>().ReverseMap();
            CreateMap<Exam, ExamSourceViewModel>().ReverseMap();
            

            #endregion

            #region ExamMark
            CreateMap<ExamMark, ExamMarkViewModels>().ReverseMap();
            CreateMap<ExamMark, ResultOfStudent>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Student.FullName)).ReverseMap();
            

            #endregion

            #region Paper
            CreateMap<Paper,PaperOfExam>().ReverseMap();
            CreateMap<Paper, PaperViewModels>().ReverseMap();
            CreateMap<Paper, PaperContentViewModel>().ReverseMap();
            CreateMap<Paper, PaperToDownload>().ReverseMap();

            #endregion


            #region Transaction
            CreateMap<Transaction, TransactionViewModels>().ReverseMap();

            #endregion

            #region Share
            CreateMap<Share, ShareViewModels>().ReverseMap();
            CreateMap<Share, ShareInMarket>().ForMember(dest => dest.NameOfQuestionSet, opt => opt.MapFrom(src => src.QuestionSet.Name)).ReverseMap();
            CreateMap<Share, ShareViewModel>().ReverseMap();
            CreateMap<ResponseRequest, ResponseRequest>().ReverseMap();
            CreateMap<ShareCreateRequest, ShareCreateRequest>().ReverseMap(); 
            CreateMap<Share, ShareCreateForIndividual>().ReverseMap();
            CreateMap<BuyQuestionSet, BuyQuestionSet>().ReverseMap();
            CreateMap<Share, MySold>().ForMember(dest => dest.NameOfQuestionSet, opt => opt.MapFrom(src => src.QuestionSet.Name)).ReverseMap();

            #endregion
        }

    }
}
