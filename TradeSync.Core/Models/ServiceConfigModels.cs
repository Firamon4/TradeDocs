using System.Text.Json.Serialization;

namespace TradeSync.Core.Models
{
    // Кореневий об'єкт конфігу
    public class ServiceConfig
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new();
        public SyncSettings SyncSettings { get; set; } = new();
        public LoggingConfig Logging { get; set; } = new();
    }

    public class ConnectionStrings
    {
        public string Source1C { get; set; } = "";
        public string AuxDb { get; set; } = "";
    }

    public class SyncSettings
    {
        public string StructureFile { get; set; } = "structure.json";
    }

    public class LoggingConfig
    {
        public LogLevel LogLevel { get; set; } = new();
    }

    public class LogLevel
    {
        [JsonPropertyName("Default")]
        public string DefaultLevel { get; set; } = "Information";

        [JsonPropertyName("Microsoft.Hosting.Lifetime")]
        public string MicrosoftHosting { get; set; } = "Information";
    }
}