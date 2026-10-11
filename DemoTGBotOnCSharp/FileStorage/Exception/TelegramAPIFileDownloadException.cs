namespace DemoTGBotOnCSharp.FileStorage.Exception
{
    internal class TelegramAPIFileDownloadException : System.Exception
    {
        public TelegramAPIFileDownloadException() : base("Telegram API error") { }
        public TelegramAPIFileDownloadException(string message) : base(message) { }
    }
}
