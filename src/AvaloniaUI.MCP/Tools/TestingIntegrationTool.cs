using System.ComponentModel;

using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Tools;

[McpServerToolType]
public static class TestingIntegrationTool
{
    [McpServerTool, Description("Generates comprehensive unit tests for AvaloniaUI controls and ViewModels")]
    public static string GenerateUnitTests(
        [Description("Target class name to test")] string className,
        [Description("Test type: 'viewmodel', 'control', 'service', 'behavior'")] string testType,
        [Description("Include mock dependencies: 'true' or 'false'")] string includeMocks = "true",
        [Description("Test framework: 'xunit', 'nunit', 'mstest'")] string framework = "xunit")
    {
        try
        {
            var config = new TestConfiguration
            {
                ClassName = className,
                TestType = testType.ToLowerInvariant(),
                IncludeMocks = bool.Parse(includeMocks),
                Framework = framework.ToLowerInvariant()
            };

            string testCode = GenerateTestClass(config);
            string dependencies = GenerateTestDependencies(config);
            string setupInstructions = GenerateSetupInstructions(config);

            return $@"# Unit Tests for {className}

## Test Configuration
- **Target**: {config.ClassName}
- **Type**: {config.TestType}
- **Framework**: {config.Framework}
- **Mocking**: {config.IncludeMocks}

## Test Class Implementation
```csharp
{testCode}
```

## Required Dependencies
```xml
{dependencies}
```

## Setup Instructions
{setupInstructions}

## Testing Best Practices

### AvaloniaUI Testing Guidelines
- **UI Thread**: Use `Dispatcher.UIThread.InvokeAsync()` for UI operations
- **Headless Testing**: Configure headless mode for CI/CD pipelines
- **Property Testing**: Test all dependency properties thoroughly
- **Event Testing**: Verify all events are raised correctly

### Test Categories
- **Unit Tests**: Individual component testing
- **Integration Tests**: Component interaction testing
- **UI Tests**: End-to-end user interface testing
- **Performance Tests**: Memory and rendering performance

### Mocking Strategy
- **Services**: Mock external dependencies and services
- **ViewModels**: Test business logic independently
- **Controls**: Test behavior without visual rendering
- **Data Access**: Mock repositories and data providers";
        }
        catch (Exception ex)
        {
            return $"Error generating unit tests: {ex.Message}";
        }
    }

