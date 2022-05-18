using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetBadger.Core.CloudSync;
using BudgetBadger.Core.DataAccess;
using BudgetBadger.Core.Logic;
using BudgetBadger.Models;

namespace BudgetBadger.Logic
{
    public class EnvelopeSyncLogic : IEnvelopeSyncLogic
    {
        readonly IDataAccess _localDataAccess;
        readonly IDataAccess _remoteDataAccess;

        public EnvelopeSyncLogic(IDataAccess localDataAccess,
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
                await mergeLogic.MergeEnvelopeGroupsAsync(_remoteDataAccess, _localDataAccess);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }

            try
            {
                await mergeLogic.MergeEnvelopesAsync(_remoteDataAccess, _localDataAccess);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }

            try
            {
                await mergeLogic.MergeBudgetSchedulesAsync(_remoteDataAccess, _localDataAccess);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }

            try
            {
                await mergeLogic.MergeBudgetsAsync(_remoteDataAccess, _localDataAccess);
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
                await mergeLogic.MergeEnvelopeGroupsAsync(_localDataAccess, _remoteDataAccess);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }

            try
            {
                await mergeLogic.MergeEnvelopesAsync(_localDataAccess, _remoteDataAccess);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }

            try
            {
                await mergeLogic.MergeBudgetSchedulesAsync(_localDataAccess, _remoteDataAccess);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }

            try
            {
                await mergeLogic.MergeBudgetsAsync(_localDataAccess, _remoteDataAccess);
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
