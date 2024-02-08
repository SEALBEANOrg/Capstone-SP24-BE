﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class DocumentViewModels
    {

        public Guid DocumentId { get; set; }
        public string Name { get; set; } = null!;
        public string Url { get; set; } = null!;
        public int Type { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class DocumentViewModel
    {

        public Guid DocumentId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Url { get; set; } = null!;
        public int Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class DocumentCreate
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Url { get; set; } = null!;
        public int Type { get; set; }
    }

}
