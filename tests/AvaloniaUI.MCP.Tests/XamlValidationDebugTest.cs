using AvaloniaUI.MCP.Tools;
using Xunit;
using Xunit.Abstractions;

namespace AvaloniaUI.MCP.Tests;

public class XamlValidationDebugTest
{
    private readonly ITestOutputHelper _output;

    public XamlValidationDebugTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
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
        _output.WriteLine($"Validation result: {result}");
        
        // Just verify it doesn't crash
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}