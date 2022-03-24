using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using ToggleReplayInfo.Installers;
using ToggleReplayInfo.Manager;
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
            Logger.Log = logger;

            zenjector.Install<TRICoreInstaller>(Location.App, conf.Generated<Configuration.PluginConfig>());
            zenjector.Install<TRIMenuInstaller>(Location.Menu);
            zenjector.Install<TRIGameInstaller>(Location.Singleplayer);
        }

        internal static ScoreSaberTypeManager SSTM { get; set; }
    }
}
