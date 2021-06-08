using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToggleReplayInfo.Exceptions
{
    public class ReplayTextInitializationException : Exception
    {
        public ReplayTextInitializationException()
        {
        }

        public ReplayTextInitializationException(string message)
            : base(message)
        {
        }

        public ReplayTextInitializationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
