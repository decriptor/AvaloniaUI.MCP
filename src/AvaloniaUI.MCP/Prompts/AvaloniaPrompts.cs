using System.ComponentModel;
using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Prompts;

[McpServerPromptType]
public static class AvaloniaPrompts
{
    [McpServerPrompt]
    [Description("Template for creating a new AvaloniaUI application with MVVM pattern")]
    public static Task<string> CreateAvaloniaAppPrompt(
        [Description("Name of the application")] string appName,
        [Description("Brief description of what the app should do")] string appDescription,
        [Description("Target platforms: desktop, mobile, or all")] string targetPlatforms = "desktop")
    {
        var prompt = $@"# Create AvaloniaUI Application: {appName}

## Project Requirements
- **Application Name:** {appName}
- **Description:** {appDescription}
- **Target Platforms:** {targetPlatforms}
- **Architecture:** MVVM (Model-View-ViewModel)

## Implementation Steps

### 1. Project Structure
Create the following project structure:
```
{appName}/
├── {appName}.csproj
├── Program.cs
├── App.axaml
├── App.axaml.cs
├── Views/
│   ├── MainWindow.axaml
│   └── MainWindow.axaml.cs
├── ViewModels/
│   ├── ViewModelBase.cs
│   └── MainWindowViewModel.cs
└── Models/
    └── (domain models as needed)
```

### 2. Required NuGet Packages
- Avalonia (11.3.2 - latest stable)
- Avalonia.Desktop (for desktop support)
- Avalonia.ReactiveUI (for MVVM support)
- Avalonia.Fonts.Inter (for modern typography)
- Avalonia.Controls.DataGrid (for data grid functionality)
- Additional platform packages as needed

### 3. Key Implementation Points
- Use .axaml extension for XAML files
- Implement ViewModelBase with INotifyPropertyChanged
- Use ReactiveCommand for command handling
- Set up proper data binding with x:DataType for compiled bindings
- Follow AvaloniaUI naming conventions and best practices

### 4. XAML Structure
- Include proper AvaloniaUI namespaces
- Use FluentTheme for modern styling
- Implement responsive design for cross-platform compatibility
- Use appropriate layout controls for the target platforms

### 5. Platform Considerations
{GetPlatformConsiderations(targetPlatforms)}

## Next Steps
1. Use the CreateAvaloniaProject tool to generate the project structure
2. Implement the core functionality based on the application description
3. Test on all target platforms
4. Apply appropriate styling and themes

Please proceed with creating this AvaloniaUI application following these guidelines.";

        return Task.FromResult(prompt);
    }

