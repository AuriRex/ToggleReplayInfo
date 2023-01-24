using HarmonyLib;
using System.Reflection;

namespace ToggleReplayInfo.HarmonyPatches.Patches
{
    [HarmonyPatch]
    internal class PatchResultsViewReplayButtonController
    {
        internal static MethodBase MB_OnWatchReplayButtonClicked;

        private static bool _replayButtonClicked = false;
        internal static bool LevelCompletedWatchReplayButtonWasClicked
        {
            get
            {
                var val = _replayButtonClicked;
                _replayButtonClicked = false;
                return val;
            }
            private set
            {
                _replayButtonClicked = value;
            }
        }

        public static MethodBase TargetMethod()
        {
            return MB_OnWatchReplayButtonClicked;
        }

        public static void Postfix()
        {
            LevelCompletedWatchReplayButtonWasClicked = true;
        }
    }
}
