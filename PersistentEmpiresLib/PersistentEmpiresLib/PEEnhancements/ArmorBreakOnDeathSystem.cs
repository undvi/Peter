using System;
using System.Collections.Generic;

namespace PEEnhancements
{
    /// <summary>
    /// Applies an increased chance for equipment to break upon death when enabled.
    /// This system does not interact with actual game objects; the caller must provide
    /// a collection of items and handle durability modifications.
    /// </summary>
    public static class ArmorBreakOnDeathSystem
    {
        private static readonly Random _rng = new();

        /// <summary>
        /// Calculates whether an item should break based on its base break chance and
        /// the extra chance configured in <see cref="FeatureFlags"/>.
        /// </summary>
        /// <param name="baseChance">The base probability of breakage (0..1).</param>
        /// <returns>True if the item should break; otherwise false.</returns>
        public static bool ShouldBreak(float baseChance)
        {
            if (!FeatureFlags.DeathArmorBreakEnabled) return false;
            var totalChance = Math.Clamp(baseChance + FeatureFlags.DeathArmorBreakExtraChance, 0f, 1f);
            return _rng.NextDouble() < totalChance;
        }

        /// <summary>
        /// Iterates over the player's equipment and yields the items that should be broken.
        /// </summary>
        /// <param name="equipment">Collection of tuples containing the item id and its base break chance.</param>
        /// <returns>A list of item ids that are deemed broken.</returns>
        public static IEnumerable<string> GetBrokenItems(IEnumerable<(string ItemId, float BaseBreakChance)> equipment)
        {
            var broken = new List<string>();
            if (!FeatureFlags.DeathArmorBreakEnabled) return broken;
            foreach (var (id, baseChance) in equipment)
            {
                if (ShouldBreak(baseChance)) broken.Add(id);
            }
            return broken;
        }
    }
}