    [McpServerPrompt]
    [Description("Template for migrating a WPF application to AvaloniaUI")]
    public static Task<string> MigrateFromWpfPrompt(
        [Description("Name of the WPF application to migrate")] string wpfAppName,
        [Description("Key features of the WPF application")] string wpfFeatures,
        [Description("Known WPF-specific components used")] string wpfComponents = "")
    {
        var prompt = $@"# Migrate WPF Application to AvaloniaUI: {wpfAppName}

## Migration Overview
- **Source:** WPF Application '{wpfAppName}'
- **Target:** AvaloniaUI Cross-Platform Application
- **Key Features:** {wpfFeatures}
- **WPF Components:** {wpfComponents}

## Migration Strategy

### Phase 1: Analysis and Preparation
1. **Inventory Assessment**
   - Catalog all WPF controls and custom controls used
   - Identify WPF-specific features (triggers, styles, resources)
   - Document data binding patterns and dependencies
   - List third-party WPF libraries and their AvaloniaUI alternatives

2. **Compatibility Check**
   - Review controls against AvaloniaUI availability
   - Identify controls that need alternatives or custom implementation
   - Check for WPF-specific features that need adaptation

### Phase 2: Project Setup
1. **Create New AvaloniaUI Project**
   - Set up project structure matching WPF organization
   - Configure target platforms and packages
   - Set up MVVM infrastructure if not already present

2. **Namespace Migration**
   - Replace WPF namespaces with AvaloniaUI equivalents
   - Update xmlns declarations in all XAML files
   - Change file extensions from .xaml to .axaml (recommended)

### Phase 3: XAML Migration
1. **Control Conversion**
   - Convert compatible controls (most WPF controls work in AvaloniaUI)
   - Replace unsupported controls with AvaloniaUI alternatives
   - Update control templates and data templates

2. **Styling Migration**
   - Convert WPF styles to AvaloniaUI selector syntax
   - Update resource dictionaries
   - Adapt triggers to AvaloniaUI styling system

3. **Binding Updates**
   - Most WPF bindings work unchanged in AvaloniaUI
   - Update RelativeSource FindAncestor to $parent syntax
   - Implement x:DataType for compiled bindings (recommended)

### Phase 4: Code-Behind Migration
1. **Event Handlers**
   - Most event handlers remain unchanged
   - Update any WPF-specific APIs to AvaloniaUI equivalents

2. **Dependency Properties**
   - Convert DependencyProperty to AvaloniaProperty
   - Update property registration syntax
   - Maintain property metadata and callbacks

### Phase 5: Testing and Refinement
1. **Cross-Platform Testing**
   - Test on Windows, macOS, and Linux
   - Verify UI layout and behavior consistency
   - Test platform-specific features

2. **Performance Optimization**
   - Use compiled bindings where possible
   - Optimize for target platforms
   - Profile and address performance issues

## Key Migration Points

### Namespace Changes
```xml
<!-- WPF -->
<Window xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">

<!-- AvaloniaUI -->
<Window xmlns=""https://github.com/avaloniaui""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
```

### Styling Changes
```xml
<!-- WPF -->
<Style TargetType=""Button"">

<!-- AvaloniaUI -->
<Style Selector=""Button"">
```

### Binding Adaptations
```xml
<!-- WPF FindAncestor -->
{{Binding RelativeSource={{RelativeSource FindAncestor, AncestorType={{x:Type UserControl}}}}}}

<!-- AvaloniaUI -->
{{Binding $parent[UserControl].PropertyName}}
```

## Tools to Use
1. **ConvertWpfXamlToAvalonia** tool for automatic XAML conversion
2. **ValidateXaml** tool to check converted XAML
3. **GetControlMappings** resource for control compatibility reference
4. **GetMigrationGuide** resource for detailed migration information

## Expected Challenges
- Custom controls may need complete reimplementation
- Complex triggers and animations may need adaptation
- Third-party WPF libraries need AvaloniaUI alternatives
- Platform-specific behaviors may need conditional implementation

Please proceed with the migration following this structured approach.";

        return Task.FromResult(prompt);
    }

    [McpServerPrompt]
    [Description("Template for debugging common AvaloniaUI issues")]
    public static Task<string> DebugAvaloniaIssuePrompt(
        [Description("Description of the issue being experienced")] string issueDescription,
        [Description("Type of issue: binding, layout, styling, performance, or platform")] string issueType = "binding")
    {
        var prompt = $@"# Debug AvaloniaUI Issue

## Issue Details
- **Problem:** {issueDescription}
- **Category:** {issueType}

## Diagnostic Approach

### Step 1: Initial Analysis
{GetDiagnosticSteps(issueType)}

### Step 2: Common Solutions
{GetCommonSolutions(issueType)}

### Step 3: Debugging Tools
1. **AvaloniaUI DevTools**
   - Enable with `AttachDevTools()` in debug mode
   - Use Visual Tree inspector
   - Monitor property values and bindings

2. **Logging**
   - Enable binding trace logging
   - Check console output for binding errors
   - Use LogToTrace() in app builder

3. **XAML Validation**
   - Use ValidateXaml tool to check syntax
   - Verify namespace declarations
   - Check for typos in property names

### Step 4: Platform-Specific Checks
- Test on different platforms if cross-platform
- Check for platform-specific behavior differences
- Verify resource availability on target platforms

## Next Steps
1. Apply the diagnostic steps above
2. Check the AvaloniaUI documentation for specific guidance
3. Use the GetControlsReference resource for control-specific information
4. Consider posting on AvaloniaUI community forums if issue persists

Please provide more details about your specific issue for targeted assistance.";

        return Task.FromResult(prompt);
    }

