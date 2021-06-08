using HarmonyLib;
using System.Reflection;
using ToggleReplayInfo.Models;

namespace ToggleReplayInfo.HarmonyPatches.Patches
{
    [HarmonyPatch]
    internal class PatchReplayMetaData
    {
        internal static MethodBase ReplayMetaDataConstructorMB;

        static MethodBase TargetMethod()
        {
            return ReplayMetaDataConstructorMB;
        }

        static void Postfix(ref object __instance)
        {
            ReplayMetaDataWrapper.Cache.ClearForNextSetOnly();
            ReplayMetaDataWrapper.Cache.Add(new ReplayMetaDataWrapper(__instance));
        }
    }
}
