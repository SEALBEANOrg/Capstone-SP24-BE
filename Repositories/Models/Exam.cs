﻿using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Exam
    {
        public Exam()
        {
            ExamMarks = new HashSet<ExamMark>();
        }

        public Guid ExamId { get; set; }
        public Guid ClassId { get; set; }
        public int TestCode { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }

        public virtual StudentClass Class { get; set; } = null!;
        public virtual ICollection<ExamMark> ExamMarks { get; set; }
    }
}
