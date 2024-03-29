﻿using ChatServer.Utils;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using ServerCoreTCP.CLogger;
using ServerCoreTCP.Job;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chat.DB
{
    public partial class DbProcess : JobSerializer
    {
        public static DbProcess Instance { get; } = new DbProcess();

        public static void Login(ClientSession session, long loginDbId)
        {
            Instance.Add(() =>
            {
                LoginRes loginRes = LoginRes.LoginSuccess;
                using (AppDbContext db = new AppDbContext())
                {
                    AccountDb account = db.Accounts
                        .Include(a => a.User)
                        .FirstOrDefault(a => a.LoginDbId == loginDbId);

                    // check account exist
                    if (account == null)
                    {
                        // create new account and user
                        UserDb user = new UserDb()
                        {
                            UserName = $"user{loginDbId:00000000}",
                        };
                        db.Users.Add(user);
                        var suc = db.SaveChangesEx();
                        if (suc == false)
                        {
                            loginRes = LoginRes.LoginError;
                            throw new Exception("Save failed");
                        }

                        account = new AccountDb()
                        {
                            LoginDbId = loginDbId,
                            UserDbId = user.UserDbId,
                            User = user
                        };
                        db.Accounts.Add(account);
                        suc = db.SaveChangesEx();
                        if (suc == false)
                        {
                            loginRes = LoginRes.LoginError;
                            throw new Exception("Save failed");
                            // TODO : error
                        }

                        // assign the the userinfo
                        UserInfo info = UserInfo.FromUserDb(user);
                        session.SetLoginned(account, info);
                    }
                    // account and user exist
                    else
                    {
                        UserInfo info = UserInfo.FromUserDb(account.User);
                        session.SetLoginned(account, info);
                    }

                    // check again
                    CLoginRes res = new CLoginRes();
                    if (loginRes == LoginRes.LoginSuccess)
                    {
                        res.LoginRes = LoginRes.LoginSuccess;
                        res.UserInfo = new UserInfo();
                        res.UserInfo.MergeFrom(session.UserInfo);
                    }
                    else
                    {
                        res.LoginRes = loginRes;
                    }

                    session.Send(res);
                }
            });
        }

        public static void EditUserName(ClientSession session, ulong userDbId, string newUserName)
        {
            // 바로 접근
            using (AppDbContext db  = new AppDbContext())
            {
                UserDb updatedUser = new()
                {
                    UserDbId = userDbId,
                    UserName = newUserName,
                };

                db.Entry(updatedUser).Property(nameof(UserDb.UserName)).IsModified = true;
                bool saved = db.SaveChangesEx();
                if (saved == true)
                {
                    // success to edit

                    // edit username in session too
                    session.UserInfo.UserName = newUserName;


                    CEditUserNameRes res = new()
                    {
                        Res = EditUserNameRes.EditOk,
                        NewUserName = newUserName,
                    };
                    session.Send(res);
                }
                else
                {
                    // TODO : error
                    CEditUserNameRes res = new()
                    {
                        Res = EditUserNameRes.EditFailed,
                        NewUserName = newUserName,
                    };
                    session.Send(res);
                }
            }
        }


        public static void CreateRoom(uint roomNumber, string roomName, ClientSession session)
        {
            Instance.Add(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    // find room
                    ChatRoomDb chatRoom = db.ChatRooms.FirstOrDefault(c => c.ChatRoomNumber == roomNumber);

                    // found the room with that req.RoomId => send failed msg
                    if (chatRoom != null)
                    {
                        CCreateRoomRes res = new CCreateRoomRes()
                        {
                            Res = CreateRoomRes.CreateRoomDuplicatedRoomId,
                            RoomNumber = roomNumber,
                            RoomInfo = null,
                        };
                        session.Send(res);
                    }
                    // not found => create new room
                    else
                    {
                        // create new room
                        ChatRoomDb newChatRoom = new ChatRoomDb()
                        {
                            ChatRoomName = roomName,
                            ChatRoomNumber = roomNumber,
                        };

                        db.ChatRooms.Add(newChatRoom);
                        bool saved = db.SaveChangesEx();

                        // if the room is created successfully
                        if (saved == true)
                        {
                            CCreateRoomRes res = new()
                            {
                                Res = CreateRoomRes.CreateRoomOk,
                                RoomNumber = roomNumber,
                                RoomInfo = RoomInfo.FromChatRoomDb(newChatRoom),
                            };
                            session.Send(res);
                            // let the user enter the room
                            EnterRoom_(newChatRoom, session, db);
                        }
                        // if failed, send error
                        else
                        {
                            // TODO : error

                            CCreateRoomRes res = new()
                            {
                                Res = CreateRoomRes.CreateRoomError,
                                RoomNumber = roomNumber,
                                RoomInfo = null,
                            };
                            session.Send(res);
                        }
                    }
                }
            });
        }

        public static void EnterRoom(uint roomNumber, ClientSession session)
        {
            if (session == null) return;
            // wrong req
            if (session.SessionStatus != SessionStatus.LOGINNED) return;

            Instance.Add(() =>
            {
                // check the room[number = roomNumber] exists
                using (AppDbContext db = new AppDbContext())
                {
                    // find room
                    ChatRoomDb chatRoom = db.ChatRooms
                        .Include(c => c.Users) // include users
                        .FirstOrDefault(c => c.ChatRoomNumber == roomNumber);

                    // if there is no such room with roomNumber
                    if (chatRoom == null)
                    {
                        // send fail msg
                        CEnterRoomRes res = new CEnterRoomRes()
                        {
                            Res = EnterRoomRes.EnterRoomNoSuchRoom,
                            RoomNumber = roomNumber,
                            RoomInfo = null,
                        };
                        session.Send(res);
                    }
                    else
                    {
                        // check duplicate already in
                        UserDb userDb = db.Users.FirstOrDefault(u => u.UserDbId == session.UserInfo.UserDbId);
                        if (userDb == null) return;

                        // TODO : 현재 객체 비교 (간단하게 id 비교 가능?)
                        if (userDb.Rooms.Contains(chatRoom) == true)
                        {
                            CEnterRoomRes res = new()
                            {
                                Res = EnterRoomRes.EnterRoomAlreadyIn,
                                RoomNumber = roomNumber,
                                RoomInfo = null,
                            };
                            session.Send(res);
                        }
                        else
                        {
                            EnterRoom_(chatRoom, session, db, userDb);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// It does not check the room is valid.
        /// </summary>
        /// <param name="roomNumber"></param>
        /// <param name="session"></param>
        static void EnterRoom_(ChatRoomDb roomDb, ClientSession session, AppDbContext db, UserDb userDb = null)
        {
            // include rooms (load)
            userDb ??= db.Users
                .Include(u => u.Rooms)
                .FirstOrDefault(u => u.UserDbId == session.UserInfo.UserDbId);

            if (userDb == null) return;

            // user
            userDb.Rooms.Add(roomDb);

            // chatroom
            roomDb.Users.Add(userDb); // Note: the roomDb is already created in Db

            // save changes
            bool saved = db.SaveChangesEx();

            // send enter res
            // if saving is successful
            if (saved == true)
            {
                RoomInfo roominfo = RoomInfo.FromChatRoomDb(roomDb, containUser: true);
                CEnterRoomRes res = new()
                {
                    Res = EnterRoomRes.EnterRoomOk,
                    RoomNumber = roomDb.ChatRoomNumber,
                    RoomInfo = roominfo,
                };
                session.Send(res);

                // 메모리 캐싱 추가
                RoomManager.Instance.EnterRoom(roominfo, session);
            }
            else
            {
                // TODO : error

                CEnterRoomRes res = new()
                {
                    Res = EnterRoomRes.EnterRoomError,
                    RoomNumber = roomDb.ChatRoomNumber
                };
                session.Send(res);
            }
        }

        public static void RoomListReq(ClientSession session)
        {
            if (session == null) return;
            // wrong req
            if (session.SessionStatus != SessionStatus.LOGINNED) return;

            Instance.Add(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    // Rooms가 lazy loading이므로 include로 직접 로드 해주어야 함
                    UserDb userDb = db.Users
                        .Include(u => u.Rooms)
                        .FirstOrDefault(u => u.UserDbId == session.UserInfo.UserDbId);

                    // error : no user data
                    if (userDb == null) return;

                    CRoomListRes res = new();
                    List<RoomInfo> rooms = new();
                    foreach (var room in userDb.Rooms)
                    {
                        // 방 추가
                        RoomInfo info = new()
                        {
                            RoomDbId = room.ChatRoomDbId,
                            RoomName = room.ChatRoomName,
                            RoomNumber = room.ChatRoomNumber,
                        };
                        // CHECK : info.Users가 null 값일까요?

                        // TODO : RoomInfo의 Users 간소화
                        foreach (var user in room.Users)
                        {
                            info.Users.Add(UserInfo.FromUserDb(user));
                        }
                        res.Rooms.Add(info);
                        rooms.Add(info);
                    }
                    res.LoadTime = Timestamp.FromDateTime(DateTime.UtcNow);
                    session.Send(res);

                    // room 정보 메모리 로드 (cache)
                    RoomManager.Instance.EnterRooms(rooms, session);
                }
            });
        }

        public static void LeaveRoom(ulong userDbId, ulong roomNumber)
        {
            Instance.Add(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    // find the userinfo
                    UserDb userDb = db.Users
                        .Include(u => u.Rooms)
                        .FirstOrDefault(u => u.UserDbId == userDbId);
                    if (userDb == null) return;

                    ChatRoomDb roomDb = db.ChatRooms
                        .Include(r => r.Users)
                        .FirstOrDefault(r => r.ChatRoomNumber == roomNumber);
                    if (roomDb == null) return;

                    userDb.Rooms.Remove(roomDb);
                    roomDb.Users.Remove(userDb);

                    bool saved = db.SaveChangesEx();
                    if (saved == true)
                    {
                        // Note: There is no res message.
                        ;
                    }
                    else
                    {
                        // TODO : error
                        throw new System.Exception("DbUpdate or DbUpdateConcurrent");
                    }
                }
            });
        }

        public static void SaveChatText(ulong userDbId, SSendChatText chat, ClientSession session)
        {
            Instance.Add(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    UserDb userDb = db.Users.FirstOrDefault(u => u.UserDbId == userDbId);
                    if (userDb == null) return;

                    ChatRoomDb roomDb = db.ChatRooms.FirstOrDefault(r => r.ChatRoomNumber == chat.RoomNumber);
                    if (roomDb == null) return;

                    ChatDb newChat = new()
                    {
                        ChatType = (ushort)chat.Chat.ChatBase.ChatType,
                        IconId = 0,
                        Message = chat.Chat.Msg,
                        RoomDbId = chat.RoomNumber,
                        Room = roomDb,
                        Sender = userDb,
                        SenderDbId = userDbId,
                        SendTime = chat.Chat.ChatBase.Timestamp.ToDateTime(),
                    };

                    db.Chats.Add(newChat);
                    userDb.SentChats.Add(newChat);
                    roomDb.Chats.Add(newChat);

                    bool saved = db.SaveChangesEx();
                    if (saved == true)
                    {
                        CSendChat res = new()
                        {
                            Error = SendChatError.Success,
                            RoomNumber = chat.RoomNumber,
                            ChatId = chat.ChatId,
                        };
                        session.Send(res);
                    }
                    else
                    {
                        // TODO : error
                        CSendChat res = new()
                        {
                            Error = SendChatError.Other,
                            RoomNumber = chat.RoomNumber,
                            ChatId = chat.ChatId,
                        };
                        session.Send(res);
                    }
                }
            });
        }

        public static void SaveChatIcon(ulong userDbId, SSendChatIcon chat, ClientSession session)
        {
            Instance.Add(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    UserDb userDb = db.Users.FirstOrDefault(u => u.UserDbId == userDbId);
                    if (userDb == null) return;

                    ChatRoomDb roomDb = db.ChatRooms.FirstOrDefault(r => r.ChatRoomNumber == chat.RoomNumber);
                    if (roomDb == null) return;

                    ChatDb newChat = new()
                    {
                        ChatType = (ushort)chat.Chat.ChatBase.ChatType,
                        IconId = chat.Chat.IconId,
                        Message = null,
                        RoomDbId = chat.RoomNumber,
                        Room = roomDb,
                        Sender = userDb,
                        SenderDbId = userDbId,
                        SendTime = chat.Chat.ChatBase.Timestamp.ToDateTime(),
                    };

                    db.Chats.Add(newChat);
                    userDb.SentChats.Add(newChat);
                    roomDb.Chats.Add(newChat);

                    bool saved = db.SaveChangesEx();
                    if (saved == true)
                    {
                        CSendChat res = new()
                        {
                            Error = SendChatError.Success,
                            RoomNumber = chat.RoomNumber,
                            ChatId = chat.ChatId,
                        };
                        session.Send(res);
                    }
                    else
                    {
                        // TODO : error
                        CSendChat res = new()
                        {
                            Error = SendChatError.Other,
                            RoomNumber = chat.RoomNumber,
                            ChatId = chat.ChatId,
                        };
                        session.Send(res);
                    }
                }
            });
        }

        // Note : roomNumber is not p.k
        public static void FindRoomInfo(uint roomNumber, Action<RoomInfo> callback)
        {
            Instance.Add(() =>
            {
                using (AppDbContext db = new())
                {
                    ChatRoomDb room = db.ChatRooms.FirstOrDefault(r => r.ChatRoomNumber == roomNumber);

                    if (room == null)
                    {
                        // error : 없을 수가 없음
                        CoreLogger.LogError("DbProcess", "There is a critical error at creating and update room info logic.");
                        return;
                    }

                    callback?.Invoke(RoomInfo.FromChatRoomDb(room));
                }
            });
        }
    }
}
