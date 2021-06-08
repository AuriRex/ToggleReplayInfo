using ToggleReplayInfo.Configuration;
using ToggleReplayInfo.Manager;
using ToggleReplayInfo.TypeReflection;
using Zenject;

namespace ToggleReplayInfo.Installers
{
    internal class TRICoreInstaller : Installer<TRICoreInstaller>
    {
        private PluginConfig _pluginConfig;

        internal TRICoreInstaller(PluginConfig pluginConfig)
        {
            _pluginConfig = pluginConfig;
        }

        public override void InstallBindings()
        {
            Container.Bind<PluginConfig>().FromInstance(_pluginConfig).AsSingle();
            Container.Bind<TypeDefinitionManager>().ToSelf().AsSingle();
            Container.BindInterfacesAndSelfTo<ScoreSaberTypeManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<HarmonyPatches.HarmonyPatcher>().AsSingle();
            Container.BindInterfacesAndSelfTo<ScoreSaberStaticBlobManager>().AsSingle();
        }
    }
}
