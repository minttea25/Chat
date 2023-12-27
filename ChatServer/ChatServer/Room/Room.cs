using Chat;
using Google.Protobuf;
using ServerCoreTCP.Job;
using ServerCoreTCP.Utils;
using System.Collections.Generic;

namespace Chat
{
    public partial class Room : JobSerializer, IUpdate
    {
        public ulong DbId => RoomInfo?.RoomDbId ?? throw new System.NullReferenceException();
        public string Name => RoomInfo?.RoomName ?? throw new System.NullReferenceException();
        public ulong Number => RoomInfo?.RoomNumber ?? throw new System.NullReferenceException();
        public int UserCount => _users.Count;
        public RoomInfo RoomInfo { get; private set; }

        Dictionary<ulong, ClientSession> _users = new Dictionary<ulong, ClientSession>();

        public Room(RoomInfo roomInfo)
        {
            // copy
            RoomInfo.MergeFrom(roomInfo);
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
            _users.Add(session.UserInfo.UserDbId, session);
        }
    }
}