using System;
using System.Diagnostics;
using System.Net;

public class ChatUtils
{
    public static string ToLocalTimeFormat(DateTime time, string format = "HH:mm")
    {
        return time.ToLocalTime().ToString(format);
    }
}

public class NetworkUtils
{
    public static string GetIPv4Address()
    {
        string localIPAddress = "";

        // Get the local machine's IP addresses
        IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

        // Iterate through the IP addresses and find the IPv4 address
        foreach (IPAddress ipAddress in localIPs)
        {
            if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                localIPAddress = ipAddress.ToString();
                break;
            }
        }

        if (string.IsNullOrEmpty(localIPAddress))
        {
            throw new Exception("Can not find IPv4 Address.");
        }

        return localIPAddress;
    }

    /// <summary>
    /// Note : It may be IPv4 or IPv6.
    /// </summary>
    /// <returns></returns>
    public static IPAddress GetLocalIPAddress()
    {
        string host = Dns.GetHostName(); // local host name of my pc
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        if (ipHost == null || ipHost.AddressList.Length == 0)
        {
            throw new Exception("Can not find local IPAddress.");
        }
        return ipHost.AddressList[0];
    } 
}
