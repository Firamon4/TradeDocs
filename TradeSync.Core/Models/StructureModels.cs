using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TradeSync.Core.Models
{
    public class TableSchema
    {
        public string Name { get; set; }      // Назва 1С
        public string SQLTable { get; set; }  // Технічна назва

        public List<ColumnInfo> Columns { get; set; } = new();
    }

    public class ColumnInfo
    {
        public string Local { get; set; }   // Human name (SQLite)
        public string Sql { get; set; }     // SQL name (MSSQL)
        public string Type { get; set; }    // Тип: Guid, String, Boolean, Decimal, Binary, DateTime, Int
    }
}