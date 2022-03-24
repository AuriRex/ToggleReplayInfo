using System;

namespace ToggleReplayInfo.TypeReflection.Core
{
    [Flags]
    public enum MemberVisibility
    {
        Private = 1,
        Family = 2,
        Assembly = 4,
        Public = 8,
        Any = Private | Family | Assembly | Public,
    }
}
