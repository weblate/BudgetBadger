﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetBadger.Models;

namespace BudgetBadger.Forms.Authentication
{
    public interface IWebAuthenticator
    {
        Task<Result<IDictionary<string, string>>> AuthenticateAsync(Uri requestUri, Uri callbackUri);
    }
}
