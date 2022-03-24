using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToggleReplayInfo.Manager;

namespace ToggleReplayInfo.Models
{
    public class ReplayMetadataContainerWrapper : TypeReflection.Wrappers.WrapperBase
    {
    
        public ReplayMetadataContainerWrapper(object obj) : base(obj)
        {

        }


        public ReplayMetaDataWrapper ReplayMetaData => new ReplayMetaDataWrapper(GetFromFirstFieldWithType(Plugin.SSTM.SSTypes.ReplayMetaData));


    }

}
