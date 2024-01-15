using Chat.DB;
using ChatServer.Utils;
using ChatSharedDb;
using Microsoft.EntityFrameworkCore;
using ServerCoreTCP.MessageWrapper;
using System;
using System.Linq;

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

        public void SetLoginned(AccountDb account, UserInfo info)
        {
            UserInfo = info;
            AccountDbId = account.AccountDbId;
            SessionStatus = SessionStatus.LOGINNED;
        }


        #region Authentication

        public void HandleLoginReq(SLoginReq req)
        {
            if (MessageValidation.Validate_SLoginReq(req) == false) return;

            CLoginRes res = new CLoginRes();

            // TODO : decrpyt
            string req_decrypted_token = req.AuthToken;
            

            using (SharedDbContext db = new SharedDbContext())
            {
                AuthTokenDb token = db.Tokens?
                    .AsNoTracking() // read-only
                    .FirstOrDefault(a => a.AccountDbId == req.AccountDbId);

                if (token != null)
                {
                    // TODO : decrpyt
                    string db_decrypted_token = req.AuthToken;

                    // TODO : auth token
                    if (db_decrypted_token == req_decrypted_token
                        && token.RecentIpAddress == req.Ipv4Address
                        && token.Expired > DateTime.UtcNow)
                    {
                        res.LoginRes = LoginRes.LoginSuccess; // success auth.
                    }
                    else if (token.Expired <= DateTime.UtcNow)
                    {
                        res.LoginRes = LoginRes.LoginExpired;
                    }
                    else
                    {
                        res.LoginRes = LoginRes.LoginFailed;
                    }
                }
                else
                {
                    res.LoginRes = LoginRes.LoginInvalid;
                }
            }

            if (res.LoginRes == LoginRes.LoginSuccess)
            {
                using (AppDbContext db = new AppDbContext())
                {
                    AccountDb account = db.Accounts
                        .Include(a => a.User)
                        .FirstOrDefault(a => a.AccountDbId == req.AccountDbId);

                    // check account exist
                    if (account == null)
                    {
                        // create new user data and account
                        UserDb user = new UserDb()
                        {
                            // default name
                            UserName = $"user{req.AccountDbId:00000000}",
                        };
                        db.Users.Add(user);
                        var suc = db.SaveChangesEx();
                        if (suc == false)
                        {
                            res.LoginRes = LoginRes.LoginError;
                            throw new Exception("Save failed");
                            // TODO : error
                        }

                        account = new AccountDb()
                        {
                            AccountDbId = req.AccountDbId,
                            UserDbId = user.UserDbId,
                            User = user
                        };
                        suc = db.SaveChangesEx();
                        if (suc == false)
                        {
                            res.LoginRes = LoginRes.LoginError;
                            throw new Exception("Save failed");
                            // TODO : error
                        }

                        // assign the the userinfo
                        UserInfo info = UserInfo.FromUserDb(user);
                        SetLoginned(account, info);
                    }
                    // account and user exist
                    else
                    {
                        UserInfo info = UserInfo.FromUserDb(account.User);
                        SetLoginned(account, info);
                    }

                    // check again
                    res.LoginRes = LoginRes.LoginSuccess;
                    res.UserInfo.MergeFrom(this.UserInfo);
                }
            }

            Send(res);
        }
        #endregion

        #region User Data
        public void HandleRoomListReq()
        {
            if (SessionStatus != SessionStatus.LOGINNED) return;

            DbProcess.RoomListReq(this);
        }

        public void HandleEditUserNameReq(SEditUserNameReq req)
        {
            if (SessionStatus != SessionStatus.LOGINNED) return;
            if (req.UserInfo.UserDbId != UserInfo.UserDbId) return;
            if (MessageValidation.Validate_SEditUserNameReq(req) == false) return;

            DbProcess.EditUserName(this, req.UserInfo.UserDbId, req.NewUserName);
        }
        #endregion

        #region Chat

        public void HandleChatText(SSendChatText chat)
        {
            if (SessionStatus != SessionStatus.LOGINNED) return;
            if (chat.SenderInfo.UserDbId != UserInfo.UserDbId) return;
            if (MessageValidation.Validate_SSendChatText(chat) == false) return;

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
            if (SessionStatus != SessionStatus.LOGINNED) return;
            if (chat.SenderInfo.UserDbId != UserInfo.UserDbId) return;
            if (MessageValidation.Validate_SSendChatIcon(chat) == false) return;

            // db 처리
            // 전송 성공 여부는 SaveChatIcon 포함
            DbProcess.SaveChatIcon(UserInfo.UserDbId, chat, this);

            // 빠른 응답을 위해 성공 여부 상관 없이 바로 broad cast
            // db 저장과 동시 진행 가능

            // broadcast
            RoomManager.Instance.HandleChatIcon(chat, this);
        }
        #endregion

        #region Rooms

        public void HandleCreateRoomReq(SCreateRoomReq req)
        {
            if (SessionStatus != SessionStatus.LOGINNED) return;
            if (MessageValidation.Validate_SCreateRoomReq(req) == false) return;

            RoomManager.Instance.HandleCreateRoom(this, req);
        }

        public void HandleEnterRoomReq(SEnterRoomReq req)
        {
            if (SessionStatus != SessionStatus.LOGINNED) return;
            if (MessageValidation.Validate_SEnterRoomReq(req) == false) return;

            RoomManager.Instance.HandleEnterRoom(this, req.RoomNumber);
        }

        public void HandleLeaveRoomReq(SLeaveRoomReq req)
        {
            if (SessionStatus != SessionStatus.LOGINNED) return;
            if (req.UserInfo.UserDbId != UserInfo.UserDbId) return;
            if (MessageValidation.Validate_SLeaveRoomReq(req) == false) return;

            RoomManager.Instance.HandleLeaveRoom(this, req.RoomNumber);
        }

        #endregion
    }
}
