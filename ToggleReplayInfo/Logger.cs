using System;
using System.Diagnostics;
using IPALogger = IPA.Logging.Logger;

namespace ToggleReplayInfo
{
    internal static class Logger
    {
        internal static IPALogger Log { get; set; }

        //[Conditional("DEBUG")]
        internal static void Debug(string v)
        {
            Log.Debug(v);
        }
    }
}
