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
                Logger.log.Debug($"Something went wrong because {_scoreSaberTypeManager.GetType().Name} is not ready, not patching!");
                return;
            }

            PreHarmonyPatching();

            Patch();
        }

        private void PreHarmonyPatching()
        {
            Logger.log.Debug("Preparing patches ...");
            PatchReplayMetaData.ReplayMetaDataConstructorMB = _scoreSaberTypeManager.ReplayMetaData.GetConstructor(new Type[] { });

            PatchLevelResultsReplayStartButtonClicker.OnWatchReplayButtonClickedMB = _scoreSaberTypeManager.ScoreSaberLevelResultsViewController
                .GetMethods(AnyBindingFlags).First(x => {
                    BeatSaberMarkupLanguage.Attributes.UIAction attribute = x.GetCustomAttribute(typeof(BeatSaberMarkupLanguage.Attributes.UIAction)) as BeatSaberMarkupLanguage.Attributes.UIAction;
                    if (attribute == null)
                        return false;
                    if(attribute.id.Equals("replay-click"))
                        return true;
                    return false;
                });

            string scoreSaberLeaderboardViewTypeName = "ScoreSaber.UI.Other.ScoreSaberLeaderboardView";

            PatchScoreSaberLeaderboardView.ReplayButtonClickedMB = _scoreSaberTypeManager.ScoreSaberAssembly
                .GetType(scoreSaberLeaderboardViewTypeName).GetMethod("ReplayClicked", AnyBindingFlags);

            PatchScoreSaberLeaderboardView_InfoButton.InfoButtonClickedMB = _scoreSaberTypeManager.ScoreSaberAssembly
                .GetType(scoreSaberLeaderboardViewTypeName).GetMethod("InfoButtonClicked", AnyBindingFlags);
        }

        private void Patch()
        {
            if (!IsPatched)
            {
                Logger.log.Debug("Patching ...");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                IsPatched = true;
            }
        }

        private void Unpatch()
        {
            if (IsPatched)
            {
                Logger.log.Debug("Unpatching ...");
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
