using AccountServer.DB;
using AccountServer.Packets;
using AccountServer.Security;
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

            if (req.Validate() == false)
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
                DateTime expired = DateTime.UtcNow.AddMinutes(3); // expired after 3 minutes

                AuthTokenDb? token = shared.Tokens?
                    .FirstOrDefault(a => a.AccountDbId == account.AccountDbId);

                string dbToken = AuthToken.GenerateAuthToken(account.AccountDbId, account.AccountName!, req.IPv4Address!);

                if (token == null)
                {
                    // make new one
                    token = new AuthTokenDb()
                    {
                        AccountDbId = account.AccountDbId,
                        Token = dbToken,
                        Expired = expired,
                        RecentIpAddress = req.IPv4Address,
                    };
                    shared.Add(token);
                    bool suc = shared.SaveChangesEx();
                    if (suc == false)
                    {
                        // TODO : error
                        throw new Exception("Save Failed");
                    }
                }
                else
                {
                    token.Token = dbToken;
                    token.Expired = expired;

                    bool suc = shared.SaveChangesEx();
                    if (suc == false)
                    {
                        // TODO : error
                        throw new Exception("Save Failed");
                    }
                }

                // ip
                // read-only
                ChatServerIpDb? server = context.ChatServers?
                    .AsNoTracking()
                    .FirstOrDefault(c => c.IsOnline == true && c.Status == 1);
                if (server == null)
                {
                    res.Res = 3; // can not find chat-server to connect.
                }
                else
                {
                    res.ServerIp = server.ChatServerIp;
                    res.ServerPort = server.ChatServerPort;
                    res.ServerName = server.ChatServerName;

                    res.Res = 1;
                    res.AccountDbId = account.AccountDbId;
                    res.AuthToken = AuthToken.EncryptAuthToken(dbToken);
                }
            }

            if (res.Validate() == false) throw new Exception("Invalid response");

            return res;
        }

        [HttpPost]
        [Route("register")]
        public CreateAccountWebRes RegisterAccount([FromBody] CreateAccountWebReq req)
        {
            CreateAccountWebRes res = new();

            if (req.Validate() == false)
            {
                res.Res = 0;
                return res;
            }

            AccountDb? account = context.Accounts?
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
                bool suc = context.SaveChangesEx();
                if (suc == false)
                {
                    // TODO : error
                    throw new Exception("Save Failed");
                }
                res.Res = 1;
            }
            else
            {
                // duplicate
                res.Res = 2;
            }

            if (res.Validate() == false) throw new Exception("Invalid response");

            return res;
        }
    }
}
