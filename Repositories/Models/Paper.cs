using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Paper
    {
        public Guid PaperId { get; set; }
        public Guid TestId { get; set; }
        public int PaperCode { get; set; }
        public string PaperContent { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
