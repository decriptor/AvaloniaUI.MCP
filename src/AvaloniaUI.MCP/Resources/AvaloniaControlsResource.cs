using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Resources;

[McpServerResourceType]
public static class AvaloniaControlsResource
{
    [McpServerResource]
    [Description("Comprehensive reference of AvaloniaUI controls with examples and usage guidelines")]
    public static Task<string> GetControlsReference()
    {
        try
        {
            var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "controls.json");
            if (!File.Exists(dataPath))
            {
                return Task.FromResult("Error: Controls reference data not found");
            }

            var jsonContent = File.ReadAllText(dataPath);
            var controlsData = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            var formattedContent = FormatControlsReference(controlsData);
            return Task.FromResult(formattedContent);
        }
        catch (Exception ex)
        {
            return Task.FromResult($"Error loading controls reference: {ex.Message}");
        }
    }

    [McpServerResource]
    [Description("Get information about a specific AvaloniaUI control")]
    public static Task<string> GetControlInfo([Description("Name of the control to get information about")] string controlName)
    {
        try
        {
            var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "controls.json");
            if (!File.Exists(dataPath))
            {
                return Task.FromResult("Error: Controls reference data not found");
            }

            var jsonContent = File.ReadAllText(dataPath);
            var controlsData = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            var controlInfo = FindControlInfo(controlsData, controlName);
            return Task.FromResult(controlInfo ?? $"Control '{controlName}' not found in the reference");
        }
        catch (Exception ex)
        {
            return Task.FromResult($"Error getting control info: {ex.Message}");
        }
    }

    private static string FormatControlsReference(JsonElement controlsData)
    {
        var result = "# AvaloniaUI Controls Reference\\n\\n";

        if (controlsData.TryGetProperty("avaloniaui_controls", out var controls))
        {
            foreach (var category in controls.EnumerateObject())
            {
                result += $"## {FormatCategoryName(category.Name)}\\n\\n";

                foreach (var control in category.Value.EnumerateObject())
                {
                    result += FormatControlInfo(control.Name, control.Value);
                    result += "\\n---\\n\\n";
                }
            }
        }

        return result;
    }

    private static string FindControlInfo(JsonElement controlsData, string controlName)
    {
        if (controlsData.TryGetProperty("avaloniaui_controls", out var controls))
        {
            foreach (var category in controls.EnumerateObject())
            {
                foreach (var control in category.Value.EnumerateObject())
                {
                    if (string.Equals(control.Name, controlName, StringComparison.OrdinalIgnoreCase))
                    {
                        return FormatControlInfo(control.Name, control.Value);
                    }
                }
            }
        }
        return null;
    }

    private static string FormatControlInfo(string controlName, JsonElement controlData)
    {
        var result = $"### {controlName}\\n\\n";

        if (controlData.TryGetProperty("description", out var description))
        {
            result += $"**Description:** {description.GetString()}\\n\\n";
        }

        if (controlData.TryGetProperty("usage", out var usage))
        {
            result += $"**Usage:** {usage.GetString()}\\n\\n";
        }

        if (controlData.TryGetProperty("properties", out var properties))
        {
            result += "**Key Properties:**\\n";
            foreach (var prop in properties.EnumerateArray())
            {
                result += $"- {prop.GetString()}\\n";
            }
            result += "\\n";
        }

        if (controlData.TryGetProperty("xaml_example", out var xamlExample))
        {
            result += "**XAML Example:**\\n```xml\\n";
            result += xamlExample.GetString()?.Replace("\\n", "\\n") ?? "";
            result += "\\n```\\n\\n";
        }

        return result;
    }

    private static string FormatCategoryName(string categoryName)
    {
        return categoryName.Replace("_", " ")
            .Split(' ')
            .Select(word => char.ToUpper(word[0]) + word.Substring(1))
            .Aggregate((a, b) => a + " " + b);
    }
}