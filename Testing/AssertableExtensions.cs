using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings;
using FowlFever.Conjugal.Affixing;

namespace FowlFever.Testing {
    internal static class AssertableExtensions {
        private const string PassIcon   = "✅";
        private const string FailIcon   = "❌";
        private const string ExcuseIcon = "";

        internal static IEnumerable<string> FormatAssertable(this IFailable failure, int indent = 0) {
            return FormatAssertable(failure, PassIcon, FailIcon, ExcuseIcon, indent);
        }

        private static IEnumerable<string> FormatAssertable(
            this IFailable failure,
            string         passIcon,
            string         failIcon,
            string         excuseIcon,
            int            indent
        ) {
            var header = GetHeader(failure, passIcon, failIcon);
            var excuse = FormatExcuse(failure, excuseIcon);

            var excuseIndentSize = header.BoundMorpheme.Length  + header.Joiner.Length;
            var excusePrefix     = " ".Repeat(excuseIndentSize) + "| ";

            return header
                   .Render()
                   .SplitLines()
                   .Concat(excuse.Indent(indentString: excusePrefix))
                   .Indent(indent);
        }

        private static IEnumerable<string> FormatExcuseMessage(IFailable failure, string excuseIcon = ExcuseIcon) {
            return failure.Excuse?.Message.SplitLines() ?? Enumerable.Empty<string>();
        }

        private static IEnumerable<string> FormatExcuse(IFailable failure, string excuseIcon = ExcuseIcon) {
            if (failure.Failed) {
                var message    = FormatExcuseMessage(failure, excuseIcon);
                var stacktrace = failure.Excuse?.StackTrace.SplitLines();
                return message.Concat(stacktrace.NonNull());
            }
            else {
                return Array.Empty<string>();
            }
        }

        private static Affixation GetHeader(IFailable assertable, string passIcon = PassIcon, string failIcon = FailIcon) {
            var icon = assertable.Failed ? failIcon : passIcon;
            return Affixation.Prefixation((assertable.Description?.Supplied().TryGet()).OrElse(assertable.GetType().Name), icon, " ");
        }
    }
}