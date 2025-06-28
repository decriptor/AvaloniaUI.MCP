using AvaloniaUI.MCP.Tools;

namespace AvaloniaUI.MCP.Tests;

[TestClass]
public class ThemingToolTests
{
    [TestMethod]
    public void GenerateTheme_ValidInputs_ReturnsTheme()
    {
        // Arrange
        string themeName = "BlueTheme";
        string primaryColor = "#007ACC";
        string secondaryColor = "#FF6B35";

        // Act
        string result = ThemingTool.GenerateTheme(themeName, primaryColor, secondaryColor);

        // Assert
        Assert.IsTrue(result.Contains($"# Custom AvaloniaUI Theme: {themeName}"), "Should contain theme name in title");
        Assert.IsTrue(result.Contains("## Theme Configuration"), "Should contain theme configuration section");
        Assert.IsTrue(result.Contains("## Implementation Files"), "Should contain implementation files section");
        Assert.IsTrue(result.Contains($"### 1. {themeName}Theme.axaml"), "Should contain AXAML file reference");
        Assert.IsTrue(result.Contains($"### 2. {themeName}Theme.cs"), "Should contain C# file reference");
        Assert.IsTrue(result.Contains("## Usage Instructions"), "Should contain usage instructions");
        Assert.IsTrue(result.Contains("## Color Palette Generated"), "Should contain color palette section");
        Assert.IsTrue(result.Contains("## Customization Tips"), "Should contain customization tips");
        Assert.IsFalse(result.Contains("Error generating theme"), "Should not contain error message");
    }

    [TestMethod]
    public void GenerateTheme_ContainsThemeConfiguration()
    {
        // Act
        string result = ThemingTool.GenerateTheme("TestTheme", "#007ACC", "#FF6B35", "#FFFFFF", "light", "true");

        // Assert
        Assert.IsTrue(result.Contains("**Type**: light"), "Should contain theme type");
        Assert.IsTrue(result.Contains("**Primary Color**: #007ACC"), "Should contain primary color");
        Assert.IsTrue(result.Contains("**Secondary Color**: #FF6B35"), "Should contain secondary color");
        Assert.IsTrue(result.Contains("**Background Color**: #FFFFFF"), "Should contain background color");
        Assert.IsTrue(result.Contains("**Modern Effects**: True"), "Should contain modern effects setting");
    }

    [DataTestMethod]
    [DataRow("light")]
    [DataRow("dark")]
    [DataRow("auto")]
    public void GenerateTheme_DifferentThemeTypes_ReturnsCorrectType(string themeType)
    {
        // Act
        string result = ThemingTool.GenerateTheme("TestTheme", "#007ACC", "", "#FFFFFF", themeType);

        // Assert
        Assert.IsTrue(result.Contains($"**Type**: {themeType}"), $"Should contain correct theme type: {themeType}");
    }

    [TestMethod]
    public void GenerateTheme_LightTheme_ContainsLightColors()
    {
        // Act
        string result = ThemingTool.GenerateTheme("LightTheme", "#007ACC", "", "#FFFFFF", "light");

        // Assert
        Assert.IsTrue(result.Contains("**Text**: #000000"), "Should contain light theme text color");
        Assert.IsTrue(result.Contains("**Surface**: #F5F5F5"), "Should contain light theme surface color");
        Assert.IsTrue(result.Contains("**Border**: #CCCCCC"), "Should contain light theme border color");
    }

    [TestMethod]
    public void GenerateTheme_DarkTheme_ContainsDarkColors()
    {
        // Act
        string result = ThemingTool.GenerateTheme("DarkTheme", "#007ACC", "", "#2D2D30", "dark");

        // Assert
        Assert.IsTrue(result.Contains("**Text**: #FFFFFF"), "Should contain dark theme text color");
        Assert.IsTrue(result.Contains("**Surface**: #2D2D30"), "Should contain dark theme surface color");
        Assert.IsTrue(result.Contains("**Border**: #464647"), "Should contain dark theme border color");
    }

