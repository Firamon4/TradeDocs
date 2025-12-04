using System.Data;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using TradeSync.Core.Logic;
using TradeSync.Core.Models;
using TradeSync.Service.Logic; // <--- Додали посилання на новий клас

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
                try { RunSync(); }
                catch (Exception ex) { _logger.LogError(ex, "Критична помилка циклу"); }
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        private void RunSync()
        {
            var jsonPath = Path.Combine(AppContext.BaseDirectory, _configuration["SyncSettings:StructureFile"]);
            if (!File.Exists(jsonPath)) { _logger.LogError("Немає structure.json"); return; }

            var tables = JsonSerializer.Deserialize<List<TableSchema>>(File.ReadAllText(jsonPath));
            string connSrc = _configuration.GetConnectionString("Source1C");
            string connAux = _configuration.GetConnectionString("AuxDb");

            if (!TestConnection(connSrc)) { _logger.LogWarning("1С недоступна. Пропускаємо цикл."); return; }

            foreach (var table in tables)
            {
                _logger.LogInformation(">>> Обробка: {Table}", table.Name);
                try
                {
                    ProcessTable(connSrc, connAux, table);
                    _logger.LogInformation("<<< Успішно: {Table}", table.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ПОМИЛКА {Table}", table.Name);
                }
            }
        }

        // Новий метод, що об'єднує логіку для однієї таблиці
        private void ProcessTable(string connSrc, string connAux, TableSchema table)
        {
            // 1. Відкриваємо з'єднання з 1С і готуємо Reader (Streaming)
            // Важливо: Reader тримає з'єднання відкритим, доки ми читаємо
            using (var connectionSource = new SqlConnection(connSrc))
            {
                connectionSource.Open();
                var query = _queryBuilder.BuildSelectQuery(table);

                using (var cmdSource = new SqlCommand(query, connectionSource))
                {
                    cmdSource.CommandTimeout = 600; // 10 хв на старт

                    // Використовуємо ExecuteReader (потік), а не Load (пам'ять)
                    using (var reader = cmdSource.ExecuteReader())
                    {
                        // Огортаємо в наш конвертер
                        using (var convertingReader = new ConvertingDataReader(reader, table))
                        {
                            // 2. Відкриваємо транзакцію в Aux базі
                            using (var connectionAux = new SqlConnection(connAux))
                            {
                                connectionAux.Open();
                                using (var transaction = connectionAux.BeginTransaction())
                                {
                                    try
                                    {
                                        // А. Видаляємо стару і створюємо нову таблицю (в транзакції)
                                        SetupTargetTable(connectionAux, transaction, table);

                                        // Б. Заливаємо дані прямо з потоку (в транзакції)
                                        PushToTarget(connectionAux, transaction, table, convertingReader);

                                        // В. Якщо все ок - комітимо
                                        transaction.Commit();
                                    }
                                    catch
                                    {
                                        transaction.Rollback(); // Відміна змін, стара таблиця залишається живою
                                        throw;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetupTargetTable(SqlConnection conn, SqlTransaction trans, TableSchema table)
        {
            var script = _schemaBuilder.GenerateCreateScript(table);

            // Видаляємо стару (якщо транзакція відкотиться, видалення скасується)
            string drop = $"IF OBJECT_ID('dbo.[{table.SQLTable}]', 'U') IS NOT NULL DROP TABLE [dbo].[{table.SQLTable}]";

            using (var cmd = new SqlCommand(drop, conn, trans)) cmd.ExecuteNonQuery();
            using (var cmd = new SqlCommand(script, conn, trans)) cmd.ExecuteNonQuery();
        }

        private void PushToTarget(SqlConnection conn, SqlTransaction trans, TableSchema table, IDataReader reader)
        {
            // Передаємо транзакцію в BulkCopy
            using (var bulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, trans))
            {
                bulk.BulkCopyTimeout = 3600; // 1 година
                bulk.BatchSize = 5000;       // Пишемо пакетами по 5000 рядків (Batching)
                bulk.DestinationTableName = $"[{table.SQLTable}]";
                bulk.EnableStreaming = true; // Увімкнути стрімінг

                foreach (var col in table.Columns)
                    bulk.ColumnMappings.Add(col.Sql, col.Sql);

                try
                {
                    // Пишемо прямо з Reader-а
                    bulk.WriteToServer(reader);
                }
                catch (Exception ex)
                {
                    throw new Exception($"BulkCopy failed: {ex.Message}", ex);
                }
            }
        }

        private bool TestConnection(string s) { try { using var c = new SqlConnection(s); c.Open(); return true; } catch { return false; } }
    }
}