using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ToggleReplayInfo.Models
{
    public class LeaderboardPageScoresContainerWrapper
    {
        private readonly object _targetOriginalObject;

        public LeaderboardPageScoresContainerWrapper(object original)
        {
            if (original != null && !original.GetType().Equals(selfType)) throw new ArgumentException($"{nameof(original)} must be of type \"{selfType?.Name}\"!");
            _targetOriginalObject = original;
        }

        public string Ranked
        {
            get
            {
                return _rankedPI.GetValue(_targetOriginalObject) as string;
            }
        }
        public string UID
        {
            get
            {
                return _uidPI.GetValue(_targetOriginalObject) as string;
            }
        }
        public IList Scores
        {
            get
            {
                return _scoresPI.GetValue(_targetOriginalObject) as IList;
            }
        }
        public string PlayerScore
        {
            get
            {
                return _playerScorePI.GetValue(_targetOriginalObject) as string;
            }
        }
        /*public IDifficultyBeatmap Level
        {
            get
            {
                return _levelPI.GetValue(_targetOriginalObject) as IDifficultyBeatmap;
            }
        }*/

        public override string ToString()
        {
            return $"[{nameof(LeaderboardPageScoresContainerWrapper)}] Ranked: {this.Ranked} | UID: {this.UID} | PlayerScore: {this.PlayerScore} | ScoreCount: {Scores?.Count}";
        }

        private static FieldInfo _plugin_leaderboardInfoDownloaderFI;
        private static FieldInfo _downloader_leaderboardPageScoresContainerFI;
        public static LeaderboardPageScoresContainerWrapper GetInstanceFromStaticDownloader()
        {
            if(_plugin_leaderboardInfoDownloaderFI == null)
                _plugin_leaderboardInfoDownloaderFI = typeof(ScoreSaber.Plugin).GetFields(TypeReflection.Utilities.AnyBindingFlags)
                    .First(x => x.FieldType.Equals(leaderboardInfoDownloaderType));

            object scoresPageDownloader = _plugin_leaderboardInfoDownloaderFI.GetValue(null);

            if(_downloader_leaderboardPageScoresContainerFI == null)
                _downloader_leaderboardPageScoresContainerFI = scoresPageDownloader.GetType().GetFields(TypeReflection.Utilities.AnyBindingFlags)
                    .First(x => x.FieldType.Equals(selfType));

            return new LeaderboardPageScoresContainerWrapper(_downloader_leaderboardPageScoresContainerFI.GetValue(scoresPageDownloader));
        }

#pragma warning disable IDE0044 // Add readonly modifier
        private static PropertyInfo _rankedPI = null!; // string ranked
        private static PropertyInfo _uidPI = null!; // string uid
        private static PropertyInfo _scoresPI = null!; // List<ReplayMetaData> scores
        private static PropertyInfo _playerScorePI = null!; // string playerScore
        //private static PropertyInfo _levelPI = null!; // IDifficultyBeatmap level
#pragma warning restore IDE0044 // Add readonly modifier

        internal static bool HasBeenSetup()
        {
            return _rankedPI != null;
        }

        internal static Type selfType;
        internal static Type leaderboardInfoDownloaderType;

        internal static void InitialSetup(Type leaderboardPageScoresContainerType, Type leaderboardInfoDownloaderType)
        {
            selfType = leaderboardPageScoresContainerType;
            LeaderboardPageScoresContainerWrapper.leaderboardInfoDownloaderType = leaderboardInfoDownloaderType;
            List<PropertyInfo> propList = new List<PropertyInfo>(leaderboardPageScoresContainerType.GetProperties(TypeReflection.Utilities.AnyBindingFlags));

            // Logger.log.Notice($"{nameof(ReplayMetaDataWrapper)}.{nameof(InitialSetup)}()");

            ReplayMetaDataWrapper.SetPropertyInfo(propList, nameof(_rankedPI), typeof(LeaderboardPageScoresContainerWrapper));
            ReplayMetaDataWrapper.SetPropertyInfo(propList, nameof(_uidPI), typeof(LeaderboardPageScoresContainerWrapper));
            ReplayMetaDataWrapper.SetPropertyInfo(propList, nameof(_scoresPI), typeof(LeaderboardPageScoresContainerWrapper));
            ReplayMetaDataWrapper.SetPropertyInfo(propList, nameof(_playerScorePI), typeof(LeaderboardPageScoresContainerWrapper));
            //ReplayMetaDataWrapper.SetPropertyInfo(propList, nameof(_levelPI), typeof(LeaderboardPageScoresContainerWrapper));
        }

    }
}
