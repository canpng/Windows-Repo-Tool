using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reflection;
using System.Text;

namespace WindowsRepoTool
{
    public static class LocalizationManager
    {
        private static Dictionary<string, string> _currentLanguage = new Dictionary<string, string>();
        private const string EncryptionKey = "canpng";

        public static async Task LoadLanguage(string languageCode)
        {
            try
            {
                string resourceName = $"WindowsRepoTool.Languages.{languageCode}.encrypted";
                
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    await LoadFromFile(languageCode);
                    return;
                }

                using var reader = new BinaryReader(stream);
                var encryptedData = reader.ReadBytes((int)stream.Length);
                
                string decryptedJson = DecryptData(encryptedData);
                
                var languageData = JsonSerializer.Deserialize<Dictionary<string, string>>(decryptedJson);
                if (languageData != null)
                {
                    _currentLanguage = languageData;
                }
            }
            catch (Exception)
            {
                await LoadFromFile(languageCode);
            }
        }

        private static async Task LoadFromFile(string languageCode)
        {
            string fileName = $"Languages/{languageCode}.json";
            if (File.Exists(fileName))
            {
                string json = await File.ReadAllTextAsync(fileName);
                var languageData = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (languageData != null)
                {
                    _currentLanguage = languageData;
                }
            }
        }

        public static string GetString(string key)
        {
            return _currentLanguage.TryGetValue(key, out var value) ? value : $"_{key}_";
        }

        public static byte[] EncryptData(string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] keyBytes = Encoding.UTF8.GetBytes(EncryptionKey);
            byte[] encrypted = new byte[dataBytes.Length];

            for (int i = 0; i < dataBytes.Length; i++)
            {
                encrypted[i] = (byte)(dataBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return encrypted;
        }

        public static string DecryptData(byte[] encryptedData)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(EncryptionKey);
            byte[] decrypted = new byte[encryptedData.Length];

            for (int i = 0; i < encryptedData.Length; i++)
            {
                decrypted[i] = (byte)(encryptedData[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Encoding.UTF8.GetString(decrypted);
        }
    }
} 