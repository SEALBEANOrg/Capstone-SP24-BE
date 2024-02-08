using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class SubjectSectionViewModels
    {
        public Guid SectionId { get; set; }
        public int Grade { get; set; }
        public string Name { get; set; } = null!;
        public int Subject { get; set; }
    }

    public class SubjectSectionViewModel
    {
        public Guid SectionId { get; set; }
        public string? Description { get; set; }
        public int Grade { get; set; }
        public string Name { get; set; } = null!;
        public int Subject { get; set; }
    }

    public class SubjectSectionCreate
    {
        public string? Description { get; set; }
        public int Grade { get; set; }
        public string Name { get; set; } = null!;
        public int Subject { get; set; }
    }

    public class SubjectSectionUpdate
    {
        public Guid SectionId { get; set; }
        public string? Description { get; set; }
        public int Grade { get; set; }
        public string Name { get; set; } = null!;
        public int Subject { get; set; }
    }

}
