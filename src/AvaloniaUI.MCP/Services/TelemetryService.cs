using System.Diagnostics;
using System.Diagnostics.Metrics;

using Microsoft.Extensions.Logging;

namespace AvaloniaUI.MCP.Services;

/// <summary>
/// Provides telemetry and metrics collection for the MCP server
/// </summary>
public interface ITelemetryService
{
    /// <summary>
    /// Records a tool execution event
    /// </summary>
    void RecordToolExecution(string toolName, bool success, TimeSpan duration, string? errorMessage = null);

    /// <summary>
    /// Records a resource access event
    /// </summary>
    void RecordResourceAccess(string resourceName, bool cacheHit);

    /// <summary>
    /// Records a validation event
    /// </summary>
    void RecordValidation(string validationType, bool success, string? errorDetails = null);

    /// <summary>
    /// Records server startup/shutdown events
    /// </summary>
    void RecordServerEvent(string eventType, Dictionary<string, object>? properties = null);

    /// <summary>
    /// Creates an activity for distributed tracing
    /// </summary>
    Activity? StartActivity(string activityName, string? parentId = null);

    /// <summary>
    /// Gets current metrics snapshot
    /// </summary>
    Dictionary<string, object> GetMetricsSnapshot();
}

public class TelemetryService : ITelemetryService, IDisposable
{
    readonly ILogger<TelemetryService> _logger;
    readonly Meter _meter;
    readonly ActivitySource _activitySource;

    // Counters
    readonly Counter<long> _toolExecutionCounter;
    readonly Counter<long> _resourceAccessCounter;
    readonly Counter<long> _validationCounter;
    readonly Counter<long> _errorCounter;

    // Histograms
    readonly Histogram<double> _toolExecutionDuration;

    // Tracking metrics
    long _totalToolExecutions;
    long _successfulToolExecutions;
    long _totalResourceAccesses;
    long _cacheHits;
    long _totalValidations;
    long _successfulValidations;
    readonly Lock _metricsLock = new();

    public TelemetryService(ILogger<TelemetryService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Initialize OpenTelemetry components
        _meter = new Meter("AvaloniaUI.MCP", "1.0.0");
        _activitySource = new ActivitySource("AvaloniaUI.MCP");

        // Initialize counters
        _toolExecutionCounter = _meter.CreateCounter<long>(
            "mcp_tool_executions_total",
            description: "Total number of tool executions");

        _resourceAccessCounter = _meter.CreateCounter<long>(
            "mcp_resource_accesses_total",
            description: "Total number of resource accesses");

        _validationCounter = _meter.CreateCounter<long>(
            "mcp_validations_total",
            description: "Total number of validations performed");

        _errorCounter = _meter.CreateCounter<long>(
            "mcp_errors_total",
            description: "Total number of errors encountered");

        // Initialize histograms
        _toolExecutionDuration = _meter.CreateHistogram<double>(
            "mcp_tool_execution_duration_ms",
            "ms",
            "Duration of tool executions in milliseconds");
    }

    public void RecordToolExecution(string toolName, bool success, TimeSpan duration, string? errorMessage = null)
    {
        lock (_metricsLock)
        {
            _totalToolExecutions++;
            if (success)
            {
                _successfulToolExecutions++;
            }
        }

        var tags = new KeyValuePair<string, object?>[]
        {
            new("tool_name", toolName),
            new("success", success)
        };

        _toolExecutionCounter.Add(1, tags);
        _toolExecutionDuration.Record(duration.TotalMilliseconds, tags);

        if (!success && !string.IsNullOrEmpty(errorMessage))
        {
            _errorCounter.Add(1,
            [
                new("tool_name", toolName),
                new("error_type", "tool_execution")
            ]);
        }

        _logger.LogInformation("Tool execution: {ToolName} - Success: {Success}, Duration: {Duration}ms",
            toolName, success, duration.TotalMilliseconds);

        if (!success && !string.IsNullOrEmpty(errorMessage))
        {
            _logger.LogWarning("Tool execution failed: {ToolName} - Error: {ErrorMessage}",
                toolName, errorMessage);
        }
    }

    public void RecordResourceAccess(string resourceName, bool cacheHit)
    {
        lock (_metricsLock)
        {
            _totalResourceAccesses++;
            if (cacheHit)
            {
                _cacheHits++;
            }
        }

        var tags = new KeyValuePair<string, object?>[]
        {
            new("resource_name", resourceName),
            new("cache_hit", cacheHit)
        };

        _resourceAccessCounter.Add(1, tags);

        _logger.LogDebug("Resource access: {ResourceName} - Cache hit: {CacheHit}",
            resourceName, cacheHit);
    }

