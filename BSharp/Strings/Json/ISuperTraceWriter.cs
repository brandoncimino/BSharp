using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Newtonsoft.Json.Serialization;

namespace FowlFever.BSharp.Strings.Json;

public interface ISuperTraceWriter : ITraceWriter {
    Stack<string?>                  Stack          { get; }
    TraceLevel                      DefaultLevel   { get; init; }
    Func<ISuperTraceWriter, string> LabelFormatter { get; init; }
    void                            Start(TraceLevel? traceLevel = default, [CallerMemberName] string? starting = default);
    void                            End(TraceLevel?   traceLevel = default);
}