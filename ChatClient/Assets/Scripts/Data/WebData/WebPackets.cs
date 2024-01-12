using System;
using System.Net;

public interface IValidateWebPacket
{
    public bool Validate();
}

[Serializable]
public class AccountLoginWebReq : IValidateWebPacket
{
    public AccountLoginWebReq(string accountId, string accountPassword)
    {
        AccountId = accountId;
        AccountPassword = accountPassword;
        IPv4Address = NetworkUtils.GetIPv4Address();
    }

    public string AccountId { get; set; }
    public string AccountPassword { get; set; } // encrypted-string
    public string IPv4Address { get; set; } // ip address ipv4

    // account 서버에서 id/pw 인증 후 클라이언트에 auth token과 account db id전송
    // 추가로 chat 서버에서 같은 auth token과 로그인 요청 예정인 IpAddress (ipv4) 전달
    // 이후 chat 서버에서는 접속 요청을 받으면 accountdbid와 authtoke, ipaddress 확인



    public bool Validate()
    {
        return !(string.IsNullOrEmpty(AccountId) || string.IsNullOrEmpty(AccountPassword) || string.IsNullOrEmpty(IPv4Address));
    }
}

[Serializable]
public class AccountLoginWebRes : IValidateWebPacket
{
    public int Res { get; set; }
    public string AuthToken { get; set; } 
    public long AccountDbId { get; set; }

    public bool Validate()
    {
        if (Res == 0) return true;
        else
        {
            return !(string.IsNullOrEmpty(AuthToken) || AccountDbId == 0); 
        }
    }
}

[Serializable]
public class CreateAccountWebReq : IValidateWebPacket
{
    public string AccountId { get; set; }
    public string AccountPassword { get; set; } // encrypted-string

    public bool Validate()
    {
        return !(string.IsNullOrEmpty(AccountId) || string.IsNullOrEmpty(AccountPassword));
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


