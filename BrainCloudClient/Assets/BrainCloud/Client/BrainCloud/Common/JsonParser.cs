// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud
{
    using System;
    using System.Text;

    namespace Common
    {
        /// <summary>
        /// Helper tool to parse Json strings and pull values out of them without having to deserialize the Json data into an object.
        /// </summary>
        public static partial class JsonParser
        {
            private const char SPLIT_TOKEN = (char)0x1F;

            private static readonly StringBuilder sbHelper = null;

            static JsonParser()
            {
                sbHelper = new(2048);
            }

            // This is a helper function for the hierarchy functions to get the hierarchy minus the last value.
            private static string[] GetHierarchyMinusOne(string[] hierarchy)
            {
                string[] hMinusOne = new string[hierarchy.Length - 1];
                for (int i = 0; i < hMinusOne.Length; i++)
                {
                    hMinusOne[i] = hierarchy[i];
                }

                return hMinusOne;
            }

            /// <summary>
            /// Gets a full <see cref="string"/> value of a property within the Json string.
            /// </summary>
            /// <param name="jsonData">A valid Json <see cref="string"/>.</param>
            /// <param name="property">The name of the property you want within the Json string's highest hierarchy layer.</param>
            /// <returns>
            /// The full content of the <see cref="string"/> value for the property name including objects and arrays.
            /// <br><b>Note</b>: If the property isn't valid or not found it will return <see cref="string.Empty"/>.</br>
            /// </returns>
            public static string GetString(string jsonData, string property)
            {
                if (string.IsNullOrWhiteSpace(jsonData) ||
                    !(jsonData.StartsWith("{") && jsonData.EndsWith("}")) &&
                    !(jsonData.StartsWith("[") && jsonData.EndsWith("]")))
                {
                    // Should maybe log that this isn't a proper Json string?
                    // throw new Exception("jsonData is not a valid Json!");
                    return string.Empty;
                }

                char current, next;
                bool insideProperty = false;
                for (int i = 0; i < jsonData.Length; i++)
                {
                    current = jsonData[i];

                    if (current == '"' && jsonData[i - 1] != '\\')
                    {
                        insideProperty = !insideProperty;
                        if (insideProperty)
                        {
                            sbHelper.Clear();
                        }
                    }
                    else if (!insideProperty && current == ':')
                    {
                        next = jsonData[i + 1];
                        if (sbHelper.ToString() == property)
                        {
                            if (next == '{' || next == '[')
                            {
                                sbHelper.Clear();

                                int level = 0;

                                do
                                {
                                    current = jsonData[++i];

                                    switch (current)
                                    {
                                        case '{':
                                        case '[':
                                            level++;
                                            goto default;
                                        case '}':
                                        case ']':
                                            level--;
                                            goto default;
                                        case '"':
                                            if (jsonData[i - 1] != '\\')
                                            {
                                                while (++i < jsonData.Length)
                                                {
                                                    current = jsonData[i - 1];
                                                    next = jsonData[i];
                                                    sbHelper.Append(current);

                                                    if (next == '"' && current != '\\')
                                                    {
                                                        current = jsonData[i];
                                                        goto default;
                                                    }
                                                }

                                                throw new Exception("JsonParser could not parse this property's value!");
                                            }
                                            goto default;
                                        default:
                                            sbHelper.Append(current);
                                            continue;
                                    }
                                }
                                while (level > 0);

                                return sbHelper.ToString();
                            }
                            else // Not an array or object
                            {
                                i++;
                                sbHelper.Clear();
                                if (next == '"') // String value
                                {
                                    i++;
                                    while (++i < jsonData.Length)
                                    {
                                        current = jsonData[i - 1];
                                        next = jsonData[i];
                                        sbHelper.Append(current);

                                        if (next == '"' && current != '\\')
                                        {
                                            break;
                                        }
                                    }

                                    if (sbHelper.ToString() == "\"" || sbHelper.ToString() == "\",") // We must have grabbed an empty string
                                    {
                                        sbHelper.Clear();
                                    }
                                    else if (i >= jsonData.Length)
                                    {
                                        throw new Exception("JsonParser could not parse this property's value!");
                                    }
                                }
                                else // Non-string value
                                {
                                    while (next != ',' && next != '}' && next != ']')
                                    {
                                        current = jsonData[i];
                                        next = jsonData[++i];
                                        sbHelper.Append(current);
                                    }
                                }

                                return sbHelper.ToString();
                            }
                        }
                        else if (next == '{' || next == '[')
                        {
                            int level = 0;

                            do
                            {
                                current = jsonData[++i];

                                switch (current)
                                {
                                    case '{':
                                    case '[':
                                        level++;
                                        continue;
                                    case '}':
                                    case ']':
                                        level--;
                                        continue;
                                    case '"':
                                        if (jsonData[i - 1] != '\\')
                                        {
                                            while (++i < jsonData.Length)
                                            {
                                                current = jsonData[i - 1];
                                                next = jsonData[i];

                                                if (next == '"' && current != '\\')
                                                {
                                                    goto default;
                                                }
                                            }

                                            return string.Empty; // If we reached the end of the string, then the Json doesn't contain the property
                                        }
                                        continue;
                                    default:
                                        continue;
                                }
                            }
                            while (level > 0);
                        }
                    }
                    else if (insideProperty)
                    {
                        sbHelper.Append(current);
                    }
                }

                return string.Empty;
            }

            /// <summary>
            /// Gets a full <see cref="string"/> value of a property at the end of the hierarchy within the Json string.
            /// </summary>
            /// <param name="jsonData">A valid Json <see cref="string"/>.</param>
            /// <param name="hierarchy">The list of properties, in progressive order, to parse through the Json string's object hierarchy.</param>
            /// <returns>
            /// The full content of the <see cref="string"/> value for the property name at the end of the hierarchy including objects and arrays.
            /// <br><b>Note</b>: If the hierarchy isn't valid or the property is not found it will return <see cref="string.Empty"/>.</br>
            /// </returns>
            public static string GetString(string jsonData, params string[] hierarchy)
            {
                if (hierarchy != null && hierarchy.Length > 0)
                {
                    int level = 0;
                    while (GetString(jsonData, hierarchy[level]) is string property)
                    {
                        if (!string.IsNullOrWhiteSpace(property))
                        {
                            jsonData = property;
                        }
                        else
                        {
                            return string.Empty;
                        }

                        if (++level >= hierarchy.Length)
                        {
                            break;
                        }
                    }

                    if (level < hierarchy.Length)
                    {
                        return string.Empty;
                    }

                    return jsonData;
                }

                return string.Empty;
            }

            /// <summary>
            /// Gets a <see cref="string"/> array that contains full <see cref="string"/> values of the property's array within the Json string.
            /// </summary>
            /// <param name="jsonData">A valid Json <see cref="string"/>.</param>
            /// <param name="property">
            /// The name of the property you want within the Json string's highest hierarchy layer.
            /// <br><b>Note</b>: Leave this empty if <paramref name="jsonData"/> is already a serialized array string.</br>
            /// </param>
            /// <returns>
            /// A <see cref="string"/> array that contains full <see cref="string"/> values of the property's array including objects and arrays.
            /// <br><b>Note</b>: If the property isn't valid or not found it will return <b>null</b>.</br>
            /// </returns>
            public static string[] GetArrayString(string jsonData, string property = "")
            {
                if (!string.IsNullOrWhiteSpace(property))
                {
                    jsonData = GetString(jsonData, property);
                }

                if (string.IsNullOrWhiteSpace(jsonData) ||
                    !(jsonData.StartsWith("[") && jsonData.EndsWith("]")))
                {
                    // Should maybe log that this isn't a proper Json array?
                    // throw new Exception($"{property} is not a Json array!");
                    return null;
                }

                char current;
                int i = 0, level = 1;
                sbHelper.Clear();
                while (level > 0)
                {
                    current = jsonData[++i];

                    switch (current)
                    {
                        case '{':
                        case '[':
                            level++;
                            goto default;
                        case '}':
                        case ']':
                            level--;
                            goto default;
                        case ',':
                            if (level == 1)
                            {
                                sbHelper.Append(SPLIT_TOKEN);
                                continue;
                            }
                            goto default;
                        case '"':
                            while (i < jsonData.Length)
                            {
                                current = jsonData[i];
                                sbHelper.Append(current);

                                if (jsonData[++i] == '"' && current != '\\')
                                {
                                    current = jsonData[i];
                                    goto default;
                                }
                            }

                            throw new Exception("JsonParser could not parse this property's array!");
                        default:
                            if (level != 0)
                            {
                                sbHelper.Append(current);
                            }
                            continue;
                    }
                }

                string arrays = sbHelper.ToString();
                if (!string.IsNullOrWhiteSpace(arrays))
                {
                    return arrays.Split(SPLIT_TOKEN);
                }

                return null;
            }

            /// <summary>
            /// Gets a <see cref="string"/> array that contains full <see cref="string"/> values of the property's array at the end of the hierarchy within the Json string.
            /// </summary>
            /// <param name="jsonData">A valid Json <see cref="string"/>.</param>
            /// <param name="hierarchy">The list of properties, in progressive order, to parse through the Json string's object hierarchy with the last property being an array.</param>
            /// <returns>
            /// A <see cref="string"/> array that contains full <see cref="string"/> values of the property's array at the end of the hierarchy including objects and arrays.
            /// <br><b>Note</b>: If the property isn't valid or not found it will return <b>null</b>.</br>
            /// </returns>
            public static string[] GetArrayString(string jsonData, params string[] hierarchy)
            {
                return GetArrayString(GetString(jsonData, GetHierarchyMinusOne(hierarchy)), hierarchy[hierarchy.Length - 1]);
            }

            /// <summary>
            /// Tries to get a full <see cref="string"/> value of a property within the Json string.
            /// </summary>
            /// <param name="jsonData">A valid Json <see cref="string"/>.</param>
            /// <param name="value">
            /// When this method returns, contains the full content of the <see cref="string"/> value for the property name including objects and arrays,
            /// if the property is found. Otherwise, it will return as <see cref="string.Empty"/>. This parameter is passed uninitialized.
            /// </param>
            /// <param name="property">The name of the property you want within the Json string's highest hierarchy layer.</param>
            /// <returns>
            /// <b>true</b> if the property is found and has a value. Otherwise <b>false</b>.
            /// <br><b>Note</b>: This will return <b>true</b> if the property contains <b>any</b> kind of value, including <b>null</b> and empty <b>objects</b> and <b>arrays</b>.</br>
            /// </returns>
            public static bool TryGetString(string jsonData, out string value, string property)
            {
                value = GetString(jsonData, property);
                return !string.IsNullOrWhiteSpace(value);
            }

            /// <summary>
            /// Tries to get a full <see cref="string"/> value of a property at the end of the hierarchy within the Json string.
            /// </summary>
            /// <param name="jsonData">A valid Json <see cref="string"/>.</param>
            /// <param name="value">
            /// When this method returns, contains the full content of the <see cref="string"/> value for the property name including objects and arrays,
            /// if the property at the end of the hierarchy is found. Otherwise, it will return as <see cref="string.Empty"/>. This parameter is passed uninitialized.
            /// </param>
            /// <param name="hierarchy">The list of properties, in progressive order, to parse through the Json string's object hierarchy.</param>
            /// <returns>
            /// <b>true</b> if the property at the end of the hierarchy is found and has a value. Otherwise <b>false</b>.
            /// <br><b>Note</b>: This will return <b>true</b> if the property contains <b>any</b> kind of value, including <b>null</b> and empty <b>objects</b> and <b>arrays</b>.</br>
            /// </returns>
            public static bool TryGetString(string jsonData, out string value, params string[] hierarchy)
            {
                value = GetString(jsonData, hierarchy);
                return !string.IsNullOrWhiteSpace(value);
            }

            /// <summary>
            /// Gets a non-nullable <b>struct</b> and <see cref="IConvertible"/> value type (<b>bool</b>, <b>int</b>, <b>float</b>, <b>enum</b>, etc) within the Json string.
            /// </summary>
            /// <typeparam name="T">A non-nullable <b>struct</b> and <see cref="IConvertible"/> value type that can be converted from a string.</typeparam>
            /// <param name="jsonData">A valid Json <see cref="string"/>.</param>
            /// <param name="property">The name of the property you want within the Json string's highest hierarchy layer.</param>
            /// <returns>
            /// The value for the property name if it is a valid non-nullable <b>struct</b> and <see cref="IConvertible"/> value type.
            /// <br><b>Note¹</b>: If the property isn't valid or not found it will return a <see cref="default"/> value.</br>
            /// <br><b>Note²</b>: If you are trying to get a <see cref="bool"/> value then this will return <b>true</b> if the property contains <b>any</b> kind of value.
            ///                   Exceptions are if the value is a number that is <b>0</b>, if the value is <b>false</b>, if the value is <b>null</b>, or if the value is an empty <b>object</b> or <b>array</b>.</br>
            /// </returns>
            public static T GetValue<T>(string jsonData, string property) where T : struct, IConvertible
            {
                if (GetString(jsonData, property) is string value && !string.IsNullOrWhiteSpace(value))
                {
                    try
                    {
                        if (typeof(T) == typeof(bool))
                        {
                            value = value.Replace(" ", string.Empty).ToLower();
                            return (T)((value != "0" &&
                                        value != "false" &&
                                        value != "null" &&
                                        value != "{}" &&
                                        value != "[]") as T?);
                        }

                        if (typeof(T).IsEnum)
                        {
                            return (T)Enum.Parse(typeof(T), value);
                        }

                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch { } // If conversion fails we silently fail to return a default value
                }

                return default;
            }

            /// <summary>
            /// Gets a non-nullable <b>struct</b> and <see cref="IConvertible"/> value type (<b>bool</b>, <b>int</b>, <b>float</b>, <b>enum</b>, etc) at the end of a hierarchy within the Json string.
            /// </summary>
            /// <typeparam name="T">A non-nullable <b>struct</b> and <see cref="IConvertible"/> value type that can be converted from a string.</typeparam>
            /// <param name="jsonData">A valid Json <see cref="string"/>.</param>
            /// <param name="hierarchy">The list of properties, in progressive order, to parse through the Json string's object hierarchy.</param>
            /// <returns>
            /// The value for the property name at the end of the hierarchy if it is a valid non-nullable <b>struct</b> and <see cref="IConvertible"/> value type.
            /// <br><b>Note¹</b>: If the hierarchy isn't valid or the property is not found it will return a <see cref="default"/> value.</br>
            /// <br><b>Note²</b>: If you are trying to get a <see cref="bool"/> value then this will return <b>true</b> if the property contains <b>any</b> kind of value.
            ///                   Exceptions are if the value is a number that is <b>0</b>, if the value is <b>false</b>, if the value is <b>null</b>, or if the value is an empty <b>object</b> or <b>array</b>.</br>
            /// </returns>
            public static T GetValue<T>(string jsonData, params string[] hierarchy) where T : struct, IConvertible
            {
                return GetValue<T>(GetString(jsonData, GetHierarchyMinusOne(hierarchy)), hierarchy[hierarchy.Length - 1]);
            }

            /// <summary>
            /// Gets a <see cref="DateTime"/> value from a <see cref="long"/> brainCloud time value within the Json string.
            /// </summary>
            /// <param name="jsonData">A valid Json <see cref="string"/>.</param>
            /// <param name="property">The name of the property you want within the Json string's highest hierarchy layer.</param>
            /// <returns>
            /// The <see cref="DateTime"/> value for the property name converted from a <see cref="long"/> brainCloud time value.
            /// <br><b>Note</b>: If the property isn't valid or not found it will return a <see cref="DateTime"/> set to the unix epoch.</br>
            /// </returns>
            public static DateTime GetDateTime(string jsonData, string property)
            {
                long t = GetValue<long>(jsonData, property);
                return t > 0 ? Util.BcTimeToDateTime(t) : Util.BcTimeToDateTime(0);
            }

            /// <summary>
            /// Gets a <see cref="DateTime"/> value from a <see cref="long"/> brainCloud time value at the end of a hierarchy within the Json string.
            /// </summary>
            /// <param name="jsonData">A valid Json <see cref="string"/>.</param>
            /// <param name="hierarchy">The list of properties, in progressive order, to parse through the Json string's object hierarchy.</param>
            /// <returns>
            /// The <see cref="DateTime"/> value for the property name at the end of the hierarchy converted from a <see cref="long"/> brainCloud time value.
            /// <br><b>Note</b>: If the hierarchy isn't valid or the property is not found it will return a <see cref="DateTime"/> set to the unix epoch.</br>
            /// </returns>
            public static DateTime GetDateTime(string jsonData, params string[] hierarchy)
            {
                return GetDateTime(GetString(jsonData, GetHierarchyMinusOne(hierarchy)), hierarchy[hierarchy.Length - 1]);
            }

            /// <summary>
            /// Gets a <see cref="TimeSpan"/> value from a <see cref="double"/> millisecond value within the Json string.
            /// </summary>
            /// <param name="jsonData">A valid Json <see cref="string"/>.</param>
            /// <param name="property">The name of the property you want within the Json string's highest hierarchy layer.</param>
            /// <returns>
            /// The <see cref="TimeSpan"/> for the property name converted from a <see cref="double"/> millisecond value.
            /// <br><b>Note</b>: If the property isn't valid or not found it will return a <see cref="TimeSpan"/> with minimum value.</br>
            /// </returns>
            public static TimeSpan GetTimeSpan(string jsonData, string property)
            {
                return TimeSpan.FromMilliseconds(GetValue<double>(jsonData, property));
            }

            /// <summary>
            /// Gets a <see cref="TimeSpan"/> value from a <see cref="double"/> millisecond value at the end of a hierarchy within the Json string.
            /// </summary>
            /// <param name="jsonData">A valid Json <see cref="string"/>.</param>
            /// <param name="hierarchy">The list of properties, in progressive order, to parse through the Json string's object hierarchy.</param>
            /// <returns>
            /// The <see cref="TimeSpan"/> for the property name at the end of the hierarchy converted from a <see cref="double"/> millisecond value.
            /// <br><b>Note</b>: If the hierarchy isn't valid or the property is not found it will return a <see cref="TimeSpan"/> with minimum value.</br>
            /// </returns>
            public static TimeSpan GetTimeSpan(string jsonData, params string[] hierarchy)
            {
                return GetTimeSpan(GetString(jsonData, GetHierarchyMinusOne(hierarchy)), hierarchy[hierarchy.Length - 1]);
            }

            /// <summary>
            /// Gets an <see cref="ACL"/> object within the Json string.
            /// </summary>
            /// <param name="jsonData">A valid Json <see cref="string"/>.</param>
            /// <param name="property">The name of the property you want within the Json string's highest hierarchy layer.</param>
            /// <returns>
            /// The <see cref="ACL"/> object for the property name.
            /// <br><b>Note</b>: If the property isn't valid or not found it will return <b>null</b>.</br>
            /// </returns>
            public static ACL GetACL(string jsonData, string property)
            {
                if (GetString(jsonData, property) is string acl && !string.IsNullOrWhiteSpace(acl))
                {
                    if (acl.Contains("Other"))
                    {
                        return new ACL(GetValue<ACL.Access>(acl, "Other"));
                    }
                }

                return null;
            }

            /// <summary>
            /// Gets an <see cref="ACL"/> object at the end of a hierarchy within the Json string.
            /// </summary>
            /// <param name="jsonData">A valid Json <see cref="string"/>.</param>
            /// <param name="hierarchy">The list of properties, in progressive order, to parse through the Json string's object hierarchy.</param>
            /// <returns>
            /// The <see cref="ACL"/> object for the property name at the end of the hierachy.
            /// <br><b>Note</b>: If the hierarchy isn't valid or the property is not found it will return <b>null</b>.</br>
            /// </returns>
            public static ACL GetACL(string jsonData, params string[] hierarchy)
            {
                return GetACL(GetString(jsonData, GetHierarchyMinusOne(hierarchy)), hierarchy[hierarchy.Length - 1]);
            }
        }
    }
}
