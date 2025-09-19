using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace PEEnhancements.Economy.Net
{
    /// <summary>
    /// Client-&gt;Server: Toggle the barkeep shift for the requesting peer.
    /// </summary>
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class MsgBarkeepToggle : GameNetworkMessage
    {
        protected override bool OnRead()
        {
            return true;
        }

        protected override void OnWrite()
        {
        }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.Mission;
        }

        protected override string OnGetLogFormat()
        {
            return "[PE] BarkeepToggle request";
        }
    }
}
