using DemoTGBotOnCSharp.FileStorage;
using DemoTGBotOnCSharp.FileStorage.Exception;
using DemoTGBotOnCSharp.Handler.Exceptions;
using DemoTGBotOnCSharp.Handler.SubHandler;
using DemoTGBotOnCSharp.Handler.SubHandler.Interface;
using DemoTGBotOnCSharp.MarkupBuilder;
using DemoTGBotOnCSharp.Services.Localization;
using DemoTGBotOnCSharp.Services.UserCache;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DemoTGBotOnCSharp.Handler
{
    internal class BotHandler
    {
        // Инициализация обработчиков
        // Initialization of handlers
        private static readonly IUpdateSubHandler textHandler = new TextMessageHandler();
        private static readonly IUpdateSubHandler documentHandler = new DocumentHandler();
        private static readonly IUpdateSubHandler callbackQueryHandler = new CallbackQueryHandler();
        private static readonly IUpdateSubHandler webAppDataHandler = new WebAppDataHandler();

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update == null) return;

            Console.WriteLine($"Getting update, callbackQuery = {(update.CallbackQuery != null ? update.CallbackQuery.Data : null)}, document = {update.Message?.Document != null}, text = {update.Message?.Text ?? "[Empty text]"}");
            if (update.CallbackQuery != null)
            {
                await HandleCallbackQuery(botClient, update, cancellationToken);
            }
            else if (update.Message?.Document != null)
            {
                await HandleDocumentMessage(botClient, update, cancellationToken);
            }
            else if (update.Message?.WebAppData != null)
            {
                await HandleWebAppData(botClient, update, cancellationToken);
            }
            else if (update.Message?.Text != null)
            {
                await HandleTextMessage(botClient, update, cancellationToken);
            }
        }

        private static async Task HandleCallbackQuery(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                await callbackQueryHandler.HandleAsync(botClient, update, cancellationToken);
            }
            catch (NullUpdateException e)
            {
                Console.WriteLine($"NullUpdateException : {e.Message}");
            }
            catch (FileExistsException e)
            {
                Console.WriteLine($"File is not exists : {e.Message}");

                var message = update!.CallbackQuery!.Message;

                long chatId = message!.Chat.Id;
                int messageId = message!.MessageId;

                string langCode = await UserCacheService.Instance.GetUserLanguage(chatId);

                string mText = LocalizationService.Get(langCode, "FileNotFound_ex_text");

                await botClient.EditMessageText(
                    chatId: chatId,
                    messageId: messageId,
                    text: mText,
                    replyMarkup: InlineMarkupBuilder.GetMarkupWithOnlyBackButton(langCode),
                    cancellationToken: cancellationToken
                );
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception, message is : {e.Message}");

                var message = update!.CallbackQuery!.Message;

                long chatId = message!.Chat.Id;
                int messageId = message!.MessageId;
                
                string langCode = await UserCacheService.Instance.GetUserLanguage(chatId);

                string mText = LocalizationService.Get(langCode, "Basic_ex_text");

                await botClient.SendMessage(
                    chatId: chatId,
                    text: mText,
                    replyMarkup: InlineMarkupBuilder.GetMenuMarkup(langCode),
                    cancellationToken: cancellationToken
                );
            }
        }
        private static async Task HandleDocumentMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                await documentHandler.HandleAsync(botClient, update, cancellationToken);
            }
            catch (NullUpdateException e)
            {
                Console.WriteLine($"NullUpdateException : {e.Message}");
            }
            catch (FileExistsException e)
            {
                Console.WriteLine($"File Exists exception, message is : {e.Message}");

                var message = update!.Message;

                long chatId = message!.Chat.Id;

                string langCode = await UserCacheService.Instance.GetUserLanguage(chatId);


                string mText = LocalizationService.Get(langCode, "FileAlreadyExists_ex_text");

                await botClient.SendMessage(
                    chatId: update!.Message!.Chat.Id,
                    text: mText,
                    replyMarkup: InlineMarkupBuilder.GetMarkupWithOnlyBackButton(langCode),
                    cancellationToken: cancellationToken
                );
            }
            catch (FileExtensionException e)
            {
                Console.WriteLine($"File extension is not allowed : {e.Message}");

                var message = update!.Message;

                long chatId = message!.Chat.Id;

                string langCode = await UserCacheService.Instance.GetUserLanguage(chatId);

                
                string mText = LocalizationService.Get(langCode, "FileExtensionNotAllowed_ex_text", string.Join(", ", FileController.Instance.AllowedExtension));

                await botClient.SendMessage(
                    chatId: chatId,
                    text: mText,
                    replyMarkup: InlineMarkupBuilder.GetMarkupWithOnlyBackButton(langCode),
                    cancellationToken: cancellationToken
                );
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception, message is : {e.Message}");

                var message = update!.CallbackQuery!.Message;

                long chatId = message!.Chat.Id;

                string langCode = await UserCacheService.Instance.GetUserLanguage(chatId);

                string mText = LocalizationService.Get(langCode, "Basic_ex_text");

                await botClient.SendMessage(
                    chatId: chatId,
                    text: mText,
                    replyMarkup: InlineMarkupBuilder.GetMenuMarkup(langCode),
                    cancellationToken: cancellationToken
                );
            }
        }

        private static async Task HandleTextMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                await textHandler.HandleAsync(botClient, update, cancellationToken);
            }
            catch (NullUpdateException e)
            {
                Console.WriteLine($"NullUpdateException : {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception, message is : {e.Message}");

                var message = update!.CallbackQuery!.Message;

                long chatId = message!.Chat.Id;

                string langCode = await UserCacheService.Instance.GetUserLanguage(chatId);

                string mText = LocalizationService.Get(langCode, "Basic_ex_text");

                await botClient.SendMessage(
                    chatId: chatId,
                    text: mText,
                    replyMarkup: InlineMarkupBuilder.GetMenuMarkup(langCode),
                    cancellationToken: cancellationToken
                );
            }
        }

        private static async Task HandleWebAppData(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                await webAppDataHandler.HandleAsync(botClient, update, cancellationToken);
            }
            catch (NullUpdateException e)
            {
                Console.WriteLine($"NullUpdateException : {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception, message is : {e.Message}");
                var message = update!.CallbackQuery!.Message;

                long chatId = message!.Chat.Id;

                string langCode = await UserCacheService.Instance.GetUserLanguage(chatId);

                string mText = LocalizationService.Get(langCode, "Basic_ex_text");

                await botClient.SendMessage(
                    chatId: chatId,
                    text: mText,
                    replyMarkup: InlineMarkupBuilder.GetMenuMarkup(langCode),
                    cancellationToken: cancellationToken
                );
            }
        }
        public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}
