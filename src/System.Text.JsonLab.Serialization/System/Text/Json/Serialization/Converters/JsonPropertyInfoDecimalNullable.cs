﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace System.Text.Json.Serialization.Converters
{
    internal class JsonPropertyInfoDecimalNullable : JsonPropertyInfo<decimal?>, IJsonSerializerInternal<decimal?>
    {
        public JsonPropertyInfoDecimalNullable(Type classType, Type propertyType, PropertyInfo propertyInfo, JsonSerializerOptions options) :
            base(classType, propertyType, propertyInfo, options)
        { }

        public decimal? Read(ref Utf8JsonReader reader)
        {
            return reader.GetDecimal();
        }

        public void Write(ref Utf8JsonWriter writer, decimal? value)
        {
            writer.WriteNumberValue(value.Value);
        }

        public void Write(ref Utf8JsonWriter writer, ReadOnlySpan<byte> name, decimal? value)
        {
            writer.WriteNumber(name, value.Value);
        }
    }
}
