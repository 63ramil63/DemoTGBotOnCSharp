using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DemoTGBotOnCSharp.MarkupBuilder
{
    internal class InlineMarkupBuilder
    {
        private static string gameUrl;
        public static InlineKeyboardMarkup GetMarkupFromListWithSuffix(List<string> buttons, string uniqSuffix, int buttonsInRow)
        {
            
            if (buttonsInRow <= 0) buttonsInRow = 1;

            // Преобразование активного списка в кнопку с суффиксом
            // Converting an active list into a button with a suffix

            var allButtons = buttons
                .Select(btn => InlineKeyboardButton.WithCallbackData(btn, btn + uniqSuffix));

            // Преобразование списка в вложенный список [][] с учетом количества кнопок в ряду
            // Converting a list into a nested list `[][]`, based on the number of buttons per row
            var rows = allButtons.Chunk(buttonsInRow).ToList();

            var backButton = InlineKeyboardButton.WithCallbackData("Back to menu", "/menu");

            // Добавление кнопки «Назад» в конец списка
            // Adding a back button to the end of the list
            rows.Add(new[] { backButton });

            // Возвращение клавиатуры
            // Return ready markup
            return new InlineKeyboardMarkup(rows);
        }

        // Возвращение клавиатуры с одной кнопкой с CallbackData
        // Returning a single-button keyboard with CallbackData
        public static InlineKeyboardMarkup GetMarkupWithOneButton(string buttonName, string callbackData)
        {
            return new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData(buttonName, callbackData) });
        }

        // Неизменяемая клавиатура для меню
        // Fixed keyboard for menus
        public static InlineKeyboardMarkup GetMenuMarkup()
        {
            return new InlineKeyboardMarkup(
            new []
            {
                new[] { InlineKeyboardButton.WithCallbackData("Show Files", "/files") },
                new[] { InlineKeyboardButton.WithCallbackData("Show Word", "/get_word") },
                new[] { InlineKeyboardButton.WithCallbackData("Show User Data", "/show_user_data") },
                new[] { InlineKeyboardButton.WithCallbackData("What This bot can do", "/about_bot") },
                new[] { InlineKeyboardButton.WithCallbackData("Contact With Me", "/contact") },
                new[] { InlineKeyboardButton.WithCallbackData("Play mini game", "/game")}
            }
            );
        }

        public static ReplyKeyboardMarkup GetGameStartReplyMarkup()
        {
            // Берём URL игры из переменных окружения
            if (string.IsNullOrEmpty(gameUrl))
            {
                gameUrl = Environment.GetEnvironmentVariable("GITHUB_PAGE");
            }

            // Если URL нет, возвращаем пустую клавиатуру (или обработайте иначе)
            if (string.IsNullOrEmpty(gameUrl))
            {
                return new ReplyKeyboardMarkup(new[] { new KeyboardButton("Back to menu") })
                {
                    ResizeKeyboard = true
                };
            }

            // 🕹️ Создаем обычную клавиатурную кнопку с Mini App
            var replyMarkup = new ReplyKeyboardMarkup(new[]
            {
                new[] { KeyboardButton.WithWebApp("Play Flappy Bird 🕹️", new WebAppInfo { Url = gameUrl }) },
                new[] { new KeyboardButton("/menu") } // Обычная текстовая кнопка возврата
            });
            

            return replyMarkup;
        }
    }
}
