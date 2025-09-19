using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PEEnhancements
{
    /// <summary>
    /// Minimal property/house ownership system. Supports claiming and unclaiming
    /// properties defined by unique identifiers. Permissions are simplified to owner only.
    /// </summary>
    public class PropertySystem
    {
        private readonly ConcurrentDictionary<string, string> _ownership = new();

        /// <summary>
        /// Attempts to claim a property for the given player. Returns false if the property
        /// is already claimed or if the feature is disabled.
        /// </summary>
        public bool Claim(string propertyId, string playerId)
        {
            if (!FeatureFlags.PropertyEnabled) return false;
            if (string.IsNullOrEmpty(propertyId) || string.IsNullOrEmpty(playerId)) return false;
            return _ownership.TryAdd(propertyId, playerId);
        }

        /// <summary>
        /// Unclaims the given property if owned by the player.
        /// </summary>
        public bool Unclaim(string propertyId, string playerId)
        {
            if (!FeatureFlags.PropertyEnabled) return false;
            if (_ownership.TryGetValue(propertyId, out var owner) && owner == playerId)
            {
                return _ownership.TryRemove(propertyId, out _);
            }
            return false;
        }

        /// <summary>
        /// Determines whether the given player has access to the property (owner only in this MVP).
        /// </summary>
        public bool HasAccess(string propertyId, string playerId)
        {
            if (!FeatureFlags.PropertyEnabled) return false;
            return _ownership.TryGetValue(propertyId, out var owner) && owner == playerId;
        }
    }
}