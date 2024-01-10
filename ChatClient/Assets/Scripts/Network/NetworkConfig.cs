using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using UnityEngine;



namespace Chat.Network
{
    [CreateAssetMenu(fileName = "NetworkConfig", menuName ="NetworkConfig")]
    public class NetworkConfig : ScriptableObject
    {
        [SerializeField]
        string _EndpointIPAddress;
        [SerializeField]
        int _Port;
        [SerializeField]
        bool _UseLocal;

        public string EndpointIPAddress => _EndpointIPAddress;
        public int Port => _Port;
        public bool UseLocal => _UseLocal;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_UseLocal == true) return;

            if (string.IsNullOrEmpty(_EndpointIPAddress) == false)
            {
                if (IPAddress.TryParse(_EndpointIPAddress, out var addr4) && (addr4.AddressFamily == AddressFamily.InterNetwork))
                {
                    Debug.Log("The ipaddress is IPv4.");
                }
                else if (IPAddress.TryParse(_EndpointIPAddress, out var addr6) && (addr6.AddressFamily == AddressFamily.InterNetworkV6))
                {
                    Debug.Log("The ipaddress is IPv6.");
                }
                else
                {
                    Debug.LogWarning("The ipaddress may be invalid.");
                }
            }
        }
#endif
    }
}
