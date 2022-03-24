using System;
using ToggleReplayInfo.TypeReflection.Core;

namespace ToggleReplayInfo.TypeReflection.Definitions
{
    public class PropertyDefinitionInfo : DefinitionInfo
    {
        public PropertyDefinitionInfo(Type memberValueType, MemberVisibility visibility, bool staticField) : base(MemberInfoType.Property, memberValueType, visibility, staticField)
        {
        }
    }
}
