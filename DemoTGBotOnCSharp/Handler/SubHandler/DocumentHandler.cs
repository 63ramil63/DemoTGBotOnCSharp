using DemoTGBotOnCSharp.FileStorage;
using DemoTGBotOnCSharp.Handler.SubHandler.Interface;
using DemoTGBotOnCSharp.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DemoTGBotOnCSharp.Handler.SubHandler
{
    internal class DocumentHandler : IUpdateSubHandler
    {
        public async Task HandleAsync(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            var message = update?.Message;
            if (message == null) throw new InvalidOperationException("Сообщение не получено");

            long chatId = message.Chat.Id;
            if (chatId == 0) throw new InvalidOperationException("ChatId равен 0");

            var document = message.Document;
            if (document == null) throw new InvalidOperationException("Document не получен или равен null");

            var fileController = FileController.Instance;
            var fileRepository = FileRepository.Instance;

            if (fileController == null) throw new InvalidOperationException("FileController равен null");
            if (fileRepository == null) throw new InvalidOperationException("FileRepository равен null");
            try
            {
                string fileName = await fileController.SaveTelegramFileAsync(botClient, document, chatId);
                await fileRepository.AddNewFile(fileName, chatId);
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "Свои файлы можно просмотреть командой '/files' или прописав команду '/menu'",
                    cancellationToken: cts
                    );
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"Произошла ошибка: {e.Message}");
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "Произошла ошибка, пожалуйста повторите попытку \n" + e.Message,
                    cancellationToken: cts
                );
                return;
            }
        }
    }
}
