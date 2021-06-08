using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToggleReplayInfo.Exceptions;
using UnityEngine;

namespace ToggleReplayInfo.Models
{
    public class ReplayMetaDataWrapper
    {
        private object _targetReplayMetaDataObject; 

        public ReplayMetaDataWrapper(object replayMetaData)
        {
            if (replayMetaData != null && !replayMetaData.GetType().Equals(replayMetaDataType)) throw new ArgumentException($"{nameof(replayMetaData)} must be of type \"{replayMetaDataType?.Name}\"!");
            _targetReplayMetaDataObject = replayMetaData;
        }

        public bool HasValue
        {
            get
            {
                return _targetReplayMetaDataObject != null;
            }
        }

        internal object JSONThing
        {
            get
            {
                return _JSONThingFI.GetValue(_targetReplayMetaDataObject);
            }
        }

        public string PlayerId
        {
            get
            {
                return _playerIdPI.GetValue(_targetReplayMetaDataObject) as string;
            }
        }
        public string Name
        {
            get
            {
                return _namePI.GetValue(_targetReplayMetaDataObject) as string;
            }
        }

        public string Mods
        {
            get
            {
                return _modsPI.GetValue(_targetReplayMetaDataObject) as string;
            }
        }
        public string Timeset
        {
            get
            {
                return _timesetPI.GetValue(_targetReplayMetaDataObject) as string;
            }
        }
        public string Country
        {
            get
            {
                return _countryPI.GetValue(_targetReplayMetaDataObject) as string;
            }
        }

        public int Rank
        {
            get
            {
                return (int) _rankPI.GetValue(_targetReplayMetaDataObject);
            }
        }
        public int Score
        {
            get
            {
                return (int) _scorePI.GetValue(_targetReplayMetaDataObject);
            }
        }
        public int BadCuts
        {
            get
            {
                return (int) _badCutsPI.GetValue(_targetReplayMetaDataObject);
            }
        }
        public int MissedNotes
        {
            get
            {
                return (int) _missedNotesPI.GetValue(_targetReplayMetaDataObject);
            }
        }
        public int MaxCombo
        {
            get
            {
                return (int) _maxComboPI.GetValue(_targetReplayMetaDataObject);
            }
        }
        public int FullCombo
        {
            get
            {
                return (int) _fullComboPI.GetValue(_targetReplayMetaDataObject);
            }
        }
        public int HMD
        {
            get
            {
                return (int) _hmdPI.GetValue(_targetReplayMetaDataObject);
            }
        }

        public double PP
        {
            get
            {
                return (double) _ppPI.GetValue(_targetReplayMetaDataObject);
            }
        }
        public double weight
        {
            get
            {
                return (double) _weightPI.GetValue(_targetReplayMetaDataObject);
            }
        }

        public override string ToString()
        {
            return $"[{nameof(ReplayMetaDataWrapper)}] PlayerID: {this.PlayerId} | Name: {this.Name} | Country: {this.Country} | TimeSet: {this.Timeset}";
        }

        //string: playerId, name, mods, timeset, country
        //int: rank, score, badCuts, missedNotes, maxCombo, fullCombo, hmd
        //double: pp, weight

#pragma warning disable IDE0044 // Add readonly modifier
        private static FieldInfo _JSONThingFI = null!;
        // string
        private static PropertyInfo _playerIdPI = null!;
        private static PropertyInfo _namePI = null!;
        private static PropertyInfo _modsPI = null!;
        private static PropertyInfo _timesetPI = null!;
        private static PropertyInfo _countryPI = null!;

        // int
        private static PropertyInfo _rankPI = null!;
        private static PropertyInfo _scorePI = null!;
        private static PropertyInfo _badCutsPI = null!;
        private static PropertyInfo _missedNotesPI = null!;
        private static PropertyInfo _maxComboPI = null!;
        private static PropertyInfo _fullComboPI = null!;
        private static PropertyInfo _hmdPI = null!;

        // double
        private static PropertyInfo _ppPI = null!;
        private static PropertyInfo _weightPI = null!;
#pragma warning restore IDE0044 // Add readonly modifier

        internal static bool HasBeenSetup()
        {
            return _playerIdPI != null;
        }

        internal static Type replayMetaDataType;
        internal static Type leaderboardPageScoresContainerType;
        private static PropertyInfo _jsonProperty_PropertyNamePI;

