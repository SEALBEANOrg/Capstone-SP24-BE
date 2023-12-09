using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class SchoolViewModels
    {
    }

    public class ComboSchool
    {
        public Guid SchoolId { get; set; }
        public string Name { get; set; } = null!;
    }
}
