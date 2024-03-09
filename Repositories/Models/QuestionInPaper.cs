using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Models
{
    public partial class QuestionInPaper
    {
        public Guid PaperId { get; set; }
        public Guid QuestionId { get; set; }

        public virtual Question Question { get; set; } = null!;
        public virtual Paper Paper { get; set; } = null!;

    }
}
