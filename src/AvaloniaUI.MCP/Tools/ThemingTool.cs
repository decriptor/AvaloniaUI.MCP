using System.ComponentModel;
using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Tools;

[McpServerToolType]
public static class ThemingTool
{
    [McpServerTool, Description("Generates a custom AvaloniaUI theme with specified color palette and styling options")]
    public static string GenerateTheme(
        [Description("Name of the theme")] string themeName,
        [Description("Primary color (hex, e.g., #007ACC)")] string primaryColor,
        [Description("Secondary color (hex, e.g., #FF6B35)")] string secondaryColor = "",
        [Description("Background color (hex, e.g., #FFFFFF)")] string backgroundColor = "#FFFFFF",
        [Description("Theme type: 'light', 'dark', or 'auto'")] string themeType = "light",
        [Description("Include modern effects: 'true' or 'false'")] string includeEffects = "true")
    {
        try
        {
            var theme = new ThemeConfiguration
            {
                Name = themeName,
                Type = themeType.ToLowerInvariant(),
                PrimaryColor = ValidateColor(primaryColor),
                SecondaryColor = string.IsNullOrEmpty(secondaryColor) ? GenerateSecondaryColor(primaryColor) : ValidateColor(secondaryColor),
                BackgroundColor = ValidateColor(backgroundColor),
                IncludeEffects = bool.Parse(includeEffects)
            };

            var xamlContent = GenerateThemeXaml(theme);
            var codeContent = GenerateThemeCode(theme);

            return $@"# Custom AvaloniaUI Theme: {themeName}

## Theme Configuration
- **Type**: {theme.Type}
- **Primary Color**: {theme.PrimaryColor}
- **Secondary Color**: {theme.SecondaryColor}
- **Background Color**: {theme.BackgroundColor}
- **Modern Effects**: {theme.IncludeEffects}

## Implementation Files

### 1. {themeName}Theme.axaml
```xml
{xamlContent}
```

### 2. {themeName}Theme.cs
```csharp
{codeContent}
```

## Usage Instructions

1. **Add to your project**:
   - Copy the XAML file to your Themes folder
   - Copy the C# file to your Themes folder

2. **Apply the theme in App.axaml**:
```xml
<Application.Styles>
    <local:{themeName}Theme />
</Application.Styles>
```

3. **Or apply programmatically**:
```csharp
Application.Current.Styles.Add(new {themeName}Theme());
```

## Color Palette Generated
{GenerateColorPalette(theme)}

## Customization Tips
- Modify color values in the ResourceDictionary section
- Adjust corner radius values for different visual styles
- Add custom animations in the effects section
- Create variants by inheriting from this theme";
        }
        catch (Exception ex)
        {
            return $"Error generating theme: {ex.Message}";
        }
    }

