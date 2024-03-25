using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.Transaction
{
    public interface ITransactionServices
    {
        Task<string> CreatePaymentAsync(TransactionPoint model, Guid currentUserId);

        Task<IEnumerable<TransactionViewModels>> GetAll();
        Task<IEnumerable<TransactionViewModels>> GetMyTransactions(Guid currentUser);

        Task<bool> MomoCallBackRedirect(CallbackViaMomo transaction);
    }
}
