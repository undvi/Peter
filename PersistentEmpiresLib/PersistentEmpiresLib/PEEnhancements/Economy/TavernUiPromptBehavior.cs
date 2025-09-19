using PEEnhancements.Economy.Net;
using PEEnhancements;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace PEEnhancements.Economy
{
    /// <summary>
    /// Client-side helper behavior that allows requesting barkeep actions from the UI.
    /// </summary>
    public sealed class TavernUiPromptBehavior : MissionBehavior
    {
        public static TavernUiPromptBehavior? Instance { get; private set; }

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
        }

        /// <summary>
        /// Shows a local notification and, if appropriate, asks the server to toggle the shift.
        /// </summary>
        public void TryToggleShiftClientNotify()
        {
            if (!FeatureFlags.EconomyBarkeepEnabled)
            {
                return;
            }

            InformationManager.DisplayMessage(new InformationMessage("Schicht-Toggle angefragt …", Color.FromUint(0xFF03A9F4)));
            if (GameNetwork.IsClient)
            {
                GameNetwork.BeginModuleEventAsClient();
                GameNetwork.WriteMessage(new MsgBarkeepToggle());
                GameNetwork.EndModuleEventAsClient();
            }
        }

        /// <summary>
        /// Sends a tip registration request to the server.
        /// </summary>
        public void RegisterTip(string barkeepId, int amount)
        {
            if (!FeatureFlags.EconomyBarkeepEnabled)
            {
                return;
            }

            if (!GameNetwork.IsClient || string.IsNullOrWhiteSpace(barkeepId) || amount <= 0)
            {
                return;
            }

            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.WriteMessage(new MsgTipRegister(barkeepId, amount));
            GameNetwork.EndModuleEventAsClient();
        }
    }
}
