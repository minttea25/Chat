using System.Net;

namespace AccountServer.Packets
{
    public interface IValidateWebPacket
    {
        public bool Validate();
    }

    [Serializable]
    public class AccountLoginWebReq : IValidateWebPacket
    {
        public string? AccountId { get; set; }
        public string? AccountPassword { get; set; } // encrypted-string
        public string? IPv4Address { get; set; } // ip address ipv4

        public bool Validate()
        {
            return (!string.IsNullOrEmpty(AccountId) && !string.IsNullOrEmpty(AccountPassword) && !string.IsNullOrEmpty(IPv4Address));
        }

    }

    [Serializable]
    public class AccountLoginWebRes : IValidateWebPacket
    {
        public int Res { get; set; }
        public string? AuthToken { get; set; }
        public long AccountDbId { get; set; }
        public string? ServerName { get; set; } // not used in client (temp)
        public string? ServerIp { get; set; } // ip address to connect for chat
        public int? ServerPort { get; set; }

        public bool Validate()
        {
            if (Res != 1) return true;

            return !(string.IsNullOrEmpty(AuthToken)
                || AccountDbId == 0
                || string.IsNullOrEmpty(ServerName)
                || string.IsNullOrEmpty(ServerIp)
                || ServerPort == 0);
        }
    }

    [Serializable]
    public class CreateAccountWebReq : IValidateWebPacket
    {
        public string? AccountId { get; set; }
        public string? AccountPassword { get; set; } // encrypted-string

        public bool Validate()
        {
            return (!string.IsNullOrEmpty(AccountId) && !string.IsNullOrEmpty(AccountPassword));
        }
    }

    [Serializable]
    public class CreateAccountWebRes : IValidateWebPacket
    {
        public int Res { get; set; }

        public bool Validate()
        {
            return true;
        }
    }
}
