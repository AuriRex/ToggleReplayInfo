using System;
using System.Reflection;
using ToggleReplayInfo.TypeReflection.Core;
using static ToggleReplayInfo.TypeReflection.Utilities;

namespace ToggleReplayInfo.TypeReflection
{
    public static class BindingHelperExtensions
    {
        public static TypeDefinition Register(this TypeDefinition self, TypeDefinitionManager manager, Assembly asm, Action<TypeDefinition> onTypeFound = null)
        {
            manager.RegisterType(asm, self, onTypeFound);

            return self;
        }

        public static TypeDefinition BindToField(this TypeDefinition self, object host, string fieldname)
        {
            Type hostType = host.GetType();

            FieldInfo fi = hostType.GetField(fieldname, AnyBindingFlags);

            self.BindTo(host, fi);

            return self;
        }

        public static TypeDefinition BindToProperty(this TypeDefinition self, object host, string propertyname)
        {
            Type hostType = host.GetType();

            PropertyInfo pi = hostType.GetProperty(propertyname, AnyBindingFlags);

            self.BindTo(host, pi);

            return self;
        }

        public static TypeDefinition BindToAction(this TypeDefinition self, object host, Action<Type> action)
        {
            self.BindTo(host, action.Method);

            return self;
        }
    }
}
