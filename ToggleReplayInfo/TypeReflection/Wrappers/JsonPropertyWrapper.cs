using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ToggleReplayInfo.TypeReflection.Wrappers
{
    public abstract class JsonPropertyWrapper : WrapperBase
    {
        protected static Dictionary<Type, Dictionary<string, PropertyInfo>> jsonPropLookup = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        protected Dictionary<string, PropertyInfo> JsonDictForThisType
        {
            get
            {
                jsonPropLookup.TryGetValue(thisObjectType, out var dict);
                return dict;
            }
        }

        public JsonPropertyWrapper(object wrappedObject) : base(wrappedObject)
        {

            if(!jsonPropLookup.TryGetValue(thisObjectType, out var _))
            {
                var dict = new Dictionary<string, PropertyInfo>();

                var properties = thisObjectType.GetProperties(Utilities.AnyBindingFlags);

                foreach (var prop in properties)
                {
                    if (TryGetNameFromJsonAttributeOnProperty(prop, out string name))
                    {
                        Logger.Debug($" -[JSON] \"{name}\" discovered! Prop:({prop.Name})");
                        dict.Add(name, prop);
                    }
                }

                jsonPropLookup.Add(thisObjectType, dict);
            }
        }

        public Type GetWrappedType() => thisObjectType;
        public object GetWrappedObject() => thisObject;

        public TVal Get<TVal>(string name)
        {
            var obj = Get(name);

            if (obj != null)
                return (TVal) obj;
                
            return default;
        }

        public object Get(string name)
        {
            if (JsonDictForThisType.TryGetValue(name, out var prop))
            {
                return prop.GetValue(thisObject);
            }
            return null;
        }

        public static bool TryGetNameFromJsonAttributeOnProperty(PropertyInfo prop, out string name)
        {
            if(TryGetJsonAttribute(prop, out var jpa))
            {
                name = jpa.PropertyName;
                return true;
            }

            name = null;
            return false;
        }

        private static bool TryGetJsonAttribute(PropertyInfo property, out JsonPropertyAttribute jpa)
        {
            var attributes = property.GetCustomAttributes();

            foreach (var attr in attributes)
            {
                if (attr is JsonPropertyAttribute jpAttr)
                {
                    jpa = jpAttr;
                    return true;
                }
            }
            jpa = null;
            return false;
        }
    }
}
