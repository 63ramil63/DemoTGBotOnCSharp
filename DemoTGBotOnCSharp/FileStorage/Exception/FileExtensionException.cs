namespace DemoTGBotOnCSharp.FileStorage.Exception
{
    internal class FileExtensionException : System.Exception
    {
        public FileExtensionException() : base("File extension is not allowed") { }
        public FileExtensionException(string message) : base(message) { }
    }
}
