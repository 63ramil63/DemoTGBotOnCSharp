using DemoTGBotOnCSharp.Database.Interface;
using MySqlConnector;

namespace DemoTGBotOnCSharp.Database
{
    internal class DatabaseCreator : IDatabaseCreator
    {
        private readonly string connectionString;

        public DatabaseCreator()
        {
            connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? throw new InvalidOperationException("Строка подключения к БД не найдена в .env");
        }

        public async Task CreateDatabaseAsync()
        {
            using var connection = new MySqlConnection(connectionString);

            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            string createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    ChatId BIGINT PRIMARY KEY,
                    FirstName VARCHAR(255),
                    LastName VARCHAR(255),
                    UserName VARCHAR(255),
                    IsAdmin BOOLEAN DEFAULT FALSE,
                    Word VARCHAR(255)
                );";

            await CreateTable(connection, createUsersTable);

            string createFileRepositoryTable = @"
                CREATE TABLE IF NOT EXISTS Files(
                    ChatId BIGINT,
                    FilePath VARCHAR(255)
                );";

            await CreateTable(connection, createFileRepositoryTable);

            string createGameScoreTable = @"
                CREATE TABLE IF NOT EXISTS UserScore(
                    ChatId BIGINT PRIMARY KEY,
                    Score INT DEFAULT 0,
                    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
                );";

            await CreateTable(connection, createGameScoreTable);


            await connection.CloseAsync();

            Console.WriteLine("[БД] Подключение установлено. Таблица Users и Files проверены/созданы");
        }

        private async Task CreateTable(MySqlConnection connection, string sqlQuery)
        {
            using var command = new MySqlCommand(sqlQuery, connection);

            await command.ExecuteNonQueryAsync();
        }
    }
}
