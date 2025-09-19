using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace PEEnhancements.Economy
{
    /// <summary>
    /// Tracks the state of barkeep shifts and tip amounts server-side.
    /// </summary>
    public static class BarkeepShiftSystem
    {
        private sealed class BarkeepShiftState
        {
            public bool OnShift;
            public DateTime LastToggleUtc;
            public int TotalTips;
            public Dictionary<string, int> TipsByPayer = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        private static readonly Dictionary<string, BarkeepShiftState> ShiftStates = new Dictionary<string, BarkeepShiftState>(StringComparer.OrdinalIgnoreCase);
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// Toggles the shift for the provided barkeep identifier.
        /// </summary>
        public static bool ToggleShift(string barkeepId)
        {
            if (string.IsNullOrWhiteSpace(barkeepId))
            {
                return false;
            }

            lock (SyncRoot)
            {
                var state = GetOrCreateState(barkeepId);
                state.OnShift = !state.OnShift;
                state.LastToggleUtc = DateTime.UtcNow;
                ShiftStates[barkeepId] = state;

                Debug.Print($"[PE] Barkeep shift {(state.OnShift ? "started" : "stopped")}: {barkeepId}", 0, DebugColor.Cyan);
                return state.OnShift;
            }
        }

        /// <summary>
        /// Registers a tip transfer between a payer and a barkeep.
        /// </summary>
        public static void RegisterTip(string payerId, string barkeepId, int amount)
        {
            if (string.IsNullOrWhiteSpace(barkeepId) || amount <= 0)
            {
                return;
            }

            lock (SyncRoot)
            {
                var state = GetOrCreateState(barkeepId);
                state.TotalTips += amount;
                if (!string.IsNullOrWhiteSpace(payerId))
                {
                    if (state.TipsByPayer.TryGetValue(payerId, out var current))
                    {
                        state.TipsByPayer[payerId] = current + amount;
                    }
                    else
                    {
                        state.TipsByPayer[payerId] = amount;
                    }
                }

                Debug.Print($"[PE] Tip registered for {barkeepId} from {payerId}: {amount}", 0, DebugColor.Cyan);
            }
        }

        /// <summary>
        /// Returns whether the barkeep is currently on shift.
        /// </summary>
        public static bool IsOnShift(string barkeepId)
        {
            if (string.IsNullOrWhiteSpace(barkeepId))
            {
                return false;
            }

            lock (SyncRoot)
            {
                return ShiftStates.TryGetValue(barkeepId, out var state) && state.OnShift;
            }
        }

        private static BarkeepShiftState GetOrCreateState(string barkeepId)
        {
            if (!ShiftStates.TryGetValue(barkeepId, out var state))
            {
                state = new BarkeepShiftState();
                ShiftStates[barkeepId] = state;
            }

            return state;
        }
    }
}
