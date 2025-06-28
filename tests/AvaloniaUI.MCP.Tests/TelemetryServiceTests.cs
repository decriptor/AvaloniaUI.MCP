using AvaloniaUI.MCP.Services;

using Microsoft.Extensions.Logging;

namespace AvaloniaUI.MCP.Tests;

[TestClass]
public class TelemetryServiceTests : IDisposable
{
    private TelemetryService _telemetryService = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        var logger = new TestLogger<TelemetryService>();
        _telemetryService = new TelemetryService(logger);
    }

    [TestMethod]
    public void RecordToolExecution_Should_UpdateMetrics()
    {
        // Arrange
        string toolName = "TestTool";
        var duration = TimeSpan.FromMilliseconds(100);

        // Act
        _telemetryService.RecordToolExecution(toolName, true, duration);

        // Assert
        Dictionary<string, object> metrics = _telemetryService.GetMetricsSnapshot();
        Assert.AreEqual(1L, metrics["total_tool_executions"], "Total tool executions should be 1");
        Assert.AreEqual(1L, metrics["successful_tool_executions"], "Successful tool executions should be 1");
        Assert.AreEqual(1.0, (double)metrics["tool_success_rate"], "Tool success rate should be 1.0");
    }

    [TestMethod]
    public void RecordToolExecution_WithFailure_Should_UpdateErrorMetrics()
    {
        // Arrange
        string toolName = "TestTool";
        var duration = TimeSpan.FromMilliseconds(50);
        string errorMessage = "Test error";

        // Act
        _telemetryService.RecordToolExecution(toolName, false, duration, errorMessage);

        // Assert
        Dictionary<string, object> metrics = _telemetryService.GetMetricsSnapshot();
        Assert.AreEqual(1L, metrics["total_tool_executions"], "Total tool executions should be 1");
        Assert.AreEqual(0L, metrics["successful_tool_executions"], "Successful tool executions should be 0");
        Assert.AreEqual(0.0, (double)metrics["tool_success_rate"], "Tool success rate should be 0.0");
    }

    [TestMethod]
    public void RecordResourceAccess_Should_UpdateCacheMetrics()
    {
        // Arrange
        string resourceName = "TestResource";

        // Act
        _telemetryService.RecordResourceAccess(resourceName, true);
        _telemetryService.RecordResourceAccess(resourceName, false);

        // Assert
        Dictionary<string, object> metrics = _telemetryService.GetMetricsSnapshot();
        Assert.AreEqual(2L, metrics["total_resource_accesses"], "Total resource accesses should be 2");
        Assert.AreEqual(1L, metrics["cache_hits"], "Cache hits should be 1");
        Assert.AreEqual(0.5, (double)metrics["cache_hit_rate"], "Cache hit rate should be 0.5");
    }

    [TestMethod]
    public void RecordValidation_Should_UpdateValidationMetrics()
    {
        // Arrange
        string validationType = "InputValidation";

        // Act
        _telemetryService.RecordValidation(validationType, true);

        // Assert
        Dictionary<string, object> metrics = _telemetryService.GetMetricsSnapshot();
        Assert.AreEqual(1L, metrics["total_validations"], "Total validations should be 1");
        Assert.AreEqual(1L, metrics["successful_validations"], "Successful validations should be 1");
        Assert.AreEqual(1.0, (double)metrics["validation_success_rate"], "Validation success rate should be 1.0");
    }

    [TestMethod]
    public void RecordValidation_WithFailure_Should_UpdateErrorMetrics()
    {
        // Arrange
        string validationType = "InputValidation";
        string errorDetails = "Invalid input format";

        // Act
        _telemetryService.RecordValidation(validationType, false, errorDetails);

        // Assert
        Dictionary<string, object> metrics = _telemetryService.GetMetricsSnapshot();
        Assert.AreEqual(1L, metrics["total_validations"], "Total validations should be 1");
        Assert.AreEqual(0L, metrics["successful_validations"], "Successful validations should be 0");
        Assert.AreEqual(0.0, (double)metrics["validation_success_rate"], "Validation success rate should be 0.0");
    }

    [TestMethod]
    public void StartActivity_Should_CreateActivityOrNull()
    {
        // Arrange
        string activityName = "TestActivity";

        // Act
        using System.Diagnostics.Activity? activity = _telemetryService.StartActivity(activityName);

        // Assert
        // Activity can be null if no listeners are registered, which is expected in unit tests
        if (activity != null)
        {
            Assert.AreEqual(activityName, activity.OperationName, "Activity operation name should match");
        }
        // This test passes as long as no exception is thrown
        Assert.IsTrue(true, "Test should pass without throwing exceptions");
    }

    [TestMethod]
    public void GetMetricsSnapshot_Should_ReturnCurrentMetrics()
    {
        // Arrange & Act
        Dictionary<string, object> metrics = _telemetryService.GetMetricsSnapshot();

        // Assert
        Assert.IsNotNull(metrics, "Metrics should not be null");
        CollectionAssert.Contains(metrics.Keys, "total_tool_executions", "Should contain total_tool_executions key");
        CollectionAssert.Contains(metrics.Keys, "successful_tool_executions", "Should contain successful_tool_executions key");
        CollectionAssert.Contains(metrics.Keys, "tool_success_rate", "Should contain tool_success_rate key");
        CollectionAssert.Contains(metrics.Keys, "total_resource_accesses", "Should contain total_resource_accesses key");
        CollectionAssert.Contains(metrics.Keys, "cache_hits", "Should contain cache_hits key");
        CollectionAssert.Contains(metrics.Keys, "cache_hit_rate", "Should contain cache_hit_rate key");
        CollectionAssert.Contains(metrics.Keys, "total_validations", "Should contain total_validations key");
        CollectionAssert.Contains(metrics.Keys, "successful_validations", "Should contain successful_validations key");
        CollectionAssert.Contains(metrics.Keys, "validation_success_rate", "Should contain validation_success_rate key");
        CollectionAssert.Contains(metrics.Keys, "snapshot_timestamp", "Should contain snapshot_timestamp key");
    }

    [TestMethod]
    public void RecordServerEvent_Should_NotThrow()
    {
        // Arrange
        string eventType = "startup";
        var properties = new Dictionary<string, object>
        {
            ["version"] = "1.0.0",
            ["environment"] = "test"
        };

        // Act & Assert
        try
        {
            _telemetryService.RecordServerEvent(eventType, properties);
            Assert.IsTrue(true, "Method should not throw an exception");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Method should not throw an exception, but threw: {ex.Message}");
        }
    }

    [TestMethod]
    public void CreateToolExecutionScope_Should_RecordMetricsOnDispose()
    {
        // Arrange
        string toolName = "ScopeTestTool";

        // Act
        using (IDisposable scope = _telemetryService.CreateToolExecutionScope(toolName))
        {
            // Simulate some work
            Thread.Sleep(10);
        }

        // Assert
        Dictionary<string, object> metrics = _telemetryService.GetMetricsSnapshot();
        Assert.AreEqual(1L, metrics["total_tool_executions"], "Total tool executions should be 1");
        Assert.AreEqual(1L, metrics["successful_tool_executions"], "Successful tool executions should be 1");
    }

    [TestMethod]
    public void CreateValidationScope_Should_RecordMetricsOnDispose()
    {
        // Arrange
        string validationType = "ScopeValidation";

        // Act
        using (IDisposable scope = _telemetryService.CreateValidationScope(validationType))
        {
            // Validation logic would go here
        }

        // Assert
        Dictionary<string, object> metrics = _telemetryService.GetMetricsSnapshot();
        Assert.AreEqual(1L, metrics["total_validations"], "Total validations should be 1");
        Assert.AreEqual(1L, metrics["successful_validations"], "Successful validations should be 1");
    }

    [TestMethod]
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
        Dictionary<string, object> metrics = _telemetryService.GetMetricsSnapshot();

        // Tool execution metrics
        Assert.AreEqual(3L, metrics["total_tool_executions"], "Total tool executions should be 3");
        Assert.AreEqual(2L, metrics["successful_tool_executions"], "Successful tool executions should be 2");
        Assert.AreEqual(2.0 / 3.0, (double)metrics["tool_success_rate"], 0.001, "Tool success rate should be 2/3");

        // Resource access metrics
        Assert.AreEqual(3L, metrics["total_resource_accesses"], "Total resource accesses should be 3");
        Assert.AreEqual(2L, metrics["cache_hits"], "Cache hits should be 2");
        Assert.AreEqual(2.0 / 3.0, (double)metrics["cache_hit_rate"], 0.001, "Cache hit rate should be 2/3");

        // Validation metrics
        Assert.AreEqual(2L, metrics["total_validations"], "Total validations should be 2");
        Assert.AreEqual(1L, metrics["successful_validations"], "Successful validations should be 1");
        Assert.AreEqual(0.5, (double)metrics["validation_success_rate"], "Validation success rate should be 0.5");
    }

    [TestMethod]
    public void MetricsSnapshot_Should_IncludeTimestamp()
    {
        // Act
        Dictionary<string, object> metrics = _telemetryService.GetMetricsSnapshot();

        // Assert
        CollectionAssert.Contains(metrics.Keys, "snapshot_timestamp", "Should contain snapshot_timestamp key");
        var timestamp = (DateTimeOffset)metrics["snapshot_timestamp"];
        Assert.IsTrue(timestamp <= DateTimeOffset.UtcNow, "Timestamp should be less than or equal to current time");
        Assert.IsTrue(timestamp > DateTimeOffset.UtcNow.AddMinutes(-1), "Timestamp should be within the last minute");
    }

    public void Dispose()
    {
        // No cleanup needed for this test class
    }
}

// Test logger implementation for testing
public class TestLogger<T> : ILogger<T>
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return new TestScope();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        // Store log messages if needed for testing
    }

    private sealed class TestScope : IDisposable
    {
        public void Dispose() { }
    }
}
