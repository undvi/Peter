namespace PEEnhancements
{
    /// <summary>
    /// Feature flag container for optional Persistent Empires enhancements.
    /// </summary>
    public static class FeatureFlags
    {
        /// <summary>
        /// Enables the barkeep economy features (shift toggling, tip registration, etc.).
        /// </summary>
        public static bool EconomyBarkeepEnabled { get; set; } = true;
    }
}
