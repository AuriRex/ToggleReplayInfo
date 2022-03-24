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

        internal static event Action OnLevelCompletedWatchReplayButtonClicked;

        static MethodBase TargetMethod()
        {
            return OnWatchReplayButtonClickedMB;
        }

        static void Postfix()
        {
            OnLevelCompletedWatchReplayButtonClicked?.Invoke();
        }
    }
#pragma warning restore IDE0051 // Remove unused private members
}
