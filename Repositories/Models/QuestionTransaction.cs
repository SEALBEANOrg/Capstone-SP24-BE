using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class QuestionTransaction
    {
        public Guid QuestionTransactionId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
