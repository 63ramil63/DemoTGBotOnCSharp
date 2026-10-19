using DemoTGBotOnCSharp.FileStorage;
using DemoTGBotOnCSharp.Handler.SubHandler.Interface;
using DemoTGBotOnCSharp.Repositories;
using DemoTGBotOnCSharp.Handler.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;
using DemoTGBotOnCSharp.MarkupBuilder;
using DemoTGBotOnCSharp.Services.Localization;
using DemoTGBotOnCSharp.Services.UserCache;

namespace DemoTGBotOnCSharp.Handler.SubHandler
{
    internal class DocumentHandler : IUpdateSubHandler
    {
        public async Task HandleAsync(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            var message = update?.Message;
            if (message == null) throw new NullUpdateException("Message dont recieved");

            long chatId = message.Chat.Id;
            if (chatId == 0) throw new NullUpdateException("ChatId equals 0");

            var document = message.Document;
            if (document == null) throw new NullUpdateException("Document is null");

            var fileController = FileController.Instance;
            var fileRepository = FileRepository.Instance;

            if (fileController == null) throw new ComponentInitializeException("FileController is null");
            if (fileRepository == null) throw new ComponentInitializeException("FileRepository is null");

            string fileName = await fileController.SaveTelegramFileAsync(botClient, document, chatId, message.Caption);

            await fileRepository.AddNewFile(fileName, chatId);

            string langCode = await UserCacheService.Instance.GetUserLanguage(chatId);

            string mText = LocalizationService.Get(langCode, "FileSaved");

            string buttonText = LocalizationService.Get(langCode, "ShowFilesBtn_text");

            await botClient.SendMessage(
                chatId: chatId,
                text: mText,
                replyMarkup: InlineMarkupBuilder.GetMarkupFromMassive(
                    (buttonText, "/files")
                ),
                cancellationToken: cts
            );
        }
    }
}
