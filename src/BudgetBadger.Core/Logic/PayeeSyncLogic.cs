using System;
using System.Threading.Tasks;
using BudgetBadger.Core.DataAccess;
using BudgetBadger.Models;

namespace BudgetBadger.Core.Logic
{
    public class PayeeSyncLogic : IPayeeSyncLogic
    {
        readonly IDataAccess _localDataAccess;
        readonly IDataAccess _remoteDataAccess;

        public PayeeSyncLogic(IDataAccess localDataAccess,
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
                await mergeLogic.MergePayeesAsync(_remoteDataAccess, _localDataAccess);
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
                await mergeLogic.MergePayeesAsync(_localDataAccess, _remoteDataAccess);
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
