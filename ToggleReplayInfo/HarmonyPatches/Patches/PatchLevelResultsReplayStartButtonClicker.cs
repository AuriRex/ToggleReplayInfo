using HarmonyLib;
using System;
using System.Reflection;

namespace ToggleReplayInfo.HarmonyPatches.Patches
{
    [HarmonyPatch]
    internal class PatchLevelResultsReplayStartButtonClicker
    {
        internal static MethodBase OnWatchReplayButtonClickedMB;

        internal static event Action onLevelCompletedWatchReplayButtonClicked;

        static MethodBase TargetMethod()
        {
            return OnWatchReplayButtonClickedMB;
        }

        static void Postfix()
        {
            onLevelCompletedWatchReplayButtonClicked?.Invoke();
        }
    }
}
