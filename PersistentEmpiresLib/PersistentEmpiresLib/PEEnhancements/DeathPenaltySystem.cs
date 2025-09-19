using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PEEnhancements
{
    /// <summary>
    /// Handles applying and tracking death penalties. When a player dies, a penalty is applied
    /// that restricts their actions for a set number of hours. This system stores the expiry
    /// timestamps in memory; persistence can be added via serialization.
    /// </summary>
    public class DeathPenaltySystem
    {
        public static DeathPenaltySystem Instance { get; } = new DeathPenaltySystem();

        private readonly ConcurrentDictionary<string, DateTime> _penaltyExpirations = new();

        /// <summary>
        /// Applies the death penalty to the given player. If penalties are disabled via configuration,
        /// this method does nothing.
        /// </summary>
        /// <param name="playerId">Unique identifier for the player (e.g., network ID).</param>
        public void ApplyPenalty(string playerId)
        {
            if (!FeatureFlags.DeathPenaltyEnabled) return;
            if (string.IsNullOrEmpty(playerId)) return;
            var until = DateTime.UtcNow.AddHours(FeatureFlags.DeathPenaltyHours);
            _penaltyExpirations[playerId] = until;
        }

        /// <summary>
        /// Clears the penalty for the given player (e.g., when healed by a medic).
        /// </summary>
        public void ClearPenalty(string playerId)
        {
            if (string.IsNullOrEmpty(playerId)) return;
            _penaltyExpirations.TryRemove(playerId, out _);
        }

        public void Clear(string playerId) => ClearPenalty(playerId);

        /// <summary>
        /// Determines whether the given player is currently under a death penalty.
        /// </summary>
        public bool IsRestricted(string playerId)
        {
            if (!FeatureFlags.DeathPenaltyEnabled) return false;
            if (string.IsNullOrEmpty(playerId)) return false;
            if (_penaltyExpirations.TryGetValue(playerId, out var expiry))
            {
                if (DateTime.UtcNow >= expiry)
                {
                    // penalty expired; remove it
                    _penaltyExpirations.TryRemove(playerId, out _);
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the remaining penalty time for a given player in seconds, or null if none.
        /// </summary>
        public double? GetRemainingSeconds(string playerId)
        {
            if (IsRestricted(playerId))
            {
                var expiry = _penaltyExpirations[playerId];
                return (expiry - DateTime.UtcNow).TotalSeconds;
            }
            return null;
        }
    }
}