using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class PaperExam
    {
        public Guid? PaperId { get; set; }
        public Guid? ExamId { get; set; }
    }
}
