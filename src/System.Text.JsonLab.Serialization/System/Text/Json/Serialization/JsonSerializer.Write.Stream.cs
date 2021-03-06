﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json.Serialization
{
    public static partial class JsonSerializer
    {
        public static Task WriteAsync<TValue>(TValue value, Stream utf8Stream, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        {
            return WriteAsyncInternal(value, typeof(TValue), utf8Stream, options, cancellationToken);
        }

        public static Task WriteAsync(object value, Type type, Stream utf8Stream, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        {
            if (utf8Stream == null)
                throw new ArgumentNullException(nameof(utf8Stream));

            VerifyValueAndType(value, type);

            return WriteAsyncInternal(value, type, utf8Stream, options, cancellationToken);
        }

        private static async Task WriteAsyncInternal(object value, Type type, Stream utf8Stream, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            if (options == null)
                options = s_defaultSettings;

            var writerState = new JsonWriterState(options.WriterOptions);

            using (var bufferWriter = new ArrayBufferWriter<byte>(options.EffectiveBufferSize))
            {
                if (value == null)
                {
                    WriteNull(ref writerState, bufferWriter);
#if BUILDING_INBOX_LIBRARY
                    await utf8Stream.WriteAsync(bufferWriter.WrittenMemory, cancellationToken).ConfigureAwait(false);
#else
                    // todo: stackalloc?
                    await utf8Stream.WriteAsync(bufferWriter.WrittenMemory.ToArray(), 0, bufferWriter.WrittenMemory.Length, cancellationToken).ConfigureAwait(false);
#endif
                    return;
                }

                if (type == null)
                {
                    type = value.GetType();
                }

                JsonClassInfo classInfo = options.GetOrAddClass(type);
                WriteObjectState current = default;
                current.ClassInfo = classInfo;
                current.CurrentValue = value;
                if (classInfo.ClassType != ClassType.Object)
                {
                    current.PropertyInfo = classInfo.GetPolicyProperty();
                }

                List<WriteObjectState> previous = null;
                int arrayIndex = 0;
                bool isFinalBlock;

                int flushThreshold;
                do
                {
                    flushThreshold = (int)(bufferWriter.Capacity * .9); //todo: determine best value here

                    isFinalBlock = Write(ref writerState, bufferWriter, flushThreshold, options, ref current, ref previous, ref arrayIndex);
#if BUILDING_INBOX_LIBRARY
                    await utf8Stream.WriteAsync(bufferWriter.WrittenMemory, cancellationToken).ConfigureAwait(false);
#else
                    // todo: stackalloc?
                    await utf8Stream.WriteAsync(bufferWriter.WrittenMemory.ToArray(), 0, bufferWriter.WrittenMemory.Length, cancellationToken).ConfigureAwait(false);
#endif
                    bufferWriter.Clear();
                } while (!isFinalBlock);
            }
            
            // todo: do we want to call FlushAsync here (or above)? It seems like leaving it to the caller would be better.
            //await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
