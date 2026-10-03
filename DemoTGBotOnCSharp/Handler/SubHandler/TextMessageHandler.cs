using DemoTGBotOnCSharp.Handler.SubHandler.Interface;
using DemoTGBotOnCSharp.Repositories;
using DemoTGBotOnCSharp.Repositories.DTO;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DemoTGBotOnCSharp.Handler.SubHandler
{
    internal class TextMessageHandler : IUpdateSubHandler
    {
        public async Task HandleAsync(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            var message = update.Message;
            if (message == null) throw new InvalidOperationException("Message равен null");

            long chatId = message.Chat.Id;
            if (chatId == 0) throw new InvalidOperationException("ChatId равен 0");

            string? text = message!.Text;
            if (text == null) throw new InvalidOperationException("Text равен null");

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

                        string m_text = "Добро пожаловать в демонстрационного бота!";

                        await botClient.SendMessage(
                            chatId: chatId,
                            text: m_text,
                            replyMarkup: GetStartMarkup(),
                            cancellationToken: cts
                        );
                        return;
                    }
                case "/get_word":
                    {
                        string? word = await UserRepository.Instance.GetUserWordById(chatId);
                        if (string.IsNullOrEmpty(word))
                        {
                            string m_text = "Вы еще не сохраняли свое слово! Напишите '/word ваше_слово'";
                            await botClient.SendMessage(
                                chatId: chatId,
                                text: m_text,
                                replyMarkup: GetmarkupWithBackButton(),
                                cancellationToken: cts
                            );
                            return;
                        }
                        else
                        {
                            string m_text = "Ваше слово: " + word;
                            await botClient.SendMessage(
                                chatId: chatId,
                                text: m_text,
                                replyMarkup: GetThirdButtonMarkup(),
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
                            throw new InvalidOperationException("File Repository равен Null");
                        }
                        List<string> userFiles = await fileRepository!.GetFilesByUserId(chatId);
                        if (userFiles.Count == 0)
                        {
                            await botClient.SendMessage(
                                chatId: chatId,
                                text: "Вы еще не отправляли файлы боту",
                                cancellationToken: cts
                            );
                        }
                        else
                        {
                            await botClient.SendMessage(
                                chatId: chatId,
                                text: "Выберите любой файл",
                                replyMarkup: CreateMarkupFromListForFiles(userFiles),
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
                    string m_text = "Неправильный формат команды, пример: '/word ваше_слово'";
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: m_text,
                        replyMarkup: GetmarkupWithBackButton(),
                        cancellationToken: cts
                    );
                    return;
                }
                else
                {
                    await UserRepository.Instance.SetUserWord(chatId, f_text);
                    string m_text = "Ваше слово сохранено, узнать его можно написав сообщение /get_word";
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: m_text,
                        replyMarkup: GetThirdButtonMarkup(),
                        cancellationToken: cts
                    );
                    return;
                }
            }
        }

        private InlineKeyboardMarkup CreateMarkupFromListForFiles(List<string> files)
        {
            var markup = new InlineKeyboardMarkup();
            foreach (string file in files)
            {
                var btn = InlineKeyboardButton.WithCallbackData(file, file + "_File");
                markup.AddNewRow(new[] { btn });
            }
            return markup;
        }

        private InlineKeyboardMarkup GetStartMarkup()
        {
            var markup = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Что умеет этот бот?", callbackData: "StartButton")
            });
            return markup;
        }

        private InlineKeyboardMarkup GetThirdButtonMarkup()
        {
            var markup = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Что еще умеет этот бот?", callbackData: "ThirdButton")
            });
            return markup;
        }

        private InlineKeyboardMarkup GetmarkupWithBackButton()
        {
            return new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("Назад", "StartButton")
            });
        }
    }
}
