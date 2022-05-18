using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetBadger.Core.CloudSync;
using BudgetBadger.Core.DataAccess;
using BudgetBadger.Core.Logic;
using BudgetBadger.Models;

namespace BudgetBadger.Logic
{
    public class AccountSyncLogic : IAccountSyncLogic
    {
        readonly IDataAccess _localDataAcces;
        readonly IDataAccess _remoteDataAccess;

        public AccountSyncLogic(IDataAccess localDataAccess,
                                IDataAccess remoteDataAccess)
        {
            _localDataAcces = localDataAccess;
            _remoteDataAccess = remoteDataAccess;
        }

        public async Task<Result> PullAsync()
        {
            var result = new Result();
            var mergeLogic = new MergeLogic();
            
            try
            {
                await mergeLogic.MergeAccountsAsync(_remoteDataAccess, _localDataAcces);
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

            try
            {
                var mergeLogic = new MergeLogic();
                await mergeLogic.MergeAccountsAsync(_localDataAcces, _remoteDataAccess);
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
