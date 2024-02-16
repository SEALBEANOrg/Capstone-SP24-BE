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

}

