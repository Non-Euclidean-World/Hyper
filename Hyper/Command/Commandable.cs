namespace Hyper.Command
{
    public abstract class Commandable
    {
        private const string CommandNotFoundMessage = "Command does not exist";

        public void Command(string[] arguments)
        {
            var key = arguments[0];
            var args = arguments.Skip(1).ToArray();

            switch (key)
            {
                case "get":
                    GetCommand(args);
                    break;
                case "set":
                    SetCommand(args);
                    break;
                default:
                    CommandNotFound();
                    break;
            }
        }

        protected virtual void SetCommand(string[] args)
        {
            CommandNotFound();
        }

        protected virtual void GetCommand(string[] args)
        {
            CommandNotFound();
        }

        protected static void CommandNotFound()
        {
            throw new CommandException(CommandNotFoundMessage);
        }
    }
}
