using System.Text;
using TradeSync.Core.Models;

namespace TradeSync.Core.Logic
{
    public class QueryBuilder
    {
        public string BuildSelectQuery(TableSchema table)
        {
            var sb = new StringBuilder();
            sb.Append("SELECT ");

            foreach (var field in table.Fields)
            {
                // Просто перераховуємо: _IDRRef, _Fld3844
                sb.Append($"{field.Value}, ");
            }

            sb.Length -= 2;
            sb.Append($" FROM {table.SqlTableNameSource}");

            // Тут можна додати (NOLOCK), щоб не блокувати 1С
            sb.Append(" WITH (NOLOCK)");

            return sb.ToString();
        }
    }
}