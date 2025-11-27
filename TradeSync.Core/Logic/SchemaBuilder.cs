using System.Text;
using TradeSync.Core.Models;

namespace TradeSync.Core.Logic
{
    public class SchemaBuilder
    {
        public string GenerateCreateScript(TableSchema table)
        {
            var sb = new StringBuilder();

            // Використовуємо технічне ім'я таблиці: _Reference283
            sb.AppendLine($"IF OBJECT_ID('dbo.[{table.SqlTableNameSource}]', 'U') IS NULL");
            sb.AppendLine("BEGIN");
            sb.AppendLine($"CREATE TABLE [dbo].[{table.SqlTableNameSource}] (");

            // Службові поля для синхронізації (опціонально, але корисно)
            sb.AppendLine("    [_SyncId] BIGINT IDENTITY(1,1) PRIMARY KEY,");

            foreach (var field in table.Fields)
            {
                // field.Value — це "_IDRRef", "_Fld3844"
                // field.Key — це "Ref", "Weight" (ми це ігноруємо тут)

                string sourceColName = field.Value;
                string sqlType = GetSqlTypeBySourceColumn(sourceColName); // Тип визначаємо так само

                sb.AppendLine($"    [{sourceColName}] {sqlType},");
            }

            sb.Length -= 3;
            sb.AppendLine();
            sb.AppendLine(");");
            sb.AppendLine("END");

            return sb.ToString();
        }

        // ... метод GetSqlTypeBySourceColumn залишається старим, 
        // єдине - типи даних. Бажано все ж таки Binary(16) конвертувати в GUID, 
        // інакше в Aux базі буде нечитабельний потік байтів.
    }
}