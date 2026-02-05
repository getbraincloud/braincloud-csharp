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
        /// 
        /// </summary>
        public static partial class JsonParser
        {
            private static readonly StringBuilder sbHelper = null;

            static JsonParser()
            {
                sbHelper = new(2048);
            }

            /// <summary>
            /// 
            /// </summary>
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
            /// 
            /// </summary>
            /// <param name="jsonData"></param>
            /// <param name="property"></param>
            /// <returns></returns>
            /// <exception cref="Exception"></exception>
            public static string GetString(string jsonData, string property)
            {
                if (string.IsNullOrWhiteSpace(jsonData) ||
                    !(jsonData.StartsWith('{') && jsonData.EndsWith('}')) &&
                    !(jsonData.StartsWith('[') && jsonData.EndsWith(']')))
                {
                    throw new Exception("jsonData is not a valid Json!");
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
                                        default:
                                            sbHelper.Append(current);
                                            continue;
                                    }
                                }
                                while (level > 0);

                                return sbHelper.ToString();
                            }
                            else
                            {
                                sbHelper.Clear();
                                next = jsonData[++i];
                                if (next == '"')
                                {
                                    next = jsonData[++i];
                                }

                                while (next != '}' && next != ']' &&
                                       next != '"' && next != ',')
                                {
                                    current = jsonData[i];
                                    next = jsonData[++i];
                                    sbHelper.Append(current);
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
            /// 
            /// </summary>
            /// <param name="jsonData"></param>
            /// <param name="hierarchy"></param>
            /// <returns></returns>
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
                        return null;
                    }

                    return jsonData;
                }

                return string.Empty;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="jsonData"></param>
            /// <param name="property"></param>
            /// <returns></returns>
            public static T GetValue<T>(string jsonData, string property) where T : struct, IConvertible
            {
                if (GetString(jsonData, property) is string value && !string.IsNullOrWhiteSpace(value))
                {
                    try
                    {
                        if (typeof(T) == typeof(bool))
                        {
                            value = value.ToLower();
                            return (T)((value != "0" && value != "false" && value != "null") as T?);
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
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="jsonData"></param>
            /// <param name="hierarchy"></param>
            /// <returns></returns>
            public static T GetValue<T>(string jsonData, params string[] hierarchy) where T : struct, IConvertible
            {
                return GetValue<T>(GetString(jsonData, GetHierarchyMinusOne(hierarchy)), hierarchy[^1]);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="jsonData"></param>
            /// <param name="property"></param>
            /// <returns></returns>
            public static DateTime GetDateTime(string jsonData, string property)
            {
                long t = GetValue<long>(jsonData, property);
                return t > 0 ? Util.BcTimeToDateTime(t) : DateTime.UnixEpoch;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="jsonData"></param>
            /// <param name="hierarchy"></param>
            /// <returns></returns>
            public static DateTime GetDateTime(string jsonData, params string[] hierarchy)
            {
                return GetDateTime(GetString(jsonData, GetHierarchyMinusOne(hierarchy)), hierarchy[^1]);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="jsonData"></param>
            /// <param name="property"></param>
            /// <returns></returns>
            public static TimeSpan GetTimeSpan(string jsonData, string property)
                    => TimeSpan.FromMilliseconds(GetValue<double>(jsonData, property));

            /// <summary>
            /// 
            /// </summary>
            /// <param name="jsonData"></param>
            /// <param name="hierarchy"></param>
            /// <returns></returns>
            public static TimeSpan GetTimeSpan(string jsonData, params string[] hierarchy)
            {
                return GetTimeSpan(GetString(jsonData, GetHierarchyMinusOne(hierarchy)), hierarchy[^1]);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="jsonData"></param>
            /// <param name="property"></param>
            /// <returns></returns>
            public static ACL GetACL(string jsonData, string property)
            {
                if (GetString(jsonData, property) is string acl && !string.IsNullOrWhiteSpace(acl))
                {
                    if (acl.Contains("Other"))
                    {
                        return new ACL(GetValue<ACL.Access>(acl, "Other"));
                    }
                }

                return ACL.None();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="jsonData"></param>
            /// <param name="hierarchy"></param>
            /// <returns></returns>
            public static ACL GetACL(string jsonData, params string[] hierarchy)
            {
                return GetACL(GetString(jsonData, GetHierarchyMinusOne(hierarchy)), hierarchy[^1]);
            }
        }
    }
}
