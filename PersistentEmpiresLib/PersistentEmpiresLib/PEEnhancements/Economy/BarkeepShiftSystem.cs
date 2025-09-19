#if SERVER
using System;
using System.Collections.Generic;
using PersistentEmpiresLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace PEEnhancements.Economy
{
    /// <summary>
    /// Tracks barkeep shifts and grants periodic payouts while players are on duty.
    /// </summary>
    internal static class BarkeepShiftSystem
    {
        private sealed class ShiftInfo
        {
            public string Id { get; }
            public WeakReference<NetworkCommunicator> Peer { get; }
            public DateTime EndsAt { get; set; }
            public float Accumulator { get; set; }

            public ShiftInfo(string id, NetworkCommunicator peer, TimeSpan duration)
            {
                Id = id;
                Peer = new WeakReference<NetworkCommunicator>(peer);
                EndsAt = DateTime.UtcNow + duration;
            }
        }

        private static readonly Dictionary<string, ShiftInfo> _active = new(StringComparer.OrdinalIgnoreCase);
        private static readonly object _lock = new();

        private static bool _initialised;
        private static TimeSpan _shiftDuration = TimeSpan.FromMinutes(30);
        private static float _payoutIntervalSeconds = 60f;
        private static int _payoutGold = 50;

        public static void Init(string moduleRootPath)
        {
            lock (_lock)
            {
                if (_initialised)
                {
                    return;
                }

                _ = moduleRootPath;
                // Use configuration values if available.
                _shiftDuration = TimeSpan.FromMinutes(Math.Max(1, FeatureFlags.EconomyBarkeepShiftMinutes));
                _payoutIntervalSeconds = Math.Max(5, FeatureFlags.EconomyBarkeepPayoutIntervalSeconds);
                _payoutGold = Math.Max(0, FeatureFlags.EconomyBarkeepPayoutGold);
                _initialised = true;
                Debug.Print($"[PEEnhancements] Barkeep system ready (duration={_shiftDuration.TotalMinutes}m, interval={_payoutIntervalSeconds}s, payout={_payoutGold}).");
            }
        }

        public static void Tick(float dt)
        {
            if (!_initialised)
            {
                return;
            }

            List<string> expired = null;

            lock (_lock)
            {
                if (_active.Count == 0)
                {
                    return;
                }

                var now = DateTime.UtcNow;
                foreach (var kvp in _active)
                {
                    var shift = kvp.Value;

                    if (shift.EndsAt <= now)
                    {
                        if (expired == null)
                        {
                            expired = new List<string>();
                        }
                        expired.Add(kvp.Key);
                        continue;
                    }

                    if (!shift.Peer.TryGetTarget(out var peer) || peer == null || !peer.IsConnectionActive)
                    {
                        if (expired == null)
                        {
                            expired = new List<string>();
                        }
                        expired.Add(kvp.Key);
                        continue;
                    }

                    shift.Accumulator += dt;
                    if (shift.Accumulator >= _payoutIntervalSeconds)
                    {
                        shift.Accumulator -= _payoutIntervalSeconds;
                        AwardPayout(peer);
                    }
                }

                if (expired != null)
                {
                    foreach (var key in expired)
                    {
                        _active.Remove(key);
                        Debug.Print($"[PEEnhancements] Barkeep shift finished for {key}.");
                    }
                }
            }
        }

        public static bool ToggleShift(NetworkCommunicator peer)
        {
            if (!_initialised || peer == null)
            {
                return false;
            }

            var id = GetPeerIdentifier(peer);
            lock (_lock)
            {
                if (_active.Remove(id))
                {
                    Debug.Print($"[PEEnhancements] Barkeep shift stopped for {id}.");
                    return false;
                }

                _active[id] = new ShiftInfo(id, peer, _shiftDuration);
                Debug.Print($"[PEEnhancements] Barkeep shift started for {id}.");
                return true;
            }
        }

        private static void AwardPayout(NetworkCommunicator peer)
        {
            if (_payoutGold <= 0)
            {
                return;
            }

            var representative = peer.GetComponent<PersistentEmpireRepresentative>();
            if (representative == null)
            {
                return;
            }

            representative.GoldGain(_payoutGold);
        }

        private static string GetPeerIdentifier(NetworkCommunicator peer)
        {
            return peer.UserName ?? peer.VirtualPlayer?.ToString() ?? peer.ToString();
        }
    }
}
#endif
