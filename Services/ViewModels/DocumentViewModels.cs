using Microsoft.AspNetCore.Http;
using System;
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
        public string Data { get; set; } = null!;
        public int Type { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class DocumentViewModel
    {

        public Guid DocumentId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Data { get; set; } = null!;
        public int Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class DocumentCreate
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public IFormFile FileImport { get; set; } = null!;
        public int Type { get; set; } // 0: template answer, 1: template question, 2: document
    }

    public class DetailOfPaper
    {
        public List<Guid> QuestionIds { get; set; }
        public decimal TimeOfTest  { get; set; } 
        public int? PaperCode { get; set; }
        public string NameOfTest { get; set; }
    }
}
