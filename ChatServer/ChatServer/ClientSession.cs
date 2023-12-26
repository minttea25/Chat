using Chat;
using Google.Protobuf.WellKnownTypes;
using ServerCoreTCP.MessageWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ChatServer
{
    public class ClientSession : PacketSession
    {
        public uint ServerSessionId { get; set; } = 0;
        public UserInfo UserInfo { get; set; }
        List<Room> _rooms;


        public List<RoomInfo> GetRoomInfos()
        {
            return _rooms.Select(r => r.RoomInfo).ToList();
        }


        public void HandleRoomListReq()
        {
            CRoomListRes res = new();
            foreach (RoomInfo room in GetRoomInfos())
            {
                res.Rooms.Add(room);
            }
            res.LoadTime = Timestamp.FromDateTime(DateTime.UtcNow);

            // TEST
            res.Rooms.Add(new RoomInfo() { RoomId = 100, RoomName = "Room100" });
            res.Rooms.Add(new RoomInfo() { RoomId = 200, RoomName = "Room200" });
            res.Rooms.Add(new RoomInfo() { RoomId = 300, RoomName = "Room300" });

            Send(res);
        }


        public override void InitSession()
        {
            _rooms = new List<Room>();
        }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"Connected to {endPoint}");


        }

        public override void OnDisconnected(EndPoint endPoint, object error = null)
        {
            Console.WriteLine($"Disconnected: {endPoint}");
        }

        public override void OnRecv(ReadOnlySpan<byte> buffer)
        {
            MessageManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfBytes)
        {
            ;
        }

        public override void PreSessionCleanup()
        {
            UserInfo = null;
            _rooms = null;
        }
    }
}
