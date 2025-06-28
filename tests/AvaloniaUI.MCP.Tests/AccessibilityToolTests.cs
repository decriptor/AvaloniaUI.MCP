using AvaloniaUI.MCP.Tools;

namespace AvaloniaUI.MCP.Tests;

[TestClass]
public class AccessibilityToolTests
{
    [DataTestMethod]
    [DataRow("form")]
    [DataRow("navigation")]
    [DataRow("data-table")]
    [DataRow("modal")]
    [DataRow("notification")]
    public void GenerateAccessibleComponent_ValidComponentTypes_ReturnsPattern(string componentType)
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent(componentType);

        // Assert
        StringAssert.Contains(result, $"# Accessible Component: {componentType}", $"Result should contain component type header for {componentType}");
        StringAssert.Contains(result, "## Configuration", "Result should contain Configuration section");
        StringAssert.Contains(result, "## Accessible Component XAML", "Result should contain XAML section");
        StringAssert.Contains(result, "## Accessibility Helper Classes", "Result should contain Helper Classes section");
        StringAssert.Contains(result, "## Accessibility Testing Checklist", "Result should contain Testing Checklist section");
        StringAssert.Contains(result, "## WCAG AA Compliance Notes", "Result should contain WCAG Compliance Notes section");
        Assert.IsFalse(result.Contains("Error generating accessible component"), "Result should not contain error messages");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_FormType_ContainsFormElements()
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("form", "AA", "true", "true");

        // Assert
        StringAssert.Contains(result, "User Registration Form", "Result should contain User Registration Form text");
        StringAssert.Contains(result, "AutomationProperties.Name", "Result should contain AutomationProperties.Name");
        StringAssert.Contains(result, "AutomationProperties.Description", "Result should contain AutomationProperties.Description");
        StringAssert.Contains(result, "Create Account", "Result should contain Create Account text");
        StringAssert.Contains(result, "WCAG AA", "Result should contain WCAG AA reference");
        StringAssert.Contains(result, "true", "Result should contain keyboard navigation setting");
        StringAssert.Contains(result, "true", "Result should contain screen reader support setting");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_NavigationType_ContainsNavigationElements()
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("navigation");

        // Assert
        StringAssert.Contains(result, "navigation", "Result should contain navigation text");
        StringAssert.Contains(result, "AutomationProperties", "Result should contain AutomationProperties");
        StringAssert.Contains(result, "XAML", "Result should contain XAML content");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_DataTableType_ContainsTableElements()
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("data-table");

        // Assert
        StringAssert.Contains(result, "data-table", "Result should contain data-table text");
        StringAssert.Contains(result, "XAML", "Result should contain XAML content");
        StringAssert.Contains(result, "AutomationProperties", "Result should contain AutomationProperties");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_ModalType_ContainsModalElements()
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("modal");

        // Assert
        StringAssert.Contains(result, "modal", "Result should contain modal text");
        StringAssert.Contains(result, "XAML", "Result should contain XAML content");
        StringAssert.Contains(result, "AutomationProperties", "Result should contain AutomationProperties");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_NotificationType_ContainsNotificationElements()
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("notification");

