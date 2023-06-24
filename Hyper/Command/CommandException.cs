namespace Hyper.Command
{
    internal class CommandException : Exception
    {
        public CommandException() { }

        public CommandException(string message) : base(message)
        {
        }
    }
}
