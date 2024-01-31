using Chat.DB;
using Chat.Network;
using ServerCoreTCP;
using ServerCoreTCP.CLogger;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Chat
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

        static void DbTask()
        {
            DbProcess.Instance.Flush();
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
                ReuseAddress = true,
                RegisterListenCount = 10,
                ListenerBacklogCount = 100,

            };

            ServerService service = new ServerService(endPoint, SessionManager.Instance.CreateNewSession, serverConfig);
            service.Start();

            //TaskManager taskManager = new TaskManager();
            //taskManager.AddTask(RoomManagerTask);
            //taskManager.AddTask(SessionTask);
            //taskManager.AddTask(DbTask);
            //taskManager.StartTasks();

            Task roomTask = new Task(() =>
            {
                while (true) { RoomManager.Instance.Update(); }
            }, TaskCreationOptions.LongRunning);

            Task sessionTask = new Task(() =>
            {
                while (true)
                {
                    foreach (var session in SessionManager.Instance.GetSessions())
                    {
                        session.FlushSend();
                    }
                }
            }, TaskCreationOptions.LongRunning);

            Task dbTask = new Task(() =>
            {
                while (true) { DbProcess.Instance.Flush(); }
            }, TaskCreationOptions.LongRunning);

            roomTask.Start();
            sessionTask.Start();
            dbTask.Start();


            Console.ReadLine();
            service.Stop();
        }
    }
}
