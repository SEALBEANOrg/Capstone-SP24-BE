using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IMomoServices
    {
        string MomoDeposit(TransactionViaMomo transactionViaMomo);
        bool VerifyMomoCallback(CallbackViaMomo callbackViaMomo);
    }
}
