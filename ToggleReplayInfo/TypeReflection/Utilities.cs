using System.Linq;
using System.Reflection;
using ToggleReplayInfo.TypeReflection.Attributes;

namespace ToggleReplayInfo.TypeReflection
{
    public class Utilities
    {

        public const BindingFlags AnyBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

        public static bool AllMembersPopulated<T>(MemberTypes memberType, T instance = null) where T : class
        {
            foreach (MemberInfo memberInfo in typeof(T).GetMembers(AnyBindingFlags))
            {
                if (memberInfo.GetCustomAttributes().Any(x => x.GetType().Equals(typeof(IgnoreVerification)))) continue;
                if (memberInfo.MemberType.Equals(memberType))
                {
                    switch (memberInfo.MemberType)
                    {
                        case MemberTypes.Field:
                            FieldInfo fi = memberInfo as FieldInfo;
                            if (fi.GetValue(instance) == null)
                            {
                                return false;
                            }
                            break;
                        case MemberTypes.Property:
                            PropertyInfo pi = memberInfo as PropertyInfo;
                            if (pi.GetValue(instance) == null)
                            {
                                return false;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return true;
        }

    }
}
