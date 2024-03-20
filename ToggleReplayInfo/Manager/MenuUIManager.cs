using BeatSaberMarkupLanguage.GameplaySetup;
using System;
using ToggleReplayInfo.UI;
using Zenject;

namespace ToggleReplayInfo.Manager
{
    internal class MenuUIManager : IInitializable, IDisposable
    {
        private readonly ModifierHost _modifierHost;

        [Inject]
        public MenuUIManager(ModifierHost modifierHost)
        {
            _modifierHost = modifierHost;
        }

        public void Initialize() => GameplaySetup.instance?.AddTab("Replay Text", "ToggleReplayInfo.UI.Views.modifier-view.bsml", _modifierHost);

        public void Dispose()
        {
            GameplaySetup.instance.RemoveTab("Replay Text");
        }
    }
}
