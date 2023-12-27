using Chat;
using Chat.DB;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using ServerCoreTCP.MessageWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public enum SessionStatus
    {
        NOT_LOGINNED = 0,
        LOGINNED = 1,
        DISCONNECTED = 2,
    }

    public partial class ClientSession : PacketSession
    {
        public long AccountDbId { get; private set; }
        public SessionStatus SessionStatus { get; private set; } = SessionStatus.NOT_LOGINNED;

        // TEST
        public void SetInfo(AccountDb account, UserInfo info)
        {
            UserInfo = info;
            AccountDbId = account.AccountDbId;
            SessionStatus = SessionStatus.LOGINNED;
        }

        public void HandleLoginReq(SLoginReq req)
        {
            // TODO : check securities

            using (AppDbContext db = new AppDbContext())
            {
                //AccountDb findAccound = db.Accounts.Include(a => a.AccountLoginId == req.UserInfo.UserName).FirstOrDefault();

                AccountDb findAccound = db.Accounts.FirstOrDefault(a => a.AccountLoginId == req.UserInfo.UserName);

                if (findAccound != null)
                {
                    // assign the found id for AccountDbId
                    AccountDbId = findAccound.AccountDbId;
                    // assign the the userinfo
                    UserInfo = new();
                    UserInfo.MergeFrom(req.UserInfo);

                    SessionStatus = SessionStatus.LOGINNED;

                    CLoginRes res = new CLoginRes()
                    {
                        LoginRes = LoginRes.LoginSuccess,
                    };

                    // TODO : Is anything to do more?

                    Send(res); // send directly OK.
                }
                else
                {
                    // TEST
                    DbProcess.CreateNewAccount(req.UserInfo.UserName, this);
                    return;


                    // no acoount found
                    CLoginRes res = new CLoginRes()
                    {
                        LoginRes = LoginRes.LoginFailed,
                    };
                    Send(res); // send directly OK.
                }
            }
        }

        public void HandleRoomListReq()
        {
            DbProcess.RoomListReq(this);
        }

        public void HandleChatText(SSendChatText chat)
        {
            // db 처리
            DbProcess.SaveChatText(UserInfo.UserDbId, chat);

            // 빠른 응답을 위해 성공 여부 상관 없이 바로 broad cast
            // db 저장과 동시 진행 가능

            // 전송 성공 여부는 broadcasting 시에 하기 in RoomManager

            // broadcast
            RoomManager.Instance.HandleChatText(chat, this);
        }

        public void HandleChatIcon(SSendChatIcon chat)
        {
            // db 처리
            DbProcess.SaveChatIcon(UserInfo.UserDbId, chat);

            // 빠른 응답을 위해 성공 여부 상관 없이 바로 broad cast
            // db 저장과 동시 진행 가능

            // 전송 성공 여부는 broadcasting 시에 하기 in RoomManager

            // broadcast
            RoomManager.Instance.HandleChatIcon(chat, this);
        }
    }
}
