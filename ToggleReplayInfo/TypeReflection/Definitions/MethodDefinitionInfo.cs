using System;
using ToggleReplayInfo.TypeReflection.Core;

namespace ToggleReplayInfo.TypeReflection.Definitions
{
    public class MethodDefinitionInfo : DefinitionInfo
    {
        public Type[] ParameterTypes { get; private set; }
        public Type ReturnType
        {
            get
            {
                return MemberType;
            }
        }

        public MethodDefinitionInfo(Type returnType, MemberVisibility visibility, Type[] parameterTypes = null, bool staticMethod = false) : base(MemberInfoType.Method, returnType, visibility, staticMethod)
        {
            ParameterTypes = parameterTypes;
        }
    }
}
