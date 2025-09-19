using System;
using System.Collections.Concurrent;

namespace PEEnhancements
{
    /// <summary>
    /// Provides simple healing functionality for clearing death penalties. When Medic features
    /// are enabled, authorized medics can heal players to remove penalties. Cooldowns are enforced
    /// per medic.
    /// </summary>
    public class MedicSystem
    {
        private readonly DeathPenaltySystem _penaltySystem;
        private readonly ConcurrentDictionary<string, DateTime> _lastHealTime = new();

        public MedicSystem(DeathPenaltySystem penaltySystem)
        {
            _penaltySystem = penaltySystem;
        }

        /// <summary>
        /// Attempts to heal the target player. Returns true if the penalty was removed.
        /// Enforces cooldowns per medic and checks whether medic features are enabled.
        /// </summary>
        /// <param name="medicId">The unique identifier of the medic performing the heal.</param>
        /// <param name="targetPlayerId">The unique identifier of the player to heal.</param>
        public bool Heal(string medicId, string targetPlayerId)
        {
            if (!FeatureFlags.MedicEnabled) return false;
            if (string.IsNullOrEmpty(medicId) || string.IsNullOrEmpty(targetPlayerId)) return false;
            var now = DateTime.UtcNow;
            if (_lastHealTime.TryGetValue(medicId, out var last) && (now - last).TotalSeconds < FeatureFlags.MedicCooldownSeconds)
            {
                return false; // cooldown
            }
            _lastHealTime[medicId] = now;
            _penaltySystem.ClearPenalty(targetPlayerId);
            return true;
        }
    }
}