namespace AccountServer.Packets
{
    [Serializable]
    public class AccountLoginWebReq
    {
        public string? AccountId { get; set; }
        public string? AccountPassword { get; set; } // encrypted-string
        public string? IPv4Address { get; set; } // ip address ipv4

    }

    [Serializable]
    public class AccountLoginWebRes
    {
        public int Res { get; set; }
        public string? AuthToken { get; set; }
        public long AccountDbId { get; set; }
    }

    [Serializable]
    public class CreateAccountWebReq 
    {
        public string? AccountId { get; set; }
        public string? AccountPassword { get; set; } // encrypted-string
    }

    [Serializable]
    public class CreateAccountWebRes
    {
        public int Res { get; set; }
    }
}
