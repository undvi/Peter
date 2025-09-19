using System;
using System.Collections.Concurrent;

namespace PEEnhancements
{
    /// <summary>
    /// Kleiner Brückenlayer: kümmert sich um Medic-Cooldowns und das Aufheben der DeathPenalty.
    /// Absichtlich unabhängig von Mission-spezifischen Typen – nur string-IDs.
    /// </summary>
    public static class MedicBridge
    {
        private static readonly ConcurrentDictionary<string, DateTime> _medicCooldownUntil = new();

        private static string SafeId(string? raw)
            => string.IsNullOrWhiteSpace(raw) ? "(unknown)" : raw!;

        public static bool TryHealPenalty(string? medicIdRaw, string? targetIdRaw, out string reason)
        {
            reason = string.Empty;
            if (!FeatureFlags.MedicEnabled)
            {
                reason = "medic feature disabled";
                return false;
            }

            var medicId = SafeId(medicIdRaw);
            var targetId = SafeId(targetIdRaw);

            var now = DateTime.UtcNow;
            var cooldown = TimeSpan.FromSeconds(FeatureFlags.MedicCooldownSeconds);

            if (_medicCooldownUntil.TryGetValue(medicId, out var until) && until > now)
            {
                reason = $"cooldown {Math.Max(0, (int)(until - now).TotalSeconds)}s";
                return false;
            }

            // eigentliche Heilung (Penalty löschen)
            DeathPenaltySystem.Instance.ClearPenalty(targetId);
            _medicCooldownUntil[medicId] = now.Add(cooldown);
            return true;
        }
    }
}
