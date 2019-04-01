using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Red;
using Red.EcsRenderer;

namespace ChatRoom.NETCore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new RedHttpServer(5002, "public");
            server.Use(new EcsRenderer(true));
            var rman = new RoomManager();
            server.RespondWithExceptionDetails = false;

            server.Get("/", async (req, res) =>
            {
                await res.RenderPage("pages/index.ecs", new RenderParams
                {
                    {"url", "Main"}
                });
            });
            
            server.Get("/:room", async (req, res) =>
            {
                var room = System.Net.WebUtility.UrlDecode(req.Parameters["room"]);
                await res.RenderPage("pages/index.ecs", new RenderParams
                {
                    {"url", room}
                });
            });

            server.WebSocket("/ws/:room", async (req, wsd) =>
            {
                var room = System.Net.WebUtility.UrlDecode(req.Parameters["room"]).ToLowerInvariant();
                rman.Join(room, wsd);
            });
            await server.RunAsync();
        }
    }
}
