namespace DemoTGBotOnCSharp.FileStorage.Exception
{
    internal class FileExistsException : System.Exception
    {
        public FileExistsException() : base("Файл с таким именем уже существует") { }
        public FileExistsException(string message) : base (message) {}
    }
}
