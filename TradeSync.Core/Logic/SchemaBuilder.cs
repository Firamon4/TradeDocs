using System.Text;
using TradeSync.Core.Models;

namespace TradeSync.Core.Logic
{
    public class SchemaBuilder
    {
        public string GenerateCreateScript(TableSchema table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"IF OBJECT_ID('dbo.[{table.SQLTable}]', 'U') IS NULL");
            sb.AppendLine("BEGIN");
            sb.AppendLine($"CREATE TABLE [dbo].[{table.SQLTable}] (");
            sb.AppendLine("    [_SyncId] BIGINT IDENTITY(1,1) PRIMARY KEY,");
            sb.AppendLine("    [_LastModified] DATETIME DEFAULT GETDATE(),");

            foreach (var col in table.Columns)
            {
                string sqlType = GetSqlType(col.Type, col.Sql);
                sb.AppendLine($"    [{col.Sql}] {sqlType},");
            }

            sb.Length -= 3;
            sb.AppendLine();
            sb.AppendLine(");");
            sb.AppendLine("END");
            return sb.ToString();
        }

        private string GetSqlType(string jsonType, string fieldName)
        {
            return jsonType switch
            {
                "Guid" => "UNIQUEIDENTIFIER",
                "Boolean" => "BIT",
                "Decimal" => "DECIMAL(19, 4)",
                "Int" => "INT",
                "DateTime" => "DATETIME",
                "Binary" => fieldName == "_Version" ? "VARBINARY(8)" : "VARBINARY(MAX)",
                _ => "NVARCHAR(MAX)" // String default
            };
        }
    }
}