using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static ToggleReplayInfo.TypeReflection.TypeDefinitionManager.TypeDefinition;
using static ToggleReplayInfo.TypeReflection.TypeDefinitionManager.TypeDefinition.DefinitionInfo;

namespace ToggleReplayInfo.TypeReflection
{
    public class TypeDefinitionManager
    {
        public Dictionary<Assembly, List<TypeDefinition>> TypeDefinitions { get; private set; }
        private readonly Dictionary<Type, MemberInfo[]> _membersForType;

        private Action<string> _logAction;

        public TypeDefinitionManager(Action<string> logAction = null)
        {
            TypeDefinitions = new Dictionary<Assembly, List<TypeDefinition>>();
            _membersForType = new Dictionary<Type, MemberInfo[]>();
            _logAction = logAction;
        }

        public void SetLoggingAction(Action<string> logAction)
        {
            _logAction = logAction;
        }

        public void RegisterTypes(Assembly assembly, TypeDefinition[] typeDefinitions, Action<TypeDefinition> onTypeFound = null)
        {
            foreach(var typeDef in typeDefinitions)
            {
                RegisterType(assembly, typeDef, onTypeFound);
            }
        }

        public void RegisterType(Assembly assembly, TypeDefinition typeDefinition, Action<TypeDefinition> onTypeFound = null)
        {
            typeDefinition.onTypeFound = onTypeFound;

            if (TypeDefinitions.TryGetValue(assembly, out List<TypeDefinition> typeDefinitionList)) {
                if (typeDefinitionList.Contains(typeDefinition)) throw new ArgumentException($"{nameof(typeDefinition)} can not define the same type twice!");
                typeDefinitionList.Add(typeDefinition);
                return;
            }

            TypeDefinitions.Add(assembly, new List<TypeDefinition>() { typeDefinition });
        }

        public bool ResolveAllTypes()
        {
            bool ret = true;
            foreach(KeyValuePair<Assembly, List<TypeDefinition>> kvp in TypeDefinitions)
            {
                if(!ResolveTypes(kvp.Key, kvp.Value) && ret)
                {
                    ret = false;
                }
            }
            return ret;
        }

        public bool ResolveTypesInAssembly(Assembly assembly)
        {
            if (TypeDefinitions.TryGetValue(assembly, out List<TypeDefinition> typeDefs))
            {
                return ResolveTypes(assembly, typeDefs);
            }
            return false;
        }

        private bool ResolveTypes(Assembly assembly, List<TypeDefinition> typeDefinitions)
        {
            bool ret = true;
            Type[] allTypes = assembly.GetTypes();

            foreach(Type type in allTypes)
            {
                MemberInfo[] members;
                if (_membersForType.TryGetValue(type, out MemberInfo[] cachedMembers))
                {
                    members = cachedMembers;
                }
                else
                {
                    members = type.GetMembers(Utilities.AnyBindingFlags);
                    _membersForType.Add(type, members);
                }

                foreach (TypeDefinition typeDefinition in typeDefinitions)
                {
                    if (typeDefinition.Type != null) continue;
                    if (typeDefinition.Resolve(type, members, out Dictionary<DefinitionInfo, MemberInfo> definitionMapping, out Type outType))
                    {
                        // Success
                        _logAction?.Invoke($"Found type for definition \"{typeDefinition.DefinitionName}\": \"{outType.FullName}\"");
                        // TODO do somthing with definitionMapping :P
                    }
                    else
                    {
                        ret = false;
                    }
                }
            }

            return ret;
        }

        public string GetUnresolvedTypes(Assembly assembly)
        {
            if (TypeDefinitions.TryGetValue(assembly, out List<TypeDefinition> typeDefs))
            {
                string outText = string.Empty;
                foreach(TypeDefinition typeDefinition in typeDefs)
                {
                    if(typeDefinition.Type == null)
                    {
                        outText += $"'{typeDefinition.DefinitionName}', ";
                    }
                }
                return outText.Substring(0, outText.Length - 2);
            }
            return "None";
        }

        public void Cleanup()
        {
            _membersForType.Clear();
        }

