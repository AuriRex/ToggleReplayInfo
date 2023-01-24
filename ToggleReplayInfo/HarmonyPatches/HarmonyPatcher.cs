using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ToggleReplayInfo.HarmonyPatches.Patches;
using ToggleReplayInfo.Manager;
using Zenject;
using static ToggleReplayInfo.TypeReflection.Utilities;

namespace ToggleReplayInfo.HarmonyPatches
{
    public class HarmonyPatcher : IInitializable, IDisposable
    {
        private readonly ScoreSaberTypeManager _scoreSaberTypeManager;
        private static Harmony _harmony;

        public HarmonyPatcher(ScoreSaberTypeManager scoreSaberTypeManager)
        {
            _scoreSaberTypeManager = scoreSaberTypeManager;
            if(_harmony == null)
                _harmony = new Harmony(Plugin.HARMONYID);
        }

        public bool IsPatched { get; private set; } = false;

        public async void Initialize()
        {
            await Task.Delay(50);

            if(!_scoreSaberTypeManager.IsReady)
            {
                Logger.Log.Debug($"Something went wrong because {_scoreSaberTypeManager.GetType().Name} is not ready, not patching!");
                return;
            }

            PreHarmonyPatching();

            Patch();
        }

        private void PreHarmonyPatching()
        {
            Logger.Log.Debug("Preparing patches ...");

            if(_scoreSaberTypeManager.SSTypes.ResultsViewReplayButtonController != null)
                PatchResultsViewReplayButtonController.MB_OnWatchReplayButtonClicked = _scoreSaberTypeManager.SSTypes.ResultsViewReplayButtonController
                    .GetMethods(AnyBindingFlags).First(x => {
                        BeatSaberMarkupLanguage.Attributes.UIAction attribute = x.GetCustomAttribute(typeof(BeatSaberMarkupLanguage.Attributes.UIAction)) as BeatSaberMarkupLanguage.Attributes.UIAction;
                        if (attribute == null)
                            return false;
                        if(attribute.id.Equals("replay-click"))
                            return true;
                        return false;
                    });

            string ss_ScoreDetailViewTypeName = "ScoreSaber.UI.Elements.Leaderboard.ScoreDetailView";

            var ss_ScoreDetailViewType = _scoreSaberTypeManager.ScoreSaberAssembly
                .GetType(ss_ScoreDetailViewTypeName);

            PatchScoreDetailView.MB_ReplayButtonClicked = ss_ScoreDetailViewType.GetMethod("ReplayClicked", AnyBindingFlags);
            PatchScoreDetailView.PI__currentScore = ss_ScoreDetailViewType.GetProperty("_currentScore", AnyBindingFlags);
        }

        private void Patch()
        {
            if (!IsPatched)
            {
                Logger.Log.Debug("Patching ...");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                IsPatched = true;
            }
        }

        private void Unpatch()
        {
            if (IsPatched)
            {
                Logger.Log.Debug("Unpatching ...");
                _harmony.UnpatchSelf();
                IsPatched = false;
            }
        }

        public void Dispose()
        {
            Unpatch();
        }
    }
}
