using PEEnhancements.Economy.Net;
using PEEnhancements;
using TaleWorlds.MountAndBlade;

namespace PEEnhancements.Economy
{
    /// <summary>
    /// Mission network bridge that registers the server handlers for barkeep-related network messages
    /// and executes the economy actions on the server side.
    /// </summary>
    public sealed class BarkeepNetBridgeBehavior : MissionNetwork
    {
        public static BarkeepNetBridgeBehavior? Instance { get; private set; }

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
        }

        protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
        {
            if (!GameNetwork.IsServer)
            {
                return;
            }

            registerer.Register<MsgBarkeepToggle>(HandleBarkeepToggleFromClient);
            registerer.Register<MsgTipRegister>(HandleTipRegisterFromClient);
        }

        private bool HandleBarkeepToggleFromClient(NetworkCommunicator fromPeer, MsgBarkeepToggle _)
        {
            if (!FeatureFlags.EconomyBarkeepEnabled || fromPeer == null)
            {
                return true;
            }

            return BarkeepShiftBehavior.Instance?.TryServerToggleViaPeer(fromPeer) ?? true;
        }

        private bool HandleTipRegisterFromClient(NetworkCommunicator fromPeer, MsgTipRegister msg)
        {
            if (!FeatureFlags.EconomyBarkeepEnabled || fromPeer == null)
            {
                return true;
            }

            if (msg.Amount <= 0 || string.IsNullOrWhiteSpace(msg.BarkeepId))
            {
                return true;
            }

            string payerId = fromPeer.UserName ?? fromPeer.VirtualPlayer?.ToString() ?? fromPeer.ToString();
            BarkeepShiftSystem.RegisterTip(payerId, msg.BarkeepId, msg.Amount);
            return true;
        }
    }
}
