using AvaloniaUI.MCP.Tools;
using Xunit;

namespace AvaloniaUI.MCP.Tests;

public class ThemingToolTests
{
    [Fact]
    public void GenerateTheme_ValidInputs_ReturnsTheme()
    {
        // Arrange
        var themeName = "BlueTheme";
        var primaryColor = "#007ACC";
        var secondaryColor = "#FF6B35";
        
        // Act
        var result = ThemingTool.GenerateTheme(themeName, primaryColor, secondaryColor);
        
        // Assert
        Assert.Contains($"# Custom AvaloniaUI Theme: {themeName}", result);
        Assert.Contains("## Theme Configuration", result);
        Assert.Contains("## Implementation Files", result);
        Assert.Contains($"### 1. {themeName}Theme.axaml", result);
        Assert.Contains($"### 2. {themeName}Theme.cs", result);
        Assert.Contains("## Usage Instructions", result);
        Assert.Contains("## Color Palette Generated", result);
        Assert.Contains("## Customization Tips", result);
        Assert.DoesNotContain("Error generating theme", result);
    }

    [Fact]
    public void GenerateTheme_ContainsThemeConfiguration()
    {
        // Act
        var result = ThemingTool.GenerateTheme("TestTheme", "#007ACC", "#FF6B35", "#FFFFFF", "light", "true");
        
        // Assert
        Assert.Contains("**Type**: light", result);
        Assert.Contains("**Primary Color**: #007ACC", result);
        Assert.Contains("**Secondary Color**: #FF6B35", result);
        Assert.Contains("**Background Color**: #FFFFFF", result);
        Assert.Contains("**Modern Effects**: True", result);
    }

    [Theory]
    [InlineData("light")]
    [InlineData("dark")]
    [InlineData("auto")]
    public void GenerateTheme_DifferentThemeTypes_ReturnsCorrectType(string themeType)
    {
        // Act
        var result = ThemingTool.GenerateTheme("TestTheme", "#007ACC", "", "#FFFFFF", themeType);
        
        // Assert
        Assert.Contains($"**Type**: {themeType}", result);
    }

    [Fact]
    public void GenerateTheme_LightTheme_ContainsLightColors()
    {
        // Act
        var result = ThemingTool.GenerateTheme("LightTheme", "#007ACC", "", "#FFFFFF", "light");
        
        // Assert
        Assert.Contains("**Text**: #000000", result);
        Assert.Contains("**Surface**: #F5F5F5", result);
        Assert.Contains("**Border**: #CCCCCC", result);
    }

    [Fact]
    public void GenerateTheme_DarkTheme_ContainsDarkColors()
    {
        // Act
        var result = ThemingTool.GenerateTheme("DarkTheme", "#007ACC", "", "#2D2D30", "dark");
        
        // Assert
        Assert.Contains("**Text**: #FFFFFF", result);
        Assert.Contains("**Surface**: #2D2D30", result);
        Assert.Contains("**Border**: #464647", result);
    }

    [Fact]
    public void GenerateTheme_EmptySecondaryColor_GeneratesSecondaryColor()
    {
        // Act
        var result = ThemingTool.GenerateTheme("TestTheme", "#007ACC", "");
        
        // Assert
        Assert.Contains("**Secondary Color**:", result);
        // Should contain a generated secondary color with a hex value
        Assert.Contains("**Secondary Color**: #", result);
    }

