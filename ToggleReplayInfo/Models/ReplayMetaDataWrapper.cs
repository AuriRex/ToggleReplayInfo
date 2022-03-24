using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToggleReplayInfo.Exceptions;
using ToggleReplayInfo.TypeReflection;
using UnityEngine;

namespace ToggleReplayInfo.Models
{
    public class ReplayMetaDataWrapper : TypeReflection.Wrappers.JsonPropertyWrapper
    {
        public ReplayMetaDataWrapper(object replayMetaData) : base(replayMetaData)
        {

        }

        public LeaderboardPlayerInfoWrapper LeaderboardPlayerInfo => new LeaderboardPlayerInfoWrapper(Get("leaderboardPlayerInfo"));
        public int Id => Get<int>("id");
        public int Rank => Get<int>("rank");
        public int BaseScore => Get<int>("baseScore");
        public int ModifiedScore => Get<int>("modifiedScore");
        public int BadCuts => Get<int>("badCuts");
        public int MissedNotes => Get<int>("missedNotes");
        public int MaxCombo => Get<int>("maxCombo");
        public int Hmd => Get<int>("hmd");
        public double PP => Get<double>("pp");
        public double Weight => Get<double>("weight");
        public double Multiplier => Get<double>("multiplier");
        public bool FullCombo => Get<bool>("fullCombo");
        public bool HasReplay => Get<bool>("hasReplay");
        public string Modifiers => Get<string>("modifiers");
        public DateTime TimeSet => Get<DateTime>("timeSet");

        public override string ToString()
        {
            return $"[{nameof(ReplayMetaDataWrapper)}] ID: {this.Id} | Rank: {this.Rank} | PP: {this.PP} | TimeSet: {this.TimeSet}";
        }

    }
}
