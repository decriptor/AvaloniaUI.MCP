using Microsoft.Extensions.Logging;
using AvaloniaUI.MCP.Services;
using Xunit;

namespace AvaloniaUI.MCP.Tests;

public class TelemetryServiceTests
{
    private readonly TelemetryService _telemetryService;

    public TelemetryServiceTests()
    {
        var logger = new TestLogger<TelemetryService>();
        _telemetryService = new TelemetryService(logger);
    }

    [Fact]
    public void RecordToolExecution_Should_UpdateMetrics()
    {
        // Arrange
        var toolName = "TestTool";
        var duration = TimeSpan.FromMilliseconds(100);

        // Act
        _telemetryService.RecordToolExecution(toolName, true, duration);

        // Assert
        var metrics = _telemetryService.GetMetricsSnapshot();
        Assert.Equal(1L, metrics["total_tool_executions"]);
        Assert.Equal(1L, metrics["successful_tool_executions"]);
        Assert.Equal(1.0, (double)metrics["tool_success_rate"]);
    }

    [Fact]
    public void RecordToolExecution_WithFailure_Should_UpdateErrorMetrics()
    {
        // Arrange
        var toolName = "TestTool";
        var duration = TimeSpan.FromMilliseconds(50);
        var errorMessage = "Test error";

        // Act
        _telemetryService.RecordToolExecution(toolName, false, duration, errorMessage);

        // Assert
        var metrics = _telemetryService.GetMetricsSnapshot();
        Assert.Equal(1L, metrics["total_tool_executions"]);
        Assert.Equal(0L, metrics["successful_tool_executions"]);
        Assert.Equal(0.0, (double)metrics["tool_success_rate"]);
    }

    [Fact]
    public void RecordResourceAccess_Should_UpdateCacheMetrics()
    {
        // Arrange
        var resourceName = "TestResource";

        // Act
        _telemetryService.RecordResourceAccess(resourceName, true);
        _telemetryService.RecordResourceAccess(resourceName, false);

        // Assert
        var metrics = _telemetryService.GetMetricsSnapshot();
        Assert.Equal(2L, metrics["total_resource_accesses"]);
        Assert.Equal(1L, metrics["cache_hits"]);
        Assert.Equal(0.5, (double)metrics["cache_hit_rate"]);
    }

    [Fact]
    public void RecordValidation_Should_UpdateValidationMetrics()
    {
        // Arrange
        var validationType = "InputValidation";

        // Act
        _telemetryService.RecordValidation(validationType, true);

        // Assert
        var metrics = _telemetryService.GetMetricsSnapshot();
        Assert.Equal(1L, metrics["total_validations"]);
        Assert.Equal(1L, metrics["successful_validations"]);
        Assert.Equal(1.0, (double)metrics["validation_success_rate"]);
    }

    [Fact]
    public void RecordValidation_WithFailure_Should_UpdateErrorMetrics()
    {
        // Arrange
        var validationType = "InputValidation";
        var errorDetails = "Invalid input format";

        // Act
        _telemetryService.RecordValidation(validationType, false, errorDetails);

        // Assert
        var metrics = _telemetryService.GetMetricsSnapshot();
        Assert.Equal(1L, metrics["total_validations"]);
        Assert.Equal(0L, metrics["successful_validations"]);
        Assert.Equal(0.0, (double)metrics["validation_success_rate"]);
    }

    [Fact]
    public void StartActivity_Should_CreateActivityOrNull()
    {
        // Arrange
        var activityName = "TestActivity";

        // Act
        using var activity = _telemetryService.StartActivity(activityName);

        // Assert
        // Activity can be null if no listeners are registered, which is expected in unit tests
        if (activity != null)
        {
            Assert.Equal(activityName, activity.OperationName);
        }
        // This test passes as long as no exception is thrown
        Assert.True(true);
    }

    [Fact]
    public void GetMetricsSnapshot_Should_ReturnCurrentMetrics()
    {
        // Arrange & Act
        var metrics = _telemetryService.GetMetricsSnapshot();

        // Assert
        Assert.NotNull(metrics);
        Assert.Contains("total_tool_executions", metrics.Keys);
        Assert.Contains("successful_tool_executions", metrics.Keys);
        Assert.Contains("tool_success_rate", metrics.Keys);
        Assert.Contains("total_resource_accesses", metrics.Keys);
        Assert.Contains("cache_hits", metrics.Keys);
        Assert.Contains("cache_hit_rate", metrics.Keys);
        Assert.Contains("total_validations", metrics.Keys);
        Assert.Contains("successful_validations", metrics.Keys);
        Assert.Contains("validation_success_rate", metrics.Keys);
        Assert.Contains("snapshot_timestamp", metrics.Keys);
    }

