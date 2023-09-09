namespace Common.Command;

public class CommandException : Exception
{
    public CommandException() { }

    public CommandException(string message) : base(message) { }
}
