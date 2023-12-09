using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Province
    {
        public Province()
        {
            Schools = new HashSet<School>();
        }

        public Guid ProvinceId { get; set; }
        public string Name { get; set; } = null!;
        public Guid? AdminId { get; set; }

        public virtual ICollection<School> Schools { get; set; }
    }
}
