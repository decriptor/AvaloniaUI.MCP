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
            string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "controls.json");

            // Use cache for both the JSON data and the formatted result
            string cacheKey = "formatted_controls_reference";

            return await ResourceCacheService.GetOrLoadResourceAsync(cacheKey, async () =>
            {
                JsonElement controlsData = await ResourceCacheService.GetOrLoadJsonResourceAsync(dataPath, TimeSpan.FromHours(1));
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
            ValidationResult validation = InputValidationService.ValidateIdentifier(controlName, "control name");
            if (!validation.IsValid)
            {
                return ErrorHandlingService.CreateValidationError("GetControlInfo", validation);
            }

            string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "controls.json");
            string cacheKey = $"control_info_{controlName.ToLowerInvariant()}";

            return await ResourceCacheService.GetOrLoadResourceAsync(cacheKey, async () =>
            {
                JsonElement controlsData = await ResourceCacheService.GetOrLoadJsonResourceAsync(dataPath, TimeSpan.FromHours(1));
                string? controlInfo = FindControlInfo(controlsData, controlName);
                return controlInfo ?? $"Control '{controlName}' not found in the reference";
            }, TimeSpan.FromMinutes(15));
        });
    }

    static string FormatControlsReference(JsonElement controlsData)
    {
        string result = "# AvaloniaUI Controls Reference\\n\\n";

        if (controlsData.TryGetProperty("avaloniaui_controls", out JsonElement controls))
        {
            foreach (JsonProperty category in controls.EnumerateObject())
            {
                result += $"## {FormatCategoryName(category.Name)}\\n\\n";

                foreach (JsonProperty control in category.Value.EnumerateObject())
                {
                    result += FormatControlInfo(control.Name, control.Value);
                    result += "\\n---\\n\\n";
                }
            }
        }

        return result;
    }

    static string? FindControlInfo(JsonElement controlsData, string controlName)
    {
        if (controlsData.TryGetProperty("avaloniaui_controls", out JsonElement controls))
        {
            foreach (JsonProperty category in controls.EnumerateObject())
            {
                foreach (JsonProperty control in category.Value.EnumerateObject())
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

    static string FormatControlInfo(string controlName, JsonElement controlData)
    {
        string result = $"### {controlName}\\n\\n";

        if (controlData.TryGetProperty("description", out JsonElement description))
        {
            result += $"**Description:** {description.GetString()}\\n\\n";
        }

        if (controlData.TryGetProperty("usage", out JsonElement usage))
        {
            result += $"**Usage:** {usage.GetString()}\\n\\n";
        }

        if (controlData.TryGetProperty("properties", out JsonElement properties))
        {
            result += "**Key Properties:**\\n";
            foreach (JsonElement prop in properties.EnumerateArray())
            {
                result += $"- {prop.GetString()}\\n";
            }
            result += "\\n";
        }

        if (controlData.TryGetProperty("xaml_example", out JsonElement xamlExample))
        {
            result += "**XAML Example:**\\n```xml\\n";
            result += xamlExample.GetString()?.Replace("\\n", "\\n") ?? "";
            result += "\\n```\\n\\n";
        }

        return result;
    }

    static string FormatCategoryName(string categoryName)
    {
        return categoryName.Replace("_", " ")
            .Split(' ')
            .Select(word => char.ToUpper(word[0]) + word[1..])
            .Aggregate((a, b) => a + " " + b);
    }
}