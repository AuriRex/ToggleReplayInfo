using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToggleReplayInfo.Models;

namespace ToggleReplayInfo.HarmonyPatches.Patches
{
    [HarmonyPatch]
    internal class PatchScoreSaberLeaderboardView
    {

        internal static MethodBase ReplayButtonClickedMB;

        static MethodBase TargetMethod()
        {
            return ReplayButtonClickedMB;
        }

        static void Postfix()
        {
            Logger.log.Debug("Replay was started.");
        }

    }

    [HarmonyPatch]
    internal class PatchScoreSaberLeaderboardView_InfoButton
    {

        internal static MethodBase InfoButtonClickedMB;
        internal static int latestIndex = 0;

        internal static ReplayMetaDataWrapper CurrentMetaData { get; private set; }

        static MethodBase TargetMethod()
        {
            return InfoButtonClickedMB;
        }

        static void Postfix(ref int index)
        {
            latestIndex = index;
            Logger.log.Debug($"Info button was pressed: {index}.");
            var scoresPage = LeaderboardPageScoresContainerWrapper.GetInstanceFromStaticDownloader();

            if(scoresPage.Scores.Count > index)
            {
                CurrentMetaData = new ReplayMetaDataWrapper(scoresPage.Scores[index]);
                return;
            }
            CurrentMetaData = null;
        }

    }
}
