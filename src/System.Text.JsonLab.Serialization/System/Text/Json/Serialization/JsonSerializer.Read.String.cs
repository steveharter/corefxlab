﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Serialization
{
    public static partial class JsonSerializer
    {
        public static T ReadString<T>(string json, JsonSerializerOptions options = null)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));

            return (T)ReadInternal(json, typeof(T), options);
        }

        public static object ReadString(string json, Type returnType, JsonSerializerOptions options = null)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));

            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));

            return ReadInternal(json, returnType, options);
        }

        private static object ReadInternal(string json, Type returnType, JsonSerializerOptions options = null)
        {
            if (options == null)
                options = s_defaultSettings;

            // todo: use an array pool here for smaller requests to avoid the alloc. Also doc the API that UTF8 is preferred for perf. 
            byte[] jsonBytes = s_utf8Encoding.GetBytes(json);
            var state = new JsonReaderState(options: options.ReaderOptions);
            var reader = new Utf8JsonReader(jsonBytes, isFinalBlock: true, state);
            return Read(reader, returnType, options);
        }
    }
}
