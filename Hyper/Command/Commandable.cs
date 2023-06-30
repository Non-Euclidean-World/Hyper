namespace Hyper.Command
{
    public abstract class Commandable
    {
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
            }
        }

        protected virtual void SetComamnd(string[] args)
        {
            throw new CommandException("Command does not exist");
        }

        protected virtual void GetComamnd(string[] args)
        {
            throw new CommandException("Command does not exist");
        }
    }
}
