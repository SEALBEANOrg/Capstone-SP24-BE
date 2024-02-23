using Repositories.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class ExamViewModels
    {
        public Guid ExamId { get; set; }
        public string? Description { get; set; } //name of exam
        public int TestCode { get; set; }
        public string ClassName { get; set; }
        public string HasMark { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class ResultForScan
    {
        public string Base64Image { get; set; }
        public string ResultString { get; set; }
    }

    public class ResultForScanViewModel
    {
        public string Base64Image { get; set; }
    }

    public class ResultToSave
    {
        public string AnswersSelected { get; set; }
        [Required]
        public Guid ExamMarkId { get; set; }
    }

    public class  ClassModel
    {
        public Guid ClassId { get; set; }
        public string Name { get; set; }
    }
}

