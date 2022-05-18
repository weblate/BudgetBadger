using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetBadger.Core.CloudSync;
using BudgetBadger.Core.DataAccess;
using BudgetBadger.Core.Logic;
using BudgetBadger.Models;

namespace BudgetBadger.Logic
{
    public class TransactionSyncLogic : ITransactionSyncLogic
    {
        readonly IDataAccess _localDataAccess;
        readonly IDataAccess _remoteDataAccess;

        public TransactionSyncLogic(IDataAccess localDataAccess,
                                   IDataAccess remoteDataAccess)
        {
            _localDataAccess = localDataAccess;
            _remoteDataAccess = remoteDataAccess;
        }

        public async Task<Result> PullAsync()
        {
            var result = new Result();
            var mergeLogic = new MergeLogic();
            
            try
            {
                await mergeLogic.MergeTransactionsAsync(_remoteDataAccess, _localDataAccess);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }

            result.Success = true;
            return result;
        }

        public async Task<Result> PushAsync()
        {
            var result = new Result();
            var mergeLogic = new MergeLogic();
            
            try
            {
                await mergeLogic.MergeTransactionsAsync(_localDataAccess, _remoteDataAccess);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }

            result.Success = true;
            return result;
        }

        public async Task<Result> SyncAsync()
        {
            var result = new Result();

            result = await PullAsync();

            if (result.Success)
            {
                result = await PushAsync();
            }

            return result;
        }
    }
}
