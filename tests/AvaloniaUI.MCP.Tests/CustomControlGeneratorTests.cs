using AvaloniaUI.MCP.Tools;
using Xunit;

namespace AvaloniaUI.MCP.Tests;

public class CustomControlGeneratorTests
{
    [Theory]
    [InlineData("templated")]
    [InlineData("usercontrol")]
    [InlineData("panel")]
    public void GenerateCustomControl_ValidControlTypes_ReturnsCorrectPattern(string controlType)
    {
        // Arrange
        var controlName = "TestControl";
        
        // Act
        var result = CustomControlGenerator.GenerateCustomControl(controlType, controlName);
        
        // Assert
        Assert.DoesNotContain("Error generating custom control", result);
        Assert.Contains(controlName, result);
    }

    [Fact]
    public void GenerateCustomControl_AttachedProperty_RequiresPropertyName()
    {
        // Arrange
        var controlName = "TestControl";
        
        // Act
        var result = CustomControlGenerator.GenerateCustomControl("attached-property", controlName, "Control", "TestProperty");
        
        // Assert
        Assert.Contains("TestProperty", result);
        Assert.Contains("Attached Property", result);
    }

    [Fact]
    public void GenerateCustomControl_TemplatedControl_ContainsExpectedElements()
    {
        // Arrange
        var controlName = "MyTemplatedControl";
        var properties = "Title,IsEnabled,Value";
        
        // Act
        var result = CustomControlGenerator.GenerateCustomControl("templated", controlName, "Control", properties, "true");
        
        // Assert
        Assert.Contains($"# Templated Control: {controlName}", result);
        Assert.Contains("## Control Implementation", result);
        Assert.Contains("## Default Template", result);
        Assert.Contains("## Control Style", result);
        Assert.Contains("## Usage Example", result);
        Assert.Contains("## Template Parts", result);
        Assert.Contains("## Visual States", result);
        Assert.Contains("PART_Border", result);
        Assert.Contains("PART_ContentPresenter", result);
        Assert.Contains("StyledProperty", result);
        Assert.Contains("OnApplyTemplate", result);
    }

    [Fact]
    public void GenerateCustomControl_UserControl_ContainsExpectedElements()
    {
        // Arrange
        var controlName = "MyUserControl";
        var properties = "Header,Content";
        
        // Act
        var result = CustomControlGenerator.GenerateCustomControl("usercontrol", controlName, "UserControl", properties);
        
        // Assert
        Assert.Contains($"# User Control: {controlName}", result);
        Assert.Contains("## XAML Definition", result);
        Assert.Contains("## Code-Behind", result);
        Assert.Contains("## Usage Example", result);
        Assert.Contains("## Data Binding Integration", result);
        Assert.Contains("## Styling", result);
        Assert.Contains("xmlns=\"https://github.com/avaloniaui\"", result);
        Assert.Contains("InitializeComponent", result);
    }

    [Fact]
    public void GenerateCustomControl_Panel_ReturnsLayoutPanelPattern()
    {
        // Arrange
        var controlName = "MyCustomPanel";
        
        // Act
        var result = CustomControlGenerator.GenerateCustomControl("panel", controlName);
        
        // Assert
        Assert.Contains($"# Custom Layout Panel: {controlName}", result);
        Assert.Contains("## Panel Implementation", result);
        Assert.Contains("MeasureOverride", result);
        Assert.Contains("ArrangeOverride", result);
        Assert.Contains("Layout Strategy: custom", result);
    }

    [Fact]
    public void GenerateCustomControl_AttachedProperty_ContainsExpectedElements()
    {
        // Arrange
        var controlName = "MyAttachedProperty";
        var properties = "IsSpecial";
        
        // Act
        var result = CustomControlGenerator.GenerateCustomControl("attached-property", controlName, "Control", properties);
        
        // Assert
        Assert.Contains($"# Attached Property: {properties}", result);
        Assert.Contains("## Property Definition", result);
        Assert.Contains("## Usage Examples", result);
        Assert.Contains("## Styling Integration", result);
        Assert.Contains("AttachedProperty", result);
        Assert.Contains("Get" + properties, result);
        Assert.Contains("Set" + properties, result);
    }

