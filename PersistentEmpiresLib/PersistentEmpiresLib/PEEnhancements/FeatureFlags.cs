using System;

namespace PEEnhancements
{
    /// <summary>
    /// Provides easy access to boolean flags and numeric values defined in the settings.
    /// Use these properties throughout the gameplay systems to conditionally enable or disable features.
    /// </summary>
    public static class FeatureFlags
    {
        /// <summary>
        /// Gets the current settings instance. Clients must set this before use via <see cref="Initialize"/>.
        /// </summary>
        public static PeSettings Current { get; private set; } = new PeSettings();

        /// <summary>
        /// Initializes the feature flags by loading the configuration from the specified JSON string.
        /// </summary>
        /// <param name="json">The JSON contents of the configuration file.</param>
        public static void Initialize(string json)
        {
            Current = PeSettings.Load(json);
        }

        public static bool DeathPenaltyEnabled => Current.Death.Penalty.Enabled;
        public static int  DeathPenaltyHours   => Current.Death.Penalty.Hours;
        public static bool DeathArmorBreakEnabled => Current.Death.ArmorBreak.Enabled;
        public static float DeathArmorBreakExtraChance => Current.Death.ArmorBreak.ExtraChance;
        public static bool MedicEnabled => Current.Medic.Enabled;
        public static int  MedicCooldownSeconds => Current.Medic.CooldownSeconds;
        public static bool PropertyEnabled => Current.Property.Enabled;
        public static bool HuntingOnlyHunterBowMeat => Current.Hunting.OnlyHunterBowMeat;
        public static bool EconomyJobsEnabled => Current.Economy.Jobs.Enabled;
    }
}