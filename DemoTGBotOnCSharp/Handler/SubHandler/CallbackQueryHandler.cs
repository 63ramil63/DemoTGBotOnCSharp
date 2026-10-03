using DemoTGBotOnCSharp.FileStorage;
using DemoTGBotOnCSharp.Handler.SubHandler.Interface;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DemoTGBotOnCSharp.Handler.SubHandler
{
    internal class CallbackQueryHandler : IUpdateSubHandler
    {
        public async Task HandleAsync(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            // Достаем объект callback-запроса
            var callbackQuery = update?.CallbackQuery;
            if (callbackQuery == null) throw new InvalidOperationException("CallbackQuery не получено");

            var message = callbackQuery.Message;
            if (message == null) throw new InvalidOperationException("Message из callbackQuery = null");

            long chatId = callbackQuery.Message!.Chat.Id;
            if (chatId == 0) throw new InvalidOperationException("ChatId равен 0");

            // Получаем callbackdata
            string? buttonData = callbackQuery.Data;

            switch (buttonData)
            {
                case "StartButton":
                    {
                        int messageId = callbackQuery.Message!.Id;
                        string m_text = "Бот умеет создавать много кнопок";

                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: m_text,
                            replyMarkup: GetMarkupWithLotOfButtons(),
                            cancellationToken: cts
                        );

                        return;
                    }
                case "SecondButton":
                    {
                        int messageId = callbackQuery.Message!.Id;
                        string m_text = "Бот работает с БД MySQL. Он уже запомнил ваше имя, фамилия, юзернейм\n" +
                            "Также бот запоминает кодовое слово которое вы напишите '/word ваше слово'";
                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: m_text,
                            replyMarkup: GetmarkupWithBackButton(),
                            cancellationToken: cts
                        );

                        return;
                    }
                case "ThirdButton":
                    {
                        int messageId = callbackQuery.Message!.Id;
                        string m_text = "Бот может сохранять ваши файлы, которые вы ему отправите";
                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: m_text,
                            replyMarkup: GetmarkupWithBackButton(),
                            cancellationToken: cts
                            );
                        return;
                    }
            }

            if (buttonData!.EndsWith("_File"))
            {
                string fileName = buttonData.Replace("_File", "");
                try
                {
                    using (FileStream fs = await FileController.Instance.GetFile(fileName, chatId))
                    {
                        var inputFile = InputFile.FromStream(fs, fileName);

                        await botClient.SendDocument(
                            chatId: chatId,
                            document: inputFile,
                            caption: $"Вот ваш файл"
                        );

                        await botClient.SendMessage(
                            chatId: chatId,
                            text: "Это самые базовые вещи которые умеет бот.\n" +
                            "Бот в тг это универсальное и удобное средство коммуникации с людьми, которое автоматизирует любые задачи. " +
                            "Боты бывают разные и я уверен, что смогу написать бота любой сложности\n" +
                            "если хотите уточнить что либо или узнать, можете написать мне\n" +
                            "@wrotoftanks63"
                        );
                    }
                }
                catch (FileNotFoundException ex)
                {
                    // Обработка ситуации, если файл не найден в БД или на диске
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: $"К сожалению, файл '{fileName}' не найден."
                    );
                }
                catch (Exception ex)
                {
                    // Обработка остальных непредвиденных ошибок
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: "Произошла ошибка при отправке файла."
                    );
                }
            }

            //await botClient.AnswerCallbackQuery(
            //    callbackQueryId: callbackQuery.Id,
            //    cancellationToken: cts
            //);
        }

        private InlineKeyboardMarkup GetMarkupWithLotOfButtons()
        {
            var markup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "1", callbackData:"none")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "2", callbackData: "none"),
                    InlineKeyboardButton.WithCallbackData(text: "3", callbackData: "none")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "4", callbackData: "none"),
                    InlineKeyboardButton.WithCallbackData(text: "5", callbackData: "none"),
                    InlineKeyboardButton.WithCallbackData(text: "6", callbackData: "none")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "7", callbackData: "none"),
                    InlineKeyboardButton.WithCallbackData(text: "8", callbackData: "none"),
                    InlineKeyboardButton.WithCallbackData(text: "9", callbackData: "none"),
                    InlineKeyboardButton.WithCallbackData(text: "10", callbackData: "none"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Что еще может бот?", callbackData: "SecondButton")
                }
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
