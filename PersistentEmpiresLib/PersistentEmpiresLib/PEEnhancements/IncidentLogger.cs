using System;
using System.IO;
using System.Text;

namespace PEEnhancements
{
    /// <summary>
    /// MVP-Incident-Logger mit CSV-Ausgabe (UTC-Zeitstempel).
    /// </summary>
    public static class IncidentLogger
    {
        private static string _path = string.Empty;

        public static void Init(string moduleRoot)
        {
            var logDir = Path.Combine(moduleRoot, "Logs");
            Directory.CreateDirectory(logDir);
            _path = Path.Combine(logDir, "pe.incidents.csv");
            if (!File.Exists(_path))
            {
                File.WriteAllText(_path, "timestamp;reporterId;incidentId;clipUrl;note\n", Encoding.UTF8);
            }
        }

        public static void Append(string reporterId, string incidentId, string clipUrl, string note)
        {
            if (string.IsNullOrEmpty(_path))
            {
                return;
            }

            var line = $"{DateTime.UtcNow:O};{reporterId};{incidentId};{clipUrl};{note}".Replace('\n', ' ').Replace('\r', ' ');
            File.AppendAllText(_path, line + "\n", Encoding.UTF8);
        }
    }
}
