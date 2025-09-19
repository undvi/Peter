using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace PEEnhancements.Economy
{
    /// <summary>
    /// Shared system logic that keeps track of barkeep shifts and registered tips.
    /// </summary>
    public static class BarkeepShiftSystem
    {
        private sealed class BarkeepShiftState
        {
            private readonly HashSet<string> _uniquePayers = new HashSet<string>();

            public string BarkeepId { get; }

            public DateTime StartedAt { get; private set; }

            public DateTime LastActivityUtc { get; private set; }

            public int TotalTipAmount { get; private set; }

            public BarkeepShiftState(string barkeepId)
            {
                BarkeepId = barkeepId;
                StartedAt = DateTime.UtcNow;
                LastActivityUtc = StartedAt;
            }

            public void RefreshShift()
            {
                if (StartedAt == DateTime.MinValue)
                {
                    StartedAt = DateTime.UtcNow;
                }

                LastActivityUtc = DateTime.UtcNow;
            }

            public void RegisterTip(string payerId, int amount)
            {
                TotalTipAmount += Math.Max(0, amount);
                LastActivityUtc = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(payerId))
                {
                    _uniquePayers.Add(payerId);
                }
            }
        }

        private static readonly Dictionary<string, BarkeepShiftState> ActiveShifts = new Dictionary<string, BarkeepShiftState>();

        private static readonly object SyncRoot = new object();

        /// <summary>
        /// Starts a new shift for the specified barkeep or refreshes the current one.
        /// </summary>
        public static void BeginOrExtendShift(string barkeepId)
        {
            if (string.IsNullOrWhiteSpace(barkeepId))
            {
                return;
            }

            lock (SyncRoot)
            {
                if (!ActiveShifts.TryGetValue(barkeepId, out BarkeepShiftState state))
                {
                    state = new BarkeepShiftState(barkeepId);
                    ActiveShifts[barkeepId] = state;
                    Debug.Print($"[PE] Barkeep shift started for {barkeepId}.");
                }
                else
                {
                    state.RefreshShift();
                    Debug.Print($"[PE] Barkeep shift refreshed for {barkeepId}.");
                }
            }
        }

        /// <summary>
        /// Ends the running shift for the specified barkeep.
        /// </summary>
        public static void EndShift(string barkeepId)
        {
            if (string.IsNullOrWhiteSpace(barkeepId))
            {
                return;
            }

            lock (SyncRoot)
            {
                if (ActiveShifts.Remove(barkeepId))
                {
                    Debug.Print($"[PE] Barkeep shift ended for {barkeepId}.");
                }
            }
        }

        /// <summary>
        /// Registers a tip for the target barkeep. If the barkeep is not currently on shift,
        /// the shift will be started automatically.
        /// </summary>
        public static void RegisterTip(string payerId, string barkeepId, int amount)
        {
            if (string.IsNullOrWhiteSpace(barkeepId) || amount <= 0)
            {
                return;
            }

            lock (SyncRoot)
            {
                if (!ActiveShifts.TryGetValue(barkeepId, out BarkeepShiftState state))
                {
                    state = new BarkeepShiftState(barkeepId);
                    ActiveShifts[barkeepId] = state;
                    Debug.Print($"[PE] Barkeep shift auto-started for {barkeepId} due to tip.");
                }

                state.RegisterTip(payerId, amount);
                Debug.Print($"[PE] Tip registered for {barkeepId}: {amount} denars by {payerId}.");
            }
        }
    }
}
