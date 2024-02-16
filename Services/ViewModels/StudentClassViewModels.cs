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
        public Guid? SchoolId { get; set; }
        public string Name { get; set; } = null!;
        public int? TotalStudent { get; set; }
        public int? Grade { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }
    }

    public class StudentClassCreate
    {
        public Guid? SchoolId { get; set; }
        [Required]
        public string Name { get; set; }
        public int? Grade { get; set; }
    }

    public class StudentClassUpdate
    {
        [Required]
        public Guid ClassId { get; set; } // không update id
        public Guid? SchoolId { get; set; }
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
        public Guid StudentId { get; set; }
        public int No { get; set; }
        public string Name { get; set; }
        public decimal? Mark { get; set;}
    }
}
