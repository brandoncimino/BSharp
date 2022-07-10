using System;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Strings;

namespace FowlFever.BSharp.Attributes;

/// <summary>
/// Indicates that a class is something I'm playing around with and should be expected to change / is probably broken.
///
/// TODO: Create a <a href="https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/tutorials/how-to-write-csharp-analyzer-code-fix">Roslyn analyzer</a> for usages of <see cref="ExperimentalAttribute"/>
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class ExperimentalAttribute : Attribute {
    public string Description { get; }
    public string Caller      { get; }

    public ExperimentalAttribute(string? description = "", [CallerMemberName] string? caller = default) {
        Description = description ?? "";
        Caller      = caller.OrNullPlaceholder();
    }
}