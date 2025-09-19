using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace PEEnhancements.Economy.Net
{
    /// <summary>
    /// Client-&gt;Server: Register a tip for a barkeep.
    /// </summary>
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class MsgTipRegister : GameNetworkMessage
    {
        public MsgTipRegister()
        {
        }

        public MsgTipRegister(string barkeepId, int amount)
        {
            BarkeepId = barkeepId ?? string.Empty;
            Amount = amount;
        }

        public string BarkeepId { get; private set; } = string.Empty;

        public int Amount { get; private set; }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;
            BarkeepId = ReadStringFromPacket(ref bufferReadValid);
            Amount = ReadIntFromPacket(CompressionBasic.Int32CompressionInfo, ref bufferReadValid);
            return bufferReadValid;
        }

        protected override void OnWrite()
        {
            WriteStringToPacket(BarkeepId);
            WriteIntToPacket(Amount, CompressionBasic.Int32CompressionInfo);
        }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.Mission;
        }

        protected override string OnGetLogFormat()
        {
            return $"[PE] TipRegister {BarkeepId}:{Amount}";
        }
    }
}
