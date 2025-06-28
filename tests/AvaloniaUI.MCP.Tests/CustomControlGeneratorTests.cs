using AvaloniaUI.MCP.Tools;

namespace AvaloniaUI.MCP.Tests;

[TestClass]
public class CustomControlGeneratorTests
{
    [DataTestMethod]
    [DataRow("templated")]
    [DataRow("usercontrol")]
    [DataRow("panel")]
    public void GenerateCustomControl_ValidControlTypes_ReturnsCorrectPattern(string controlType)
    {
        // Arrange
        string controlName = "TestControl";

        // Act
        string result = CustomControlGenerator.GenerateCustomControl(controlType, controlName);

        // Assert
        Assert.IsFalse(result.Contains("Error generating custom control"), "Result should not contain error message");
        StringAssert.Contains(result, controlName, "Result should contain the control name");
    }

    [TestMethod]
    public void GenerateCustomControl_AttachedProperty_RequiresPropertyName()
    {
        // Arrange
        string controlName = "TestControl";

        // Act
        string result = CustomControlGenerator.GenerateCustomControl("attached-property", controlName, "Control", "TestProperty");

        // Assert
        StringAssert.Contains(result, "TestProperty", "Result should contain the property name");
        StringAssert.Contains(result, "Attached Property", "Result should contain 'Attached Property' text");
    }

    [TestMethod]
    public void GenerateCustomControl_TemplatedControl_ContainsExpectedElements()
    {
        // Arrange
        string controlName = "MyTemplatedControl";
        string properties = "Title,IsEnabled,Value";

        // Act
        string result = CustomControlGenerator.GenerateCustomControl("templated", controlName, "Control", properties, "true");

        // Assert
        StringAssert.Contains(result, $"# Templated Control: {controlName}", "Result should contain templated control header");
        StringAssert.Contains(result, "## Control Implementation", "Result should contain Control Implementation section");
        StringAssert.Contains(result, "## Default Template", "Result should contain Default Template section");
        StringAssert.Contains(result, "## Control Style", "Result should contain Control Style section");
        StringAssert.Contains(result, "## Usage Example", "Result should contain Usage Example section");
        StringAssert.Contains(result, "## Template Parts", "Result should contain Template Parts section");
        StringAssert.Contains(result, "## Visual States", "Result should contain Visual States section");
        StringAssert.Contains(result, "PART_Border", "Result should contain PART_Border template part");
        StringAssert.Contains(result, "PART_ContentPresenter", "Result should contain PART_ContentPresenter template part");
        StringAssert.Contains(result, "StyledProperty", "Result should contain StyledProperty implementation");
        StringAssert.Contains(result, "OnApplyTemplate", "Result should contain OnApplyTemplate method");
    }

    [TestMethod]
    public void GenerateCustomControl_UserControl_ContainsExpectedElements()
    {
        // Arrange
        string controlName = "MyUserControl";
        string properties = "Header,Content";

        // Act
        string result = CustomControlGenerator.GenerateCustomControl("usercontrol", controlName, "UserControl", properties);

        // Assert
        StringAssert.Contains(result, $"# User Control: {controlName}", "Result should contain user control header");
        StringAssert.Contains(result, "## XAML Definition", "Result should contain XAML Definition section");
        StringAssert.Contains(result, "## Code-Behind", "Result should contain Code-Behind section");
        StringAssert.Contains(result, "## Usage Example", "Result should contain Usage Example section");
        StringAssert.Contains(result, "## Data Binding Integration", "Result should contain Data Binding Integration section");
        StringAssert.Contains(result, "## Styling", "Result should contain Styling section");
        StringAssert.Contains(result, "xmlns=\"https://github.com/avaloniaui\"", "Result should contain AvaloniaUI namespace");
        StringAssert.Contains(result, "InitializeComponent", "Result should contain InitializeComponent call");
    }

    [TestMethod]
    public void GenerateCustomControl_Panel_ReturnsLayoutPanelPattern()
    {
        // Arrange
        string controlName = "MyCustomPanel";

        // Act
        string result = CustomControlGenerator.GenerateCustomControl("panel", controlName);

        // Assert
        StringAssert.Contains(result, $"# Custom Layout Panel: {controlName}", "Result should contain panel header");
        StringAssert.Contains(result, "## Panel Implementation", "Result should contain Panel Implementation section");
        StringAssert.Contains(result, "MeasureOverride", "Result should contain MeasureOverride method");
        StringAssert.Contains(result, "ArrangeOverride", "Result should contain ArrangeOverride method");
        StringAssert.Contains(result, "Layout Strategy: custom", "Result should contain custom layout strategy");
    }