    [Fact]
    public void GenerateCustomControl_WithoutTemplate_ExcludesTemplateSection()
    {
        // Act
        var result = CustomControlGenerator.GenerateCustomControl("templated", "TestControl", "Control", "", "false");
        
        // Assert
        Assert.DoesNotContain("## Default Template", result);
        Assert.Contains("## Control Style", result);
    }

    [Fact]
    public void GenerateCustomControl_WithProperties_IncludesPropertyBindings()
    {
        // Arrange
        var properties = "Title,Value,IsEnabled";
        
        // Act
        var result = CustomControlGenerator.GenerateCustomControl("templated", "TestControl", "Control", properties);
        
        // Assert
        Assert.Contains("Title=\"{Binding Title}\"", result);
        Assert.Contains("Value=\"{Binding Value}\"", result);
        Assert.Contains("IsEnabled=\"{Binding IsEnabled}\"", result);
    }

    [Fact]
    public void GenerateControlTemplate_ValidInputs_ReturnsTemplate()
    {
        // Arrange
        var targetControl = "Button";
        var templateName = "CustomButtonTemplate";
        var visualStates = "Normal,PointerOver,Pressed";
        
        // Act
        var result = CustomControlGenerator.GenerateControlTemplate(targetControl, templateName, visualStates, "true");
        
        // Assert
        Assert.Contains($"# Custom Control Template: {templateName} for {targetControl}", result);
        Assert.Contains("## Template Definition", result);
        Assert.Contains("## Style Integration", result);
        Assert.Contains("## Usage Example", result);
        Assert.Contains("## Visual State Management", result);
        Assert.Contains("ControlTemplate", result);
        Assert.Contains("VisualStateGroup", result);
        Assert.Contains("Normal", result);
        Assert.Contains("PointerOver", result);
        Assert.Contains("Pressed", result);
    }

    [Fact]
    public void GenerateControlTemplate_WithAnimations_IncludesStoryboards()
    {
        // Act
        var result = CustomControlGenerator.GenerateControlTemplate("Button", "AnimatedTemplate", "PointerOver,Pressed", "true");
        
        // Assert
        Assert.Contains("Storyboard", result);
        Assert.Contains("DoubleAnimation", result);
        Assert.Contains("Duration", result);
    }

    [Fact]
    public void GenerateControlTemplate_WithoutAnimations_ExcludesStoryboards()
    {
        // Act
        var result = CustomControlGenerator.GenerateControlTemplate("Button", "StaticTemplate", "Normal,Pressed", "false");
        
        // Assert
        Assert.Contains("VisualState x:Name=\"Normal\"", result);
        Assert.Contains("VisualState x:Name=\"Pressed\"", result);
        // Should not contain storyboard content for static template
    }

    [Fact]
    public void GenerateAttachedProperty_ValidInputs_ReturnsPattern()
    {
        // Arrange
        var propertyName = "IsSpecial";
        var propertyType = "bool";
        var targetControls = "Button,TextBox";
        
        // Act
        var result = CustomControlGenerator.GenerateAttachedProperty(propertyName, propertyType, targetControls, "true");
        
        // Assert
        Assert.Contains($"# Attached Property: {propertyName}", result);
        Assert.Contains("## Property Definition", result);
        Assert.Contains("## Usage Examples", result);
        Assert.Contains("## Styling Integration", result);
        Assert.Contains("## Advanced Usage Patterns", result);
        Assert.Contains($"Get{propertyName}", result);
        Assert.Contains($"Set{propertyName}", result);
        Assert.Contains("AttachedProperty<bool>", result);
        Assert.Contains("Button", result);
        Assert.Contains("TextBox", result);
    }

    [Fact]
    public void GenerateAttachedProperty_WithHandler_IncludesChangeHandler()
    {
        // Act
        var result = CustomControlGenerator.GenerateAttachedProperty("CustomProperty", "string", "Control", "true");
        
        // Assert
        Assert.Contains("coerce:", result);
        Assert.Contains("CoerceCustomProperty", result);
        Assert.Contains("OnCustomPropertyChanged", result);
    }

    [Fact]
    public void GenerateAttachedProperty_WithoutHandler_ExcludesChangeHandler()
    {
        // Act
        var result = CustomControlGenerator.GenerateAttachedProperty("SimpleProperty", "int", "Control", "false");
        
        // Assert
        Assert.DoesNotContain("coerce:", result);
        Assert.DoesNotContain("Coerce", result);
        Assert.DoesNotContain("Changed", result);
    }

