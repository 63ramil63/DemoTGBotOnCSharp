namespace DemoTGBotOnCSharp.Handler.Exceptions
{
    internal class NullUpdateException : Exception
    {
        public NullUpdateException() : base("Update is null") { }
        public NullUpdateException(string message) : base(message) { }
    }
}