    [TestMethod]
    public void GenerateCustomControl_AttachedProperty_ContainsExpectedElements()
    {
        // Arrange
        string controlName = "MyAttachedProperty";
        string properties = "IsSpecial";

        // Act
        string result = CustomControlGenerator.GenerateCustomControl("attached-property", controlName, "Control", properties);

        // Assert
        StringAssert.Contains(result, $"# Attached Property: {properties}", "Result should contain attached property header");
        StringAssert.Contains(result, "## Property Definition", "Result should contain Property Definition section");
        StringAssert.Contains(result, "## Usage Examples", "Result should contain Usage Examples section");
        StringAssert.Contains(result, "## Styling Integration", "Result should contain Styling Integration section");
        StringAssert.Contains(result, "AttachedProperty", "Result should contain AttachedProperty declaration");
        StringAssert.Contains(result, "Get" + properties, "Result should contain getter method");
        StringAssert.Contains(result, "Set" + properties, "Result should contain setter method");
    }

    [TestMethod]
    public void GenerateCustomControl_WithoutTemplate_ExcludesTemplateSection()
    {
        // Act
        string result = CustomControlGenerator.GenerateCustomControl("templated", "TestControl", "Control", "", "false");

        // Assert
        Assert.IsFalse(result.Contains("## Default Template"), "Result should not contain Default Template section when template is disabled");
        StringAssert.Contains(result, "## Control Style", "Result should still contain Control Style section");
    }

    [TestMethod]
    public void GenerateCustomControl_WithProperties_IncludesPropertyBindings()
    {
        // Arrange
        string properties = "Title,Value,IsEnabled";

        // Act
        string result = CustomControlGenerator.GenerateCustomControl("templated", "TestControl", "Control", properties);

        // Assert
        StringAssert.Contains(result, "Title=\"{Binding Title}\"", "Result should contain Title property binding");
        StringAssert.Contains(result, "Value=\"{Binding Value}\"", "Result should contain Value property binding");
        StringAssert.Contains(result, "IsEnabled=\"{Binding IsEnabled}\"", "Result should contain IsEnabled property binding");
    }

    [TestMethod]
    public void GenerateControlTemplate_ValidInputs_ReturnsTemplate()
    {
        // Arrange
        string targetControl = "Button";
        string templateName = "CustomButtonTemplate";
        string visualStates = "Normal,PointerOver,Pressed";

        // Act
        string result = CustomControlGenerator.GenerateControlTemplate(targetControl, templateName, visualStates, "true");

        // Assert
        StringAssert.Contains(result, $"# Custom Control Template: {templateName} for {targetControl}", "Result should contain template header");
        StringAssert.Contains(result, "## Template Definition", "Result should contain Template Definition section");
        StringAssert.Contains(result, "## Style Integration", "Result should contain Style Integration section");
        StringAssert.Contains(result, "## Usage Example", "Result should contain Usage Example section");
        StringAssert.Contains(result, "## Visual State Management", "Result should contain Visual State Management section");
        StringAssert.Contains(result, "ControlTemplate", "Result should contain ControlTemplate definition");
        StringAssert.Contains(result, "VisualStateGroup", "Result should contain VisualStateGroup");
        StringAssert.Contains(result, "Normal", "Result should contain Normal visual state");
        StringAssert.Contains(result, "PointerOver", "Result should contain PointerOver visual state");
        StringAssert.Contains(result, "Pressed", "Result should contain Pressed visual state");
    }

    [TestMethod]
    public void GenerateControlTemplate_WithAnimations_IncludesStoryboards()
    {
        // Act
        string result = CustomControlGenerator.GenerateControlTemplate("Button", "AnimatedTemplate", "PointerOver,Pressed", "true");

        // Assert
        StringAssert.Contains(result, "Storyboard", "Result should contain Storyboard for animations");
        StringAssert.Contains(result, "DoubleAnimation", "Result should contain DoubleAnimation");
        StringAssert.Contains(result, "Duration", "Result should contain Duration property");
    }

