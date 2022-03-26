using System;

namespace ToggleReplayInfo.Models
{
    public class ReplayMetadataContainerWrapper : TypeReflection.Wrappers.WrapperBase
    {    
        public ReplayMetadataContainerWrapper(object obj) : base(obj)
        {
            if (!obj.GetType().IsAssignableFrom(Plugin.SSTM.SSTypes.SS_ReplayMetadataContainer))
                throw new ArgumentException($"Invalid Object of type \"{obj.GetType()}\" provided to wrapper for \"{thisObjectType}\" ({nameof(ReplayMetadataContainerWrapper)})!");
        }

        public ReplayMetaDataWrapper ReplayMetaData => new ReplayMetaDataWrapper(GetFromFirstFieldWithType(Plugin.SSTM.SSTypes.ReplayMetaData));
    }
}