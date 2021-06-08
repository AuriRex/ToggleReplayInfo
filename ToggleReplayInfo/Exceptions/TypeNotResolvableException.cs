using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToggleReplayInfo.Exceptions
{
    public class TypeNotResolvableException : Exception
    {
        public TypeNotResolvableException()
        {
        }

        public TypeNotResolvableException(string message)
            : base(message)
        {
        }

        public TypeNotResolvableException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
