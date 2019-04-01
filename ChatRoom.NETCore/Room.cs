using System.Collections.Generic;
using Red;

namespace ChatRoom.NETCore
{
    partial class RoomManager
    {
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