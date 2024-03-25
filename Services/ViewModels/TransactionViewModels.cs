using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class TransactionViewModels
    {
        public Guid TransactionId { get; set; }
        public Guid UserId { get; set; }
        public int PointValue { get; set; }
        public string? TransactionCode { get; set; }
        public int Type { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class TransactionPoint
    {
        public int PointValue { get; set; }
    }

}
