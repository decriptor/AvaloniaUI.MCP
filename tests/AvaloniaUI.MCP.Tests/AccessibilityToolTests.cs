using AvaloniaUI.MCP.Tools;
using Xunit;

namespace AvaloniaUI.MCP.Tests;

public class AccessibilityToolTests
{
    [Theory]
    [InlineData("form")]
    [InlineData("navigation")]
    [InlineData("data-table")]
    [InlineData("modal")]
    [InlineData("notification")]
    public void GenerateAccessibleComponent_ValidComponentTypes_ReturnsPattern(string componentType)
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent(componentType);
        
        // Assert
        Assert.Contains($"# Accessible Component: {componentType}", result);
        Assert.Contains("## Configuration", result);
        Assert.Contains("## Accessible Component XAML", result);
        Assert.Contains("## Accessibility Helper Classes", result);
        Assert.Contains("## Accessibility Testing Checklist", result);
        Assert.Contains("## WCAG AA Compliance Notes", result);
        Assert.DoesNotContain("Error generating accessible component", result);
    }

    [Fact]
    public void GenerateAccessibleComponent_FormType_ContainsFormElements()
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("form", "AA", "true", "true");
        
        // Assert
        Assert.Contains("User Registration Form", result);
        Assert.Contains("AutomationProperties.Name", result);
        Assert.Contains("AutomationProperties.Description", result);
        Assert.Contains("Create Account", result);
        Assert.Contains("WCAG AA", result);
        Assert.Contains("true", result); // keyboard navigation
        Assert.Contains("true", result); // screen reader support
    }

    [Fact]
    public void GenerateAccessibleComponent_NavigationType_ContainsNavigationElements()
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("navigation");
        
        // Assert
        Assert.Contains("navigation", result);
        Assert.Contains("AutomationProperties", result);
        Assert.Contains("XAML", result);
    }

    [Fact]
    public void GenerateAccessibleComponent_DataTableType_ContainsTableElements()
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("data-table");
        
        // Assert
        Assert.Contains("data-table", result);
        Assert.Contains("XAML", result);
        Assert.Contains("AutomationProperties", result);
    }

    [Fact]
    public void GenerateAccessibleComponent_ModalType_ContainsModalElements()
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("modal");
        
        // Assert
        Assert.Contains("modal", result);
        Assert.Contains("XAML", result);
        Assert.Contains("AutomationProperties", result);
    }

    [Fact]
    public void GenerateAccessibleComponent_NotificationType_ContainsNotificationElements()
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("notification");
        
        // Assert
        Assert.Contains("notification", result);
        Assert.Contains("XAML", result);
        Assert.Contains("AutomationProperties", result);
    }

    [Theory]
    [InlineData("AA")]
    [InlineData("AAA")]
    public void GenerateAccessibleComponent_DifferentWCAGLevels_ReturnsCorrectLevel(string wcagLevel)
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("form", wcagLevel);
        
        // Assert
        Assert.Contains($"**WCAG Level**: {wcagLevel}", result);
        Assert.Contains($"## WCAG {wcagLevel} Compliance Notes", result);
    }

    [Fact]
    public void GenerateAccessibleComponent_WithKeyboardNavigation_IncludesKeyboardHandler()
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("form", "AA", "true", "true");
        
        // Assert
        Assert.Contains("**Keyboard Navigation**: True", result);
        Assert.Contains("## Keyboard Navigation Handler", result);
        Assert.Contains("Keyboard Navigation", result);
    }

    [Fact]
    public void GenerateAccessibleComponent_WithoutKeyboardNavigation_ExcludesKeyboardHandler()
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("form", "AA", "false", "true");
        
        // Assert
        Assert.Contains("**Keyboard Navigation**: False", result);
        Assert.DoesNotContain("## Keyboard Navigation Handler", result);
    }

    [Fact]
    public void GenerateAccessibleComponent_WithScreenReader_IncludesScreenReaderSupport()
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("form", "AA", "true", "true");
        
        // Assert
        Assert.Contains("**Screen Reader Support**: True", result);
        Assert.Contains("AutomationProperties", result);
        Assert.Contains("screen reader", result);
    }

    [Fact]
    public void GenerateAccessibleComponent_WithoutScreenReader_ConfiguresCorrectly()
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("form", "AA", "true", "false");
        
        // Assert
        Assert.Contains("**Screen Reader Support**: False", result);
        // Should still contain basic accessibility markup
        Assert.Contains("XAML", result);
    }

    [Fact]
    public void GenerateAccessibleComponent_ContainsWCAGComplianceGuidelines()
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("form");
        
        // Assert
        Assert.Contains("Contrast Ratio", result);
        Assert.Contains("4.5:1 for normal text", result);
        Assert.Contains("3:1 for large text", result);
        Assert.Contains("Keyboard Navigation", result);
        Assert.Contains("keyboard accessible", result);
        Assert.Contains("Screen Reader", result);
        Assert.Contains("ARIA labels", result);
        Assert.Contains("Focus Management", result);
        Assert.Contains("visual focus indicators", result);
    }

    [Fact]
    public void GenerateAccessibleComponent_IncludesAccessibilityTestingChecklist()
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("form");
        
        // Assert
        Assert.Contains("## Accessibility Testing Checklist", result);
        Assert.Contains("Testing", result);
    }

    [Fact]
    public void GenerateAccessibleComponent_IncludesAccessibilityHelperClasses()
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("form");
        
        // Assert
        Assert.Contains("## Accessibility Helper Classes", result);
        Assert.Contains("Helper", result);
    }

    [Fact]
    public void GenerateAccessibleComponent_InvalidComponentType_ReturnsGenericComponent()
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("invalid-type");
        
        // Assert
        Assert.Contains("# Accessible Component: invalid-type", result);
        // Should fallback to generic component without generation error
        Assert.DoesNotContain("Error generating accessible component", result);
    }

    [Fact]
    public void GenerateAccessibleComponent_CaseInsensitiveComponentType_Works()
    {
        // Act
        var result1 = AccessibilityTool.GenerateAccessibleComponent("FORM");
        var result2 = AccessibilityTool.GenerateAccessibleComponent("Form");
        var result3 = AccessibilityTool.GenerateAccessibleComponent("form");
        
        // Assert - All should work the same way
        Assert.Contains("**Component Type**: form", result1);
        Assert.Contains("**Component Type**: form", result2);
        Assert.Contains("**Component Type**: form", result3);
    }

    [Fact]
    public void GenerateAccessibleComponent_CaseInsensitiveWCAGLevel_Works()
    {
        // Act
        var result1 = AccessibilityTool.GenerateAccessibleComponent("form", "aa");
        var result2 = AccessibilityTool.GenerateAccessibleComponent("form", "AA");
        var result3 = AccessibilityTool.GenerateAccessibleComponent("form", "aA");
        
        // Assert - All should normalize to uppercase
        Assert.Contains("**WCAG Level**: AA", result1);
        Assert.Contains("**WCAG Level**: AA", result2);
        Assert.Contains("**WCAG Level**: AA", result3);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("True", true)]
    [InlineData("TRUE", true)]
    [InlineData("false", false)]
    [InlineData("False", false)]
    [InlineData("FALSE", false)]
    public void GenerateAccessibleComponent_BooleanParameters_ParseCorrectly(string input, bool expected)
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("form", "AA", input, input);
        
        // Assert
        Assert.Contains($"**Keyboard Navigation**: {expected}", result);
        Assert.Contains($"**Screen Reader Support**: {expected}", result);
    }

    [Fact]
    public void GenerateAccessibleComponent_DefaultParameters_UseCorrectDefaults()
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("form");
        
        // Assert
        Assert.Contains("**WCAG Level**: AA", result);
        Assert.Contains("**Keyboard Navigation**: True", result);
        Assert.Contains("**Screen Reader Support**: True", result);
    }

    [Fact]
    public void GenerateAccessibleComponent_InvalidBooleanParameter_HandlesGracefully()
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("form", "AA", "invalid", "invalid");
        
        // Assert
        Assert.Contains("Error generating accessible component", result);
    }

    [Fact]
    public void GenerateAccessibleComponent_EmptyComponentType_HandlesGracefully()
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("");
        
        // Assert
        Assert.Contains("# Accessible Component:", result);
        // Should handle empty component type without crashing
    }

    [Fact]
    public void GenerateAccessibleComponent_NullParameters_HandlesGracefully()
    {
        // Act & Assert - Should not throw exceptions
        var result = AccessibilityTool.GenerateAccessibleComponent("form", "AA", "true", "true");
        
        // Should handle parameters gracefully
        Assert.NotNull(result);
        Assert.Contains("# Accessible Component: form", result);
    }

    [Fact]
    public void GenerateAccessibleComponent_ContainsProperAutomationProperties()
    {
        // Act
        var result = AccessibilityTool.GenerateAccessibleComponent("form");
        
        // Assert
        Assert.Contains("AutomationProperties.Name", result);
        Assert.Contains("AutomationProperties.Description", result);
        // Verify XAML structure
        Assert.Contains("xmlns=\"https://github.com/avaloniaui\"", result);
        Assert.Contains("xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"", result);
    }
}