        public class TypeDefinition
        {
            internal Action<TypeDefinition> onTypeFound;
            public string DefinitionName { get; private set; }
            public Type Type { get; private set; }
            public List<DefinitionInfo> Definitions { get; private set; } = new List<DefinitionInfo>();

            //public HashSet<TypeDefinition> Requires { get; private set; } = new HashSet<TypeDefinition>();
            private object _bindingHost = null;
            private MemberInfo _bindingTarget = null;

            public bool? IsStaticType { get; private set; }
            public bool? IsAbstractType { get; private set; }
            public bool? IsSealedType { get; private set; }
            public MemberVisibility TypeVisibility { get; private set; }

            public string SpecialDebug { get; set; } = string.Empty;

            internal void SpecialDebugLog(Type type, string msg)
            {
                if (type == null) return;
                if (string.IsNullOrEmpty(SpecialDebug)) return;
                if (type.Name.Equals(SpecialDebug))
                {
                    Logger.log.Notice(msg);
                }
            }

            public TypeDefinition(MemberVisibility visibility = MemberVisibility.Any, bool? isStatic = null, bool? isAbstract = null, bool? isSealed = null, string definitionName = "UnnamedType")
            {
                DefinitionName = definitionName;
                TypeVisibility = visibility;
                IsStaticType = isStatic;
                IsAbstractType = isAbstract;
                IsSealedType = isSealed;
            }

            public bool AllRequirementsMet
            {
                get
                {
                    return !Definitions.Any(x => typeof(RequirementDefinitionInfo).IsInstanceOfType(x));
                }
            }

            public static bool VisibilityMatches(MemberVisibility visibility, MemberInfo memberInfo)
            {
                switch (memberInfo)
                {
                    case FieldInfo fi:
                        return VisibilityMatches(visibility, fi);
                    case PropertyInfo pi:
                        return VisibilityMatches(visibility, pi);
                    case MethodInfo mi:
                        return VisibilityMatches(visibility, mi);
                    default:
                        throw new ArgumentException($"Unhandled {nameof(MemberInfo)} Type: \"{memberInfo.GetType()}\"");
                }
            }

            public static bool VisibilityMatches(MemberVisibility visibility, FieldInfo fieldInfo)
            {
                if (visibility == MemberVisibility.Any) return true;

                if (fieldInfo.IsPublic && visibility == MemberVisibility.Public) return true;
                if (fieldInfo.IsPrivate && visibility == MemberVisibility.Private) return true;
                if (fieldInfo.IsAssembly && visibility == MemberVisibility.Assembly) return true;
                if (fieldInfo.IsFamily && visibility == MemberVisibility.Family) return true;

                return false;
            }

            public static bool VisibilityMatches(MemberVisibility visibility, PropertyInfo propertyInfo)
            {
                if (propertyInfo.GetMethod == null) return false;
                return VisibilityMatches(visibility, propertyInfo.GetMethod);
            }

            public static bool VisibilityMatches(MemberVisibility visibility, MethodInfo methodInfo)
            {
                if (visibility == MemberVisibility.Any) return true;
                
                if (methodInfo.IsPublic && visibility == MemberVisibility.Public) return true;
                if (methodInfo.IsPrivate && visibility == MemberVisibility.Private) return true;
                if (methodInfo.IsAssembly && visibility == MemberVisibility.Assembly) return true;
                if (methodInfo.IsFamily && visibility == MemberVisibility.Family) return true;

                return false;
            }

