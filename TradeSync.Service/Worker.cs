using System.Data;
using Microsoft.Data.SqlClient; // Використовуй NuGet пакет Microsoft.Data.SqlClient
using System.Text.Json;
using TradeSync.Core.Logic;
using TradeSync.Core.Models;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace TradeSync.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly SchemaBuilder _schemaBuilder;
        private readonly QueryBuilder _queryBuilder;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _schemaBuilder = new SchemaBuilder();
            _queryBuilder = new QueryBuilder();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Початок циклу синхронізації: {time}", DateTimeOffset.Now);

                try
                {
                    RunSync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Критична помилка синхронізації");
                }

                // Чекаємо 10 хвилин перед наступним циклом (або налаштуй як треба)
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        private void RunSync()
        {
            // 1. Читаємо структуру
            var jsonPath = Path.Combine(AppContext.BaseDirectory, _configuration["SyncSettings:StructureFile"]);
            if (!File.Exists(jsonPath))
            {
                _logger.LogError("Файл структури не знайдено: {path}", jsonPath);
                return;
            }

            var jsonContent = File.ReadAllText(jsonPath);
            var tables = JsonSerializer.Deserialize<List<TableSchema>>(jsonContent);

            string connStringSource = _configuration.GetConnectionString("Source1C");
            string connStringAux = _configuration.GetConnectionString("AuxDb");

            if (!TestConnection(connStringSource))
            {
                _logger.LogWarning("Неможливо підключитися до 1С. Пропускаємо цикл синхронізації.");
                return; // Виходимо, не заходимо в цикл по таблицях
            }

            foreach (var table in tables)
            {
                _logger.LogInformation(">>> Початок обробки таблиці: {Table}", table.Name1C);

                try
                {
                    SetupTargetTable(connStringAux, table);
                    var dataTable = LoadFromSource(connStringSource, table);
                    PushToTarget(connStringAux, table, dataTable);

                    _logger.LogInformation("<<< Успішно завершено: {Table}. Записів: {Count}", table.Name1C, dataTable.Rows.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "КРИТИЧНА ПОМИЛКА при обробці {Table}", table.Name1C);
                    _logger.LogWarning("!!! Сталася помилка з таблицею {Table}. Деталі див. у файлі помилок (errors-*.log)", table.Name1C);
                }
            }
        }
        private bool TestConnection(string connStr)
        {
            try
            {
                using var c = new SqlConnection(connStr);
                c.Open();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка підключення до БД");
                return false;
            }
        }

        private void SetupTargetTable(string connString, TableSchema table)
        {
            var createScript = _schemaBuilder.GenerateCreateScript(table);

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                // Створення таблиці
                using (var cmd = new SqlCommand(createScript, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // ОЧИЩЕННЯ ТАБЛИЦІ ПЕРЕД ЗАВАНТАЖЕННЯМ (Повна синхронізація)
                // У майбутньому замінимо на MERGE/UPDATE logic
                using (var cmd = new SqlCommand($"TRUNCATE TABLE [{table.TargetTableName}]", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private DataTable LoadFromSource(string connString, TableSchema table)
        {
            var dt = new DataTable();
            var query = _queryBuilder.BuildSelectQuery(table);

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    // Створюємо колонки в DataTable з ТЕХНІЧНИМИ іменами
                    foreach (var field in table.Fields)
                    {
                        string colName = field.Value; // _IDRRef

                        // Типізація залишається, щоб дані були нормальними
                        Type colType = typeof(string);
                        if (colName.EndsWith("RRef") || colName == "_IDRRef") colType = typeof(Guid);
                        else if (colName == "_Marked") colType = typeof(bool);
                        else if (field.Key.Contains("Сума") || field.Key.Contains("Кількість")) colType = typeof(decimal); // Тут підглядаємо в HumanKey для типу, але ім'я колонки лишаємо технічним

                        dt.Columns.Add(colName, colType);
                    }

                    while (reader.Read())
                    {
                        var row = dt.NewRow();
                        foreach (var field in table.Fields)
                        {
                            string colName = field.Value;
                            var val = reader[colName];

                            // Конвертація типів
                            if (dt.Columns[colName].DataType == typeof(Guid))
                                row[colName] = DataHelper.ConvertToGuid(val);
                            else if (dt.Columns[colName].DataType == typeof(bool))
                                row[colName] = DataHelper.ConvertToBool(val);
                            else
                                row[colName] = val == DBNull.Value ? DBNull.Value : val;
                        }
                        dt.Rows.Add(row);
                    }
                }
            }
            return dt;
        }

        private void PushToTarget(string connString, TableSchema table, DataTable dt)
        {
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                using (var bulk = new SqlBulkCopy(conn))
                {
                    // Ім'я таблиці в Aux базі тепер теж технічне: _Reference283
                    bulk.DestinationTableName = $"[{table.SqlTableNameSource}]";

                    foreach (DataColumn col in dt.Columns)
                    {
                        // Mapping: _IDRRef -> _IDRRef
                        bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulk.WriteToServer(dt);
                }
            }
        }
    }
}