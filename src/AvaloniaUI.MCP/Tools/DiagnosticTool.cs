using System.ComponentModel;
using System.Diagnostics;

using AvaloniaUI.MCP.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Tools;

/// <summary>
/// Provides diagnostic and health monitoring capabilities for the MCP server
/// </summary>
[McpServerToolType]
public static class DiagnosticTool
{
    [McpServerTool, Description("Gets current server metrics and performance statistics")]
    public static string GetServerMetrics()
    {
        try
        {
            // Try to get telemetry service from DI container if available
            IServiceProvider? serviceProvider = GetServiceProvider();
            ITelemetryService? telemetry = serviceProvider?.GetService<ITelemetryService>();

            if (telemetry == null)
            {
                return "# ⚠️ Server Metrics Unavailable\n\nTelemetry service is not configured or available.";
            }

            Dictionary<string, object> metrics = telemetry.GetMetricsSnapshot();

            return $@"# 📊 AvaloniaUI MCP Server Metrics

## Tool Execution Statistics
- **Total Executions**: {metrics.GetValueOrDefault("total_tool_executions", 0)}
- **Successful Executions**: {metrics.GetValueOrDefault("successful_tool_executions", 0)}
- **Success Rate**: {(double)metrics.GetValueOrDefault("tool_success_rate", 0.0):P2}

## Resource Access Statistics
- **Total Resource Accesses**: {metrics.GetValueOrDefault("total_resource_accesses", 0)}
- **Cache Hits**: {metrics.GetValueOrDefault("cache_hits", 0)}
- **Cache Hit Rate**: {(double)metrics.GetValueOrDefault("cache_hit_rate", 0.0):P2}

## Validation Statistics
- **Total Validations**: {metrics.GetValueOrDefault("total_validations", 0)}
- **Successful Validations**: {metrics.GetValueOrDefault("successful_validations", 0)}
- **Validation Success Rate**: {(double)metrics.GetValueOrDefault("validation_success_rate", 0.0):P2}

## System Information
- **Snapshot Time**: {metrics.GetValueOrDefault("snapshot_timestamp", DateTimeOffset.UtcNow)}
- **Server Uptime**: {DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime():hh\:mm\:ss}
- **Memory Usage**: {GC.GetTotalMemory(false) / 1024 / 1024:F2} MB
- **GC Collections**: Gen0: {GC.CollectionCount(0)}, Gen1: {GC.CollectionCount(1)}, Gen2: {GC.CollectionCount(2)}

## Performance Tips
- Monitor cache hit rate - should be >80% for optimal performance
- Watch tool success rate - investigate if below 95%
- Memory usage should remain stable over time";
        }
        catch (Exception ex)
        {
            return $"# ❌ Error Getting Server Metrics\n\nError: {ex.Message}";
        }
    }