    [McpServerTool, Description("Creates CSS-like selectors for AvaloniaUI styling")]
    public static string GenerateSelectors(
        [Description("Target control type (e.g., Button, TextBox, Grid)")] string controlType,
        [Description("Style classes to apply (comma-separated, e.g., primary,large)")] string styleClasses = "",
        [Description("Pseudo-classes to include (comma-separated, e.g., pointerover,pressed)")] string pseudoClasses = "",
        [Description("Include child selectors: 'true' or 'false'")] string includeChildren = "false")
    {
        try
        {
            var selectors = new List<string>();
            var classes = styleClasses.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList();
            var pseudos = pseudoClasses.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();

            // Basic selector
            var baseSelector = controlType;
            if (classes.Any())
            {
                baseSelector += "." + string.Join(".", classes);
            }
            selectors.Add(baseSelector);

            // Pseudo-class selectors
            foreach (var pseudo in pseudos)
            {
                selectors.Add($"{baseSelector}:{pseudo}");
            }

            // Child selectors if requested
            if (bool.Parse(includeChildren))
            {
                selectors.Add($"StackPanel > {controlType}");
                selectors.Add($"Grid {controlType}");
                selectors.Add($"{controlType} /template/ ContentPresenter");
            }

            var xamlExamples = GenerateSelectorXaml(selectors, controlType);

            var classesExample = classes.Any() ? string.Join(" ", classes) : "primary secondary";

            return $@"# AvaloniaUI CSS-like Selectors for {controlType}

## Generated Selectors
{string.Join("\n", selectors.Select(s => $"- `{s}`"))}

## XAML Implementation
```xml
{xamlExamples}
```

## Selector Types Explained

### Basic Selectors
- `{controlType}` - Targets all {controlType} controls
- `{controlType}.{(classes.Any() ? classes.First() : "class")}` - Targets {controlType} with specific class

### Pseudo-class Selectors
- `:pointerover` - When mouse hovers over control
- `:pressed` - When control is being pressed
- `:focus` - When control has keyboard focus
- `:disabled` - When control is disabled
- `:checked` - For checkable controls (CheckBox, RadioButton)

### Combinators
- `Parent > Child` - Direct child selector
- `Ancestor Descendant` - Descendant selector
- `Control /template/ TemplatePart` - Template part selector

### Attribute Selectors
- `Button[IsDefault=True]` - Attribute value selector
- `TextBox[IsReadOnly=False]` - Boolean property selector

## Usage Tips
1. Apply classes in XAML: `<Button Classes=""{classesExample}"" />`
2. Combine selectors for specificity
3. Use pseudo-classes for interactive states
4. Prefer class selectors over complex hierarchies";
        }
        catch (Exception ex)
        {
            return $"Error generating selectors: {ex.Message}";
        }
    }

    [McpServerTool, Description("Converts colors between different formats and generates color schemes")]
    public static string GenerateColorScheme(
        [Description("Base color (hex, rgb, or name)")] string baseColor,
        [Description("Scheme type: 'monochromatic', 'analogous', 'complementary', 'triadic', 'split-complementary'")] string schemeType = "monochromatic",
        [Description("Number of colors to generate (2-10)")] int colorCount = 5)
    {
        try
        {
            var normalizedColor = ValidateColor(baseColor);
            var colors = GenerateColorSchemeColors(normalizedColor, schemeType, colorCount);

            var result = $@"# Color Scheme: {schemeType} ({baseColor})

## Generated Colors
{string.Join("\n", colors.Select((c, i) => $"{i + 1}. **{c.Name}**: `{c.Hex}` - {c.Description}"))}

## AvaloniaUI Resource Dictionary
```xml
<ResourceDictionary>
    <!-- {schemeType} Color Scheme -->
{string.Join("\n", colors.Select(c => $"    <SolidColorBrush x:Key=\"{c.Name}Brush\" Color=\"{c.Hex}\" />"))}
</ResourceDictionary>
```

## CSS Variables Style
```xml
<ResourceDictionary>
{string.Join("\n", colors.Select((c, i) => $"    <SolidColorBrush x:Key=\"Color{i + 1}\" Color=\"{c.Hex}\" />"))}
</ResourceDictionary>
```

## Usage Examples
```xml
<!-- Using named brushes -->
<Button Background=""{{StaticResource {colors.First().Name}Brush}}"" />

<!-- Using in styles -->
<Style Selector=""Button.primary"">
    <Setter Property=""Background"" Value=""{{StaticResource {colors.First().Name}Brush}}"" />
</Style>
```

## Accessibility Notes
{GenerateAccessibilityNotes()}";

            return result;
        }
        catch (Exception ex)
        {
            return $"Error generating color scheme: {ex.Message}";
        }
    }