    [McpServerPrompt]
    [Description("Template for implementing responsive design in AvaloniaUI")]
    public static Task<string> ResponsiveDesignPrompt(
        [Description("Target screen sizes or device types")] string targetDevices,
        [Description("Layout requirements or constraints")] string layoutRequirements)
    {
        var prompt = $@"# Implement Responsive Design in AvaloniaUI

## Design Requirements
- **Target Devices:** {targetDevices}
- **Layout Requirements:** {layoutRequirements}

## Responsive Design Strategy

### 1. Layout Approach
- Use flexible layout containers (Grid with * sizing, StackPanel)
- Implement adaptive layouts that respond to screen size changes
- Consider different orientations (portrait/landscape)

### 2. Screen Size Adaptation
```xml
<!-- Responsive Grid Layout -->
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height=""Auto"" />
        <RowDefinition Height=""*"" />
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width=""*"" MinWidth=""200"" />
        <ColumnDefinition Width=""2*"" />
    </Grid.ColumnDefinitions>
    
    <!-- Adaptive content here -->
</Grid>
```

### 3. Platform-Specific Considerations
{GetPlatformSpecificConsiderations(targetDevices)}

### 4. Control Sizing
- Use relative sizing (*, Auto) instead of fixed pixels
- Set MinWidth/MinHeight for essential elements
- Use Margin and Padding for spacing

### 5. Typography and Icons
- Use scalable fonts and icon fonts
- Consider different DPI settings
- Test readability across devices

### 6. Touch and Input
- Ensure touch targets are appropriately sized (44px minimum)
- Consider keyboard navigation
- Implement proper focus management

## Implementation Guidelines
1. Start with flexible layout containers
2. Use data binding for dynamic content sizing
3. Test on different screen sizes early and often
4. Consider using different views for dramatically different form factors
5. Implement proper scrolling for content overflow

Please proceed with implementing responsive design following these guidelines.";

        return Task.FromResult(prompt);
    }

    private static string GetPlatformConsiderations(string targetPlatforms)
    {
        return targetPlatforms.ToLower() switch
        {
            "desktop" => @"
**Desktop Platforms:**
- Windows: Full feature support, native window chrome
- macOS: Native menu bar integration, proper window behavior
- Linux: Various window managers, theme integration",

            "mobile" => @"
**Mobile Platforms:**
- Android: Touch optimization, back button handling
- iOS: Safe area considerations, platform UI guidelines
- Consider different screen sizes and orientations",

            "all" => @"
**Cross-Platform Considerations:**
- Responsive design for different screen sizes
- Platform-specific UI patterns and behaviors
- Input method adaptation (touch, mouse, keyboard)
- Platform-specific resources and assets",

            _ => "Consider the specific requirements of your target platforms."
        };
    }

    private static string GetDiagnosticSteps(string issueType)
    {
        return issueType.ToLower() switch
        {
            "binding" => @"
1. Check binding syntax for typos
2. Verify DataContext is set correctly
3. Ensure property names match exactly (case-sensitive)
4. Check if properties implement INotifyPropertyChanged
5. Verify namespace imports in XAML
6. Use debug binding traces",

            "layout" => @"
1. Check container sizes and constraints
2. Verify row/column definitions in Grid
3. Check for conflicting layout properties
4. Ensure proper margin and padding values
5. Verify content doesn't exceed container bounds
6. Test with different window sizes",

            "styling" => @"
1. Check selector syntax (use Selector not TargetType)
2. Verify resource keys and references
3. Check for conflicting styles
4. Ensure styles are in proper resource scope
5. Verify property names in setters
6. Test style inheritance",

            "performance" => @"
1. Profile app startup and runtime performance
2. Check for binding performance issues
3. Look for memory leaks in ViewModels
4. Verify efficient data structures usage
5. Check for unnecessary UI updates
6. Monitor resource usage",

            _ => @"
1. Reproduce the issue consistently
2. Check console output for errors
3. Verify XAML syntax is correct
4. Test on different platforms
5. Check for recent changes that might have caused the issue"
        };
    }

    private static string GetCommonSolutions(string issueType)
    {
        return issueType.ToLower() switch
        {
            "binding" => @"
- Use x:DataType for compiled bindings
- Implement INotifyPropertyChanged properly
- Check binding mode (OneWay, TwoWay, OneTime)
- Verify converter usage if applicable
- Use $parent syntax for ancestor bindings",

            "layout" => @"
- Use appropriate layout containers for your scenario
- Set proper sizing (*, Auto, fixed values)
- Check HorizontalAlignment and VerticalAlignment
- Verify Grid row/column spanning
- Consider using ScrollViewer for overflow content",

            "styling" => @"
- Use correct selector syntax: Selector=""Button.myClass""
- Define styles in proper resource scope
- Check style precedence and inheritance
- Use StaticResource for resource references
- Consider using style classes instead of direct styling",

            "performance" => @"
- Use compiled bindings with x:DataType
- Implement virtualization for large lists
- Avoid complex binding expressions
- Use async patterns for data loading
- Optimize image loading and caching",

            _ => @"
- Check AvaloniaUI documentation
- Search community forums and GitHub issues
- Use debugging tools and logging
- Test isolated scenarios
- Update to latest AvaloniaUI version"
        };
    }

