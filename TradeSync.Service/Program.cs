using TradeSync.Service;
using Serilog;
using Serilog.Events;

// 1. Конфігурація Serilog
Log.Logger = new LoggerConfiguration()
    // Файл 1: ЗАГАЛЬНИЙ (Info, Warning) - БЕЗ помилок
    .WriteTo.Logger(l => l
        .Filter.ByIncludingOnly(e => e.Level < LogEventLevel.Error) // Тільки те, що НЕ помилка
        .WriteTo.File(
            path: Path.Combine(AppContext.BaseDirectory, "logs", "service-.log"),
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}"
        ))
    // Файл 2: ПОМИЛКИ (Error, Fatal) - Тільки треш
    .WriteTo.Logger(l => l
        .Filter.ByIncludingOnly(e => e.Level >= LogEventLevel.Error) // Тільки помилки
        .WriteTo.File(
            path: Path.Combine(AppContext.BaseDirectory, "logs", "errors-.log"),
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30, // Зберігаємо місяць
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        ))
    .CreateLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddSerilog(); // Підключаємо наш налаштований логер

    builder.Services.AddWindowsService(options =>
    {
        options.ServiceName = "TradeSyncService";
    });

    builder.Services.AddHostedService<Worker>();

    var host = builder.Build();
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Сервіс впав при старті (Critical Fail)!");
}
finally
{
    Log.CloseAndFlush();
}