using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToggleReplayInfo.Configuration;
using ToggleReplayInfo.Manager;
using UnityEngine;
using UnityEngine.UI;
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

        [UIParams]
        protected BSMLParserParams parserParams = null!;

        private string _statusText = "This only affects your replays!";
        [UIValue("status-text")]
        public string StatusText
        {
            get => _statusText;
            set
            {
                _statusText = value;
                NotifyPropertyChanged();
            }
        }

        [UIValue("enable-mod")]
        protected bool EnableMod
        {
            get => _pluginConfig.Enabled;
            set
            {
                _pluginConfig.Enabled = value;
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }

        [UIValue("base-text-color")]
        protected Color BaseTextColor
        {
            get => _pluginConfig.GetColor();
            set
            {
                _pluginConfig.SetColor(value);
                NotifyPropertyChanged();
            }
        }

        [UIValue("base-text-alpha")]
        protected float BaseTextAlpha
        {
            get => _pluginConfig.GetColor().a;
            set
            {
                _pluginConfig.SetColor(_pluginConfig.GetColor().ColorWithAlpha(value));
                NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }

        [UIValue("rotation-x")]
        protected string RotationX
        {
            get => _pluginConfig.Rotation.X.ToString();
            set
            {
                try
                {
                    var val = float.Parse(value);
                    _pluginConfig.Rotation.X = val;
                    NotifyPropertyChanged();
                }
                catch (Exception) { }
            }
        }

        [UIValue("rotation-y")]
        protected string RotationY
        {
            get => _pluginConfig.Rotation.Y.ToString();
            set
            {
                try
                {
                    var val = float.Parse(value);
                    _pluginConfig.Rotation.Y = val;
                    NotifyPropertyChanged();
                }
                catch (Exception) { }
            }
        }

        [UIValue("rotation-z")]
        protected string RotationZ
        {
            get => _pluginConfig.Rotation.Z.ToString();
            set
            {
                try
                {
                    var val = float.Parse(value);
                    _pluginConfig.Rotation.Z = val;
                    NotifyPropertyChanged();
                }
                catch (Exception) { }
            }
        }

        [UIComponent("enable-mod-component")]
        protected ToggleSetting enableModComponent = null!;
        [UIComponent("disable-replay-text-component")]
        protected ToggleSetting disableReplayTextSetting = null!;
        [UIComponent("text-color-component")]
        protected ColorSetting baseTextColorComponent = null!;
        [UIComponent("text-alpha-component")]
        protected SliderSetting baseTextAlphaComponent = null!;
        [UIComponent("position-x-component")]
        protected StringSetting positionXComponent = null!;
        [UIComponent("position-y-component")]
        protected StringSetting positionYComponent = null!;
        [UIComponent("position-z-component")]
        protected StringSetting positionZComponent = null!;
        [UIComponent("scale-component")]
        protected SliderSetting sliderComponent = null!;
        [UIComponent("rotation-x-component")]
        protected StringSetting rotationXComponent = null!;
        [UIComponent("rotation-y-component")]
        protected StringSetting rotationYComponent = null!;
        [UIComponent("rotation-z-component")]
        protected StringSetting rotationZComponent = null!;
        [UIComponent("reset-button")]
        protected Button resetButtonComponent = null!;

        [UIAction("#post-parse")]
        public void PostParse()
        {
            if(_scoreSaberTypeManager.HasErrorsOnInit)
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
            baseTextColorComponent.interactable = state;
            baseTextAlphaComponent.interactable = state;
            positionXComponent.interactable = state;
            positionYComponent.interactable = state;
            positionZComponent.interactable = state;
            sliderComponent.interactable = state;
            rotationXComponent.interactable = state;
            rotationYComponent.interactable = state;
            rotationZComponent.interactable = state;
            resetButtonComponent.interactable = state;
        }

        [UIAction("reset-values-for-real")]
        private void OnResetValuesForReal()
        {
            Logger.Log.Info("Resetting values to their defaults!");

            parserParams.EmitEvent("hide-ays-modal");

            BaseTextColor = Color.white;
            BaseTextAlpha = 1f;
            _pluginConfig.Position = PluginConfig.Vector3S.Default();
            NotifyPropertyChanged(nameof(PositionX));
            NotifyPropertyChanged(nameof(PositionY));
            NotifyPropertyChanged(nameof(PositionZ));
            RotationX = 0.ToString();
            RotationY = 0.ToString();
            RotationZ = 0.ToString();
            Scale = 1f;
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