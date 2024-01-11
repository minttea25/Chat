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
        readonly static UnityManager _unity = new();
        readonly static RoomManager _room = new();

        public static NetworkManager Network => _network;
        public static UnityManager Unity => _unity;
        public static RoomManager Room => _room;


        readonly static List<IManager> _managers = new()
        {
            _resource, _ui, _scene, _sound, _data, // add customs...
            _network, _unity, _room
        };

        private void OnEnable()
        {
            LoadBaseResource();
        }

        private void Update()
        {
            _network.Update();
            _unity.Update();
        }

        public void OnDisable()
        {
            ReleaseBaseResource();
            _network.StopService();
            Clear();
        }

        // Game Contents
        void LoadBaseResource()
        {
            Resource.LoadAllAsync(
                (failed) =>
                {
                    Core.Utils.AssertCrash(failed.Count == 0);
                },
                AddrKeys.SimplePopupUI);
        }

        void ReleaseBaseResource()
        {
            Resource.ReleaseAll(
                AddrKeys.SimplePopupUI);
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


