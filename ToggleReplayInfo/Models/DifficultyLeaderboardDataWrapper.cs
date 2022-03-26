namespace ToggleReplayInfo.Models
{
    public class DifficultyLeaderboardDataWrapper : TypeReflection.Wrappers.JsonPropertyWrapper
    {
        public DifficultyLeaderboardDataWrapper(object wrappedObject) : base(wrappedObject)
        {

        }

        public int LeaderboardId => Get<int>("leaderboardId");
        public int Difficulty => Get<int>("difficulty");
        public string GameMode => Get<string>("gameMode");
        public string DifficultyRaw => Get<string>("difficultyRaw");
    }
}
