namespace FowlFever.BSharp.Chronic {
    /// <summary>
    /// An approximate knockoff of Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/time/temporal/ChronoUnit.html">ChronoUnit</a>.
    /// </summary>
    public enum TimeUnit {
        Milliseconds,
        Seconds,
        Minutes,
        Hours,
        Days,
        Weeks,
        Ticks,
        /// <summary>
        /// The <b>minimum time</b> required to complete a <a href="https://www.dndbeyond.com/sources/phb/adventuring#ShortRest">Short Rest</a>, i.e. 1 <see cref="Hours"/>.
        /// </summary>
        ShortRests,
        /// <summary>
        /// The <b>minimum time</b> required to complete a <a href="https://www.dndbeyond.com/sources/phb/adventuring#LongRest">Long Rest</a>, i.e. 8 <see cref="Hours"/>.
        /// </summary>
        /// <remarks>In addition to being at least 8 <see cref="Hours"/> long, a long rest must:
        /// <ul>
        /// <li>Include at least 6 <see cref="Hours"/> of sleep</li>
        /// <li>Include no more than 2 <see cref="Hours"/> of light activity</li>
        /// <li>Include no more than 1 <see cref="Hours"/> of strenuous activity</li>
        /// </ul>
        /// </remarks>
        LongRests,
        /// <summary>
        /// Equivalent to 3 <see cref="Minutes"/>s. See: <a href="https://en.wikipedia.org/wiki/Round_(boxing)">Round (boxing)</a>
        /// </summary>
        QueensberryRounds,
    }
}