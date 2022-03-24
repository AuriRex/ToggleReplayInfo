using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ToggleReplayInfo.TypeReflection.Wrappers
{
    public abstract class WrapperBase
    {

        protected static Dictionary<Type, List<PropertyInfo>> propLookup = new Dictionary<Type, List<PropertyInfo>>();
        protected static Dictionary<Type, List<FieldInfo>> fieldLookup = new Dictionary<Type, List<FieldInfo>>();
        protected readonly object thisObject;
        protected readonly Type thisObjectType;

        protected List<PropertyInfo> PropsForThisType
        {
            get
            {
                propLookup.TryGetValue(thisObjectType, out var list);
                return list;
            }
        }

        protected List<FieldInfo> FieldsForThisType
        {
            get
            {
                fieldLookup.TryGetValue(thisObjectType, out var list);
                return list;
            }
        }

        public bool HasValue
        {
            get
            {
                return thisObject != null;
            }
        }

        public WrapperBase(object wrappedObject)
        {
            thisObject = wrappedObject;
            thisObjectType = wrappedObject.GetType();

            if(!propLookup.ContainsKey(thisObjectType))
            {
                Logger.Debug($"Setting up wrapper for type \"{thisObjectType.Name}\"! ({this.GetType().Name})");

                propLookup.Add(thisObjectType, thisObjectType.GetProperties(Utilities.AnyBindingFlags).ToList());
                fieldLookup.Add(thisObjectType, thisObjectType.GetFields(Utilities.AnyBindingFlags).ToList());
            }
        }

        public T GetFromFirstFieldWithType<T>(Type t)
        {
            var val = GetFromFirstFieldWithType(t);
            if (val != null) return (T) val;
            return default;
        }

        public object GetFromFirstFieldWithType(Type t)
        {
            return FieldsForThisType.Where(fi => fi.FieldType == t).FirstOrDefault()?.GetValue(thisObject);
        }

        public T GetFromFirstPropWithType<T>(Type t)
        {
            var val = GetFromFirstPropWithType(t);
            if (val != null) return (T) val;
            return default;
        }

        public object GetFromFirstPropWithType(Type t)
        {
            return PropsForThisType.Where(pi => pi.GetGetMethod(true)?.ReturnType == t).FirstOrDefault()?.GetValue(thisObject);
        }

        public override bool Equals(object obj)
        {
            if(obj is WrapperBase wb)
            {
                return thisObject == wb.thisObject;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return thisObject.GetHashCode();
        }
    }
}
