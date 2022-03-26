using HarmonyLib;
using System;
using System.Reflection;

namespace ToggleReplayInfo.HarmonyPatches.Patches
{
#pragma warning disable IDE0051 // Remove unused private members
    [HarmonyPatch]
    internal class PatchLevelResultsReplayStartButtonClicker
    {
        internal static MethodBase OnWatchReplayButtonClickedMB;

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

        static MethodBase TargetMethod()
        {
            return OnWatchReplayButtonClickedMB;
        }

        static void Postfix()
        {
            LevelCompletedWatchReplayButtonWasClicked = true;
        }
    }
#pragma warning restore IDE0051 // Remove unused private members
}
