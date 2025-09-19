using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace PEEnhancements.Economy.Net
{
    /// <summary>
    /// Client -> Server message requesting to toggle the barkeep shift.
    /// Payload free; identity is derived from the sending <see cref="NetworkCommunicator"/>.
    /// </summary>
    public sealed class MsgBarkeepToggle : GameNetworkMessage
    {
        protected override bool OnRead()
        {
            // No payload to read.
            return true;
        }

        protected override void OnWrite()
        {
            // No payload to write.
        }

        protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

        protected override string OnGetLogFormat() => "[PE] BarkeepToggle request";
    }
}
