using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public interface IManager
    {
        void InitManager();
        void ClearManager();
    }

    public class ManagerCore : MonoBehaviour
    {
        static ManagerCore _instance;
        public static ManagerCore Instance => _instance;

        readonly static ResourceManagerCore _resource = new();
        readonly static UIManagerCore _ui = new();
        readonly static SceneManagerCore _scene = new();
        readonly static SoundManagerCore _sound = new();
        readonly static DataManagerCore _data = new();

        public static ResourceManagerCore Resource => _resource;
        public static UIManagerCore UI => _ui;
        public static SceneManagerCore Scene => _scene;
        public static SoundManagerCore Sound => _sound;
        public static DataManagerCore Data => _data;

        // add custom managers...
        readonly static NetworkManager _network = new();

        public static NetworkManager Network => _network;

        readonly static List<IManager> _managers = new()
        {
            _resource, _ui, _scene, _sound, _data, // add customs...
            _network
        };

        private void Update()
        {
            _network.Update();
        }

        public void OnDisable()
        {
            _network.StopService();
        }

        // Game Contents

        public static Coroutine StartCoroutineEx(IEnumerator coroutine)
        {
            return _instance.StartCoroutine(coroutine);
        }

        internal static void Init()
        {
            if (_instance == null)
            {
                GameObject o = GameObject.Find(Const.ManagerName);
                if (o == null)
                {
                    o = new GameObject { name = Const.ManagerName };
                    _ = o.AddComponent<ManagerCore>();
                }

                DontDestroyOnLoad(o);
                _instance = o.GetOrAddComponent<ManagerCore>();

                foreach (var managers in _managers)
                {
                    managers.InitManager();
                }
            }
        }

        internal static void Clear()
        {
            foreach (var managers in _managers)
            {
                managers.ClearManager();
            }
        }
    }
}


