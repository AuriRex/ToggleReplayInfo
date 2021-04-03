using ToggleReplayInfo.Manager;
using ToggleReplayInfo.UI;
using Zenject;

namespace ToggleReplayInfo.Installers
{
    class TRIMenuInstaller : Installer<TRIMenuInstaller>
    {
        public override void InstallBindings()
        {
            if (IPA.Loader.PluginManager.GetPluginFromId("BeatSaberMarkupLanguage") != null)
            {
                Container.BindInterfacesAndSelfTo<ModifierHost>().AsSingle();
                Container.BindInterfacesAndSelfTo<MenuUIManager>().AsSingle();
            }
        }
    }
}