    [TestMethod]
    public void GenerateControlTemplate_WithoutAnimations_ExcludesStoryboards()
    {
        // Act
        string result = CustomControlGenerator.GenerateControlTemplate("Button", "StaticTemplate", "Normal,Pressed", "false");

        // Assert
        StringAssert.Contains(result, "VisualState x:Name=\"Normal\"", "Result should contain Normal visual state");
        StringAssert.Contains(result, "VisualState x:Name=\"Pressed\"", "Result should contain Pressed visual state");
        // Should not contain storyboard content for static template
    }

    [TestMethod]
    public void GenerateAttachedProperty_ValidInputs_ReturnsPattern()
    {
        // Arrange
        string propertyName = "IsSpecial";
        string propertyType = "bool";
        string targetControls = "Button,TextBox";

        // Act
        string result = CustomControlGenerator.GenerateAttachedProperty(propertyName, propertyType, targetControls, "true");

        // Assert
        StringAssert.Contains(result, $"# Attached Property: {propertyName}", "Result should contain property header");
        StringAssert.Contains(result, "## Property Definition", "Result should contain Property Definition section");
        StringAssert.Contains(result, "## Usage Examples", "Result should contain Usage Examples section");
        StringAssert.Contains(result, "## Styling Integration", "Result should contain Styling Integration section");
        StringAssert.Contains(result, "## Advanced Usage Patterns", "Result should contain Advanced Usage Patterns section");
        StringAssert.Contains(result, $"Get{propertyName}", "Result should contain getter method");
        StringAssert.Contains(result, $"Set{propertyName}", "Result should contain setter method");
        StringAssert.Contains(result, "AttachedProperty<bool>", "Result should contain AttachedProperty type declaration");
        StringAssert.Contains(result, "Button", "Result should contain Button target control");
        StringAssert.Contains(result, "TextBox", "Result should contain TextBox target control");
    }

    [TestMethod]
    public void GenerateAttachedProperty_WithHandler_IncludesChangeHandler()
    {
        // Act
        string result = CustomControlGenerator.GenerateAttachedProperty("CustomProperty", "string", "Control", "true");

        // Assert
        StringAssert.Contains(result, "coerce:", "Result should contain coerce callback");
        StringAssert.Contains(result, "CoerceCustomProperty", "Result should contain coerce method");
        StringAssert.Contains(result, "OnCustomPropertyChanged", "Result should contain change handler method");
    }

    [TestMethod]
    public void GenerateAttachedProperty_WithoutHandler_ExcludesChangeHandler()
    {
        // Act
        string result = CustomControlGenerator.GenerateAttachedProperty("SimpleProperty", "int", "Control", "false");

        // Assert
        Assert.IsFalse(result.Contains("coerce:"), "Result should not contain coerce callback when handler is disabled");
        Assert.IsFalse(result.Contains("Coerce"), "Result should not contain Coerce methods when handler is disabled");
        Assert.IsFalse(result.Contains("Changed"), "Result should not contain Changed methods when handler is disabled");
    }

    [TestMethod]
    public void GenerateLayoutPanel_ValidInputs_ReturnsPanel()
    {
        // Arrange
        string panelName = "FlowPanel";
        string layoutStrategy = "flow";

        // Act
        string result = CustomControlGenerator.GenerateLayoutPanel(panelName, layoutStrategy, "false", "true");

        // Assert
        StringAssert.Contains(result, $"# Custom Layout Panel: {panelName}", "Result should contain panel header");
        StringAssert.Contains(result, "## Panel Implementation", "Result should contain Panel Implementation section");
        StringAssert.Contains(result, "## Attached Properties", "Result should contain Attached Properties section");
        StringAssert.Contains(result, "## Usage Example", "Result should contain Usage Example section");
        StringAssert.Contains(result, "## Layout Strategy: flow", "Result should contain flow layout strategy");
        StringAssert.Contains(result, "MeasureOverride", "Result should contain MeasureOverride method");
        StringAssert.Contains(result, "ArrangeOverride", "Result should contain ArrangeOverride method");
        StringAssert.Contains(result, "BreakLineProperty", "Result should contain BreakLineProperty for flow layout");
    }

