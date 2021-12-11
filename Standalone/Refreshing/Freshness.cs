namespace BrandonUtils.Standalone.Refreshing {
    /// <summary>
    /// Describes the state of a <see cref="Refreshing{TValue,TStaleness}"/> value.
    /// </summary>
    /// <remarks>
    /// I had a third state, "Pristine", which was the default, but I removed it because it:
    /// <ul>
    /// <li>Only seemed useful "internally" to the <see cref="Refreshing{TValue,TStaleness}"/> itself</li>
    /// <li>Caused the statements "IsStale" and "NeedsRefresh" to somehow mean _different things_, which is very confusing</li>
    /// </ul>
    /// </remarks>
    public enum Freshness {
        /// <summary>
        /// Implies that:
        /// <ul>
        /// <li>The result of <see cref="Refreshing{TValue,TStaleness}.StalenessPredicate"/> is <c>false</c>.</li>
        /// <li>Calling <see cref="Refreshing{TValue,TStaleness}.Value"/> will <b>not</b> trigger <see cref="Refreshing{TValue,TStaleness}.Refresh"/>.</li>
        /// </ul>
        /// </summary>
        Fresh,
        /// <summary>
        /// Implies that:
        /// <ul>
        /// <li>The result of <see cref="Refreshing{TValue,TStaleness}.StalenessPredicate"/> is <c>true</c>.</li>
        /// <li>Calling <see cref="Refreshing{TValue,TStaleness}.Value"/> will trigger <see cref="Refreshing{TValue,TStaleness}.Refresh"/>.</li>
        /// </ul>
        /// </summary>
        Stale
    }
}