    [McpServerTool, Description("Performs a comprehensive health check of the MCP server")]
    public static string PerformHealthCheck()
    {
        try
        {
            var healthStatus = new List<(string Component, bool IsHealthy, string Details)>();

            // Check telemetry service
            IServiceProvider? serviceProvider = GetServiceProvider();
            ITelemetryService? telemetry = serviceProvider?.GetService<ITelemetryService>();
            healthStatus.Add(("Telemetry Service", telemetry != null,
                telemetry != null ? "✅ Available" : "❌ Not configured"));

            // Check validation service (static class)
            bool validationAvailable = false;
            string validationDetails = "";
            try
            {
                ValidationResult testResult = InputValidationService.ValidateProjectName("TestProject");
                validationAvailable = true;
                validationDetails = "✅ Available (static)";
            }
            catch (Exception ex)
            {
                validationDetails = $"❌ Error: {ex.Message}";
            }
            healthStatus.Add(("Validation Service", validationAvailable, validationDetails));

            // Check error handling service (static class)
            bool errorHandlingAvailable = false;
            string errorHandlingDetails = "";
            try
            {
                string testResult = ErrorHandlingService.SafeExecute("test", () => "success");
                errorHandlingAvailable = !string.IsNullOrEmpty(testResult);
                errorHandlingDetails = "✅ Available (static)";
            }
            catch (Exception ex)
            {
                errorHandlingDetails = $"❌ Error: {ex.Message}";
            }
            healthStatus.Add(("Error Handling Service", errorHandlingAvailable, errorHandlingDetails));

            // Check resource cache
            bool cacheHealthy = false;
            string cacheDetails = "";
            try
            {
                // Test cache functionality by using existing methods
                ResourceCacheService.CacheProcessedResource("health_check_test", "test_value");
                string testResult = ResourceCacheService.GetOrLoadResource("health_check_test", () => "fallback");
                cacheHealthy = testResult == "test_value";
                cacheDetails = cacheHealthy ? "✅ Operational" : "⚠️ Cache not working as expected";

                // Clean up test entry
                ResourceCacheService.RemoveFromCache("health_check_test");
            }
            catch (Exception ex)
            {
                cacheDetails = $"❌ Error: {ex.Message}";
            }
            healthStatus.Add(("Resource Cache", cacheHealthy, cacheDetails));

            // Check memory pressure
            long totalMemory = GC.GetTotalMemory(false);
            bool memoryPressure = totalMemory > 500 * 1024 * 1024; // 500MB threshold
            healthStatus.Add(("Memory Usage", !memoryPressure,
                memoryPressure ? $"⚠️ High: {totalMemory / 1024 / 1024:F2} MB" : $"✅ Normal: {totalMemory / 1024 / 1024:F2} MB"));

            // Check file system access
            bool fileSystemHealthy = false;
            string fileSystemDetails = "";
            try
            {
                string tempPath = Path.GetTempFileName();
                File.WriteAllText(tempPath, "health check");
                File.Delete(tempPath);
                fileSystemHealthy = true;
                fileSystemDetails = "✅ Read/Write access available";
            }
            catch (Exception ex)
            {
                fileSystemDetails = $"❌ Error: {ex.Message}";
            }
            healthStatus.Add(("File System", fileSystemHealthy, fileSystemDetails));

            bool overallHealthy = healthStatus.All(h => h.IsHealthy);
            string healthIcon = overallHealthy ? "✅" : "⚠️";

            string result = $@"# {healthIcon} AvaloniaUI MCP Server Health Check

## Overall Status: {(overallHealthy ? "HEALTHY" : "DEGRADED")}

## Component Health
";

            foreach ((string component, bool isHealthy, string details) in healthStatus)
            {
                result += $"- **{component}**: {details}\n";
            }

            result += $@"

## Recommendations
{(overallHealthy ? "✅ All systems operational" : GenerateHealthRecommendations(healthStatus))}

**Checked at**: {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss UTC}";

            return result;
        }
        catch (Exception ex)
        {
            return $"# ❌ Health Check Failed\n\nError performing health check: {ex.Message}";
        }
    }

    [McpServerTool, Description("Tests logging functionality and telemetry recording")]
    public static string TestLogging(
        [Description("Log level to test: 'trace', 'debug', 'info', 'warn', 'error'")] string logLevel = "info",
        [Description("Custom message to log")] string message = "Test message from DiagnosticTool")
    {
        try
        {
            IServiceProvider? serviceProvider = GetServiceProvider();
            ILoggerFactory? loggerFactory = serviceProvider?.GetService<ILoggerFactory>();
            ILogger? logger = loggerFactory?.CreateLogger("DiagnosticTool");
            ITelemetryService? telemetry = serviceProvider?.GetService<ITelemetryService>();

            if (logger == null)
            {
                return "# ❌ Logging Test Failed\n\nLogger service not available";
            }

            // Test logging at specified level
            LogLevel level = logLevel.ToLowerInvariant() switch
            {
                "trace" => LogLevel.Trace,
                "debug" => LogLevel.Debug,
                "info" or "information" => LogLevel.Information,
                "warn" or "warning" => LogLevel.Warning,
                "error" => LogLevel.Error,
                _ => LogLevel.Information
            };

            string testMessage = $"Logging test - {message}";
            logger.Log(level, testMessage);

            // Test telemetry if available
            string telemetryResult = "";
            if (telemetry != null)
            {
                using IDisposable scope = telemetry.CreateToolExecutionScope("diagnostic_test");
                telemetry.RecordServerEvent("logging_test", new Dictionary<string, object>
                {
                    ["log_level"] = logLevel,
                    ["message"] = message,
                    ["timestamp"] = DateTimeOffset.UtcNow
                });
                telemetryResult = "\n- ✅ Telemetry recording successful";
            }
            else
            {
                telemetryResult = "\n- ⚠️ Telemetry service not available";
            }

            return $@"# ✅ Logging Test Completed

## Test Results
- ✅ Logger service available
- ✅ Log level '{logLevel}' tested successfully
- ✅ Message logged: ""{testMessage}""{telemetryResult}

**Test completed at**: {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss UTC}";
        }
        catch (Exception ex)
        {
            return $"# ❌ Logging Test Failed\n\nError: {ex.Message}";
        }
    }

