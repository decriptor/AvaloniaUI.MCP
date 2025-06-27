using System.ComponentModel;

using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Tools;

[McpServerToolType]
public static class DebuggingAssistantTool
{
    [McpServerTool, Description("Diagnoses common AvaloniaUI problems and provides solutions")]
    public static string DiagnoseCommonIssues(
        [Description("Issue category: 'binding', 'layout', 'styling', 'performance', 'startup', 'threading'")] string issueCategory,
        [Description("Problem description or error message")] string problemDescription,
        [Description("Include diagnostic code: 'true' or 'false'")] string includeDiagnosticCode = "true")
    {
        try
        {
            string category = issueCategory.ToLowerInvariant();
            string diagnostics = GenerateDiagnostics(category, problemDescription);
            string solutions = GenerateSolutions(category, problemDescription);
            string debugCode = bool.Parse(includeDiagnosticCode) ? GenerateDebugCode(category) : "";

            return $@"# AvaloniaUI Debugging Assistant: {issueCategory}

## Problem Analysis
**Category**: {category}
**Description**: {problemDescription}

## Diagnostic Information
{diagnostics}

## Recommended Solutions
{solutions}

{(bool.Parse(includeDiagnosticCode) ? $@"## Diagnostic Code
```csharp
{debugCode}
```" : "")}

## Additional Resources
- [AvaloniaUI Documentation](https://docs.avaloniaui.net/)
- [Common Issues Repository](https://github.com/AvaloniaUI/Avalonia/issues)
- [Community Forum](https://github.com/AvaloniaUI/Avalonia/discussions)";
        }
        catch (Exception ex)
        {
            return $"Error generating diagnostics: {ex.Message}";
        }
    }

    [McpServerTool, Description("Generates debug utilities and logging helpers for AvaloniaUI")]
    public static string GenerateDebugUtilities(
        [Description("Utility type: 'logger', 'visualtree', 'binding', 'performance', 'memory'")] string utilityType,
        [Description("Include DevTools integration: 'true' or 'false'")] string includeDevTools = "true",
        [Description("Include telemetry: 'true' or 'false'")] string includeTelemetry = "false")
    {
        try
        {
            string utility = utilityType.ToLowerInvariant();
            string utilityCode = GenerateUtilityCode(utility);
            string devToolsCode = bool.Parse(includeDevTools) ? GenerateDevToolsIntegration() : "";
            string telemetryCode = bool.Parse(includeTelemetry) ? GenerateTelemetryCode() : "";

            return $@"# Debug Utilities: {utilityType}

## Utility Implementation
```csharp
{utilityCode}
```

{(bool.Parse(includeDevTools) ? $@"## DevTools Integration
```csharp
{devToolsCode}
```" : "")}

{(bool.Parse(includeTelemetry) ? $@"## Telemetry Integration
```csharp
{telemetryCode}
```" : "")}

## Setup Instructions
1. Add the utility classes to your project
2. Register services in Program.cs or App.axaml.cs
3. Configure logging and debugging options
4. Use the utilities in your ViewModels and Views

## Usage Tips
- Enable DevTools in debug builds only
- Use structured logging for better analysis
- Monitor performance metrics in production
- Implement proper error boundaries";
        }
        catch (Exception ex)
        {
            return $"Error generating debug utilities: {ex.Message}";
        }
    }

    static string GenerateDiagnostics(string category, string problemDescription)
    {
        return category switch
        {
            "binding" => GenerateBindingDiagnostics(problemDescription),
            "layout" => GenerateLayoutDiagnostics(problemDescription),
            "styling" => GenerateStylingDiagnostics(problemDescription),
            "performance" => GeneratePerformanceDiagnostics(problemDescription),
            "startup" => GenerateStartupDiagnostics(problemDescription),
            "threading" => GenerateThreadingDiagnostics(problemDescription),
            _ => "Unknown issue category. Please specify: binding, layout, styling, performance, startup, or threading."
        };
    }

    static string GenerateBindingDiagnostics(string problem)
    {
        return @"### Data Binding Issues

**Common Causes:**
- Property not implementing INotifyPropertyChanged
- Incorrect binding path or typos
- DataContext not set properly
- Missing converter for type conversion
- Binding mode issues (OneWay vs TwoWay)

**Diagnostic Questions:**
1. Is the property raising PropertyChanged events?
2. Is the DataContext set correctly on the control?
3. Are there any binding errors in the debug output?
4. Is the property accessible (public with getter/setter)?
5. Are you using the correct binding syntax?

**Debug Output Check:**
Look for binding errors in the debug console:
- ""Cannot find property [PropertyName]""
- ""Failed to convert value""
- ""Binding path error""

**Immediate Actions:**
1. Enable binding error logging
2. Check DataContext hierarchy
3. Verify property names match exactly
4. Test with simple string properties first";
    }

    static string GenerateLayoutDiagnostics(string problem)
    {
        return @"### Layout Issues

**Common Causes:**
- Conflicting size constraints (Width/Height vs Min/Max)
- Improper panel usage (Grid vs StackPanel vs DockPanel)
- Margin and Padding conflicts
- Alignment issues (HorizontalAlignment, VerticalAlignment)
- Infinite size constraints in scrollable containers

**Diagnostic Questions:**
1. Are you using the appropriate panel for your layout needs?
2. Are there conflicting size constraints?
3. Is the parent container providing proper space?
4. Are margins and padding calculated correctly?
5. Is content clipping or overflowing?

**Layout Debugging:**
- Use DevTools to inspect the visual tree
- Check Bounds, DesiredSize, and RenderSize
- Verify measure and arrange passes
- Look for layout cycles or infinite loops

**Common Fixes:**
1. Set appropriate RowDefinitions/ColumnDefinitions for Grid
2. Use proper alignment properties
3. Check for circular dependencies in layout
4. Ensure containers have proper sizing constraints";
    }

    static string GenerateStylingDiagnostics(string problem)
    {
        return @"### Styling Issues

**Common Causes:**
- Selector specificity conflicts
- Style inheritance problems
- Resource not found errors
- Template binding issues
- Style not applied due to incorrect targeting

**Diagnostic Questions:**
1. Is the style selector correct and specific enough?
2. Are required resources (brushes, fonts) available?
3. Is the style in the correct resource dictionary scope?
4. Are there conflicting styles with higher specificity?
5. Is the target type specified correctly?

**Style Debugging:**
- Use DevTools to inspect applied styles
- Check resource resolution in visual tree
- Verify selector syntax and specificity
- Look for template binding errors

**Resolution Steps:**
1. Verify selector syntax (e.g., 'Button.primary')
2. Check resource dictionary inclusion
3. Ensure proper style inheritance chain
4. Test with simple styles first";
    }

    static string GeneratePerformanceDiagnostics(string problem)
    {
        return @"### Performance Issues

**Common Causes:**
- Inefficient data binding (not using compiled bindings)
- Layout thrashing (too many measure/arrange cycles)
- Memory leaks from event handlers or bindings
- Synchronous operations on UI thread
- Large collections without virtualization

**Performance Indicators:**
1. High CPU usage during UI operations
2. Memory usage growing over time
3. Slow rendering or frame drops
4. Unresponsive UI during operations
5. Long startup times

**Profiling Areas:**
- Memory allocation patterns
- Garbage collection frequency
- UI thread blocking operations
- Binding performance and updates
- Visual tree complexity

**Optimization Checklist:**
1. Use x:DataType for compiled bindings
2. Implement proper virtualization for lists
3. Avoid complex visual trees
4. Use async operations for I/O
5. Profile memory usage and leaks";
    }

    static string GenerateStartupDiagnostics(string problem)
    {
        return @"### Startup Issues

**Common Causes:**
- Missing required dependencies or assemblies
- Configuration errors in App.axaml or Program.cs
- Resource loading failures
- Service registration problems
- Platform-specific initialization issues

**Startup Failure Points:**
1. Assembly loading and resolution
2. Application initialization
3. Main window creation
4. Resource dictionary loading
5. Service container configuration

**Diagnostic Steps:**
1. Check application entry point configuration
2. Verify all required packages are referenced
3. Test with minimal application setup
4. Check for unhandled exceptions during startup
5. Validate platform-specific configurations

**Common Solutions:**
1. Ensure proper SDK and package versions
2. Check app.manifest and project settings
3. Verify resource paths and availability
4. Test dependency injection configuration
5. Check for platform compatibility issues";
    }

    static string GenerateThreadingDiagnostics(string problem)
    {
        return @"### Threading Issues

**Common Causes:**
- Accessing UI controls from background threads
- Dispatcher invoke errors or deadlocks
- Cross-thread operation exceptions
- Race conditions in data updates
- Blocking UI thread with synchronous operations

**Threading Violations:**
1. ""Cross-thread operation not valid""
2. UI updates from background threads
3. Deadlocks in async/await patterns
4. Dispatcher queue overflow
5. Thread starvation

**Resolution Approach:**
1. Use Dispatcher.UIThread.InvokeAsync for UI updates
2. Implement proper async/await patterns
3. Avoid sync-over-async antipatterns
4. Use ConfigureAwait(false) for non-UI operations
5. Consider using reactive patterns

**Threading Best Practices:**
- Keep UI thread free for rendering
- Use background threads for I/O operations
- Implement proper cancellation support
- Avoid blocking calls on UI thread
- Use thread-safe collections when needed";
    }

    static string GenerateSolutions(string category, string problemDescription)
    {
        return category switch
        {
            "binding" => GenerateBindingSolutions(),
            "layout" => GenerateLayoutSolutions(),
            "styling" => GenerateStylingStolutions(),
            "performance" => GeneratePerformanceSolutions(),
            "startup" => GenerateStartupSolutions(),
            "threading" => GenerateThreadingSolutions(),
            _ => "No specific solutions available for this category."
        };
    }

    static string GenerateBindingSolutions()
    {
        return @"### Binding Solutions

1. **Enable Binding Error Logging**
   ```csharp
   // In App.axaml.cs
   public override void Initialize()
   {
       AvaloniaXamlLoader.Load(this);

   #if DEBUG
       // Enable binding error logging
       this.AttachDevTools();
   #endif
   }
   ```

2. **Implement Proper ViewModelBase**
   ```csharp
   public class ViewModelBase : ReactiveObject
   {
       private string _title = string.Empty;

       public string Title
       {
           get => _title;
           set => this.RaiseAndSetIfChanged(ref _title, value);
       }
   }
   ```

3. **Use Compiled Bindings**
   ```xml
   <UserControl x:DataType=""vm:MyViewModel"">
       <TextBlock Text=""{Binding Title}"" />
   </UserControl>
   ```

4. **Debug Binding Context**
   ```xml
   <!-- Temporary debug binding to see DataContext -->
   <TextBlock Text=""{Binding GetType().Name}"" />
   ```";
    }

    static string GenerateLayoutSolutions()
    {
        return @"### Layout Solutions

1. **Use Appropriate Panels**
   ```xml
   <!-- For forms - use Grid -->
   <Grid RowDefinitions=""Auto,Auto,*"" ColumnDefinitions=""Auto,*"">
       <TextBlock Grid.Row=""0"" Grid.Column=""0"" Text=""Name:"" />
       <TextBox Grid.Row=""0"" Grid.Column=""1"" />
   </Grid>

   <!-- For lists - use StackPanel -->
   <StackPanel Orientation=""Vertical"">
       <Button Content=""Button 1"" />
       <Button Content=""Button 2"" />
   </StackPanel>
   ```

2. **Fix Size Constraints**
   ```xml
   <!-- Avoid conflicting constraints -->
   <Button Width=""100"" MinWidth=""50"" MaxWidth=""200"" />

   <!-- Use proper alignment -->
   <Button HorizontalAlignment=""Stretch""
           VerticalAlignment=""Center"" />
   ```

3. **Debug Layout with DevTools**
   ```csharp
   #if DEBUG
   this.AttachDevTools();
   #endif
   ```";
    }

    static string GenerateStylingStolutions()
    {
        return @"### Styling Solutions

1. **Proper Style Selectors**
   ```xml
   <!-- Target specific control type -->
   <Style Selector=""Button"">
       <Setter Property=""Margin"" Value=""4"" />
   </Style>

   <!-- Target with class -->
   <Style Selector=""Button.primary"">
       <Setter Property=""Background"" Value=""Blue"" />
   </Style>

   <!-- Target with state -->
   <Style Selector=""Button:pointerover"">
       <Setter Property=""Background"" Value=""LightBlue"" />
   </Style>
   ```

2. **Resource Dictionary Setup**
   ```xml
   <Application.Styles>
       <StyleInclude Source=""avares://YourApp/Styles/Buttons.axaml"" />
   </Application.Styles>
   ```

3. **Debug Style Issues**
   ```xml
   <!-- Test with inline styles first -->
   <Button Background=""Red"" Content=""Test"" />
   ```";
    }

    static string GeneratePerformanceSolutions()
    {
        return @"### Performance Solutions

1. **Use Compiled Bindings**
   ```xml
   <UserControl x:DataType=""vm:MyViewModel"">
       <TextBlock Text=""{Binding Title}"" />
   </UserControl>
   ```

2. **Implement Virtualization**
   ```xml
   <ListBox VirtualizationMode=""Simple"">
       <ListBox.ItemsPanel>
           <ItemsPanelTemplate>
               <VirtualizingStackPanel />
           </ItemsPanelTemplate>
       </ListBox.ItemsPanel>
   </ListBox>
   ```

3. **Async Operations**
   ```csharp
   public async Task LoadDataAsync()
   {
       IsLoading = true;
       try
       {
           var data = await _dataService.GetDataAsync();
           await Dispatcher.UIThread.InvokeAsync(() =>
           {
               Items.Clear();
               Items.AddRange(data);
           });
       }
       finally
       {
           IsLoading = false;
       }
   }
   ```

4. **Memory Leak Prevention**
   ```csharp
   // Dispose of subscriptions
   public void Dispose()
   {
       _subscription?.Dispose();
   }
   ```";
    }

    static string GenerateStartupSolutions()
    {
        return @"### Startup Solutions

1. **Proper App Configuration**
   ```csharp
   public static void Main(string[] args)
   {
       try
       {
           BuildAvaloniaApp()
               .StartWithClassicDesktopLifetime(args);
       }
       catch (Exception ex)
       {
           // Log startup errors
           Console.WriteLine($""Startup error: {ex}"");
           throw;
       }
   }

   public static AppBuilder BuildAvaloniaApp()
       => AppBuilder.Configure<App>()
           .UsePlatformDetect()
           .LogToTrace()
           .UseReactiveUI();
   ```

2. **Resource Loading**
   ```xml
   <Application.Styles>
       <FluentTheme />
       <StyleInclude Source=""avares://YourApp/App.axaml"" />
   </Application.Styles>
   ```

3. **Error Handling**
   ```csharp
   public override void OnFrameworkInitializationCompleted()
   {
       try
       {
           // Your initialization code
           base.OnFrameworkInitializationCompleted();
       }
       catch (Exception ex)
       {
           // Handle initialization errors
           ShowErrorDialog(ex);
       }
   }
   ```";
    }

    static string GenerateThreadingSolutions()
    {
        return @"### Threading Solutions

1. **UI Thread Updates**
   ```csharp
   public async Task UpdateUIAsync()
   {
       // Background work
       var data = await GetDataAsync();

       // UI updates
       await Dispatcher.UIThread.InvokeAsync(() =>
       {
           Items.Clear();
           Items.AddRange(data);
       });
   }
   ```

2. **Proper Async Patterns**
   ```csharp
   public async Task<string> LoadDataAsync()
   {
       // Use ConfigureAwait(false) for non-UI operations
       var result = await _httpClient.GetStringAsync(url)
           .ConfigureAwait(false);

       return result;
   }
   ```

3. **Reactive Commands**
   ```csharp
   public ReactiveCommand<Unit, Unit> LoadCommand { get; }

   public MyViewModel()
   {
       LoadCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
   }
   ```

4. **Cancellation Support**
   ```csharp
   private CancellationTokenSource _cancellationTokenSource = new();

   public async Task LoadDataAsync()
   {
       try
       {
           var data = await _service.GetDataAsync(_cancellationTokenSource.Token);
           // Update UI
       }
       catch (OperationCanceledException)
       {
           // Handle cancellation
       }
   }
   ```";
    }

    static string GenerateDebugCode(string category)
    {
        return category switch
        {
            "binding" => GenerateBindingDebugCode(),
            "layout" => GenerateLayoutDebugCode(),
            "styling" => GenerateStylingDebugCode(),
            "performance" => GeneratePerformanceDebugCode(),
            "startup" => GenerateStartupDebugCode(),
            "threading" => GenerateThreadingDebugCode(),
            _ => "// No specific debug code available"
        };
    }

    static string GenerateBindingDebugCode()
    {
        return @"// Binding Debug Helper
public static class BindingDebugHelper
{
    public static void EnableBindingErrorLogging()
    {
        // Enable binding error logging in debug builds
#if DEBUG
        Avalonia.Logging.Logger.Sink = new CustomLogSink();
#endif
    }

    public static void DebugDataContext(Control control)
    {
        Console.WriteLine($""Control: {control.GetType().Name}"");
        Console.WriteLine($""DataContext: {control.DataContext?.GetType().Name ?? ""null""}"");

        if (control.Parent != null)
        {
            Console.WriteLine($""Parent DataContext: {control.Parent.DataContext?.GetType().Name ?? ""null""}"");
        }
    }
}

public class CustomLogSink : Avalonia.Logging.ILogSink
{
    public bool IsEnabled(Avalonia.Logging.LogEventLevel level, string area)
    {
        return level >= Avalonia.Logging.LogEventLevel.Warning;
    }

    public void Log(Avalonia.Logging.LogEventLevel level, string area, object source, string messageTemplate, params object[] propertyValues)
    {
        if (area.Contains(""Binding""))
        {
            Console.WriteLine($""[BINDING] {messageTemplate}"", propertyValues);
        }
    }
}";
    }

    static string GenerateLayoutDebugCode()
    {
        return @"// Layout Debug Helper
public static class LayoutDebugHelper
{
    public static void PrintLayoutInfo(Control control)
    {
        Console.WriteLine($""Control: {control.GetType().Name}"");
        Console.WriteLine($""Bounds: {control.Bounds}"");
        Console.WriteLine($""DesiredSize: {control.DesiredSize}"");
        Console.WriteLine($""Margin: {control.Margin}"");
        Console.WriteLine($""Padding: {(control as Decorator)?.Padding ?? new Thickness()}"");
        Console.WriteLine($""HorizontalAlignment: {control.HorizontalAlignment}"");
        Console.WriteLine($""VerticalAlignment: {control.VerticalAlignment}"");
        Console.WriteLine(""---"");
    }

    public static void HighlightControl(Control control, IBrush brush)
    {
        if (control is Border border)
        {
            border.BorderBrush = brush;
            border.BorderThickness = new Thickness(2);
        }
        else
        {
            // Wrap in border for highlighting
            var parent = control.Parent as Panel;
            if (parent != null)
            {
                var index = parent.Children.IndexOf(control);
                parent.Children.RemoveAt(index);

                var highlightBorder = new Border
                {
                    BorderBrush = brush,
                    BorderThickness = new Thickness(2),
                    Child = control
                };

                parent.Children.Insert(index, highlightBorder);
            }
        }
    }
}";
    }

    static string GenerateStylingDebugCode()
    {
        return @"// Styling Debug Helper
public static class StylingDebugHelper
{
    public static void PrintAppliedStyles(StyledElement element)
    {
        Console.WriteLine($""Element: {element.GetType().Name}"");
        Console.WriteLine($""Classes: {string.Join("", "", element.Classes)}"");

        // This would require internal access to style system
        // In practice, use DevTools for style inspection
    }

    public static void TestResourceResolution(Control control, string resourceKey)
    {
        var resource = control.TryFindResource(resourceKey);
        Console.WriteLine($""Resource '{resourceKey}': {resource?.GetType().Name ?? ""Not Found""}"");
    }

    public static void AddDebugClass(StyledElement element, string className)
    {
        element.Classes.Add(className);
        Console.WriteLine($""Added debug class '{className}' to {element.GetType().Name}"");
    }
}";
    }

    static string GeneratePerformanceDebugCode()
    {
        return @"// Performance Debug Helper
public static class PerformanceDebugHelper
{
    private static readonly Dictionary<string, Stopwatch> _timers = new();

    public static void StartTimer(string operation)
    {
        _timers[operation] = Stopwatch.StartNew();
    }

    public static void StopTimer(string operation)
    {
        if (_timers.TryGetValue(operation, out var timer))
        {
            timer.Stop();
            Console.WriteLine($""{operation}: {timer.ElapsedMilliseconds}ms"");
            _timers.Remove(operation);
        }
    }

    public static void MeasureMemory(string operation)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var beforeMemory = GC.GetTotalMemory(false);
        Console.WriteLine($""{operation} - Memory before: {beforeMemory:N0} bytes"");

        // Store operation name for later measurement
        _memoryOperations[operation] = beforeMemory;
    }

    private static readonly Dictionary<string, long> _memoryOperations = new();

    public static void EndMeasureMemory(string operation)
    {
        if (_memoryOperations.TryGetValue(operation, out var beforeMemory))
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var afterMemory = GC.GetTotalMemory(false);
            var difference = afterMemory - beforeMemory;

            Console.WriteLine($""{operation} - Memory after: {afterMemory:N0} bytes"");
            Console.WriteLine($""{operation} - Memory difference: {difference:N0} bytes"");

            _memoryOperations.Remove(operation);
        }
    }
}";
    }

    static string GenerateStartupDebugCode()
    {
        return @"// Startup Debug Helper
public static class StartupDebugHelper
{
    public static void LogStartupStep(string step)
    {
        Console.WriteLine($""[STARTUP] {DateTime.Now:HH:mm:ss.fff} - {step}"");
    }

    public static void MeasureStartupTime(Action startupAction)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            startupAction();
        }
        finally
        {
            stopwatch.Stop();
            Console.WriteLine($""Startup completed in {stopwatch.ElapsedMilliseconds}ms"");
        }
    }

    public static void ValidateConfiguration()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        Console.WriteLine($""Loaded assemblies: {assemblies.Length}"");

        foreach (var assembly in assemblies.Where(a => a.GetName().Name?.Contains(""Avalonia"") == true))
        {
            Console.WriteLine($""  - {assembly.GetName().Name} v{assembly.GetName().Version}"");
        }
    }
}";
    }

    static string GenerateThreadingDebugCode()
    {
        return @"// Threading Debug Helper
public static class ThreadingDebugHelper
{
    public static void ValidateUIThread()
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            throw new InvalidOperationException(""This operation must be called on the UI thread"");
        }
    }

    public static async Task SafeUIUpdateAsync(Action uiUpdate)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            uiUpdate();
        }
        else
        {
            await Dispatcher.UIThread.InvokeAsync(uiUpdate);
        }
    }

    public static void LogThreadInfo(string operation)
    {
        var thread = Thread.CurrentThread;
        var isUIThread = Dispatcher.UIThread.CheckAccess();

        Console.WriteLine($""[THREAD] {operation}"");
        Console.WriteLine($""  Thread ID: {thread.ManagedThreadId}"");
        Console.WriteLine($""  Thread Name: {thread.Name ?? ""<unnamed>""}"");
        Console.WriteLine($""  Is UI Thread: {isUIThread}"");
        Console.WriteLine($""  Is Background: {thread.IsBackground}"");
    }

    public static IDisposable MonitorDispatcher()
    {
        var timer = new System.Timers.Timer(1000);
        timer.Elapsed += (s, e) =>
        {
            // Monitor dispatcher queue (simplified)
            Console.WriteLine($""[DISPATCHER] {DateTime.Now:HH:mm:ss} - Monitoring"");
        };
        timer.Start();

        return new DisposableAction(() => timer.Dispose());
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

    static string GenerateUtilityCode(string utilityType)
    {
        return utilityType switch
        {
            "logger" => GenerateLoggerUtility(),
            "visualtree" => GenerateVisualTreeUtility(),
            "binding" => GenerateBindingUtility(),
            "performance" => GeneratePerformanceUtility(),
            "memory" => GenerateMemoryUtility(),
            _ => "// Unknown utility type"
        };
    }

    static string GenerateLoggerUtility()
    {
        return @"// Structured Logging Utility
public static class AvaloniaLogger
{
    private static ILogger _logger = NullLogger.Instance;

    public static void Initialize(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public static void LogBinding(string property, object? value, string source)
    {
        _logger.LogDebug(""Binding Update: {Property} = {Value} from {Source}"", property, value, source);
    }

    public static void LogLayout(string element, Rect bounds, Size desiredSize)
    {
        _logger.LogDebug(""Layout: {Element} - Bounds: {Bounds}, DesiredSize: {DesiredSize}"",
            element, bounds, desiredSize);
    }

    public static void LogNavigation(string from, string to)
    {
        _logger.LogInformation(""Navigation: {From} -> {To}"", from, to);
    }

    public static void LogPerformance(string operation, TimeSpan duration)
    {
        _logger.LogInformation(""Performance: {Operation} took {Duration}ms"", operation, duration.TotalMilliseconds);
    }

    public static void LogError(Exception exception, string context)
    {
        _logger.LogError(exception, ""Error in {Context}"", context);
    }
}

// Custom log sink for Avalonia
public class AvaloniaLogSink : ILogSink
{
    private readonly ILogger _logger;

    public AvaloniaLogSink(ILogger logger)
    {
        _logger = logger;
    }

    public bool IsEnabled(LogEventLevel level, string area)
    {
        return level >= LogEventLevel.Information;
    }

    public void Log(LogEventLevel level, string area, object source, string messageTemplate, params object[] propertyValues)
    {
        var logLevel = level switch
        {
            LogEventLevel.Verbose => Microsoft.Extensions.Logging.LogLevel.Trace,
            LogEventLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
            LogEventLevel.Information => Microsoft.Extensions.Logging.LogLevel.Information,
            LogEventLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
            LogEventLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
            LogEventLevel.Fatal => Microsoft.Extensions.Logging.LogLevel.Critical,
            _ => Microsoft.Extensions.Logging.LogLevel.Information
        };

        _logger.Log(logLevel, ""[{Area}] {MessageTemplate}"", area, messageTemplate);
    }
}";
    }

    static string GenerateVisualTreeUtility()
    {
        return @"// Visual Tree Debugging Utility
public static class VisualTreeHelper
{
    public static void PrintVisualTree(Visual visual, int depth = 0)
    {
        var indent = new string(' ', depth * 2);
        var info = $""{visual.GetType().Name}"";

        if (visual is Control control)
        {
            info += $"" - Name: {control.Name ?? ""<unnamed>""}"";
            if (control.DataContext != null)
            {
                info += $"", DataContext: {control.DataContext.GetType().Name}"";
            }
        }

        Console.WriteLine($""{indent}{info}"");

        foreach (var child in visual.GetVisualChildren())
        {
            PrintVisualTree(child, depth + 1);
        }
    }

    public static T? FindChild<T>(Visual parent, string name = """") where T : class
    {
        foreach (var child in parent.GetVisualChildren())
        {
            if (child is T target && (string.IsNullOrEmpty(name) ||
                (child is Control control && control.Name == name)))
            {
                return target;
            }

            var found = FindChild<T>(child, name);
            if (found != null)
                return found;
        }

        return null;
    }

    public static IEnumerable<T> FindChildren<T>(Visual parent) where T : class
    {
        foreach (var child in parent.GetVisualChildren())
        {
            if (child is T target)
                yield return target;

            foreach (var grandChild in FindChildren<T>(child))
                yield return grandChild;
        }
    }

    public static void HighlightElement(Visual element, Color color)
    {
        if (element is Control control)
        {
            // Add visual indicator
            var overlay = new Border
            {
                Background = new SolidColorBrush(color) { Opacity = 0.3 },
                IsHitTestVisible = false
            };

            // This would require more complex implementation to properly overlay
            // In practice, use DevTools for visual debugging
        }
    }

    public static string GetVisualPath(Visual element)
    {
        var path = new List<string>();
        var current = element;

        while (current != null)
        {
            var name = current.GetType().Name;
            if (current is Control control && !string.IsNullOrEmpty(control.Name))
            {
                name += $""#{control.Name}"";
            }
            path.Insert(0, name);
            current = current.GetVisualParent();
        }

        return string.Join("" -> "", path);
    }
}";
    }

    static string GenerateBindingUtility()
    {
        return @"// Binding Debug Utility
public static class BindingDebugUtility
{
    public static void TraceBinding(Control control, string propertyName)
    {
        // This would require internal access to binding system
        // Implementation would track binding updates and errors
        Console.WriteLine($""Tracing binding for {control.GetType().Name}.{propertyName}"");
    }

    public static void ValidateBindingPath(object dataContext, string bindingPath)
    {
        if (dataContext == null)
        {
            Console.WriteLine($""❌ DataContext is null for path: {bindingPath}"");
            return;
        }

        var parts = bindingPath.Split('.');
        var current = dataContext;
        var currentPath = """";

        foreach (var part in parts)
        {
            currentPath = string.IsNullOrEmpty(currentPath) ? part : $""{currentPath}.{part}"";

            var property = current?.GetType().GetProperty(part);
            if (property == null)
            {
                Console.WriteLine($""❌ Property not found: {currentPath} on {current?.GetType().Name}"");
                return;
            }

            Console.WriteLine($""✅ Found property: {currentPath} ({property.PropertyType.Name})"");
            current = property.GetValue(current);

            if (current == null && parts.Last() != part)
            {
                Console.WriteLine($""⚠️  Null value at: {currentPath}"");
                return;
            }
        }

        Console.WriteLine($""✅ Binding path valid: {bindingPath}"");
    }

    public static void CheckPropertyChangeNotification(object viewModel, string propertyName)
    {
        if (viewModel is INotifyPropertyChanged notifyObject)
        {
            var eventFired = false;
            PropertyChangedEventHandler handler = (s, e) =>
            {
                if (e.PropertyName == propertyName)
                    eventFired = true;
            };

            notifyObject.PropertyChanged += handler;

            // Use reflection to trigger property change
            var property = viewModel.GetType().GetProperty(propertyName);
            if (property?.CanWrite == true)
            {
                var currentValue = property.GetValue(viewModel);
                property.SetValue(viewModel, currentValue); // Trigger setter

                if (eventFired)
                {
                    Console.WriteLine($""✅ PropertyChanged fired for: {propertyName}"");
                }
                else
                {
                    Console.WriteLine($""❌ PropertyChanged NOT fired for: {propertyName}"");
                }
            }

            notifyObject.PropertyChanged -= handler;
        }
        else
        {
            Console.WriteLine($""❌ ViewModel does not implement INotifyPropertyChanged"");
        }
    }
}";
    }

    static string GeneratePerformanceUtility()
    {
        return @"// Performance Monitoring Utility
public class PerformanceMonitor : IDisposable
{
    private readonly Timer _timer;
    private readonly ILogger _logger;
    private long _lastGcCount0;
    private long _lastGcCount1;
    private long _lastGcCount2;

    public PerformanceMonitor(ILogger logger, TimeSpan interval = default)
    {
        _logger = logger;
        var monitorInterval = interval == default ? TimeSpan.FromSeconds(5) : interval;

        _timer = new Timer(MonitorPerformance, null, TimeSpan.Zero, monitorInterval);

        _lastGcCount0 = GC.CollectionCount(0);
        _lastGcCount1 = GC.CollectionCount(1);
        _lastGcCount2 = GC.CollectionCount(2);
    }

    private void MonitorPerformance(object? state)
    {
        var process = Process.GetCurrentProcess();
        var memoryUsage = GC.GetTotalMemory(false);

        var gc0 = GC.CollectionCount(0) - _lastGcCount0;
        var gc1 = GC.CollectionCount(1) - _lastGcCount1;
        var gc2 = GC.CollectionCount(2) - _lastGcCount2;

        _logger.LogInformation(""Performance Metrics: Memory={MemoryMB}MB, GC=[{GC0},{GC1},{GC2}], Threads={ThreadCount}"",
            memoryUsage / (1024 * 1024),
            gc0, gc1, gc2,
            process.Threads.Count);

        _lastGcCount0 = GC.CollectionCount(0);
        _lastGcCount1 = GC.CollectionCount(1);
        _lastGcCount2 = GC.CollectionCount(2);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}

// Operation timing utility
public class OperationTimer : IDisposable
{
    private readonly Stopwatch _stopwatch;
    private readonly string _operationName;
    private readonly ILogger _logger;

    public OperationTimer(string operationName, ILogger logger)
    {
        _operationName = operationName;
        _logger = logger;
        _stopwatch = Stopwatch.StartNew();

        _logger.LogDebug(""Started operation: {Operation}"", operationName);
    }

    public void Dispose()
    {
        _stopwatch.Stop();
        _logger.LogInformation(""Completed operation: {Operation} in {Duration}ms"",
            _operationName, _stopwatch.ElapsedMilliseconds);
    }
}

// Usage
public static class PerformanceExtensions
{
    public static IDisposable TimeOperation(this ILogger logger, string operationName)
    {
        return new OperationTimer(operationName, logger);
    }
}";
    }

    static string GenerateMemoryUtility()
    {
        return @"// Memory Monitoring Utility
public static class MemoryMonitor
{
    private static readonly ConcurrentDictionary<string, WeakReference> _trackedObjects = new();

    public static void TrackObject(string key, object obj)
    {
        _trackedObjects[key] = new WeakReference(obj);
    }

    public static void CheckTrackedObjects()
    {
        var collected = new List<string>();

        foreach (var kvp in _trackedObjects)
        {
            if (!kvp.Value.IsAlive)
            {
                collected.Add(kvp.Key);
            }
        }

        foreach (var key in collected)
        {
            _trackedObjects.TryRemove(key, out _);
        }

        Console.WriteLine($""Memory Check: {_trackedObjects.Count} objects alive, {collected.Count} collected"");
    }

    public static void ForceGarbageCollection()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var memoryBefore = GC.GetTotalMemory(false);
        GC.Collect();
        var memoryAfter = GC.GetTotalMemory(true);

        Console.WriteLine($""Garbage Collection: Freed {(memoryBefore - memoryAfter) / 1024}KB"");
    }

    public static MemorySnapshot TakeSnapshot()
    {
        return new MemorySnapshot
        {
            Timestamp = DateTime.UtcNow,
            TotalMemory = GC.GetTotalMemory(false),
            Gen0Collections = GC.CollectionCount(0),
            Gen1Collections = GC.CollectionCount(1),
            Gen2Collections = GC.CollectionCount(2),
            TrackedObjectCount = _trackedObjects.Count
        };
    }

    public static void CompareSnapshots(MemorySnapshot before, MemorySnapshot after)
    {
        var memoryDiff = after.TotalMemory - before.TotalMemory;
        var gen0Diff = after.Gen0Collections - before.Gen0Collections;
        var gen1Diff = after.Gen1Collections - before.Gen1Collections;
        var gen2Diff = after.Gen2Collections - before.Gen2Collections;
        var duration = after.Timestamp - before.Timestamp;

        Console.WriteLine($""Memory Analysis ({duration.TotalSeconds:F1}s):"");
        Console.WriteLine($""  Memory Change: {memoryDiff / 1024:N0}KB"");
        Console.WriteLine($""  GC Collections: Gen0={gen0Diff}, Gen1={gen1Diff}, Gen2={gen2Diff}"");
        Console.WriteLine($""  Tracked Objects: {after.TrackedObjectCount}"");
    }
}

public class MemorySnapshot
{
    public DateTime Timestamp { get; set; }
    public long TotalMemory { get; set; }
    public int Gen0Collections { get; set; }
    public int Gen1Collections { get; set; }
    public int Gen2Collections { get; set; }
    public int TrackedObjectCount { get; set; }
}";
    }

    static string GenerateDevToolsIntegration()
    {
        return @"// DevTools Integration
public static class DevToolsHelper
{
    public static void EnableDevTools(Application app)
    {
#if DEBUG
        app.AttachDevTools(new DevToolsOptions
        {
            ShowAsChildWindow = true,
            LaunchView = DevToolsViewKind.LogicalTree
        });
#endif
    }

    public static void ConfigureDevTools(AppBuilder builder)
    {
        builder.ConfigureDevTools(options =>
        {
#if DEBUG
            options.ShowAsChildWindow = true;
            options.LaunchView = DevToolsViewKind.LogicalTree;
            options.Size = new Size(1024, 768);
            options.Position = new PixelPoint(100, 100);
#endif
        });
    }

    public static void AddCustomDevToolsTab(Window mainWindow)
    {
#if DEBUG
        // Custom DevTools extensions would be added here
        // This requires extending the DevTools infrastructure
#endif
    }
}

// Custom DevTools view (example)
#if DEBUG
public class PerformanceDevToolsView : UserControl
{
    private readonly Timer _updateTimer;
    private readonly TextBlock _metricsDisplay;

    public PerformanceDevToolsView()
    {
        _metricsDisplay = new TextBlock { FontFamily = ""Consolas"" };
        Content = new ScrollViewer { Content = _metricsDisplay };

        _updateTimer = new Timer(UpdateMetrics, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    private void UpdateMetrics(object? state)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var process = Process.GetCurrentProcess();
            var memory = GC.GetTotalMemory(false);

            var metrics = $@""Performance Metrics
Memory: {memory / (1024 * 1024):N1} MB
GC Gen0: {GC.CollectionCount(0)}
GC Gen1: {GC.CollectionCount(1)}
GC Gen2: {GC.CollectionCount(2)}
Threads: {process.Threads.Count}
Updated: {DateTime.Now:HH:mm:ss}"";

            _metricsDisplay.Text = metrics;
        });
    }
}
#endif";
    }

    static string GenerateTelemetryCode()
    {
        return @"// Telemetry Integration
public class AvaloniaTelemetry
{
    private readonly ILogger _logger;
    private readonly string _applicationName;

    public AvaloniaTelemetry(ILogger logger, string applicationName)
    {
        _logger = logger;
        _applicationName = applicationName;
    }

    public void TrackEvent(string eventName, Dictionary<string, object>? properties = null)
    {
        var telemetryEvent = new
        {
            Application = _applicationName,
            Event = eventName,
            Timestamp = DateTime.UtcNow,
            Properties = properties ?? new Dictionary<string, object>()
        };

        _logger.LogInformation(""Telemetry: {Event}"", JsonSerializer.Serialize(telemetryEvent));
    }

    public void TrackException(Exception exception, Dictionary<string, object>? properties = null)
    {
        var telemetryData = new
        {
            Application = _applicationName,
            Exception = new
            {
                Type = exception.GetType().Name,
                Message = exception.Message,
                StackTrace = exception.StackTrace
            },
            Timestamp = DateTime.UtcNow,
            Properties = properties ?? new Dictionary<string, object>()
        };

        _logger.LogError(""Exception Telemetry: {Data}"", JsonSerializer.Serialize(telemetryData));
    }

    public void TrackPerformance(string operationName, TimeSpan duration, Dictionary<string, object>? properties = null)
    {
        var performanceData = new
        {
            Application = _applicationName,
            Operation = operationName,
            Duration = duration.TotalMilliseconds,
            Timestamp = DateTime.UtcNow,
            Properties = properties ?? new Dictionary<string, object>()
        };

        _logger.LogInformation(""Performance Telemetry: {Data}"", JsonSerializer.Serialize(performanceData));
    }

    public void TrackUserAction(string action, string? elementName = null, Dictionary<string, object>? properties = null)
    {
        var actionData = new
        {
            Application = _applicationName,
            Action = action,
            Element = elementName,
            Timestamp = DateTime.UtcNow,
            Properties = properties ?? new Dictionary<string, object>()
        };

        _logger.LogInformation(""User Action Telemetry: {Data}"", JsonSerializer.Serialize(actionData));
    }
}

// Usage in ViewModels
public class TelemetryExtensions
{
    public static void TrackCommand(AvaloniaTelemetry telemetry, string commandName)
    {
        telemetry.TrackUserAction(""Command"", commandName);
    }

    public static void TrackNavigation(AvaloniaTelemetry telemetry, string fromView, string toView)
    {
        telemetry.TrackEvent(""Navigation"", new Dictionary<string, object>
        {
            { ""From"", fromView },
            { ""To"", toView }
        });
    }

    public static void TrackError(AvaloniaTelemetry telemetry, Exception exception, string context)
    {
        telemetry.TrackException(exception, new Dictionary<string, object>
        {
            { ""Context"", context }
        });
    }
}";
    }
}