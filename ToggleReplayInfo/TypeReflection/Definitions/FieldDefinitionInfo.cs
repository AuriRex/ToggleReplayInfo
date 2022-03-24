using System;
using ToggleReplayInfo.TypeReflection.Core;

namespace ToggleReplayInfo.TypeReflection.Definitions
{
    public class FieldDefinitionInfo : DefinitionInfo
    {
        public bool IsReadonly { get; private set; }
        public FieldDefinitionInfo(Type memberValueType, MemberVisibility visibility, bool staticField, bool isReadonly = false) : base(MemberInfoType.Field, memberValueType, visibility, staticField)
        {
            IsReadonly = isReadonly;
        }
    }
}
