using DemoTGBotOnCSharp.FileStorage;
using DemoTGBotOnCSharp.Handler.SubHandler.Interface;
using DemoTGBotOnCSharp.MarkupBuilder;
using DemoTGBotOnCSharp.Repositories;
using DemoTGBotOnCSharp.Repositories.DTO;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DemoTGBotOnCSharp.Handler.SubHandler
{
    internal class CallbackQueryHandler : IUpdateSubHandler
    {
        public async Task HandleAsync(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            // Достаем объект callback-запроса
            var callbackQuery = update?.CallbackQuery;
            if (callbackQuery == null) throw new NullUpdateException("CallbackQuery не получено");

            var message = callbackQuery.Message;
            if (message == null) throw new NullUpdateException("Message из callbackQuery = null");

            long chatId = callbackQuery.Message!.Chat.Id;
            if (chatId == 0) throw new NullUpdateException("ChatId равен 0");

            // Получаем callbackdata
            string? buttonData = callbackQuery.Data;

            switch (buttonData)
            {
                case "/menu":
                    {
                        int messageId = message.Id;
                        string m_text = "Select option from a list";

                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: m_text,
                            replyMarkup: InlineMarkupBuilder.GetMenuMarkup(),
                            cancellationToken: cts
                        );
                        return;
                    }
                case "/about_bot":
                    {
                        int messageId = message.Id;
                        string m_text = "This bot was created to demonstrate the basic capabilities of the Telegram Bot API.\n" +
                            "It operates in conjunction with a MySQL database, where all user data is stored. Additionally, the bot interacts with the server's file system, enabling it to save and send files submitted by users.";

                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: m_text,
                            replyMarkup: InlineMarkupBuilder.GetMarkupWithOneButton("Read more", "/read_more_about_bot"),
                            cancellationToken: cts
                        );
                        return;
                    }
                case "/read_more_about_bot":
                    {
                        int messageId = message.Id;
                        string m_text = "Todo";

                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: m_text,
                            replyMarkup: InlineMarkupBuilder.GetMarkupWithOneButton("Back to menu", "/menu"),
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
                        string? f_name = "no data";
                        string? l_name = "no data";
                        string? username = "no data";
                        string? word = "no data";
                        // Add a user if there is no data in the database.
                        if (user != null)
                        {
                            if (!string.IsNullOrEmpty(user.f_name)) f_name = user.f_name;
                            if (!string.IsNullOrEmpty(user.l_name)) l_name = user.f_name;
                            if (!string.IsNullOrEmpty(user.username)) username = user.f_name;
                            if (!string.IsNullOrEmpty(user.word)) word = user.word;
                        } 
                        else
                        {
                            f_name = message.Chat.FirstName;
                            l_name = message.Chat.LastName;
                            username = message.Chat.Username;

                            await userRepository.AddNewUser(chatId, f_name, l_name, username);
                        }

                        string m_text = $"First name - {f_name}\nLast name - {l_name}\nUsername - {username}\nSecret word - {word}";

                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: m_text,
                            replyMarkup: InlineMarkupBuilder.GetMarkupWithOneButton("More about data stored in the DB", "/about_data_stored_in_db"),
                            cancellationToken: cts
                        );
                        return;
                    }
                case "/about_data_stored_in_db":
                    {
                        int messageId = message.Id;

                        string m_text = "The bot stores user data in the `Users` table. This table contains the chat ID, first name, last name, username, a secret word, and the `isAdmin` boolean variable. " +
                            "Only the `chatId` field is mandatory for sending messages; the other fields allow the bot, among other things, to address the user by name.";

                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: m_text,
                            replyMarkup: InlineMarkupBuilder.GetMarkupWithOneButton("Back to menu", "/menu"),
                            cancellationToken: cts
                        );
                        return;
                    }
                case "/get_word":
                    {
                        int messageId = message.Id;

                        var userRepository = UserRepository.Instance;
                        if (userRepository == null) throw new ComponentInitializeException("User repository is null");

                        string m_text;

                        string? word = await userRepository.GetUserWordById(chatId);
                        if (string.IsNullOrEmpty(word))
                        {
                            m_text = "You haven't saved your secret word yet. Write /word 'your_word'";
                        }
                        else
                        {
                            m_text = $"Your secret word - {word}";
                        }

                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: m_text,
                            replyMarkup: InlineMarkupBuilder.GetMarkupWithOneButton("Back to menu", "/menu"),
                            cancellationToken: cts
                        );
                        return;
                    }
                case "/contact":
                    {
                        int messageId = message.Id;

                        string m_text = "If you are interested in anything or would like to ask a question, feel free to write.\n@wrotoftanks63";

                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: m_text,
                            replyMarkup: InlineMarkupBuilder.GetMarkupWithOneButton("Back to menu", "/menu"),
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

                    await botClient.SendDocument(
                        chatId: chatId,
                        document: inputFile,
                        caption: $"Here your file!"
                    );

                    await botClient.SendMessage(
                        chatId: chatId,
                        text: "Select options from a list",
                        replyMarkup: InlineMarkupBuilder.GetMenuMarkup(),
                        cancellationToken: cts
                    );
                }
                return;
            }
        }
    }
}