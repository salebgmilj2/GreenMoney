using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace GM.Utility
{
    public class EnumUtils
    {
        public static string StringValueOf(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static T EnumValueOf<T>(string value)
        {
            var names = Enum.GetNames(typeof(T));
            var namesList = names.Where(name => StringValueOf((Enum)Enum.Parse(typeof(T), name)).Equals(value, StringComparison.InvariantCultureIgnoreCase));
            return (T)namesList.Select(name => Enum.Parse(typeof(T), name)).FirstOrDefault();
        }

        public static string GetDescription(Enum value)
        {
            var attribute = StringValueOf(value);
            return attribute;
        }

        public static List<string> GetDescriptions<T>()
        {
            var values = Enum.GetValues(typeof(T));
            return (values.Cast<Enum>().Select(StringValueOf).OrderBy(i => i)).ToList();
        }

        public static T ConvertStringToEnum<T>(string enumString, string bindToString = "")
        {
            if (enumString.Length == 0)
            {
                enumString = "Unknown"; //default type value
            }

            var typeName = string.Concat(bindToString, enumString);
            var clearTypeName = RemoveSpecialChars(typeName);
            var type = (T)Enum.Parse(typeof(T), clearTypeName, true);
            return type;
        }

        private static string RemoveSpecialChars(string s)
        {
            var len = s.Length;
            var s2 = new char[len];
            var i2 = 0;
            for (var i = 0; i < len; i++)
            {
                var c = s[i];
                if (c != ' ' && c != '-' && c != '(' && c != ')' && c != '/' && c != ',' && c != '\'')
                    s2[i2++] = c;
            }
            return new String(s2, 0, i2);
        }
    }
}