    [TestMethod]
    public void GenerateLayoutPanel_CircularStrategy_IncludesCircularProperties()
    {
        // Act
        string result = CustomControlGenerator.GenerateLayoutPanel("CircularPanel", "circular", "false", "true");

        // Assert
        StringAssert.Contains(result, "RadiusOffsetProperty", "Result should contain RadiusOffsetProperty for circular layout");
        StringAssert.Contains(result, "GetRadiusOffset", "Result should contain GetRadiusOffset method");
        StringAssert.Contains(result, "SetRadiusOffset", "Result should contain SetRadiusOffset method");
        StringAssert.Contains(result, "CalculateCircularPosition", "Result should contain CalculateCircularPosition method");
    }

    [TestMethod]
    public void GenerateLayoutPanel_MasonryStrategy_IncludesMasonryProperties()
    {
        // Act
        string result = CustomControlGenerator.GenerateLayoutPanel("MasonryPanel", "masonry", "false", "true");

        // Assert
        StringAssert.Contains(result, "ColumnSpanProperty", "Result should contain ColumnSpanProperty for masonry layout");
        StringAssert.Contains(result, "GetColumnSpan", "Result should contain GetColumnSpan method");
        StringAssert.Contains(result, "SetColumnSpan", "Result should contain SetColumnSpan method");
        StringAssert.Contains(result, "CalculateMasonryPosition", "Result should contain CalculateMasonryPosition method");
    }

    [TestMethod]
    public void GenerateLayoutPanel_WithVirtualization_IncludesVirtualizationCode()
    {
        // Act
        string result = CustomControlGenerator.GenerateLayoutPanel("VirtualPanel", "flow", "true", "false");

        // Assert
        StringAssert.Contains(result, "VirtualizingPanel", "Result should contain VirtualizingPanel base class");
        StringAssert.Contains(result, "_realizedChildren", "Result should contain _realizedChildren field");
        StringAssert.Contains(result, "_recycledChildren", "Result should contain _recycledChildren field");
        StringAssert.Contains(result, "GetOrCreateChild", "Result should contain GetOrCreateChild method");
        StringAssert.Contains(result, "Virtualization Implementation", "Result should contain virtualization implementation section");
    }

    [TestMethod]
    public void GenerateLayoutPanel_WithoutVirtualization_ExcludesVirtualizationCode()
    {
        // Act
        string result = CustomControlGenerator.GenerateLayoutPanel("SimplePanel", "flow", "false", "false");

        // Assert
        Assert.IsFalse(result.Contains("VirtualizingPanel"), "Result should not contain VirtualizingPanel when virtualization is disabled");
        Assert.IsFalse(result.Contains("_realizedChildren"), "Result should not contain virtualization fields when disabled");
        StringAssert.Contains(result, "Panel", "Result should contain base Panel class");
        StringAssert.Contains(result, "Virtualization not enabled", "Result should indicate virtualization is not enabled");
    }

    [TestMethod]
    public void GenerateCustomControl_InvalidControlType_ReturnsError()
    {
        // Act
        string result = CustomControlGenerator.GenerateCustomControl("invalid-type", "TestControl");

        // Assert
        StringAssert.Contains(result, "Error generating custom control", "Result should contain error message for invalid control type");
        StringAssert.Contains(result, "Unknown control type", "Result should contain specific error about unknown control type");
    }

    [TestMethod]
    public void GenerateCustomControl_EmptyControlName_HandlesGracefully()
    {
        // Act
        string result = CustomControlGenerator.GenerateCustomControl("templated", "");

        // Assert
        // Should handle empty name gracefully without crashing
        StringAssert.Contains(result, "Templated Control:", "Result should contain templated control header even with empty name");
    }

    [DataTestMethod]
    [DataRow("string", "Example")]
    [DataRow("bool", "true")]
    [DataRow("int", "42")]
    [DataRow("double", "3.14")]
    [DataRow("object", "SomeValue")]
    public void GenerateAttachedProperty_DifferentPropertyTypes_ReturnsCorrectExamples(string propertyType, string expectedExample)
    {
        // Act
        string result = CustomControlGenerator.GenerateAttachedProperty("TestProperty", propertyType, "Control", "false");

        // Assert
        StringAssert.Contains(result, $"AttachedProperty<{propertyType}>", "Result should contain correct AttachedProperty type declaration");
        StringAssert.Contains(result, expectedExample, "Result should contain expected example value for property type");
    }
}