using ToggleReplayInfo.Manager;
using Zenject;

namespace ToggleReplayInfo.Installers
{
    internal class TRIGameInstaller : Installer<TRIGameInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ReplayTextManager>().AsSingle();
        } 
        
    }
}
