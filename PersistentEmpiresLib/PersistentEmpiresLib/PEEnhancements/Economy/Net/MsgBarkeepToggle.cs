using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace PEEnhancements.Economy.Net
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class MsgBarkeepToggle : GameNetworkMessage
    {
        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.MissionObjects;
        }

        protected override string OnGetLogFormat()
        {
            return "Barkeep shift toggle request";
        }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;
            return bufferReadValid;
        }

        protected override void OnWrite()
        {
        }
    }
}
