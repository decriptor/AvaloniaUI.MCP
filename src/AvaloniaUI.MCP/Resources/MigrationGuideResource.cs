using System.ComponentModel;
using System.Text.Json;

using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Resources;

[McpServerResourceType]
public static class MigrationGuideResource
{
    [McpServerResource]
    [Description("Complete guide for migrating from WPF to AvaloniaUI")]
    public static Task<string> GetMigrationGuide()
    {
        try
        {
            string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "migration-guide.json");
            if (!File.Exists(dataPath))
            {
                return Task.FromResult("Error: Migration guide data not found");
            }

            string jsonContent = File.ReadAllText(dataPath);
            JsonElement migrationData = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            string formattedContent = FormatMigrationGuide(migrationData);
            return Task.FromResult(formattedContent);
        }
        catch (Exception ex)
        {
            return Task.FromResult($"Error loading migration guide: {ex.Message}");
        }
    }

    [McpServerResource]
    [Description("Get control mapping information from WPF to AvaloniaUI")]
    public static Task<string> GetControlMappings()
    {
        try
        {
            string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "migration-guide.json");
            if (!File.Exists(dataPath))
            {
                return Task.FromResult("Error: Migration guide data not found");
            }

            string jsonContent = File.ReadAllText(dataPath);
            JsonElement migrationData = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            string controlMappings = ExtractControlMappings(migrationData);
            return Task.FromResult(controlMappings);
        }
        catch (Exception ex)
        {
            return Task.FromResult($"Error loading control mappings: {ex.Message}");
        }
    }

    [McpServerResource]
    [Description("Get namespace and binding changes needed for WPF to AvaloniaUI migration")]
    public static Task<string> GetNamespaceAndBindingChanges()
    {
        try
        {
            string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "migration-guide.json");
            if (!File.Exists(dataPath))
            {
                return Task.FromResult("Error: Migration guide data not found");
            }

            string jsonContent = File.ReadAllText(dataPath);
            JsonElement migrationData = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            string changes = ExtractNamespaceAndBindingChanges(migrationData);
            return Task.FromResult(changes);
        }
        catch (Exception ex)
        {
            return Task.FromResult($"Error loading namespace and binding changes: {ex.Message}");
        }
    }

    [McpServerResource]
    [Description("Get step-by-step migration process from WPF to AvaloniaUI")]
    public static Task<string> GetMigrationSteps()
    {
        try
        {
            string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "migration-guide.json");
            if (!File.Exists(dataPath))
            {
                return Task.FromResult("Error: Migration guide data not found");
            }

            string jsonContent = File.ReadAllText(dataPath);
            JsonElement migrationData = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            string steps = ExtractMigrationSteps(migrationData);
            return Task.FromResult(steps);
        }
        catch (Exception ex)
        {
            return Task.FromResult($"Error loading migration steps: {ex.Message}");
        }
    }

    static string FormatMigrationGuide(JsonElement migrationData)
    {
        string result = "# WPF to AvaloniaUI Migration Guide\\n\\n";
        result += "This comprehensive guide will help you migrate your WPF applications to AvaloniaUI.\\n\\n";

        if (migrationData.TryGetProperty("wpf_to_avalonia_migration", out JsonElement migration))
        {
            // Namespace changes
            if (migration.TryGetProperty("namespace_changes", out JsonElement namespaceChanges))
            {
                result += "## Namespace Changes\\n\\n";
                result += FormatNamespaceChanges(namespaceChanges);
                result += "\\n";
            }

            // File extensions
            if (migration.TryGetProperty("file_extensions", out JsonElement fileExtensions))
            {
                result += "## File Extension Changes\\n\\n";
                result += FormatFileExtensions(fileExtensions);
                result += "\\n";
            }

            // Control mappings
            if (migration.TryGetProperty("control_mappings", out JsonElement controlMappings))
            {
                result += "## Control Compatibility\\n\\n";
                result += FormatControlMappings(controlMappings);
                result += "\\n";
            }

            // Binding changes
            if (migration.TryGetProperty("binding_changes", out JsonElement bindingChanges))
            {
                result += "## Data Binding Changes\\n\\n";
                result += FormatBindingChanges(bindingChanges);
                result += "\\n";
            }

            // Styling changes
            if (migration.TryGetProperty("styling_changes", out JsonElement stylingChanges))
            {
                result += "## Styling System Changes\\n\\n";
                result += FormatStylingChanges(stylingChanges);
                result += "\\n";
            }

            // Migration steps
            if (migration.TryGetProperty("common_migration_steps", out JsonElement migrationSteps))
            {
                result += "## Migration Process\\n\\n";
                result += FormatMigrationSteps(migrationSteps);
                result += "\\n";
            }
        }

        return result;
    }

    static string ExtractControlMappings(JsonElement migrationData)
    {
        string result = "# WPF to AvaloniaUI Control Mappings\\n\\n";

        if (migrationData.TryGetProperty("wpf_to_avalonia_migration", out JsonElement migration) &&
            migration.TryGetProperty("control_mappings", out JsonElement controlMappings))
        {
            result += FormatControlMappings(controlMappings);
        }

        return result;
    }

    static string ExtractNamespaceAndBindingChanges(JsonElement migrationData)
    {
        string result = "# Namespace and Binding Changes\\n\\n";

        if (migrationData.TryGetProperty("wpf_to_avalonia_migration", out JsonElement migration))
        {
            if (migration.TryGetProperty("namespace_changes", out JsonElement namespaceChanges))
            {
                result += "## Namespace Changes\\n\\n";
                result += FormatNamespaceChanges(namespaceChanges);
                result += "\\n";
            }

            if (migration.TryGetProperty("binding_changes", out JsonElement bindingChanges))
            {
                result += "## Binding Changes\\n\\n";
                result += FormatBindingChanges(bindingChanges);
            }
        }

        return result;
    }

    static string ExtractMigrationSteps(JsonElement migrationData)
    {
        string result = "# Migration Process Steps\\n\\n";

        if (migrationData.TryGetProperty("wpf_to_avalonia_migration", out JsonElement migration) &&
            migration.TryGetProperty("common_migration_steps", out JsonElement migrationSteps))
        {
            result += FormatMigrationSteps(migrationSteps);
        }

        return result;
    }

    static string FormatNamespaceChanges(JsonElement namespaceChanges)
    {
        string result = "";

        if (namespaceChanges.TryGetProperty("description", out JsonElement description))
        {
            result += $"{description.GetString()}\\n\\n";
        }

        if (namespaceChanges.TryGetProperty("changes", out JsonElement changes))
        {
            foreach (JsonElement change in changes.EnumerateArray())
            {
                if (change.TryGetProperty("from", out JsonElement from) &&
                    change.TryGetProperty("to", out JsonElement to) &&
                    change.TryGetProperty("description", out JsonElement desc))
                {
                    result += $"**{desc.GetString()}**\\n";
                    result += $"- From: `{from.GetString()}`\\n";
                    result += $"- To: `{to.GetString()}`\\n\\n";
                }
            }
        }

        return result;
    }

    static string FormatFileExtensions(JsonElement fileExtensions)
    {
        string result = "";

        if (fileExtensions.TryGetProperty("description", out JsonElement description))
        {
            result += $"{description.GetString()}\\n\\n";
        }

        if (fileExtensions.TryGetProperty("changes", out JsonElement changes))
        {
            foreach (JsonElement change in changes.EnumerateArray())
            {
                if (change.TryGetProperty("from", out JsonElement from) &&
                    change.TryGetProperty("to", out JsonElement to) &&
                    change.TryGetProperty("description", out JsonElement desc))
                {
                    result += $"**{desc.GetString()}**\\n";
                    result += $"- From: `{from.GetString()}`\\n";
                    result += $"- To: `{to.GetString()}`\\n";

                    if (change.TryGetProperty("note", out JsonElement note))
                    {
                        result += $"- Note: {note.GetString()}\\n";
                    }
                    result += "\\n";
                }
            }
        }

        return result;
    }

    static string FormatControlMappings(JsonElement controlMappings)
    {
        string result = "";

        if (controlMappings.TryGetProperty("available_controls", out JsonElement availableControls))
        {
            result += "### Available Controls\\n\\n";
            result += "| WPF Control | AvaloniaUI Control | Compatibility | Notes |\\n";
            result += "|-------------|-------------------|---------------|-------|\\n";

            foreach (JsonElement control in availableControls.EnumerateArray())
            {
                string? wpfControl = control.TryGetProperty("wpf_control", out JsonElement wpf) ? wpf.GetString() : "";
                string? avaloniaControl = control.TryGetProperty("avalonia_control", out JsonElement avalonia) ? avalonia.GetString() : "";
                string? compatibility = control.TryGetProperty("compatibility", out JsonElement compat) ? compat.GetString() : "";
                string? notes = control.TryGetProperty("notes", out JsonElement note) ? note.GetString() : "";

                result += $"| {wpfControl} | {avaloniaControl} | {compatibility} | {notes} |\\n";
            }
            result += "\\n";
        }

        if (controlMappings.TryGetProperty("unavailable_controls", out JsonElement unavailableControls))
        {
            result += "### Unavailable Controls\\n\\n";
            foreach (JsonElement control in unavailableControls.EnumerateArray())
            {
                string? wpfControl = control.TryGetProperty("wpf_control", out JsonElement wpf) ? wpf.GetString() : "";
                string? alternative = control.TryGetProperty("alternative", out JsonElement alt) ? alt.GetString() : "";
                string? notes = control.TryGetProperty("notes", out JsonElement note) ? note.GetString() : "";

                result += $"**{wpfControl}**\\n";
                result += $"- Alternative: {alternative}\\n";
                result += $"- Notes: {notes}\\n\\n";
            }
        }

        return result;
    }

    static string FormatBindingChanges(JsonElement bindingChanges)
    {
        string result = "";

        if (bindingChanges.TryGetProperty("compatible_bindings", out JsonElement compatibleBindings))
        {
            result += "### Compatible Bindings\\n\\n";
            result += "The following WPF binding syntaxes work in AvaloniaUI:\\n\\n";
            foreach (JsonElement binding in compatibleBindings.EnumerateArray())
            {
                result += $"- `{binding.GetString()}`\\n";
            }
            result += "\\n";
        }

        if (bindingChanges.TryGetProperty("unsupported_bindings", out JsonElement unsupportedBindings))
        {
            result += "### Changed Bindings\\n\\n";
            foreach (JsonElement binding in unsupportedBindings.EnumerateArray())
            {
                string? wpfSyntax = binding.TryGetProperty("wpf_syntax", out JsonElement wpf) ? wpf.GetString() : "";
                string? avaloniaAlternative = binding.TryGetProperty("avalonia_alternative", out JsonElement avalonia) ? avalonia.GetString() : "";
                string? notes = binding.TryGetProperty("notes", out JsonElement note) ? note.GetString() : "";

                result += $"**WPF:** `{wpfSyntax}`\\n";
                result += $"**AvaloniaUI:** `{avaloniaAlternative}`\\n";
                result += $"**Notes:** {notes}\\n\\n";
            }
        }

        return result;
    }

    static string FormatStylingChanges(JsonElement stylingChanges)
    {
        string result = "";

        if (stylingChanges.TryGetProperty("description", out JsonElement description))
        {
            result += $"{description.GetString()}\\n\\n";
        }

        if (stylingChanges.TryGetProperty("wpf_style", out JsonElement wpfStyle) &&
            stylingChanges.TryGetProperty("avalonia_style", out JsonElement avaloniaStyle))
        {
            result += $"**WPF:** `{wpfStyle.GetString()}`\\n";
            result += $"**AvaloniaUI:** `{avaloniaStyle.GetString()}`\\n\\n";
        }

        if (stylingChanges.TryGetProperty("selector_examples", out JsonElement selectorExamples))
        {
            result += "**Selector Examples:**\\n";
            foreach (JsonElement example in selectorExamples.EnumerateArray())
            {
                result += $"- `{example.GetString()}`\\n";
            }
            result += "\\n";
        }

        return result;
    }

    static string FormatMigrationSteps(JsonElement migrationSteps)
    {
        string result = "";

        foreach (JsonElement step in migrationSteps.EnumerateArray())
        {
            int stepNumber = step.TryGetProperty("step", out JsonElement num) ? num.GetInt32() : 0;
            string? action = step.TryGetProperty("action", out JsonElement act) ? act.GetString() : "";
            string? description = step.TryGetProperty("description", out JsonElement desc) ? desc.GetString() : "";

            result += $"{stepNumber}. **{action}**\\n";
            result += $"   {description}\\n\\n";
        }

        return result;
    }
}