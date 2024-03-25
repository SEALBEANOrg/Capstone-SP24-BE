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

        Task<IEnumerable<ShareInMarket>> GetQuestionSetInMarket(int? grade, int? subjectEnum, int year, Guid currentUser);
        Task<List<ShareInMarket>> GetBoughtList(Guid currentUser, int? grade, int? subjectEnum, int year);
        Task<List<MySold>> GetSoldList(Guid currentUser, int? grade, int? subjectEnum, int? status, int year);

    }
}
