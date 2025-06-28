using System.ComponentModel;

using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Tools;

[McpServerToolType]
public static class CustomControlGenerator
{
    [McpServerTool, Description("Generates professional custom controls including templated controls, user controls, and attached properties")]
    public static string GenerateCustomControl(
        [Description("Control type: 'templated', 'usercontrol', 'panel', 'attached-property'")] string controlType,
        [Description("Control name")] string controlName,
        [Description("Base class or functionality description")] string baseClass = "Control",
        [Description("Properties to include (comma-separated)")] string properties = "",
        [Description("Include styling template: 'true' or 'false'")] string includeTemplate = "true")
    {
        try
        {
            var config = new CustomControlConfiguration
            {
                Type = controlType.ToLowerInvariant(),
                Name = controlName,
                BaseClass = baseClass,
                Properties = [.. properties.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim())],
                IncludeTemplate = bool.Parse(includeTemplate)
            };

            string result = config.Type switch
            {
                "templated" => GenerateTemplatedControl(config),
                "usercontrol" => GenerateUserControl(config),
                "panel" => GenerateCustomPanel(config),
                "attached-property" => GenerateAttachedProperty(config),
                _ => throw new ArgumentException($"Unknown control type: {controlType}")
            };

            return result;
        }
        catch (Exception ex)
        {
            return $"Error generating custom control: {ex.Message}";
        }
    }

    [McpServerTool, Description("Creates complex control templates with visual states and triggers")]
    public static string GenerateControlTemplate(
        [Description("Target control type")] string targetControl,
        [Description("Template name")] string templateName,
        [Description("Visual states (comma-separated, e.g., Normal,PointerOver,Pressed,Disabled)")] string visualStates = "Normal,PointerOver,Pressed,Disabled",
        [Description("Include animations: 'true' or 'false'")] string includeAnimations = "true")
    {
        try
        {
            var states = visualStates.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
            bool animations = bool.Parse(includeAnimations);

            string template = GenerateComplexControlTemplate(targetControl, templateName, states, animations);
            string styleCode = GenerateTemplateStyle(targetControl, templateName);
            string usageExample = GenerateTemplateUsage(targetControl, templateName);

            return $@"# Custom Control Template: {templateName} for {targetControl}

## Template Definition
```xml
{template}
```

## Style Integration
```xml
{styleCode}
```

## Usage Example
```xml
{usageExample}
```

## Visual State Management

### State Transitions
The template includes smooth transitions between the following states:
{string.Join("\n", states.Select(s => $"- **{s}**: {GetStateDescription(s)}"))}

### Custom Visual States
```xml
<VisualStateGroup x:Name=""CustomStates"">
    <VisualState x:Name=""Loading"">
        <Storyboard>
            <DoubleAnimation Storyboard.TargetName=""LoadingIndicator""
                             Storyboard.TargetProperty=""Opacity""
                             To=""1"" Duration=""0:0:0.2"" />
        </Storyboard>
    </VisualState>
    <VisualState x:Name=""Success"">
        <Storyboard>
            <ColorAnimation Storyboard.TargetName=""Background""
                            Storyboard.TargetProperty=""Color""
                            To=""Green"" Duration=""0:0:0.3"" />
        </Storyboard>
    </VisualState>
</VisualStateGroup>
```

## Template Parts

### Required Parts
- **PART_ContentPresenter**: Main content display
- **PART_Border**: Visual boundary
- **PART_Background**: Background element

### Optional Parts
- **PART_Icon**: Icon display area
- **PART_LoadingIndicator**: Loading state indicator
- **PART_FocusVisual**: Focus indication

## Customization Guide

### Colors and Brushes
```xml
<Style.Resources>
    <SolidColorBrush x:Key=""Primary"" Color=""#007ACC"" />
    <SolidColorBrush x:Key=""PrimaryHover"" Color=""#005A9E"" />
    <SolidColorBrush x:Key=""PrimaryPressed"" Color=""#004578"" />
</Style.Resources>
```

### Typography
```xml
<Setter Property=""FontFamily"" Value=""{{StaticResource InterFont}}"" />
<Setter Property=""FontSize"" Value=""14"" />
<Setter Property=""FontWeight"" Value=""Normal"" />
```

### Spacing and Layout
```xml
<Setter Property=""Padding"" Value=""12,8"" />
<Setter Property=""Margin"" Value=""4"" />
<Setter Property=""MinHeight"" Value=""32"" />
```";
        }
        catch (Exception ex)
        {
            return $"Error generating control template: {ex.Message}";
        }
    }

    [McpServerTool, Description("Generates attached properties for extending existing controls")]
    public static string GenerateAttachedProperty(
        [Description("Property name")] string propertyName,
        [Description("Property type: 'string', 'bool', 'int', 'double', 'object'")] string propertyType,
        [Description("Target control types (comma-separated)")] string targetControls = "Control",
        [Description("Include change handler: 'true' or 'false'")] string includeHandler = "true")
    {
        try
        {
            var targets = targetControls.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList();
            bool handler = bool.Parse(includeHandler);

            string attachedPropertyCode = GenerateAttachedPropertyCode(propertyName, propertyType, targets, handler);
            string usageExamples = GenerateAttachedPropertyUsage(propertyName, propertyType, targets);
            string stylingIntegration = GenerateAttachedPropertyStyling(propertyName, targets);

            return $@"# Attached Property: {propertyName}

## Property Definition
```csharp
{attachedPropertyCode}
```

## Usage Examples
```xml
{usageExamples}
```

## Styling Integration
```xml
{stylingIntegration}
```

## Advanced Usage Patterns

### Conditional Styling
```xml
<Style Selector=""Button[local:{propertyName}=true]"">
    <Setter Property=""Background"" Value=""SpecialBrush"" />
</Style>
```

### Behavior Attachment
```csharp
public static void SetBehavior(Control element, IBehavior behavior)
{{
    if (Get{propertyName}(element))
    {{
        behavior.Attach(element);
    }}
}}
```

### Data-Driven Properties
```xml
<Button local:{propertyName}=""{{Binding IsSpecial}}"" />
```

## Performance Considerations
- Attached properties are stored in a global dictionary
- Consider using direct properties for frequently accessed values
- Implement efficient change detection for complex scenarios
- Use weak references for event handlers to prevent memory leaks";
        }
        catch (Exception ex)
        {
            return $"Error generating attached property: {ex.Message}";
        }
    }

    [McpServerTool, Description("Creates custom layout panels with sophisticated arrangement logic")]
    public static string GenerateLayoutPanel(
        [Description("Panel name")] string panelName,
        [Description("Layout strategy: 'flow', 'circular', 'masonry', 'star', 'custom'")] string layoutStrategy = "flow",
        [Description("Include virtualization: 'true' or 'false'")] string includeVirtualization = "false",
        [Description("Support for attached properties: 'true' or 'false'")] string supportAttached = "true")
    {
        try
        {
            bool virtualization = bool.Parse(includeVirtualization);
            bool attachedProps = bool.Parse(supportAttached);

            string panelCode = GenerateLayoutPanelCode(panelName, layoutStrategy, virtualization, attachedProps);
            string usageExample = GenerateLayoutPanelUsage(panelName, layoutStrategy);
            string attachedProperties = attachedProps ? GenerateLayoutAttachedProperties(panelName, layoutStrategy) : "";

            return $@"# Custom Layout Panel: {panelName}

## Panel Implementation
```csharp
{panelCode}
```

{(attachedProps ? $@"## Attached Properties
```csharp
{attachedProperties}
```" : "")}

## Usage Example
```xml
{usageExample}
```

## Layout Strategy: {layoutStrategy}

### Key Features
{GetLayoutStrategyFeatures(layoutStrategy)}

### Performance Characteristics
{GetLayoutPerformanceInfo(layoutStrategy, virtualization)}

## Advanced Customization

### Custom Arrangement Logic
```csharp
protected override Size ArrangeOverride(Size finalSize)
{{
    // Custom arrangement implementation
    var currentPosition = new Point(0, 0);
    
    foreach (Control child in Children)
    {{
        if (child.IsVisible)
        {{
            var childSize = CalculateChildSize(child, finalSize);
            var childPosition = CalculateChildPosition(child, currentPosition, childSize);
            
            child.Arrange(new Rect(childPosition, childSize));
            currentPosition = UpdatePosition(currentPosition, childSize);
        }}
    }}
    
    return finalSize;
}}
```

### Virtualization Implementation
{(virtualization ? @"```csharp
private readonly Dictionary<int, Control> _realizedChildren = new();
private readonly Queue<Control> _recycledChildren = new();

private Control GetOrCreateChild(int index)
{
    if (_realizedChildren.TryGetValue(index, out var child))
        return child;
        
    if (_recycledChildren.Count > 0)
    {
        child = _recycledChildren.Dequeue();
        UpdateChildContent(child, index);
    }
    else
    {
        child = CreateChildControl(index);
    }
    
    _realizedChildren[index] = child;
    return child;
}
```" : "// Virtualization not enabled for this panel")}

## Integration with Data Binding
```xml
<local:{panelName} ItemsSource=""{{Binding Items}}"">
    <local:{panelName}.ItemTemplate>
        <DataTemplate>
            <Border Background=""LightBlue"" Margin=""2"">
                <TextBlock Text=""{{Binding}}"" />
            </Border>
        </DataTemplate>
    </local:{panelName}.ItemTemplate>
</local:{panelName}>
```";
        }
        catch (Exception ex)
        {
            return $"Error generating layout panel: {ex.Message}";
        }
    }

    private sealed class CustomControlConfiguration
    {
        public string Type { get; set; } = "";
        public string Name { get; set; } = "";
        public string BaseClass { get; set; } = "";
        public List<string> Properties { get; set; } = new();
        public bool IncludeTemplate { get; set; }
    }

    private static string GenerateTemplatedControl(CustomControlConfiguration config)
    {
        string controlCode = GenerateTemplatedControlCode(config);
        string templateXaml = config.IncludeTemplate ? GenerateDefaultTemplate(config) : "";
        string styleXaml = GenerateControlStyle(config);
        string propertyBindings = string.Join("\n    ", config.Properties.Select(p => $"{p}=\"{{Binding {p}}}\""));
        string templateParts = string.Join("\n", GetTemplateParts(config).Select(part => $"- **{part.Name}**: {part.Description}"));
        string visualStates = string.Join("\n", GetVisualStates(config).Select(state => $"- **{state}**: Standard visual state"));

        return $@"# Templated Control: {config.Name}

## Control Implementation
```csharp
{controlCode}
```

{(config.IncludeTemplate ? $@"## Default Template
```xml
{templateXaml}
```" : "")}

## Control Style
```xml
{styleXaml}
```

## Usage Example
```xml
<local:{config.Name} 
    {propertyBindings}
    />
```

## Template Parts
The control defines the following template parts:
{templateParts}

## Visual States
{visualStates}

## Customization
Override the default template by creating a new style:
```xml
<Style TargetType=""local:{config.Name}"">
    <Setter Property=""Template"">
        <ControlTemplate>
            <!-- Custom template here -->
        </ControlTemplate>
    </Setter>
</Style>
```";
    }

    private static string GenerateUserControl(CustomControlConfiguration config)
    {
        string xamlCode = GenerateUserControlXaml(config);
        string codeCode = GenerateUserControlCode(config);
        string propertyBindings = string.Join("\n    ", config.Properties.Select(p => $"{p}=\"{{Binding {p}}}\""));

        return $@"# User Control: {config.Name}

## XAML Definition
```xml
{xamlCode}
```

## Code-Behind
```csharp
{codeCode}
```

## Usage Example
```xml
<local:{config.Name} 
    {propertyBindings}
    />
```

## Data Binding Integration
The UserControl supports full data binding through dependency properties and can be used with MVVM patterns seamlessly.

## Styling
Style the UserControl using standard AvaloniaUI styling:
```xml
<Style Selector=""local:{config.Name}"">
    <Setter Property=""Background"" Value=""LightGray"" />
    <Setter Property=""Padding"" Value=""10"" />
</Style>
```";
    }

    private static string GenerateCustomPanel(CustomControlConfiguration config)
    {
        return GenerateLayoutPanel(config.Name, "custom", "false", "true");
    }

    private static string GenerateAttachedProperty(CustomControlConfiguration config)
    {
        if (config.Properties.Count == 0)
        {
            throw new ArgumentException("Attached property requires at least one property definition");
        }

        string propertyName = config.Properties.First();
        return GenerateAttachedProperty(propertyName, "object", config.BaseClass, "true");
    }

    private static string GenerateTemplatedControlCode(CustomControlConfiguration config)
    {
        string properties = string.Join("\n\n", config.Properties.Select(GenerateControlProperty));

        return $@"using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace YourApp.Controls;

public class {config.Name} : {config.BaseClass}
{{
    static {config.Name}()
    {{
        DefaultStyleKeyProperty.OverrideMetadata(typeof({config.Name}), new FuncStyleKeyMetadata<{config.Name}>(x => typeof({config.Name})));
    }}

{properties}

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {{
        base.OnApplyTemplate(e);
        
        // Get template parts
{string.Join("\n", GetTemplateParts(config).Select(part => $"        var {part.Name.ToLowerInvariant()} = e.NameScope.Find<{part.Type}>(\"{part.Name}\");"))}
        
        // Initialize control
        InitializeControl();
    }}

    private void InitializeControl()
    {{
        // Initialize control logic here
    }}

{string.Join("\n\n", config.Properties.Select(GeneratePropertyChangedHandler))}
}}";
    }

    private static string GenerateDefaultTemplate(CustomControlConfiguration config)
    {
        return $@"<Style Selector=""local|{config.Name}"">
    <Setter Property=""Template"">
        <ControlTemplate>
            <Border Name=""PART_Border""
                    Background=""{{TemplateBinding Background}}""
                    BorderBrush=""{{TemplateBinding BorderBrush}}""
                    BorderThickness=""{{TemplateBinding BorderThickness}}""
                    CornerRadius=""4"">
                
                <Grid>
                    <ContentPresenter Name=""PART_ContentPresenter""
                                      Content=""{{TemplateBinding Content}}""
                                      ContentTemplate=""{{TemplateBinding ContentTemplate}}""
                                      Margin=""{{TemplateBinding Padding}}"" />
                </Grid>
            </Border>
        </ControlTemplate>
    </Setter>
</Style>";
    }

    private static string GenerateControlStyle(CustomControlConfiguration config)
    {
        return $@"<Style Selector=""local|{config.Name}"">
    <Setter Property=""Background"" Value=""{{DynamicResource ButtonBackground}}"" />
    <Setter Property=""BorderBrush"" Value=""{{DynamicResource ButtonBorderBrush}}"" />
    <Setter Property=""BorderThickness"" Value=""1"" />
    <Setter Property=""Padding"" Value=""8,4"" />
    <Setter Property=""Margin"" Value=""2"" />
    <Setter Property=""HorizontalContentAlignment"" Value=""Center"" />
    <Setter Property=""VerticalContentAlignment"" Value=""Center"" />
</Style>

<Style Selector=""local|{config.Name}:pointerover"">
    <Setter Property=""Background"" Value=""{{DynamicResource ButtonBackgroundPointerOver}}"" />
</Style>

<Style Selector=""local|{config.Name}:pressed"">
    <Setter Property=""Background"" Value=""{{DynamicResource ButtonBackgroundPressed}}"" />
</Style>";
    }

    private static string GenerateUserControlXaml(CustomControlConfiguration config)
    {
        return $@"<UserControl xmlns=""https://github.com/avaloniaui""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             x:Class=""YourApp.Controls.{config.Name}"">
    
    <Border Background=""{{Binding Background, RelativeSource={{RelativeSource AncestorType=UserControl}}}}""
            BorderBrush=""{{Binding BorderBrush, RelativeSource={{RelativeSource AncestorType=UserControl}}}}""
            BorderThickness=""{{Binding BorderThickness, RelativeSource={{RelativeSource AncestorType=UserControl}}}}""
            Padding=""{{Binding Padding, RelativeSource={{RelativeSource AncestorType=UserControl}}}}"">
        
        <StackPanel>
{string.Join("\n", config.Properties.Select(p => $"            <TextBlock Text=\"{{Binding {p}, RelativeSource={{RelativeSource AncestorType=UserControl}}}}\" />"))}
            
            <!-- Add your custom UI here -->
            <ContentPresenter Content=""{{Binding Content, RelativeSource={{RelativeSource AncestorType=UserControl}}}}"" />
        </StackPanel>
    </Border>
</UserControl>";
    }

    private static string GenerateUserControlCode(CustomControlConfiguration config)
    {
        string properties = string.Join("\n\n", config.Properties.Select(GenerateUserControlProperty));

        return $@"using Avalonia;
using Avalonia.Controls;

namespace YourApp.Controls;

public partial class {config.Name} : UserControl
{{
{properties}

    public {config.Name}()
    {{
        InitializeComponent();
    }}
}}";
    }

    private static string GenerateControlProperty(string propertyName)
    {
        string typeName = InferPropertyType(propertyName);
        return $@"    public static readonly StyledProperty<{typeName}> {propertyName}Property =
        AvaloniaProperty.Register<{propertyName}, {typeName}>(nameof({propertyName}));

    public {typeName} {propertyName}
    {{
        get => GetValue({propertyName}Property);
        set => SetValue({propertyName}Property, value);
    }}";
    }

    private static string GenerateUserControlProperty(string propertyName)
    {
        string typeName = InferPropertyType(propertyName);
        return $@"    public static readonly StyledProperty<{typeName}> {propertyName}Property =
        AvaloniaProperty.Register<{propertyName}, {typeName}>(nameof({propertyName}));

    public {typeName} {propertyName}
    {{
        get => GetValue({propertyName}Property);
        set => SetValue({propertyName}Property, value);
    }}";
    }

    private static string GeneratePropertyChangedHandler(string propertyName)
    {
        return $@"    private static void On{propertyName}Changed(AvaloniaPropertyChangedEventArgs e)
    {{
        if (e.Sender is {propertyName} control)
        {{
            control.Handle{propertyName}Changed(e.OldValue, e.NewValue);
        }}
    }}

    private void Handle{propertyName}Changed(object oldValue, object newValue)
    {{
        // Handle property change
    }}";
    }

    private static string InferPropertyType(string propertyName)
    {
        string lowerName = propertyName.ToLowerInvariant();

        if (lowerName.Contains("text") || lowerName.Contains("title") || lowerName.Contains("name"))
        {
            return "string";
        }

        if (lowerName.Contains("count") || lowerName.Contains("index") || lowerName.Contains("number"))
        {
            return "int";
        }

        return lowerName.Contains("is") || lowerName.Contains("can") || lowerName.Contains("has")
            ? "bool"
            : lowerName.Contains("width") || lowerName.Contains("height") || lowerName.Contains("size")
            ? "double"
            : lowerName.Contains("color") || lowerName.Contains("brush") ? "IBrush" : "object";
    }

    private static List<(string Name, string Type, string Description)> GetTemplateParts(CustomControlConfiguration config)
    {
        return new List<(string, string, string)>
        {
            ("PART_Border", "Border", "Main border element"),
            ("PART_ContentPresenter", "ContentPresenter", "Content display area"),
            ("PART_Background", "Panel", "Background element")
        };
    }

    private static List<string> GetVisualStates(CustomControlConfiguration config)
    {
        return new List<string> { "Normal", "PointerOver", "Pressed", "Disabled", "Focused" };
    }

    private static string GenerateComplexControlTemplate(string targetControl, string templateName, List<string> states, bool animations)
    {
        string stateGroups = GenerateVisualStateGroups(states, animations);

        return $@"<ControlTemplate x:Key=""{templateName}"" TargetType=""{targetControl}"">
    <Border Name=""PART_Border""
            Background=""{{TemplateBinding Background}}""
            BorderBrush=""{{TemplateBinding BorderBrush}}""
            BorderThickness=""{{TemplateBinding BorderThickness}}""
            CornerRadius=""4"">
        
        <VisualStateManager.VisualStateGroups>
{stateGroups}
        </VisualStateManager.VisualStateGroups>
        
        <Grid>
            <ContentPresenter Name=""PART_ContentPresenter""
                              Content=""{{TemplateBinding Content}}""
                              ContentTemplate=""{{TemplateBinding ContentTemplate}}""
                              HorizontalAlignment=""{{TemplateBinding HorizontalContentAlignment}}""
                              VerticalAlignment=""{{TemplateBinding VerticalContentAlignment}}""
                              Margin=""{{TemplateBinding Padding}}"" />
        </Grid>
    </Border>
</ControlTemplate>";
    }

    private static string GenerateVisualStateGroups(List<string> states, bool animations)
    {
        string stateDefinitions = string.Join("\n", states.Select(state => GenerateVisualState(state, animations)));

        return $@"            <VisualStateGroup x:Name=""CommonStates"">
{stateDefinitions}
            </VisualStateGroup>";
    }

    private static string GenerateVisualState(string stateName, bool animations)
    {
        string storyboard = animations ? GenerateStateStoryboard(stateName) : "";

        return $@"                <VisualState x:Name=""{stateName}"">
{storyboard}
                </VisualState>";
    }

    private static string GenerateStateStoryboard(string stateName)
    {
        return stateName.ToLowerInvariant() switch
        {
            "pointerover" => @"                    <Storyboard>
                        <DoubleAnimation Storyboard.TargetName=""PART_Border""
                                         Storyboard.TargetProperty=""Opacity""
                                         To=""0.8"" Duration=""0:0:0.1"" />
                    </Storyboard>",

            "pressed" => @"                    <Storyboard>
                        <DoubleAnimation Storyboard.TargetName=""PART_Border""
                                         Storyboard.TargetProperty=""(ScaleTransform.ScaleX)""
                                         To=""0.95"" Duration=""0:0:0.05"" />
                        <DoubleAnimation Storyboard.TargetName=""PART_Border""
                                         Storyboard.TargetProperty=""(ScaleTransform.ScaleY)""
                                         To=""0.95"" Duration=""0:0:0.05"" />
                    </Storyboard>",

            _ => ""
        };
    }

    private static string GenerateTemplateStyle(string targetControl, string templateName)
    {
        return $@"<Style Selector=""{targetControl}"">
    <Setter Property=""Template"" Value=""{{StaticResource {templateName}}}"" />
</Style>";
    }

    private static string GenerateTemplateUsage(string targetControl, string templateName)
    {
        return $@"<!-- Apply template to specific control -->
<{targetControl} Template=""{{StaticResource {templateName}}}"" Content=""Custom Styled Control"" />

<!-- Apply via style -->
<{targetControl} Classes=""custom-template"" Content=""Styled Control"" />

<Style Selector=""{targetControl}.custom-template"">
    <Setter Property=""Template"" Value=""{{StaticResource {templateName}}}"" />
</Style>";
    }

    private static string GetStateDescription(string state)
    {
        return state switch
        {
            "Normal" => "Default state",
            "PointerOver" => "Mouse hover state",
            "Pressed" => "Button press state",
            "Disabled" => "Disabled control state",
            "Focused" => "Keyboard focus state",
            _ => "Custom state"
        };
    }

    private static string GenerateAttachedPropertyCode(string propertyName, string propertyType, List<string> targets, bool includeHandler)
    {
        string dotNetType = ConvertPropertyType(propertyType);
        string handlerCode = includeHandler ? GenerateAttachedPropertyHandler(propertyName, dotNetType) : "";

        return $@"using Avalonia;
using Avalonia.Controls;

namespace YourApp.AttachedProperties;

public static class {propertyName}Extensions
{{
    public static readonly AttachedProperty<{dotNetType}> {propertyName}Property =
        AvaloniaProperty.RegisterAttached<{propertyName}Extensions, Control, {dotNetType}>(
            ""{propertyName}""{(includeHandler ? $", defaultValue: default({dotNetType}), coerce: Coerce{propertyName}" : "")});

    public static {dotNetType} Get{propertyName}(Control element)
    {{
        return element.GetValue({propertyName}Property);
    }}

    public static void Set{propertyName}(Control element, {dotNetType} value)
    {{
        element.SetValue({propertyName}Property, value);
    }}

{handlerCode}
}}";
    }

    private static string GenerateAttachedPropertyHandler(string propertyName, string dotNetType)
    {
        return $@"    private static {dotNetType} Coerce{propertyName}(AvaloniaObject sender, {dotNetType} value)
    {{
        if (sender is Control control)
        {{
            On{propertyName}Changed(control, value);
        }}
        return value;
    }}

    private static void On{propertyName}Changed(Control element, {dotNetType} newValue)
    {{
        // Handle property change
        switch (newValue)
        {{
            case bool boolValue when boolValue:
                element.Classes.Add(""{propertyName.ToLowerInvariant()}-enabled"");
                break;
            case bool boolValue when !boolValue:
                element.Classes.Remove(""{propertyName.ToLowerInvariant()}-enabled"");
                break;
            default:
                // Handle other types
                break;
        }}
    }}";
    }

    private static string GenerateAttachedPropertyUsage(string propertyName, string propertyType, List<string> targets)
    {
        string examples = string.Join("\n", targets.Select(target =>
            $"<{target} local:{propertyName}=\"{GetExampleValue(propertyType)}\" />"));

        return $@"<!-- Basic usage -->
{examples}

<!-- Data binding -->
<Button local:{propertyName}=""{{Binding Is{propertyName}}}"" />

<!-- In code-behind -->
<Button x:Name=""MyButton"" />
<!-- Set in code: {propertyName}Extensions.Set{propertyName}(MyButton, {GetExampleValue(propertyType)}); -->";
    }

    private static string GenerateAttachedPropertyStyling(string propertyName, List<string> targets)
    {
        return $@"<!-- Style based on attached property -->
<Style Selector=""Button[local:{propertyName}=true]"">
    <Setter Property=""Background"" Value=""SpecialBrush"" />
    <Setter Property=""FontWeight"" Value=""Bold"" />
</Style>

<!-- Conditional styling -->
<Style Selector=""Control[local:{propertyName}]"">
    <Setter Property=""Opacity"" Value=""0.8"" />
</Style>";
    }

    private static string GenerateLayoutPanelCode(string panelName, string layoutStrategy, bool virtualization, bool attachedProps)
    {
        string baseClass = virtualization ? "VirtualizingPanel" : "Panel";
        string attachedPropsCode = attachedProps ? GenerateLayoutAttachedProperties(panelName, layoutStrategy) : "";

        return $@"using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace YourApp.Panels;

public class {panelName} : {baseClass}
{{
{attachedPropsCode}

    protected override Size MeasureOverride(Size availableSize)
    {{
        var totalSize = new Size();
        
        foreach (Control child in Children)
        {{
            child.Measure(availableSize);
            var childDesiredSize = child.DesiredSize;
            
            // Apply layout strategy
            var adjustedSize = Apply{layoutStrategy}Measure(child, childDesiredSize, availableSize);
            totalSize = CombineSizes(totalSize, adjustedSize);
        }}
        
        return totalSize;
    }}

    protected override Size ArrangeOverride(Size finalSize)
    {{
        var currentPosition = new Point(0, 0);
        
        foreach (Control child in Children)
        {{
            if (child.IsVisible)
            {{
                var childSize = child.DesiredSize;
                var childRect = Calculate{layoutStrategy}Position(child, currentPosition, childSize, finalSize);
                
                child.Arrange(childRect);
                currentPosition = UpdatePosition(currentPosition, childRect, finalSize);
            }}
        }}
        
        return finalSize;
    }}

    private Size Apply{layoutStrategy}Measure(Control child, Size childSize, Size availableSize)
    {{
        return layoutStrategy switch
        {{
            ""flow"" => childSize,
            ""circular"" => new Size(Math.Min(childSize.Width, availableSize.Width / Children.Count), childSize.Height),
            ""masonry"" => childSize,
            _ => childSize
        }};
    }}

    private Rect Calculate{layoutStrategy}Position(Control child, Point currentPosition, Size childSize, Size finalSize)
    {{
        return layoutStrategy switch
        {{
            ""flow"" => CalculateFlowPosition(currentPosition, childSize, finalSize),
            ""circular"" => CalculateCircularPosition(child, childSize, finalSize),
            ""masonry"" => CalculateMasonryPosition(child, currentPosition, childSize, finalSize),
            _ => new Rect(currentPosition, childSize)
        }};
    }}

    private Rect CalculateFlowPosition(Point currentPosition, Size childSize, Size finalSize)
    {{
        if (currentPosition.X + childSize.Width > finalSize.Width)
        {{
            currentPosition = new Point(0, currentPosition.Y + childSize.Height);
        }}
        
        return new Rect(currentPosition, childSize);
    }}

    private Rect CalculateCircularPosition(Control child, Size childSize, Size finalSize)
    {{
        var center = new Point(finalSize.Width / 2, finalSize.Height / 2);
        var radius = Math.Min(finalSize.Width, finalSize.Height) * 0.3;
        var angle = (Children.IndexOf(child) * 2 * Math.PI) / Children.Count;
        
        var x = center.X + Math.Cos(angle) * radius - childSize.Width / 2;
        var y = center.Y + Math.Sin(angle) * radius - childSize.Height / 2;
        
        return new Rect(new Point(x, y), childSize);
    }}

    private Rect CalculateMasonryPosition(Control child, Point currentPosition, Size childSize, Size finalSize)
    {{
        // Simplified masonry layout
        return new Rect(currentPosition, childSize);
    }}

    private Point UpdatePosition(Point currentPosition, Rect childRect, Size finalSize)
    {{
        return new Point(childRect.Right, currentPosition.Y);
    }}

    private Size CombineSizes(Size totalSize, Size childSize)
    {{
        return new Size(
            Math.Max(totalSize.Width, childSize.Width),
            totalSize.Height + childSize.Height);
    }}
}}";
    }

    private static string GenerateLayoutAttachedProperties(string panelName, string layoutStrategy)
    {
        return layoutStrategy switch
        {
            "flow" => @"    public static readonly AttachedProperty<bool> BreakLineProperty =
        AvaloniaProperty.RegisterAttached<FlowPanel, Control, bool>(""BreakLine"");

    public static bool GetBreakLine(Control element) => element.GetValue(BreakLineProperty);
    public static void SetBreakLine(Control element, bool value) => element.SetValue(BreakLineProperty, value);",

            "circular" => @"    public static readonly AttachedProperty<double> RadiusOffsetProperty =
        AvaloniaProperty.RegisterAttached<CircularPanel, Control, double>(""RadiusOffset"");

    public static double GetRadiusOffset(Control element) => element.GetValue(RadiusOffsetProperty);
    public static void SetRadiusOffset(Control element, double value) => element.SetValue(RadiusOffsetProperty, value);",

            "masonry" => @"    public static readonly AttachedProperty<int> ColumnSpanProperty =
        AvaloniaProperty.RegisterAttached<MasonryPanel, Control, int>(""ColumnSpan"", 1);

    public static int GetColumnSpan(Control element) => element.GetValue(ColumnSpanProperty);
    public static void SetColumnSpan(Control element, int value) => element.SetValue(ColumnSpanProperty, value);",

            _ => @"    public static readonly AttachedProperty<int> OrderProperty =
        AvaloniaProperty.RegisterAttached<CustomPanel, Control, int>(""Order"");

    public static int GetOrder(Control element) => element.GetValue(OrderProperty);
    public static void SetOrder(Control element, int value) => element.SetValue(OrderProperty, value);"
        };
    }

    private static string GenerateLayoutPanelUsage(string panelName, string layoutStrategy)
    {
        return $@"<local:{panelName}>
    <Button Content=""Item 1"" />
    <Button Content=""Item 2"" />
    <Button Content=""Item 3"" />
    <TextBlock Text=""Text Item"" local:Order=""1"" />
</local:{panelName}>";
    }

    private static string GetLayoutStrategyFeatures(string strategy)
    {
        return strategy switch
        {
            "flow" => "- Automatic line wrapping\n- Supports break hints\n- Efficient for dynamic content",
            "circular" => "- Arranges items in a circle\n- Customizable radius\n- Good for navigation menus",
            "masonry" => "- Pinterest-style layout\n- Variable item heights\n- Column-based arrangement",
            "star" => "- Star/radial arrangement\n- Even distribution\n- Good for wheel interfaces",
            _ => "- Custom arrangement logic\n- Flexible positioning\n- Extensible design"
        };
    }

    private static string GetLayoutPerformanceInfo(string strategy, bool virtualization)
    {
        string baseInfo = strategy switch
        {
            "flow" => "Good performance for moderate item counts",
            "circular" => "Excellent performance, fixed calculation complexity",
            "masonry" => "Performance depends on column count and item variance",
            _ => "Performance depends on custom implementation"
        };

        return virtualization ? $"{baseInfo}\n- **Virtualization enabled**: Excellent performance for large datasets" : baseInfo;
    }

    private static string ConvertPropertyType(string propertyType)
    {
        return propertyType.ToLowerInvariant() switch
        {
            "string" => "string",
            "bool" => "bool",
            "int" => "int",
            "double" => "double",
            "object" => "object",
            _ => "object"
        };
    }

    private static string GetExampleValue(string propertyType)
    {
        return propertyType.ToLowerInvariant() switch
        {
            "string" => "\"Example\"",
            "bool" => "true",
            "int" => "42",
            "double" => "3.14",
            _ => "SomeValue"
        };
    }
}
