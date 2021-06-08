using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using System;
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

        [UIValue("enable-mod")]
        protected bool EnableMod
        {
            get => _pluginConfig.Enabled;
            set
            {
                _pluginConfig.Enabled = value;
                NotifyPropertyChanged(nameof(EnableMod));
                OnPluginEnabledStateChanged(value);
            }
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

        [UIValue("position-x")]
        protected string PositionX
        {
            get => _pluginConfig.Position.X.ToString();
            set
            {
                try
                {
                    var val = float.Parse(value);
                    _pluginConfig.Position.X = val;
                    NotifyPropertyChanged(nameof(PositionX));
                }
                catch(Exception _) { }
            }
        }

        [UIValue("position-y")]
        protected string PositionY
        {
            get => _pluginConfig.Position.Y.ToString();
            set
            {
                try
                {
                    var val = float.Parse(value);
                    _pluginConfig.Position.Y = val;
                    NotifyPropertyChanged(nameof(PositionY));
                }
                catch (Exception _) { }
            }
        }

        [UIValue("position-z")]
        protected string PositionZ
        {
            get => _pluginConfig.Position.Z.ToString();
            set
            {
                try
                {
                    var val = float.Parse(value);
                    _pluginConfig.Position.Z = val;
                    NotifyPropertyChanged(nameof(PositionZ));
                }
                catch (Exception _) { }
            }
        }

        [UIValue("scale")]
        protected float Scale
        {
            get => _pluginConfig.Scale;
            set
            {
                _pluginConfig.Scale = value;
                NotifyPropertyChanged(nameof(Scale));
            }
        }

        [UIComponent("disable-replay-text-component")]
        protected ToggleSetting disableReplayTextSetting = null!;
        [UIComponent("position-x-component")]
        protected StringSetting positionXComponent = null!;
        [UIComponent("position-y-component")]
        protected StringSetting positionYComponent = null!;
        [UIComponent("position-z-component")]
        protected StringSetting positionZComponent = null!;
        [UIComponent("scale-component")]
        protected SliderSetting sliderComponent = null!;

        [UIAction("#post-parse")]
        public void PostParse()
        {
            OnPluginEnabledStateChanged(EnableMod);
        }

        private void OnPluginEnabledStateChanged(bool state)
        {
            disableReplayTextSetting.interactable = state;
            positionXComponent.interactable = state;
            positionYComponent.interactable = state;
            positionZComponent.interactable = state;
            sliderComponent.interactable = state;
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