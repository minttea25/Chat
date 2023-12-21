using System;
using System.Net;
using ChatServer.Chat;
using ServerCoreTCP;
using ServerCoreTCP.CLogger;
using ServerCoreTCP.Core;
using ServerCoreTCP.Utils;

namespace ChatServer
{
    class Program
    {
        static void RoomManagerTask()
        {
            RoomManager.Instance.Update();
        }

        static void SessionTask()
        {
            foreach (var session in SessionManager.Instance.GetSessions())
            {
                session.FlushSend();
            }
        }

        static void Main(string[] args)
        {
            MessageManager.Instance.Init();

            var config = LoggerConfig.GetDefault();
            config.RestrictedMinimumLevel = Serilog.Events.LogEventLevel.Error;
            CoreLogger.CreateLoggerWithFlag(
                (uint)(CoreLogger.LoggerSinks.CONSOLE | CoreLogger.LoggerSinks.FILE),
                config);

            string host = Dns.GetHostName(); // local host name of my pc
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(address: ipAddr, port: 8888);

            ServerServiceConfig serverConfig = new()
            {
                SessionPoolCount = 50,
                SocketAsyncEventArgsPoolCount = 20,
                ReuseAddress = true,
                RegisterListenCount = 10,
                ListenerBacklogCount = 100,

            };

            ServerService service = new ServerService(endPoint, SessionManager.Instance.CreateNewSession, serverConfig);
            service.Start();

            TaskManager taskManager = new TaskManager();
            taskManager.AddTask(RoomManagerTask);
            taskManager.AddTask(SessionTask);
            taskManager.StartTasks();


            Console.ReadLine();
            service.Stop();
        }
    }
}
