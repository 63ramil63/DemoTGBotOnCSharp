using DemoTGBotOnCSharp.Handler.SubHandler.Interface;
using DemoTGBotOnCSharp.MarkupBuilder;
using DemoTGBotOnCSharp.Repositories;
using DemoTGBotOnCSharp.Handler.Exceptions;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Text.Json.Serialization;

namespace DemoTGBotOnCSharp.Handler.SubHandler
{
    internal class WebAppDataHandler : IUpdateSubHandler
    {
        public async Task HandleAsync(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            var message = update.Message;
            if (message == null) throw new NullUpdateException("Message is null");

            var webAppData = message.WebAppData;
            if (webAppData == null) throw new NullUpdateException("Web app data from message is null");

            long chatId = message.Chat.Id;
            if (chatId == 0) throw new NullUpdateException("ChatId equals 0");

            string rawJson = webAppData.Data;

            try
            {
                // Десериализуем данные из JSON в C# объект
                // Deserializing data from JSON into a C# object
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<WebAppGameResult>(rawJson, options);

                if (result != null)
                {
                    // Записываем результат напрямую в базу данных
                    // Write result directly to the database
                    await GameScoreRepository.Instance.SaveGameScore(chatId, result.Score);
                    int? playerRank = await GameScoreRepository.Instance.GetPlayerRankAsync(chatId);

                    string rankText = playerRank.HasValue ? $"{playerRank.Value}" : "Not ranked yet";

                    string m_text = $"🎮 Congratulations, your score is : {result.Score}\n" +
                        $"🏆 Your rank is {rankText}";

                    await botClient.SendMessage(
                        chatId: chatId,
                        text: m_text,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: InlineMarkupBuilder.GetMenuMarkup(),
                        cancellationToken: cts
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving the record: {ex.Message}");
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "❌ Failed to save your record",
                    replyMarkup: InlineMarkupBuilder.GetMarkupWithOneButton("Back to menu", "/menu"),
                    cancellationToken: cts
                );
            }
        }
    }

    public class WebAppGameResult
    {
        [JsonPropertyName("score")]
        public int Score { get; set; }
    }
}
