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
    internal class TextMessageHandler : IUpdateSubHandler
    {
        public async Task HandleAsync(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            var message = update.Message;
            if (message == null) throw new NullUpdateException("Message is null");

            long chatId = message.Chat.Id;
            if (chatId == 0) throw new NullUpdateException("ChatId equals 0");

            string? text = message!.Text;
            if (text == null) throw new NullUpdateException("Text is null");

            // Return 'en' by default
            string langCode = await UserCacheService.Instance.GetUserLanguage(chatId);

            switch (text)
            {
                case "/start" or "/change_lang":
                    {
                        UserDTO? user = await UserRepository.Instance.GetUserById(chatId);
                        if (user == null)
                        {
                            string? f_name = update?.Message?.From?.FirstName;
                            string? l_name = update?.Message?.From?.LastName;
                            string? username = update?.Message?.From?.Username;
                            await UserRepository.Instance.AddNewUser(chatId, f_name, l_name, username);
                        }

                        string mText = "Select language";

                        var markup = InlineMarkupBuilder.GetMarkupFromMassive(
                            ("Set English language", "/en_Lang"),
                            ("Сменить язык на Русский", "/ru_Lang")
                        );

                        await botClient.SendMessage(
                            chatId: chatId,
                            text: mText,
                            replyMarkup: markup,
                            cancellationToken: cts
                        );
                        return;
                    }
                case "/get_word":
                    {
                        string? word = await UserRepository.Instance.GetUserWordById(chatId);
                        if (string.IsNullOrEmpty(word))
                        {                           
                            string mText = LocalizationService.Get(langCode, "WordIsNull_text");
                            await botClient.SendMessage(
                                chatId: chatId,
                                text: mText,
                                replyMarkup: InlineMarkupBuilder.GetMarkupWithOnlyBackButton(langCode),
                                cancellationToken: cts
                            );
                            return;
                        }
                        else
                        {
                            string mText = LocalizationService.Get(langCode, "HasWord_text", word);
                            await botClient.SendMessage(
                                chatId: chatId,
                                text: mText,
                                replyMarkup: InlineMarkupBuilder.GetMarkupWithOnlyBackButton(langCode),
                                cancellationToken: cts
                            );
                            return;
                        }
                    }
                case "/files":
                    {
                        var fileRepository = FileRepository.Instance;
                        if (fileRepository == null)
                        {
                            throw new ComponentInitializeException("File Repository is Null");
                        }
                        List<string> userFiles = await fileRepository!.GetFilesByUserId(chatId);
                        if (userFiles.Count == 0)
                        {
                            string mText = LocalizationService.Get(langCode, "FilesNotFound_text");

                            await botClient.SendMessage(
                                chatId: chatId,
                                text: mText,
                                cancellationToken: cts
                            );
                        }
                        else
                        {
                            string mText = LocalizationService.Get(langCode, "FilesFound_text");

                            await botClient.SendMessage(
                                chatId: chatId,
                                text: mText,
                                replyMarkup: InlineMarkupBuilder.GetMarkupFromListWithSuffix(userFiles, "_File", 1, langCode),
                                cancellationToken: cts
                                );
                        }
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
                case "/menu":
                    {
                        string mText = LocalizationService.Get(langCode, "Menu_text");

                        await botClient.SendMessage(
                            chatId: chatId,
                            text: mText,
                            replyMarkup: InlineMarkupBuilder.GetMenuMarkup(langCode),
                            cancellationToken: cts
                        );
                        return;
                    }  
            }

            if (text.StartsWith("/word"))
            {
                string f_text = text.Replace("/word", "").Trim();
                if (string.IsNullOrEmpty(f_text))
                {
                    string mText = LocalizationService.Get(langCode, "WordIsNull_text");

                    await botClient.SendMessage(
                        chatId: chatId,
                        text: mText,
                        replyMarkup: InlineMarkupBuilder.GetMarkupWithOnlyBackButton(langCode),
                        cancellationToken: cts
                    );
                    return;
                }
                else
                {
                    await UserRepository.Instance.SetUserWord(chatId, f_text);
                    string mText = LocalizationService.Get(langCode, "WordSaved_text");

                    await botClient.SendMessage(
                        chatId: chatId,
                        text: mText,
                        replyMarkup: InlineMarkupBuilder.GetMarkupWithOnlyBackButton(langCode),
                        cancellationToken: cts
                    );
                    return;
                }
            }
        }
    }
}
