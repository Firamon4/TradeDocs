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

            // Службові поля для синхронізації
            sb.AppendLine("    [_SyncId] BIGINT IDENTITY(1,1) PRIMARY KEY,");
            sb.AppendLine("    [_LastModified] DATETIME DEFAULT GETDATE(),"); // Корисно додати дату вставки

            foreach (var field in table.Fields)
            {
                // field.Value — це "_IDRRef", "_Fld3844"
                string sourceColName = field.Value;

                // Передаємо також humanKey, щоб краще вгадати тип (наприклад для сум)
                string sqlType = GetSqlTypeBySourceColumn(sourceColName, field.Key);

                sb.AppendLine($"    [{sourceColName}] {sqlType},");
            }

            sb.Length -= 3; // Видаляємо останню кому і перенос
            sb.AppendLine();
            sb.AppendLine(");");
            sb.AppendLine("END");

            return sb.ToString();
        }

        private string GetSqlTypeBySourceColumn(string sourceName, string humanName)
        {
            // 1. Посилання (GUID)
            // _IDRRef або поля, що закінчуються на RRef (посилання на інші таблиці)
            if (sourceName.EndsWith("RRef", StringComparison.OrdinalIgnoreCase))
                return "UNIQUEIDENTIFIER";

            // 2. Версія (Timestamp 1C)
            if (sourceName.Equals("_Version", StringComparison.OrdinalIgnoreCase))
                return "VARBINARY(8)";

            // 3. Булево
            if (sourceName.Equals("_Marked", StringComparison.OrdinalIgnoreCase) ||
                sourceName.Equals("_Folder", StringComparison.OrdinalIgnoreCase))
                return "BIT";

            // 4. Складені типи (TYPE) - зазвичай це бінарні дані або ID типу
            if (sourceName.EndsWith("_TYPE", StringComparison.OrdinalIgnoreCase))
                return "VARBINARY(MAX)";

            // 5. Числа (Спроба вгадати по людській назві, бо 1С імена полів _Fld... нічого не кажуть)
            if (humanName.Contains("Сумма") || humanName.Contains("Цена") ||
                humanName.Contains("Количество") || humanName.Contains("Вес") ||
                humanName.Contains("Коефіцієнт") || humanName.Contains("Объем"))
            {
                return "DECIMAL(19, 4)";
            }

            // 6. Спеціальні поля 1С (LineNo)
            if (sourceName.StartsWith("_LineNo"))
                return "INT";

            // 7. Рядки та все інше
            return "NVARCHAR(MAX)";
        }
    }
}