using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToggleReplayInfo.Exceptions;
using ToggleReplayInfo.TypeReflection.Attributes;
using Zenject;
using static ToggleReplayInfo.TypeReflection.Utilities;

namespace ToggleReplayInfo.Manager
{
    public class ScoreSaberStaticBlobManager : IInitializable, IDisposable
    {
        private readonly ScoreSaberTypeManager _scoreSaberTypeManager;

        public ScoreSaberStaticBlobManager(ScoreSaberTypeManager scoreSaberTypeManager)
        {
            _scoreSaberTypeManager = scoreSaberTypeManager;
        }

        private MethodInfo _getReplayInstanceMI;

        [IgnoreVerification]
        private object _scoreSaberReplay;

        [IgnoreVerification]
        private FieldInfo _isInReplayFI;
        [IgnoreVerification]
        private FieldInfo _uploadGUIDFI;
        [IgnoreVerification]
        private FieldInfo _playerName;
        [IgnoreVerification]
        private FieldInfo _replayDataFI;

        public void Initialize()
        {
            if(!_scoreSaberTypeManager.IsReady)
            {
                Logger.log.Debug($"{nameof(ScoreSaberTypeManager)} is not ready, aborting initialisation of {nameof(ScoreSaberStaticBlobManager)}.");
                return;
            }
            Init();
        }

        private void Init()
        {
            _getReplayInstanceMI = _scoreSaberTypeManager.BigStaticThing.GetMethods(AnyBindingFlags).First(x => x.ReturnType.Equals(_scoreSaberTypeManager.ScoreSaberReplayContainer));

            if (!AllMembersPopulated<ScoreSaberStaticBlobManager>(MemberTypes.Field, this))
            {
                throw new ReplayTextInitializationException($"Not all members in {nameof(ScoreSaberStaticBlobManager)} were populated!");
            }

            HarmonyPatches.Patches.PatchLevelResultsReplayStartButtonClicker.onLevelCompletedWatchReplayButtonClicked += PatchLevelResultsReplayStartButtonClicker_onLevelCompletedWatchReplayButtonClicked;
        }

        public bool IsFromLevelCompletedWatchButton { get; private set; }

        private void PatchLevelResultsReplayStartButtonClicker_onLevelCompletedWatchReplayButtonClicked()
        {
            IsFromLevelCompletedWatchButton = true;
        }

        internal void OnGameSceneInitializing()
        {
            IsFromLevelCompletedWatchButton = false;
        }

        public object GetReplayInstance()
        {
            _scoreSaberReplay = _getReplayInstanceMI.Invoke(null, null);
            return _scoreSaberReplay;
        }

        public bool IsInReplay()
        {
            if(_isInReplayFI == null)
                _isInReplayFI = _scoreSaberTypeManager.ScoreSaberReplayContainer.GetFields(TypeReflection.Utilities.AnyBindingFlags).Last(x => x.FieldType.Equals(typeof(bool)));

            return (bool) _isInReplayFI.GetValue(GetReplayInstance());
        }

        private string GetEmptyStringLol()
        {
            if (_uploadGUIDFI == null)
                _uploadGUIDFI = _scoreSaberTypeManager.ScoreSaberReplayContainer.GetFields(TypeReflection.Utilities.AnyBindingFlags).First(x => x.FieldType.Equals(typeof(string)));

            return _uploadGUIDFI.GetValue(GetReplayInstance()) as string;
        }

        public string GetRawPlayerName()
        {
            if (_playerName == null)
                _playerName = _scoreSaberTypeManager.ScoreSaberReplayContainer.GetFields(TypeReflection.Utilities.AnyBindingFlags).Last(x => x.FieldType.Equals(typeof(string)));

            return _playerName.GetValue(GetReplayInstance()) as string;
        }

        private string GetCoreDataStringOne()
        {
            // ReplayCoreData
            if(_replayDataFI == null)
                _replayDataFI = _scoreSaberTypeManager.ScoreSaberReplayContainer.GetFields(AnyBindingFlags).First(x => x.FieldType.Equals(_scoreSaberTypeManager.ScoreSaberReplay));

            FieldInfo replayCoreDataFI = _scoreSaberTypeManager.ScoreSaberReplay.GetFields(AnyBindingFlags).First(x => x.FieldType.Equals(_scoreSaberTypeManager.ReplayCoreData));

            FieldInfo firstStringFI = _scoreSaberTypeManager.ReplayCoreData.GetFields(AnyBindingFlags).First(x => x.FieldType.Equals(typeof(string)));

            object replayKeyframeDataInstance = _replayDataFI.GetValue(GetReplayInstance());

            object replayCoreDataInstance = replayCoreDataFI.GetValue(replayKeyframeDataInstance);

            return firstStringFI.GetValue(replayCoreDataInstance) as string;
        }

        public void Dispose()
        {
            HarmonyPatches.Patches.PatchLevelResultsReplayStartButtonClicker.onLevelCompletedWatchReplayButtonClicked -= PatchLevelResultsReplayStartButtonClicker_onLevelCompletedWatchReplayButtonClicked;
        }
    }
}
