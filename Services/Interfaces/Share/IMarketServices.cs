using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.Share
{
    public interface IMarketServices
    {
        Task<bool> BuyQuestionSet(BuyQuestionSet buyQuestionSet, Guid currentUser);

        Task<IEnumerable<ShareInMarket>> GetQuestionSetInMarket(int? grade, int? subjectEnum, string studyYear, Guid currentUser);
        Task<List<ShareInMarket>> GetBoughtList(Guid currentUser, int? grade, int? subjectEnum, string studyYear);
        Task<List<MySold>> GetSoldList(Guid currentUser, int? grade, int? subjectEnum, int? status, string studyYear);

    }
}
