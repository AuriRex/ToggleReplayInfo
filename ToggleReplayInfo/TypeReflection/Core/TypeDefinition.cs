using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToggleReplayInfo.TypeReflection.Definitions;

namespace ToggleReplayInfo.TypeReflection.Core
{
    public class TypeDefinition
    {
        internal Action<TypeDefinition> onTypeFound;
        public string DefinitionName { get; private set; }
        public Type Type { get; private set; }
        public List<DefinitionInfo> Definitions { get; private set; } = new List<DefinitionInfo>();

        //public HashSet<TypeDefinition> Requires { get; private set; } = new HashSet<TypeDefinition>();
        private object _bindingHost = null;
        private MemberInfo _bindingTarget = null;

        public MemberLimits Limits { get; private set; }

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
                Logger.Log.Notice(msg);
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

        public TypeDefinition WithLimits(MemberLimits limits)
        {

            Limits = limits;

            return this;
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
            return memberInfo switch
            {
                FieldInfo fi => VisibilityMatches(visibility, fi),
                PropertyInfo pi => VisibilityMatches(visibility, pi),
                MethodInfo mi => VisibilityMatches(visibility, mi),
                _ => throw new ArgumentException($"Unhandled {nameof(MemberInfo)} Type: \"{memberInfo.GetType()}\""),
            };
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

            if (Limits != null)
            {
                if (Limits.MaxFields >= 0 && members.Count(m => m is FieldInfo) > Limits.MaxFields) return false;
                if (Limits.MaxProperties >= 0 && members.Count(m => m is PropertyInfo) > Limits.MaxProperties) return false;
            }

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
                    if (MatchesMember(definitionInfo, memberInfo))
                    {
                        matchingDefInfo = definitionInfo;
                        break;
                    }
                }
                if (matchingDefInfo != null)
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

                if (IsStaticType.HasValue)
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

            foreach (ParameterInfo parameterInfo in parameters)
            {
                if (types.Contains(parameterInfo.ParameterType))
                {
                    types.Remove(parameterInfo.ParameterType);
                }
            }

            return types.Count == 0;
        }

        public bool ResolveRequirements()
        {
            for (int i = 0; i < Definitions.Count; i++)
            {
                DefinitionInfo definition = Definitions[i];

                if (typeof(RequirementDefinitionInfo).IsInstanceOfType(definition))
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
            switch (memberInfo)
            {
                case FieldInfo fi:
                    if (!fi.FieldType.Equals(typeof(Type)))
                        throw new ArgumentException($"Field Type of Field \"{fi.Name}\" must be Type!");
                    _bindingTarget = memberInfo;
                    break;
                case PropertyInfo pi:
                    if (!pi.PropertyType.Equals(typeof(Type)))
                        throw new ArgumentException($"Property Type of Property \"{pi.Name}\" must be Type!");
                    _bindingTarget = memberInfo;
                    break;
                case MethodInfo methodInfo:
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    if (parameters.Length != 1 || !parameters[0].ParameterType.Equals(typeof(Type)))
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

            switch (_bindingTarget)
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

        public TypeDefinition AddPropertyWrappersForAllCurrentlyAddedFields(MemberVisibility visibility)
        {
            foreach (FieldDefinitionInfo def in Definitions.Where(d => d is FieldDefinitionInfo).ToArray())
            {
                AddPropertyDefinition(def.MemberType, visibility);
            }
            return this;
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

        public TypeDefinition AddMultipleSamePropertyDefinitions(int number, Type fieldValueType, MemberVisibility visibility, bool isStatic = false)
        {
            for (int i = 0; i < number; i++)
                AddPropertyDefinition(fieldValueType, visibility, isStatic);
            return this;
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

        

        public static MemberInfoType MemberInfoTypeFromMemberInfo(MemberInfo memberInfo)
        {
            return memberInfo switch
            {
                FieldInfo _ => MemberInfoType.Field,
                PropertyInfo _ => MemberInfoType.Property,
                MethodInfo _ => MemberInfoType.Method,
                _ => MemberInfoType.Error,
            };
        }

        

        public class MemberLimits
        {
            public int MaxFields { get; set; } = -1;
            public int MaxProperties { get; set; } = -1;
        }
    }

}
