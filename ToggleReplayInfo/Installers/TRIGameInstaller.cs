using ToggleReplayInfo.Configuration;
using ToggleReplayInfo.HarmonyPatches.Patches;
using ToggleReplayInfo.Manager;
using ToggleReplayInfo.Models;
using Zenject;

namespace ToggleReplayInfo.Installers
{
    internal class TRIGameInstaller : Installer<TRIGameInstaller>
    {
        private readonly PluginConfig _pluginConfig;
        private readonly ScoreSaberTypeManager _scoreSaberTypeManager;

        internal TRIGameInstaller(PluginConfig pluginConfig, ScoreSaberTypeManager sstm)
        {
            _pluginConfig = pluginConfig;
            _scoreSaberTypeManager = sstm;
        }

        public override void InstallBindings()
        {
            if (_scoreSaberTypeManager.HasErrorsOnInit) return;
            if (!_pluginConfig.Enabled) return;

            Container.Bind<bool>().WithId("FromResultsViewReplayButton").FromInstance(PatchLevelResultsReplayStartButtonClicker.LevelCompletedWatchReplayButtonWasClicked).AsSingle();
            Container.BindInterfacesAndSelfTo<ReplayMetaDataWrapper>().FromInstance(PatchScoreDetailView.ReplayMetaData).AsSingle();
            Container.BindInterfacesAndSelfTo<ReplayTextManager>().AsSingle();
        }
    }
}
