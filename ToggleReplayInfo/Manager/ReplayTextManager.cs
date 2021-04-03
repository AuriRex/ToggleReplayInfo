using ScoreSaber;
using ScoreSaber.Core.Data;
using System;
using System.Collections;
using ToggleReplayInfo.Configuration;
using ToggleReplayInfo.HarmonyPatches;
using UnityEngine;
using Zenject;

namespace ToggleReplayInfo.Manager
{
    internal class ReplayTextManager : IInitializable, IDisposable
    {
        public const string SCORESABER_REPLAYTEXT_GAMEOBJECT_NAME = "InGameReplayUI/CustomUIText-ScoreSaber";

        private PluginConfig _pluginConfig;

        private Coroutine _coroutine;

        public bool IsReplayPlaying
        {
            get
            {
                bool? value = ReplayPlayer.instance?.playbackEnabled;
                if (value.HasValue) return value.Value;
                return false;
            }
        }

        [Inject]
        public ReplayTextManager(PluginConfig pluginConfig)
        {
            _pluginConfig = pluginConfig;
        }

        public void Initialize()
        {
            Score score = ReplayPlayer.instance?.currentScore;

            string playerId = score?.playerId;

            if (IsReplayPlaying && LocalPlayerInfoPatch.localPlayerInfo.playerId.Equals(score.playerId) && _pluginConfig.HideReplayInfo)
            {
                _coroutine = SharedCoroutineStarter.instance.StartCoroutine(DoAfter(.2f, () => {
                    Logger.log.Debug("Deactivating the ScoreSaber Replay Text.");
                    var go = GameObject.Find($"/{SCORESABER_REPLAYTEXT_GAMEOBJECT_NAME}");
                    go?.SetActive(false);
                }));
            }
        }

        public void Dispose()
        {
            if (_coroutine != null)
            {
                SharedCoroutineStarter.instance.StopCoroutine(_coroutine);
            }
        }

        public static IEnumerator DoAfter(float time, Action action)
        {
            float start = Time.fixedTime;
            while (start + time > Time.fixedTime)
                yield return null;
            action?.Invoke();
            yield break;
        }
    }
}