    [McpServerTool, Description("Creates UI automation tests for AvaloniaUI applications")]
    public static string GenerateUITests(
        [Description("Application or view name to test")] string targetName,
        [Description("Test scenarios (comma-separated, e.g., 'login,navigation,data-entry')")] string scenarios,
        [Description("Include accessibility tests: 'true' or 'false'")] string includeAccessibility = "true",
        [Description("Test runner: 'appium', 'selenium', 'avalonia-ui-tests'")] string testRunner = "avalonia-ui-tests")
    {
        try
        {
            var scenarioList = scenarios.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
            bool accessibility = bool.Parse(includeAccessibility);

            string testCode = GenerateUITestClass(targetName, scenarioList, accessibility, testRunner);
            string pageObjects = GeneratePageObjects(targetName, scenarioList);
            string helpers = GenerateTestHelpers(testRunner);

            return $@"# UI Automation Tests for {targetName}

## Test Configuration
- **Target**: {targetName}
- **Scenarios**: {string.Join(", ", scenarioList)}
- **Test Runner**: {testRunner}
- **Accessibility**: {accessibility}

## Main Test Class
```csharp
{testCode}
```

## Page Object Models
```csharp
{pageObjects}
```

## Test Helpers
```csharp
{helpers}
```

## Running the Tests

### Local Development
```bash
dotnet test --logger:console
```

### CI/CD Pipeline
```yaml
# GitHub Actions example
- name: Run UI Tests
  run: |
    export DISPLAY=:99
    Xvfb :99 -screen 0 1024x768x24 > /dev/null 2>&1 &
    dotnet test --configuration Release --logger:trx --results-directory ./TestResults
```

## Test Data Management
- **Test Fixtures**: Reusable test data sets
- **Data Builders**: Fluent test data creation
- **Cleanup**: Automatic test data cleanup
- **Isolation**: Each test runs independently

## Accessibility Testing
{(accessibility ? @"- **Screen Reader**: Test with screen reader simulation
- **Keyboard Navigation**: Verify full keyboard accessibility
- **Color Contrast**: Validate color contrast ratios
- **Focus Management**: Test focus order and visibility" : "// Accessibility testing disabled")}

## Performance Testing
- **Startup Time**: Measure application launch performance
- **Memory Usage**: Monitor memory consumption
- **Rendering Performance**: Test UI rendering efficiency
- **Response Time**: Measure user interaction responsiveness";
        }
        catch (Exception ex)
        {
            return $"Error generating UI tests: {ex.Message}";
        }
    }

    [McpServerTool, Description("Generates mock objects and test data builders for AvaloniaUI testing")]
    public static string GenerateMocksAndBuilders(
        [Description("Interface or class name to mock")] string targetInterface,
        [Description("Mock framework: 'moq', 'nsubstitute', 'fakeiteasy'")] string mockFramework = "moq",
        [Description("Generate test data builder: 'true' or 'false'")] string includeBuilder = "true",
        [Description("Include fluent assertions: 'true' or 'false'")] string includeAssertions = "true")
    {
        try
        {
            string mockCode = GenerateMockImplementation(targetInterface, mockFramework);
            string builderCode = bool.Parse(includeBuilder) ? GenerateTestDataBuilder(targetInterface) : "";
            string assertionExamples = bool.Parse(includeAssertions) ? GenerateFluentAssertions() : "";

            return $@"# Mocks and Test Builders for {targetInterface}

## Mock Framework: {mockFramework}

## Mock Implementation
```csharp
{mockCode}
```

{(bool.Parse(includeBuilder) ? $@"## Test Data Builder
```csharp
{builderCode}
```" : "")}

{(bool.Parse(includeAssertions) ? $@"## Fluent Assertions Examples
```csharp
{assertionExamples}
```" : "")}

## Usage Patterns

### Dependency Injection Testing
```csharp
// Arrange
var mockService = new Mock<IMyService>();
mockService.Setup(x => x.GetDataAsync()).ReturnsAsync(testData);

var container = new Container();
container.RegisterInstance(mockService.Object);

// Act & Assert
var viewModel = container.Resolve<MyViewModel>();
```

### Property Change Testing
```csharp
// Test INotifyPropertyChanged
var propertyChangedRaised = false;
viewModel.PropertyChanged += (s, e) =>
{{
    if (e.PropertyName == nameof(MyViewModel.MyProperty))
        propertyChangedRaised = true;
}};

viewModel.MyProperty = newValue;
Assert.True(propertyChangedRaised);
```

### Command Testing
```csharp
// Test ReactiveCommand
var command = viewModel.MyCommand;
Assert.True(command.CanExecute(null));

var executed = false;
command.Subscribe(_ => executed = true);
command.Execute(null);
Assert.True(executed);
```

## Test Verification Strategies
- **State Verification**: Check object state after operations
- **Behavior Verification**: Verify method calls and interactions
- **Event Verification**: Ensure events are raised correctly
- **Exception Testing**: Test error handling and edge cases";
        }
        catch (Exception ex)
        {
            return $"Error generating mocks and builders: {ex.Message}";
        }
    }

    [McpServerTool, Description("Creates performance and load tests for AvaloniaUI applications")]
    public static string GeneratePerformanceTests(
        [Description("Component name to test")] string componentName,
        [Description("Test type: 'memory', 'rendering', 'startup', 'load', 'stress'")] string performanceType,
        [Description("Include profiling setup: 'true' or 'false'")] string includeProfiling = "true",
        [Description("Target metrics (comma-separated, e.g., 'memory,cpu,render-time')")] string metrics = "memory,cpu,render-time")
    {
        try
        {
            var metricsList = metrics.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(m => m.Trim()).ToList();
            bool profiling = bool.Parse(includeProfiling);

            string testCode = GeneratePerformanceTestClass(componentName, performanceType, metricsList, profiling);
            string benchmarks = GenerateBenchmarkCode(componentName, performanceType);
            string profilingSetup = profiling ? GenerateProfilingSetup() : "";

            return $@"# Performance Tests for {componentName}

## Test Configuration
- **Component**: {componentName}
- **Type**: {performanceType}
- **Metrics**: {string.Join(", ", metricsList)}
- **Profiling**: {profiling}

## Performance Test Implementation
```csharp
{testCode}
```

## Benchmark Tests
```csharp
{benchmarks}
```

{(profiling ? $@"## Profiling Setup
```csharp
{profilingSetup}
```" : "")}

## Performance Targets

### Memory Usage
- **Startup Memory**: < 50MB for basic applications
- **Memory Growth**: < 1MB per operation cycle
- **Memory Leaks**: Zero detectable leaks
- **GC Pressure**: Minimize Gen 2 collections

### Rendering Performance
- **Frame Rate**: Maintain 60 FPS during animations
- **First Paint**: < 100ms from navigation
- **Layout Time**: < 16ms per layout pass
- **Draw Calls**: Minimize overdraw and batch operations

### Startup Performance
- **Cold Start**: < 2 seconds to first UI
- **Warm Start**: < 500ms to first UI
- **Assembly Loading**: Optimize with ReadyToRun
- **Initialization**: Lazy load non-critical components

## Monitoring and Alerts
```csharp
// Performance monitoring setup
public class PerformanceMonitor
{{
    private readonly ILogger _logger;
    private readonly MetricsCollector _metrics;

    public void MonitorOperation(string operationName, Action operation)
    {{
        var stopwatch = Stopwatch.StartNew();
        var startMemory = GC.GetTotalMemory(false);

        try
        {{
            operation();
        }}
        finally
        {{
            stopwatch.Stop();
            var endMemory = GC.GetTotalMemory(false);
            var memoryDelta = endMemory - startMemory;

            _metrics.RecordOperation(operationName, stopwatch.ElapsedMilliseconds, memoryDelta);

            if (stopwatch.ElapsedMilliseconds > 1000) // Alert if > 1 second
            {{
                _logger.LogWarning(""Slow operation detected: {{Operation}} took {{Duration}}ms"",
                    operationName, stopwatch.ElapsedMilliseconds);
            }}
        }}
    }}
}}
```

## Continuous Performance Testing
- **CI Integration**: Run performance tests in CI pipeline
- **Baseline Tracking**: Compare against performance baselines
- **Regression Detection**: Alert on performance degradation
- **Trend Analysis**: Track performance trends over time";
        }
        catch (Exception ex)
        {
            return $"Error generating performance tests: {ex.Message}";
        }
    }

    sealed class TestConfiguration
    {
        public string ClassName { get; set; } = "";
        public string TestType { get; set; } = "";
        public bool IncludeMocks { get; set; }
        public string Framework { get; set; } = "";
    }

    static string GenerateTestClass(TestConfiguration config)
    {
        string frameworkUsing = GetFrameworkUsings(config.Framework);
        string testAttribute = GetTestAttribute(config.Framework);
        string factAttribute = GetFactAttribute(config.Framework);

        return config.TestType switch
        {
            "viewmodel" => GenerateViewModelTests(config, frameworkUsing, testAttribute, factAttribute),
            "control" => GenerateControlTests(config, frameworkUsing, testAttribute, factAttribute),
            "service" => GenerateServiceTests(config, frameworkUsing, testAttribute, factAttribute),
            "behavior" => GenerateBehaviorTests(config, frameworkUsing, testAttribute, factAttribute),
            _ => throw new ArgumentException($"Unknown test type: {config.TestType}")
        };
    }

    static string GenerateViewModelTests(TestConfiguration config, string frameworkUsing, string testAttribute, string factAttribute)
    {
        string mockSetup = config.IncludeMocks ? GenerateViewModelMockSetup() : "";

        return $@"{frameworkUsing}
using Avalonia.Threading;
using ReactiveUI;
{(config.IncludeMocks ? "using Moq;" : "")}

namespace YourApp.Tests;

{testAttribute}
public class {config.ClassName}Tests
{{
{mockSetup}

    [{factAttribute}]
    public void Constructor_ShouldInitializeProperties()
    {{
        // Arrange & Act
        var viewModel = new {config.ClassName}();

        // Assert
        Assert.NotNull(viewModel);
        // Add specific property assertions
    }}

    [{factAttribute}]
    public async Task PropertyChanged_ShouldRaiseEvent()
    {{
        // Arrange
        var viewModel = new {config.ClassName}();
        var propertyChangedRaised = false;
        string? changedPropertyName = null;

        viewModel.PropertyChanged += (sender, args) =>
        {{
            propertyChangedRaised = true;
            changedPropertyName = args.PropertyName;
        }};

        // Act
        await Dispatcher.UIThread.InvokeAsync(() =>
        {{
            // Set a property that should raise PropertyChanged
            // viewModel.SomeProperty = ""new value"";
        }});

        // Assert
        Assert.True(propertyChangedRaised);
        Assert.Equal(nameof(viewModel.SomeProperty), changedPropertyName);
    }}

    [{factAttribute}]
    public void Command_CanExecute_ShouldReturnExpectedValue()
    {{
        // Arrange
        var viewModel = new {config.ClassName}();

        // Act
        var canExecute = viewModel.SomeCommand?.CanExecute(null);

        // Assert
        Assert.True(canExecute);
    }}

    [{factAttribute}]
    public async Task Command_Execute_ShouldPerformExpectedAction()
    {{
        // Arrange
        var viewModel = new {config.ClassName}();
        var commandExecuted = false;

        if (viewModel.SomeCommand != null)
        {{
            viewModel.SomeCommand.Subscribe(_ => commandExecuted = true);

            // Act
            await viewModel.SomeCommand.Execute();

            // Assert
            Assert.True(commandExecuted);
        }}
    }}

    [{factAttribute}]
    public void Validation_ShouldReturnExpectedErrors()
    {{
        // Arrange
        var viewModel = new {config.ClassName}();

        // Act
        viewModel.SomeProperty = ""invalid value"";
        var errors = viewModel.GetErrors(nameof(viewModel.SomeProperty));

        // Assert
        Assert.NotEmpty(errors);
    }}
}}";
    }

    static string GenerateControlTests(TestConfiguration config, string frameworkUsing, string testAttribute, string factAttribute)
    {
        return $@"{frameworkUsing}
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Headless;

namespace YourApp.Tests;

{testAttribute}
public class {config.ClassName}Tests
{{
    public {config.ClassName}Tests()
    {{
        // Initialize Avalonia for headless testing
        AppBuilder.Configure<App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions
            {{
                UseHeadlessDrawing = false
            }})
            .SetupWithoutStarting();
    }}

    [{factAttribute}]
    public async Task Control_ShouldInitializeCorrectly()
    {{
        // Arrange & Act
        var control = await Dispatcher.UIThread.InvokeAsync(() => new {config.ClassName}());

        // Assert
        Assert.NotNull(control);
        Assert.Equal(typeof({config.ClassName}), control.GetType());
    }}

    [{factAttribute}]
    public async Task Property_ShouldUpdateUI()
    {{
        // Arrange
        var control = await Dispatcher.UIThread.InvokeAsync(() => new {config.ClassName}());

        // Act
        await Dispatcher.UIThread.InvokeAsync(() =>
        {{
            // Set property and trigger layout
            control.Width = 200;
            control.Height = 100;
            control.Measure(Size.Infinity);
            control.Arrange(new Rect(0, 0, 200, 100));
        }});

        // Assert
        Assert.Equal(200, control.Bounds.Width);
        Assert.Equal(100, control.Bounds.Height);
    }}

    [{factAttribute}]
    public async Task Event_ShouldRaiseWhenExpected()
    {{
        // Arrange
        var control = await Dispatcher.UIThread.InvokeAsync(() => new {config.ClassName}());
        var eventRaised = false;

        await Dispatcher.UIThread.InvokeAsync(() =>
        {{
            control.SomeEvent += (sender, args) => eventRaised = true;

            // Act - trigger the event
            // control.TriggerSomeAction();
        }});

        // Assert
        Assert.True(eventRaised);
    }}

    [{factAttribute}]
    public async Task DataBinding_ShouldUpdateControl()
    {{
        // Arrange
        var control = await Dispatcher.UIThread.InvokeAsync(() => new {config.ClassName}());
        var viewModel = new TestViewModel {{ TestProperty = ""Initial Value"" }};

        await Dispatcher.UIThread.InvokeAsync(() =>
        {{
            control.DataContext = viewModel;

            // Act
            viewModel.TestProperty = ""Updated Value"";
        }});

        // Assert
        // Verify the control updated appropriately
        // Assert.Equal(""Updated Value"", control.SomeDisplayProperty);
    }}
}}

public class TestViewModel : ReactiveObject
{{
    private string _testProperty = """";
    public string TestProperty
    {{
        get => _testProperty;
        set => this.RaiseAndSetIfChanged(ref _testProperty, value);
    }}
}}";
    }

    static string GenerateServiceTests(TestConfiguration config, string frameworkUsing, string testAttribute, string factAttribute)
    {
        string mockSetup = config.IncludeMocks ? GenerateServiceMockSetup() : "";

        return $@"{frameworkUsing}
{(config.IncludeMocks ? "using Moq;" : "")}

namespace YourApp.Tests;

{testAttribute}
public class {config.ClassName}Tests
{{
{mockSetup}

    [{factAttribute}]
    public void Service_ShouldInitializeCorrectly()
    {{
        // Arrange & Act
        var service = new {config.ClassName}();

        // Assert
        Assert.NotNull(service);
    }}

    [{factAttribute}]
    public async Task Method_ShouldReturnExpectedResult()
    {{
        // Arrange
        var service = new {config.ClassName}();

        // Act
        var result = await service.SomeMethodAsync();

        // Assert
        Assert.NotNull(result);
        // Add specific assertions based on expected behavior
    }}

    [{factAttribute}]
    public async Task Method_WithInvalidInput_ShouldThrowException()
    {{
        // Arrange
        var service = new {config.ClassName}();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.SomeMethodAsync(null));
    }}
}}";
    }

    static string GenerateBehaviorTests(TestConfiguration config, string frameworkUsing, string testAttribute, string factAttribute)
    {
        return $@"{frameworkUsing}
using Avalonia.Controls;
using Avalonia.Threading;

namespace YourApp.Tests;

{testAttribute}
public class {config.ClassName}Tests
{{
    [{factAttribute}]
    public async Task Behavior_ShouldAttachToControl()
    {{
        // Arrange
        var control = await Dispatcher.UIThread.InvokeAsync(() => new Button());
        var behavior = new {config.ClassName}();

        // Act
        await Dispatcher.UIThread.InvokeAsync(() =>
        {{
            behavior.Attach(control);
        }});

        // Assert
        Assert.Equal(control, behavior.AssociatedObject);
    }}

    [{factAttribute}]
    public async Task Behavior_ShouldModifyControlBehavior()
    {{
        // Arrange
        var control = await Dispatcher.UIThread.InvokeAsync(() => new Button());
        var behavior = new {config.ClassName}();

        await Dispatcher.UIThread.InvokeAsync(() =>
        {{
            behavior.Attach(control);

            // Act - trigger behavior
            // Simulate user interaction or property change
        }});

        // Assert
        // Verify the behavior modified the control as expected
    }}
}}";
    }

    static string GetFrameworkUsings(string framework)
    {
        return framework switch
        {
            "xunit" => "using Xunit;",
            "nunit" => "using NUnit.Framework;",
            "mstest" => "using Microsoft.VisualStudio.TestTools.UnitTesting;",
            _ => "using Xunit;"
        };
    }

    static string GetTestAttribute(string framework)
    {
        return framework switch
        {
            "xunit" => "",
            "nunit" => "[TestFixture]",
            "mstest" => "[TestClass]",
            _ => ""
        };
    }

    static string GetFactAttribute(string framework)
    {
        return framework switch
        {
            "xunit" => "Fact",
            "nunit" => "Test",
            "mstest" => "TestMethod",
            _ => "Fact"
        };
    }

    static string GenerateViewModelMockSetup()
    {
        return @"    private readonly Mock<IMyService> _mockService;

    public YourViewModelTests()
    {
        _mockService = new Mock<IMyService>();
        // Setup default mock behavior
        _mockService.Setup(x => x.GetDataAsync())
                   .ReturnsAsync(new List<MyData>());
    }";
    }

    static string GenerateServiceMockSetup()
    {
        return @"    private readonly Mock<IRepository> _mockRepository;
    private readonly Mock<ILogger> _mockLogger;

    public YourServiceTests()
    {
        _mockRepository = new Mock<IRepository>();
        _mockLogger = new Mock<ILogger>();

        // Setup default mock behavior
        _mockRepository.Setup(x => x.SaveAsync(It.IsAny<MyEntity>()))
                      .Returns(Task.CompletedTask);
    }";
    }

    static string GenerateTestDependencies(TestConfiguration config)
    {
        var packages = new List<string>
        {
            GetTestFrameworkPackage(config.Framework),
            "<PackageReference Include=\"Avalonia.Headless\" Version=\"11.3.2\" />"
        };

        if (config.IncludeMocks)
        {
            packages.Add("<PackageReference Include=\"Moq\" Version=\"4.20.70\" />");
        }

        packages.Add("<PackageReference Include=\"FluentAssertions\" Version=\"6.12.0\" />");
        packages.Add("<PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"17.8.0\" />");

        return string.Join("\n", packages);
    }

    static string GetTestFrameworkPackage(string framework)
    {
        return framework switch
        {
            "xunit" => "<PackageReference Include=\"xunit\" Version=\"2.6.1\" />\n<PackageReference Include=\"xunit.runner.visualstudio\" Version=\"2.5.3\" />",
            "nunit" => "<PackageReference Include=\"NUnit\" Version=\"3.14.0\" />\n<PackageReference Include=\"NUnit3TestAdapter\" Version=\"4.5.0\" />",
            "mstest" => "<PackageReference Include=\"MSTest.TestFramework\" Version=\"3.1.1\" />\n<PackageReference Include=\"MSTest.TestAdapter\" Version=\"3.1.1\" />",
            _ => "<PackageReference Include=\"xunit\" Version=\"2.6.1\" />\n<PackageReference Include=\"xunit.runner.visualstudio\" Version=\"2.5.3\" />"
        };
    }

    static string GenerateSetupInstructions(TestConfiguration config)
    {
        return $@"### 1. Install Required Packages
Add the following packages to your test project:

```xml
<ItemGroup>
{GenerateTestDependencies(config)}
</ItemGroup>
```

### 2. Configure Headless Testing
For UI tests, ensure headless mode is properly configured:

```csharp
// In test constructor or setup
AppBuilder.Configure<App>()
    .UseHeadless(new AvaloniaHeadlessPlatformOptions
    {{
        UseHeadlessDrawing = false
    }})
    .SetupWithoutStarting();
```

### 3. Test Execution
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:""XPlat Code Coverage""

# Run specific test category
dotnet test --filter Category=Unit
```";
    }

    static string GenerateUITestClass(string targetName, List<string> scenarios, bool accessibility, string testRunner)
    {
        return $@"using Xunit;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Headless;

namespace YourApp.UITests;

public class {targetName}UITests : IDisposable
{{
    private readonly TestApplication _app;
    private readonly MainWindow _mainWindow;

    public {targetName}UITests()
    {{
        _app = new TestApplication();
        _mainWindow = new MainWindow();
    }}

    [Fact]
    public async Task Application_ShouldStartCorrectly()
    {{
        // Arrange & Act
        await Dispatcher.UIThread.InvokeAsync(() =>
        {{
            _mainWindow.Show();
        }});

        // Assert
        Assert.True(_mainWindow.IsVisible);
    }}

{string.Join("\n\n", scenarios.Select(GenerateScenarioTest))}

{(accessibility ? GenerateAccessibilityTests() : "")}

    public void Dispose()
    {{
        _mainWindow?.Close();
        _app?.Dispose();
    }}
}}

public class TestApplication : Application
{{
    public override void Initialize()
    {{
        AvaloniaXamlLoader.Load(this);
    }}
}}";
    }

    static string GenerateScenarioTest(string scenario)
    {
        return $@"    [Fact]
    public async Task {scenario.Replace("-", "_").Replace(" ", "_")}_ShouldWorkCorrectly()
    {{
        // Arrange
        await Dispatcher.UIThread.InvokeAsync(() =>
        {{
            // Setup test scenario for {scenario}
        }});

        // Act
        await Dispatcher.UIThread.InvokeAsync(() =>
        {{
            // Perform {scenario} actions
        }});

        // Assert
        // Verify {scenario} completed successfully
    }}";
    }

    static string GenerateAccessibilityTests()
    {
        return @"
    [Fact]
    public async Task KeyboardNavigation_ShouldWorkCorrectly()
    {
        // Test keyboard accessibility
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            // Simulate keyboard navigation
            // Verify focus management
        });
    }

    [Fact]
    public async Task ScreenReader_ShouldProvideCorrectLabels()
    {
        // Test screen reader accessibility
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            // Verify ARIA labels and descriptions
        });
    }";
    }

    static string GeneratePageObjects(string targetName, List<string> scenarios)
    {
        return $@"public class {targetName}PageObject
{{
    private readonly Control _rootControl;

    public {targetName}PageObject(Control rootControl)
    {{
        _rootControl = rootControl;
    }}

    public Button GetButton(string name) =>
        _rootControl.FindControl<Button>(name) ?? throw new InvalidOperationException($""Button {{name}} not found"");

    public TextBox GetTextBox(string name) =>
        _rootControl.FindControl<TextBox>(name) ?? throw new InvalidOperationException($""TextBox {{name}} not found"");

    public async Task ClickButtonAsync(string buttonName)
    {{
        await Dispatcher.UIThread.InvokeAsync(() =>
        {{
            var button = GetButton(buttonName);
            button.Command?.Execute(button.CommandParameter);
        }});
    }}

    public async Task EnterTextAsync(string textBoxName, string text)
    {{
        await Dispatcher.UIThread.InvokeAsync(() =>
        {{
            var textBox = GetTextBox(textBoxName);
            textBox.Text = text;
        }});
    }}

    public async Task<string> GetTextAsync(string controlName)
    {{
        return await Dispatcher.UIThread.InvokeAsync(() =>
        {{
            var control = _rootControl.FindControl<TextBlock>(controlName);
            return control?.Text ?? string.Empty;
        }});
    }}
}}";
    }

    static string GenerateTestHelpers(string testRunner)
    {
        return @"public static class TestHelpers
{
    public static async Task WaitForConditionAsync(Func<bool> condition, TimeSpan timeout = default)
    {
        timeout = timeout == default ? TimeSpan.FromSeconds(5) : timeout;
        var endTime = DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < endTime)
        {
            if (condition())
                return;

            await Task.Delay(50);
        }

        throw new TimeoutException(""Condition was not met within the specified timeout"");
    }

    public static async Task WaitForUIUpdateAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() => { });
        await Task.Delay(50); // Allow time for UI updates
    }

    public static T FindControl<T>(this Control parent, string name) where T : Control
    {
        if (parent.Name == name && parent is T control)
            return control;

        if (parent is Panel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is Control childControl)
                {
                    var found = FindControl<T>(childControl, name);
                    if (found != null)
                        return found;
                }
            }
        }

        return null;
    }
}";
    }

    static string GenerateMockImplementation(string targetInterface, string mockFramework)
    {
        return mockFramework.ToLowerInvariant() switch
        {
            "moq" => GenerateMoqImplementation(targetInterface),
            "nsubstitute" => GenerateNSubstituteImplementation(targetInterface),
            "fakeiteasy" => GenerateFakeItEasyImplementation(targetInterface),
            _ => GenerateMoqImplementation(targetInterface)
        };
    }

    static string GenerateMoqImplementation(string targetInterface)
    {
        return $@"using Moq;

public class {targetInterface}MockSetup
{{
    public static Mock<{targetInterface}> CreateMock()
    {{
        var mock = new Mock<{targetInterface}>();

        // Setup default behavior
        mock.Setup(x => x.MethodAsync(It.IsAny<string>()))
            .ReturnsAsync(""default result"");

        mock.Setup(x => x.Property)
            .Returns(""default property value"");

        return mock;
    }}

    public static Mock<{targetInterface}> CreateMockWithCustomBehavior()
    {{
        var mock = new Mock<{targetInterface}>();

        // Setup custom behavior
        mock.Setup(x => x.MethodAsync(""special input""))
            .ReturnsAsync(""special result"");

        mock.Setup(x => x.MethodAsync(It.Is<string>(s => s.StartsWith(""error""))))
            .ThrowsAsync(new InvalidOperationException(""Simulated error""));

        return mock;
    }}
}}";
    }

    static string GenerateNSubstituteImplementation(string targetInterface)
    {
        return $@"using NSubstitute;

public class {targetInterface}MockSetup
{{
    public static {targetInterface} CreateMock()
    {{
        var mock = Substitute.For<{targetInterface}>();

        // Setup default behavior
        mock.MethodAsync(Arg.Any<string>()).Returns(""default result"");
        mock.Property.Returns(""default property value"");

        return mock;
    }}

    public static {targetInterface} CreateMockWithCustomBehavior()
    {{
        var mock = Substitute.For<{targetInterface}>();

        // Setup custom behavior
        mock.MethodAsync(""special input"").Returns(""special result"");
        mock.MethodAsync(Arg.Is<string>(s => s.StartsWith(""error"")))
            .Returns(Task.FromException<string>(new InvalidOperationException(""Simulated error"")));

        return mock;
    }}
}}";
    }

    static string GenerateFakeItEasyImplementation(string targetInterface)
    {
        return $@"using FakeItEasy;

public class {targetInterface}MockSetup
{{
    public static {targetInterface} CreateMock()
    {{
        var mock = A.Fake<{targetInterface}>();

        // Setup default behavior
        A.CallTo(() => mock.MethodAsync(A<string>._)).Returns(""default result"");
        A.CallTo(() => mock.Property).Returns(""default property value"");

        return mock;
    }}

    public static {targetInterface} CreateMockWithCustomBehavior()
    {{
        var mock = A.Fake<{targetInterface}>();

        // Setup custom behavior
        A.CallTo(() => mock.MethodAsync(""special input"")).Returns(""special result"");
        A.CallTo(() => mock.MethodAsync(A<string>.That.StartsWith(""error"")))
            .Throws<InvalidOperationException>();

        return mock;
    }}
}}";
    }

    static string GenerateTestDataBuilder(string targetInterface)
    {
        string className = targetInterface.StartsWith('I') ? targetInterface[1..] : targetInterface;

        return $@"public class {className}TestDataBuilder
{{
    private string _id = Guid.NewGuid().ToString();
    private string _name = ""Test {className}"";
    private DateTime _createdDate = DateTime.UtcNow;
    private bool _isActive = true;

    public {className}TestDataBuilder WithId(string id)
    {{
        _id = id;
        return this;
    }}

    public {className}TestDataBuilder WithName(string name)
    {{
        _name = name;
        return this;
    }}

    public {className}TestDataBuilder WithCreatedDate(DateTime createdDate)
    {{
        _createdDate = createdDate;
        return this;
    }}

    public {className}TestDataBuilder WithInactiveStatus()
    {{
        _isActive = false;
        return this;
    }}

    public {className} Build()
    {{
        return new {className}
        {{
            Id = _id,
            Name = _name,
            CreatedDate = _createdDate,
            IsActive = _isActive
        }};
    }}

    public static implicit operator {className}({className}TestDataBuilder builder)
    {{
        return builder.Build();
    }}

    // Factory methods for common scenarios
    public static {className}TestDataBuilder Default() => new();

    public static {className}TestDataBuilder Invalid() => new {className}TestDataBuilder()
        .WithId(string.Empty)
        .WithName(string.Empty);

    public static {className}TestDataBuilder Expired() => new {className}TestDataBuilder()
        .WithCreatedDate(DateTime.UtcNow.AddYears(-1))
        .WithInactiveStatus();
}}";
    }

    static string GenerateFluentAssertions()
    {
        return @"using FluentAssertions;

// Object comparison
result.Should().NotBeNull();
result.Should().BeOfType<ExpectedType>();
result.Should().BeEquivalentTo(expectedObject);

// String assertions
text.Should().NotBeNullOrEmpty();
text.Should().StartWith(""expected prefix"");
text.Should().Contain(""expected substring"");

// Numeric assertions
number.Should().BePositive();
number.Should().BeInRange(1, 100);
number.Should().BeCloseTo(expectedValue, 0.01);

// Collection assertions
collection.Should().NotBeEmpty();
collection.Should().HaveCount(5);
collection.Should().Contain(expectedItem);
collection.Should().OnlyContain(x => x.IsValid);

// DateTime assertions
date.Should().BeAfter(DateTime.Today);
date.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));

// Exception assertions
Action act = () => methodThatThrows();
act.Should().Throw<ArgumentException>()
   .WithMessage(""*expected message*"");

// Async method assertions
Func<Task> asyncAct = async () => await methodThatThrowsAsync();
await asyncAct.Should().ThrowAsync<InvalidOperationException>();

// Event monitoring
using var monitor = subject.Monitor();
subject.RaiseEvent();
monitor.Should().Raise(nameof(subject.SomeEvent));

// Property change monitoring (for INotifyPropertyChanged)
using var monitor = viewModel.Monitor();
viewModel.Property = ""new value"";
monitor.Should().RaisePropertyChangeFor(x => x.Property);";
    }

    static string GeneratePerformanceTestClass(string componentName, string performanceType, List<string> metrics, bool profiling)
    {
        return $@"using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Diagnostics;

[MemoryDiagnoser]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80)]
public class {componentName}PerformanceTests
{{
    private {componentName} _component;
    private TestData _testData;

    [GlobalSetup]
    public void Setup()
    {{
        _component = new {componentName}();
        _testData = TestDataBuilder.CreateLargeDataSet(1000);
    }}

    [Benchmark]
    public void {performanceType}_Performance()
    {{
        // Benchmark the specific performance type
        switch (""{performanceType}"")
        {{
            case ""memory"":
                TestMemoryUsage();
                break;
            case ""rendering"":
                TestRenderingPerformance();
                break;
            case ""startup"":
                TestStartupPerformance();
                break;
            case ""load"":
                TestLoadPerformance();
                break;
            case ""stress"":
                TestStressPerformance();
                break;
        }}
    }}

    [Benchmark]
    [Arguments(100)]
    [Arguments(1000)]
    [Arguments(10000)]
    public void ScalabilityTest(int itemCount)
    {{
        var data = TestDataBuilder.CreateDataSet(itemCount);
        _component.ProcessData(data);
    }}

    private void TestMemoryUsage()
    {{
        var initialMemory = GC.GetTotalMemory(false);

        // Perform operation
        _component.LoadData(_testData);

        var finalMemory = GC.GetTotalMemory(false);
        var memoryUsed = finalMemory - initialMemory;

        // Memory usage should be reasonable
        Assert.True(memoryUsed < 10_000_000, $""Memory usage too high: {{memoryUsed}} bytes"");
    }}

    private void TestRenderingPerformance()
    {{
        var stopwatch = Stopwatch.StartNew();

        // Simulate rendering operations
        for (int i = 0; i < 60; i++) // 60 frames
        {{
            _component.Render();
        }}

        stopwatch.Stop();

        // Should maintain 60 FPS (16.67ms per frame)
        var averageFrameTime = stopwatch.ElapsedMilliseconds / 60.0;
        Assert.True(averageFrameTime < 16.67, $""Frame time too slow: {{averageFrameTime}}ms"");
    }}

    private void TestStartupPerformance()
    {{
        var stopwatch = Stopwatch.StartNew();

        var component = new {componentName}();
        component.Initialize();

        stopwatch.Stop();

        // Startup should be fast
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, $""Startup too slow: {{stopwatch.ElapsedMilliseconds}}ms"");
    }}

    private void TestLoadPerformance()
    {{
        var stopwatch = Stopwatch.StartNew();

        _component.LoadData(_testData);

        stopwatch.Stop();

        // Load operation should complete quickly
        Assert.True(stopwatch.ElapsedMilliseconds < 5000, $""Load too slow: {{stopwatch.ElapsedMilliseconds}}ms"");
    }}

    private void TestStressPerformance()
    {{
        // Perform stress testing
        for (int i = 0; i < 1000; i++)
        {{
            _component.ProcessItem(_testData.Items[i % _testData.Items.Count]);
        }}

        // Component should remain responsive
        Assert.True(_component.IsResponsive, ""Component became unresponsive under stress"");
    }}

    [GlobalCleanup]
    public void Cleanup()
    {{
        _component?.Dispose();
    }}
}}

// Run benchmarks
public class Program
{{
    public static void Main(string[] args)
    {{
        var summary = BenchmarkRunner.Run<{componentName}PerformanceTests>();
    }}
}}";
    }

    static string GenerateBenchmarkCode(string componentName, string performanceType)
    {
        return $@"[MemoryDiagnoser]
[ThreadingDiagnoser]
[ExceptionDiagnoser]
public class {componentName}Benchmarks
{{
    private {componentName} _component;

    [Params(100, 1000, 10000)]
    public int ItemCount {{ get; set; }}

    [GlobalSetup]
    public void Setup()
    {{
        _component = new {componentName}();
    }}

    [Benchmark(Baseline = true)]
    public void Baseline()
    {{
        // Baseline measurement
        var items = Enumerable.Range(0, ItemCount).ToList();
        foreach (var item in items)
        {{
            // Minimal processing
        }}
    }}

    [Benchmark]
    public void OptimizedVersion()
    {{
        // Test optimized implementation
        _component.ProcessItemsOptimized(ItemCount);
    }}

    [Benchmark]
    public async Task AsyncVersion()
    {{
        // Test async implementation
        await _component.ProcessItemsAsync(ItemCount);
    }}
}}";
    }

    static string GenerateProfilingSetup()
    {
        return @"using System.Diagnostics;
using Microsoft.Extensions.Logging;

public class ProfilingSetup
{
    private readonly ILogger _logger;

    public ProfilingSetup(ILogger logger)
    {
        _logger = logger;
    }

    public void EnableDotNetCounters()
    {
        // Enable .NET runtime counters
        var process = Process.GetCurrentProcess();
        _logger.LogInformation(""Process ID for monitoring: {ProcessId}"", process.Id);

        // Use dotnet-counters for monitoring:
        // dotnet-counters monitor --process-id {ProcessId} --counters System.Runtime,Microsoft.AspNetCore.Hosting
    }

    public void EnableEventPipeTracing()
    {
        // Enable EventPipe tracing for detailed performance analysis
        // Use dotnet-trace for collection:
        // dotnet-trace collect --process-id {ProcessId} --providers Microsoft-Windows-DotNETRuntime
    }

    public IDisposable StartPerfCounterCollection()
    {
        var perfCounters = new List<PerformanceCounter>
        {
            new(""Process"", ""% Processor Time"", Process.GetCurrentProcess().ProcessName),
            new(""Process"", ""Working Set"", Process.GetCurrentProcess().ProcessName),
            new("".NET CLR Memory"", ""# Total committed Bytes"", Process.GetCurrentProcess().ProcessName)
        };

        var timer = new Timer(state =>
        {
            foreach (var counter in perfCounters)
            {
                var value = counter.NextValue();
                _logger.LogInformation(""{CounterName}: {Value}"", counter.CounterName, value);
            }
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        return new DisposableAction(() =>
        {
            timer.Dispose();
            foreach (var counter in perfCounters)
                counter.Dispose();
        });
    }
}

public class DisposableAction : IDisposable
{
    private readonly Action _action;

    public DisposableAction(Action action)
    {
        _action = action;
    }

    public void Dispose() => _action();
}";
    }
}