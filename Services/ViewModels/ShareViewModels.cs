using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class ShareViewModels
    {
        public Guid ShareId { get; set; }
        public Guid QuestionSetId { get; set; }
        public int Type { get; set; }
        public string? Note { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class ShareViewModel
    {
        public Guid ShareId { get; set; }
        public Guid? QuestionSetId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SchoolId { get; set; }
        public int Type { get; set; }
        public int PermissionType { get; set; }
        public string? Note { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }
        public int? ShareLevel { get; set; }
    }

    public class ResponseRequest
    {
        public bool IsAccept { get; set; }
        public string? Note { get; set; }

    }

}
