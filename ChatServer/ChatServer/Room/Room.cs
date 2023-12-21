using ChatServer.Chat;
using Google.Protobuf;
using ServerCoreTCP.Job;
using ServerCoreTCP.Utils;
using System.Collections.Generic;

namespace ChatServer
{
    public partial class Room : JobSerializer, IUpdate
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public int UserCount => _users.Count;
        public RoomInfo RoomInfo { get; private set; }

        Dictionary<ulong, ClientSession> _users = new Dictionary<ulong, ClientSession>();

        public Room(ulong roomId, string name)
        {
            Id = roomId;
            Name = name;

            RoomInfo = new RoomInfo();
            RoomInfo.RoomId = roomId;
            RoomInfo.RoomName = name;
        }

        void Broadcast<T>(T message) where T : IMessage
        {
            foreach (var session in  _users.Values)
            {
                session.Send(message);
            }
        }

        public void Update()
        {
            Flush();


        }

        void UserEnterRoom(ClientSession session)
        {
            _users.Add(session.UserInfo.UserId, session);
        }
    }
}