using DemoTGBotOnCSharp.Repositories.Interface;
using MySqlConnector;

namespace DemoTGBotOnCSharp.Repositories
{
    internal class FileRepository : IFileRepository
    {
        // Потокобезопасное создание singleton при помощи Lazy
        // Thread-safe singleton creation using Lazy
        private static readonly Lazy<FileRepository> _instance =
            new Lazy<FileRepository>(() => new FileRepository());

        public static FileRepository Instance => _instance.Value;

        private readonly string connectionString;
        private string GET_FILES_BY_ID = "SELECT FilePath FROM Files WHERE ChatId = @chatId";
        private string ADD_NEW_FILE = "INSERT INTO Files (ChatId, FilePath) values (@chatId, @filePath)";
        private string DELETE_FILE_BY_FILE_PATH = "DELETE FROM Files WHERE FilePath=@filePath";
        public FileRepository()
        {
            connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? throw new InvalidOperationException("Database connection string not found");
        }

        public async Task AddNewFile(string filePath, long chatId)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(ADD_NEW_FILE, connection);

            // Безопасно добавляем параметры
            // Safely adding parameters
            command.Parameters.AddWithValue("@chatId", chatId);
            command.Parameters.AddWithValue("@filePath", filePath);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<string>> GetFilesByUserId(long chatId)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(GET_FILES_BY_ID, connection);

            // Безопасно добавляем параметры
            // Safely adding parameters
            command.Parameters.AddWithValue("@chatId", chatId);

            using var reader = await command.ExecuteReaderAsync();
            List<string> files = new List<string>();
            while (await reader.ReadAsync())
            {
                // Получаем номер столбца
                // Get the column number
                int filePathOrdinal = reader.GetOrdinal("FilePath");

                if (!reader.IsDBNull(filePathOrdinal))
                {
                    files.Add(reader.GetString(filePathOrdinal));
                }
            }
            return files;
        }

        public async Task DeleteFileByFilePath(string filePath)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(DELETE_FILE_BY_FILE_PATH, connection);

            // Безопасно добавляем параметры
            // Safely adding parameters
            command.Parameters.AddWithValue("@filePath", filePath);

            await command.ExecuteNonQueryAsync();
        }
    }
}