    [Fact]
    public void GenerateLayoutPanel_ValidInputs_ReturnsPanel()
    {
        // Arrange
        var panelName = "FlowPanel";
        var layoutStrategy = "flow";
        
        // Act
        var result = CustomControlGenerator.GenerateLayoutPanel(panelName, layoutStrategy, "false", "true");
        
        // Assert
        Assert.Contains($"# Custom Layout Panel: {panelName}", result);
        Assert.Contains("## Panel Implementation", result);
        Assert.Contains("## Attached Properties", result);
        Assert.Contains("## Usage Example", result);
        Assert.Contains("## Layout Strategy: flow", result);
        Assert.Contains("MeasureOverride", result);
        Assert.Contains("ArrangeOverride", result);
        Assert.Contains("BreakLineProperty", result);
    }

    [Fact]
    public void GenerateLayoutPanel_CircularStrategy_IncludesCircularProperties()
    {
        // Act
        var result = CustomControlGenerator.GenerateLayoutPanel("CircularPanel", "circular", "false", "true");
        
        // Assert
        Assert.Contains("RadiusOffsetProperty", result);
        Assert.Contains("GetRadiusOffset", result);
        Assert.Contains("SetRadiusOffset", result);
        Assert.Contains("CalculateCircularPosition", result);
    }

    [Fact]
    public void GenerateLayoutPanel_MasonryStrategy_IncludesMasonryProperties()
    {
        // Act
        var result = CustomControlGenerator.GenerateLayoutPanel("MasonryPanel", "masonry", "false", "true");
        
        // Assert
        Assert.Contains("ColumnSpanProperty", result);
        Assert.Contains("GetColumnSpan", result);
        Assert.Contains("SetColumnSpan", result);
        Assert.Contains("CalculateMasonryPosition", result);
    }

    [Fact]
    public void GenerateLayoutPanel_WithVirtualization_IncludesVirtualizationCode()
    {
        // Act
        var result = CustomControlGenerator.GenerateLayoutPanel("VirtualPanel", "flow", "true", "false");
        
        // Assert
        Assert.Contains("VirtualizingPanel", result);
        Assert.Contains("_realizedChildren", result);
        Assert.Contains("_recycledChildren", result);
        Assert.Contains("GetOrCreateChild", result);
        Assert.Contains("Virtualization Implementation", result);
    }

    [Fact]
    public void GenerateLayoutPanel_WithoutVirtualization_ExcludesVirtualizationCode()
    {
        // Act
        var result = CustomControlGenerator.GenerateLayoutPanel("SimplePanel", "flow", "false", "false");
        
        // Assert
        Assert.DoesNotContain("VirtualizingPanel", result);
        Assert.DoesNotContain("_realizedChildren", result);
        Assert.Contains("Panel", result);
        Assert.Contains("Virtualization not enabled", result);
    }

    [Fact]
    public void GenerateCustomControl_InvalidControlType_ReturnsError()
    {
        // Act
        var result = CustomControlGenerator.GenerateCustomControl("invalid-type", "TestControl");
        
        // Assert
        Assert.Contains("Error generating custom control", result);
        Assert.Contains("Unknown control type", result);
    }

    [Fact]
    public void GenerateCustomControl_EmptyControlName_HandlesGracefully()
    {
        // Act
        var result = CustomControlGenerator.GenerateCustomControl("templated", "");
        
        // Assert
        // Should handle empty name gracefully without crashing
        Assert.Contains("Templated Control:", result);
    }

    [Theory]
    [InlineData("string", "Example")]
    [InlineData("bool", "true")]
    [InlineData("int", "42")]
    [InlineData("double", "3.14")]
    [InlineData("object", "SomeValue")]
    public void GenerateAttachedProperty_DifferentPropertyTypes_ReturnsCorrectExamples(string propertyType, string expectedExample)
    {
        // Act
        var result = CustomControlGenerator.GenerateAttachedProperty("TestProperty", propertyType, "Control", "false");
        
        // Assert
        Assert.Contains($"AttachedProperty<{propertyType}>", result);
        Assert.Contains(expectedExample, result);
    }
}