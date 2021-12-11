using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FowlFever.BSharp.Strings {
    /// <summary>
    /// Groups together a bunch of different ways to match strings into a <b>mega-predicate</b> using builder-style chainable functions:
    ///
    /// <ul>
    /// <li><see cref="Containing"/></li>
    /// <li><see cref="Matching(System.Text.RegularExpressions.Regex)"/></li>
    /// <li><see cref="PredicatedOn"/></li>
    /// </ul>
    /// </summary>
    public class StringFilter {
        public readonly List<string>             Substrings = new List<string>();
        public readonly List<Regex>              Patterns   = new List<Regex>();
        public readonly List<Func<string, bool>> Predicates = new List<Func<string, bool>>();

        /// <summary>
        /// Causes this <see cref="StringFilter"/> to match strings that <see cref="string.Contains"/> <see cref="substring"/>.
        /// </summary>
        /// <param name="substring">a substring to check via <see cref="string.Contains"/></param>
        /// <returns><see langword="this"/></returns>
        public StringFilter Containing(string substring) {
            Substrings.Add(substring);
            return this;
        }

        /// <summary>
        /// Causes this <see cref="StringFilter"/> to match strings that <see cref="Regex.IsMatch(string)"/> with <see cref="pattern"/>
        /// </summary>
        /// <param name="pattern">a <see cref="Regex"/> to check via <see cref="Regex.IsMatch(string)"/></param>
        /// <returns><see langword="this"/></returns>
        public StringFilter Matching(Regex pattern) {
            Patterns.Add(pattern);
            return this;
        }

        /**
         * <inheritdoc cref="Matching(System.Text.RegularExpressions.Regex)"/>
         */
        public StringFilter Matching(string regex) {
            Matching(new Regex(regex));
            return this;
        }

        /// <summary>
        /// Causes this <see cref="StringFilter"/> to match arbitrary <see cref="Func{T,T}"/> predicates.
        /// </summary>
        /// <param name="predicate">an arbitrary <see cref="Func{T,T}"/> predicate</param>
        /// <returns><see langword="this"/></returns>
        public StringFilter PredicatedOn(Func<string, bool> predicate) {
            Predicates.Add(predicate);
            return this;
        }

        private bool TestContaining(string str) {
            return Substrings.Any(str.Contains);
        }

        private bool TestMatching(string str) {
            return Patterns.Any(it => it.IsMatch(str));
        }

        private bool TestPredicates(string stackTraceLine) {
            return Predicates.Any(it => it.Invoke(stackTraceLine));
        }

        /// <summary>
        /// Returns true if <b>any</b> of this <see cref="StringFilter"/>'s conditions match the given <see cref="string"/>.
        /// </summary>
        /// <param name="str">a <see cref="string"/> to compare against this <see cref="StringFilter"/></param>
        /// <returns>true if <b>any</b> of this <see cref="StringFilter"/>'s conditions match the given <see cref="string"/></returns>
        public bool TestFilter(string str) {
            return TestContaining(str) ||
                   TestMatching(str)   ||
                   TestPredicates(str);
        }
    }
}