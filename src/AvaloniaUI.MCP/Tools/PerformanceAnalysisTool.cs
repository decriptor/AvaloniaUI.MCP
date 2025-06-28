using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Tools;

[McpServerToolType]
public static class PerformanceAnalysisTool
{
    [McpServerTool, Description("Analyzes AvaloniaUI XAML and C# code for performance issues and optimization opportunities")]
    public static string AnalyzePerformance(
        [Description("The XAML or C# code content to analyze")] string codeContent,
        [Description("Type of analysis: 'xaml', 'csharp', or 'auto' to detect automatically")] string analysisType = "auto")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(codeContent))
            {
                return "Error: Code content cannot be empty";
            }

            var detectedType = analysisType == "auto" ? DetectCodeType(codeContent) : analysisType;
            var issues = new List<string>();
            var recommendations = new List<string>();

            switch (detectedType.ToLowerInvariant())
            {
                case "xaml":
                    AnalyzeXamlPerformance(codeContent, issues, recommendations);
                    break;
                case "csharp":
                    AnalyzeCSharpPerformance(codeContent, issues, recommendations);
                    break;
                default:
                    return "Error: Unable to determine code type. Please specify 'xaml' or 'csharp' explicitly.";
            }

            return FormatPerformanceReport(detectedType, issues, recommendations);
        }
        catch (Exception ex)
        {
            return $"Error during performance analysis: {ex.Message}";
        }
    }

    [McpServerTool, Description("Provides performance optimization recommendations for AvaloniaUI applications")]
    public static string GetPerformanceRecommendations(
        [Description("Specific performance area: 'bindings', 'layout', 'styling', 'collections', 'general'")] string area = "general")
    {
        return area.ToLowerInvariant() switch
        {
            "bindings" => GetBindingPerformanceRecommendations(),
            "layout" => GetLayoutPerformanceRecommendations(),
            "styling" => GetStylingPerformanceRecommendations(),
            "collections" => GetCollectionPerformanceRecommendations(),
            _ => GetGeneralPerformanceRecommendations()
        };
    }

    private static string DetectCodeType(string content)
    {
        if (content.TrimStart().StartsWith("<") && (content.Contains("xmlns") || content.Contains("</")))
        {
            return "xaml";
        }
        if (content.Contains("namespace") || content.Contains("class") || content.Contains("using"))
        {
            return "csharp";
        }
        return "unknown";
    }

    private static void AnalyzeXamlPerformance(string xamlContent, List<string> issues, List<string> recommendations)
    {
        try
        {
            var doc = XDocument.Parse(xamlContent);

            // Check for missing x:DataType (compiled bindings)
            CheckForCompiledBindings(doc, issues, recommendations);

            // Check for inefficient binding patterns
            CheckBindingPatterns(doc, issues, recommendations);

            // Check for layout performance issues
            CheckLayoutPerformance(doc, issues, recommendations);

            // Check for resource usage
            CheckResourceUsage(doc, issues, recommendations);

            // Check for control virtualization
            CheckVirtualization(doc, issues, recommendations);

            // Check for styling performance
            CheckStylingPerformance(doc, issues, recommendations);
        }
        catch (XmlException)
        {
            issues.Add("❌ Invalid XAML syntax - cannot analyze performance");
        }
    }

    private static void CheckForCompiledBindings(XDocument doc, List<string> issues, List<string> recommendations)
    {
        var root = doc.Root;
        if (root != null)
        {
            var hasDataType = root.Attributes().Any(a => a.Name.LocalName == "DataType");
            var hasBindings = doc.Descendants().Any(e =>
                e.Attributes().Any(a => a.Value.Contains("{Binding")));

            if (hasBindings && !hasDataType)
            {
                issues.Add("⚠️ Missing x:DataType - using reflection-based bindings");
                recommendations.Add("✅ Add x:DataType attribute to enable compiled bindings for better performance");
                recommendations.Add("   Example: x:DataType=\"vm:MyViewModel\"");
            }
            else if (hasDataType)
            {
                recommendations.Add("✅ Using compiled bindings - good for performance!");
            }
        }
    }

    private static void CheckBindingPatterns(XDocument doc, List<string> issues, List<string> recommendations)
    {
        var elementsWithBindings = doc.Descendants().Where(e =>
            e.Attributes().Any(a => a.Value.Contains("{Binding")));

        foreach (var element in elementsWithBindings)
        {
            var bindingAttrs = element.Attributes().Where(a => a.Value.Contains("{Binding"));
            foreach (var attr in bindingAttrs)
            {
                var bindingValue = attr.Value;

                // Check for complex binding expressions
                if (bindingValue.Count(c => c == '.') > 2)
                {
                    issues.Add($"⚠️ Complex binding path detected: {bindingValue.Substring(0, Math.Min(50, bindingValue.Length))}...");
                    recommendations.Add("✅ Consider flattening complex binding paths or using converters");
                }

                // Check for string format in bindings
                if (bindingValue.Contains("StringFormat") && !bindingValue.Contains("x:DataType"))
                {
                    recommendations.Add("✅ StringFormat with compiled bindings performs better than with reflection bindings");
                }

                // Check for inefficient RelativeSource bindings
                if (bindingValue.Contains("RelativeSource") && bindingValue.Contains("FindAncestor"))
                {
                    issues.Add($"⚠️ FindAncestor binding detected - consider using $parent syntax");
                    recommendations.Add("✅ Use $parent[ControlType] instead of FindAncestor for better performance");
                }
            }
        }
    }

    private static void CheckLayoutPerformance(XDocument doc, List<string> issues, List<string> recommendations)
    {
        // Check for nested layout containers
        var layoutControls = new[] { "Grid", "StackPanel", "DockPanel", "Canvas", "WrapPanel" };
        var nestedLayouts = doc.Descendants().Where(e =>
            layoutControls.Contains(e.Name.LocalName) &&
            e.Descendants().Any(child => layoutControls.Contains(child.Name.LocalName)));

        var nestedCount = nestedLayouts.Count();
        if (nestedCount > 3)
        {
            issues.Add($"⚠️ Deep layout nesting detected ({nestedCount} levels) - may impact performance");
            recommendations.Add("✅ Consider flattening layout hierarchy where possible");
        }

        // Check for Grid without explicit row/column definitions
        var gridsWithoutDefinitions = doc.Descendants("Grid").Where(g =>
            !g.Elements().Any(e => e.Name.LocalName.Contains("Definition")));

        foreach (var grid in gridsWithoutDefinitions)
        {
            var hasGridProperties = grid.Descendants().Any(e =>
                e.Attributes().Any(a => a.Name.LocalName.StartsWith("Grid.")));

            if (hasGridProperties)
            {
                issues.Add("⚠️ Grid with Grid.Row/Column but no RowDefinitions/ColumnDefinitions");
                recommendations.Add("✅ Define explicit RowDefinitions and ColumnDefinitions for better performance");
            }
        }

        // Check for Canvas overuse
        var canvasElements = doc.Descendants("Canvas").Count();
        if (canvasElements > 2)
        {
            issues.Add($"⚠️ Multiple Canvas controls detected ({canvasElements}) - Canvas doesn't provide layout optimization");
            recommendations.Add("✅ Use Grid or other layout panels when possible - Canvas bypasses layout optimization");
        }
    }

    private static void CheckResourceUsage(XDocument doc, List<string> issues, List<string> recommendations)
    {
        var resources = doc.Descendants().Where(e => e.Name.LocalName.EndsWith("Resources"));
        var resourceCount = resources.SelectMany(r => r.Elements()).Count();

        if (resourceCount > 50)
        {
            issues.Add($"⚠️ Large number of resources ({resourceCount}) in single file");
            recommendations.Add("✅ Consider splitting resources into separate ResourceDictionary files");
        }

        // Check for duplicate resource keys
        var allResources = resources.SelectMany(r => r.Elements()).ToList();
        var resourceKeys = allResources
            .Select(r => r.Attribute(XName.Get("Key", "http://schemas.microsoft.com/winfx/2006/xaml"))?.Value)
            .Where(k => k != null)
            .ToList();

        var duplicateKeys = resourceKeys.GroupBy(k => k).Where(g => g.Count() > 1).Select(g => g.Key);
        foreach (var key in duplicateKeys)
        {
            issues.Add($"⚠️ Duplicate resource key: {key}");
        }
    }

    private static void CheckVirtualization(XDocument doc, List<string> issues, List<string> recommendations)
    {
        var listControls = doc.Descendants().Where(e =>
            new[] { "ListBox", "ListView", "DataGrid", "TreeView" }.Contains(e.Name.LocalName));

        foreach (var control in listControls)
        {
            var hasVirtualization = control.Attributes().Any(a =>
                a.Name.LocalName.Contains("Virtualization") ||
                a.Value.Contains("VirtualizingStackPanel"));

            if (!hasVirtualization)
            {
                recommendations.Add($"✅ Consider enabling virtualization for {control.Name.LocalName} with large data sets");
                recommendations.Add("   Add VirtualizingStackPanel.IsVirtualizing=\"True\"");
            }
        }
    }

    private static void CheckStylingPerformance(XDocument doc, List<string> issues, List<string> recommendations)
    {
        var styles = doc.Descendants("Style");
        var complexSelectors = styles.Where(s =>
        {
            var selector = s.Attribute("Selector")?.Value ?? "";
            return selector.Count(c => c == ' ') > 3 || selector.Contains(">");
        });

        if (complexSelectors.Any())
        {
            issues.Add($"⚠️ Complex CSS selectors detected ({complexSelectors.Count()}) - may impact styling performance");
            recommendations.Add("✅ Simplify selectors where possible or use style classes");
        }

        // Check for inline styles
        var inlineStyles = doc.Descendants().Where(e =>
            e.Attributes().Any(a => new[] { "Background", "Foreground", "FontSize", "FontWeight" }.Contains(a.Name.LocalName)));

        if (inlineStyles.Count() > 10)
        {
            issues.Add($"⚠️ Many inline style properties ({inlineStyles.Count()}) - consider using styles or themes");
            recommendations.Add("✅ Define reusable styles instead of inline properties");
        }
    }

    private static void AnalyzeCSharpPerformance(string csharpContent, List<string> issues, List<string> recommendations)
    {
        // Check for INotifyPropertyChanged implementation
        CheckPropertyChangeNotification(csharpContent, issues, recommendations);

        // Check for async/await patterns
        CheckAsyncPatterns(csharpContent, issues, recommendations);

        // Check for collection performance
        CheckCollectionUsage(csharpContent, issues, recommendations);

        // Check for memory leaks
        CheckMemoryLeakPatterns(csharpContent, issues, recommendations);

        // Check for AvaloniaProperty usage
        CheckAvaloniaPropertyUsage(csharpContent, issues, recommendations);
    }

    private static void CheckPropertyChangeNotification(string content, List<string> issues, List<string> recommendations)
    {
        var hasINotifyPropertyChanged = content.Contains("INotifyPropertyChanged");
        var hasPropertyChangedEvent = content.Contains("PropertyChanged");
        var hasReactiveUI = content.Contains("ReactiveObject") || content.Contains("RaiseAndSetIfChanged");

        if (!hasINotifyPropertyChanged && !hasReactiveUI)
        {
            issues.Add("⚠️ No property change notification detected in ViewModel");
            recommendations.Add("✅ Implement INotifyPropertyChanged or inherit from ReactiveObject for data binding");
        }

        if (hasPropertyChangedEvent && !hasReactiveUI)
        {
            var hasPropertyChangedInvocation = Regex.IsMatch(content, @"PropertyChanged\?.Invoke|OnPropertyChanged");
            if (!hasPropertyChangedInvocation)
            {
                issues.Add("⚠️ PropertyChanged event declared but not invoked");
                recommendations.Add("✅ Ensure PropertyChanged is invoked when properties change");
            }
        }
    }

    private static void CheckAsyncPatterns(string content, List<string> issues, List<string> recommendations)
    {
        var hasAsyncVoid = Regex.IsMatch(content, @"async\s+void\s+(?!.*EventArgs)");
        if (hasAsyncVoid)
        {
            issues.Add("⚠️ async void methods detected (except event handlers)");
            recommendations.Add("✅ Use async Task instead of async void for non-event handlers");
        }

        var hasConfigureAwaitFalse = content.Contains("ConfigureAwait(false)");
        var hasAwait = content.Contains("await");

        if (hasAwait && !hasConfigureAwaitFalse && !content.Contains("UI") && !content.Contains("Dispatcher"))
        {
            recommendations.Add("✅ Consider using ConfigureAwait(false) for non-UI async operations");
        }
    }

    private static void CheckCollectionUsage(string content, List<string> issues, List<string> recommendations)
    {
        if (content.Contains("List<") && content.Contains("Add(") && content.Contains("foreach"))
        {
            recommendations.Add("✅ Consider using ObservableCollection for data-bound collections");
        }

        var hasLinqInLoop = Regex.IsMatch(content, @"(for|foreach|while).*\{[^}]*\.(Where|Select|First|Single)");
        if (hasLinqInLoop)
        {
            issues.Add("⚠️ LINQ operations inside loops detected");
            recommendations.Add("✅ Move LINQ operations outside loops or optimize queries");
        }
    }

    private static void CheckMemoryLeakPatterns(string content, List<string> issues, List<string> recommendations)
    {
        var hasEventSubscription = content.Contains("+=") && (content.Contains("Event") || content.Contains("Changed"));
        var hasEventUnsubscription = content.Contains("-=");

        if (hasEventSubscription && !hasEventUnsubscription)
        {
            issues.Add("⚠️ Event subscriptions without unsubscription detected");
            recommendations.Add("✅ Ensure proper event unsubscription to prevent memory leaks");
        }

        var hasTimerOrDispatcher = content.Contains("Timer") || content.Contains("DispatcherTimer");
        if (hasTimerOrDispatcher && !content.Contains("Dispose"))
        {
            issues.Add("⚠️ Timer usage without disposal pattern");
            recommendations.Add("✅ Properly dispose timers to prevent memory leaks");
        }
    }

    private static void CheckAvaloniaPropertyUsage(string content, List<string> issues, List<string> recommendations)
    {
        var hasAvaloniaProperty = content.Contains("AvaloniaProperty");
        var hasStyledProperty = content.Contains("StyledProperty");

        if (content.Contains("DependencyProperty"))
        {
            issues.Add("⚠️ WPF DependencyProperty usage detected");
            recommendations.Add("✅ Use AvaloniaProperty instead of DependencyProperty");
        }

        if (hasAvaloniaProperty || hasStyledProperty)
        {
            recommendations.Add("✅ Good use of AvaloniaProperty for custom controls");
        }
    }

    private static string FormatPerformanceReport(string codeType, List<string> issues, List<string> recommendations)
    {
        var report = $"# AvaloniaUI Performance Analysis Report ({codeType.ToUpper()})\n\n";

        if (issues.Any())
        {
            report += "## 🚨 Performance Issues Found\n\n";
            foreach (var issue in issues)
            {
                report += $"{issue}\n";
            }
            report += "\n";
        }
        else
        {
            report += "## ✅ No Performance Issues Found\n\n";
        }

        if (recommendations.Any())
        {
            report += "## 💡 Performance Recommendations\n\n";
            foreach (var recommendation in recommendations)
            {
                report += $"{recommendation}\n";
            }
            report += "\n";
        }

        report += "## 📊 Performance Score\n\n";
        var score = Math.Max(0, 100 - (issues.Count * 10));
        report += $"**Score: {score}/100**\n\n";

        if (score >= 90)
            report += "🏆 Excellent performance optimization!\n";
        else if (score >= 70)
            report += "👍 Good performance with room for improvement\n";
        else if (score >= 50)
            report += "⚠️ Moderate performance issues - consider optimizations\n";
        else
            report += "🚨 Significant performance issues - optimization recommended\n";

        return report;
    }

    private static string GetBindingPerformanceRecommendations()
    {
        return @"# Data Binding Performance Recommendations

## 🏆 Best Practices

### 1. Use Compiled Bindings
- Add `x:DataType=""vm:YourViewModel""` to your controls
- Provides compile-time validation and better performance
- IntelliSense support for binding expressions

### 2. Optimize Binding Paths
- Keep binding paths as short as possible
- Avoid deeply nested property paths
- Use converters for complex transformations

### 3. Choose Appropriate Binding Modes
- `OneWay`: For read-only display (default for most properties)
- `TwoWay`: For input controls that need to update the source
- `OneTime`: For static data that never changes
- `OneWayToSource`: For custom scenarios

### 4. Use $parent Syntax
Instead of: `{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type UserControl}}}`
Use: `{Binding $parent[UserControl].PropertyName}`

### 5. Implement INotifyPropertyChanged Efficiently
- Use ReactiveUI's `RaiseAndSetIfChanged` method
- Only raise notifications when values actually change
- Consider using ReactiveCommand for commands

## ❌ Avoid These Patterns
- Complex binding expressions with multiple calculations
- Binding to methods (use properties with getters instead)
- Frequent binding updates in loops";
    }

    private static string GetLayoutPerformanceRecommendations()
    {
        return @"# Layout Performance Recommendations

## 🏆 Best Practices

### 1. Choose the Right Layout Container
- **Grid**: For complex layouts with rows/columns
- **StackPanel**: For simple linear layouts
- **DockPanel**: For edge-docked layouts with fill center
- **Canvas**: Only when absolute positioning is required

### 2. Optimize Grid Usage
- Define explicit RowDefinitions and ColumnDefinitions
- Use `*` for proportional sizing, `Auto` for content sizing
- Avoid deep Grid nesting

### 3. Use Spacing Properties (11.3+)
```xml
<Grid RowSpacing=""8"" ColumnSpacing=""12"">
<StackPanel Spacing=""10"">
<DockPanel Spacing=""5"">
```

### 4. Minimize Layout Passes
- Set fixed sizes when content size is known
- Use `MaxWidth/MaxHeight` instead of complex constraints
- Avoid frequent layout-invalidating property changes

### 5. Consider Virtualization
- Enable for large lists: `VirtualizingStackPanel.IsVirtualizing=""True""`
- Use `ScrollViewer` with virtualization for long content

## ❌ Avoid These Patterns
- Deep nesting of layout containers (>5 levels)
- Canvas for general layout purposes
- Frequent programmatic layout changes
- Missing RowDefinitions/ColumnDefinitions in Grid";
    }

    private static string GetStylingPerformanceRecommendations()
    {
        return @"# Styling Performance Recommendations

## 🏆 Best Practices

### 1. Use Efficient Selectors
- Prefer class selectors: `Button.primary`
- Use specific selectors: `#MyButton`
- Avoid overly complex descendant selectors

### 2. Organize Styles Efficiently
- Group related styles together
- Use ResourceDictionaries for large style collections
- Minimize inline styling

### 3. Leverage CSS-like Features
```xml
<Style Selector=""Button:pointerover"">
<Style Selector=""Button.primary:pressed"">
<Style Selector=""StackPanel > Button"">
```

### 4. Optimize Resource Usage
- Use StaticResource for static references
- Consider DynamicResource only when needed
- Avoid duplicate resource definitions

### 5. Use Style Classes
```xml
<Button Classes=""primary large"" Content=""Styled Button"" />
```

## ❌ Avoid These Patterns
- Excessive use of DynamicResource
- Complex selector hierarchies
- Inline styles for repeated patterns
- Large numbers of unused styles";
    }

    private static string GetCollectionPerformanceRecommendations()
    {
        return @"# Collection Performance Recommendations

## 🏆 Best Practices

### 1. Use Appropriate Collection Types
- `ObservableCollection<T>`: For data-bound collections
- `List<T>`: For internal collections not bound to UI
- `ReadOnlyObservableCollection<T>`: For exposing collections

### 2. Enable Virtualization
```xml
<ListBox VirtualizingStackPanel.IsVirtualizing=""True""
         VirtualizingStackPanel.VirtualizationMode=""Recycling"">
```

### 3. Optimize Data Templates
- Keep templates simple and lightweight
- Use data binding efficiently
- Consider template selectors for different item types

### 4. Batch Collection Updates
```csharp
// Instead of multiple Add() calls
collection.AddRange(items);

// Or use batching pattern
using (collection.DeferRefresh())
{
    // Multiple operations
}
```

### 5. Use Async Loading
- Load data asynchronously for large collections
- Implement paging or incremental loading
- Show loading indicators during data operations

## ❌ Avoid These Patterns
- Clearing and repopulating collections frequently
- Complex calculations in ItemTemplate bindings
- Synchronous loading of large datasets
- Missing virtualization for large lists";
    }

    private static string GetGeneralPerformanceRecommendations()
    {
        return @"# General AvaloniaUI Performance Recommendations

## 🏆 Best Practices

### 1. Application Startup
- Minimize work in Application constructor
- Use lazy loading for heavy resources
- Consider splash screens for complex apps

### 2. Memory Management
- Implement proper disposal patterns
- Unsubscribe from events when done
- Use weak event patterns where appropriate

### 3. Async Programming
- Use async/await for I/O operations
- Use ConfigureAwait(false) for non-UI code
- Avoid async void except for event handlers

### 4. Platform Optimization
- Test on all target platforms
- Use platform-specific optimizations when needed
- Profile on resource-constrained devices

### 5. Developer Tools
- Use AvaloniaUI DevTools for debugging
- Profile with dotTrace or similar tools
- Monitor memory usage during development

## 📊 Performance Monitoring

### Key Metrics to Watch
- Application startup time
- Memory usage patterns
- UI responsiveness
- Layout calculation time
- Binding evaluation time

### Tools to Use
- AvaloniaUI DevTools (F12 in debug)
- .NET profilers (dotTrace, PerfView)
- Memory profilers
- Platform-specific profiling tools

## 🚀 Advanced Optimizations
- Consider custom control development for specialized needs
- Use compiled expressions for complex calculations
- Implement custom layout panels for specific scenarios
- Use GPU acceleration where supported";
    }
}