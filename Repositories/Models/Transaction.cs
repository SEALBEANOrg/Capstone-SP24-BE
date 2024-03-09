using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Transaction
    {
        public Guid TransactionId { get; set; }
        public Guid UserId { get; set; }
        public int PointValue { get; set; }
        public string? TransactionCode { get; set; }
        public int Type { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
