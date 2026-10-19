using DemoTGBotOnCSharp.Services.Localization;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DemoTGBotOnCSharp.MarkupBuilder
{
    internal class InlineMarkupBuilder
    {
        private static string gameUrl;
        public static InlineKeyboardMarkup GetMarkupFromListWithSuffix(List<string> buttons, string uniqSuffix, int buttonsInRow, string langCode)
        {
            if (buttonsInRow <= 0) buttonsInRow = 1;

            // Преобразование активного списка в кнопку с суффиксом
            // Converting an active list into a button with a suffix
            var allButtons = buttons
                .Select(btn => InlineKeyboardButton.WithCallbackData(btn, btn + uniqSuffix));

            // Преобразование списка в вложенный список [][] с учетом количества кнопок в ряду
            // Converting a list into a nested list `[][]`, based on the number of buttons per row
            var rows = allButtons.Chunk(buttonsInRow).ToList();

            var backButton = InlineKeyboardButton.WithCallbackData(LocalizationService.Get(langCode, "BackBtn_text"), "/menu");

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

        public static InlineKeyboardMarkup GetMarkupFromMassive(params (string Text, string CallbackData)[] buttons)
        {
            // Создаем список строк, где каждая строка содержит ровно одну кнопку
            var rows = buttons.Select(b => new[] { InlineKeyboardButton.WithCallbackData(b.Text, b.CallbackData) });

            return new InlineKeyboardMarkup(rows);
        }

        public static InlineKeyboardMarkup GetMarkupWithOnlyBackButton(string langCode)
        {
            string buttonText = LocalizationService.Get(langCode, "BackBtn_text");
            return GetMarkupFromMassive(
                (buttonText, "/menu")
            );
        }

        // Неизменяемая клавиатура для меню
        // Fixed keyboard for menus
        public static InlineKeyboardMarkup GetMenuMarkup(string langCode)
        {
            return new InlineKeyboardMarkup(
            new []
            {
                new[] { InlineKeyboardButton.WithCallbackData(LocalizationService.Get(langCode, "ShowFilesBtn_text"), "/files") },
                new[] { InlineKeyboardButton.WithCallbackData(LocalizationService.Get(langCode, "ShowWordBtn_text"), "/get_word") },
                new[] { InlineKeyboardButton.WithCallbackData(LocalizationService.Get(langCode, "ShowUserDataBtn_text"), "/show_user_data") },
                new[] { InlineKeyboardButton.WithCallbackData(LocalizationService.Get(langCode, "WhatThisBotCanDoBtn_text"), "/about_bot") },
                new[] { InlineKeyboardButton.WithCallbackData(LocalizationService.Get(langCode, "ContactMeBtn_text"), "/contact") },
                new[] { InlineKeyboardButton.WithCallbackData(LocalizationService.Get(langCode, "PlayMiniGameBtn_text"), "/game")},
                new[] { InlineKeyboardButton.WithCallbackData(LocalizationService.Get(langCode, "ChangeLanguageBtn_text"), "/change_lang" )}
            }
            );
        }

        public static ReplyKeyboardMarkup GetGameStartReplyMarkup(string langCode)
        {
            // Берём URL игры из переменных окружения
            if (string.IsNullOrEmpty(gameUrl))
            {
                gameUrl = Environment.GetEnvironmentVariable("GITHUB_PAGE");
            }

            // Если URL нет, возвращаем пустую клавиатуру
            if (string.IsNullOrEmpty(gameUrl))
            {
                return new ReplyKeyboardMarkup(new[] { new KeyboardButton(LocalizationService.Get(langCode, "BackBtn_text")) })
                {
                    ResizeKeyboard = true
                };
            }

            // Создаем обычную клавиатурную кнопку с Mini App
            var replyMarkup = new ReplyKeyboardMarkup(new[]
            {
                new[] { KeyboardButton.WithWebApp(LocalizationService.Get(langCode, "PlayFlappyBirdBtn_text"), new WebAppInfo { Url = gameUrl }) },
                new[] { new KeyboardButton("/menu") } // Обычная текстовая кнопка возврата
            });
            
            return replyMarkup;
        }
    }
}
