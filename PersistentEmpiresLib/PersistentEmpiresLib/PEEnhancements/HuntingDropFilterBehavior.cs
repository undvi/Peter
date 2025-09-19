using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using TaleWorlds.Core;

namespace PEEnhancements
{
    /// <summary>
    /// MVP-Filter: Wenn Hunting-Flag aktiv ist, akzeptieren wir Fleisch-Drops nur,
    /// wenn der Killer gerade eine Waffe mit StringId "hunter_bow" führt.
    /// NOTE: Die eigentliche Drop-Erzeugung ist meist in separaten Komponenten.
    /// Dieser Behavior dient als „Gate“, das andere Systeme leicht abfragen/konsultieren können.
    /// </summary>
    public class HuntingDropFilterBehavior : MissionLogic
    {
        public static HuntingDropFilterBehavior? Instance { get; private set; }
        public override void OnBehaviorInitialize()
        {
            base.OnBehaviorInitialize();
            Instance = this;
        }

        /// <summary>
        /// Kann von Drop-Systemen/SceneScripts abgefragt werden, bevor Fleisch gespawnt wird.
        /// </summary>
        public bool AllowMeatForKill(Agent? killer)
        {
            if (!FeatureFlags.HuntingOnlyHunterBowMeat) return true;
            if (killer == null) return false;
            var itemId = killer.WieldedWeapon.Item?.StringId;
            return itemId == "hunter_bow";
        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
        {
            base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
            if (!FeatureFlags.HuntingOnlyHunterBowMeat) return;
            if (affectedAgent == null || affectedAgent.IsHuman) return; // wir interessieren uns nur für Tiere

            var ok = AllowMeatForKill(affectorAgent);
            if (!ok)
            {
                // Hier nur Debug-Info. Das eigentliche Unterbinden des Drops
                // sollte dort passieren, wo Fleisch gespawnt wird:
                // if (!HuntingDropFilterBehavior.Instance.AllowMeatForKill(killer)) -> spawn skippen.
                Debug.Print("[PEEnhancements] HuntingGate: meat blocked (killer has no hunter_bow)");
            }
        }
    }
}
