using System;
using System.Collections.Generic;
using System.Reflection;
using ToggleReplayInfo.TypeReflection.Definitions;

namespace ToggleReplayInfo.TypeReflection.Core
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
    }
}
