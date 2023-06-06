using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
