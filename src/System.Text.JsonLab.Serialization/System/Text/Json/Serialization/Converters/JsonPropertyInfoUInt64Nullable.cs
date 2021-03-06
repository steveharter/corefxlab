﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace System.Text.Json.Serialization.Converters
{
    internal class JsonPropertyInfoUInt64Nullable : JsonPropertyInfo<ulong?>, IJsonSerializerInternal<ulong?>
    {
        public JsonPropertyInfoUInt64Nullable(Type classType, Type propertyType, PropertyInfo propertyInfo, JsonSerializerOptions options) :
            base(classType, propertyType, propertyInfo, options)
        { }

        public ulong? Read(ref Utf8JsonReader reader)
        {
            return reader.GetUInt64();
        }

        public void Write(ref Utf8JsonWriter writer, ulong? value)
        {
            writer.WriteNumberValue(value.Value);
        }

        public void Write(ref Utf8JsonWriter writer, ReadOnlySpan<byte> name, ulong? value)
        {
            writer.WriteNumber(name, value.Value);
        }
    }
}
