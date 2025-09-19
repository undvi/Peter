using System.Text.Json;
using System.Text.Json.Serialization;

namespace PEEnhancements
{
    /// <summary>
    /// Strongly typed representation of the persistent empire enhancement settings.
    /// Each property maps to a section in the JSON settings file.
    /// </summary>
    public class PeSettings
    {
        [JsonPropertyName("death")] public DeathSettings Death { get; set; } = new();
        [JsonPropertyName("medic")] public MedicSettings Medic { get; set; } = new();
        [JsonPropertyName("property")] public PropertySettings Property { get; set; } = new();
        [JsonPropertyName("hunting")] public HuntingSettings Hunting { get; set; } = new();
        [JsonPropertyName("economy")] public EconomySettings Economy { get; set; } = new();

        public class DeathSettings
        {
            [JsonPropertyName("penalty")] public PenaltySettings Penalty { get; set; } = new();
            [JsonPropertyName("armorBreak")] public ArmorBreakSettings ArmorBreak { get; set; } = new();

            public class PenaltySettings
            {
                [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
                [JsonPropertyName("hours")] public int Hours { get; set; } = 10;
            }

            public class ArmorBreakSettings
            {
                [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
                [JsonPropertyName("extraChance")] public float ExtraChance { get; set; } = 0.35f;
            }
        }

        public class MedicSettings
        {
            [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
            [JsonPropertyName("cooldownSeconds")] public int CooldownSeconds { get; set; } = 180;
        }

        public class PropertySettings
        {
            [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        }

        public class HuntingSettings
        {
            [JsonPropertyName("onlyHunterBowMeat")] public bool OnlyHunterBowMeat { get; set; } = true;
        }

        public class EconomySettings
        {
            [JsonPropertyName("jobs")] public JobsSettings Jobs { get; set; } = new();
            [JsonPropertyName("barkeep")] public BarkeepSettings Barkeep { get; set; } = new();

            public class JobsSettings
            {
                [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
            }

            public class BarkeepSettings
            {
                [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
                [JsonPropertyName("payoutGold")] public int PayoutGold { get; set; } = 45;
                [JsonPropertyName("payoutIntervalSeconds")] public int PayoutIntervalSeconds { get; set; } = 60;
                [JsonPropertyName("shiftMinutes")] public int ShiftMinutes { get; set; } = 30;
            }
        }

        /// <summary>
        /// Deserialize a settings file from JSON. Returns default values if file cannot be parsed.
        /// </summary>
        public static PeSettings Load(string json)
        {
            try
            {
                var settings = JsonSerializer.Deserialize<PeSettings>(json) ?? new PeSettings();
                return settings;
            }
            catch
            {
                return new PeSettings();
            }
        }
    }
}