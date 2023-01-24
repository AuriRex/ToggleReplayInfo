using HarmonyLib;
using System.Reflection;
using ToggleReplayInfo.Models;

namespace ToggleReplayInfo.HarmonyPatches.Patches
{
    [HarmonyPatch]
    internal class PatchScoreDetailView
    {
        internal static PropertyInfo PI__currentScore;
        internal static MethodBase MB_ReplayButtonClicked;

        private static ReplayScoreWrapper _replayMetaData;
        internal static ReplayScoreWrapper Score
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

        public static MethodBase TargetMethod()
        {
            return MB_ReplayButtonClicked;
        }

        public static void Postfix(object __instance)
        {
            Logger.Log.Debug("Replay was started.");

            var val = PI__currentScore.GetValue(__instance);
            if(val != null)
            {
                Score = new ReplayScoreMapWrapper(val)?.Score;
            }
        }
    }
}
