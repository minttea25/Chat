using ServerCoreTCP;
using System.Net;
using System.Net.Sockets;
using UnityEngine;



namespace Chat.Network
{
    [CreateAssetMenu(fileName = "NetworkConfig", menuName ="Network/NetworkConfig")]
    public class NetworkConfig : ScriptableObject
    {
        public bool UseLocal { get; private set; }
        public string EndpointIPAddress { get; private set; }
        public int Port { get; private set; }


#if UNITY_EDITOR
        const string useLocalFieldName = nameof(UseLocal);
        const string ipFieldName = nameof(EndpointIPAddress);
        const string portFieldName = nameof(Port);

        [UnityEditor.CustomEditor(typeof(NetworkConfig))]
        class NetworkConfigEditor : UnityEditor.Editor
        {
            NetworkConfig configs = null;
            private void OnEnable()
            {
                configs = (NetworkConfig)target;
            }

            public override void OnInspectorGUI()
            {
                configs.UseLocal = UnityEditor.EditorGUILayout.Toggle("Use Local", configs.UseLocal);

                // Disable IP Address and Port fields if UseLocal is true
                UnityEditor.EditorGUI.BeginDisabledGroup(configs.UseLocal);

                // Draw IP Address field
                configs.EndpointIPAddress = UnityEditor.EditorGUILayout.TextField("IP Address", configs.EndpointIPAddress);

                UnityEditor.EditorGUI.EndDisabledGroup();

                // Draw Port field
                configs.Port = UnityEditor.EditorGUILayout.IntField(portFieldName, configs.Port);
            }
        }

        private void OnValidate()
        {
            if (UseLocal == true) return;

            if (string.IsNullOrEmpty(EndpointIPAddress) == false)
            {
                if (IPAddress.TryParse(EndpointIPAddress, out var addr4) && (addr4.AddressFamily == AddressFamily.InterNetwork))
                {
                    Debug.Log("The ipaddress is IPv4.");
                }
                else if (IPAddress.TryParse(EndpointIPAddress, out var addr6) && (addr6.AddressFamily == AddressFamily.InterNetworkV6))
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
