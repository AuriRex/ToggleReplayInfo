using System;

namespace ToggleReplayInfo.Models
{
    public class ReplayScoreMapWrapper : TypeReflection.Wrappers.WrapperBase
    {    
        public ReplayScoreMapWrapper(object obj) : base(obj)
        {
            if (!obj.GetType().IsAssignableFrom(Plugin.SSTM.SSTypes.ScoreMap))
                throw new ArgumentException($"Invalid Object of type \"{obj.GetType()}\" provided to wrapper for \"{thisObjectType}\" ({nameof(ReplayScoreMapWrapper)})!");
        }

        public ReplayScoreWrapper Score => new ReplayScoreWrapper(GetFromFirstFieldWithType(Plugin.SSTM.SSTypes.Score));
    }
}