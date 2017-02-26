using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using RHttpServer;

namespace ChatRoom
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new TaskBasedHttpServer(5002);
            var rman = new RoomManager();

            server.Get("/", (req, res) =>
            {
                res.RenderPage("pages/index.ecs", new RenderParams
                {
                    {"url", "Main"}
                });
            });

            server.Get("/:room", (req, res) =>
            {
                var room = HttpUtility.UrlDecode(req.Params["room"]);
                res.RenderPage("pages/index.ecs", new RenderParams
                {
                    {"url", room}
                });
            });

            server.WebSocket("/:room", (req, wsd) =>
            {
                var room = HttpUtility.UrlDecode(req.Params["room"]).ToLowerInvariant();
                rman.Join(room, wsd);
            });

            server.Start(true);
        }
    }
}
