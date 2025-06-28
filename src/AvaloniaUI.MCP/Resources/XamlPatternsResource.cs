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
            var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "xaml-patterns.json");
            var cacheKey = "formatted_xaml_patterns";

            return await ResourceCacheService.GetOrLoadResourceAsync(cacheKey, async () =>
            {
                var patternsData = await ResourceCacheService.GetOrLoadJsonResourceAsync(dataPath, TimeSpan.FromHours(1));
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
            var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "xaml-patterns.json");
            if (!File.Exists(dataPath))
            {
                return Task.FromResult("Error: XAML patterns data not found");
            }

            var jsonContent = File.ReadAllText(dataPath);
            var patternsData = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            var patternInfo = FindXamlPattern(patternsData, patternName);
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
            var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "xaml-patterns.json");
            if (!File.Exists(dataPath))
            {
                return Task.FromResult("Error: XAML patterns data not found");
            }

            var jsonContent = File.ReadAllText(dataPath);
            var patternsData = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            var mvvmPatterns = ExtractMvvmPatterns(patternsData);
            return Task.FromResult(mvvmPatterns);
        }
        catch (Exception ex)
        {
            return Task.FromResult($"Error loading MVVM patterns: {ex.Message}");
        }
    }

    private static string FormatXamlPatterns(JsonElement patternsData)
    {
        var result = "# AvaloniaUI XAML Patterns\\n\\n";
        result += "This reference contains common XAML patterns and templates for AvaloniaUI development.\\n\\n";

        if (patternsData.TryGetProperty("avalonia_xaml_patterns", out var patterns))
        {
            foreach (var pattern in patterns.EnumerateObject())
            {
                result += FormatPattern(pattern.Value);
                result += "\\n---\\n\\n";
            }
        }

        return result;
    }

    private static string? FindXamlPattern(JsonElement patternsData, string patternName)
    {
        if (patternsData.TryGetProperty("avalonia_xaml_patterns", out var patterns))
        {
            foreach (var pattern in patterns.EnumerateObject())
            {
                if (string.Equals(pattern.Name, patternName, StringComparison.OrdinalIgnoreCase) ||
                    (pattern.Value.TryGetProperty("name", out var nameElement) &&
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
        var result = "# MVVM Patterns for AvaloniaUI\\n\\n";

        if (patternsData.TryGetProperty("avalonia_xaml_patterns", out var patterns))
        {
            var mvvmRelatedPatterns = new[] { "mvvm_window", "data_binding" };

            foreach (var patternName in mvvmRelatedPatterns)
            {
                if (patterns.TryGetProperty(patternName, out var pattern))
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
        var result = "";

        if (pattern.TryGetProperty("name", out var name))
        {
            result += $"## {name.GetString()}\\n\\n";
        }

        if (pattern.TryGetProperty("description", out var description))
        {
            result += $"**Description:** {description.GetString()}\\n\\n";
        }

        if (pattern.TryGetProperty("xaml", out var xaml))
        {
            result += "**XAML:**\\n```xml\\n";
            result += xaml.GetString()?.Replace("\\n", "\\n") ?? "";
            result += "\\n```\\n\\n";
        }

        if (pattern.TryGetProperty("key_points", out var keyPoints))
        {
            result += "**Key Points:**\\n";
            foreach (var point in keyPoints.EnumerateArray())
            {
                result += $"- {point.GetString()}\\n";
            }
            result += "\\n";
        }

        return result;
    }
}