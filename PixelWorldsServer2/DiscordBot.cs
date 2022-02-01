using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Text;
using Discord.Net;
using System.Threading.Tasks;

namespace PixelWorldsServer2
{
    public class DiscordBot
    {
        private static DiscordSocketClient _client = new DiscordSocketClient();
        private const string token = "OTI1NTExMTEwODUxOTUyNjcw.YcuLYw.GQEp1utua4ckeXpfKGMQ-1YaqCo";
        public static bool hasInit = false;

        public static async Task UpdateStatus(string status)
        {
            await _client.SetGameAsync(status);
        }

        private static Task InternalLog(LogMessage msg)
        {
            return Task.CompletedTask;
        }

        private static Task Connected()
        {
            Util.Log("Discord Bot connected successfully!");
            return Task.CompletedTask;
        }

        public static void Init()
        {
            _client.Connected += Connected;
            _client.Log += InternalLog;
            hasInit = true;

            _ = Login();
        }

        public static async Task Login()
        {
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }
    }
}
