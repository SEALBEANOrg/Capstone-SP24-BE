using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class StudentClass
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
}
