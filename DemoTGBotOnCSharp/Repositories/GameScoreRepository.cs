using DemoTGBotOnCSharp.Repositories.DTO;
using DemoTGBotOnCSharp.Repositories.Interface;
using MySqlConnector;

namespace DemoTGBotOnCSharp.Repositories
{
    
    internal class GameScoreRepository : IGameScoreRepository
    {
        // Потокобезопасное создание singleton при помощи Lazy
        private static readonly Lazy<GameScoreRepository> _instance =
            new Lazy<GameScoreRepository>(() => new GameScoreRepository());

        public static GameScoreRepository Instance => _instance.Value;
        private readonly string connectionString;
        private string SELECT_SCORE_BY_ID = "SELECT Score, UpdatedAt FROM UserScore WHERE ChatId = @chatId";
        private string SAVE_GAME_SCORE = "INSERT INTO UserScore (ChatId, Score) VALUES (@chatId, @score);";
        // @ Позволяет переносить строки без использования + 
        private string GET_PLAYER_RANK = @"
            SELECT player_rank
            FROM (
                SELECT ChatId,
                            ROW_NUMBER() OVER (ORDER BY Score DESC, UpdatedAt ASC) as player_rank
                FROM UserScore
            ) as leaderboard
            WHERE ChatId = @chatId;
            ";

        private GameScoreRepository()
        {
            connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? throw new InvalidOperationException("Database connection string not found");
        }
        public async Task<GameScoreDTO> GetGameScoreById(long chatId)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(SELECT_SCORE_BY_ID, connection);

            // Безопасно добавляем параметры
            command.Parameters.AddWithValue("@chatId", chatId);

            // ExecuteReaderAsync читает строки асинхронно, не блокируя бота
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                int scoreOrdinal = reader.GetOrdinal("Score");
                int updatedAtOrdinal = reader.GetOrdinal("UpdatedAt");

                var gameScoreDTO = new GameScoreDTO
                {
                    chatId = chatId,
                    score = reader.IsDBNull(scoreOrdinal) ? 0 : reader.GetInt32(scoreOrdinal),
                    updatedAt = reader.IsDBNull(updatedAtOrdinal) ? null : reader.GetDateTime(updatedAtOrdinal) 
                };

                await connection.CloseAsync();

                return gameScoreDTO;
            }

            return null;
        }

        public async Task SaveGameScore(long chatId, int score)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(SAVE_GAME_SCORE, connection);

            // Безопасно добавляем параметры
            command.Parameters.AddWithValue("@chatId", chatId);
            command.Parameters.AddWithValue("@score", score);

            await command.ExecuteNonQueryAsync();        
        }

        public async Task<int?> GetPlayerRankAsync(long chatId)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(GET_PLAYER_RANK, connection);

            // Безопасно добавляем параметры
            command.Parameters.AddWithValue("@chatId", chatId);
            
            // Получаем скалярное значение (одно число - место)
            var result = await command.ExecuteScalarAsync();

            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }

            return null;
        }
    }
}
