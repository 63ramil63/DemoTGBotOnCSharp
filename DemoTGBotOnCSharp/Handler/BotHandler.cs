using DemoTGBotOnCSharp.Handler.SubHandler;
using DemoTGBotOnCSharp.Handler.SubHandler.Interface;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DemoTGBotOnCSharp.Handler
{
    internal class BotHandler
    {
        private static readonly IUpdateSubHandler textHandler = new TextMessageHandler();
        private static readonly IUpdateSubHandler documentHandler = new DocumentHandler();
        private static readonly IUpdateSubHandler callbackQueryHandler = new CallbackQueryHandler();

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update == null) return;

                Console.WriteLine($"Получен update, callbackQuery = {(update.CallbackQuery != null ? update.CallbackQuery.Data : null)}, document = {update.Message?.Document != null}, text = {update.Message?.Text ?? "[Нет текста]"}");
                try
                {
                    if (update.CallbackQuery != null)
                    {
                        await callbackQueryHandler.HandleAsync(botClient, update, cancellationToken);
                    }
                    else if (update.Message?.Document != null)
                    {
                        await documentHandler.HandleAsync(botClient, update, cancellationToken);
                    }
                    else if (update.Message?.Text != null)
                    {
                        await textHandler.HandleAsync(botClient, update, cancellationToken);
                    }
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine($"{e.Message}");
                    await HandleError(update.Message.Chat.Id, botClient, cancellationToken, e.Message);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при обработке update: {e.Message}");
            }
        }

        private static async Task HandleError(long chatId, ITelegramBotClient botClient, CancellationToken cts, string errorMessage)
        {
            string m_text = "Произошла ошибка, повторите попытку и напишите '/menu'\n" + errorMessage;
            await botClient.SendMessage(
                chatId: chatId,
                text: m_text,
                cancellationToken: cts
                );
            return;
        }
        public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Ошибка Long Polling : {exception.Message}");
            return Task.CompletedTask;
        }
    }
}
