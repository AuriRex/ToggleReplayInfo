namespace ToggleReplayInfo.Models
{
    public class LeaderboardPlayerInfoWrapper : TypeReflection.Wrappers.JsonPropertyWrapper
    {

        public LeaderboardPlayerInfoWrapper(object wrappedObject) : base(wrappedObject)
        {

        }

        public string Id => Get<string>("id");
        public string Name => Get<string>("name");
        public string ProfilePicture => Get<string>("profilePicture");
        public string Country => Get<string>("country");
        public int Permissions => Get<int>("permissions");
        public string Role => Get<string>("role");


    }
}