            public bool Resolve(Type type, MemberInfo[] members, out Dictionary<DefinitionInfo, MemberInfo> definitionMapping, out Type outType)
            {
                definitionMapping = null;
                outType = null;
                SpecialDebugLog(type, $"Resolve() called for type '{SpecialDebug}' on definition '{this.DefinitionName}'.");
                if (!ResolveRequirements()) return false;
                SpecialDebugLog(type, $"All Required Types have been resolved for type '{SpecialDebug}' on definition '{this.DefinitionName}'.");

                List<DefinitionInfo> definitions = new List<DefinitionInfo>(Definitions.ToArray());

                Dictionary<DefinitionInfo, MemberInfo> defToMember = new Dictionary<DefinitionInfo, MemberInfo>();

                DefinitionInfo matchingDefInfo;
                foreach (MemberInfo memberInfo in members)
                {
                    SpecialDebugLog(type, $"Testing Member: '{memberInfo.Name}' with type '{Utilities.GetMemberType(memberInfo)?.Name}' Memberinfo:'{MemberInfoTypeFromMemberInfo(memberInfo)}'.");

                    matchingDefInfo = null;
                    foreach (DefinitionInfo definitionInfo in definitions)
                    {
                        if(MatchesMember(definitionInfo, memberInfo))
                        {
                            matchingDefInfo = definitionInfo;
                            break;
                        }
                    }
                    if(matchingDefInfo != null)
                    {
                        definitions.Remove(matchingDefInfo);
                        defToMember.Add(matchingDefInfo, memberInfo);
                        SpecialDebugLog(type, $"Member '{memberInfo.Name}' with type '{Utilities.GetMemberType(memberInfo)?.Name}' matched - {definitions.Count} remaining definitions - for type '{SpecialDebug}' on definition '{this.DefinitionName}'.");
                    }
                }
                foreach (DefinitionInfo definitionInfo in definitions)
                {
                    SpecialDebugLog(type, $"Unresolved Definition: MemberType:'{definitionInfo.MemberInfo}', Type:'{definitionInfo.MemberType}', IsStatic:'{definitionInfo.IsStatic}', Visibility:'{ definitionInfo.Visibility}'");
                }

                if (defToMember.Count == Definitions.Count)
                {
                    // All definitions matched so far
                    SpecialDebugLog(type, $"All member defenitions have been met so far for type '{SpecialDebug}' on definition '{this.DefinitionName}'.");

                    if (TypeVisibility != MemberVisibility.Any)
                    {
                        if (type.IsPublic && TypeVisibility != MemberVisibility.Public) return false;
                        if (type.IsNotPublic && TypeVisibility == MemberVisibility.Public) return false;
                    }

                    if(IsStaticType.HasValue)
                    {
                        if ((type.IsAbstract && type.IsSealed) != IsStaticType.Value)
                            return false;
                    }

                    if (IsSealedType.HasValue)
                    {
                        if ((type.IsSealed) != IsSealedType.Value)
                            return false;
                    }

                    if (IsAbstractType.HasValue)
                    {
                        if ((type.IsAbstract) != IsAbstractType.Value)
                            return false;
                    }

                    Type = type;
                    definitionMapping = defToMember;
                    outType = Type;
                    SetOrInvokeBinding();
                    onTypeFound?.Invoke(this);
                    return true;
                }

                return false;
            }

            public static bool MatchesMember(DefinitionInfo definitionInfo, MemberInfo memberInfo)
            {
                if (definitionInfo == null || memberInfo == null) return false;

                switch (memberInfo)
                {
                    case FieldInfo fi:
                        if (!typeof(FieldDefinitionInfo).IsInstanceOfType(definitionInfo)) return false;
                        var fieldDefInfo = definitionInfo as FieldDefinitionInfo;
                        return fieldDefInfo.IsStatic == fi.IsStatic
                            && fieldDefInfo.IsReadonly == fi.IsInitOnly
                            && (fieldDefInfo.MemberType?.IsAssignableFrom(fi.FieldType) ?? false)
                            && VisibilityMatches(fieldDefInfo.Visibility, fi);
                    case PropertyInfo pi:
                        if (!typeof(PropertyDefinitionInfo).IsInstanceOfType(definitionInfo)) return false;
                        var propertyDefInfo = definitionInfo as PropertyDefinitionInfo;
                        return propertyDefInfo.IsStatic == (pi?.GetMethod?.IsStatic ?? false)
                            && (propertyDefInfo.MemberType?.IsAssignableFrom(pi.PropertyType) ?? false)
                            && VisibilityMatches(propertyDefInfo.Visibility, pi);
                    case MethodInfo mi:
                        if (!typeof(MethodDefinitionInfo).IsInstanceOfType(definitionInfo)) return false;
                        var methodDefInfo = definitionInfo as MethodDefinitionInfo;
                        return methodDefInfo.IsStatic == mi.IsStatic
                            && (mi.ReturnType.Equals(typeof(void)) || (methodDefInfo.ReturnType?.IsAssignableFrom(mi?.ReturnType) ?? false))
                            && ParametersMatch(methodDefInfo, mi)
                            && VisibilityMatches(methodDefInfo.Visibility, mi);
                    default:
                        break;
                }
                return false;
            }