    public void RecordValidation(string validationType, bool success, string? errorDetails = null)
    {
        lock (_metricsLock)
        {
            _totalValidations++;
            if (success)
            {
                _successfulValidations++;
            }
        }

        var tags = new KeyValuePair<string, object?>[]
        {
            new("validation_type", validationType),
            new("success", success)
        };

        _validationCounter.Add(1, tags);

        if (!success)
        {
            _errorCounter.Add(1,
            [
                new("validation_type", validationType),
                new("error_type", "validation")
            ]);
        }

        _logger.LogDebug("Validation: {ValidationType} - Success: {Success}",
            validationType, success);

        if (!success && !string.IsNullOrEmpty(errorDetails))
        {
            _logger.LogWarning("Validation failed: {ValidationType} - Details: {ErrorDetails}", validationType, errorDetails);
        }
    }

    public void RecordServerEvent(string eventType, Dictionary<string, object>? properties = null)
    {
        LogLevel logLevel = eventType.ToLowerInvariant() switch
        {
            "startup" => LogLevel.Information,
            "shutdown" => LogLevel.Information,
            "error" => LogLevel.Error,
            "warning" => LogLevel.Warning,
            _ => LogLevel.Debug
        };

        if (properties?.Count > 0)
        {
            _logger.Log(logLevel, "Server event: {EventType} - Properties: {@Properties}", eventType, properties);
        }
        else
        {
            _logger.Log(logLevel, "Server event: {EventType}", eventType);
        }
    }

    public Activity? StartActivity(string activityName, string? parentId = null)
    {
        Activity? activity = _activitySource.StartActivity(activityName);

        if (activity != null && !string.IsNullOrEmpty(parentId))
        {
            activity.SetParentId(parentId);
        }

        _logger.LogTrace("Started activity: {ActivityName} - TraceId: {TraceId}",
            activityName, activity?.TraceId);

        return activity;
    }

    public Dictionary<string, object> GetMetricsSnapshot()
    {
        lock (_metricsLock)
        {
            double cacheHitRate = _totalResourceAccesses > 0
                ? (double)_cacheHits / _totalResourceAccesses
                : 0.0;

            double toolSuccessRate = _totalToolExecutions > 0
                ? (double)_successfulToolExecutions / _totalToolExecutions
                : 0.0;

            double validationSuccessRate = _totalValidations > 0
                ? (double)_successfulValidations / _totalValidations
                : 0.0;

            return new Dictionary<string, object>
            {
                ["total_tool_executions"] = _totalToolExecutions,
                ["successful_tool_executions"] = _successfulToolExecutions,
                ["tool_success_rate"] = toolSuccessRate,
                ["total_resource_accesses"] = _totalResourceAccesses,
                ["cache_hits"] = _cacheHits,
                ["cache_hit_rate"] = cacheHitRate,
                ["total_validations"] = _totalValidations,
                ["successful_validations"] = _successfulValidations,
                ["validation_success_rate"] = validationSuccessRate,
                ["snapshot_timestamp"] = DateTimeOffset.UtcNow
            };
        }
    }

    public void Dispose()
    {
        _meter?.Dispose();
        _activitySource?.Dispose();
    }
}

/// <summary>
/// Extension methods for telemetry service integration
/// </summary>
public static class TelemetryExtensions
{
    /// <summary>
    /// Creates a telemetry scope that automatically records tool execution metrics
    /// </summary>
    public static IDisposable CreateToolExecutionScope(this ITelemetryService telemetry, string toolName)
    {
        return new ToolExecutionScope(telemetry, toolName);
    }

    /// <summary>
    /// Creates a telemetry scope that automatically records validation metrics
    /// </summary>
    public static IDisposable CreateValidationScope(this ITelemetryService telemetry, string validationType)
    {
        return new ValidationScope(telemetry, validationType);
    }
}

sealed class ToolExecutionScope(ITelemetryService telemetry, string toolName) : IDisposable
{
    readonly ITelemetryService _telemetry = telemetry;
    readonly string _toolName = toolName;
    readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    readonly Activity? _activity = telemetry.StartActivity($"tool_execution_{toolName}");
    bool _success = true;
    string? _errorMessage;

    public void MarkFailure(string errorMessage)
    {
        _success = false;
        _errorMessage = errorMessage;
    }

    public void Dispose()
    {
        _stopwatch.Stop();
        _telemetry.RecordToolExecution(_toolName, _success, _stopwatch.Elapsed, _errorMessage);
        _activity?.Dispose();
    }
}

sealed class ValidationScope(ITelemetryService telemetry, string validationType) : IDisposable
{
    readonly ITelemetryService _telemetry = telemetry;
    readonly string _validationType = validationType;
    bool _success = true;
    string? _errorDetails;

    public void MarkFailure(string errorDetails)
    {
        _success = false;
        _errorDetails = errorDetails;
    }

    public void Dispose()
    {
        _telemetry.RecordValidation(_validationType, _success, _errorDetails);
    }
}