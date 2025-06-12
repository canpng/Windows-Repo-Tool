using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace WindowsRepoTool
{
    public static class LocalizationManager
    {
        private static Dictionary<string, string> _localizedStrings = new Dictionary<string, string>();

        public static async Task LoadLanguage(string langCode)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Languages", $"{langCode}.json");
            if (File.Exists(filePath))
            {
                string json = await File.ReadAllTextAsync(filePath);
                _localizedStrings = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            }
            else
            {
                // Fallback to English if the selected language file doesn't exist
                filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Languages", "en.json");
                if (File.Exists(filePath))
                {
                    string json = await File.ReadAllTextAsync(filePath);
                    _localizedStrings = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
            }
        }

        public static string GetString(string key)
        {
            return _localizedStrings.TryGetValue(key, out var value) ? value : $"_{{{key}}}_";
        }
    }
} 