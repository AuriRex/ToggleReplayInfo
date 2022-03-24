using System;

namespace ToggleReplayInfo.TypeReflection.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IgnoreVerification : Attribute
    {
        
    }
}
