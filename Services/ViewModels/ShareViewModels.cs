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
        public string NameOfQuestionSet { get; set; }
        public string NameOfSubject { get; set; }
        public int? Price { get; set; } 
        public int Type { get; set; }
        public string? Note { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public string NameOfRequester { get; set; }
    }

    public class ShareInMarket
    {
        public Guid ShareId { get; set; }
        public Guid QuestionSetId { get; set; }
        public string NameOfQuestionSet { get; set; }
        public int? Price { get; set; }
        public int Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public string NameOfSeller { get; set; }
    }

    public class MySold
    {
        public Guid ShareId { get; set; }
        public Guid QuestionSetId { get; set; }
        public string NameOfQuestionSet { get; set; }
        public int? Price { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public int? CountSold { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class ShareViewModel
    {
        public Guid ShareId { get; set; }
        public Guid? QuestionSetId { get; set; }
        public Guid? UserId { get; set; }
        public int Type { get; set; }
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

    public class ShareCreateRequest
    {
        public Guid QuestionSetId { get; set; }
        public int Type { get; set; } // 0 hoặc 2 

    }

    public class BuyQuestionSet
    {
        public Guid ShareId { get; set; }
    }

    public class ShareCreateForIndividual
    {
        public Guid QuestionSetId { get; set; }
        public List<string> Email { get; set; }

    }

    public class NoteReport
    {
        public string Note { get; set; }
    }

}
