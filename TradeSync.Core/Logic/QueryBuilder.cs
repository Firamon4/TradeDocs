using System.Text;
using TradeSync.Core.Models;

namespace TradeSync.Core.Logic
{
    public class QueryBuilder
    {
        public string BuildSelectQuery(TableSchema table)
        {
            var sb = new StringBuilder("SELECT ");
            foreach (var col in table.Columns)
            {
                sb.Append($"{col.Sql}, ");
            }
            sb.Length -= 2;
            sb.Append($" FROM {table.SQLTable} WITH (NOLOCK)");
            return sb.ToString();
        }
    }
}