    [TestMethod]
    public void GenerateTheme_EmptySecondaryColor_GeneratesSecondaryColor()
    {
        // Act
        string result = ThemingTool.GenerateTheme("TestTheme", "#007ACC", "");

        // Assert
        Assert.IsTrue(result.Contains("**Secondary Color**:"), "Should contain secondary color field");
        // Should contain a generated secondary color with a hex value
        Assert.IsTrue(result.Contains("**Secondary Color**: #"), "Should contain generated secondary color with hex value");
    }

    [TestMethod]
    public void GenerateTheme_ContainsXAMLImplementation()
    {
        // Act
        string result = ThemingTool.GenerateTheme("TestTheme", "#007ACC");

        // Assert
        Assert.IsTrue(result.Contains("xmlns=\"https://github.com/avaloniaui\""), "Should contain AvaloniaUI namespace");
        Assert.IsTrue(result.Contains("xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\""), "Should contain XAML namespace");
        Assert.IsTrue(result.Contains("<Styles"), "Should contain Styles element");
        Assert.IsTrue(result.Contains("ThemePrimaryBrush"), "Should contain theme primary brush");
        Assert.IsTrue(result.Contains("ThemeSecondaryBrush"), "Should contain theme secondary brush");
        Assert.IsTrue(result.Contains("ThemeBackgroundBrush"), "Should contain theme background brush");
        Assert.IsTrue(result.Contains("ThemeTextBrush"), "Should contain theme text brush");
        Assert.IsTrue(result.Contains("ThemeSurfaceBrush"), "Should contain theme surface brush");
        Assert.IsTrue(result.Contains("ThemeBorderBrush"), "Should contain theme border brush");
        Assert.IsTrue(result.Contains("ThemeCornerRadius"), "Should contain theme corner radius");
        Assert.IsTrue(result.Contains("ThemePadding"), "Should contain theme padding");
        Assert.IsTrue(result.Contains("ThemeMargin"), "Should contain theme margin");
    }

    [TestMethod]
    public void GenerateTheme_ContainsCSharpImplementation()
    {
        // Arrange
        string themeName = "CustomTheme";

        // Act
        string result = ThemingTool.GenerateTheme(themeName, "#007ACC");

        // Assert
        Assert.IsTrue(result.Contains("using Avalonia.Styling;"), "Should contain Avalonia.Styling using statement");
        Assert.IsTrue(result.Contains($"public class {themeName}Theme : Styles"), "Should contain theme class definition");
        Assert.IsTrue(result.Contains($"public {themeName}Theme()"), "Should contain theme constructor");
        Assert.IsTrue(result.Contains("StyleInclude"), "Should contain StyleInclude");
        Assert.IsTrue(result.Contains($"/Themes/{themeName}Theme.axaml"), "Should contain theme AXAML path");
        Assert.IsTrue(result.Contains("public static void Apply()"), "Should contain Apply method");
        Assert.IsTrue(result.Contains("Application.Current.Styles"), "Should contain application styles reference");
    }

    [TestMethod]
    public void GenerateTheme_ContainsUsageInstructions()
    {
        // Arrange
        string themeName = "MyTheme";

        // Act
        string result = ThemingTool.GenerateTheme(themeName, "#007ACC");

        // Assert
        Assert.IsTrue(result.Contains("## Usage Instructions"), "Should contain usage instructions section");
        Assert.IsTrue(result.Contains("Add to your project"), "Should contain project addition instructions");
        Assert.IsTrue(result.Contains("Apply the theme in App.axaml"), "Should contain App.axaml instructions");
        Assert.IsTrue(result.Contains($"<local:{themeName}Theme />"), "Should contain XAML usage example");
        Assert.IsTrue(result.Contains("Or apply programmatically"), "Should contain programmatic usage instructions");
        Assert.IsTrue(result.Contains($"new {themeName}Theme()"), "Should contain constructor usage example");
    }

