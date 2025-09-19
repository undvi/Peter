namespace PEEnhancements
{
    /// <summary>
    /// Feature toggles for experimental PE enhancements.
    /// </summary>
    public static class FeatureFlags
    {
        /// <summary>
        /// Controls whether the enhanced barkeep economy features are enabled.
        /// Defaults to <c>true</c>, but can be changed at runtime by other systems if required.
        /// </summary>
        public static bool EconomyBarkeepEnabled { get; set; } = true;
    }
}
