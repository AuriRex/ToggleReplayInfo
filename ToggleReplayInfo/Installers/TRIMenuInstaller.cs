using ToggleReplayInfo.Manager;
using ToggleReplayInfo.UI;
using Zenject;

namespace ToggleReplayInfo.Installers
{
    class TRIMenuInstaller : Installer<TRIMenuInstaller>
    {
        public override void InstallBindings()
        {
            Logger.log.Error("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            if (IPA.Loader.PluginManager.GetPluginFromId("BeatSaberMarkupLanguage") != null)
            {
                Logger.log.Error("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");
                Container.BindInterfacesAndSelfTo<ModifierHost>().AsSingle();
                Container.BindInterfacesAndSelfTo<MenuUIManager>().AsSingle();
            }
        }
    }
}
