﻿using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.Transaction
{
    public interface IMomoServices
    {
        string MomoDeposit(TransactionPoint transactionViaMomo, Guid currentUserId);
        bool VerifyMomoCallback(CallbackViaMomo callbackViaMomo);
    }
}
