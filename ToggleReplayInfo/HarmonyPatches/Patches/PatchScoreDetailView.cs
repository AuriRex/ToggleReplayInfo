using HarmonyLib;
using System.Reflection;
using ToggleReplayInfo.Models;

namespace ToggleReplayInfo.HarmonyPatches.Patches
{
#pragma warning disable IDE0051 // Remove unused private members
    [HarmonyPatch]
    internal class PatchScoreDetailView
    {
        internal static PropertyInfo _currentScorePI;
        internal static MethodBase ReplayButtonClickedMB;

        private static ReplayMetaDataWrapper _replayMetaData;
        internal static ReplayMetaDataWrapper ReplayMetaData
        {
            get
            {
                var val = _replayMetaData;
                _replayMetaData = null;
                return val;
            }
            private set
            {
                _replayMetaData = value;
            }
        }

        static MethodBase TargetMethod()
        {
            return ReplayButtonClickedMB;
        }

        static void Postfix(object __instance)
        {
            Logger.Log.Debug("Replay was started.");

            var val = _currentScorePI.GetValue(__instance);
            if(val != null)
            {
                ReplayMetaData = new ReplayMetadataContainerWrapper(val)?.ReplayMetaData;
            }
        }

    }
#pragma warning restore IDE0051 // Remove unused private members
}
