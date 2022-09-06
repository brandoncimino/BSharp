using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Exceptions;

/// <summary>
/// An <see cref="Exception"/> thrown by code that should have been unreachable.
/// </summary>
[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument", Justification = "Values are being passed-through to matching caller info parameters.")]
public class UnreachableException : BrandonException {
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
    public UnreachableException(
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
    public UnreachableException(
        string?                    message   = default,
        [CallerMemberName] string? _caller   = default,
        [CallerFilePath]   string? _filePath = default,
        [CallerLineNumber] int?    _lineNo   = default
    ) : this(message, default, _caller, _filePath, _lineNo) { }
}