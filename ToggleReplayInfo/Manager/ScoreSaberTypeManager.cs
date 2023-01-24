using System;
using System.Reflection;
using Zenject;

namespace ToggleReplayInfo.Manager
{
    public class ScoreSaberTypeManager : IInitializable
    {
        public static bool HasErrorsOnInitStatic { get; private set; } = false;
        public bool HasErrorsOnInit => HasErrorsOnInitStatic;

        public ScoreSaberTypes SSTypes = new ScoreSaberTypes();

        public Assembly ScoreSaberAssembly { get; private set; }

        public class ScoreSaberTypes
        {
            public Type ScoreMap { get; internal set; }
            public Type Score { get; internal set; }
            public Type LeaderboardPlayerInfo { get; internal set; }
            public Type ResultsViewReplayButtonController { get; internal set; }
        }

        public bool IsReady { get; private set; }

        public void Initialize()
        {
            ScoreSaberAssembly = Assembly.GetAssembly(typeof(ScoreSaber.Plugin));

            GetInternalScoreSaberTypes();

            IsReady = true;

            Plugin.SSTM = this;
        }

        private void GetInternalScoreSaberTypes()
        {
            try
            {
                SSTypes.LeaderboardPlayerInfo = ScoreSaberAssembly.GetType("ScoreSaber.Core.Data.Models.LeaderboardPlayer");
                SSTypes.Score = ScoreSaberAssembly.GetType("ScoreSaber.Core.Data.Models.Score");
                SSTypes.ScoreMap = ScoreSaberAssembly.GetType("ScoreSaber.Core.Data.Wrappers.ScoreMap");
                SSTypes.ResultsViewReplayButtonController = ScoreSaberAssembly.GetType("ScoreSaber.Core.ReplaySystem.UI.ResultsViewReplayButtonController");
            }
            catch(Exception ex)
            {
                Logger.Log.Error($"{nameof(ScoreSaberTypeManager)} failed: {ex.Message}");
                Logger.Log.Error($"{ex.StackTrace}");
                HasErrorsOnInitStatic = true;
            }
        }
    }
}
