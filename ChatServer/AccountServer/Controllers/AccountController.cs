﻿using AccountServer.DB;
using AccountServer.Packets;
using ChatSharedDb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        readonly AppDbContext context;
        readonly SharedDbContext shared;

        public AccountController(AppDbContext context, SharedDbContext shared)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.shared = shared ?? throw new ArgumentNullException(nameof(shared));
        }

        [HttpPost]
        [Route("login")]
        public AccountLoginWebRes LoginAccount([FromBody] AccountLoginWebReq req)
        {
            AccountLoginWebRes res = new AccountLoginWebRes();

            if (string.IsNullOrEmpty(req.AccountId)
                || string.IsNullOrEmpty(req.AccountPassword)
                || string.IsNullOrEmpty(req.IPv4Address))
            {
                res.Res = 0;
                return res;
            }


            AccountDb? account = context.Accounts?
                .AsNoTracking() // read only
                .FirstOrDefault(a => a.AccountName == req.AccountId && a.Password == req.AccountPassword);

            if (account == null)
            {
                res.Res = 2;
            }
            else
            {
                // AutoToken
                DateTime expired = DateTime.UtcNow;
                expired.AddMinutes(5); // 5 minutes

                AuthTokenDb? token = shared.Tokens?
                    .FirstOrDefault(a => a.AccountDbId == account.AccountDbId);

                if (token == null)
                {
                    // make new one
                    token = new AuthTokenDb()
                    {
                        AccountDbId = account.AccountDbId,
                        Token = "",
                        Expired = expired,
                        RecentIpAddress = req.IPv4Address,
                    };
                    shared.Add(token);
                    bool suc = shared.SaveChangesEx();
                    if (suc == false)
                    {
                        // TODO : error
                    }
                }
                else
                {
                    token.Token = "";
                    token.Expired = expired;

                    bool suc = shared.SaveChangesEx();
                    if (suc == false)
                    {
                        // TODO : error
                    }
                }

                res.Res = 1;
                res.AccountDbId = account.AccountDbId;
                res.AuthToken = token.Token;
            }

            return res;
        }

        [HttpPost]
        [Route("register")]
        public CreateAccountWebRes RegisterAccount([FromBody] CreateAccountWebReq req)
        {
            CreateAccountWebRes res = new();

            if (string.IsNullOrEmpty(req.AccountId)
                || string.IsNullOrEmpty(req.AccountPassword))
            {
                res.Res = 0;
                return res;
            }

            AccountDb? account = context.Accounts?
                .AsNoTracking()
                .FirstOrDefault(a => a.AccountName == req.AccountId);

            if (account == null)
            {
                // creatable
                AccountDb newAccount = new()
                {
                    AccountName = req.AccountId,
                    Password = req.AccountPassword,
                };
                context.Accounts?.Add(newAccount);
                bool suc = shared.SaveChangesEx();
                if (suc == false)
                {
                    // TODO : error
                }
                res.Res = 1;
            }
            else
            {
                // duplicate
                res.Res = 2;
            }

            return res;
        }
    }
}
