using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Repositories;
using Repositories.Models;
using Services.Interfaces.Transaction;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Transaction
{
    public class TransactionServices : ITransactionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMomoServices _momoServices;
        private readonly IOptions<MomoModel> _options;

        public TransactionServices(IUnitOfWork unitOfWork, IMomoServices momoServices, IMapper mapper, IOptions<MomoModel> options)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _momoServices = momoServices;
            _options = options;
        }

        public async Task<string> CreatePaymentAsync(TransactionPoint model, Guid currentUserId)
        {
            try
            {
                var redirectUrl = _momoServices.MomoDeposit(model, currentUserId);
                return redirectUrl;

            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở TransactionServices - CreatePaymentAsync: " + e.Message);
            }
        }

        public async Task<IEnumerable<TransactionViewModels>> GetAll()
        {
            try
            {
                var transactions = await _unitOfWork.TransactionRepo.GetAllAsync();
                if (transactions == null)
                {
                    return null;
                }

                return _mapper.Map<IEnumerable<TransactionViewModels>>(transactions);
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở TransactionServices - GetAll: " + e.Message);
            }
        }

        public async Task<IEnumerable<TransactionViewModels>> GetMyTransactions(Guid currentUser)
        {
            try
            {
                var transactions = await _unitOfWork.TransactionRepo.FindListByField(transaction => transaction.UserId == currentUser);
                if (transactions == null)
                {
                    return null;
                }

                return _mapper.Map<IEnumerable<TransactionViewModels>>(transactions);
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở TransactionServices - GetMyTransactions: " + e.Message);
            }
        }

        public async Task<bool> MomoCallBackRedirect(CallbackViaMomo callbackViaMomo)
        {
            try
            {
                var verify = _momoServices.VerifyMomoCallback(callbackViaMomo);
                if (verify)
                {
                    var isSuccess = callbackViaMomo.ResultCode == 0;
                    if (!isSuccess)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
                else
                {
                    throw new Exception("Giao dịch Momo không hợp lệ");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở TransactionServices - CallbackMomo: " + e.Message);
            }
        }

        public async Task<bool> MomoCallBackIpn(CallbackViaMomo callbackViaMomo)
        {
            try
            {
                Console.WriteLine("call back đã đc gọi");
                var verify = _momoServices.VerifyMomoCallback(callbackViaMomo);
                if (verify)
                {
                    var isSuccess = callbackViaMomo.ResultCode == 0;
                    if (!isSuccess)
                    {
                        throw new Exception("Giao dịch Momo không thành công");
                    }

                    var orderId = callbackViaMomo.OrderId;
                    var orderInfo = callbackViaMomo.OrderInfo;
                    var pointValue = callbackViaMomo.Amount / 100;
                    Guid currentUserId = Guid.Parse(callbackViaMomo.ExtraData);

                    if (callbackViaMomo.Amount == 10000)
                    {
                        pointValue = 100;
                    }
                    else if (callbackViaMomo.Amount == 45000)
                    {
                        pointValue = 500;
                    }
                    else if (callbackViaMomo.Amount == 85000)
                    {
                        pointValue = 1000;
                    }

                    var transaction = new Repositories.Models.Transaction()
                    {
                        UserId = currentUserId,
                        PointValue = (int)pointValue,
                        TransactionCode = callbackViaMomo.TransId.ToString(),
                        CreatedOn = DateTime.Now,
                        Type = 0,

                    };
                    _unitOfWork.TransactionRepo.AddAsync(transaction);

                    var user = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);
                    user.Point += (int)pointValue;
                    _unitOfWork.UserRepo.Update(user);

                    var result = await _unitOfWork.SaveChangesAsync();

                    return result > 0 ? true : false;
                }
                else
                {
                    throw new Exception("Giao dịch Momo không hợp lệ");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở TransactionServices - CallbackMomo: " + e.Message);
            }
        }
    }
}
