using System;
using System.Diagnostics;

using FowlFever.BSharp.Collections;

using Newtonsoft.Json.Serialization;

namespace FowlFever.BSharp.Strings.Json {
    public static class TraceWriterExtensions {
        public static void Info(
            this ITraceWriter? traceWriter,
            Func<string>       message,
            int                indent    = 0,
            Exception?         exception = default
        ) => traceWriter?.Trace(message, indent, exception, TraceLevel.Info);

        public static void Error(
            this ITraceWriter? traceWriter,
            Func<string>       message,
            int                indent    = 0,
            Exception?         exception = default
        ) => traceWriter?.Trace(message, indent, exception, TraceLevel.Error);

        public static void Warning(
            this ITraceWriter? traceWriter,
            Func<string>       message,
            int                indent    = 0,
            Exception?         exception = default
        ) => traceWriter?.Trace(message, indent, exception, TraceLevel.Warning);

        public static void Verbose(
            this ITraceWriter? traceWriter,
            Func<string>       message,
            int                indent    = 0,
            Exception?         exception = default
        ) => traceWriter?.Trace(message, indent, exception, TraceLevel.Verbose);

        /// <summary>
        /// <see cref="ITraceWriter.Trace"/>s a message, lazily evaluating the <paramref name="message"/> <see cref="Func{TResult}"/> only if the <see cref="ITraceWriter.LevelFilter"/> includes the desired <see cref="TraceLevel"/>.
        /// </summary>
        /// <remarks>
        /// Also has other niceties like handling null <see cref="ITraceWriter"/>s, allowing the <paramref name="exception"/> to be omitted, etc.
        /// </remarks>
        /// <param name="traceWriter">the <see cref="ITraceWriter"/> that will be doing the logging</param>
        /// <param name="message">a <see cref="Func{TResult}"/> that will supply the string that gets logged</param>
        /// <param name="indent"></param>
        /// <param name="exception">an optional <see cref="Exception"/> to include in the logged message</param>
        /// <param name="level">the desired <see cref="TraceLevel"/> of the message</param>
        public static void Trace(
            this ITraceWriter? traceWriter,
            Supplied<string>   message,
            int                indent    = 0,
            Exception?         exception = default,
            TraceLevel?        level     = default
        ) {
            if (traceWriter == null) {
                return;
            }

            var lvl = level ?? traceWriter.GetDefaultLevel();

            if (traceWriter.LevelFilter >= lvl) {
                traceWriter.Trace(lvl, message.Value?.Indent(indent).JoinLines()!, exception);
            }
        }

        private static TraceLevel GetDefaultLevel(this ITraceWriter? traceWriter) {
            return traceWriter switch {
                ISuperTraceWriter tracer => tracer.DefaultLevel,
                _                        => TraceLevel.Info,
            };
        }
    }
}