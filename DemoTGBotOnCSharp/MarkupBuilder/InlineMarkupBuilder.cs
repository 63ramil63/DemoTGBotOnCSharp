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

            // Converting an active list into a button with a suffix

            var allButtons = buttons
                .Select(btn => InlineKeyboardButton.WithCallbackData(btn, btn + uniqSuffix));

            // Split this list into rows and convert it into a list of lists of buttons.
            var rows = allButtons.Chunk(buttonsInRow).ToList();

            var backButton = InlineKeyboardButton.WithCallbackData("Back to menu", "/menu");

            // Add a "Back" button as a separate (last) row at the end of the keyboard.
            rows.Add(new[] { backButton });

            // Return ready markup
            return new InlineKeyboardMarkup(rows);
        }

        public static InlineKeyboardMarkup GetMarkupWithOneButton(string buttonName, string callbackData)
        {
            return new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData(buttonName, callbackData) });
        }

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

        public static InlineKeyboardMarkup GetGameStartMarkup()
        {
            if (string.IsNullOrEmpty(gameUrl))
            {
                gameUrl = Environment.GetEnvironmentVariable("GITHUB_PAGE");
            }
            if (string.IsNullOrEmpty(gameUrl))
            {
                return new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Back to menu", "/menu"));
            }

            var markup = new InlineKeyboardMarkup(
            new[]
            {       
                new[] { InlineKeyboardButton.WithWebApp("Play game", new WebAppInfo { Url = gameUrl }) },
                new[] { InlineKeyboardButton.WithCallbackData("Back to menu", "/menu") }
            }
            );

            return markup;
        }
    }
}
