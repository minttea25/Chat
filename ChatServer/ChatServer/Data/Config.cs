using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Data
{
    [Serializable]
    public class ServerConfig
    {
        public string DBConnectionString = null;
        public string LogPath;
    }

    public class Config
    {
        public static ServerConfig Configs { get; set; }

        public static void LoadConfig()
        {
            string text = File.ReadAllText("config.json");
            Configs = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerConfig>(text);

            // null check
            if (Configs == null)
            {
                throw new NullReferenceException("Failed to read config.json");
            }

            if (string.IsNullOrEmpty(Configs.DBConnectionString))
            {
                throw new Exception("DBConnectionString is null or empty.");
            }

            if (string.IsNullOrEmpty(Configs.LogPath))
            {
                throw new Exception("LogPath is null or empty.");
            }
        }
    }
}
