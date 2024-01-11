﻿using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class TestResult
    {
        public Guid ResultId { get; set; }
        public Guid TestId { get; set; }
        public Guid ClassId { get; set; }
        public string? Marks { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }

        public virtual StudentClass Class { get; set; } = null!;
    }
}
