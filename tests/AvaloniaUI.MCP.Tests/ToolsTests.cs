using AvaloniaUI.MCP.Tools;
using Xunit;

namespace AvaloniaUI.MCP.Tests;

public class ToolsTests
{
    [Fact]
    public void EchoTool_ReturnsCorrectMessage()
    {
        // Arrange
        var message = "Hello World";
        
        // Act
        var result = EchoTool.Echo(message);
        
        // Assert
        Assert.Contains("Hello from AvaloniaUI MCP Server", result);
        Assert.Contains(message, result);
    }

    [Fact]
    public void EchoTool_GetServerInfo_ReturnsServerInformation()
    {
        // Act
        var result = EchoTool.GetServerInfo();
        
        // Assert
        Assert.Contains("AvaloniaUI MCP Server", result);
        Assert.Contains("project generation", result);
        Assert.Contains("XAML validation", result);
    }

    [Fact]
    public void ProjectGeneratorTool_CreateAvaloniaProject_ValidatesProjectName()
    {
        // Arrange
        var emptyProjectName = "";
        
        // Act
        var result = ProjectGeneratorTool.CreateAvaloniaProject(emptyProjectName);
        
        // Assert
        Assert.Contains("Error: Project name cannot be empty", result);
    }

    [Fact]
    public void ProjectGeneratorTool_CreateAvaloniaProject_ValidatesTemplate()
    {
        // Arrange
        var projectName = "TestProject";
        var invalidTemplate = "invalid";
        
        // Act
        var result = ProjectGeneratorTool.CreateAvaloniaProject(projectName, invalidTemplate);
        
        // Assert
        Assert.Contains("Error: Invalid template", result);
        Assert.Contains("invalid", result);
    }

    [Fact]
    public void XamlValidationTool_ValidateXaml_RejectsEmptyContent()
    {
        // Arrange
        var emptyXaml = "";
        
        // Act
        var result = XamlValidationTool.ValidateXaml(emptyXaml);
        
        // Assert
        Assert.Contains("Error: XAML content cannot be empty", result);
    }

    [Fact]
    public void XamlValidationTool_ValidateXaml_AcceptsValidXaml()
    {
        // Arrange
        var validXaml = @"<Window xmlns=""https://github.com/avaloniaui""
                                 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                             <TextBlock Text=""Hello World"" />
                          </Window>";
        
        // Act
        var result = XamlValidationTool.ValidateXaml(validXaml);
        
        // Assert
        Assert.Contains("✅ XAML Validation Passed", result);
        Assert.Contains("✓ XML syntax is valid", result);
    }

    [Fact]
    public void XamlValidationTool_ConvertWpfXamlToAvalonia_RejectsEmptyContent()
    {
        // Arrange
        var emptyXaml = "";
        
        // Act
        var result = XamlValidationTool.ConvertWpfXamlToAvalonia(emptyXaml);
        
        // Assert
        Assert.Contains("Error: WPF XAML content cannot be empty", result);
    }

    [Fact]
    public void XamlValidationTool_ConvertWpfXamlToAvalonia_ConvertsNamespaces()
    {
        // Arrange
        var wpfXaml = @"<Window xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                               xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                           <TextBlock Text=""Hello World"" />
                        </Window>";
        
        // Act
        var result = XamlValidationTool.ConvertWpfXamlToAvalonia(wpfXaml);
        
        // Assert
        Assert.Contains("🔄 WPF to AvaloniaUI XAML Conversion Complete", result);
        Assert.Contains("https://github.com/avaloniaui", result);
        Assert.Contains("✓ Replaced WPF presentation namespace", result);
    }
}