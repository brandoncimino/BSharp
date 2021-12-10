using System;
using System.Diagnostics;

using Newtonsoft.Json.Serialization;

namespace BrandonUtils.Standalone.Strings.Json {
    public static class TraceWriterExtensions {
        public static void Info(this    ITraceWriter? traceWriter, Func<string> message, int indent = 0, Exception? exception = default) => traceWriter?.Trace(TraceLevel.Info,    message, indent, exception);
        public static void Error(this   ITraceWriter? traceWriter, Func<string> message, int indent = 0, Exception? exception = default) => traceWriter?.Trace(TraceLevel.Error,   message, indent, exception);
        public static void Warning(this ITraceWriter? traceWriter, Func<string> message, int indent = 0, Exception? exception = default) => traceWriter?.Trace(TraceLevel.Warning, message, indent, exception);
        public static void Verbose(this ITraceWriter? traceWriter, Func<string> message, int indent = 0, Exception? exception = default) => traceWriter?.Trace(TraceLevel.Verbose, message, indent, exception);

        /// <summary>
        /// <see cref="ITraceWriter.Trace"/>s a message, lazily evaluating the <paramref name="message"/> <see cref="Func{TResult}"/> only if the <see cref="ITraceWriter.LevelFilter"/> includes the desired <see cref="TraceLevel"/>.
        /// </summary>
        /// <remarks>
        /// Also has other niceties like handling null <see cref="ITraceWriter"/>s, allowing the <paramref name="exception"/> to be omitted, etc.
        /// </remarks>
        /// <param name="traceWriter">the <see cref="ITraceWriter"/> that will be doing the logging</param>
        /// <param name="level">the desired <see cref="TraceLevel"/> of the message</param>
        /// <param name="message">a <see cref="Func{TResult}"/> that will supply the string that gets logged</param>
        /// <param name="indent"></param>
        /// <param name="exception">an optional <see cref="Exception"/> to include in the logged message</param>
        public static void Trace(this ITraceWriter? traceWriter, TraceLevel level, Func<string> message, int indent = 0, Exception? exception = default) {
            if (traceWriter == null) {
                return;
            }

            if (traceWriter.LevelFilter >= level) {
                traceWriter.Trace(level, message.Invoke().Indent(indent), exception);
            }
        }
    }
}