using System;
using ToggleReplayInfo.TypeReflection.Core;

namespace ToggleReplayInfo.TypeReflection.Definitions
{
    public class RequirementDefinitionInfo : DefinitionInfo
    {
        public TypeDefinition RequiredType { get; private set; }
        public Type[] ParameterTypes { get; private set; }
        public bool IsReadonly { get; private set; }
        public RequirementDefinitionInfo(MemberInfoType memberInfoType, TypeDefinition requiredType, MemberVisibility visibility, Type[] parameterTypes, bool staticField, bool isReadonly) : base(memberInfoType, null, visibility, staticField)
        {
            RequiredType = requiredType;
            ParameterTypes = parameterTypes;
            IsReadonly = isReadonly;
        }

        public DefinitionInfo ResolveFromRequiredType()
        {
            if (!RequiredType.AllRequirementsMet || RequiredType.Type == null) return null;
            Type type = RequiredType.Type;
            return MemberInfo switch
            {
                MemberInfoType.Field => new FieldDefinitionInfo(type, Visibility, IsStatic, IsReadonly),
                MemberInfoType.Property => new PropertyDefinitionInfo(type, Visibility, IsStatic),
                MemberInfoType.Method => new MethodDefinitionInfo(type, Visibility, ParameterTypes, IsStatic),
                _ => null,
            };
        }
    }
}
