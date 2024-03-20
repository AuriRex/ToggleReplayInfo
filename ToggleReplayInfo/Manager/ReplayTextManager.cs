using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using ToggleReplayInfo.Configuration;
using ToggleReplayInfo.Models;
using UnityEngine;
using Zenject;

namespace ToggleReplayInfo.Manager
{
    internal class ReplayTextManager : IInitializable, IDisposable
    {
        public const string SCORESABER_REPLAYTEXT_GAMEOBJECT_NAME = "InGameReplayUI/CustomUIText-ScoreSaber";

        private readonly PluginConfig _pluginConfig;
        private readonly ReplayScoreWrapper _replayMetaData;
        private readonly IPlatformUserModel _platformUserModel;
        private readonly bool _isFromResultsViewReplayButton;

        [Inject]
        public ReplayTextManager(PluginConfig pluginConfig, ReplayScoreWrapper replayMetaDataWrapper, IPlatformUserModel platformUserModel, [Inject(Id = "FromResultsViewReplayButton")] bool isFromResultsViewReplayButton)
        {
            _pluginConfig = pluginConfig;
            _replayMetaData = replayMetaDataWrapper;
            _platformUserModel = platformUserModel;
            _isFromResultsViewReplayButton = isFromResultsViewReplayButton;
        }

        public async void Initialize()
        {
            bool enable = _isFromResultsViewReplayButton;

            // Delay is to wait for the ScoreSaber Replay Text GameObject to get instantiated first
            await Task.Delay(200);

            if (!enable)
            {
                if (_replayMetaData == null) return;

                Logger.Log.Debug($"Replay is playing, Player Id: {this._replayMetaData.LeaderboardPlayerInfo.PlayerId}");

                var ctx = new CancellationToken();
                UserInfo userInfo = await _platformUserModel.GetUserInfo(ctx);

                Logger.Log.Debug($"UserInfo:\"{userInfo.platformUserId}\" - MetaData:\"{_replayMetaData.LeaderboardPlayerInfo.PlayerId}\"");

                enable = userInfo.platformUserId.Equals(_replayMetaData.LeaderboardPlayerInfo.PlayerId);
            }

            if(enable)
            {
                var go = GameObject.Find($"/{SCORESABER_REPLAYTEXT_GAMEOBJECT_NAME}");

                if(go == null)
                {
                    Logger.Log.Error($"The replay text gameObject \"/{SCORESABER_REPLAYTEXT_GAMEOBJECT_NAME}\" does not exist!");
                    return;
                }

                if (_pluginConfig.HideReplayInfo)
                {
                    Logger.Log.Info("Replay is from local player, hiding the ScoreSaber replay text gameobject.");
                    go.SetActive(false);
                    return;
                }

                var parent = go.transform?.parent?.gameObject;

                if (parent == null)
                {
                    Logger.Log.Error("parent should not be null but it was anyways...");
                    return;
                }

                Logger.Log.Debug("Setting additional replay text values ...");

                parent.transform.position = _pluginConfig.Position.ToVector3();
                parent.transform.rotation = Quaternion.Euler(_pluginConfig.Rotation.ToVector3());

                go.transform.localScale = go.transform.localScale * _pluginConfig.Scale;

                var tmp = go.GetComponent<TextMeshProUGUI>();

                tmp.color = _pluginConfig.GetColor();
            }
        }

        public void Dispose()
        {
            
        }
    }
}
