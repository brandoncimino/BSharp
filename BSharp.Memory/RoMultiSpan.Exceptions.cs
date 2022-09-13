using System;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Memory;

public readonly ref partial struct RoMultiSpan<T> {
    private static InvalidOperationException _Unreachable([CallerMemberName] string? _caller = default, [CallerFilePath] string? _file = default, [CallerLineNumber] int _lineNo = default) {
        return new InvalidOperationException($"{_caller} in {_file} @ line #{_lineNo} should have been unreachable!");
    }

    private void _RequireElementIndex(int elementIndex, [CallerArgumentExpression("elementIndex")] string? paramName = default) {
        if (elementIndex < 0 || elementIndex >= ElementCount) {
            throw new ArgumentOutOfRangeException(paramName, elementIndex, $"{elementIndex} is out-of-bounds: this {nameof(RoMultiSpan<T>)} contains {ElementCount} elements!");
        }
    }

    private (int off, int len) _RequireElementRange(Range elementRange, [CallerArgumentExpression("elementRange")] string? _rangeParam = default) {
        var offLen = elementRange.GetOffsetAndLength(ElementCount);
        _RequireElementRange(offLen.Offset, offLen.Length, _rangeParam, nameof(Range.Start), nameof(Range.End));
        return offLen;
    }

    private void _RequireElementRange(
        int                                         start,
        int                                         end,
        string?                                     _rangeParam = default,
        [CallerArgumentExpression("start")] string? _start      = default,
        [CallerArgumentExpression("end")]   string? _end        = default
    ) {
        if (start < 0 || end >= ElementCount) {
            _RequireElementRange_Indexes(start, end, _rangeParam, _start, _end);
        }
    }

    private void _RequireElementRange_Indexes(
        Index                                       start,
        Index                                       end,
        string?                                     _rangeParam = default,
        [CallerArgumentExpression("start")] string? _start      = default,
        [CallerArgumentExpression("end")]   string? _end        = default
    ) {
        string _IndexValStr(Index index, int length) => index.IsFromEnd ? $"{index}⇒{index.GetOffset(length)}" : $"{index}";

        var ec = ElementCount;

        string? startMsg = default;
        string? endMsg   = default;

        if (start.GetOffset(ec) < 0) {
            startMsg = $"{_start} {_IndexValStr(start, ec)} is < 0";
        }

        if (start.GetOffset(ec) >= ElementCount) {
            endMsg = $"{_end} {_IndexValStr(end, ec)} is >= {nameof(ElementCount)} {ElementCount}";
        }

        if (startMsg != null || endMsg != null) {
            throw new ArgumentOutOfRangeException(
                _rangeParam ?? $"{_start}..{_end}",
                start..end,
                string.Join("; ", startMsg, endMsg)
            );
        }
    }
}