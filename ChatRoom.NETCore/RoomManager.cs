using System.Collections.Concurrent;
using System.Collections.Generic;
using RedHttpServer;

namespace ChatRoom.NETCore
{
    class RoomManager
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

        class Room
        {
            private readonly object _lock = new object();
            private readonly List<WebSocketDialog> _connected = new List<WebSocketDialog>();
            public void Add(WebSocketDialog wsd)
            {
                lock (_lock)
                {
                    _connected.Add(wsd);
                }
                wsd.OnTextReceived += RelayMessage;
                BroadcastCount();
            }

            private void RelayMessage(object sender, WebSocketDialog.TextMessageEventArgs e)
            {
                lock (_lock)
                {
                    foreach (var wsd in _connected)
                    {
                        wsd.SendText(e.Text);
                    }
                }
            }

            private void BroadcastCount()
            {
                var c = "c" + _connected.Count;
                lock (_lock)
                {
                    foreach (var wsd in _connected)
                    {
                        wsd.SendText(c);
                    }
                }
            }
            

            public bool Leave(WebSocketDialog wsd)
            {
                lock (_lock)
                {
                    _connected.Remove(wsd);
                }
                if (_connected.Count == 0)
                    return true;

                BroadcastCount();
                return false;
            }
        }
    }
}