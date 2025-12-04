using System.Data;
using Microsoft.Data.SqlClient; // Для Aux MSSQL
using System.Data.SQLite;       // Для Local SQLite
using System.Text.Json;
using TradeSync.Core.Models;

namespace TradeSync.Desktop.Logic
{
    public class SyncManager
    {
        private readonly string _auxConnectionString;
        private readonly string _sqliteConnectionString;
        private readonly string _structureFilePath;
        private readonly SqliteBuilder _sqliteBuilder;

        // Події для оновлення UI (прогрес бар)
        public event Action<string> OnLog;
        public event Action<int, int> OnProgress;

        public SyncManager(string auxConn, string sqlitePath, string jsonPath)
        {
            _auxConnectionString = auxConn;
            _sqliteConnectionString = $"Data Source={sqlitePath};Version=3;";
            _structureFilePath = jsonPath;
            _sqliteBuilder = new SqliteBuilder();
        }

        public async Task RunSyncAsync()
        {
            // 1. Читаємо JSON
            if (!File.Exists(_structureFilePath)) throw new FileNotFoundException("structure.json not found");
            var json = await File.ReadAllTextAsync(_structureFilePath);
            var tables = JsonSerializer.Deserialize<List<TableSchema>>(json);

            int totalTables = tables.Count;
            int currentTable = 0;

            using (var sqliteConn = new SQLiteConnection(_sqliteConnectionString))
            {
                sqliteConn.Open();

                foreach (var table in tables)
                {
                    currentTable++;
                    OnProgress?.Invoke(currentTable, totalTables);
                    OnLog?.Invoke($"Синхронізація таблиці: {table.Name}...");

                    // 2. Створюємо таблицю в SQLite (якщо немає)
                    var createSql = _sqliteBuilder.GenerateCreateScript(table);
                    using (var cmd = new SQLiteCommand(createSql, sqliteConn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // 3. Очищаємо локальну таблицю (Повна синхронізація)
                    string localTableName = _sqliteBuilder.GetLocalTableName(table.Name);
                    using (var cmd = new SQLiteCommand($"DELETE FROM [{localTableName}]", sqliteConn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // 4. Тягнемо дані з Aux MSSQL
                    await PullAndSaveData(table, sqliteConn, localTableName);
                }
            }
            OnLog?.Invoke("Синхронізацію завершено!");
        }

        private async Task PullAndSaveData(TableSchema table, SQLiteConnection sqliteConn, string localTableName)
        {
            // SELECT _Fld1, _Fld2 FROM _Reference...
            var selectColumns = string.Join(", ", table.Columns.Select(c => c.Sql));
            var query = $"SELECT {selectColumns} FROM [{table.SQLTable}]";

            using (var mssqlConn = new SqlConnection(_auxConnectionString))
            {
                await mssqlConn.OpenAsync();
                using (var cmd = new SqlCommand(query, mssqlConn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    using (var transaction = sqliteConn.BeginTransaction())
                    {
                        var humanCols = string.Join(", ", table.Columns.Select(c => $"[{c.Local}]"));
                        var paramNames = string.Join(", ", table.Columns.Select((c, i) => $"@p{i}"));
                        var insertSql = $"INSERT INTO [{localTableName}] ({humanCols}) VALUES ({paramNames})";
                        var insertCmd = new SQLiteCommand(insertSql, sqliteConn);

                        var paramsList = new List<SQLiteParameter>();
                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            var p = new SQLiteParameter($"@p{i}");
                            insertCmd.Parameters.Add(p);
                            paramsList.Add(p);
                        }

                        while (await reader.ReadAsync())
                        {
                            int idx = 0;
                            foreach (var col in table.Columns)
                            {
                                var val = reader[col.Sql];

                                // Конвертація для SQLite
                                if (val == DBNull.Value) paramsList[idx].Value = DBNull.Value;
                                else if (val is Guid g) paramsList[idx].Value = g.ToString();
                                else if (val is byte[] b) paramsList[idx].Value = Convert.ToBase64String(b); // Binary як текст
                                else paramsList[idx].Value = val;

                                idx++;
                            }
                            insertCmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                }
            }
        }
    }
}