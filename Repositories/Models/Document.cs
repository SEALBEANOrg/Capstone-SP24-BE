using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Document
    {
        public Guid DocumentId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Data { get; set; } = null!;
        public int Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
