using AvaloniaUI.MCP.Tools;

namespace AvaloniaUI.MCP.Tests;

[TestClass]
public class XamlValidationDebugTest
{
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    public void Debug_XamlValidation_Result()
    {
        // Arrange
        var validXaml = @"<Window xmlns=""https://github.com/avaloniaui""
                                 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                             <TextBlock Text=""Hello World"" />
                          </Window>";

        // Act
        var result = XamlValidationTool.ValidateXaml(validXaml);

        // Assert
        TestContext.WriteLine($"Validation result: {result}");

        // Just verify it doesn't crash
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0, "Result should not be empty");
    }
}