using System.Diagnostics;

using AvaloniaUI.MCP.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

sealed class Program
{
    static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        // Configure logging with structured logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        // Add Sentry logging with configuration
        builder.Logging.AddSentry(o =>
        {
            o.Dsn = "https://82c12a7f9520219b0fe9f91ac1d14b37@o4509369388761088.ingest.us.sentry.io/4509576978235392";
            o.Environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "development";
            o.TracesSampleRate = 0.1; // 10% of transactions for performance monitoring
            o.ProfilesSampleRate = 0.1; // 10% for profiling
            o.AutoSessionTracking = true;
            o.AttachStacktrace = true;
            o.SendDefaultPii = false; // Don't send personally identifiable information
            o.MaxBreadcrumbs = 100;
            o.Release = "avalonia-mcp@1.0.0";
        });

        // Set log levels based on environment
        LogLevel logLevel = Environment.GetEnvironmentVariable("AVALONIA_MCP_LOG_LEVEL") switch
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

        IHost host = builder.Build();

        // Initialize telemetry and logging
        ITelemetryService telemetry = host.Services.GetRequiredService<ITelemetryService>();
        ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();

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
            using Activity? activity = telemetry.StartActivity("resource_cache_preload");
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
    }
}