using System.Text;
using TradeSync.Core.Models;

namespace TradeSync.Desktop.Logic
{
    public class SqliteBuilder
    {
        // Перетворюємо "Справочник.Номенклатура" -> "Nomenclature" або "Ref_Nomenclature"
        // Для простоти візьмемо трансліт або просто замінимо крапки
        public string GetLocalTableName(string onECName)
        {
            return onECName.Replace(".", "_").Replace(" ", "");
        }

        public string GenerateCreateScript(TableSchema table)
        {
            string tableName = GetLocalTableName(table.Name1C);
            var sb = new StringBuilder();

            sb.AppendLine($"CREATE TABLE IF NOT EXISTS [{tableName}] (");

            // У SQLite ми будемо використовувати людські назви полів (Keys)
            foreach (var field in table.Fields)
            {
                string humanFieldName = field.Key; // "Ref", "Weight", "Name"
                string sqlType = "TEXT"; // SQLite любить TEXT, він універсальний

                // Проста евристика типів для SQLite
                if (humanFieldName.Contains("Сума") || humanFieldName.Contains("Ціна") || humanFieldName.Contains("Вага") || humanFieldName.Contains("Кількість"))
                    sqlType = "REAL";
                else if (humanFieldName == "DeletionMark" || humanFieldName == "IsFolder")
                    sqlType = "INTEGER"; // 0 або 1

                // PK
                if (humanFieldName == "Ref")
                    sb.AppendLine($"    [{humanFieldName}] TEXT PRIMARY KEY,");
                else
                    sb.AppendLine($"    [{humanFieldName}] {sqlType},");
            }

            // Прибираємо кому
            sb.Length -= 3;
            sb.AppendLine();
            sb.AppendLine(");");

            return sb.ToString();
        }
    }
}