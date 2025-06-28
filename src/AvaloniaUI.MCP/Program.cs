using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using AvaloniaUI.MCP.Services;
using System.Diagnostics;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging with structured logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.IncludeScopes = true;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
});

// Set log levels based on environment
var logLevel = Environment.GetEnvironmentVariable("AVALONIA_MCP_LOG_LEVEL") switch
{
    "Trace" => LogLevel.Trace,
    "Debug" => LogLevel.Debug,
    "Information" => LogLevel.Information,
    "Warning" => LogLevel.Warning,
    "Error" => LogLevel.Error,
    "Critical" => LogLevel.Critical,
    _ => LogLevel.Information
};
builder.Logging.SetMinimumLevel(logLevel);

// Register telemetry service
builder.Services.AddSingleton<ITelemetryService, TelemetryService>();

// Configure MCP server with STDIO transport
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly()
    .WithResourcesFromAssembly()
    .WithPromptsFromAssembly();

var host = builder.Build();

// Initialize telemetry and logging
var telemetry = host.Services.GetRequiredService<ITelemetryService>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

// Record server startup
telemetry.RecordServerEvent("startup", new Dictionary<string, object>
{
    ["version"] = "1.0.0",
    ["environment"] = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "development",
    ["log_level"] = logLevel.ToString(),
    ["startup_time"] = DateTimeOffset.UtcNow
});

logger.LogInformation("AvaloniaUI MCP Server starting up - Version: 1.0.0, LogLevel: {LogLevel}", logLevel);

// Preload common resources into cache for better performance
try
{
    using var activity = telemetry.StartActivity("resource_cache_preload");
    var stopwatch = Stopwatch.StartNew();
    
    await ResourceCacheService.PreloadCommonResourcesAsync();
    
    stopwatch.Stop();
    logger.LogInformation("Resource cache preloaded successfully in {Duration}ms", stopwatch.ElapsedMilliseconds);
    
    telemetry.RecordServerEvent("cache_preload_success", new Dictionary<string, object>
    {
        ["duration_ms"] = stopwatch.ElapsedMilliseconds
    });
}
catch (Exception ex)
{
    logger.LogWarning(ex, "Failed to preload resource cache, will load on demand");
    telemetry.RecordServerEvent("cache_preload_failure", new Dictionary<string, object>
    {
        ["error"] = ex.Message
    });
}

// Set up graceful shutdown handling
var cancellationTokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    logger.LogInformation("Shutdown requested, stopping server gracefully...");
    telemetry.RecordServerEvent("shutdown_requested");
    cancellationTokenSource.Cancel();
};

try
{
    logger.LogInformation("AvaloniaUI MCP Server is ready to accept connections");
    telemetry.RecordServerEvent("server_ready");
    
    // Run the MCP server
    await host.RunAsync(cancellationTokenSource.Token);
}
catch (OperationCanceledException)
{
    logger.LogInformation("Server shutdown completed");
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Server encountered a fatal error");
    telemetry.RecordServerEvent("fatal_error", new Dictionary<string, object>
    {
        ["error"] = ex.Message,
        ["stack_trace"] = ex.StackTrace ?? string.Empty
    });
    throw;
}
finally
{
    telemetry.RecordServerEvent("shutdown", new Dictionary<string, object>
    {
        ["shutdown_time"] = DateTimeOffset.UtcNow,
        ["metrics"] = telemetry.GetMetricsSnapshot()
    });
    
    logger.LogInformation("AvaloniaUI MCP Server shutdown complete");
}
