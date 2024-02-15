using Chat;
using DummyClient;
using Google.Protobuf.WellKnownTypes;
using ServerCoreTCP;
using ServerCoreTCP.CLogger;
using System.Net;

class Program
{
    static List<ServerSession> sessions = new List<ServerSession>();
    static Random random = new Random();

    const int MAX_ROOM_NUMBER = 50;
    const int CLIENTS_COUNT = 1000;

    static void TrafficTest()
    {
        while (true)
        {
            foreach (var session in sessions)
            {
                if (session.IsConnected == false) continue;
                // test logic

                int r = random.Next(100);
                DoSomething(r, session);
            }
            int t = random.Next(2000);
            Task.Delay(t).Wait();
        }
    }

    static void DoSomething(int r, ServerSession ss)
    {
        // create room for 7%
        if (r < 7)
        {
            int roomId = random.Next(MAX_ROOM_NUMBER);
            SCreateRoomReq req = new SCreateRoomReq()
            {
                RoomName = $"room_{roomId}",
                RoomNumber = (uint)roomId,
            };
            CoreLogger.LogInfo("", req.ToString());
            ss.Send(req);

        }
        // leave room for 7%
        else if (r < 14)
        {
            int roomId = random.Next(ss.Rooms.Count);
            SLeaveRoomReq req = new SLeaveRoomReq()
            {
                RoomNumber = (uint)roomId,
                UserInfo = ss.UserInfo
            };
            CoreLogger.LogInfo("", req.ToString());
            ss.Send(req);
        }
        // enter room for 7%
        else if (r < 21)
        {
            int roomId = random.Next(MAX_ROOM_NUMBER);
            SEnterRoomReq req = new SEnterRoomReq()
            {
                RoomNumber = (uint)roomId,
            };
            CoreLogger.LogInfo("", req.ToString());
            ss.Send(req);
        }
        // send chat for 43%
        else if (r < 64) 
        {
            int roomId = random.Next(ss.Rooms.Count);
            SSendChatText chat = new SSendChatText()
            {
                RoomNumber = (uint)roomId,
                ChatId = 1,
                Chat = new ChatText()
                {
                    Msg = "This is TestMessage.",
                    ChatBase = new ChatBase() { ChatType = ChatType.Text, Timestamp = Timestamp.FromDateTime(DateTime.UtcNow) }
                },
                SenderInfo = ss.UserInfo
            };
            CoreLogger.LogInfo("", chat.ToString());
            ss.Send(chat);
        }
        // send emoticon for 36%
        else
        {
            int roomId = random.Next(ss.Rooms.Count);
            SSendChatIcon chat = new SSendChatIcon()
            {
                RoomNumber = (uint)roomId,
                ChatId = 1,
                Chat = new ChatIcon()
                {
                    IconId = 1,
                    ChatBase = new ChatBase() { ChatType = ChatType.Icon, Timestamp = Timestamp.FromDateTime(DateTime.UtcNow) }
                },
                SenderInfo = ss.UserInfo
            };
            CoreLogger.LogInfo("", chat.ToString());
            ss.Send(chat);
        }
    }

    static void SessionTask()
    {
        while (true)
        {
            foreach (var session in sessions)
            {
                session.FlushSend();
            }
        }
    }

    static void Main()
    {
        Thread.Sleep(3000);

        MessageManager.Instance.Init();

        string host = Dns.GetHostName(); // local host name of my pc
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(address: ipAddr, port: 8888);

        var config2 = LoggerConfig.GetDefault();
        //config.RestrictedMinimumLevel = Serilog.Events.LogEventLevel.Error;
        config2.RestrictedMinimumLevel = Serilog.Events.LogEventLevel.Verbose;
        CoreLogger.CreateLoggerWithFlag(
            (uint)(CoreLogger.LoggerSinks.FILE | CoreLogger.LoggerSinks.CONSOLE),
            config2);

        ClientServiceConfig config = new ClientServiceConfig()
        {
            ClientServiceSessionCount = CLIENTS_COUNT,
            ReuseAddress = true,
        };

        ClientService clientService = new ClientService(
            endPoint,
            () => { ServerSession session = new ServerSession(); sessions.Add(session); return session; },
           config, null);

        clientService.Start();

        Thread.Sleep(10000); // wait 10 seconds for connecting server
        Console.WriteLine("Begin---");

        Task sessionTask = new Task(SessionTask, TaskCreationOptions.LongRunning);
        sessionTask.Start();

        Task task = new Task(TrafficTest, TaskCreationOptions.LongRunning);
        task.Start();

        Console.ReadLine();
    }
}