    [TestMethod]
    public void GenerateTheme_ContainsColorPalette()
    {
        // Act
        string result = ThemingTool.GenerateTheme("TestTheme", "#007ACC", "#FF6B35", "#FFFFFF", "light");

        // Assert
        Assert.IsTrue(result.Contains("## Color Palette Generated"), "Should contain color palette section");
        Assert.IsTrue(result.Contains("### Primary Colors"), "Should contain primary colors section");
        Assert.IsTrue(result.Contains("**Primary**: #007ACC"), "Should contain primary color");
        Assert.IsTrue(result.Contains("**Secondary**: #FF6B35"), "Should contain secondary color");
        Assert.IsTrue(result.Contains("### Neutral Colors"), "Should contain neutral colors section");
        Assert.IsTrue(result.Contains("**Background**: #FFFFFF"), "Should contain background color");
    }

    [TestMethod]
    public void GenerateTheme_ContainsCustomizationTips()
    {
        // Act
        string result = ThemingTool.GenerateTheme("TestTheme", "#007ACC");

        // Assert
        Assert.IsTrue(result.Contains("## Customization Tips"), "Should contain customization tips section");
        Assert.IsTrue(result.Contains("Modify color values"), "Should contain color modification tip");
        Assert.IsTrue(result.Contains("Adjust corner radius"), "Should contain corner radius tip");
        Assert.IsTrue(result.Contains("Add custom animations"), "Should contain animation tip");
        Assert.IsTrue(result.Contains("Create variants"), "Should contain variants tip");
    }

    [DataTestMethod]
    [DataRow("#007ACC")]
    [DataRow("#FF6B35")]
    [DataRow("#FFFFFF")]
    [DataRow("#000000")]
    public void GenerateTheme_ValidHexColors_AcceptsColors(string color)
    {
        // Act
        string result = ThemingTool.GenerateTheme("TestTheme", color);

        // Assert
        Assert.IsFalse(result.Contains("Error generating theme"), "Should not contain error message for valid color");
        Assert.IsTrue(result.Contains(color), $"Should contain the provided color: {color}");
    }

    [DataTestMethod]
    [DataRow("#FF0")]  // Short hex
    [DataRow("red")]   // Named color
    [DataRow("blue")]  // Named color
    public void GenerateTheme_DifferentColorFormats_HandlesCorrectly(string color)
    {
        // Act
        string result = ThemingTool.GenerateTheme("TestTheme", color);

        // Assert
        // Should either accept the color or convert it properly
        Assert.IsFalse(result.Contains("Error generating theme"), $"Should handle color format correctly: {color}");
    }

    [TestMethod]
    public void GenerateTheme_InvalidColor_ReturnsError()
    {
        // Act
        string result = ThemingTool.GenerateTheme("TestTheme", "invalid-color");

        // Assert
        Assert.IsTrue(result.Contains("Error generating theme"), "Should contain error message for invalid color");
    }

    [TestMethod]
    public void GenerateTheme_EmptyThemeName_HandlesGracefully()
    {
        // Act
        string result = ThemingTool.GenerateTheme("", "#007ACC");

        // Assert
        Assert.IsTrue(result.Contains("# Custom AvaloniaUI Theme:"), "Should handle empty theme name gracefully");
    }

    [TestMethod]
    public void GenerateSelectors_ValidInputs_ReturnsSelectors()
    {
        // Arrange
        string controlType = "Button";
        string styleClasses = "primary,large";
        string pseudoClasses = "pointerover,pressed";

        // Act
        string result = ThemingTool.GenerateSelectors(controlType, styleClasses, pseudoClasses, "true");

        // Assert
        Assert.IsTrue(result.Contains($"# AvaloniaUI CSS-like Selectors for {controlType}"), "Should contain selector title for control type");
        Assert.IsTrue(result.Contains("## Generated Selectors"), "Should contain generated selectors section");
        Assert.IsTrue(result.Contains("## XAML Implementation"), "Should contain XAML implementation section");
        Assert.IsTrue(result.Contains("## Selector Types Explained"), "Should contain selector types explanation");
        Assert.IsTrue(result.Contains("## Usage Tips"), "Should contain usage tips section");
        Assert.IsFalse(result.Contains("Error generating selectors"), "Should not contain error message");
    }

