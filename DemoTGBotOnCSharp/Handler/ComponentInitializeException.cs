namespace DemoTGBotOnCSharp.Handler
{
    internal class ComponentInitializeException : Exception
    {
        public ComponentInitializeException() : base("Issue on the server (The required component is null)") { }
        public ComponentInitializeException(string message) : base(message) { }
    }
}
