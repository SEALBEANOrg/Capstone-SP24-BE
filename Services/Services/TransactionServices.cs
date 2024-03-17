using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Repositories;
using Services.Interfaces;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
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



        //create payment
        public async Task<string> CreatePaymentAsync(TransactionViaMomo model)
        {
            try
            {
                var redirectUrl = _momoServices.MomoDeposit(model);
                return redirectUrl;

            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở TransactionServices - CreatePaymentAsync: " + e.Message);
            }
        }

        //public TransactionResponse CallbackMomo(CallbackViaMomo callbackViaMomo, Guid currentUserId)
        //{
        //    try
        //    {
        //        var verify = _momoServices.VerifyMomoCallback(callbackViaMomo);
        //        if (verify)
        //        {
        //            var isSuccess = callbackViaMomo.ResultCode == "0";
        //            var orderId = callbackViaMomo.orderId;
        //            var orderInfo = callbackViaMomo.orderInfo;
        //            var pointValue = callbackViaMomo.amount;
        //            var userId = callbackViaMomo.partnerCode;
        //            var transaction = new Transaction()
        //            {
        //                Id = Guid.NewGuid(),
        //                UserId = Guid.Parse(userId),
        //                PointValue = pointValue,
        //                OrderId = orderId,
        //                OrderInfo = orderInfo,
        //                CreatedAt = DateTime.Now
        //            };
        //            _unitOfWork.TransactionRepo.Add(transaction);
        //            _unitOfWork.Commit();
        //        }
        //        else
        //        {
        //            throw new Exception("Giao dịch Momo không hợp lệ");
        //        }
        //        return new TransactionResponse()
        //        {
        //            PointValue = pointValue,
        //            OrderId = orderId,
        //            OrderInfo = orderInfo
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("Lỗi ở TransactionServices - CallbackMomo: " + e.Message);
        //    }
        //}

    }
}
