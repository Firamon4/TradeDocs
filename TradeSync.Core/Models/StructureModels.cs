using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TradeSync.Core.Models
{
    // Головний клас опису таблиці
    public class TableSchema
    {
        [JsonPropertyName("Name")]
        public string Name1C { get; set; } // Напр: "Справочник.Номенклатура"

        [JsonPropertyName("SQLTable")]
        public string SqlTableNameSource { get; set; } // Напр: "_Reference283"

        [JsonPropertyName("Fields")]
        public Dictionary<string, string> Fields { get; set; } // Key="Ref", Value="_IDRRef"

        // Властивість для генерації імені таблиці в допоміжній базі
        // Перетворює "Справочник.Номенклатура" -> "Ref_Nomenclature" або просто трансліт,
        // але для простоти поки візьмемо очищену назву
        public string TargetTableName => Name1C.Replace(".", "_").Replace(" ", "");
    }
}