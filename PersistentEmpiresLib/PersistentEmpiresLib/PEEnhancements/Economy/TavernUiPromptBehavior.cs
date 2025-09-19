using System;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace PEEnhancements.Economy
{
    /// <summary>
    /// Client side prompts and helper interactions for barkeep/tavern areas.
    /// </summary>
    public class TavernUiPromptBehavior : MissionLogic
    {
        public static TavernUiPromptBehavior? Instance { get; private set; }

        private GameEntity[] _spots = Array.Empty<GameEntity>();
        private float _lastMsgAt;

        private const float PromptIntervalSeconds = 2.5f;
        private const float PromptRange = 5.0f;

        public override void OnBehaviorInitialize()
        {
            base.OnBehaviorInitialize();
            Instance = this;
        }

        public override void OnRemoveBehaviour()
        {
            base.OnRemoveBehaviour();
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public override void AfterStart()
        {
            base.AfterStart();
#if CLIENT
            try
            {
                var areas = Mission.Current?.Scene?.FindEntitiesWithTag("tavern_area")?.ToArray() ?? Array.Empty<GameEntity>();
                var counters = Mission.Current?.Scene?.FindEntitiesWithTag("tavern_counter")?.ToArray() ?? Array.Empty<GameEntity>();
                _spots = areas.Concat(counters).Where(e => e != null).Distinct().ToArray();
            }
            catch
            {
                _spots = Array.Empty<GameEntity>();
            }
#endif
        }

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);
#if CLIENT
            if (!FeatureFlags.EconomyBarkeepEnabled)
            {
                return;
            }

            var peer = GameNetwork.MyPeer;
            var agent = Mission.MainAgent;
            if (peer == null || agent == null || !agent.IsActive())
            {
                return;
            }

            var now = MissionTime.Now.ToSeconds;
            var near = IsNearTavern(agent);
            if (near && now >= _lastMsgAt + PromptIntervalSeconds)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Taverne: [E] Schicht starten/stoppen • [T] Trinkgeld geben",
                    Color.FromUint(0xFF8BC34Au)));
                _lastMsgAt = now;
            }

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
                InformationManager.DisplayMessage(new InformationMessage(
                    "Trinkgeld: Nutze /tip <Barkeeper> <Betrag> oder euer Interaktionsmenü.",
                    Color.FromUint(0xFFFFC107u)));
            }
#endif
        }

        private bool IsNearTavern(Agent agent)
        {
            if (_spots == null || _spots.Length == 0)
            {
                return false;
            }

            var position = agent.Position;
            foreach (var entity in _spots)
            {
                try
                {
                    if (entity == null)
                    {
                        continue;
                    }

                    if (entity.GlobalPosition.Distance(position) <= PromptRange)
                    {
                        return true;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            return false;
        }

        private void TryToggleShiftClientNotify()
        {
            InformationManager.DisplayMessage(new InformationMessage(
                "Schicht-Toggle angefragt …",
                Color.FromUint(0xFF03A9F4u)));
            // Network message hook can be added here once available.
        }
    }
}
