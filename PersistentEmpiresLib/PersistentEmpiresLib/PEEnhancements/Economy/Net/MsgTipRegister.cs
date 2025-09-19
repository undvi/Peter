using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace PEEnhancements.Economy.Net
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class MsgTipRegister : GameNetworkMessage
    {
        public MsgTipRegister()
        {
        }

        public MsgTipRegister(string barkeepId, int amount)
        {
            BarkeepId = barkeepId;
            Amount = amount;
        }

        public string BarkeepId { get; private set; }

        public int Amount { get; private set; }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.MissionObjects;
        }

        protected override string OnGetLogFormat()
        {
            return "Register tip";
        }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;
            BarkeepId = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
            Amount = GameNetworkMessage.ReadIntFromPacket(new CompressionInfo.Integer(0, int.MaxValue, true), ref bufferReadValid);
            return bufferReadValid;
        }

        protected override void OnWrite()
        {
            GameNetworkMessage.WriteStringToPacket(BarkeepId);
            GameNetworkMessage.WriteIntToPacket(Amount, new CompressionInfo.Integer(0, int.MaxValue, true));
        }
    }
}
