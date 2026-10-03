using DemoTGBotOnCSharp.FileStorage.Interface;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DemoTGBotOnCSharp.FileStorage
{
    internal class FileController : IFileController
    {
        private static readonly Lazy<FileController> _instance =
            new Lazy<FileController>(() => new FileController());
        public static FileController Instance => _instance.Value;
        private readonly string baseStoragePath;
        private List<string> allowedExtensions;

        private FileController()
        {
            baseStoragePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserDirectory");
            if (allowedExtensions == null)
            {
                var envVariable = Environment.GetEnvironmentVariable("ALLOWED_EXTENSIONS");
                if (string.IsNullOrEmpty(envVariable))
                {
                    throw new InvalidOperationException("String with extensions not found in .env file");
                }

                allowedExtensions = envVariable.Split(',')
                    .Select(ext => ext.Trim().ToLower())
                    .ToList();
            }
        }
        public async Task<string> SaveTelegramFileAsync(ITelegramBotClient botClient, Document document, long chatId)
        {
            if (string.IsNullOrEmpty(document.FileName))
            {
                throw new InvalidOperationException("File has no name, unable to get its extension.");
            }

            string fileExtension = Path.GetExtension(document.FileName).TrimStart('.').ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException($"Extension '.{fileExtension}' not allowed.");
            }

            string userFolder = Path.Combine(baseStoragePath, chatId.ToString());
            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
            }

            string safeFileName = document.FileName;
            string finalFilePath = Path.Combine(userFolder, safeFileName);
            if (File.Exists(finalFilePath))
            {
                throw new InvalidOperationException($"File with name - {safeFileName} already exists in path - {userFolder}");
            }

            var fileInfo = await botClient.GetFile(document.FileId);

            if (string.IsNullOrEmpty(fileInfo.FilePath))
            {
                throw new InvalidOperationException("Failed to get file download path from Telegram API.");
            }

            // Открываем асинхронный поток записи на диск
            using (var fileStream = new FileStream(finalFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
            {
                // Скачиваем файл с сервером Telegram нарпямую на пк
                await botClient.DownloadFile(fileInfo.FilePath, fileStream);
            }

            return safeFileName; // Возвращаем путь чтобы записать его в БД
        }

        public async Task RemoveFileFromDirectory(string fileName, long chatId)
        {
            string userFolder = Path.Combine(baseStoragePath, chatId.ToString());
            if (!Directory.Exists(userFolder))
            {
                throw new InvalidOperationException($"No folder : {userFolder}");
            }

            string filePath = Path.Combine(userFolder, fileName);
            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException($"File doesn't exists by path - {filePath}");
            }

            await Task.Run(() => File.Delete(filePath));
        }

        public async Task<FileStream> GetFile(string fileName, long chatId)
        {
            string filePath = Path.Combine(baseStoragePath, chatId.ToString(), fileName);

            if (!File.Exists(filePath)) throw new InvalidOperationException("File not found");

            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        }
    }
}
