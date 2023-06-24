namespace Hyper.Command
{
    public abstract class Commandable
    {
        public void Command(string[] argumants)
        {
            var key = argumants[0];
            var args = argumants.Skip(1).ToArray();

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
