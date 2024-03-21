﻿using Repositories;
using Repositories.Implements;
using Repositories.Interfaces;
using Repositories.Models;
using Services.Interfaces;
using Services.Mapper;
using Services.Services;
using Services.ViewModels;
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
            services.AddScoped<IDocumentRepo, DocumentRepo>();
            services.AddScoped<IPaperRepo, PaperRepo>();
            services.AddScoped<IQuestionRepo, QuestionRepo>();
            services.AddScoped<IQuestionInPaperRepo, QuestionInPaperRepo>();
            services.AddScoped<IShareRepo, ShareRepo>();
            services.AddScoped<IStudentClassRepo, StudentClassRepo>();
            services.AddScoped<IStudentRepo, StudentRepo>();
            services.AddScoped<ISubjectSectionRepo, SubjectSectionRepo>();
            services.AddScoped<IExamRepo, ExamRepo>();
            services.AddScoped<IExamMarkRepo, ExamMarkRepo>();
            services.AddScoped<IPaperSetRepo, PaperSetRepo>();
            services.AddScoped<IQuestionInExamRepo, QuestionInExamRepo>();
            services.AddScoped<IQuestionSetRepo, QuestionSetRepo>();
            services.AddScoped<ISubjectRepo, SubjectRepo>();
            services.AddScoped<ITransactionRepo, TransactionRepo>();
            services.AddScoped<ISectionPaperSetConfigRepo, SectionPaperSetConfigRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            #endregion

            #region Services
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDocumentServices, DocumentServices>();
            services.AddScoped<IPaperServices, PaperServices>();
            services.AddScoped<IQuestionServices, QuestionServices>();
            services.AddScoped<IPaperExamServices, PaperExamServices>();
            services.AddScoped<IShareServices, ShareServices>();
            services.AddScoped<IStudentClassServices, StudentClassServices>();
            services.AddScoped<IStudentServices, StudentServices>();
            services.AddScoped<ISubjectSectionServices, SubjectSectionServices>();
            services.AddScoped<IExamServices, ExamServices>();
            services.AddScoped<IExamMarkServices, ExamMarkServices>();
            services.AddScoped<IQuestionSetServices, QuestionSetServices>();
            services.AddScoped<ISubjectServices, SubjectServices>();
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<ITransactionServices, TransactionServices>();
            services.AddScoped<IMomoServices, MomoServices>();
            services.AddScoped<IS3Services, S3Services>();
            #endregion
           
            services.AddSingleton<GlobalExceptionMiddleware>();
            services.AddRouting(opt => opt.LowercaseUrls = true);
            services.AddScoped<IClaimsService, ClaimsService>();

            return services;

        }
    }
}
