using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace PEEnhancements.Economy
{
    /// <summary>
    /// Mission wrapper that initialises the barkeep shift system and ticks payouts.
    /// </summary>
    public class BarkeepShiftBehavior : MissionLogic
    {
        public static BarkeepShiftBehavior? Instance { get; private set; }

        public override void OnBehaviorInitialize()
        {
            base.OnBehaviorInitialize();
            Instance = this;
#if SERVER
            try
            {
                var root = TaleWorlds.ModuleManager.ModuleHelper.GetModuleFullPath("PersistentEmpires");
                BarkeepShiftSystem.Init(root);
            }
            catch (Exception e)
            {
                Debug.Print("[PEEnhancements] Barkeep init failed: " + e.Message);
            }
#endif
        }

        public override void OnRemoveBehaviour()
        {
            base.OnRemoveBehaviour();
            if (Instance == this)
            {
                Instance = null;
            }
        }

#if SERVER
        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);
            if (!FeatureFlags.EconomyBarkeepEnabled)
            {
                return;
            }

            BarkeepShiftSystem.Tick(dt);
        }

        /// <summary>
        /// Helper to toggle a barkeep shift server side via any interaction handler.
        /// </summary>
        public bool TryServerToggleViaPeer(NetworkCommunicator peer)
        {
            if (!FeatureFlags.EconomyBarkeepEnabled || peer == null)
            {
                return false;
            }

            return BarkeepShiftSystem.ToggleShift(peer);
        }
#endif
    }
}