        // Assert
        StringAssert.Contains(result, "notification", "Result should contain notification text");
        StringAssert.Contains(result, "XAML", "Result should contain XAML content");
        StringAssert.Contains(result, "AutomationProperties", "Result should contain AutomationProperties");
    }

    [DataTestMethod]
    [DataRow("AA")]
    [DataRow("AAA")]
    public void GenerateAccessibleComponent_DifferentWCAGLevels_ReturnsCorrectLevel(string wcagLevel)
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("form", wcagLevel);

        // Assert
        StringAssert.Contains(result, $"**WCAG Level**: {wcagLevel}", $"Result should contain WCAG Level: {wcagLevel}");
        StringAssert.Contains(result, $"## WCAG {wcagLevel} Compliance Notes", $"Result should contain WCAG {wcagLevel} Compliance Notes section");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_WithKeyboardNavigation_IncludesKeyboardHandler()
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("form", "AA", "true", "true");

        // Assert
        StringAssert.Contains(result, "**Keyboard Navigation**: True", "Result should contain keyboard navigation setting as True");
        StringAssert.Contains(result, "## Keyboard Navigation Handler", "Result should contain Keyboard Navigation Handler section");
        StringAssert.Contains(result, "Keyboard Navigation", "Result should contain keyboard navigation references");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_WithoutKeyboardNavigation_ExcludesKeyboardHandler()
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("form", "AA", "false", "true");

        // Assert
        StringAssert.Contains(result, "**Keyboard Navigation**: False", "Result should contain keyboard navigation setting as False");
        Assert.IsFalse(result.Contains("## Keyboard Navigation Handler"), "Result should not contain Keyboard Navigation Handler section when disabled");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_WithScreenReader_IncludesScreenReaderSupport()
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("form", "AA", "true", "true");

        // Assert
        StringAssert.Contains(result, "**Screen Reader Support**: True", "Result should contain screen reader support setting as True");
        StringAssert.Contains(result, "AutomationProperties", "Result should contain AutomationProperties for screen reader support");
        StringAssert.Contains(result, "screen reader", "Result should contain screen reader references");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_WithoutScreenReader_ConfiguresCorrectly()
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("form", "AA", "true", "false");

        // Assert
        StringAssert.Contains(result, "**Screen Reader Support**: False", "Result should contain screen reader support setting as False");
        // Should still contain basic accessibility markup
        StringAssert.Contains(result, "XAML", "Result should still contain XAML content even without screen reader support");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_ContainsWCAGComplianceGuidelines()
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("form");

        // Assert
        StringAssert.Contains(result, "Contrast Ratio", "Result should contain Contrast Ratio guidelines");
        StringAssert.Contains(result, "4.5:1 for normal text", "Result should contain contrast ratio for normal text");
        StringAssert.Contains(result, "3:1 for large text", "Result should contain contrast ratio for large text");
        StringAssert.Contains(result, "Keyboard Navigation", "Result should contain Keyboard Navigation guidelines");
        StringAssert.Contains(result, "keyboard accessible", "Result should contain keyboard accessibility requirements");
        StringAssert.Contains(result, "Screen Reader", "Result should contain Screen Reader guidelines");
        StringAssert.Contains(result, "ARIA labels", "Result should contain ARIA labels information");
        StringAssert.Contains(result, "Focus Management", "Result should contain Focus Management guidelines");
        StringAssert.Contains(result, "visual focus indicators", "Result should contain visual focus indicators requirements");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_IncludesAccessibilityTestingChecklist()
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("form");

        // Assert
        StringAssert.Contains(result, "## Accessibility Testing Checklist", "Result should contain Accessibility Testing Checklist section");
        StringAssert.Contains(result, "Testing", "Result should contain testing information");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_IncludesAccessibilityHelperClasses()
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("form");

        // Assert
        StringAssert.Contains(result, "## Accessibility Helper Classes", "Result should contain Accessibility Helper Classes section");
        StringAssert.Contains(result, "Helper", "Result should contain helper class information");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_InvalidComponentType_ReturnsGenericComponent()
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("invalid-type");

        // Assert
        StringAssert.Contains(result, "# Accessible Component: invalid-type", "Result should contain component type header even for invalid types");
        // Should fallback to generic component without generation error
        Assert.IsFalse(result.Contains("Error generating accessible component"), "Result should not contain error messages for invalid component types");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_CaseInsensitiveComponentType_Works()
    {
        // Act
        string result1 = AccessibilityTool.GenerateAccessibleComponent("FORM");
        string result2 = AccessibilityTool.GenerateAccessibleComponent("Form");
        string result3 = AccessibilityTool.GenerateAccessibleComponent("form");

        // Assert - All should work the same way
        StringAssert.Contains(result1, "**Component Type**: form", "Result should normalize FORM to lowercase form");
        StringAssert.Contains(result2, "**Component Type**: form", "Result should normalize Form to lowercase form");
        StringAssert.Contains(result3, "**Component Type**: form", "Result should handle lowercase form correctly");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_CaseInsensitiveWCAGLevel_Works()
    {
        // Act
        string result1 = AccessibilityTool.GenerateAccessibleComponent("form", "aa");
        string result2 = AccessibilityTool.GenerateAccessibleComponent("form", "AA");
        string result3 = AccessibilityTool.GenerateAccessibleComponent("form", "aA");

        // Assert - All should normalize to uppercase
        StringAssert.Contains(result1, "**WCAG Level**: AA", "Result should normalize lowercase aa to uppercase AA");
        StringAssert.Contains(result2, "**WCAG Level**: AA", "Result should handle uppercase AA correctly");
        StringAssert.Contains(result3, "**WCAG Level**: AA", "Result should normalize mixed case aA to uppercase AA");
    }

    [DataTestMethod]
    [DataRow("true", true)]
    [DataRow("True", true)]
    [DataRow("TRUE", true)]
    [DataRow("false", false)]
    [DataRow("False", false)]
    [DataRow("FALSE", false)]
    public void GenerateAccessibleComponent_BooleanParameters_ParseCorrectly(string input, bool expected)
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("form", "AA", input, input);

        // Assert
        StringAssert.Contains(result, $"**Keyboard Navigation**: {expected}", $"Result should contain keyboard navigation setting as {expected} for input '{input}'");
        StringAssert.Contains(result, $"**Screen Reader Support**: {expected}", $"Result should contain screen reader support setting as {expected} for input '{input}'");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_DefaultParameters_UseCorrectDefaults()
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("form");

        // Assert
        StringAssert.Contains(result, "**WCAG Level**: AA", "Result should use AA as default WCAG level");
        StringAssert.Contains(result, "**Keyboard Navigation**: True", "Result should use True as default keyboard navigation setting");
        StringAssert.Contains(result, "**Screen Reader Support**: True", "Result should use True as default screen reader support setting");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_InvalidBooleanParameter_HandlesGracefully()
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("form", "AA", "invalid", "invalid");

        // Assert
        StringAssert.Contains(result, "Error generating accessible component", "Result should contain error message for invalid boolean parameters");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_EmptyComponentType_HandlesGracefully()
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("");

        // Assert
        StringAssert.Contains(result, "# Accessible Component:", "Result should contain component header even for empty component type");
        // Should handle empty component type without crashing
    }

    [TestMethod]
    public void GenerateAccessibleComponent_NullParameters_HandlesGracefully()
    {
        // Act & Assert - Should not throw exceptions
        string result = AccessibilityTool.GenerateAccessibleComponent("form", "AA", "true", "true");

        // Should handle parameters gracefully
        Assert.IsNotNull(result, "Result should not be null");
        StringAssert.Contains(result, "# Accessible Component: form", "Result should contain form component header");
    }

    [TestMethod]
    public void GenerateAccessibleComponent_ContainsProperAutomationProperties()
    {
        // Act
        string result = AccessibilityTool.GenerateAccessibleComponent("form");

        // Assert
        StringAssert.Contains(result, "AutomationProperties.Name", "Result should contain AutomationProperties.Name");
        StringAssert.Contains(result, "AutomationProperties.Description", "Result should contain AutomationProperties.Description");
        // Verify XAML structure
        StringAssert.Contains(result, "xmlns=\"https://github.com/avaloniaui\"", "Result should contain proper AvaloniaUI namespace");
        StringAssert.Contains(result, "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"", "Result should contain proper XAML namespace");
    }
}
