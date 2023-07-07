namespace Hyper.Command
{
    public abstract class Commandable
    {
        private const string _commandNotFound = "Command does not exist";

        public void Command(string[] arguments)
        {
            var key = arguments[0];
            var args = arguments.Skip(1).ToArray();

            switch (key)
            {
                case "get":
                    GetComamnd(args);
                    break;
                case "set":
                    SetComamnd(args);
                    break;
                default:
                    CommandNotFound();
                    break;
            }
        }

        protected virtual void SetComamnd(string[] args)
        {
            CommandNotFound();
        }

        protected virtual void GetComamnd(string[] args)
        {
            CommandNotFound();
        }

        private static void CommandNotFound()
        {
            throw new CommandException(_commandNotFound);
        }
    }
}
