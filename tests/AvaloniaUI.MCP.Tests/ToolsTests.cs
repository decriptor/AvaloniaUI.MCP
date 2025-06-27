using AvaloniaUI.MCP.Tools;

namespace AvaloniaUI.MCP.Tests;

[TestClass]
public class ToolsTests
{
    [TestMethod]
    public void EchoTool_ReturnsCorrectMessage()
    {
        // Arrange
        string message = "Hello World";

        // Act
        string result = EchoTool.Echo(message);

        // Assert
        StringAssert.Contains(result, "Hello from AvaloniaUI MCP Server", "Result should contain server greeting");
        StringAssert.Contains(result, message, "Result should contain the original message");
    }

    [TestMethod]
    public void EchoTool_GetServerInfo_ReturnsServerInformation()
    {
        // Act
        string result = EchoTool.GetServerInfo();

        // Assert
        StringAssert.Contains(result, "AvaloniaUI MCP Server", "Result should contain server name");
        StringAssert.Contains(result, "project generation", "Result should mention project generation capabilities");
        StringAssert.Contains(result, "XAML validation", "Result should mention XAML validation capabilities");
    }

    [TestMethod]
    public void ProjectGeneratorTool_CreateAvaloniaProject_ValidatesProjectName()
    {
        // Arrange
        string emptyProjectName = "";

        // Act
        string result = ProjectGeneratorTool.CreateAvaloniaProject(emptyProjectName);

        // Assert
        StringAssert.Contains(result, "# ❌ Error", "Result should contain error header");
        StringAssert.Contains(result, "Project name cannot be empty", "Result should contain specific validation error message");
    }

    [TestMethod]
    public void ProjectGeneratorTool_CreateAvaloniaProject_ValidatesTemplate()
    {
        // Arrange
        string projectName = "TestProject";
        string invalidTemplate = "invalid";

        // Act
        string result = ProjectGeneratorTool.CreateAvaloniaProject(projectName, invalidTemplate);

        // Assert
        StringAssert.Contains(result, "# ❌ Error", "Result should contain error header");
        StringAssert.Contains(result, "Invalid template type", "Result should contain template validation error");
        StringAssert.Contains(result, "invalid", "Result should contain the invalid template name");
    }

    [TestMethod]
    public void XamlValidationTool_ValidateXaml_RejectsEmptyContent()
    {
        // Arrange
        string emptyXaml = "";

        // Act
        string result = XamlValidationTool.ValidateXaml(emptyXaml);

        // Assert
        StringAssert.Contains(result, "# ❌ Error", "Result should contain error header");
        StringAssert.Contains(result, "XAML content cannot be empty", "Result should contain empty content validation error");
    }

    [TestMethod]
    public void XamlValidationTool_ValidateXaml_AcceptsValidXaml()
    {
        // Arrange
        string validXaml = @"<Window xmlns=""https://github.com/avaloniaui""
                                 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                             <TextBlock Text=""Hello World"" />
                          </Window>";

        // Act
        string result = XamlValidationTool.ValidateXaml(validXaml);

        // Assert
        StringAssert.Contains(result, "✅ XAML Validation Passed", "Result should contain validation success message");
        StringAssert.Contains(result, "✓ XML syntax is valid", "Result should confirm XML syntax validity");
    }

    [TestMethod]
    public void XamlValidationTool_ConvertWpfXamlToAvalonia_RejectsEmptyContent()
    {
        // Arrange
        string emptyXaml = "";

        // Act
        string result = XamlValidationTool.ConvertWpfXamlToAvalonia(emptyXaml);

        // Assert
        StringAssert.Contains(result, "Error: WPF XAML content cannot be empty", "Result should contain empty content error message");
    }

    [TestMethod]
    public void XamlValidationTool_ConvertWpfXamlToAvalonia_ConvertsNamespaces()
    {
        // Arrange
        string wpfXaml = @"<Window xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                               xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                           <TextBlock Text=""Hello World"" />
                        </Window>";

        // Act
        string result = XamlValidationTool.ConvertWpfXamlToAvalonia(wpfXaml);

        // Assert
        StringAssert.Contains(result, "🔄 WPF to AvaloniaUI XAML Conversion Complete", "Result should contain conversion completion message");
        StringAssert.Contains(result, "https://github.com/avaloniaui", "Result should contain AvaloniaUI namespace");
        StringAssert.Contains(result, "✓ Replaced WPF presentation namespace", "Result should confirm namespace replacement");
    }
}