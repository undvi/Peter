using System;
using System.IO;

namespace PEEnhancements
{
    /// <summary>
    /// Loads PE enhancement settings from disk and initializes the <see cref="FeatureFlags"/> system.
    /// </summary>
    public static class PeSettingsLoader
    {
        /// <summary>
        /// Attempts to load the configuration file from the given path. If the file does not exist
        /// or cannot be read, default values are used.
        /// </summary>
        /// <param name="filePath">Absolute or relative path to the JSON settings file.</param>
        public static void LoadFromFile(string filePath)
        {
            string json;
            try
            {
                json = File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;
            }
            catch
            {
                json = string.Empty;
            }
            FeatureFlags.Initialize(json);
        }
    }
}