            private static bool ParametersMatch(MethodDefinitionInfo methodDefInfo, MethodInfo mi)
            {
                ParameterInfo[] parameters = mi.GetParameters();
                if (methodDefInfo.ParameterTypes == null)
                {
                    if (parameters.Length == 0)
                        return true;
                    return false;
                }

                List<Type> types = new List<Type>(methodDefInfo.ParameterTypes);

                foreach(ParameterInfo parameterInfo in parameters)
                {
                    if (types.Contains(parameterInfo.ParameterType)) {
                        types.Remove(parameterInfo.ParameterType);
                    }
                }

                return types.Count == 0;
            }

            public bool ResolveRequirements()
            {
                for(int i = 0; i < Definitions.Count; i++)
                {
                    DefinitionInfo definition = Definitions[i];

                    if(typeof(RequirementDefinitionInfo).IsInstanceOfType(definition))
                    {
                        var newDefinition = (definition as RequirementDefinitionInfo).ResolveFromRequiredType();

                        if (newDefinition == null)
                            return false;

                        //Logger.log.Notice($"{(definition as RequirementDefinitionInfo).RequiredType.DefinitionName} type resolution returned: {newDefinition.MemberType.Name} - {newDefinition.GetType().Name}");

                        Definitions[i] = newDefinition;
                    }
                }
                return true;
            }

            public TypeDefinition BindTo(object host, MemberInfo memberInfo)
            {
                switch(memberInfo)
                {
                    case FieldInfo fi:
                        if (!fi.FieldType.Equals(typeof(Type)))
                            throw new ArgumentException($"Field Type of Field \"{fi.Name}\" must be Type!");
                        _bindingTarget = memberInfo;
                        break;
                    case PropertyInfo pi:
                        if(!pi.PropertyType.Equals(typeof(Type)))
                            throw new ArgumentException($"Property Type of Property \"{pi.Name}\" must be Type!");
                        _bindingTarget = memberInfo;
                        break;
                    case MethodInfo methodInfo:
                        ParameterInfo[] parameters = methodInfo.GetParameters();
                        if(parameters.Length != 1 || !parameters[0].ParameterType.Equals(typeof(Type)))
                            throw new ArgumentException($"Method \"{methodInfo.Name}\" must contain (only) one argument with Type Type!");
                        _bindingTarget = memberInfo;
                        break;
                    default:
                        throw new ArgumentException($"Unhandled MemberInfo Type: {memberInfo.GetType()}");
                }
                _bindingHost = host;
                return this;
            }

            public void SetOrInvokeBinding()
            {
                if (_bindingTarget == null || _bindingHost == null) return;

                switch(_bindingTarget)
                {
                    case FieldInfo field:
                        field.SetValue(_bindingHost, Type);
                        break;
                    case PropertyInfo property:
                        property.SetValue(_bindingHost, Type);
                        break;
                    case MethodInfo method:
                        method.Invoke(_bindingHost, new object[] { Type });
                        break;
                }
            }

            public TypeDefinition AddMultipleSameFieldDefinitions(int number, Type fieldValueType, MemberVisibility visibility, bool isStatic = false, bool isReadonly = false)
            {
                for (int i = 0; i < number; i++)
                    AddFieldDefinition(fieldValueType, visibility, isStatic, isReadonly);
                return this;
            }

            public TypeDefinition AddFieldDefinition(Type fieldValueType, MemberVisibility visibility, bool isStatic = false, bool isReadonly = false)
            {
                AddMemberDefinition(new FieldDefinitionInfo(fieldValueType, visibility, isStatic, isReadonly));
                return this;
            }

            public TypeDefinition AddFieldDefinition(TypeDefinition requiredType, MemberVisibility visibility, bool isStatic = false, bool isReadonly = false)
            {
                if (requiredType == this) throw new ArgumentException($"{nameof(requiredType)} can not be itself!");
                return AddRequirementMemberDefinition(MemberInfoType.Field, requiredType, visibility, isStatic, isReadonly);
            }