    [McpServerTool, Description("Forces garbage collection and reports memory statistics")]
    public static string ForceGarbageCollection()
    {
        try
        {
            long beforeMemory = GC.GetTotalMemory(false);
            int beforeGen0 = GC.CollectionCount(0);
            int beforeGen1 = GC.CollectionCount(1);
            int beforeGen2 = GC.CollectionCount(2);

            // Force garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long afterMemory = GC.GetTotalMemory(false);
            int afterGen0 = GC.CollectionCount(0);
            int afterGen1 = GC.CollectionCount(1);
            int afterGen2 = GC.CollectionCount(2);

            long memoryFreed = beforeMemory - afterMemory;

            return $@"# 🧹 Garbage Collection Completed

## Memory Statistics
- **Before GC**: {beforeMemory / 1024.0 / 1024.0:F2} MB
- **After GC**: {afterMemory / 1024.0 / 1024.0:F2} MB
- **Memory Freed**: {memoryFreed / 1024.0 / 1024.0:F2} MB

## GC Collections
- **Generation 0**: {beforeGen0} → {afterGen0} (+{afterGen0 - beforeGen0})
- **Generation 1**: {beforeGen1} → {afterGen1} (+{afterGen1 - beforeGen1})
- **Generation 2**: {beforeGen2} → {afterGen2} (+{afterGen2 - beforeGen2})

**GC completed at**: {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss UTC}";
        }
        catch (Exception ex)
        {
            return $"# ❌ Garbage Collection Failed\n\nError: {ex.Message}";
        }
    }

    static IServiceProvider? GetServiceProvider()
    {
        // This is a simplified way to access DI - in a real implementation,
        // you might inject the service provider or use a different approach
        try
        {
            // Try to get from current activity context or other means
            return null; // Placeholder - would need proper DI integration
        }
        catch
        {
            return null;
        }
    }

    static string GenerateHealthRecommendations(List<(string Component, bool IsHealthy, string Details)> healthStatus)
    {
        var recommendations = new List<string>();

        foreach ((string component, bool isHealthy, string details) in healthStatus.Where(h => !h.IsHealthy))
        {
            string recommendation = component switch
            {
                "Telemetry Service" => "• Configure telemetry service in Program.cs for monitoring",
                "Validation Service" => "• Register validation service for input validation",
                "Error Handling Service" => "• Register error handling service for better error management",
                "Resource Cache" => "• Check resource cache configuration and file permissions",
                "Memory Usage" => "• Consider running garbage collection or reviewing memory usage patterns",
                "File System" => "• Check file system permissions and available disk space",
                _ => $"• Review {component} configuration"
            };
            recommendations.Add(recommendation);
        }

        return string.Join("\n", recommendations);
    }
}

// Extension method to help with metrics formatting
static class MetricsExtensions
{
    public static T GetValueOrDefault<T>(this Dictionary<string, object> dict, string key, T defaultValue)
    {
        return dict.TryGetValue(key, out object? value) && value is T typedValue ? typedValue : defaultValue;
    }
}