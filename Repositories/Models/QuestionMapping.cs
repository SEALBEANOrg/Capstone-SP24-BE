using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class QuestionMapping
    {
        public Guid? QuestionId { get; set; }
        public Guid? QuestionSetId { get; set; }
    }
}
