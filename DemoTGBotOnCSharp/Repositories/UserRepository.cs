using DemoTGBotOnCSharp.Repositories.DTO;
using DemoTGBotOnCSharp.Repositories.Interface;
using MySqlConnector;
using System.Runtime.Intrinsics.X86;
using Telegram.Bot.Types;

namespace DemoTGBotOnCSharp.Repositories
{
    internal class UserRepository : IUserRepository
    {
        // Потокобезопасное создание singleton при помощи Lazy
        // Thread-safe singleton creation using Lazy
        private static readonly Lazy<UserRepository> _instance =
            new Lazy<UserRepository>(() => new UserRepository());

        public static UserRepository Instance => _instance.Value;
        private readonly string connectionString;
        private string SELECT_USER_BY_ID = "SELECT ChatId, FirstName, LastName, UserName, IsAdmin, Word FROM Users WHERE ChatId = @chatId";
        private string SELECT_WORD_BY_ID = "SELECT Word FROM Users WHERE ChatId = @chatId";
        private string ADD_NEW_USER = "INSERT INTO Users (ChatId, FirstName, LastName, UserName) VALUES (@chatId, @f_name, @l_name, @userName)";
        private string SET_USER_WORD = "UPDATE Users SET Word = @word WHERE ChatId = @chatId";
        public UserRepository()
        {
            connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? throw new InvalidOperationException("Database connection string not found");
        }
        public async Task<UserDTO?> GetUserById(long? chatId)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(SELECT_USER_BY_ID, connection);

            // Безопасно добавляем параметры, защищаясь от SQL инъекций
            // Adding parameters safely, while protecting against SQL injection
            command.Parameters.AddWithValue("@chatId", chatId);

            // ExecuteReaderAsync читает строки асинхронно, не блокируя бота
            // ExecuteReaderAsync reads rows asynchronously, without blocking the bot
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                // Получаем номер столбца
                // Get the column number
                int firstNameOrdinal = reader.GetOrdinal("FirstName");
                int lastNameOrdinal = reader.GetOrdinal("LastName");
                int userNameOrdinal = reader.GetOrdinal("UserName");
                int wordOrdinal = reader.GetOrdinal("Word");
                int isAdminOrdinal = reader.GetOrdinal("IsAdmin");

                var userDTO = new UserDTO
                {
                    chatId = chatId.Value,
                    f_name = reader.IsDBNull(firstNameOrdinal) ? null : reader.GetString(firstNameOrdinal),
                    l_name = reader.IsDBNull(lastNameOrdinal) ? null : reader.GetString(lastNameOrdinal),
                    username = reader.IsDBNull(userNameOrdinal) ? null : reader.GetString(userNameOrdinal),
                    isAdmin = reader.GetBoolean(isAdminOrdinal),
                    word = reader.IsDBNull(wordOrdinal) ? null : reader.GetString(wordOrdinal)
                };

                await connection.CloseAsync();

                return userDTO;
            }
            // Returns null if a user with the specified ID was not found.
            // Возврат null если так и не нашли пользователя с указанным Id
            return null; 
        }
        public async Task<string?> GetUserWordById(long? chatId)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(SELECT_WORD_BY_ID, connection);

            // Безопасно добавляем параметры, защищаясь от SQL инъекций
            // Adding parameters safely, while protecting against SQL injection
            command.Parameters.AddWithValue("@chatId", chatId);

            // Так как мы запрашиваем ровно одно поле (Word), быстрее использовать ExecuteScalarAsync
            // Since we are requesting exactly one field(Word), it is faster to use ExecuteScalarAsync
            Object? result = await command.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
            {
                return null;
            }
            return result.ToString();
        }

        public async Task AddNewUser(long? id, string? f_name, string? l_name, string? username)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(ADD_NEW_USER, connection);
            // Безопасно добавляем параметры
            // Safely adding parameters
            command.Parameters.AddWithValue("@chatId", id);
            command.Parameters.AddWithValue("@f_name", (object?)f_name ?? DBNull.Value);
            command.Parameters.AddWithValue("@l_name", (object?)l_name ?? DBNull.Value);
            command.Parameters.AddWithValue("@userName", (object?)username ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task SetUserWord(long? id, string? word)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(SET_USER_WORD, connection);
            // Безопасно добавляем параметры
            // Safely adding parameters
            command.Parameters.AddWithValue("@chatId", id);
            command.Parameters.AddWithValue("@word", word);

            await command.ExecuteNonQueryAsync();
        }
    }
}
