using ToggleReplayInfo.Configuration;
using ToggleReplayInfo.Manager;
using Zenject;

namespace ToggleReplayInfo.Installers
{
    internal class TRICoreInstaller : Installer<TRICoreInstaller>
    {
        private readonly PluginConfig _pluginConfig;

        internal TRICoreInstaller(PluginConfig pluginConfig)
        {
            _pluginConfig = pluginConfig;
        }

        public override void InstallBindings()
        {
            Container.Bind<PluginConfig>().FromInstance(_pluginConfig).AsSingle();
            Container.BindInterfacesAndSelfTo<ScoreSaberTypeManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<HarmonyPatches.HarmonyPatcher>().AsSingle();
        }
    }
}
