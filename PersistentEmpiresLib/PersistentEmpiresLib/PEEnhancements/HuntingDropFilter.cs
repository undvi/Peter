namespace PEEnhancements
{
    /// <summary>
    /// Filters animal drop results to enforce the "only hunter bow yields meat" rule.
    /// This class does not interact with actual game drop tables; callers must supply
    /// the weapon identifier.
    /// </summary>
    public static class HuntingDropFilter
    {
        /// <summary>
        /// Determines whether meat should drop based on the wielded weapon string id.
        /// </summary>
        /// <param name="weaponId">String identifier of the weapon used to kill the animal.</param>
        public static bool ShouldDropMeat(string weaponId)
        {
            if (!FeatureFlags.HuntingOnlyHunterBowMeat) return true;
            return weaponId == "hunter_bow";
        }
    }
}