using Chat;
using Google.Protobuf;
using ServerCoreTCP.Job;
using ServerCoreTCP.Utils;
using System.Collections.Generic;

namespace Chat
{
    public partial class Room : JobSerializer, IUpdate
    {
        public ulong DbId => RoomInfo.RoomDbId;
        public string Name => RoomInfo.RoomName;
        public uint Number => RoomInfo.RoomNumber;
        public int UserCount => sessions.Count;
        public RoomInfo RoomInfo { get; private set; }

        Dictionary<ulong, ClientSession> sessions = new Dictionary<ulong, ClientSession>();

        public Room(RoomInfo roomInfo)
        {
            RoomInfo = new();
            // copy
            RoomInfo.MergeFrom(roomInfo);
        }

        void Broadcast<T>(T message) where T : IMessage
        {
            foreach (var session in  sessions.Values)
            {
                session.Send(message);
            }
        }

        public void Update()
        {
            Flush();
        }

        public void AddSession(ClientSession session)
        {
            Add(() =>
            {
                if (sessions.ContainsKey(session.UserInfo.UserDbId) == true) return;
                sessions.Add(session.UserInfo.UserDbId, session);
            });
        }
    }
}