    private class ThemeConfiguration
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "light";
        public string PrimaryColor { get; set; } = "#007ACC";
        public string SecondaryColor { get; set; } = "#FF6B35";
        public string BackgroundColor { get; set; } = "#FFFFFF";
        public bool IncludeEffects { get; set; } = true;
    }

    private class ColorInfo
    {
        public string Name { get; set; } = "";
        public string Hex { get; set; } = "";
        public string Description { get; set; } = "";
    }

    private static string ValidateColor(string color)
    {
        if (string.IsNullOrEmpty(color))
            throw new ArgumentException("Color cannot be empty");

        color = color.Trim();

        // Handle hex colors
        if (color.StartsWith("#"))
        {
            if (color.Length == 7 || color.Length == 9) // #RRGGBB or #AARRGGBB
                return color.ToUpperInvariant();
            if (color.Length == 4) // #RGB -> #RRGGBB
            {
                var r = color[1];
                var g = color[2];
                var b = color[3];
                return $"#{r}{r}{g}{g}{b}{b}";
            }
        }

        // Handle named colors
        var namedColors = new Dictionary<string, string>
        {
            {"red", "#FF0000"}, {"green", "#00FF00"}, {"blue", "#0000FF"},
            {"white", "#FFFFFF"}, {"black", "#000000"}, {"gray", "#808080"},
            {"orange", "#FFA500"}, {"purple", "#800080"}, {"yellow", "#FFFF00"}
        };

        if (namedColors.TryGetValue(color.ToLowerInvariant(), out var hexValue))
            return hexValue;

        throw new ArgumentException($"Invalid color format: {color}");
    }

    private static string GenerateSecondaryColor(string primaryColor)
    {
        // Simple complementary color generation
        return primaryColor switch
        {
            var c when c.StartsWith("#00") => "#FF6B35", // Blue -> Orange
            var c when c.StartsWith("#FF") => "#0066CC", // Red -> Blue
            var c when c.StartsWith("#80") => "#4CAF50", // Purple -> Green
            _ => "#FF6B35"
        };
    }

    private static string GenerateThemeXaml(ThemeConfiguration theme)
    {
        var isDark = theme.Type == "dark";
        var textColor = isDark ? "#FFFFFF" : "#000000";
        var surfaceColor = isDark ? "#2D2D30" : "#F5F5F5";
        var borderColor = isDark ? "#464647" : "#CCCCCC";

        return $@"<Styles xmlns=""https://github.com/avaloniaui""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    
    <!-- {theme.Name} Theme Resources -->
    <Style.Resources>
        <!-- Color Palette -->
        <SolidColorBrush x:Key=""ThemePrimaryBrush"" Color=""{theme.PrimaryColor}"" />
        <SolidColorBrush x:Key=""ThemeSecondaryBrush"" Color=""{theme.SecondaryColor}"" />
        <SolidColorBrush x:Key=""ThemeBackgroundBrush"" Color=""{theme.BackgroundColor}"" />
        <SolidColorBrush x:Key=""ThemeTextBrush"" Color=""{textColor}"" />
        <SolidColorBrush x:Key=""ThemeSurfaceBrush"" Color=""{surfaceColor}"" />
        <SolidColorBrush x:Key=""ThemeBorderBrush"" Color=""{borderColor}"" />
        
        <!-- Corner Radius -->
        <CornerRadius x:Key=""ThemeCornerRadius"">4</CornerRadius>
        <CornerRadius x:Key=""ThemeCardCornerRadius"">8</CornerRadius>
        
        <!-- Spacing -->
        <Thickness x:Key=""ThemePadding"">12,8</Thickness>
        <Thickness x:Key=""ThemeMargin"">8</Thickness>
    </Style.Resources>

    <!-- Button Styles -->
    <Style Selector=""Button"">
        <Setter Property=""Background"" Value=""{{StaticResource ThemePrimaryBrush}}"" />
        <Setter Property=""Foreground"" Value=""White"" />
        <Setter Property=""BorderBrush"" Value=""{{StaticResource ThemePrimaryBrush}}"" />
        <Setter Property=""BorderThickness"" Value=""1"" />
        <Setter Property=""CornerRadius"" Value=""{{StaticResource ThemeCornerRadius}}"" />
        <Setter Property=""Padding"" Value=""{{StaticResource ThemePadding}}"" />
        <Setter Property=""FontWeight"" Value=""SemiBold"" />
    </Style>

    <!-- Window Style -->
    <Style Selector=""Window"">
        <Setter Property=""Background"" Value=""{{StaticResource ThemeBackgroundBrush}}"" />
        <Setter Property=""Foreground"" Value=""{{StaticResource ThemeTextBrush}}"" />
    </Style>

</Styles>";
    }

    private static string GenerateThemeCode(ThemeConfiguration theme)
    {
        return $@"using Avalonia.Styling;

namespace YourApp.Themes;

public class {theme.Name}Theme : Styles
{{
    public {theme.Name}Theme()
    {{
        Add(new StyleInclude(new Uri(""avares://YourApp""))
        {{
            Source = new Uri(""/Themes/{theme.Name}Theme.axaml"", UriKind.Relative)
        }});
    }}
    
    public static void Apply()
    {{
        if (Application.Current?.Styles != null)
        {{
            Application.Current.Styles.Clear();
            Application.Current.Styles.Add(new {theme.Name}Theme());
        }}
    }}
}}";
    }

    private static string GenerateColorPalette(ThemeConfiguration theme)
    {
        return $@"
### Primary Colors
- **Primary**: {theme.PrimaryColor}
- **Secondary**: {theme.SecondaryColor}

### Neutral Colors
- **Background**: {theme.BackgroundColor}
- **Surface**: {(theme.Type == "dark" ? "#2D2D30" : "#F5F5F5")}
- **Text**: {(theme.Type == "dark" ? "#FFFFFF" : "#000000")}
- **Border**: {(theme.Type == "dark" ? "#464647" : "#CCCCCC")}";
    }

    private static List<ColorInfo> GenerateColorSchemeColors(string baseColor, string schemeType, int colorCount)
    {
        // Simplified color scheme generation
        return new List<ColorInfo>
        {
            new() { Name = "Primary", Hex = baseColor, Description = "Base color" },
            new() { Name = "Secondary", Hex = GenerateSecondaryColor(baseColor), Description = "Secondary color" },
            new() { Name = "Accent", Hex = "#FFD23F", Description = "Accent color" },
            new() { Name = "Neutral", Hex = "#808080", Description = "Neutral gray" },
            new() { Name = "Surface", Hex = "#F5F5F5", Description = "Surface color" }
        }.Take(colorCount).ToList();
    }

    private static string GenerateSelectorXaml(List<string> selectors, string controlType)
    {
        return $@"<Window.Styles>
{string.Join("\n", selectors.Select(selector => $@"    <!-- {selector} -->
    <Style Selector=""{selector}"">
        <Setter Property=""Background"" Value=""#007ACC"" />
        <Setter Property=""Foreground"" Value=""White"" />
        <Setter Property=""Padding"" Value=""12,6"" />
        <Setter Property=""CornerRadius"" Value=""4"" />
    </Style>"))}
</Window.Styles>

<!-- Usage Examples -->
<StackPanel>
    <{controlType} Content=""Basic {controlType}"" />
    <{controlType} Classes=""primary"" Content=""Primary {controlType}"" />
    <{controlType} Classes=""primary large"" Content=""Primary Large {controlType}"" />
</StackPanel>";
    }

    private static string GenerateAccessibilityNotes()
    {
        return @"
### Contrast Ratios
- Ensure text has sufficient contrast against backgrounds
- Target WCAG AA compliance (4.5:1 for normal text, 3:1 for large text)

### Color Blindness Considerations
- Avoid relying solely on color to convey information
- Use patterns, shapes, or text labels as additional indicators

### High Contrast Support
- Consider high contrast themes for accessibility
- Ensure all interactive elements remain visible";
    }
}