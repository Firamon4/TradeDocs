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
                    OnLog?.Invoke($"Синхронізація таблиці: {table.Name1C}...");

                    // 2. Створюємо таблицю в SQLite (якщо немає)
                    var createSql = _sqliteBuilder.GenerateCreateScript(table);
                    using (var cmd = new SQLiteCommand(createSql, sqliteConn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // 3. Очищаємо локальну таблицю (Повна синхронізація)
                    string localTableName = _sqliteBuilder.GetLocalTableName(table.Name1C);
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
            // Формуємо SELECT до MSSQL (Технічні імена)
            var selectColumns = string.Join(", ", table.Fields.Values); // _IDRRef, _Fld3844
            var query = $"SELECT {selectColumns} FROM [{table.SqlTableNameSource}]";

            using (var mssqlConn = new SqlConnection(_auxConnectionString))
            {
                await mssqlConn.OpenAsync();
                using (var cmd = new SqlCommand(query, mssqlConn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    // Підготовка INSERT транзакції для SQLite (для швидкості)
                    using (var transaction = sqliteConn.BeginTransaction())
                    {
                        // Формуємо INSERT (Людські імена)
                        // INSERT INTO Ref_Nom (Ref, Weight) VALUES (@p0, @p1)
                        var humanCols = string.Join(", ", table.Fields.Keys.Select(k => $"[{k}]"));
                        var paramNames = string.Join(", ", table.Fields.Keys.Select((k, i) => $"@p{i}"));

                        var insertSql = $"INSERT INTO [{localTableName}] ({humanCols}) VALUES ({paramNames})";
                        var insertCmd = new SQLiteCommand(insertSql, sqliteConn);

                        // Додаємо параметри заглушки
                        var paramsList = new List<SQLiteParameter>();
                        for (int i = 0; i < table.Fields.Count; i++)
                        {
                            var p = new SQLiteParameter($"@p{i}");
                            insertCmd.Parameters.Add(p);
                            paramsList.Add(p);
                        }

                        while (await reader.ReadAsync())
                        {
                            int idx = 0;
                            foreach (var field in table.Fields)
                            {
                                // Читаємо з MSSQL по технічному імені (Value)
                                var val = reader[field.Value];

                                // Конвертуємо GUID -> String для SQLite
                                if (val is Guid g) val = g.ToString();
                                if (val is DBNull) val = DBNull.Value;

                                // Записуємо в параметр
                                paramsList[idx].Value = val;
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