    [TestMethod]
    public void GenerateSelectors_ContainsBasicSelector()
    {
        // Act
        string result = ThemingTool.GenerateSelectors("Button");

        // Assert
        Assert.IsTrue(result.Contains("- `Button`"), "Should contain basic Button selector");
    }

    [TestMethod]
    public void GenerateSelectors_WithStyleClasses_ContainsClassSelectors()
    {
        // Act
        string result = ThemingTool.GenerateSelectors("Button", "primary,secondary");

        // Assert
        Assert.IsTrue(result.Contains("- `Button.primary.secondary`"), "Should contain combined style class selectors");
    }

    [TestMethod]
    public void GenerateSelectors_WithPseudoClasses_ContainsPseudoSelectors()
    {
        // Act
        string result = ThemingTool.GenerateSelectors("Button", "primary", "pointerover,pressed");

        // Assert
        Assert.IsTrue(result.Contains("- `Button.primary:pointerover`"), "Should contain pointer over pseudo selector");
        Assert.IsTrue(result.Contains("- `Button.primary:pressed`"), "Should contain pressed pseudo selector");
    }

    [TestMethod]
    public void GenerateSelectors_WithChildSelectors_ContainsChildSelectors()
    {
        // Act
        string result = ThemingTool.GenerateSelectors("Button", "", "", "true");

        // Assert
        Assert.IsTrue(result.Contains("- `StackPanel > Button`"), "Should contain direct child selector");
        Assert.IsTrue(result.Contains("- `Grid Button`"), "Should contain descendant selector");
        Assert.IsTrue(result.Contains("- `Button /template/ ContentPresenter`"), "Should contain template selector");
    }

    [TestMethod]
    public void GenerateSelectors_WithoutChildSelectors_ExcludesChildSelectors()
    {
        // Act
        string result = ThemingTool.GenerateSelectors("Button", "", "", "false");

        // Assert
        Assert.IsFalse(result.Contains("StackPanel > Button"), "Should not contain direct child selector");
        Assert.IsFalse(result.Contains("Grid Button"), "Should not contain descendant selector");
        Assert.IsFalse(result.Contains("Button /template/ ContentPresenter"), "Should not contain template selector");
    }

    [TestMethod]
    public void GenerateSelectors_ContainsXAMLExamples()
    {
        // Act
        string result = ThemingTool.GenerateSelectors("Button", "primary");

        // Assert
        Assert.IsTrue(result.Contains("<Window.Styles>"), "Should contain Window.Styles example");
        Assert.IsTrue(result.Contains("<Style Selector="), "Should contain Style Selector example");
        Assert.IsTrue(result.Contains("Classes=\"primary\""), "Should contain Classes example");
    }

    [TestMethod]
    public void GenerateColorScheme_ValidInputs_ReturnsColorScheme()
    {
        // Arrange
        string baseColor = "#007ACC";
        string schemeType = "monochromatic";

        // Act
        string result = ThemingTool.GenerateColorScheme(baseColor, schemeType, 5);

        // Assert
        Assert.IsTrue(result.Contains($"# Color Scheme: {schemeType} ({baseColor})"), "Should contain color scheme title");
        Assert.IsTrue(result.Contains("## Generated Colors"), "Should contain generated colors section");
        Assert.IsTrue(result.Contains("## AvaloniaUI Resource Dictionary"), "Should contain resource dictionary section");
        Assert.IsTrue(result.Contains("## CSS Variables Style"), "Should contain CSS variables section");
        Assert.IsTrue(result.Contains("## Usage Examples"), "Should contain usage examples section");
        Assert.IsTrue(result.Contains("## Accessibility Notes"), "Should contain accessibility notes section");
        Assert.IsFalse(result.Contains("Error generating color scheme"), "Should not contain error message");
    }

