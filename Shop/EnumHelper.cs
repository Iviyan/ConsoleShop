using System;
using System.Collections;
using System.ComponentModel;

namespace Shop
{
    public static class EnumHelper
    {
        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        /// <example><![CDATA[string desc = myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;]]></example>
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        public static string Description(this Enum value)
        {
            var attribute = value.GetAttributeOfType<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string[] GetArrayFromEnum<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T)) as IList;
            string[] strVals = new string[values.Count];

            for (int i = 0; i < values.Count; i++)
                strVals[i] = (values[i] as Enum)?.Description() ?? values[i].ToString();

            return strVals;
        }
    }
}
