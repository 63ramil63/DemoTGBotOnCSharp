using DemoTGBotOnCSharp.Repositories.DTO;

namespace DemoTGBotOnCSharp.Repositories.Interface
{
    internal interface IGameScoreRepository
    {
        Task<GameScoreDTO> GetGameScoreById(long chatId);
        Task SaveGameScore(long chatId, int score);
        Task<int?> GetPlayerRankAsync(long chatId);
    }
}