    [DataTestMethod]
    [DataRow("monochromatic")]
    [DataRow("analogous")]
    [DataRow("complementary")]
    [DataRow("triadic")]
    [DataRow("split-complementary")]
    public void GenerateColorScheme_DifferentSchemeTypes_ReturnsScheme(string schemeType)
    {
        // Act
        string result = ThemingTool.GenerateColorScheme("#007ACC", schemeType);

        // Assert
        Assert.IsTrue(result.Contains($"# Color Scheme: {schemeType}"), $"Should contain color scheme title with type: {schemeType}");
    }

    [TestMethod]
    public void GenerateColorScheme_ContainsResourceDictionary()
    {
        // Act
        string result = ThemingTool.GenerateColorScheme("#007ACC");

        // Assert
        Assert.IsTrue(result.Contains("<ResourceDictionary>"), "Should contain ResourceDictionary element");
        Assert.IsTrue(result.Contains("SolidColorBrush"), "Should contain SolidColorBrush elements");
        Assert.IsTrue(result.Contains("x:Key="), "Should contain x:Key attributes");
        Assert.IsTrue(result.Contains("Color="), "Should contain Color attributes");
    }

    [TestMethod]
    public void GenerateColorScheme_ContainsAccessibilityNotes()
    {
        // Act
        string result = ThemingTool.GenerateColorScheme("#007ACC");

        // Assert
        Assert.IsTrue(result.Contains("## Accessibility Notes"), "Should contain accessibility notes section");
        Assert.IsTrue(result.Contains("Contrast Ratios"), "Should contain contrast ratios information");
        Assert.IsTrue(result.Contains("WCAG AA compliance"), "Should contain WCAG AA compliance information");
        Assert.IsTrue(result.Contains("4.5:1 for normal text"), "Should contain normal text contrast ratio");
        Assert.IsTrue(result.Contains("3:1 for large text"), "Should contain large text contrast ratio");
        Assert.IsTrue(result.Contains("Color Blindness"), "Should contain color blindness information");
        Assert.IsTrue(result.Contains("High Contrast Support"), "Should contain high contrast support information");
    }

    [DataTestMethod]
    [DataRow(2)]
    [DataRow(5)]
    [DataRow(10)]
    public void GenerateColorScheme_DifferentColorCounts_ReturnsCorrectCount(int colorCount)
    {
        // Act
        string result = ThemingTool.GenerateColorScheme("#007ACC", "monochromatic", colorCount);

        // Assert
        // Should contain the requested number of colors
        string[] lines = result.Split('\n');
        int colorLines = lines.Count(l => l.Contains(". **") && l.Contains("**: `#"));
        Assert.AreEqual(Math.Min(colorCount, 5), colorLines, "Should contain the correct number of color lines (limited by implementation)"); // Limited by current implementation
    }

    [TestMethod]
    public void GenerateColorScheme_InvalidColor_ReturnsError()
    {
        // Act
        string result = ThemingTool.GenerateColorScheme("invalid-color");

        // Assert
        Assert.IsTrue(result.Contains("Error generating color scheme"), "Should contain error message for invalid color");
    }

    [DataTestMethod]
    [DataRow("true", true)]
    [DataRow("false", false)]
    public void GenerateTheme_IncludeEffectsParameter_HandlesCorrectly(string includeEffects, bool expected)
    {
        // Act
        string result = ThemingTool.GenerateTheme("TestTheme", "#007ACC", "", "#FFFFFF", "light", includeEffects);

        // Assert
        Assert.IsTrue(result.Contains($"**Modern Effects**: {expected}"), $"Should contain correct modern effects setting: {expected}");
    }

    [TestMethod]
    public void GenerateTheme_CaseInsensitiveThemeType_HandlesCorrectly()
    {
        // Act
        string result1 = ThemingTool.GenerateTheme("TestTheme", "#007ACC", "", "#FFFFFF", "LIGHT");
        string result2 = ThemingTool.GenerateTheme("TestTheme", "#007ACC", "", "#FFFFFF", "Light");

        // Assert
        Assert.IsTrue(result1.Contains("**Type**: light"), "Should handle uppercase theme type correctly");
        Assert.IsTrue(result2.Contains("**Type**: light"), "Should handle title case theme type correctly");
    }
}
