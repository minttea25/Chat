using Chat;
using Google.Protobuf.WellKnownTypes;
using ServerCoreTCP.MessageWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Chat
{
    public partial class ClientSession : PacketSession
    {
        public uint ServerSessionId { get; set; } = 0;
        public UserInfo UserInfo { get; set; }
        List<Room> _rooms;


        public List<RoomInfo> GetRoomInfos()
        {
            return _rooms.Select(r => r.RoomInfo).ToList();
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

        public override void ClearSession()
        {
            UserInfo = null;
            _rooms = null;
        }
    }
}
