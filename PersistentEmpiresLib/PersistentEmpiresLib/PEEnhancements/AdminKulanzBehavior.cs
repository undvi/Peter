using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace PEEnhancements
{
    /// <summary>
    /// MissionBehavior, das lediglich die Admin-Services initialisiert.
    /// Chat-/Slash-Command-Parsing ist projektspezifisch – hier nicht erzwungen.
    /// </summary>
    public class AdminKulanzBehavior : MissionLogic
    {
        public static AdminKulanzBehavior? Instance { get; private set; }

        public override void OnBehaviorInitialize()
        {
            base.OnBehaviorInitialize();
            Instance = this;
        }

        public override void AfterStart()
        {
            base.AfterStart();
            try
            {
                var root = TaleWorlds.ModuleManager.ModuleHelper.GetModuleFullPath("PersistentEmpires");
                AdminCompensation.Init(root);
                IncidentLogger.Init(root);
            }
            catch (Exception e)
            {
                Debug.Print("[PEEnhancements] Admin init failed: " + e.Message);
            }
        }

        // --- Public API, z.B. aufrufbar aus eurem ChatCommand-Handler ---
        public bool Compensate(string adminId, string playerId, string itemId, int amount, string reason)
        {
            return AdminCompensation.Request(adminId, playerId, itemId, amount, reason);
        }

        public void IncidentNote(string reporterId, string incidentId, string clipUrl, string note)
        {
            IncidentLogger.Append(reporterId, incidentId, clipUrl, note);
        }
    }
}
