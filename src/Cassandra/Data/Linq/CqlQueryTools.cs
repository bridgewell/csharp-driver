//
//      Copyright (C) 2012-2014 DataStax Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace BWCassandra.Data.Linq
{
    internal static class CqlQueryTools
    {
        private static readonly Regex IdentifierRx = new Regex(@"\b[a-z][a-z0-9_]*\b", RegexOptions.Compiled);

        /// <summary>
        /// Hex string lookup table.
        /// </summary>
        private static readonly string[] HexStringTable = new string[]
        {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F",
            "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F",
            "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F",
            "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F",
            "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F",
            "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F",
            "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F",
            "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F",
            "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F",
            "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F",
            "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF",
            "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF",
            "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF",
            "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF",
            "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF",
            "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF"
        };

        internal static readonly DateTimeOffset UnixStart = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);

        private static readonly Dictionary<Type, string> CQLTypeNames = new Dictionary<Type, string>()
        {
            {typeof (Int32), "int"},
            {typeof (Int64), "bigint"},
            {typeof (string), "text"},
            {typeof (byte[]), "blob"},
            {typeof (Boolean), "boolean"},
            {typeof (Decimal), "decimal"},
            {typeof (Double), "double"},
            {typeof (Single), "float"},
            {typeof (Guid), "uuid"},
            {typeof (TimeUuid), "timeuuid"},
            {typeof (DateTimeOffset), "timestamp"},
            {typeof (DateTime), "timestamp"},
        };

        public static string CqlIdentifier(this string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (!IdentifierRx.IsMatch(id))
                {
                    return "\"" + id.Replace("\"", "\"\"") + "\"";
                }
                else
                {
                    return id;
                }
            }
            throw new ArgumentException("invalid identifier");
        }


        public static string QuoteIdentifier(this string id)
        {
            return "\"" + id.Replace("\"", "\"\"") + "\"";
        }

        /// <summary>
        /// Returns a hex string representation of an array of bytes.
        /// http://blogs.msdn.com/b/blambert/archive/2009/02/22/blambert-codesnip-fast-byte-array-to-hex-string-conversion.aspx
        /// </summary>
        /// <param name="value">The array of bytes.</param>
        /// <returns>A hex string representation of the array of bytes.</returns>
        public static string ToHex(this byte[] value)
        {
            var stringBuilder = new StringBuilder();
            if (value != null)
            {
                foreach (byte b in value)
                {
                    stringBuilder.Append(HexStringTable[b]);
                }
            }

            return stringBuilder.ToString();
        }


        public static string Encode(this object obj)
        {
            if (obj is string) return Encode(obj as string);
            else if (obj is Boolean) return Encode((Boolean) obj);
            else if (obj is byte[]) return Encode((byte[]) obj);
            else if (obj is Double) return Encode((Double) obj);
            else if (obj is Single) return Encode((Single) obj);
            else if (obj is Decimal) return Encode((Decimal) obj);
            else if (obj is DateTimeOffset) return Encode((DateTimeOffset) obj);
                // need to treat "Unspecified" as UTC (+0) not the default behavior of DateTimeOffset which treats as Local Timezone
                // because we are about to do math against EPOCH which must align with UTC. 
                // If we don't, then the value saved will be shifted by the local timezone when retrieved back out as DateTime.
            else if (obj is DateTime)
                return Encode(((DateTime) obj).Kind == DateTimeKind.Unspecified
                                  ? new DateTimeOffset((DateTime) obj, TimeSpan.Zero)
                                  : new DateTimeOffset((DateTime) obj));
            else if (obj.GetType().IsGenericType)
            {
                if (obj.GetType().GetInterface("ISet`1") != null)
                {
                    var sb = new StringBuilder();
                    foreach (object el in (IEnumerable) obj)
                    {
                        if (sb.ToString() != "")
                            sb.Append(", ");
                        sb.Append(el.Encode());
                    }
                    return "{" + sb.ToString() + "}";
                }
                else if (obj.GetType().GetInterface("IDictionary`2") != null)
                {
                    var sb = new StringBuilder();
                    IDictionaryEnumerator enn = ((IDictionary) obj).GetEnumerator();
                    while (enn.MoveNext())
                    {
                        if (sb.ToString() != "")
                            sb.Append(", ");
                        sb.Append(enn.Key.Encode() + ":" + enn.Value.Encode());
                    }
                    return "{" + sb.ToString() + "}";
                }
                else if (obj.GetType().GetInterface("IEnumerable`1") != null)
                {
                    var sb = new StringBuilder();
                    foreach (object el in (IEnumerable) obj)
                    {
                        if (sb.ToString() != "")
                            sb.Append(", ");
                        sb.Append(el.Encode());
                    }
                    return "[" + sb.ToString() + "]";
                }
            }
            return obj.ToString();
        }


        public static string Encode(string str)
        {
            return '\'' + str.Replace("\'", "\'\'") + '\'';
        }

        public static string Encode(bool val)
        {
            return (val ? "true" : "false");
        }

        public static string Encode(byte[] val)
        {
            return "0x" + val.ToHex();
        }

        public static string Encode(Double val)
        {
            return val.ToString(new CultureInfo("en-US"));
        }

        public static string Encode(Single val)
        {
            return val.ToString(new CultureInfo("en-US"));
        }

        public static string Encode(Decimal val)
        {
            return val.ToString(new CultureInfo("en-US"));
        }

        public static string Encode(DateTimeOffset val)
        {
            if (val == DateTimeOffset.MinValue)
                return 0.ToString(CultureInfo.InvariantCulture);
            else
                return Convert.ToInt64(Math.Floor((val - UnixStart).TotalMilliseconds)).ToString(CultureInfo.InvariantCulture);
        }

        private static string GetCqlTypeFromType(Type tpy)
        {
            if (CQLTypeNames.ContainsKey(tpy))
                return CQLTypeNames[tpy];
            else
            {
                if (tpy.IsGenericType)
                {
                    if (tpy.Name.Equals("Nullable`1"))
                    {
                        return GetCqlTypeFromType(tpy.GetGenericArguments()[0]);
                    }
                    else if (tpy.GetInterface("ISet`1") != null)
                    {
                        return "set<" + GetCqlTypeFromType(tpy.GetGenericArguments()[0]) + ">";
                    }
                    else if (tpy.GetInterface("IDictionary`2") != null)
                    {
                        return "map<" + GetCqlTypeFromType(tpy.GetGenericArguments()[0]) + ", " + GetCqlTypeFromType(tpy.GetGenericArguments()[1]) +
                               ">";
                    }
                    else if (tpy.GetInterface("IEnumerable`1") != null)
                    {
                        return "list<" + GetCqlTypeFromType(tpy.GetGenericArguments()[0]) + ">";
                    }
                }
                else if (tpy.Name == "BigDecimal")
                    return "decimal";
            }

            var supportedTypes = new StringBuilder();
            foreach (Type tn in CQLTypeNames.Keys)
                supportedTypes.Append(tn.FullName + ", ");
            supportedTypes.Append(", their nullable counterparts, and implementations of IEnumerable<T>, IDictionary<K,V>");

            throw new ArgumentException("Unsupported datatype " + tpy.Name + ". Supported are: " + supportedTypes.ToString() + ".");
        }
    }
}
