using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace PEEnhancements.Economy
{
    /// <summary>
    /// Maintains barkeep shift state while a mission is running.
    /// </summary>
    public sealed class BarkeepShiftBehavior : MissionBehavior
    {
        public static BarkeepShiftBehavior? Instance { get; private set; }

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public override void OnBehaviorInitialize()
        {
            base.OnBehaviorInitialize();
            Instance = this;
        }

        public override void OnRemoveBehavior()
        {
            if (Instance == this)
            {
                Instance = null;
            }

            base.OnRemoveBehavior();
        }

        /// <summary>
        /// Server-side entry point that toggles the calling peer's barkeep shift.
        /// </summary>
        public bool TryServerToggleViaPeer(NetworkCommunicator peer)
        {
            if (!GameNetwork.IsServer || peer == null)
            {
                return true;
            }

            string barkeepId = ResolvePeerId(peer);
            bool nowOnShift = BarkeepShiftSystem.ToggleShift(barkeepId);

            Debug.Print($"[PE] Barkeep shift toggle via network: {barkeepId} -> {(nowOnShift ? "ON" : "OFF")}", 0, DebugColor.Cyan);
            return true;
        }

        private static string ResolvePeerId(NetworkCommunicator peer)
        {
            if (!string.IsNullOrWhiteSpace(peer.UserName))
            {
                return peer.UserName;
            }

            if (peer.VirtualPlayer != null)
            {
                return peer.VirtualPlayer.ToString();
            }

            return peer.ToString();
        }
    }
}
