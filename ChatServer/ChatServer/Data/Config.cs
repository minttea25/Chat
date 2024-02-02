using ServerCoreTCP;
using ServerCoreTCP.CLogger;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Chat
{
    [Serializable]
    public class ServerConfig
    {
        public string DBConnectionString { get; set; }
        public string SharedDBConnectionString { get; set; }
        public int SessionPoolCount { get; set; }
        public bool ReuseAddress { get; set; }
        public int RegisterListenCount { get; set; }
        public int ListenerBacklogCount { get; set; }
        public int Port { get; set; }
        public int HostEntryIndex { get; set; } = 0;
        public bool LogFile { get; set; }
        public bool LogConsole { get; set; }
        public bool LogDebug { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            PropertyInfo[] properties = typeof(ServerConfig).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                sb.Append($"{property.Name}: ");
                sb.Append($"{property.GetValue(this)}");
                sb.Append('\n');
            }

            return sb.ToString();
        }

        public bool Validate()
        {
            return !string.IsNullOrEmpty(DBConnectionString)
                && !string.IsNullOrEmpty(SharedDBConnectionString)
                && Port != 0
                && SessionPoolCount != 0
                && RegisterListenCount != 0
                && ListenerBacklogCount != 0;
        }
    }

    public class Config
    {
        public static Config Instance => m_instance;
        readonly static Config m_instance = new();

        public const string ConfigFilePath = "config.json";

        public bool Loaded { get; private set; } = false;
        public ServerConfig Configs { get; set; }

        Config()
        {
            LoadConfig();
        }

        public ServerServiceConfig GetServiceConfig()
        {
            ServerServiceConfig serviceConfig = new()
            {
                SessionPoolCount = Configs.SessionPoolCount,
                ReuseAddress = Configs.ReuseAddress,
                RegisterListenCount = Configs.RegisterListenCount,
                ListenerBacklogCount = Configs.ListenerBacklogCount
            };

            return serviceConfig;
        }

        public uint GetLoggerSinks()
        {
            uint sinks = 0;
            if (Configs.LogConsole == true)
            {
                sinks |= (uint)CoreLogger.LoggerSinks.FILE;
            }
            if (Configs.LogDebug == true)
            {
                sinks |= (uint)CoreLogger.LoggerSinks.DEBUG;
            }
            if (Configs.LogFile == true)
            {
                sinks |= (uint)CoreLogger.LoggerSinks.FILE;
            }

            return sinks;
        }

        void LoadConfig()
        {
            if (File.Exists(ConfigFilePath) == false)
            {
                CreateNewFile();
            }

            string text = File.ReadAllText(ConfigFilePath);
            Configs = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerConfig>(text);

            // null check
            if (Configs == null)
            {
                throw new NullReferenceException("Failed to read config.json");
            }

            if (Configs.Validate() == false)
            {
                throw new Exception("Some values in config.json are not valid.");
            }

            Loaded = true;
        }

        void CreateNewFile()
        {
            var config = new ServerConfig()
            {
            };

            var text = Newtonsoft.Json.JsonConvert.SerializeObject(config);
            File.WriteAllText(ConfigFilePath, text);
        }

    }
}
