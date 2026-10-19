using DemoTGBotOnCSharp.FileStorage;
using DemoTGBotOnCSharp.Handler.SubHandler.Interface;
using DemoTGBotOnCSharp.MarkupBuilder;
using DemoTGBotOnCSharp.Repositories;
using DemoTGBotOnCSharp.Repositories.DTO;
using DemoTGBotOnCSharp.Handler.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;
using DemoTGBotOnCSharp.Services.UserCache;
using DemoTGBotOnCSharp.Services.Localization;

namespace DemoTGBotOnCSharp.Handler.SubHandler
{
    internal class CallbackQueryHandler : IUpdateSubHandler
    {
        public async Task HandleAsync(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            // Достаем объект callback-запроса
            // Retrieve the callback request object
            var callbackQuery = update?.CallbackQuery;
            if (callbackQuery == null) throw new NullUpdateException("CallbackQuery not received");

            var message = callbackQuery.Message;
            if (message == null) throw new NullUpdateException("Message from callbackQuery is null");

            long chatId = callbackQuery.Message!.Chat.Id;
            if (chatId == 0) throw new NullUpdateException("ChatId is 0");

            // Если не значение не найдено, по умолчанию вернет 'en'
            string langCode = await UserCacheService.Instance.GetUserLanguage(chatId);

            // Получаем callbackData
            // Retrieve the callbackData
            string? buttonData = callbackQuery.Data;

            switch (buttonData)
            {
                case "/menu":
                    {
                        int messageId = message.Id;
                        string mText = LocalizationService.Get(langCode, "Menu_text");

                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: mText,
                            replyMarkup: InlineMarkupBuilder.GetMenuMarkup(langCode),
                            cancellationToken: cts
                        );
                        return;
                    }
                case "/about_bot":
                    {
                        int messageId = message.Id;
                        string mText = LocalizationService.Get(langCode, "AboutBot_text");

                        string buttonText = LocalizationService.Get(langCode, "ReadMoreBtn_text");
                        string backButtonText = LocalizationService.Get(langCode, "BackBtn_text");

                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: mText,
                            replyMarkup: InlineMarkupBuilder.GetMarkupFromMassive(
                                (buttonText, "/read_more_about_bot"),
                                (backButtonText, "/menu")
                            ),
                            cancellationToken: cts
                        );
                        return;
                    }
                case "/read_more_about_bot":
                    {
                        int messageId = message.Id;
                        string mText = LocalizationService.Get(langCode, "ReadMoreAboutBot_text");

                        string buttonText = LocalizationService.Get(langCode, "BackBtn_text");
                        string backButtonText = LocalizationService.Get(langCode, "BackBtn_text");

                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: mText,
                            replyMarkup: InlineMarkupBuilder.GetMarkupWithOnlyBackButton(langCode),
                            cancellationToken: cts
                        );
                        return;
                    }
                case "/show_user_data":
                    {
                        int messageId = message.Id;

                        var userRepository = UserRepository.Instance;
                        if (userRepository == null) throw new ComponentInitializeException("User repository is null");

                        UserDTO? user = await userRepository.GetUserById(chatId);
                        string? f_name = LocalizationService.Get(langCode, "NoData");
                        string? l_name = LocalizationService.Get(langCode, "NoData");
                        string? username = LocalizationService.Get(langCode, "NoData");
                        string? word = LocalizationService.Get(langCode, "NoData");
                        // Добавляем юзера если его нет в базе данных
                        // Add a user if there is no data in the database.
                        if (user != null)
                        {
                            if (!string.IsNullOrEmpty(user.f_name)) f_name = user.f_name;
                            if (!string.IsNullOrEmpty(user.l_name)) l_name = user.l_name;
                            if (!string.IsNullOrEmpty(user.username)) username = user.username;
                            if (!string.IsNullOrEmpty(user.word)) word = user.word;
                        } 
                        else
                        {
                            f_name = message.Chat.FirstName;
                            l_name = message.Chat.LastName;
                            username = message.Chat.Username;

                            await userRepository.AddNewUser(chatId, f_name, l_name, username);
                        }

                        string mText = LocalizationService.Get(langCode, "UserInfo_text", f_name, l_name, username, word);

                        string buttonText = LocalizationService.Get(langCode, "MoreAboutDataInDB");

                        string backButtonText = LocalizationService.Get(langCode, "BackBtn_text");

                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: mText,
                            replyMarkup: InlineMarkupBuilder.GetMarkupFromMassive(
                                (buttonText, "/about_data_stored_in_db"),
                                (backButtonText, "/menu")
                            ),
                            cancellationToken: cts
                        );
                        return;
                    }
                case "/about_data_stored_in_db":
                    {
                        int messageId = message.Id;

                        string mText = LocalizationService.Get(langCode, "AboutDataStoredInDB");

                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: mText,
                            replyMarkup: InlineMarkupBuilder.GetMarkupWithOnlyBackButton(langCode),
                            cancellationToken: cts
                        );
                        return;
                    }
                case "/get_word":
                    {
                        int messageId = message.Id;

                        var userRepository = UserRepository.Instance;
                        if (userRepository == null) throw new ComponentInitializeException("User repository is null");

                        string mText;

                        string? word = await userRepository.GetUserWordById(chatId);
                        if (string.IsNullOrEmpty(word))
                        {
                            mText = LocalizationService.Get(langCode, "WordIsNull_text");
                        }
                        else
                        {
                            mText = LocalizationService.Get(langCode, "HasWord_text", word);
                        }

                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: mText,
                            replyMarkup: InlineMarkupBuilder.GetMarkupWithOnlyBackButton(langCode),
                            cancellationToken: cts
                        );
                        return;
                    }
                case "/contact":
                    {
                        int messageId = message.Id;

                        string mText = LocalizationService.Get(langCode, "Contact_text");

                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: mText,
                            replyMarkup: InlineMarkupBuilder.GetMarkupWithOnlyBackButton(langCode),
                            cancellationToken: cts
                        );
                        return;
                    }
                case "/game":
                    {
                        string mText = LocalizationService.Get(langCode, "Game_text");

                        await botClient.SendMessage(
                            chatId: chatId,
                            text: mText,
                            replyMarkup: InlineMarkupBuilder.GetGameStartReplyMarkup(langCode),
                            cancellationToken: cts
                        );
                        return;
                    }
                case "/files":
                    {
                        var fileRepository = FileRepository.Instance;
                        if (fileRepository == null) throw new ComponentInitializeException("File repository is null");

                        int messageId = message.Id;

                        List<string> files = await fileRepository.GetFilesByUserId(chatId);

                        string mText;

                        if (files.Count > 0)
                        {
                            mText = LocalizationService.Get(langCode, "FilesFound_text");
                        } 
                        else
                        {
                            mText = LocalizationService.Get(langCode, "FilesNotFound_text");
                        }

                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: mText,
                            replyMarkup: InlineMarkupBuilder.GetMarkupFromListWithSuffix(files, "_File", 1, langCode),
                            cancellationToken: cts
                        );

                        return;
                    }
                case "/change_lang":
                    {
                        int messageId = message.Id;

                        string mText = "Select language";

                        var markup = InlineMarkupBuilder.GetMarkupFromMassive(
                            ("Set English language", "/en_Lang"),
                            ("Сменить язык на Русский", "/ru_Lang")
                        );

                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: mText,
                            replyMarkup: markup,
                            cancellationToken: cts
                        );
                        return;
                    }
            }

            if (buttonData!.EndsWith("_File"))
            {
                string fileName = buttonData.Replace("_File", "");
                using (FileStream fs = await FileController.Instance.GetFile(fileName, chatId))
                {
                    var inputFile = InputFile.FromStream(fs, fileName);

                    string captionText = LocalizationService.Get(langCode, "HereYourFile");

                    await botClient.SendDocument(
                        chatId: chatId,
                        document: inputFile,
                        caption: captionText,
                        cancellationToken: cts
                    );

                    string mText = LocalizationService.Get(langCode, "Menu_text");

                    await botClient.SendMessage(
                        chatId: chatId,
                        text: mText,
                        replyMarkup: InlineMarkupBuilder.GetMenuMarkup(langCode),
                        cancellationToken: cts
                    );
                }
                return;
            }
            else if (buttonData!.EndsWith("_Lang"))
            {
                int messageId = message.Id;

                string newLangCode = buttonData.Trim().Replace("_Lang", "");
                newLangCode = newLangCode.Replace("/", "");

                await UserCacheService.Instance.UpdateUserLanguageAsync(chatId, newLangCode);

                string mText = LocalizationService.Get(newLangCode, "LanguageUpdated", newLangCode);

                await botClient.EditMessageText(
                    chatId: chatId,
                    messageId: messageId,
                    text: mText,
                    replyMarkup: InlineMarkupBuilder.GetMarkupWithOnlyBackButton(langCode),
                    cancellationToken: cts
                );

            }
        }
    }
}