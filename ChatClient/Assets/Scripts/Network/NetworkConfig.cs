using UnityEditor;
using UnityEngine;

namespace Chat.Network
{
    [CreateAssetMenu(fileName = "NetworkConfig", menuName ="Network/NetworkConfig")]
    public class NetworkConfig : ScriptableObject
    {
        public bool UseLocal;
        public string EndpointIPAddress;
        public int Port;

        public override string ToString()
        {
            return $"UseLocal: {UseLocal}, Endpoint: {EndpointIPAddress}, Port: {Port}";
        }
    }
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(NetworkConfig))]
    class NetworkConfigEditor : UnityEditor.Editor
    {
        SerializedProperty m_useLocal;
        SerializedProperty m_endpointIPAddress;
        SerializedProperty m_port;

        NetworkConfig configs;

        private void OnEnable()
        {
            configs = (NetworkConfig)target;

            m_useLocal = serializedObject.FindProperty(nameof(NetworkConfig.UseLocal));
            m_endpointIPAddress = serializedObject.FindProperty(nameof(NetworkConfig.EndpointIPAddress));
            m_port = serializedObject.FindProperty(nameof(NetworkConfig.Port));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_useLocal);

            if (configs.UseLocal == true)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(m_endpointIPAddress);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.PropertyField(m_endpointIPAddress);
            }

            EditorGUILayout.PropertyField(m_port);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
