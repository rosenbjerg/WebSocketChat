using System.Collections.Concurrent;
using RedHttpServerCore.Response;

namespace ChatRoom.NETCore
{
    partial class RoomManager
    {
        private readonly ConcurrentDictionary<string, Room> _rooms = new ConcurrentDictionary<string, Room>();

        public void Join(string room, WebSocketDialog wsd)
        {
            Room r;
            if (!_rooms.TryGetValue(room, out r))
            {
                r = new Room();
                _rooms.TryAdd(room, r);
            }
            r.Add(wsd);
            wsd.OnClosed += (sender, args) => LeaveRoom(room, r, wsd);
        }

        private void LeaveRoom(string r, Room room, WebSocketDialog wsd)
        {
            if (room.Leave(wsd))
                _rooms.TryRemove(r, out room);
        }
    }
}