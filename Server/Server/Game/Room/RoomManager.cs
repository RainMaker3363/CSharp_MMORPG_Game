using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class RoomManager
    {
        public static RoomManager Instance { get; } = new RoomManager();

        object _lock = new object();
        Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
        int _roomId = 1;

        public GameRoom Add(int mapid)
        {
            GameRoom gameRoom = new GameRoom();
            gameRoom.Push(gameRoom.init, mapid);

            lock(_lock)
            {
                gameRoom.RoomID = _roomId;
                _rooms.Add(_roomId, gameRoom);
                _roomId++;
            }

            return gameRoom;
        }

        public bool Remove(int roomId)
        {
            lock(_lock)
            {
                return _rooms.Remove(roomId);
            }
        }

        public GameRoom Find(int roomid)
        {
            lock(_lock)
            {
                GameRoom room = null;
                if(_rooms.TryGetValue(roomid, out room))
                {
                    return room;
                }

                return null;
            }
        }
    }
}
