using Telegram.Bot;
using Telegram.Bot.Types;

namespace DemoTGBotOnCSharp.FileStorage.Interface
{
    internal interface IFileController
    {
        Task<string> SaveTelegramFileAsync(ITelegramBotClient botClient, Document document, long chatId, string? caption = null);
        Task RemoveFileFromDirectory(string filePath, long chatId);
        Task<FileStream> GetFile(string fileName, long chatId);
    }
}