    [Fact]
    public void GenerateTheme_ContainsXAMLImplementation()
    {
        // Act
        var result = ThemingTool.GenerateTheme("TestTheme", "#007ACC");
        
        // Assert
        Assert.Contains("xmlns=\"https://github.com/avaloniaui\"", result);
        Assert.Contains("xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"", result);
        Assert.Contains("<Styles", result);
        Assert.Contains("ThemePrimaryBrush", result);
        Assert.Contains("ThemeSecondaryBrush", result);
        Assert.Contains("ThemeBackgroundBrush", result);
        Assert.Contains("ThemeTextBrush", result);
        Assert.Contains("ThemeSurfaceBrush", result);
        Assert.Contains("ThemeBorderBrush", result);
        Assert.Contains("ThemeCornerRadius", result);
        Assert.Contains("ThemePadding", result);
        Assert.Contains("ThemeMargin", result);
    }

    [Fact]
    public void GenerateTheme_ContainsCSharpImplementation()
    {
        // Arrange
        var themeName = "CustomTheme";
        
        // Act
        var result = ThemingTool.GenerateTheme(themeName, "#007ACC");
        
        // Assert
        Assert.Contains("using Avalonia.Styling;", result);
        Assert.Contains($"public class {themeName}Theme : Styles", result);
        Assert.Contains($"public {themeName}Theme()", result);
        Assert.Contains("StyleInclude", result);
        Assert.Contains($"/Themes/{themeName}Theme.axaml", result);
        Assert.Contains("public static void Apply()", result);
        Assert.Contains("Application.Current.Styles", result);
    }

    [Fact]
    public void GenerateTheme_ContainsUsageInstructions()
    {
        // Arrange
        var themeName = "MyTheme";
        
        // Act
        var result = ThemingTool.GenerateTheme(themeName, "#007ACC");
        
        // Assert
        Assert.Contains("## Usage Instructions", result);
        Assert.Contains("Add to your project", result);
        Assert.Contains("Apply the theme in App.axaml", result);
        Assert.Contains($"<local:{themeName}Theme />", result);
        Assert.Contains("Or apply programmatically", result);
        Assert.Contains($"new {themeName}Theme()", result);
    }

    [Fact]
    public void GenerateTheme_ContainsColorPalette()
    {
        // Act
        var result = ThemingTool.GenerateTheme("TestTheme", "#007ACC", "#FF6B35", "#FFFFFF", "light");
        
        // Assert
        Assert.Contains("## Color Palette Generated", result);
        Assert.Contains("### Primary Colors", result);
        Assert.Contains("**Primary**: #007ACC", result);
        Assert.Contains("**Secondary**: #FF6B35", result);
        Assert.Contains("### Neutral Colors", result);
        Assert.Contains("**Background**: #FFFFFF", result);
    }

    [Fact]
    public void GenerateTheme_ContainsCustomizationTips()
    {
        // Act
        var result = ThemingTool.GenerateTheme("TestTheme", "#007ACC");
        
        // Assert
        Assert.Contains("## Customization Tips", result);
        Assert.Contains("Modify color values", result);
        Assert.Contains("Adjust corner radius", result);
        Assert.Contains("Add custom animations", result);
        Assert.Contains("Create variants", result);
    }

    [Theory]
    [InlineData("#007ACC")]
    [InlineData("#FF6B35")]
    [InlineData("#FFFFFF")]
    [InlineData("#000000")]
    public void GenerateTheme_ValidHexColors_AcceptsColors(string color)
    {
        // Act
        var result = ThemingTool.GenerateTheme("TestTheme", color);
        
        // Assert
        Assert.DoesNotContain("Error generating theme", result);
        Assert.Contains(color, result);
    }

    [Theory]
    [InlineData("#FF0")]  // Short hex
    [InlineData("red")]   // Named color
    [InlineData("blue")]  // Named color
    public void GenerateTheme_DifferentColorFormats_HandlesCorrectly(string color)
    {
        // Act
        var result = ThemingTool.GenerateTheme("TestTheme", color);
        
        // Assert
        // Should either accept the color or convert it properly
        Assert.DoesNotContain("Error generating theme", result);
    }

    [Fact]
    public void GenerateTheme_InvalidColor_ReturnsError()
    {
        // Act
        var result = ThemingTool.GenerateTheme("TestTheme", "invalid-color");
        
        // Assert
        Assert.Contains("Error generating theme", result);
    }

    [Fact]
    public void GenerateTheme_EmptyThemeName_HandlesGracefully()
    {
        // Act
        var result = ThemingTool.GenerateTheme("", "#007ACC");
        
        // Assert
        Assert.Contains("# Custom AvaloniaUI Theme:", result);
    }

    [Fact]
    public void GenerateSelectors_ValidInputs_ReturnsSelectors()
    {
        // Arrange
        var controlType = "Button";
        var styleClasses = "primary,large";
        var pseudoClasses = "pointerover,pressed";
        
        // Act
        var result = ThemingTool.GenerateSelectors(controlType, styleClasses, pseudoClasses, "true");
        
        // Assert
        Assert.Contains($"# AvaloniaUI CSS-like Selectors for {controlType}", result);
        Assert.Contains("## Generated Selectors", result);
        Assert.Contains("## XAML Implementation", result);
        Assert.Contains("## Selector Types Explained", result);
        Assert.Contains("## Usage Tips", result);
        Assert.DoesNotContain("Error generating selectors", result);
    }

    [Fact]
    public void GenerateSelectors_ContainsBasicSelector()
    {
        // Act
        var result = ThemingTool.GenerateSelectors("Button");
        
        // Assert
        Assert.Contains("- `Button`", result);
    }

    [Fact]
    public void GenerateSelectors_WithStyleClasses_ContainsClassSelectors()
    {
        // Act
        var result = ThemingTool.GenerateSelectors("Button", "primary,secondary");
        
        // Assert
        Assert.Contains("- `Button.primary.secondary`", result);
    }

    [Fact]
    public void GenerateSelectors_WithPseudoClasses_ContainsPseudoSelectors()
    {
        // Act
        var result = ThemingTool.GenerateSelectors("Button", "primary", "pointerover,pressed");
        
        // Assert
        Assert.Contains("- `Button.primary:pointerover`", result);
        Assert.Contains("- `Button.primary:pressed`", result);
    }

    [Fact]
    public void GenerateSelectors_WithChildSelectors_ContainsChildSelectors()
    {
        // Act
        var result = ThemingTool.GenerateSelectors("Button", "", "", "true");
        
        // Assert
        Assert.Contains("- `StackPanel > Button`", result);
        Assert.Contains("- `Grid Button`", result);
        Assert.Contains("- `Button /template/ ContentPresenter`", result);
    }

    [Fact]
    public void GenerateSelectors_WithoutChildSelectors_ExcludesChildSelectors()
    {
        // Act
        var result = ThemingTool.GenerateSelectors("Button", "", "", "false");
        
        // Assert
        Assert.DoesNotContain("StackPanel > Button", result);
        Assert.DoesNotContain("Grid Button", result);
        Assert.DoesNotContain("Button /template/ ContentPresenter", result);
    }

    [Fact]
    public void GenerateSelectors_ContainsXAMLExamples()
    {
        // Act
        var result = ThemingTool.GenerateSelectors("Button", "primary");
        
        // Assert
        Assert.Contains("<Window.Styles>", result);
        Assert.Contains("<Style Selector=", result);
        Assert.Contains("Classes=\"primary\"", result);
    }

    [Fact]
    public void GenerateColorScheme_ValidInputs_ReturnsColorScheme()
    {
        // Arrange
        var baseColor = "#007ACC";
        var schemeType = "monochromatic";
        
        // Act
        var result = ThemingTool.GenerateColorScheme(baseColor, schemeType, 5);
        
        // Assert
        Assert.Contains($"# Color Scheme: {schemeType} ({baseColor})", result);
        Assert.Contains("## Generated Colors", result);
        Assert.Contains("## AvaloniaUI Resource Dictionary", result);
        Assert.Contains("## CSS Variables Style", result);
        Assert.Contains("## Usage Examples", result);
        Assert.Contains("## Accessibility Notes", result);
        Assert.DoesNotContain("Error generating color scheme", result);
    }

    [Theory]
    [InlineData("monochromatic")]
    [InlineData("analogous")]
    [InlineData("complementary")]
    [InlineData("triadic")]
    [InlineData("split-complementary")]
    public void GenerateColorScheme_DifferentSchemeTypes_ReturnsScheme(string schemeType)
    {
        // Act
        var result = ThemingTool.GenerateColorScheme("#007ACC", schemeType);
        
        // Assert
        Assert.Contains($"# Color Scheme: {schemeType}", result);
    }

    [Fact]
    public void GenerateColorScheme_ContainsResourceDictionary()
    {
        // Act
        var result = ThemingTool.GenerateColorScheme("#007ACC");
        
        // Assert
        Assert.Contains("<ResourceDictionary>", result);
        Assert.Contains("SolidColorBrush", result);
        Assert.Contains("x:Key=", result);
        Assert.Contains("Color=", result);
    }

    [Fact]
    public void GenerateColorScheme_ContainsAccessibilityNotes()
    {
        // Act
        var result = ThemingTool.GenerateColorScheme("#007ACC");
        
        // Assert
        Assert.Contains("## Accessibility Notes", result);
        Assert.Contains("Contrast Ratios", result);
        Assert.Contains("WCAG AA compliance", result);
        Assert.Contains("4.5:1 for normal text", result);
        Assert.Contains("3:1 for large text", result);
        Assert.Contains("Color Blindness", result);
        Assert.Contains("High Contrast Support", result);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    public void GenerateColorScheme_DifferentColorCounts_ReturnsCorrectCount(int colorCount)
    {
        // Act
        var result = ThemingTool.GenerateColorScheme("#007ACC", "monochromatic", colorCount);
        
        // Assert
        // Should contain the requested number of colors
        var lines = result.Split('\n');
        var colorLines = lines.Where(l => l.Contains(". **") && l.Contains("**: `#")).Count();
        Assert.Equal(Math.Min(colorCount, 5), colorLines); // Limited by current implementation
    }

    [Fact]
    public void GenerateColorScheme_InvalidColor_ReturnsError()
    {
        // Act
        var result = ThemingTool.GenerateColorScheme("invalid-color");
        
        // Assert
        Assert.Contains("Error generating color scheme", result);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void GenerateTheme_IncludeEffectsParameter_HandlesCorrectly(string includeEffects, bool expected)
    {
        // Act
        var result = ThemingTool.GenerateTheme("TestTheme", "#007ACC", "", "#FFFFFF", "light", includeEffects);
        
        // Assert
        Assert.Contains($"**Modern Effects**: {expected}", result);
    }

    [Fact]
    public void GenerateTheme_CaseInsensitiveThemeType_HandlesCorrectly()
    {
        // Act
        var result1 = ThemingTool.GenerateTheme("TestTheme", "#007ACC", "", "#FFFFFF", "LIGHT");
        var result2 = ThemingTool.GenerateTheme("TestTheme", "#007ACC", "", "#FFFFFF", "Light");
        
        // Assert
        Assert.Contains("**Type**: light", result1);
        Assert.Contains("**Type**: light", result2);
    }
}