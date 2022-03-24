using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToggleReplayInfo.Configuration;
using ToggleReplayInfo.Manager;
using Zenject;

namespace ToggleReplayInfo.UI
{
    public class ModifierHost : IInitializable, INotifyPropertyChanged
    {
        private readonly PluginConfig _pluginConfig;
        private readonly ScoreSaberTypeManager _scoreSaberTypeManager;
        private readonly IPlatformUserModel _platformUserModel;


        public ModifierHost(PluginConfig pluginConfig, ScoreSaberTypeManager sstm, IPlatformUserModel platformUserModel)
        {
            _pluginConfig = pluginConfig;
            _scoreSaberTypeManager = sstm;
            _platformUserModel = platformUserModel;
        }

        public async void Initialize()
        {
            var userInfo = await _platformUserModel.GetUserInfo();

            _statusText = $"This only affects <u>your</u> replays, <color=purple>{userInfo.userName}</color>!";
        }

        private string _statusText = "This only affects your replays!";
        [UIValue("status-text")]
        public string StatusText
        {
            get => _statusText;
            set
            {
                _statusText = value;
                NotifyPropertyChanged(nameof(StatusText));
            }
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
                catch(Exception) { }
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
                catch (Exception) { }
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
                catch (Exception) { }
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

        [UIComponent("enable-mod-component")]
        protected ToggleSetting enableModComponent = null!;
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
            if(_scoreSaberTypeManager.HasErrorsOnInitInstance)
            {
                enableModComponent.interactable = false;
                OnPluginEnabledStateChanged(false);
                StatusText = "MOD DISABLED!\nSomething went wrong on startup!";
            }
            else
            {
                OnPluginEnabledStateChanged(EnableMod);
            }
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