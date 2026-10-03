using Telegram.Bot;
using Telegram.Bot.Types;

namespace DemoTGBotOnCSharp.Handler.SubHandler.Interface
{
    internal interface IUpdateSubHandler
    {
        Task HandleAsync(ITelegramBotClient botClient, Update update, CancellationToken cts);
    }
}
