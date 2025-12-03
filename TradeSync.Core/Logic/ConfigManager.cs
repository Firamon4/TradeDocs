using System.Text.Json;
using TradeSync.Core.Models;

namespace TradeSync.Core.Logic 
{
    public class ConfigManager
    {
        private readonly string _configPath;

        public ConfigManager()
        {
            string configName = "appsettings.json";
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;

            // Логіка пошуку файлу (пріоритет: поточна папка -> папки розробки)
            string[] paths = new[] {
                Path.Combine(currentDir, configName),
                Path.GetFullPath(Path.Combine(currentDir, @"..\..\..\..\TradeSync.Service\bin\Debug\net8.0", configName)),
                Path.GetFullPath(Path.Combine(currentDir, @"..\..\..\..\TradeSync.Service\appsettings.json"))
            };

            _configPath = paths.FirstOrDefault(File.Exists) ?? Path.Combine(currentDir, configName);
        }

        public async Task<ServiceConfig> LoadAsync()
        {
            if (!File.Exists(_configPath)) return new ServiceConfig();
            try
            {
                string json = await File.ReadAllTextAsync(_configPath);
                var config = JsonSerializer.Deserialize<ServiceConfig>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                }) ?? new ServiceConfig();

                // Дешифрування при читанні
                config.ConnectionStrings.Source1C = SecurityHelper.Unprotect(config.ConnectionStrings.Source1C);
                config.ConnectionStrings.AuxDb = SecurityHelper.Unprotect(config.ConnectionStrings.AuxDb);

                return config;
            }
            catch { return new ServiceConfig(); }
        }

        public async Task SaveAsync(ServiceConfig config)
        {
            // Створюємо копію для збереження, щоб зашифрувати
            var configToSave = new ServiceConfig
            {
                Logging = config.Logging,
                SyncSettings = config.SyncSettings,
                ConnectionStrings = new ConnectionStrings
                {
                    Source1C = SecurityHelper.Protect(config.ConnectionStrings.Source1C),
                    AuxDb = SecurityHelper.Protect(config.ConnectionStrings.AuxDb)
                }
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(configToSave, options);
            await File.WriteAllTextAsync(_configPath, json);
        }

        public string GetConfigPath() => _configPath;
    }
}