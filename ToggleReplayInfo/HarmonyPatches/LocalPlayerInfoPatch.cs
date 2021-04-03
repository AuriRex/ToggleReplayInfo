using HarmonyLib;
using ScoreSaber.Core.Data;
using System;

namespace ToggleReplayInfo.HarmonyPatches
{
    [HarmonyPatch(typeof(LocalPlayerInfo))]
    [HarmonyPatch(MethodType.Constructor, new Type[] { typeof(string), typeof(string), typeof(string) })]
    internal class LocalPlayerInfoPatch
    {
        public static LocalPlayerInfo localPlayerInfo;
        static void Postfix(ref LocalPlayerInfo __instance)
        {
            Logger.log.Debug("LocalPlayerInfo set!");
            localPlayerInfo = __instance;
        }
    }
}
