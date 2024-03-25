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
        public string Name { get; set; } = null!;
        public int SectionNo { get; set; }
    }

    public class SubjectSectionNav
    {
        public Guid SectionId { get; set; }
        public string Name { get; set; } = null!;
        public int SectionNo { get; set; }
        public List<NumOfEachDifficulty> NumOfEachDifficulties { get; set; } = null!;
    }

    public class NumOfEachDifficulty
    {
        public int Difficulty { get; set; }
        public int CHCN { get; set; }
        public int NHD { get; set; }
    }

    public class SubjectSectionViewModel
    {
        public Guid SectionId { get; set; }
        public int SectionNo { get; set; }
        public string? Description { get; set; }
        public int Grade { get; set; }
        public string Name { get; set; } = null!;
    }

    public class SubjectSectionCreate
    {
        public string? Description { get; set; }
        public string Name { get; set; } = null!;
        public Guid SubjectId { get; set; }
    }

    public class SubjectSectionUpdate
    {
        public Guid SectionId { get; set; }
        public string? Description { get; set; }
        public int SectionNo { get; set; }
        public string Name { get; set; } = null!;
        public Guid SubjectId { get; set; }
    }

    public class SubjectViewModels
    {
        public Guid SubjectId { get; set; }
        public string Name { get; set; } = null!;
        public int Grade { get; set; }
    }

}