    private static string GetPlatformSpecificConsiderations(string targetDevices)
    {
        return targetDevices.ToLower() switch
        {
            var devices when devices.Contains("mobile") => @"
**Mobile Considerations:**
- Touch-friendly controls (minimum 44px touch targets)
- Orientation changes and screen rotation
- Different screen densities and sizes
- Platform-specific navigation patterns
- Keyboard handling and virtual keyboards",

            var devices when devices.Contains("desktop") => @"
**Desktop Considerations:**
- Window resizing and state management
- Keyboard shortcuts and accelerators
- Context menus and right-click behavior
- Multiple monitor support
- Platform-specific window chrome",

            _ => @"
**General Considerations:**
- Flexible layouts that adapt to different screen sizes
- Scalable fonts and UI elements
- Platform-appropriate input methods
- Consistent user experience across platforms"
        };
    }

    [McpServerPrompt]
    [Description("Template for implementing container queries for responsive design in AvaloniaUI 11.3+")]
    public static Task<string> ContainerQueryPrompt(
        [Description("Component name to make responsive")] string componentName,
        [Description("Breakpoints for responsive behavior (comma-separated widths in px)")] string breakpoints = "300,600,900")
    {
        var breakpointList = breakpoints.Split(',').Select(b => b.Trim()).ToList();
        
        var prompt = $@"# Implement Container Queries for {componentName}

## Responsive Design with Container Queries (AvaloniaUI 11.3+)

### Target Breakpoints
{string.Join("\n", breakpointList.Select((bp, i) => $"- **{bp}px**: Breakpoint {i + 1}"))}

### Implementation
```xml
<Border Name=""{componentName}Container"">
    <Border.Styles>
        <Style Selector=""Border"">
            <Setter Property=""Padding"" Value=""8"" />
        </Style>
        
{string.Join("\n", breakpointList.Select(bp => $"        <Style Selector=\"@container(min-width: {bp}px) Border\">\n            <Setter Property=\"Padding\" Value=\"{Math.Max(8, int.Parse(bp) / 25)}\" />\n        </Style>"))}
    </Border.Styles>
    
    <StackPanel>
        <TextBlock Text=""{componentName}"" FontWeight=""Bold"" />
        <TextBlock Text=""Responsive content"" />
    </StackPanel>
</Border>
```

### Best Practices
- Use container queries for component-level responsiveness
- Test at various container sizes
- Combine with modern CSS-like selectors
- Monitor performance with complex queries

Please implement container queries for {componentName} following these guidelines.";

        return Task.FromResult(prompt);
    }

    [McpServerPrompt]
    [Description("Template for optimizing AvaloniaUI application performance")]
    public static Task<string> PerformanceOptimizationPrompt(
        [Description("Performance area to focus on: 'startup', 'rendering', 'memory', 'binding', 'collections'")] string focusArea = "general",
        [Description("Target platforms: 'desktop', 'mobile', 'all'")] string targetPlatforms = "desktop")
    {
        var prompt = $@"# AvaloniaUI Performance Optimization Strategy

## Focus Area: {focusArea.ToUpperInvariant()}
## Target Platforms: {targetPlatforms}

### Key Optimization Areas

#### 1. Compiled Bindings (Critical)
```xml
<UserControl x:DataType=""vm:UserViewModel"">
    <TextBlock Text=""{{Binding UserName}}"" />
</UserControl>
```

#### 2. Efficient Resource Usage
```xml
<Button Background=""{{StaticResource PrimaryBrush}}"" />
```

#### 3. Layout Optimization
```xml
<Grid RowSpacing=""8"" ColumnSpacing=""12"">
    <Grid.RowDefinitions>
        <RowDefinition Height=""Auto"" />
        <RowDefinition Height=""*"" />
    </Grid.RowDefinitions>
</Grid>
```

#### 4. Virtualization for Collections
```xml
<ListBox VirtualizingStackPanel.IsVirtualizing=""True""
         VirtualizingStackPanel.VirtualizationMode=""Recycling"">
```

### Measurement and Monitoring
- Profile before optimizing
- Use dotTrace, PerfView, or similar tools
- Monitor key metrics: startup time, memory usage, frame rate
- Test on target devices

### Implementation Checklist
- [ ] Implement compiled bindings
- [ ] Optimize resource usage
- [ ] Review layout hierarchy
- [ ] Enable virtualization for lists
- [ ] Test on target platforms

Please implement these optimizations based on your specific performance requirements for {focusArea}.";

        return Task.FromResult(prompt);
    }
}