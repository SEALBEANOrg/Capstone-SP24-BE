using DucumentProcessing;
using ExagenSharedProject;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class QuestionViewModels
    {
        public Guid QuestionId { get; set; }
        public Guid? SectionId { get; set; }
        public string QuestionContent { get; set; } = null!;
        public int Difficulty { get; set; }
        public int? Grade { get; set; }
        public int? Subject { get; set; }
        public int Status { get; set; }
    }

    public class QuestionViewModel
    {
        public Guid QuestionId { get; set; }
        public string QuestionPart { get; set; } = null!;
        public string Answer1 { get; set; } = null!;
        public string Answer2 { get; set; } = null!;
        public string Answer3 { get; set; } = null!;
        public string Answer4 { get; set; } = null!;
        public string CorrectAnswer { get; set; } = null!;
        public int Difficulty { get; set; }
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = null!;
    }

    public class QuestionCreate
    {
        public Guid? SectionId { get; set; }
        public string Content { get; set; } = null!;
        public int Difficulty { get; set; }
        public string QuestionPart { get; set; } = null!;
        public string Answer1 { get; set; } = null!;
        public string Answer2 { get; set; } = null!;
        public string Answer3 { get; set; } = null!;
        public string Answer4 { get; set; } = null!;
        public string CorrectAnswer { get; set; } = null!;
        public int? Grade { get; set; }
        public int? Subject { get; set; }
    }

    public class QuestionUpdate
    {
        public Guid QuestionId { get; set; }
        public Guid? SectionId { get; set; }
        public string Content { get; set; } = null!;
        public int Difficulty { get; set; }
        public List<string> Answers { get; set; }
        public List<string> CorrectAnswers { get; set; }
        public int? Grade { get; set; }
        public int? Subject { get; set; }
    }

    public class QuestionJson
    {
        public string Content { get; set; } = null!;
        public List<string> Answers { get; set; }
        public List<string> CorrectAnswers { get; set; }
    }

    public class QuestionSetViewModel
    {
        public Guid QuestionSetId { get; set; }
        public string Name { get; set; } = null!;
        public int NumOfQuestion { get; set; }
        public string? Description { get; set; }
        public int Grade { get; set; }
        public string SubjectName { get; set; }
        public int Status { get; set; }
        public int? Price { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public List<QuestionViewModel> Questions { get; set; }
    }

    public class QuestionSetViewModels
    {
        public Guid QuestionSetId { get; set; }
        public string Name { get; set; } = null!;
        public int Type { get; set; }
        public int? Price { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class OwnQuestionSet
    {
        public Guid QuestionSetId { get; set; }
        public string Name { get; set; } = null!;
        public int Status { get; set; }
        public int Grade { get; set; }
        public int SubjectEnum { get; set; }
        public int NumOfQuestion { get; set; }
        public int Type { get; set; }
    }

    public class SharedQuestionSet
    {
        public Guid QuestionSetId { get; set; }
        public string Name { get; set; } = null!;
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class StatusQuestionSet
    {
        public bool IsActive { get; set; }
    }

    public class SetConfig
    {
        public int NB { get; set; }
        public int TH { get; set; }
        public int VDT { get; set; }
        public int VDC { get; set; }
    }

    public class ImportQuestionSet
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? Grade { get; set; }
        public int? Subject { get; set; }
        public IFormFile File { get; set; } = null!;
    }

    public class QuestionReturn
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? Grade { get; set; }
        public Guid SubjectId { get; set; }
        public List<Question> Questions { get; set; }
    }

    public class QuestionSetSave
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Grade { get; set; }
        public Guid SubjectId { get; set; }
        public List<QuestionSave> Questions { get; set; }
    }

    public class QuestionSave
    {
        public string QuestionPart { get; set; } = null!;
        public string Answer1 { get; set; } = null!;
        public string Answer2 { get; set; } = null!;
        public string Answer3 { get; set; } = null!;
        public string Answer4 { get; set; } = null!;
        public string CorrectAnswer { get; set; } = null!;
        public int Difficulty { get; set; }
        public Guid SectionId { get; set; }
    }

}