        internal static void InitialSetup(Type replayMetaDataType, Type leaderboardPageScoresContainer)
        {
            ReplayMetaDataWrapper.replayMetaDataType = replayMetaDataType;
            ReplayMetaDataWrapper.leaderboardPageScoresContainerType = leaderboardPageScoresContainer;
            List<PropertyInfo> propList = new List<PropertyInfo>(replayMetaDataType.GetProperties(TypeReflection.Utilities.AnyBindingFlags));

            // Logger.log.Notice($"{nameof(ReplayMetaDataWrapper)}.{nameof(InitialSetup)}()");

            _JSONThingFI = replayMetaDataType.GetFields(TypeReflection.Utilities.AnyBindingFlags).First(x => x.FieldType.Equals(leaderboardPageScoresContainer));

            SetPropertyInfo(propList, nameof(_playerIdPI), typeof(ReplayMetaDataWrapper));
            SetPropertyInfo(propList, nameof(_namePI), typeof(ReplayMetaDataWrapper));
            SetPropertyInfo(propList, nameof(_modsPI), typeof(ReplayMetaDataWrapper));
            SetPropertyInfo(propList, nameof(_timesetPI), typeof(ReplayMetaDataWrapper));
            SetPropertyInfo(propList, nameof(_countryPI), typeof(ReplayMetaDataWrapper));

            SetPropertyInfo(propList, nameof(_rankPI), typeof(ReplayMetaDataWrapper));
            SetPropertyInfo(propList, nameof(_scorePI), typeof(ReplayMetaDataWrapper));
            SetPropertyInfo(propList, nameof(_badCutsPI), typeof(ReplayMetaDataWrapper));
            SetPropertyInfo(propList, nameof(_missedNotesPI), typeof(ReplayMetaDataWrapper));
            SetPropertyInfo(propList, nameof(_maxComboPI), typeof(ReplayMetaDataWrapper));
            SetPropertyInfo(propList, nameof(_fullComboPI), typeof(ReplayMetaDataWrapper));
            SetPropertyInfo(propList, nameof(_hmdPI), typeof(ReplayMetaDataWrapper));

            SetPropertyInfo(propList, nameof(_ppPI), typeof(ReplayMetaDataWrapper));
            SetPropertyInfo(propList, nameof(_weightPI), typeof(ReplayMetaDataWrapper));
        }

        internal static void SetPropertyInfo(List<PropertyInfo> propList, string fieldName, Type type)
        {
            string name = fieldName.Substring(1, fieldName.Length - 3);
            FieldInfo fi = type.GetField(fieldName, TypeReflection.Utilities.AnyBindingFlags);
            fi.SetValue(null, propList.First(x => HasJsonAttribute(x, name)));

            if((fi.GetValue(null) as PropertyInfo) == null)
            {
                throw new ReplayTextInitializationException($"{type.Name}.{fi.Name} has not been found!");
            }
        }

        private static bool HasJsonAttribute(PropertyInfo property, string name)
        {
            var attributes = property.GetCustomAttributes();

            if(attributes.Count() > 0)
            {
                var first = attributes.First();
                if (_jsonProperty_PropertyNamePI == null)
                    _jsonProperty_PropertyNamePI = first.GetType().GetProperty("PropertyName");
                return _jsonProperty_PropertyNamePI.GetValue(first).Equals(name);
            }
            return false;
        }

        public static class Cache
        {
            public static HashSet<ReplayMetaDataWrapper> _cache = new HashSet<ReplayMetaDataWrapper>();
            private static float _lastClearTime = 0f;

            public static void ClearForNextSetOnly()
            {
                if (_lastClearTime + 0.1f > Time.realtimeSinceStartup)
                    return;
                _lastClearTime = Time.realtimeSinceStartup;
                _cache.Clear();
            }

            public static void Clear()
            {
                _cache.Clear();
            }

            public static void Add(ReplayMetaDataWrapper replayMetaDataWrapper)
            {
                _cache.Add(replayMetaDataWrapper);
            }

            internal static ReplayMetaDataWrapper GetById(string platformUserId, int latestIndex)
            {
                foreach (ReplayMetaDataWrapper wrapper in _cache)
                {
                    if (wrapper.PlayerId.Equals(platformUserId) && wrapper.Rank == latestIndex)
                    {
                        return wrapper;
                    }
                }

                return null;
            }
        }

    }
}
