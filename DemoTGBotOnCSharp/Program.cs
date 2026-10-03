using DemoTGBotOnCSharp.Handler;
using DotNetEnv;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace DemoTGBotOnCSharp
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Enter path to .env file...");
            string path = Console.ReadLine();
            while (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                Console.WriteLine("File not found. Try again...");
                path = Console.ReadLine();
            }

            // 2. Загружаем переменные из найденного файла в память приложения
            try
            {
                // 3. Загружаем указанный пользователем файл через DotNetEnv
                Env.Load(path);
                Console.WriteLine("Configuration succesfuly loaded from .env file.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading the file by DotNetEnv: {e.Message}");
                return;
            }

            string botToken = Environment.GetEnvironmentVariable("TG_BOT_TOKEN");

            if (string.IsNullOrEmpty(botToken))
            {
                Console.WriteLine("Bot token not found in .env file");
                return;
            }
           
            var botClient = new TelegramBotClient(botToken);

            var me = await botClient.GetMe();
            Console.WriteLine($"Bot {me.Username} started");

            var recieverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery },
                DropPendingUpdates = true
            };

            using var cts = new CancellationTokenSource();
            
            botClient.StartReceiving(
                updateHandler: BotHandler.HandleUpdateAsync,
                errorHandler: BotHandler.HandlePollingErrorAsync,
                receiverOptions: recieverOptions,
                cancellationToken: cts.Token);

            Console.ReadLine();
            cts.Cancel();
        }
    }
}
