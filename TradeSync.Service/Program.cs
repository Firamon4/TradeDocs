using TradeSync.Service;
using Serilog;
using Serilog.Events;

// Íàëàøòóâàííÿ: Äâà îêðåìèõ ôàéëè
Log.Logger = new LoggerConfiguration()
    // 1. ÇÀÃÀËÜÍÈÉ (Ò³ëüêè Info/Warning, áåç ïîìèëîê)
    .WriteTo.Logger(l => l
        .Filter.ByIncludingOnly(e => e.Level < LogEventLevel.Error)
        .WriteTo.File(
            path: Path.Combine(AppContext.BaseDirectory, "logs", "service-.log"),
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7,
            outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}"
        ))
    // 2. ÏÎÌÈËÊÈ (Ò³ëüêè Error/Fatal)
    .WriteTo.Logger(l => l
        .Filter.ByIncludingOnly(e => e.Level >= LogEventLevel.Error)
        .WriteTo.File(
            path: Path.Combine(AppContext.BaseDirectory, "logs", "errors-.log"),
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        ))
    .CreateLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);
    builder.Services.AddSerilog();
    builder.Services.AddWindowsService(o => o.ServiceName = "TradeSyncService");
    builder.Services.AddHostedService<Worker>();
    var host = builder.Build();
    host.Run();
}
catch (Exception ex) { Log.Fatal(ex, "CRITICAL FAIL"); }
finally { Log.CloseAndFlush(); }