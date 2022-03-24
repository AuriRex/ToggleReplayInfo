using System;
using ToggleReplayInfo.TypeReflection.Core;

namespace ToggleReplayInfo.TypeReflection.Definitions
{
    public class DefinitionInfo
    {
        public MemberInfoType MemberInfo { get; private set; }
        public Type MemberType { get; internal set; }
        public MemberVisibility Visibility { get; private set; }
        public bool IsStatic { get; private set; }

        public DefinitionInfo(MemberInfoType memberType, Type memberValueType, MemberVisibility visibility, bool staticMember = false)
        {
            MemberInfo = memberType;
            MemberType = memberValueType;
            Visibility = visibility;
            IsStatic = staticMember;
        }
    }
}
