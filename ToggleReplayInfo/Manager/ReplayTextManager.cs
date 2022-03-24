using System;
using System.Collections;
using System.Threading.Tasks;
using ToggleReplayInfo.Configuration;
using ToggleReplayInfo.HarmonyPatches;
using ToggleReplayInfo.HarmonyPatches.Patches;
using ToggleReplayInfo.Models;
using UnityEngine;
using Zenject;

namespace ToggleReplayInfo.Manager
{
    internal class ReplayTextManager : IInitializable, IDisposable
    {
        public const string SCORESABER_REPLAYTEXT_GAMEOBJECT_NAME = "InGameReplayUI/CustomUIText-ScoreSaber";

        private readonly PluginConfig _pluginConfig;
        private readonly ReplayMetaDataWrapper _replayMetaData;
        private readonly IPlatformUserModel _platformUserModel;

        [Inject]
        public ReplayTextManager(PluginConfig pluginConfig, ReplayMetaDataWrapper replayMetaDataWrapper, IPlatformUserModel platformUserModel)
        {
            _pluginConfig = pluginConfig;
            _replayMetaData = replayMetaDataWrapper;
            _platformUserModel = platformUserModel;
        }

        public async void Initialize()
        {
            if (!_pluginConfig.Enabled) return;

            if (_replayMetaData == null) return;

            // Delay is to wait for the ScoreSaber Replay Text GameObject to get instantiated first
            await Task.Delay(200);

            Logger.Log.Debug($"Replay is playing, Player Id: {this._replayMetaData.LeaderboardPlayerInfo.Id}");

            UserInfo userInfo = await _platformUserModel.GetUserInfo();


            Logger.Log.Debug($"UserInfo:\"{userInfo.platformUserId}\" - MetaData:\"{_replayMetaData.LeaderboardPlayerInfo.Id}\"");

            if(userInfo.platformUserId.Equals(_replayMetaData.LeaderboardPlayerInfo.Id))
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

                go.transform.localScale = go.transform.localScale * _pluginConfig.Scale;

                // TODO: Rotation
            }
        }

        public void Dispose()
        {
            
        }
    }
}
