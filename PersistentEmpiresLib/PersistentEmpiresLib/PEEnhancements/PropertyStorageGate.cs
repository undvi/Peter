using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace PEEnhancements
{
    /// <summary>
    /// Zentrale Zugriffskontrolle für Property-gebundene Storages.
    /// </summary>
    public static class PropertyStorageGate
    {
        /// <summary>
        /// Prüft, ob <paramref name="player"/> auf ein Storage mit ID zugreifen darf.
        /// ID kann aus SceneProp-Tag "property:&lt;ID&gt;" stammen.
        /// </summary>
        public static bool Check(string propertyId, NetworkCommunicator player)
        {
            if (!FeatureFlags.PropertyEnabled) return true;
            if (string.IsNullOrWhiteSpace(propertyId) || player == null) return true;

            var pid = player.UserName ?? player.VirtualPlayer?.ToString() ?? player.ToString();
            var ok = PropertyMvpBehavior.Instance?.IsAllowed(propertyId, pid) ?? true;
            if (!ok)
            {
                InformationComponent.Instance.SendMessage(
                    $"Zugriff verweigert (Eigentum: {propertyId})",
                    Color.ConvertStringToColor("#F44336FF").ToUnsignedInteger(),
                    player);
            }
            return ok;
        }

        /// <summary>
        /// Extrahiert eine Property-ID aus einem SceneProp (z. B. Entity.Tag = "property:house_north_01").
        /// </summary>
        public static string? TryGetPropertyIdFrom(GameEntity? entity)
        {
            try
            {
                if (entity == null) return null;
                var tag = entity.GetFirstTag() ?? "";
                if (string.IsNullOrEmpty(tag)) return null;
                if (tag.StartsWith("property:", StringComparison.OrdinalIgnoreCase))
                    return tag.Substring("property:".Length);
            }
            catch
            {
                // ignore
            }
            return null;
        }
    }
}
