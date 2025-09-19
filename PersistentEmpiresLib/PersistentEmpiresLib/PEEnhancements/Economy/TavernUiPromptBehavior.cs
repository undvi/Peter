using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using PEEnhancements.Economy.Net;

namespace PEEnhancements.Economy
{
    public class TavernUiPromptBehavior : MissionBehavior
    {
        private static readonly string[] TavernShiftTags =
        {
            "pe_barkeep_shift",
            "pe_tavern_shift"
        };

        private const float InteractionRadius = 4f;

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

#if CLIENT
        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);
            if (!GameNetwork.IsClient)
            {
                return;
            }

            Agent mainAgent = Agent.Main;
            if (mainAgent == null || !mainAgent.IsHuman || !mainAgent.IsActive())
            {
                return;
            }

            bool near = IsNearTavernShift(mainAgent);
            if (!near)
            {
                return;
            }

            if (Input.IsKeyReleased(InputKey.E))
            {
                TryToggleShiftClientNotify();
            }

            if (Input.IsKeyReleased(InputKey.T))
            {
                OpenTipPrompt();
            }
        }
#endif

        private static bool IsNearTavernShift(Agent agent)
        {
            if (agent == null || Mission.Current == null || Mission.Current.Scene == null)
            {
                return false;
            }

            float sqrRadius = InteractionRadius * InteractionRadius;
            Vec3 agentPosition = agent.Position;

            foreach (string tag in TavernShiftTags)
            {
                var entities = Mission.Current.Scene.FindEntitiesWithTag(tag);
                if (entities == null)
                {
                    continue;
                }

                foreach (GameEntity entity in entities)
                {
                    if (entity == null)
                    {
                        continue;
                    }

                    Vec3 position = entity.GlobalPosition;
                    if (position.DistanceSquared(agentPosition) <= sqrRadius)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void TryToggleShiftClientNotify()
        {
            InformationManager.DisplayMessage(new InformationMessage("Schicht-Toggle angefragt …", Color.FromUint(0xFF03A9F4)));
            if (GameNetwork.IsClient)
            {
                GameNetwork.BeginModuleEventAsClient();
                GameNetwork.WriteMessage(new MsgBarkeepToggle());
                GameNetwork.EndModuleEventAsClient();
            }
        }

        private void OpenTipPrompt()
        {
            var amountData = new TextInquiryData(
                "Trinkgeld",
                "Betrag eingeben (≥ 1):",
                true,
                true,
                "Weiter",
                "Abbrechen",
                s =>
                {
                    if (!int.TryParse(s, out var amount) || amount < 1)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("Ungültiger Betrag.", Color.FromUint(0xFFF44336)));
                        return;
                    }

                    var barkeepData = new TextInquiryData(
                        "Trinkgeld",
                        "Barkeeper-ID (Spielername) eingeben:",
                        true,
                        true,
                        "Senden",
                        "Abbrechen",
                        id =>
                        {
                            if (string.IsNullOrWhiteSpace(id))
                            {
                                InformationManager.DisplayMessage(new InformationMessage("Kein Ziel angegeben.", Color.FromUint(0xFFF44336)));
                                return;
                            }

                            SendTipRegister(id.Trim(), amount);
                        },
                        null);

                    InformationManager.ShowTextInquiry(barkeepData, false);
                },
                null);

            InformationManager.ShowTextInquiry(amountData, false);
        }

        private void SendTipRegister(string barkeepId, int amount)
        {
            if (!GameNetwork.IsClient)
            {
                return;
            }

            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.WriteMessage(new MsgTipRegister(barkeepId, amount));
            GameNetwork.EndModuleEventAsClient();
            InformationManager.DisplayMessage(new InformationMessage($"Trinkgeld gesendet: {amount} an {barkeepId}", Color.FromUint(0xFFFFC107)));
        }
    }
}
