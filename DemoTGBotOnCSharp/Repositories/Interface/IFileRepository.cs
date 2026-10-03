namespace DemoTGBotOnCSharp.Repositories.Interface
{
    internal interface IFileRepository
    {
        Task AddNewFile(string filePath, long chatId);
        Task<List<string>> GetFilesByUserId(long chatId);
        Task DeleteFileByFilePath(string filePath);
    }
}