    [Fact]
    public void RecordServerEvent_Should_NotThrow()
    {
        // Arrange
        var eventType = "startup";
        var properties = new Dictionary<string, object>
        {
            ["version"] = "1.0.0",
            ["environment"] = "test"
        };

        // Act & Assert
        var exception = Record.Exception(() => _telemetryService.RecordServerEvent(eventType, properties));
        Assert.Null(exception);
    }

    [Fact]
    public void CreateToolExecutionScope_Should_RecordMetricsOnDispose()
    {
        // Arrange
        var toolName = "ScopeTestTool";

        // Act
        using (var scope = _telemetryService.CreateToolExecutionScope(toolName))
        {
            // Simulate some work
            Thread.Sleep(10);
        }

        // Assert
        var metrics = _telemetryService.GetMetricsSnapshot();
        Assert.Equal(1L, metrics["total_tool_executions"]);
        Assert.Equal(1L, metrics["successful_tool_executions"]);
    }

    [Fact]
    public void CreateValidationScope_Should_RecordMetricsOnDispose()
    {
        // Arrange
        var validationType = "ScopeValidation";

        // Act
        using (var scope = _telemetryService.CreateValidationScope(validationType))
        {
            // Validation logic would go here
        }

        // Assert
        var metrics = _telemetryService.GetMetricsSnapshot();
        Assert.Equal(1L, metrics["total_validations"]);
        Assert.Equal(1L, metrics["successful_validations"]);
    }

    [Fact]
    public void MultipleOperations_Should_AccumulateMetrics()
    {
        // Arrange & Act
        _telemetryService.RecordToolExecution("Tool1", true, TimeSpan.FromMilliseconds(10));
        _telemetryService.RecordToolExecution("Tool2", false, TimeSpan.FromMilliseconds(20), "Error");
        _telemetryService.RecordToolExecution("Tool3", true, TimeSpan.FromMilliseconds(15));
        
        _telemetryService.RecordResourceAccess("Resource1", true);
        _telemetryService.RecordResourceAccess("Resource2", true);
        _telemetryService.RecordResourceAccess("Resource3", false);
        
        _telemetryService.RecordValidation("Validation1", true);
        _telemetryService.RecordValidation("Validation2", false, "Validation error");

        // Assert
        var metrics = _telemetryService.GetMetricsSnapshot();
        
        // Tool execution metrics
        Assert.Equal(3L, metrics["total_tool_executions"]);
        Assert.Equal(2L, metrics["successful_tool_executions"]);
        Assert.Equal(2.0/3.0, (double)metrics["tool_success_rate"], 3);
        
        // Resource access metrics
        Assert.Equal(3L, metrics["total_resource_accesses"]);
        Assert.Equal(2L, metrics["cache_hits"]);
        Assert.Equal(2.0/3.0, (double)metrics["cache_hit_rate"], 3);
        
        // Validation metrics
        Assert.Equal(2L, metrics["total_validations"]);
        Assert.Equal(1L, metrics["successful_validations"]);
        Assert.Equal(0.5, (double)metrics["validation_success_rate"]);
    }

    [Fact]
    public void MetricsSnapshot_Should_IncludeTimestamp()
    {
        // Act
        var metrics = _telemetryService.GetMetricsSnapshot();

        // Assert
        Assert.Contains("snapshot_timestamp", metrics.Keys);
        var timestamp = (DateTimeOffset)metrics["snapshot_timestamp"];
        Assert.True(timestamp <= DateTimeOffset.UtcNow);
        Assert.True(timestamp > DateTimeOffset.UtcNow.AddMinutes(-1));
    }
}

// Test logger implementation for testing
public class TestLogger<T> : ILogger<T>
{
    public IDisposable BeginScope<TState>(TState state) => new TestScope();
    public bool IsEnabled(LogLevel logLevel) => true;
    
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        // Store log messages if needed for testing
    }
    
    private class TestScope : IDisposable
    {
        public void Dispose() { }
    }
}