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

            await CreateUserDatabes(connection);
            await CreateFilesDatabase(connection);

            Console.WriteLine("[БД] Подключение установлено. Таблица Users и Files проверены/созданы");
        }

        private async Task CreateUserDatabes(MySqlConnection connection)
        {
            // Открываем соединение асинхронно
            await connection.OpenAsync();

            // SQL-запрос
            string createTableSql = @"
                CREATE TABLE IF NOT EXISTS Users (
                    ChatId BIGINT PRIMARY KEY,
                    FirstName VARCHAR(255),
                    LastName VARCHAR(255),
                    UserName VARCHAR(255),
                    IsAdmin BOOLEAN DEFAULT FALSE,
                    Word VARCHAR(255)
                    );";

            using var command = new MySqlCommand(createTableSql, connection);

            // Выполняем команду без возвращения строк
            await command.ExecuteNonQueryAsync();
        }

        private async Task CreateFilesDatabase(MySqlConnection connection)
        {
            string createFileRepositoryTable = @"
                CREATE TABLE IF NOT EXISTS Files(
                    ChatId BIGINT PRIMARY KEY,
                    FilePath VARCHAR(255)
                    )";

            using var command = new MySqlCommand(createFileRepositoryTable, connection);

            await command.ExecuteNonQueryAsync();
        }
    }
}
