using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace WinstantPay.Common.Extension
{

    public static class EnumExtensions
    {
        public static IDictionary<string, int> GetDictionary<T>()
        {
            Type type = typeof(T);
            if (type.IsEnum)
            {
                var values = Enum.GetValues(type);
                var result = new Dictionary<string, int>();
                foreach (var value in values)
                {
                    result.Add(value.ToString(), (int)value);
                }
                return result;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }


    public static class EnumHelper<T>
    {
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
        }

        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static IList<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<string> GetDisplayValues(Enum value)
        {
            return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }

        private static string lookupResource(Type resourceManagerProvider, string resourceKey)
        {
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetProperty))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    var res = resourceManager.GetString(resourceKey);
                    if (!String.IsNullOrEmpty(res))
                        return res;
                    foreach (DictionaryEntry dictionaryEntry in resourceManager.GetResourceSet(CultureInfo.CurrentCulture, true,true))
                    {
                        if(dictionaryEntry.Key.ToString().Replace(".", "_")== resourceKey)
                            res = dictionaryEntry.Value.ToString();

                    }

                    return res;
                }
            }

            return resourceKey; // Fallback with the key name
        }

        public static string GetDisplayValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes == null) return string.Empty;

            if (descriptionAttributes[0].ResourceType != null)
                return lookupResource(descriptionAttributes[0].ResourceType, descriptionAttributes[0].Name);

            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }


    }
}