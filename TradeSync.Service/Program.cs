using TradeSync.Service;
using Serilog; // Додали Serilog

// Налаштування логера: писати у файл logs/log-.txt, новий файл щодня
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        path: Path.Combine(AppContext.BaseDirectory, "logs", "service-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7) // Зберігати за останній тиждень
    .CreateLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    // Підключаємо Serilog замість стандартного логера
    builder.Services.AddSerilog();

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
    // Якщо сервіс впав при старті - запишемо це
    Log.Fatal(ex, "Сервіс аварійно зупинився!");
}
finally
{
    Log.CloseAndFlush();
}