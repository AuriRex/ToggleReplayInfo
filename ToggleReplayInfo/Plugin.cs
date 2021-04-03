using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using System.Reflection;
using ToggleReplayInfo.Installers;
using IPALogger = IPA.Logging.Logger;

namespace ToggleReplayInfo
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        private Harmony _harmony;
        public const string HARMONYID = "com.aurirex.togglereplayinfo";

        [Init]
        public void InitWithConfig(IPALogger logger, Config conf, Zenjector zenjector)
        {
            Logger.log = logger;
            _harmony = new Harmony(HARMONYID);
            zenjector.OnApp<TRICoreInstaller>().WithParameters(conf.Generated<Configuration.PluginConfig>());
            zenjector.OnMenu<TRIMenuInstaller>();
            zenjector.OnGame<TRIGameInstaller>().ShortCircuitForTutorial().ShortCircuitForCampaign().ShortCircuitForMultiplayer();
        }

        [OnEnable]
        public void OnApplicationStart() => _harmony.PatchAll(Assembly.GetExecutingAssembly());

        [OnDisable]
        public void OnApplicationQuit() => _harmony.UnpatchAll(HARMONYID);
    }
}
