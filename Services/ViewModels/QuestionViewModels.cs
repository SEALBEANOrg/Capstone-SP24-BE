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
        public Guid? SchoolId { get; set; }
        public string QuestionContent { get; set; } = null!;
        public int Difficulty { get; set; }
        public int? Grade { get; set; }
        public int? Subject { get; set; }
        public int Status { get; set; }
    }

    public class QuestionViewModel
    {
        public Guid QuestionId { get; set; }
        public Guid? SectionId { get; set; }
        public Guid? SchoolId { get; set; }
        public string QuestionContent { get; set; } = null!;
        public int Difficulty { get; set; }
        public int Status { get; set; }
    }

    public class ParagraphProcessing
    {
        public int Type { get; set; } //0: text, 1: image
        public string Content { get; set; } = null!;
    }

    public class QuestionCreate
    {
        public Guid? SectionId { get; set; }
        public Guid? SchoolId { get; set; }
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
        public Guid? SchoolId { get; set; }
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
        public string? Description { get; set; }
        public int Type { get; set; }
        public int Subject { get; set; }
        public int Grade { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public List<QuestionViewModel> Questions { get; set; }
    }

    public class StatusQuestionSet
    {
        public bool IsActive { get; set; }
    }
}