            public TypeDefinition AddPropertyDefinition(Type propertyValueType, MemberVisibility visibility, bool isStatic = false)
            {
                AddMemberDefinition(new PropertyDefinitionInfo(propertyValueType, visibility, isStatic));
                return this;
            }

            public TypeDefinition AddPropertyDefinition(TypeDefinition requiredType, MemberVisibility visibility, bool isStatic = false)
            {
                if (requiredType == this) throw new ArgumentException($"{nameof(requiredType)} can not be itself!");
                return AddRequirementMemberDefinition(MemberInfoType.Property, requiredType, visibility, isStatic);
            }

            public TypeDefinition AddMethodDefinition(Type returnValueType, MemberVisibility visibility, bool isStatic = false, Type[] parameterTypes = null)
            {
                AddMemberDefinition(new MethodDefinitionInfo(returnValueType, visibility, parameterTypes, isStatic));
                return this;
            }

            public TypeDefinition AddMethodDefinition(TypeDefinition requiredType, MemberVisibility visibility, bool isStatic = false, Type[] parameterTypes = null)
            {
                if (requiredType == this) throw new ArgumentException($"{nameof(requiredType)} can not be itself!");
                return AddRequirementMemberDefinition(MemberInfoType.Method, requiredType, visibility, isStatic, false, parameterTypes);
            }

            private TypeDefinition AddMemberDefinition(DefinitionInfo definitionInfo)
            {
                Definitions.Add(definitionInfo);
                return this;
            }

            private TypeDefinition AddRequirementMemberDefinition(MemberInfoType memberInfoType, TypeDefinition requiredType, MemberVisibility visibility, bool isStatic, bool isReadonly = false, Type[] parameterTypes = null)
            {
                if (requiredType == this) throw new ArgumentException($"{nameof(requiredType)} can not be itself!");
                Definitions.Add(new RequirementDefinitionInfo(memberInfoType, requiredType, visibility, parameterTypes, isStatic, isReadonly));
                return this;
            }

            public enum MemberInfoType
            {
                Field,
                Property,
                Method,
                Error
            }

            public static MemberInfoType MemberInfoTypeFromMemberInfo(MemberInfo memberInfo)
            {
                switch (memberInfo)
                {
                    case FieldInfo fi:
                        return MemberInfoType.Field;
                    case PropertyInfo pi:
                        return MemberInfoType.Property;
                    case MethodInfo mi:
                        return MemberInfoType.Method;
                    default:
                        return MemberInfoType.Error;
                }
            }

            public class MethodDefinitionInfo : DefinitionInfo
            {
               
                public Type[] ParameterTypes { get; private set; }
                public Type ReturnType {
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

            public class FieldDefinitionInfo : DefinitionInfo
            {
                public bool IsReadonly { get; private set; }
                public FieldDefinitionInfo(Type memberValueType, MemberVisibility visibility, bool staticField, bool isReadonly = false) : base(MemberInfoType.Field, memberValueType, visibility, staticField)
                {
                    IsReadonly = isReadonly;
                }
            }

            public class PropertyDefinitionInfo : DefinitionInfo
            {
                public PropertyDefinitionInfo(Type memberValueType, MemberVisibility visibility, bool staticField) : base(MemberInfoType.Property, memberValueType, visibility, staticField)
                {
                }
            }

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
                    switch(MemberInfo)
                    {
                        case MemberInfoType.Field:
                            return new FieldDefinitionInfo(type, Visibility, IsStatic, IsReadonly);
                        case MemberInfoType.Property:
                            return new PropertyDefinitionInfo(type, Visibility, IsStatic);
                        case MemberInfoType.Method:
                            return new MethodDefinitionInfo(type, Visibility, ParameterTypes, IsStatic);
                    }
                    return null;
                }
            }

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
                
                public enum MemberVisibility
                {
                    Private = 1,
                    Family = 2,
                    Assembly = 4,
                    Public = 8,
                    Any = 16
                }
            }
        }

    }
}
