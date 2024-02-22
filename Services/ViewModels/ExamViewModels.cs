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
        public string? Description { get; set; } //name of exam
        public int Subject { get; set; }
        public ClassModel Class { get; set; }
        public Guid ExamId { get; set; }
        public int TestCode { get; set; }
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

