﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudgetBadger.Models;

namespace BudgetBadger.Core.Logic
{
    public interface IAccountLogic
    {
        Task<Result> ValidateAccountAsync(Account account);
        Task<Result<Account>> SaveAccountAsync(Account account);
        Task<Result> DeleteAccountAsync(Guid id);
        Task<Result<Account>> GetAccountAsync(Guid id);
        Task<Result<IReadOnlyList<Account>>> GetAccountsAsync();
        Task<Result<IReadOnlyList<Account>>> GetAccountsForSelectionAsync();

        Task<Result<IEnumerable<AccountType>>> GetAccountTypesAsync();

        IEnumerable<Account> SearchAccounts(IEnumerable<Account> accounts, string searchText);
        ILookup<string, Account> GroupAccounts(IEnumerable<Account> accounts);
    }
}
