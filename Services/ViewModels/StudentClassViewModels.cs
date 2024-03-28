using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class StudentClassViewModels
    {
        public Guid ClassId { get; set; }
        public string Name { get; set; } = null!;
        public int? TotalStudent { get; set; }
        public int? Grade { get; set; }
        public int Status { get; set; }
    }

    public class ClassInfo
    {
        public List<StudentInfo>? Students { get; set; }
        public List<ExamViewModels>? ExamViews { get; set; }
    }

    public class StudentClassCreate
    {
        [Required]
        public string Name { get; set; }
        public int? Grade { get; set; }
    }

    public class StudentClassUpdate
    {
        public string Name { get; set; } = null!;
        public int? Grade { get; set; }
        public int Status { get; set; }
    }

    public class InfoClassInExam 
    {
        public int TestCode { get; set; }
        public List<ComboStudent> StudentInExam { get; set; }
        public string DescriptionOfTest { get; set; }

    }

    public class  ComboStudent 
    {
        public Guid ExamMarkId { get; set; }
        public Guid StudentId { get; set; }
        public int No { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
    }

    public class ComboClass
    {
        public Guid ClassId { get; set; }
        public string Name { get; set; }
    }

}
