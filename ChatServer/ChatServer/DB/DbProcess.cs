using Chat;
using ChatServer.Utils;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ServerCoreTCP;
using ServerCoreTCP.Job;
using System;
using System.Linq;

namespace Chat.DB
{
    public partial class DbProcess : JobSerializer
    {
        public static DbProcess Instance { get; } = new DbProcess();

        /// <summary>
        /// for TEST
        /// </summary>
        public static void CreateNewAccount(string username, ClientSession session)
        {
            // TEST codes
            AccountDb newAccount = new()
            {
                AccountLoginId = username,
            };
            
            using (AppDbContext db = new AppDbContext())
            {
                db.Accounts.Add(newAccount);
                bool saved = db.SaveChangesEx();

                if (saved == false) return;

                UserDb newUser = new()
                {
                    UserName = username,
                    Account = newAccount,
                    AccountDbId = newAccount.AccountDbId,
                };
                db.Users.Add(newUser);
                saved = db.SaveChangesEx();
                if (saved == false) return;

                UserInfo user = new()
                {
                    UserDbId = newUser.UserDbId, UserName = username,
                    UserLoginId = username,
                };

                session.SetInfo(newAccount, user);

                CLoginRes res = new CLoginRes()
                {
                    LoginRes = LoginRes.LoginSuccess,
                    UserInfo = session.UserInfo,
                };

                session.Send(res); // send directly OK.
            }
        }


        public static void CreateRoom(ulong roomNumber, string roomName, ClientSession session)
        {
            if (session == null) return;
            // wrong req
            if (session.SessionStatus != SessionStatus.LOGINNED) return;

            Instance.Add(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    // find room
                    ChatRoomDb chatRoom = db.ChatRooms.FirstOrDefault(c =>  c.ChatRoomNumber == roomNumber);

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
                            ChatRoomNumber = (uint)roomNumber,
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

        public static void EnterRoom(ulong roomNumber, ClientSession session)
        {
            if (session == null) return;
            // wrong req
            if (session.SessionStatus != SessionStatus.LOGINNED) return;

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
                CEnterRoomRes res = new()
                {
                    Res = EnterRoomRes.EnterRoomOk,
                    RoomNumber = roomDb.ChatRoomNumber,
                    RoomInfo = RoomInfo.FromChatRoomDb(roomDb, containUser: true),
                };
                session.Send(res);
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

            using (AppDbContext db = new AppDbContext())
            {
                // Rooms가 lazy loading이므로 include로 직접 로드 해주어야 함
                UserDb userDb = db.Users
                    .Include(u => u.Rooms)
                    .FirstOrDefault(u => u.UserDbId == session.UserInfo.UserDbId);

                // error : no user data
                if (userDb == null) return;

                CRoomListRes res = new();
                foreach (var room in userDb.Rooms)
                {
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
                        info.Users.Add(new UserInfo()
                        {
                            // 현대 user login id 빠져있음!
                            UserDbId = user.UserDbId,
                            UserName = user.UserName,
                        });
                    }
                }
                res.LoadTime = Timestamp.FromDateTime(DateTime.UtcNow);
                session.Send(res);
            }
        }

        public static void LeaveRoom(ulong userDbId, ulong roomNumber)
        {
            using (AppDbContext db = new AppDbContext())
            {
                // find the userinfo
                UserDb userDb = db.Users.FirstOrDefault(u => u.UserDbId == userDbId);
                if (userDb == null) return;

                ChatRoomDb roomDb = db.ChatRooms.FirstOrDefault(r => r.ChatRoomNumber == roomNumber);
                if (roomDb == null) return;

                userDb.Rooms.Remove(roomDb);
                roomDb.Users.Remove(userDb);

                bool saved = db.SaveChangesEx();
                if (saved == true)
                {
                    ;
                }
                else
                {
                    // TODO : error
                    throw new System.Exception("DbUpdate or DbUpdateConcurrent");
                }
            }
        }

        public static void SaveChatText(ulong userDbId, SSendChatText chat, ClientSession session)
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
        }

        public static void SaveChatIcon(ulong userDbId, SSendChatIcon chat, ClientSession session)
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
        }
    }
}
