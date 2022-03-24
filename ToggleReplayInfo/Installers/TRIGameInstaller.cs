using ToggleReplayInfo.HarmonyPatches.Patches;
using ToggleReplayInfo.Manager;
using ToggleReplayInfo.Models;
using Zenject;

namespace ToggleReplayInfo.Installers
{
    internal class TRIGameInstaller : Installer<TRIGameInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ReplayMetaDataWrapper>().FromInstance(PatchScoreDetailView.ReplayMetaData).AsSingle();
            Container.BindInterfacesAndSelfTo<ReplayTextManager>().AsSingle();
        } 
        
    }
}
