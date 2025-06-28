using System.ComponentModel;
using System.Text.Json;

using AvaloniaUI.MCP.Services;

using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Resources;

[McpServerResourceType]
public static class AvaloniaControlsResource
{
    [McpServerResource]
    [Description("Comprehensive reference of AvaloniaUI controls with examples and usage guidelines")]
    public static async Task<string> GetControlsReference()
    {
        return await ErrorHandlingService.SafeExecuteAsync("GetControlsReference", async () =>
        {
            var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "controls.json");

            // Use cache for both the JSON data and the formatted result
            var cacheKey = "formatted_controls_reference";

            return await ResourceCacheService.GetOrLoadResourceAsync(cacheKey, async () =>
            {
                var controlsData = await ResourceCacheService.GetOrLoadJsonResourceAsync(dataPath, TimeSpan.FromHours(1));
                return FormatControlsReference(controlsData);
            }, TimeSpan.FromMinutes(30));
        });
    }

    [McpServerResource]
    [Description("Get information about a specific AvaloniaUI control")]
    public static async Task<string> GetControlInfo([Description("Name of the control to get information about")] string controlName)
    {
        return await ErrorHandlingService.SafeExecuteAsync("GetControlInfo", async () =>
        {
            // Validate input
            var validation = InputValidationService.ValidateIdentifier(controlName, "control name");
            if (!validation.IsValid)
                return ErrorHandlingService.CreateValidationError("GetControlInfo", validation);

            var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "controls.json");
            var cacheKey = $"control_info_{controlName.ToLowerInvariant()}";

            return await ResourceCacheService.GetOrLoadResourceAsync(cacheKey, async () =>
            {
                var controlsData = await ResourceCacheService.GetOrLoadJsonResourceAsync(dataPath, TimeSpan.FromHours(1));
                var controlInfo = FindControlInfo(controlsData, controlName);
                return controlInfo ?? $"Control '{controlName}' not found in the reference";
            }, TimeSpan.FromMinutes(15));
        });
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

    private static string? FindControlInfo(JsonElement controlsData, string controlName)
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