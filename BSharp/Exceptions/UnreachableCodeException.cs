using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Exceptions;

/// <summary>
/// An <see cref="Exception"/> thrown by code that should have been unreachable.
/// </summary>
/// <remarks>
/// This is named so that it won't conflict with the .NET 7+ <a href="https://learn.microsoft.com/en-us/dotnet/api/System.Diagnostics.UnreachableException?view=net-7.0">UnreachableException</a>.
///
/// TODO: Figure out a better way to handle the conflict with <a href="https://learn.microsoft.com/en-us/dotnet/api/System.Diagnostics.UnreachableException?view=net-7.0">UnreachableException</a>. Maybe just steal the version from .NET 7 and conditional compile it, like I do with <see cref="CallerArgumentExpressionAttribute"/>?
/// </remarks>
[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument", Justification = "Values are being passed-through to matching caller info parameters.")]
public class UnreachableCodeException : BrandonException {
    private readonly   string? _caller;
    private readonly   string? _filePath;
    private readonly   int?    _lineNo;
    protected override string  BaseMessage => $"[{_caller}] {_filePath}:line {_lineNo} should have been unreachable!";

    /// <summary>
    /// <inheritdoc cref="T:FowlFever.BSharp.Exceptions.UnreachableException"/>
    /// </summary>
    /// <param name="message">a <see cref="BrandonException.CustomMessage"/> to be included in the <see cref="Exception.Message"/></param>
    /// <param name="innerException">the <see cref="Exception.InnerException"/></param>
    /// <param name="_caller">see <see cref="CallerMemberNameAttribute"/></param>
    /// <param name="_filePath">see <see cref="CallerFilePathAttribute"/></param>
    /// <param name="_lineNo">see <see cref="CallerLineNumberAttribute"/></param>
    public UnreachableCodeException(
        string?                    message,
        Exception?                 innerException,
        [CallerMemberName] string? _caller   = default,
        [CallerFilePath]   string? _filePath = default,
        [CallerLineNumber] int?    _lineNo   = default
    ) : base(message, innerException) {
        this._caller   = _caller;
        this._filePath = _filePath;
        this._lineNo   = _lineNo;
    }

    /// <inheritdoc cref="M:FowlFever.BSharp.Exceptions.UnreachableException.#ctor(System.String,System.Exception,System.String,System.String,System.Nullable{System.Int32})"/>
    public UnreachableCodeException(
        string?                    message   = default,
        [CallerMemberName] string? _caller   = default,
        [CallerFilePath]   string? _filePath = default,
        [CallerLineNumber] int?    _lineNo   = default
    ) : this(message, default, _caller, _filePath, _lineNo) { }
}