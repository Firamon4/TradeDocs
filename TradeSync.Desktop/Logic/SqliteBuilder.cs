using System.Text;
using TradeSync.Core.Models;

namespace TradeSync.Desktop.Logic
{
    public class SqliteBuilder
    {
        public string GetLocalTableName(string onECName) => onECName.Replace(".", "_").Replace(" ", "");

        public string GenerateCreateScript(TableSchema table)
        {
            string tableName = GetLocalTableName(table.Name);
            var sb = new StringBuilder();

            sb.AppendLine($"CREATE TABLE IF NOT EXISTS [{tableName}] (");

            foreach (var col in table.Columns)
            {
                string sqliteType = col.Type switch
                {
                    "Decimal" or "Int" => "REAL", // SQLite numeric
                    "Boolean" => "INTEGER",       // 0/1
                    _ => "TEXT"
                };

                if (col.Local == "Ref") sb.AppendLine($"    [{col.Local}] TEXT PRIMARY KEY,");
                else sb.AppendLine($"    [{col.Local}] {sqliteType},");
            }
            sb.Length -= 3;
            sb.AppendLine();
            sb.AppendLine(");");
            return sb.ToString();
        }
    }
}