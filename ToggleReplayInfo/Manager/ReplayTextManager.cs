using ScoreSaber;
using ScoreSaber.Core.Data;
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
        private readonly ScoreSaberStaticBlobManager _scoreSaberStaticBlobManager;
        private readonly IPlatformUserModel _platformUserModel;

        [Inject]
        public ReplayTextManager(PluginConfig pluginConfig, ScoreSaberStaticBlobManager scoreSaberStaticBlobManager, IPlatformUserModel platformUserModel)
        {
            _pluginConfig = pluginConfig;
            _scoreSaberStaticBlobManager = scoreSaberStaticBlobManager;
            _platformUserModel = platformUserModel;
        }

        public async void Initialize()
        {
            bool fromLevelCompletedWatchReplayButton = _scoreSaberStaticBlobManager.IsFromLevelCompletedWatchButton;
            _scoreSaberStaticBlobManager.OnGameSceneInitializing();

            if (!_pluginConfig.Enabled) return;

            await Task.Delay(200);

            // TODO: implement this later
            if (fromLevelCompletedWatchReplayButton) return;

            if (!_scoreSaberStaticBlobManager.IsInReplay())
            {
                return;
            }

            Logger.log.Debug($"Replay is playing, PlayerName: {_scoreSaberStaticBlobManager.GetRawPlayerName()}");

            UserInfo userInfo = await _platformUserModel.GetUserInfo();



            ReplayMetaDataWrapper metaData = PatchScoreSaberLeaderboardView_InfoButton.CurrentMetaData;

            if (metaData == null || !metaData.HasValue)
            {
                Logger.log.Debug($"MetaData is null or empty, cancelling!");
                return;
            }

           

            Logger.log.Debug($"UserInfo:\"{userInfo.platformUserId}\" - MetaData:\"{metaData.PlayerId}\"");

            if(fromLevelCompletedWatchReplayButton || userInfo.platformUserId.Equals(metaData.PlayerId))
            {
                var go = GameObject.Find($"/{SCORESABER_REPLAYTEXT_GAMEOBJECT_NAME}");

                if(go == null)
                {
                    Logger.log.Error($"The replay text gameObject \"/{SCORESABER_REPLAYTEXT_GAMEOBJECT_NAME}\" does not exist!");
                    return;
                }

                if (_pluginConfig.HideReplayInfo)
                {
                    Logger.log.Debug("Deactivating the ScoreSaber replay text gameobject.");
                    go.SetActive(false);
                    return;
                }

                var parent = go.transform?.parent?.gameObject;

                if (parent == null)
                {
                    Logger.log.Error("parent should not be null but it was anyways...");
                    return;
                }

                Logger.log.Debug("Setting additional replay text values ...");

                parent.transform.position = _pluginConfig.Position.ToVector3();

                go.transform.localScale = go.transform.localScale * _pluginConfig.Scale;

            }
        }

        public void Dispose()
        {
            
        }
    }
}
