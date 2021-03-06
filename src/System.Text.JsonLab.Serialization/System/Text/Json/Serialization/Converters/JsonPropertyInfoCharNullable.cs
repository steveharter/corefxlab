﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Text.Json.Serialization.Converters
{
    internal class JsonPropertyInfoCharNullable : JsonPropertyInfo<char?>, IJsonSerializerInternal<char?>
    {
        public JsonPropertyInfoCharNullable(Type classType, Type propertyType, PropertyInfo propertyInfo, JsonSerializerOptions options) :
            base(classType, propertyType, propertyInfo, options)
        { }

        public char? Read(ref Utf8JsonReader reader)
        {
            return reader.GetString()[0];
        }

        public void Write(ref Utf8JsonWriter writer, char? value)
        {
#if BUILDING_INBOX_LIBRARY
            char tempChar = value.Value;
            Span<char> tempSpan = MemoryMarshal.CreateSpan<char>(ref tempChar, 1);
            writer.WriteStringValue(tempSpan);
#else
            writer.WriteStringValue(value.ToString());
#endif
        }

        public void Write(ref Utf8JsonWriter writer, ReadOnlySpan<byte> name, char? value)
        {
#if BUILDING_INBOX_LIBRARY
            char tempChar = value.Value;
            Span<char> tempSpan = MemoryMarshal.CreateSpan<char>(ref tempChar, 1);
            writer.WriteString(name, tempSpan);
#else
            writer.WriteString(name, value.ToString());
#endif
        }
    }
}
