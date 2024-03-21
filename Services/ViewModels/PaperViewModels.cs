using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class PaperViewModels
    {
    }

    public class PaperContentViewModel
    {
        public string UrlS3 { get; set; } = null!;
        public string Answer { get; set; } = null!;
    }

    public class PaperToDownload
    {
        public string PaperCode { get; set; } = null!;
        public string UrlS3 { get; set; } = null!;
    }
}
