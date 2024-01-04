﻿using Chat;
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
                AccountDb findAccound = db.Accounts
                    .FirstOrDefault(a => a.AccountLoginId == req.UserInfo.UserLoginId);

                if (findAccound != null)
                {
                    // assign the found id for AccountDbId
                    AccountDbId = findAccound.AccountDbId;

                    // find user db
                    UserDb user = db.Users.FirstOrDefault(u => u.UserDbId == findAccound.UserDbId);

                    if (user == null) return;

                    // assign the the userinfo
                    UserInfo info = UserInfo.FromUserDb(user, findAccound.AccountLoginId);
                    SetInfo(findAccound, info);

                    CLoginRes res = new CLoginRes()
                    {
                        LoginRes = LoginRes.LoginSuccess,
                        UserInfo = UserInfo,
                    };

                    // TODO : Is anything to do more?

                    Send(res); // send directly OK.
                }
                else
                {
                    // TEST
                    DbProcess.CreateNewAccount(req.UserInfo.UserLoginId, req.UserInfo.UserName, this);
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
            // 전송 성공 여부는 SaveChatText에 포함
            DbProcess.SaveChatText(UserInfo.UserDbId, chat, this);

            // 빠른 응답을 위해 성공 여부 상관 없이 바로 broad cast
            // db 저장과 동시 진행 가능
            

            // broadcast
            RoomManager.Instance.HandleChatText(chat, this);
        }

        public void HandleChatIcon(SSendChatIcon chat)
        {
            // db 처리
            // 전송 성공 여부는 SaveChatIcon 포함
            DbProcess.SaveChatIcon(UserInfo.UserDbId, chat, this);

            // 빠른 응답을 위해 성공 여부 상관 없이 바로 broad cast
            // db 저장과 동시 진행 가능

            // broadcast
            RoomManager.Instance.HandleChatIcon(chat, this);
        }
    }
}
