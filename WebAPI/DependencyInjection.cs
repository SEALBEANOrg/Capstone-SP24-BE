using Repositories;
using Repositories.Implements;
using Repositories.Interfaces;
using Repositories.Models;
using Services.Interfaces;
using Services.Mapper;
using Services.Services;
using WebAPI.Middlewares;
using WebAPI.Service;

namespace WebAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(ExagenContext));
            services.AddAutoMapper(typeof(MapperConfig).Assembly);
            services.AddHttpContextAccessor();

            #region Repositories
            services.AddScoped<IPaperRepo, PaperRepo>();
            services.AddScoped<IQuestionRepo, QuestionRepo>();
            services.AddScoped<IQuestionTransactionRepo, QuestionTransactionRepo>();
            services.AddScoped<ISchoolRepo, SchoolRepo>();
            services.AddScoped<IShareRepo, ShareRepo>();
            services.AddScoped<IStudentClassRepo, StudentClassRepo>();
            services.AddScoped<IStudentRepo, StudentRepo>();
            services.AddScoped<ISubjectSectionRepo, SubjectSectionRepo>();
            services.AddScoped<ITestResultRepo, TestResultRepo>();
            services.AddScoped<ITestRepo, TestRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            #endregion

            #region Services
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPaperServices, PaperServices>();
            services.AddScoped<IQuestionServices, QuestionServices>();
            services.AddScoped<IQuestionTransactionServices, QuestionTransactionServices>();
            services.AddScoped<ISchoolServices, SchoolServices>();
            services.AddScoped<IShareServices, ShareServices>();
            services.AddScoped<IStudentClassServices, StudentClassServices>();
            services.AddScoped<IStudentServices, StudentServices>();
            services.AddScoped<ISubjectSectionServices, SubjectSectionServices>();
            services.AddScoped<ITestResultServices, TestResultServices>();
            services.AddScoped<ITestServices, TestServices>();
            services.AddScoped<IUserServices, UserServices>();
            #endregion
            

            services.AddSingleton<GlobalExceptionMiddleware>();
            services.AddRouting(opt => opt.LowercaseUrls = true);
            services.AddScoped<IClaimsService, ClaimsService>();

            return services;

        }
    }
}
