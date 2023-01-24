using System;

namespace ToggleReplayInfo.Models
{
    public class LeaderboardPlayerWrapper : TypeReflection.Wrappers.JsonPropertyWrapper
    {
        public LeaderboardPlayerWrapper(object wrappedObject) : base(wrappedObject)
        {
            if (!wrappedObject.GetType().IsAssignableFrom(Plugin.SSTM.SSTypes.LeaderboardPlayerInfo))
                throw new ArgumentException($"Invalid Object of type \"{wrappedObject.GetType()}\" provided to wrapper for \"{thisObjectType}\" ({nameof(LeaderboardPlayerWrapper)})!");
        }

        public string PlayerId => Get<string>("id");
        public string Name => Get<string>("name");
        public string ProfilePicture => Get<string>("profilePicture");
        public string Country => Get<string>("country");
        public int Permissions => Get<int>("permissions");
        public string Role => Get<string>("role");
    }
}