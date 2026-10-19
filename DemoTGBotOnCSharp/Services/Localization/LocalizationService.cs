using System.Xml;

namespace DemoTGBotOnCSharp.Services.Localization
{
    internal static class LocalizationService
    {
        private static readonly Dictionary<string, Dictionary<string, string>> _localizedTexts =
            new Dictionary<string, Dictionary<string, string>>
            {
                { "ru", new Dictionary<string, string> {
                    { "AboutBot_text", "Этот бот создан чтобы продемонстрировать основные возможности телеграм ботов\n" +
                        "Он работает в связке с базой данных MySQL, в которой хранятся все пользовательские данные. " +
                        "Кроме того, бот взаимодействует с файловой системой сервера, что позволяет ему сохранять и отправлять файлы, предоставленные пользователями." },

                    { "AboutDataStoredInDB", "Бот хранит данные пользователя в таблице 'Users'. " +
                        "Таблица содержит такие поля как Id чата, Имя, Фамилия, UserName, Секретное слово, переменную IsAdmin и строку LanguageCode(en/ru)\n" +
                        "Для отправки сообщения боту достаточно лишь знать Id чата. Остальные данные могут быть использованы для многих других вещей. " +
                        "Например для удобства администрирования бота и много чего еще." },

                    { "Contact_text", "Если вас что-то заинтересовало или вы хотите задать вопрос — пишите, не стесняйтесь. @wrotoftanks63" },
                    { "FilesNotFound_text", "Вы еще не отправляли файлы боту! Отправьте любой файл" },
                    { "FilesFound_text", "Выберите нужный файл"},
                    { "FileSaved", "Вы можете просмотреть свои файлы написав команду /files" },
                    { "Game_text", "Нажми на кнопку играть чтобы запустить игру" },
                    { "GameEnd_text", "🎮 Поздравляю, ваш счет : {0}\n" +
                        "🏆 Твой ранг в таблице лидеров : {1}" },
                    { "HasWord_text", "Ваше слово: {0}"},
                    { "HereYourFile", "Вот твое слово" },
                    { "InvalidWordFormat_text", "Неправильный формат, пример: '/word ваше_слово'" },
                    
                    { "Menu_text", "Нажмите кнопку из списка ниже" },
                    { "ReadMoreAboutBot_text", "Доделать!" },
                    { "Start_text", "Добро пожаловать в демонстрационного бота!" },
                    { "ScoreSaveError_text", "❌ Не удалось сохранить рекорд" },
                    { "WordIsNull_text", "Вы еще не сохранили свое слово, напишите /word 'Ваше слово'" },
                    { "WordSaved_text", "Ваше слово сохранено, вы можете получить его в любое время написав /get_word" },
                    { "WhatThisBotCanDoBtn_text", "Что умеет этот бот?" },

                    // Текст исключений
                    // Exception text
                    { "FileNotFound_ex_text", "Файл не найден\nОтправьте новый файл" },
                    { "Basic_ex_text", "Проиошла непредвиденная ошибка, пожалуйста повторите попытку или напишите /start" },
                    { "FileAlreadyExists_ex_text", "Файл с таким именем уже существует!" },

                    // Текст кнопок
                    // Button text
                    { "BackBtn_text", "Назад в меню" },
                    { "ContactMeBtn_text", "Связаться со мной" },
                    { "LanguageUpdated", "Вы успешно сменили язык на {0}" },
                    { "MoreAboutDataInDB", "Узнать больше про базу данных" },
                    { "PlayMiniGameBtn_text", "🕹️ Сыграть в игру 🕹️" },
                    { "PlayFlappyBirdBtn_text", "🕹️ Сыграть в Flappy Bird 🕹️" },
                    { "ReadMoreBtn_text", "Узнать больше!" },
                    { "ShowUserDataBtn_text", "Вывести информацию обо мне" },
                    { "ShowFilesBtn_text", "Показать мои файлы" },
                    { "ShowWordBtn_text", "Показать мое слово" },
                    { "ChangeLanguageBtn_text", "Сменить язык" },

                    // Значений
                    // Values
                    { "NoData", "Пусто" },
                    { "NotRankerYet", "Ранг еще не присвоен" },

                    // Форматируемые строки
                    // Formattable strings
                    { "UserInfo_text", "First name - {0}\nLast name - {1}\nUsername - {2}\nSecret word - {3}" },
                    { "FileExtensionNotAllowed_ex_text", "Расширение файла не поддерживается!\nПоддерживаемые расширения: {0}" },

                }},
                { "en", new Dictionary<string, string> {
                    { "AboutBot_text", "This bot was created to demonstrate the main features of Telegram bots.\n" +
                        "It works in tandem with a MySQL database that stores all user data. " +
                        "In addition, the bot interacts with the server's file system, allowing it to save and send files provided by users." },

                    { "AboutDataStoredInDB", "The bot stores user data in the 'Users' table. " +
                        "The table contains fields such as Chat Id, First Name, Last Name, UserName, Secret Word, an IsAdmin variable, and a LanguageCode (en/ru) string.\n" +
                        "To send a message, the bot only needs to know the Chat Id. The rest of the data can be used for many other purposes. " +
                        "For example, for easier bot administration and much more." },

                    { "Contact_text", "If you are interested in something or want to ask a question, feel free to drop a message. @wrotoftanks63" },
                    { "FilesNotFound_text", "You haven't sent any files to the bot yet! Send any file." },
                    { "FilesFound_text", "Select the required file"},
                    { "FileSaved", "You can view your files by typing the /files command." },
                    { "Game_text", "Click the play button to start the game." },
                    { "GameEnd_text", "🎮 Congratulations! Your score: {0}\n" +
                        "🏆 Your rank on the leaderboard: {1}" },
                    { "HasWord_text", "Your word: {0}"},
                    { "HereYourFile", "Here is your file" }, // Исправил логическую опечатку оригинала (вместо "слово" для ключа HereYourFile логичнее "file")
                    { "InvalidWordFormat_text", "Invalid format. Example: '/word your_word'" },

                    { "Menu_text", "Press a button from the list below" },
                    { "ReadMoreAboutBot_text", "To be completed!" },
                    { "Start_text", "Welcome to the demonstration bot!" },
                    { "ScoreSaveError_text", "❌ Failed to save the high score" },
                    { "WordIsNull_text", "You haven't saved your word yet. Type /word 'Your word'" },
                    { "WordSaved_text", "Your word has been saved. You can retrieve it anytime by typing /get_word" },
                    { "WhatThisBotCanDoBtn_text", "What can this bot do?" },

                    // Текст исключений
                    // Exception text
                    { "FileNotFound_ex_text", "File not found\nSend an another file" },
                    { "Basic_ex_text", "An error occured, please try again or write /start" },
                    { "FileAlreadyExists_ex_text", "File with the same name already exists!" },

                    // Текст кнопок
                    // Button text
                    { "BackBtn_text", "Back to menu" },
                    { "ContactMeBtn_text", "Contact me" },
                    { "LanguageUpdated", "You have successfully changed your language to {0}" },
                    { "MoreAboutDataInDB", "Learn more about the database" },
                    { "PlayMiniGameBtn_text", "🕹️ Play a Game 🕹️" },
                    { "PlayFlappyBirdBtn_text", "🕹️ Play Flappy Bird 🕹️" },
                    { "ReadMoreBtn_text", "Learn more!" },
                    { "ShowUserDataBtn_text", "Show info about me" },
                    { "ShowFilesBtn_text", "Show my files" },
                    { "ShowWordBtn_text", "Show my word" },
                    { "ChangeLanguageBtn_text", "Change language" },


                    // Значений
                    // Values
                    { "NoData", "Empty" },
                    { "NotRankerYet", "Not ranked yet" },

                    // Форматируемые строки
                    // Formattable strings
                    { "UserInfo_text", "First name - {0}\nLast name - {1}\nUsername - {2}\nSecret word - {3}" },
                    { "FileExtensionNotAllowed_ex_text", "File extension is not allowed!\nAllowed extensions: {0}" },

                }}
            };
        // args нужен для форматирования строки
        // `args` is used for string formatting.
        public static string Get(string langCode, string key, params object[] args)
        {
            var lang = _localizedTexts.ContainsKey(langCode) ? langCode : "en";
            var text = _localizedTexts[lang].ContainsKey(key) ? _localizedTexts[lang][key] : key;

            if (args != null && args.Length > 0)
            {
                return string.Format(text, args);
            }

            return text;
        }

    }
}
