using ToggleReplayInfo.Configuration;
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

        public override void InstallBindings() => Container.Bind<PluginConfig>().FromInstance(_pluginConfig).AsSingle();
    }
}
