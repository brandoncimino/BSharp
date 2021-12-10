using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Enums;

using JetBrains.Annotations;

using Newtonsoft.Json.Serialization;

namespace BrandonUtils.Standalone.Strings.Json {
    /// <summary>
    /// A simple implementation of <see cref="Newtonsoft"/>'s <see cref="ITraceWriter"/> that logs messages to <see cref="Console.WriteLine()"/>
    /// </summary>
    [PublicAPI]
    public class ConsoleTraceWriter : ITraceWriter {
        public string? Nickname { get; }


        private IEnumerable<string> GetNameParts() => new[] {
            Nickname,
            $"#{GetHashCode()}",
            Thread.CurrentThread.Name
        }.NonBlank();


        private string GetFullName() {
            return Nickname.IsBlank() ? "|" : $"{GetNameParts().JoinString("-")}";
        }

        public TraceLevel LevelFilter { get; set; }

        public ConsoleTraceWriter(string? nickname = default) {
            Nickname = nickname;
        }

        public ConsoleTraceWriter(Type? owner) : this(owner.Prettify()) { }

        public ConsoleTraceWriter(object? self) : this(self?.GetType()) { }

        public void Trace(TraceLevel level, string? message, Exception? ex) {
            var msg = new[] {
                    GetTraceLevelIcon(level),
                    GetFullName(),
                    message,
                    ex != null ? $"-> {ex}" : default
                }.NonBlank()
                 .JoinString(" ");

            Console.WriteLine(msg);
        }


        private static string GetTraceLevelIcon(TraceLevel traceLevel) {
            return traceLevel switch {
                TraceLevel.Error   => "🌋",
                TraceLevel.Info    => "📎",
                TraceLevel.Off     => "🔇",
                TraceLevel.Verbose => "📜",
                TraceLevel.Warning => "⚠",
                _                  => throw BEnum.InvalidEnumArgumentException(nameof(traceLevel), traceLevel)
            };
        }
    }
}