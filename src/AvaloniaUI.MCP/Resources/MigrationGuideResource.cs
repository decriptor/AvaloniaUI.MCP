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
            var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "migration-guide.json");
            if (!File.Exists(dataPath))
            {
                return Task.FromResult("Error: Migration guide data not found");
            }

            var jsonContent = File.ReadAllText(dataPath);
            var migrationData = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            var formattedContent = FormatMigrationGuide(migrationData);
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
            var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "migration-guide.json");
            if (!File.Exists(dataPath))
            {
                return Task.FromResult("Error: Migration guide data not found");
            }

            var jsonContent = File.ReadAllText(dataPath);
            var migrationData = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            var controlMappings = ExtractControlMappings(migrationData);
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
            var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "migration-guide.json");
            if (!File.Exists(dataPath))
            {
                return Task.FromResult("Error: Migration guide data not found");
            }

            var jsonContent = File.ReadAllText(dataPath);
            var migrationData = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            var changes = ExtractNamespaceAndBindingChanges(migrationData);
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
            var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "migration-guide.json");
            if (!File.Exists(dataPath))
            {
                return Task.FromResult("Error: Migration guide data not found");
            }

            var jsonContent = File.ReadAllText(dataPath);
            var migrationData = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            var steps = ExtractMigrationSteps(migrationData);
            return Task.FromResult(steps);
        }
        catch (Exception ex)
        {
            return Task.FromResult($"Error loading migration steps: {ex.Message}");
        }
    }

    private static string FormatMigrationGuide(JsonElement migrationData)
    {
        var result = "# WPF to AvaloniaUI Migration Guide\\n\\n";
        result += "This comprehensive guide will help you migrate your WPF applications to AvaloniaUI.\\n\\n";

        if (migrationData.TryGetProperty("wpf_to_avalonia_migration", out var migration))
        {
            // Namespace changes
            if (migration.TryGetProperty("namespace_changes", out var namespaceChanges))
            {
                result += "## Namespace Changes\\n\\n";
                result += FormatNamespaceChanges(namespaceChanges);
                result += "\\n";
            }

            // File extensions
            if (migration.TryGetProperty("file_extensions", out var fileExtensions))
            {
                result += "## File Extension Changes\\n\\n";
                result += FormatFileExtensions(fileExtensions);
                result += "\\n";
            }

            // Control mappings
            if (migration.TryGetProperty("control_mappings", out var controlMappings))
            {
                result += "## Control Compatibility\\n\\n";
                result += FormatControlMappings(controlMappings);
                result += "\\n";
            }

            // Binding changes
            if (migration.TryGetProperty("binding_changes", out var bindingChanges))
            {
                result += "## Data Binding Changes\\n\\n";
                result += FormatBindingChanges(bindingChanges);
                result += "\\n";
            }

            // Styling changes
            if (migration.TryGetProperty("styling_changes", out var stylingChanges))
            {
                result += "## Styling System Changes\\n\\n";
                result += FormatStylingChanges(stylingChanges);
                result += "\\n";
            }

            // Migration steps
            if (migration.TryGetProperty("common_migration_steps", out var migrationSteps))
            {
                result += "## Migration Process\\n\\n";
                result += FormatMigrationSteps(migrationSteps);
                result += "\\n";
            }
        }

        return result;
    }

    private static string ExtractControlMappings(JsonElement migrationData)
    {
        var result = "# WPF to AvaloniaUI Control Mappings\\n\\n";

        if (migrationData.TryGetProperty("wpf_to_avalonia_migration", out var migration) &&
            migration.TryGetProperty("control_mappings", out var controlMappings))
        {
            result += FormatControlMappings(controlMappings);
        }

        return result;
    }

    private static string ExtractNamespaceAndBindingChanges(JsonElement migrationData)
    {
        var result = "# Namespace and Binding Changes\\n\\n";

        if (migrationData.TryGetProperty("wpf_to_avalonia_migration", out var migration))
        {
            if (migration.TryGetProperty("namespace_changes", out var namespaceChanges))
            {
                result += "## Namespace Changes\\n\\n";
                result += FormatNamespaceChanges(namespaceChanges);
                result += "\\n";
            }

            if (migration.TryGetProperty("binding_changes", out var bindingChanges))
            {
                result += "## Binding Changes\\n\\n";
                result += FormatBindingChanges(bindingChanges);
            }
        }

        return result;
    }

    private static string ExtractMigrationSteps(JsonElement migrationData)
    {
        var result = "# Migration Process Steps\\n\\n";

        if (migrationData.TryGetProperty("wpf_to_avalonia_migration", out var migration) &&
            migration.TryGetProperty("common_migration_steps", out var migrationSteps))
        {
            result += FormatMigrationSteps(migrationSteps);
        }

        return result;
    }

    private static string FormatNamespaceChanges(JsonElement namespaceChanges)
    {
        var result = "";

        if (namespaceChanges.TryGetProperty("description", out var description))
        {
            result += $"{description.GetString()}\\n\\n";
        }

        if (namespaceChanges.TryGetProperty("changes", out var changes))
        {
            foreach (var change in changes.EnumerateArray())
            {
                if (change.TryGetProperty("from", out var from) &&
                    change.TryGetProperty("to", out var to) &&
                    change.TryGetProperty("description", out var desc))
                {
                    result += $"**{desc.GetString()}**\\n";
                    result += $"- From: `{from.GetString()}`\\n";
                    result += $"- To: `{to.GetString()}`\\n\\n";
                }
            }
        }

        return result;
    }

    private static string FormatFileExtensions(JsonElement fileExtensions)
    {
        var result = "";

        if (fileExtensions.TryGetProperty("description", out var description))
        {
            result += $"{description.GetString()}\\n\\n";
        }

        if (fileExtensions.TryGetProperty("changes", out var changes))
        {
            foreach (var change in changes.EnumerateArray())
            {
                if (change.TryGetProperty("from", out var from) &&
                    change.TryGetProperty("to", out var to) &&
                    change.TryGetProperty("description", out var desc))
                {
                    result += $"**{desc.GetString()}**\\n";
                    result += $"- From: `{from.GetString()}`\\n";
                    result += $"- To: `{to.GetString()}`\\n";

                    if (change.TryGetProperty("note", out var note))
                    {
                        result += $"- Note: {note.GetString()}\\n";
                    }
                    result += "\\n";
                }
            }
        }

        return result;
    }

    private static string FormatControlMappings(JsonElement controlMappings)
    {
        var result = "";

        if (controlMappings.TryGetProperty("available_controls", out var availableControls))
        {
            result += "### Available Controls\\n\\n";
            result += "| WPF Control | AvaloniaUI Control | Compatibility | Notes |\\n";
            result += "|-------------|-------------------|---------------|-------|\\n";

            foreach (var control in availableControls.EnumerateArray())
            {
                var wpfControl = control.TryGetProperty("wpf_control", out var wpf) ? wpf.GetString() : "";
                var avaloniaControl = control.TryGetProperty("avalonia_control", out var avalonia) ? avalonia.GetString() : "";
                var compatibility = control.TryGetProperty("compatibility", out var compat) ? compat.GetString() : "";
                var notes = control.TryGetProperty("notes", out var note) ? note.GetString() : "";

                result += $"| {wpfControl} | {avaloniaControl} | {compatibility} | {notes} |\\n";
            }
            result += "\\n";
        }

        if (controlMappings.TryGetProperty("unavailable_controls", out var unavailableControls))
        {
            result += "### Unavailable Controls\\n\\n";
            foreach (var control in unavailableControls.EnumerateArray())
            {
                var wpfControl = control.TryGetProperty("wpf_control", out var wpf) ? wpf.GetString() : "";
                var alternative = control.TryGetProperty("alternative", out var alt) ? alt.GetString() : "";
                var notes = control.TryGetProperty("notes", out var note) ? note.GetString() : "";

                result += $"**{wpfControl}**\\n";
                result += $"- Alternative: {alternative}\\n";
                result += $"- Notes: {notes}\\n\\n";
            }
        }

        return result;
    }

    private static string FormatBindingChanges(JsonElement bindingChanges)
    {
        var result = "";

        if (bindingChanges.TryGetProperty("compatible_bindings", out var compatibleBindings))
        {
            result += "### Compatible Bindings\\n\\n";
            result += "The following WPF binding syntaxes work in AvaloniaUI:\\n\\n";
            foreach (var binding in compatibleBindings.EnumerateArray())
            {
                result += $"- `{binding.GetString()}`\\n";
            }
            result += "\\n";
        }

        if (bindingChanges.TryGetProperty("unsupported_bindings", out var unsupportedBindings))
        {
            result += "### Changed Bindings\\n\\n";
            foreach (var binding in unsupportedBindings.EnumerateArray())
            {
                var wpfSyntax = binding.TryGetProperty("wpf_syntax", out var wpf) ? wpf.GetString() : "";
                var avaloniaAlternative = binding.TryGetProperty("avalonia_alternative", out var avalonia) ? avalonia.GetString() : "";
                var notes = binding.TryGetProperty("notes", out var note) ? note.GetString() : "";

                result += $"**WPF:** `{wpfSyntax}`\\n";
                result += $"**AvaloniaUI:** `{avaloniaAlternative}`\\n";
                result += $"**Notes:** {notes}\\n\\n";
            }
        }

        return result;
    }

    private static string FormatStylingChanges(JsonElement stylingChanges)
    {
        var result = "";

        if (stylingChanges.TryGetProperty("description", out var description))
        {
            result += $"{description.GetString()}\\n\\n";
        }

        if (stylingChanges.TryGetProperty("wpf_style", out var wpfStyle) &&
            stylingChanges.TryGetProperty("avalonia_style", out var avaloniaStyle))
        {
            result += $"**WPF:** `{wpfStyle.GetString()}`\\n";
            result += $"**AvaloniaUI:** `{avaloniaStyle.GetString()}`\\n\\n";
        }

        if (stylingChanges.TryGetProperty("selector_examples", out var selectorExamples))
        {
            result += "**Selector Examples:**\\n";
            foreach (var example in selectorExamples.EnumerateArray())
            {
                result += $"- `{example.GetString()}`\\n";
            }
            result += "\\n";
        }

        return result;
    }

    private static string FormatMigrationSteps(JsonElement migrationSteps)
    {
        var result = "";

        foreach (var step in migrationSteps.EnumerateArray())
        {
            var stepNumber = step.TryGetProperty("step", out var num) ? num.GetInt32() : 0;
            var action = step.TryGetProperty("action", out var act) ? act.GetString() : "";
            var description = step.TryGetProperty("description", out var desc) ? desc.GetString() : "";

            result += $"{stepNumber}. **{action}**\\n";
            result += $"   {description}\\n\\n";
        }

        return result;
    }
}