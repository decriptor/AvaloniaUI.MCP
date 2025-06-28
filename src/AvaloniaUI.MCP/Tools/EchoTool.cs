using System.ComponentModel;

using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Tools;

[McpServerToolType]
public static class EchoTool
{
    [McpServerTool, Description("Echoes the message back to the client.")]
    public static string Echo([Description("The message to echo")] string message)
        => $"Hello from AvaloniaUI MCP Server: {message}";

    [McpServerTool, Description("Gets information about the AvaloniaUI MCP server")]
    public static string GetServerInfo()
        => "AvaloniaUI MCP Server v1.0 - Provides comprehensive AvaloniaUI development assistance including project generation, XAML validation, WPF migration, and more.";
}