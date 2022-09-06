using System;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Exceptions;

/// <summary>
/// An <see cref="Exception"/> thrown by code that should have been unreachable.
/// </summary>
public class UnreachableException : BrandonException {
    private readonly   string? _caller;
    private readonly   string? _filePath;
    private readonly   int?    _lineNo;
    protected override string  BaseMessage => $"[{_caller}] {_filePath}:line {_lineNo} should have been unreachable!";

    /// <summary>
    /// <inheritdoc cref="UnreachableException"/>
    /// </summary>
    /// <param name="message">a <see cref="BrandonException.CustomMessage"/> to be included in the <see cref="Exception.Message"/></param>
    /// <param name="innerException">the <see cref="Exception.InnerException"/></param>
    /// <param name="_caller">see <see cref="CallerMemberNameAttribute"/></param>
    /// <param name="_filePath">see <see cref="CallerFilePathAttribute"/></param>
    /// <param name="_lineNo">see <see cref="CallerLineNumberAttribute"/></param>
    public UnreachableException(
        string?                    message        = default,
        Exception?                 innerException = default,
        [CallerMemberName] string? _caller        = default,
        [CallerFilePath]   string? _filePath      = default,
        [CallerLineNumber] int?    _lineNo        = default
    ) : base(message, innerException) {
        this._caller   = _caller;
        this._filePath = _filePath;
        this._lineNo   = _lineNo;
    }
}