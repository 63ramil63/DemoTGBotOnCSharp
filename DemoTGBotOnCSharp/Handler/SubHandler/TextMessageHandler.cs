using DemoTGBotOnCSharp.Handler.SubHandler.Interface;
using DemoTGBotOnCSharp.MarkupBuilder;
using DemoTGBotOnCSharp.Repositories;
using DemoTGBotOnCSharp.Repositories.DTO;
using Telegram.Bot;
using Telegram.Bot.Types;

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

            switch (text)
            {
                case "/start":
                    {
                        UserDTO? user = await UserRepository.Instance.GetUserById(chatId);
                        if (user == null)
                        {
                            string? f_name = update?.Message?.From?.FirstName;
                            string? l_name = update?.Message?.From?.LastName;
                            string? username = update?.Message?.From?.Username;
                            await UserRepository.Instance.AddNewUser(chatId, f_name, l_name, username);
                        }

                        string m_text = "Welcome to demonstration bot!";

                        await botClient.SendMessage(
                            chatId: chatId,
                            text: m_text,
                            replyMarkup: InlineMarkupBuilder.GetMarkupWithOneButton("Menu", "/menu"),
                            cancellationToken: cts
                        );
                        return;
                    }
                case "/get_word":
                    {
                        string? word = await UserRepository.Instance.GetUserWordById(chatId);
                        if (string.IsNullOrEmpty(word))
                        {
                            string m_text = "You haven't saved your word yet! Write '/word your_word'";
                            await botClient.SendMessage(
                                chatId: chatId,
                                text: m_text,
                                replyMarkup: InlineMarkupBuilder.GetMarkupWithOneButton("Back to menu", "/menu"),
                                cancellationToken: cts
                            );
                            return;
                        }
                        else
                        {
                            string m_text = "Your word: " + word;
                            await botClient.SendMessage(
                                chatId: chatId,
                                text: m_text,
                                replyMarkup: InlineMarkupBuilder.GetMarkupWithOneButton("Back to menu", "/menu"),
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
                            await botClient.SendMessage(
                                chatId: chatId,
                                text: "You haven't sent any files to the bot yet.",
                                cancellationToken: cts
                            );
                        }
                        else
                        {
                            await botClient.SendMessage(
                                chatId: chatId,
                                text: "Select file",
                                replyMarkup: InlineMarkupBuilder.GetMarkupFromListWithSuffix(userFiles, "_File", 1),
                                cancellationToken: cts
                                );
                        }
                        return;
                    }
            }

            if (text.StartsWith("/word"))
            {
                string f_text = text.Replace("/word", "").Trim();
                if (string.IsNullOrEmpty(f_text))
                {
                    string m_text = "Invalid format, example: '/word ваше_слово'";

                    await botClient.SendMessage(
                        chatId: chatId,
                        text: m_text,
                        replyMarkup: InlineMarkupBuilder.GetMarkupWithOneButton("Back to menu", "/menu"),
                        cancellationToken: cts
                    );
                    return;
                }
                else
                {
                    await UserRepository.Instance.SetUserWord(chatId, f_text);
                    string m_text = "Your word has been saved; you can retrieve it by sending the message /get_word.";

                    await botClient.SendMessage(
                        chatId: chatId,
                        text: m_text,
                        replyMarkup: InlineMarkupBuilder.GetMarkupWithOneButton("Back to menu", "/menu"),
                        cancellationToken: cts
                    );
                    return;
                }
            }
        }
    }
}
