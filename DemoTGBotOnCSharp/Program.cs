using DemoTGBotOnCSharp.Database;
using DemoTGBotOnCSharp.Handler;
using DotNetEnv;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
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

            // Загружаем переменные из найденного файла в память приложения
            // Loading variables from the discovered file into the application's memory
            try
            {
                // 3. Loading the user-specified file via DotNetEnv
                Env.Load(path);
                Console.WriteLine("Configuration succesfuly loaded from .env file.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading the file by DotNetEnv: {e.Message}");
                return;
            }

            // Создание таблиц в базе данных
            // Creating tables in a database
            var databaseCreator = new DatabaseCreator();
            await databaseCreator.CreateDatabaseAsync();

            string botToken = Environment.GetEnvironmentVariable("TG_BOT_TOKEN");

            if (string.IsNullOrEmpty(botToken))
            {
                Console.WriteLine("Bot token not found in .env file");
                return;
            }
           
            var botClient = new TelegramBotClient(botToken);

            // Проверка инициализирован ли бот
            // Check whether the bot has been initialized   
            var me = await botClient.GetMe();
            Console.WriteLine($"Bot {me.Username} started");
            try
            {
                await ConfigureBotCommandAsync(botClient);
            }
            catch (Exception e)
            {
                throw new InvalidCastException(e.Message);
            }

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

        // Создание команд, которые будут находится слева от поля ввода сообщения
        // Creating commands that will be positioned to the left of the message input field
        private static async Task ConfigureBotCommandAsync(ITelegramBotClient botClient)
        {
            var command = new List<BotCommand>
            {
                new BotCommand { Command = "start", Description = "Hello message with register in MySQL database"},
                new BotCommand { Command = "menu", Description = "Bot menu with main commands"},
                new BotCommand { Command = "contact", Description = "Contact with bot's creator"},
            };

            await botClient.SetMyCommands(command);

            Console.WriteLine("Menu's button succesfuly updated");
        }
    }
}
