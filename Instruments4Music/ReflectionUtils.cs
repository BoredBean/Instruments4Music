using System.Reflection;

namespace Instruments4Music
{
    static class ReflectionUtils
    {
        public static T GetPrivateField<T>(this object obj, string field)
        {
            return (T)obj.GetType().GetField(field, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
        }
    }
}
