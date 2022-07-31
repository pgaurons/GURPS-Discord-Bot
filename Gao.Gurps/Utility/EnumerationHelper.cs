using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gao.Gurps.Utility
{
    /// <summary>
    /// Helper methods for Enumeration types.
    /// </summary>
    public static class EnumerationHelper
    {
        /// <summary>
        /// Converts a string to an enumeration value, if a default value is provided, then that will be used if the string can't be converted.
        /// </summary>
        /// <param name="enumType">The type of the enum to convert to.</param>
        /// <param name="source">The string to convert.</param>
        /// <param name="defaultValue">A default value to use if the string does not match any enum value.</param>
        /// <returns>an object representing the enum value that matches the provided string</returns>
        /// <exception cref="System.ArgumentException">If type is not an enum type</exception>
        /// <exception cref="System.ArgumentException">If default value is provided and does not match the type provided by enum type</exception>
        public static object StringToEnum(Type enumType, string source, Object defaultValue)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");
            if (source == null)
                throw new ArgumentNullException("source");

            object returnValue;
            //Parameter sentinal.
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Conversion type must be an enumeration.", "enumType");
            }
            if (defaultValue != null && defaultValue.GetType() != enumType)
            {
                throw new ArgumentException(
                    "If a default value is provided, it must match the type of the conversion.", "defaultValue");
            }
            //Remove white space and punctuation and case desensitize the string.
            var massagedString = Regex.Replace(source.ToUpperInvariant(), @"[\s.,]*", string.Empty, RegexOptions.Multiline).Replace("-", "");
            try
            {
                returnValue = Enum.Parse(enumType, massagedString, true);
            }
            catch (Exception)
            {
                if (defaultValue != null)
                    returnValue = defaultValue;
                else
                    throw;
            }
            return returnValue;
        }

        /// <summary>
        /// Converts a string to an enumeration value.
        /// </summary>
        /// <param name="enumType">The type of the enum to convert to.</param>
        /// <param name="source">The string to convert.</param>
        /// <returns>an object representing the enum value that matches the provided string</returns>
        /// <exception cref="System.ArgumentException">If type is not an enum type</exception>
        /// <exception cref="System.ArgumentException">If default value is provided and does not match the type provided by enum type</exception>
        public static object StringToEnum(Type enumType, string source)
        {
            return StringToEnum(enumType, source, null);
        }

        /// <summary>
        /// Generic wrapper of StringToEnum that converts to the given enum type.
        /// </summary>
        /// <typeparam name="TEnum">Enum to convert to.</typeparam>
        /// <param name="source">String to convert</param>
        /// <returns>The enum equivalent of the provided string.</returns>
        public static TEnum StringTo<TEnum>(string source)
        {
            return (TEnum)StringToEnum(typeof(TEnum), source);
        }

        /// <summary>
        /// Generic Wrapper of string to enum with a provided default value.
        /// </summary>
        /// <typeparam name="TEnum">The type to convert to.</typeparam>
        /// <param name="source">The string to convert.</param>
        /// <param name="defaultValue">A value to use if the string does not match the enumeration</param>
        /// <returns>The enum equivalent of the provided string.</returns>
        public static TEnum StringTo<TEnum>(string source, TEnum defaultValue)
        {
            return (TEnum)StringToEnum(typeof(TEnum), source, defaultValue);
        }

        /// <summary>
        /// Converts an enumeration value to a neatly printed string.
        /// </summary>
        /// <param name="value">Enum to convert</param>
        /// <returns>A nicely spaced version of that enum value</returns>
        public static string ToFormattedString(this Enum value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return PascalToNormalSpaced(value.ToString());

        }

        public static string ToPreferredString(this Enum value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var returnValue = string.Empty;
            //check if it has the human readable text attribute.
            var type = value.GetType();
            var info = type.GetField(value.ToString());
            var attribute = info.GetCustomAttributes(typeof(EnumHumanReadableTextAttribute), false).FirstOrDefault() as EnumHumanReadableTextAttribute;
            if (attribute != null)
                returnValue = attribute.Text;
            else
                returnValue = value.ToFormattedString();
            return returnValue;
        }

        /// <summary>
        /// Converts a pascal spaced value to a normal spaced value.
        /// </summary>
        /// <param name="pascalCased">A string in pascal spacing</param>
        /// <returns>That string value nicely spaced</returns>
        public static string PascalToNormalSpaced(string pascalCased)
        {
            const string pascalCaseSplitterExpression =
                "([A-Z][a-z]+)|" + //A capital letter proceeded by lowercase letters.;
                "([0-9]+)|" + //Any numbers
                "([A-Z]+(?=[A-Z][a-z]))|" + //Acronym before a word
                "([A-Z]+(?=[0-9]+))";  //Acronym before a number.
            var splitter = new Regex(pascalCaseSplitterExpression);
            var returnValue = new StringBuilder();

            foreach (var match in splitter.Matches(pascalCased))
            {
                returnValue.Append(match + " ");
            }

            return returnValue.ToString().Trim();
        }
    }
}
