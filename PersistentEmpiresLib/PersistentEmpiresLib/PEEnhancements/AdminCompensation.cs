using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace PEEnhancements
{
    /// <summary>
    /// MVP: Schreibt Kompensations-Aufträge in eine CSV-Queue + Log.
    /// Die tatsächliche Item-/Gold-Ausgabe hängt an eurem existierenden Inventar-/Banking-API
    /// und kann über einen Callback integriert werden.
    /// </summary>
    public static class AdminCompensation
    {
        public delegate bool GrantHandler(string playerId, string itemId, int amount, string reason);

        /// <summary>
        /// Optionaler Hook: Wenn gesetzt, wird reale Ausgabe versucht; sonst nur CSV-Queue.
        /// </summary>
        public static GrantHandler? OnGrant;

        private static string _queuePath = string.Empty;
        private static string _logPath = string.Empty;
        private static readonly ConcurrentQueue<string> _pending = new();

        public static void Init(string moduleRoot)
        {
            var logDir = Path.Combine(moduleRoot, "Logs");
            Directory.CreateDirectory(logDir);
            _queuePath = Path.Combine(logDir, "pe.compensations.todo.csv");
            _logPath = Path.Combine(logDir, "pe.compensations.done.csv");
        }

        public static bool Request(string adminId, string playerId, string itemId, int amount, string reason)
        {
            var line = $"{DateTime.UtcNow:O};{adminId};{playerId};{itemId};{amount};{reason}";
            _pending.Enqueue(line);
            FlushQueue();

            if (OnGrant != null)
            {
                try
                {
                    var ok = OnGrant(playerId, itemId, amount, reason);
                    File.AppendAllText(_logPath, $"{line};granted={ok}\\n", Encoding.UTF8);
                    return ok;
                }
                catch (Exception e)
                {
                    File.AppendAllText(_logPath, $"{line};error={e.Message}\\n", Encoding.UTF8);
                    return false;
                }
            }

            return true;
        }

        private static void FlushQueue()
        {
            if (string.IsNullOrEmpty(_queuePath))
            {
                return;
            }

            using var sw = new StreamWriter(_queuePath, append: true, Encoding.UTF8);
            while (_pending.TryDequeue(out var line))
            {
                sw.WriteLine(line);
            }
        }
    }
}
