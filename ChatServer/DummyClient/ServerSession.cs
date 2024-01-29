using Chat;
using ServerCoreTCP.MessageWrapper;
using System.Net;

namespace DummyClient
{
    internal class ServerSession : PacketSession
    {
        public override string ToString()
        {
            return $"Session_{Id}";
        }

        static object _lock = new object();
        static int _id = 10000;

        public List<uint> Rooms = new List<uint>();
        public UserInfo UserInfo = new UserInfo();
        public int Id { get; private set; }

        public bool IsConnected { get; set; } = false;

        public override void InitSession()
        {
        }

        public override void OnConnected(EndPoint endPoint)
        {
            Id = Interlocked.Increment(ref _id);
            // login
            SLoginReq req = new SLoginReq()
            {
                AccountDbId = Id,
                AuthToken = "", // not use in test
                Ipv4Address = "",
            };

            Send(req);
        }

        public override void OnDisconnected(EndPoint endPoint, object? error = null)
        {
        }

        public override void OnRecv(ReadOnlySpan<byte> buffer)
        {
            MessageManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfBytes)
        {
        }

        public override void PreSessionCleanup()
        {
        }
    }
}
