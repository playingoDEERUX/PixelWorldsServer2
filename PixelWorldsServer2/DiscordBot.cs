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
            if (_client.ConnectionState == ConnectionState.Disconnected)
            {
                await Login();
            }

            await _client.SetGameAsync(status);
        }

        private static Task InternalLog(LogMessage msg)
        {
            System.Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public static void Init()
        {
            _client.Log += InternalLog;
            hasInit = true;
        }

        public static async Task Login()
        {
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }
    }
}
