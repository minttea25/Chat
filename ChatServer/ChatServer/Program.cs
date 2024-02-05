using Chat.DB;
using Chat.Network;
using ChatSharedDb;
using ServerCoreTCP;
using ServerCoreTCP.CLogger;
using System;
using System.Net;
using System.Threading;
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
            ServerService service;

            try
            {
                MessageManager.Instance.Init();
                Console.WriteLine(Config.Instance.Configs);

                SharedDbContext.SetConnString(Config.Instance.Configs.SharedDBConnectionString);

                var config = LoggerConfig.GetDefault();
                //config.RestrictedMinimumLevel = Serilog.Events.LogEventLevel.Error;
                config.RestrictedMinimumLevel = Serilog.Events.LogEventLevel.Verbose;
                CoreLogger.CreateLoggerWithFlag(
                    Config.Instance.GetLoggerSinks(),
                    config);

                string host = Dns.GetHostName(); // local host name of my pc
                IPHostEntry ipHost = Dns.GetHostEntry(host);
                IPAddress ipAddr = ipHost.AddressList[Config.Instance.Configs.HostEntryIndex];
                IPEndPoint endPoint = new IPEndPoint(address: ipAddr, port: Config.Instance.Configs.Port);

                ServerServiceConfig serverConfig = Config.Instance.GetServiceConfig();

                service = new ServerService(endPoint, SessionManager.Instance.CreateNewSession, serverConfig);
                service.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }

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

            MainCommand(service);

            //Console.ReadLine();
            service.Stop();
        }

        static void MainCommand(ServerService service)
        {
            while (true)
            {
                string command = Console.ReadLine();
                switch (command)
                {
                    case "stop":
                        return;
                    case "config":
                        Console.WriteLine(Config.Instance.Configs);
                        break;
                    case "pool_count":
                        Console.WriteLine($"Total SAEA Pool Count: {service.SAEATotalPoolCount}");
                        Console.WriteLine($"Current Pooled SAEA: {service.SAEACurrentPooledCount}");
                        Console.WriteLine($"Total Session Pool Count: {service.SessionTotalPoolCount}");
                        Console.WriteLine($"Current Pooled Session: {service.SessionCurrentPooledCount}");
                        break;
                    default:
                        Console.WriteLine($"Unknown Command: {command}");
                        break;
                }
                Thread.Yield();
            }
        }
    }
}
