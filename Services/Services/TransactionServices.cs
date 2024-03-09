using AutoMapper;
using Repositories;
using Services.Interfaces;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class TransactionServices : ITransactionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TransactionServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
    }
}
