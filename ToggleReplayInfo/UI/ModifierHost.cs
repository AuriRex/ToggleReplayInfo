using BeatSaberMarkupLanguage.Attributes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToggleReplayInfo.Configuration;

namespace ToggleReplayInfo.UI
{
    public class ModifierHost : INotifyPropertyChanged
    {
        private PluginConfig _pluginConfig;

        public ModifierHost(PluginConfig pluginConfig)
        {
            _pluginConfig = pluginConfig;
        }

        [UIValue("disable-replay-text")]
        protected bool HideReplayInfo
        {
            get => _pluginConfig.HideReplayInfo;
            set
            {
                _pluginConfig.HideReplayInfo = value;
                NotifyPropertyChanged(nameof(HideReplayInfo));
            }
        }

#nullable enable annotations
        public event PropertyChangedEventHandler? PropertyChanged;
#nullable restore annotations

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch { }
        }
    }
}