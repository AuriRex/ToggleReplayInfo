using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using ToggleReplayInfo.Installers;
using IPALogger = IPA.Logging.Logger;

namespace ToggleReplayInfo
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public const string HARMONYID = "com.aurirex.togglereplayinfo";

        [Init]
        public void InitWithConfig(IPALogger logger, Config conf, Zenjector zenjector)
        {
            Logger.log = logger;
            zenjector.OnApp<TRICoreInstaller>().WithParameters(conf.Generated<Configuration.PluginConfig>());
            zenjector.OnMenu<TRIMenuInstaller>();
            zenjector.OnGame<TRIGameInstaller>().ShortCircuitForTutorial().ShortCircuitForCampaign().ShortCircuitForMultiplayer();
        }

        [OnEnable]
        public void OnApplicationStart() { }

        [OnDisable]
        public void OnApplicationQuit() { }
    }
}
