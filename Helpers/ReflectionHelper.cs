using System;
using System.Reflection;

namespace MoreOrLessStars.Helpers {
    public static class ReflectionHelper {
        public static T GetField<T>(this object target, string fieldName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic) {
            Type type = target.GetType();
            FieldInfo fi = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            T result = (T)fi.GetValue(target);
            return result;
        }
    }
}
