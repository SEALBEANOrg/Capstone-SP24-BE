using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class ExamMarkViewModels
    {
        public Guid ExamMarkId { get; set; }
        public Guid ExamId { get; set; }
        public Guid StudentId { get; set; }
        public int? StudentNo { get; set; }
        public int? PaperCode { get; set; }
        public string? Answer { get; set; }
        public decimal? Mark { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }
    }

    public class ResultOfStudent
    {
        public Guid ExamMarkId { get; set; }
        public Guid StudentId { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public int? PaperCode { get; set; }
        public decimal? Mark { get; set; }  
    }

}
