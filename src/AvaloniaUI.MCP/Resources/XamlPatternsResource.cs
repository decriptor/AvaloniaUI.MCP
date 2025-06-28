using System.ComponentModel;
using System.Text.Json;

using AvaloniaUI.MCP.Services;

using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Resources;

[McpServerResourceType]
public static class XamlPatternsResource
{
    [McpServerResource]
    [Description("Common XAML patterns and templates for AvaloniaUI development")]
    public static async Task<string> GetXamlPatterns()
    {
        return await ErrorHandlingService.SafeExecuteAsync("GetXamlPatterns", async () =>
        {
            string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "xaml-patterns.json");
            string cacheKey = "formatted_xaml_patterns";

            return await ResourceCacheService.GetOrLoadResourceAsync(cacheKey, async () =>
            {
                JsonElement patternsData = await ResourceCacheService.GetOrLoadJsonResourceAsync(dataPath, TimeSpan.FromHours(1));
                return FormatXamlPatterns(patternsData);
            }, TimeSpan.FromMinutes(30));
        });
    }

    [McpServerResource]
    [Description("Get a specific XAML pattern by name")]
    public static Task<string> GetXamlPattern([Description("Name of the XAML pattern to retrieve")] string patternName)
    {
        try
        {
            string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "xaml-patterns.json");
            if (!File.Exists(dataPath))
            {
                return Task.FromResult("Error: XAML patterns data not found");
            }

            string jsonContent = File.ReadAllText(dataPath);
            JsonElement patternsData = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            string? patternInfo = FindXamlPattern(patternsData, patternName);
            return Task.FromResult(patternInfo ?? $"XAML pattern '{patternName}' not found");
        }
        catch (Exception ex)
        {
            return Task.FromResult($"Error getting XAML pattern: {ex.Message}");
        }
    }

    [McpServerResource]
    [Description("Get XAML patterns for MVVM development")]
    public static Task<string> GetMvvmPatterns()
    {
        try
        {
            string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "xaml-patterns.json");
            if (!File.Exists(dataPath))
            {
                return Task.FromResult("Error: XAML patterns data not found");
            }

            string jsonContent = File.ReadAllText(dataPath);
            JsonElement patternsData = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            string mvvmPatterns = ExtractMvvmPatterns(patternsData);
            return Task.FromResult(mvvmPatterns);
        }
        catch (Exception ex)
        {
            return Task.FromResult($"Error loading MVVM patterns: {ex.Message}");
        }
    }

    private static string FormatXamlPatterns(JsonElement patternsData)
    {
        string result = "# AvaloniaUI XAML Patterns\\n\\n";
        result += "This reference contains common XAML patterns and templates for AvaloniaUI development.\\n\\n";

        if (patternsData.TryGetProperty("avalonia_xaml_patterns", out JsonElement patterns))
        {
            foreach (JsonProperty pattern in patterns.EnumerateObject())
            {
                result += FormatPattern(pattern.Value);
                result += "\\n---\\n\\n";
            }
        }

        return result;
    }

    private static string? FindXamlPattern(JsonElement patternsData, string patternName)
    {
        if (patternsData.TryGetProperty("avalonia_xaml_patterns", out JsonElement patterns))
        {
            foreach (JsonProperty pattern in patterns.EnumerateObject())
            {
                if (string.Equals(pattern.Name, patternName, StringComparison.OrdinalIgnoreCase) ||
                    (pattern.Value.TryGetProperty("name", out JsonElement nameElement) &&
                     string.Equals(nameElement.GetString(), patternName, StringComparison.OrdinalIgnoreCase)))
                {
                    return FormatPattern(pattern.Value);
                }
            }
        }
        return null;
    }

    private static string ExtractMvvmPatterns(JsonElement patternsData)
    {
        string result = "# MVVM Patterns for AvaloniaUI\\n\\n";

        if (patternsData.TryGetProperty("avalonia_xaml_patterns", out JsonElement patterns))
        {
            string[] mvvmRelatedPatterns = ["mvvm_window", "data_binding"];

            foreach (string? patternName in mvvmRelatedPatterns)
            {
                if (patterns.TryGetProperty(patternName, out JsonElement pattern))
                {
                    result += FormatPattern(pattern);
                    result += "\\n---\\n\\n";
                }
            }
        }

        return result;
    }

    private static string FormatPattern(JsonElement pattern)
    {
        string result = "";

        if (pattern.TryGetProperty("name", out JsonElement name))
        {
            result += $"## {name.GetString()}\\n\\n";
        }

        if (pattern.TryGetProperty("description", out JsonElement description))
        {
            result += $"**Description:** {description.GetString()}\\n\\n";
        }

        if (pattern.TryGetProperty("xaml", out JsonElement xaml))
        {
            result += "**XAML:**\\n```xml\\n";
            result += xaml.GetString()?.Replace("\\n", "\\n") ?? "";
            result += "\\n```\\n\\n";
        }

        if (pattern.TryGetProperty("key_points", out JsonElement keyPoints))
        {
            result += "**Key Points:**\\n";
            foreach (JsonElement point in keyPoints.EnumerateArray())
            {
                result += $"- {point.GetString()}\\n";
            }
            result += "\\n";
        }

        return result;
    }
}

