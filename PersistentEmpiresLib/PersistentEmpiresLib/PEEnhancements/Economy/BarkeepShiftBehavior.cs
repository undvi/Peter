using System.Collections.Generic;
using TaleWorlds.MountAndBlade;

namespace PEEnhancements.Economy
{
    /// <summary>
    /// Mission behavior that keeps track of barkeep shifts on the server.
    /// </summary>
    public sealed class BarkeepShiftBehavior : MissionBehavior
    {
        private readonly Dictionary<string, MissionPeer> _activeBarkeepers = new Dictionary<string, MissionPeer>();

        public static BarkeepShiftBehavior? Instance { get; private set; }

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public override void OnBehaviorInitialize()
        {
            base.OnBehaviorInitialize();
            Instance = this;
        }

        public override void OnRemoveBehavior()
        {
            base.OnRemoveBehavior();
            if (Instance == this)
            {
                Instance = null;
            }

            _activeBarkeepers.Clear();
        }

        /// <summary>
        /// Server side helper that toggles the shift of the provided peer.
        /// </summary>
        public bool TryServerToggleViaPeer(NetworkCommunicator peer)
        {
            if (!GameNetwork.IsServer || peer == null)
            {
                return false;
            }

            string barkeepId = peer.UserName ?? peer.VirtualPlayer?.ToString() ?? peer.ToString();

            if (_activeBarkeepers.ContainsKey(barkeepId))
            {
                _activeBarkeepers.Remove(barkeepId);
                BarkeepShiftSystem.EndShift(barkeepId);
                return true;
            }

            MissionPeer missionPeer = peer.GetComponent<MissionPeer>();
            if (missionPeer != null)
            {
                _activeBarkeepers[barkeepId] = missionPeer;
            }

            BarkeepShiftSystem.BeginOrExtendShift(barkeepId);
            return true;
        }

        public bool IsOnShift(string barkeepId) => _activeBarkeepers.ContainsKey(barkeepId);
    }
}
