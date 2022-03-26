using System;

namespace ToggleReplayInfo.Models
{
    public class ReplayMetaDataWrapper : TypeReflection.Wrappers.JsonPropertyWrapper
    {
        public ReplayMetaDataWrapper(object replayMetaData) : base(replayMetaData)
        {
            if (!replayMetaData.GetType().IsAssignableFrom(Plugin.SSTM.SSTypes.ReplayMetaData))
                throw new ArgumentException($"Invalid Object of type \"{replayMetaData.GetType()}\" provided to wrapper for \"{thisObjectType}\" ({nameof(ReplayMetaDataWrapper)})!");
        }

        public LeaderboardPlayerInfoWrapper LeaderboardPlayerInfo => new LeaderboardPlayerInfoWrapper(Get("leaderboardPlayerInfo"));
        public int ReplayId => Get<int>("id");
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
            return $"[{nameof(ReplayMetaDataWrapper)}] ID: {this.ReplayId} | Rank: {this.Rank} | PP: {this.PP} | TimeSet: {this.TimeSet}";
        }
    }
}
