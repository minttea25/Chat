using Chat;
using ServerCoreTCP.Job;
using ServerCoreTCP.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public partial class RoomManager : JobSerializerWithTimer, IUpdate
    {
        #region Singleton
        static RoomManager _instance = new RoomManager();
        public static RoomManager Instance { get { return _instance; } }
        #endregion

        const int CheckEmptyRoomIntervalMilliseconds = 1000;

        /// <summary>
        /// RoomNumber / Room
        /// </summary>
        Dictionary<ulong, Room> Rooms = new Dictionary<ulong, Room>();

        public void Update()
        {
            Flush();

            foreach (Room room in Rooms.Values)
            {
                room.Update();
            }
        }

        public RoomManager()
        {
            // 보류
            //AddAfter(CheckAndRemoveEmptyRoomsTimer, CheckEmptyRoomIntervalMilliseconds);
        }

        public List<RoomInfo> GetRooms()
        {
            return Rooms.Values.Select(r => r.RoomInfo).ToList();
        }

        public void AddRoom(RoomInfo roomInfo)
        {
            // 먼저 빠르게 거르기
            if (Rooms.ContainsKey(roomInfo.RoomNumber) == true) return;

            Add(() =>
            {
                if (Rooms.ContainsKey(roomInfo.RoomNumber) == true) return;

                Room room = new Room(roomInfo);
                Rooms.Add(roomInfo.RoomNumber, room);
            });
        }

        public void EnterRoom(RoomInfo roomInfo, ClientSession session)
        {
            Add(() =>
            {
                // 없으면 먼저 만든다
                if (Rooms.TryGetValue(roomInfo.RoomNumber, out var room) == false)
                {
                    room = new Room(roomInfo);
                    Rooms.Add(roomInfo.RoomNumber, room);
                }

                // 세션 추가
                room.AddSession(session);
            });
        }

        public void EnterRooms(List<RoomInfo> roomInfos, ClientSession session)
        {
            Add(() =>
            {
                foreach (var roomInfo in roomInfos)
                {
                    // 없으면 먼저 만든다
                    if (Rooms.TryGetValue(roomInfo.RoomNumber, out var room) == false)
                    {
                        room = new Room(roomInfo);
                        Rooms.Add(roomInfo.RoomNumber, room);
                    }

                    // 세션 추가
                    room.AddSession(session);
                }
            });
        }

        public void CreateRooms(List<RoomInfo> rooms)
        {
            Add(() =>
            {
                foreach (RoomInfo roomInfo in rooms)
                {
                    if (Rooms.ContainsKey(roomInfo.RoomNumber) == true) return;

                    Room room = new Room(roomInfo);
                    Rooms.Add(roomInfo.RoomNumber, room);
                }
            });
        }


        void CheckAndRemoveEmptyRoomsTimer()
        {
            // 보류
            //CheckAndRemoveEmptyRooms();
            //AddAfter(CheckAndRemoveEmptyRoomsTimer, CheckEmptyRoomIntervalMilliseconds);
        }

        /// <summary>
        /// 비어있는 방 확인 후 삭제
        /// </summary>
        void CheckAndRemoveEmptyRooms()
        {
            //Console.WriteLine("CheckAndRemoveEmptyRooms");
            foreach (var roomId in Rooms.Keys.ToList())
            {
                if (Rooms[roomId].UserCount == 0) Rooms.Remove(roomId);
            }
        }
    }
}
