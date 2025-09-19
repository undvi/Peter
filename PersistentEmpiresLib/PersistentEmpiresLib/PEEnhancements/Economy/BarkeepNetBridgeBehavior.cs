using TaleWorlds.MountAndBlade;
using PEEnhancements.Economy.Net;

namespace PEEnhancements.Economy
{
    /// <summary>
    /// Bridges client network messages for the barkeep economy features to the server logic.
    /// </summary>
    public class BarkeepNetBridgeBehavior : MissionNetwork
    {
        public static BarkeepNetBridgeBehavior? Instance { get; private set; }

        public override void OnBehaviorInitialize()
        {
            base.OnBehaviorInitialize();
            Instance = this;
        }

        public override void AfterStart()
        {
            base.AfterStart();
            if (!GameNetwork.IsServer)
            {
                return;
            }

            RegisterMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);

            if (Mission.Current != null && Mission.Current.GetMissionBehavior<BarkeepShiftBehavior>() == null)
            {
                Mission.Current.AddMissionBehavior(new BarkeepShiftBehavior());
            }
        }

        public override void OnRemoveBehavior()
        {
            if (Instance == this)
            {
                Instance = null;
            }

            if (GameNetwork.IsServer)
            {
                RegisterMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
            }

            base.OnRemoveBehavior();
        }

        private void RegisterMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
        {
            var registerer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
            registerer.Register<MsgBarkeepToggle>(HandleBarkeepToggleFromClient);
            registerer.Register<MsgTipRegister>(HandleTipRegisterFromClient);
        }

        private bool HandleBarkeepToggleFromClient(NetworkCommunicator fromPeer, MsgBarkeepToggle message)
        {
            if (!FeatureFlags.EconomyBarkeepEnabled || fromPeer == null)
            {
                return true;
            }

            return BarkeepShiftBehavior.Instance?.TryServerToggleViaPeer(fromPeer) ?? true;
        }

        private bool HandleTipRegisterFromClient(NetworkCommunicator fromPeer, MsgTipRegister message)
        {
            if (!FeatureFlags.EconomyBarkeepEnabled || fromPeer == null)
            {
                return true;
            }

            if (message.Amount <= 0 || string.IsNullOrWhiteSpace(message.BarkeepId))
            {
                return true;
            }

            string payerId = ResolvePeerId(fromPeer);
            BarkeepShiftSystem.RegisterTip(payerId, message.BarkeepId, message.Amount);
            return true;
        }

        private static string ResolvePeerId(NetworkCommunicator peer)
        {
            if (!string.IsNullOrEmpty(peer.UserName))
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
