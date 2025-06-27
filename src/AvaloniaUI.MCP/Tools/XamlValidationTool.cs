using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Tools;

[McpServerToolType]
public static class XamlValidationTool
{
    [McpServerTool, Description("Validates AvaloniaUI XAML content for syntax errors and common issues")]
    public static string ValidateXaml(
        [Description("The XAML content to validate")] string xamlContent,
        [Description("Additional validation options: 'strict' for strict validation, 'warnings' to include warnings")] string validationLevel = "normal")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(xamlContent))
            {
                return "Error: XAML content cannot be empty";
            }

            var validationResult = new List<string>();
            var hasErrors = false;

            // 1. Check for basic XML validity
            try
            {
                var doc = XDocument.Parse(xamlContent);
                validationResult.Add("✓ XML syntax is valid");

                // 2. Check for required AvaloniaUI namespaces
                var root = doc.Root;
                if (root != null)
                {
                    var avaloniaNamespace = root.GetDefaultNamespace();
                    if (avaloniaNamespace?.NamespaceName != "https://github.com/avaloniaui")
                    {
                        validationResult.Add("⚠ Warning: Missing or incorrect default AvaloniaUI namespace 'https://github.com/avaloniaui'");
                        if (validationLevel == "strict")
                        {
                            hasErrors = true;
                        }
                    }
                    else
                    {
                        validationResult.Add("✓ AvaloniaUI namespace is correctly declared");
                    }

                    // Check for XAML namespace
                    var xamlNamespace = root.GetNamespaceOfPrefix("x");
                    if (xamlNamespace?.NamespaceName != "http://schemas.microsoft.com/winfx/2006/xaml")
                    {
                        validationResult.Add("⚠ Warning: Missing or incorrect XAML namespace 'http://schemas.microsoft.com/winfx/2006/xaml'");
                        if (validationLevel == "strict")
                        {
                            hasErrors = true;
                        }
                    }
                    else
                    {
                        validationResult.Add("✓ XAML namespace is correctly declared");
                    }

                    // 3. Check for common AvaloniaUI-specific issues
                    ValidateAvaloniaSpecificIssues(doc, validationResult, validationLevel, ref hasErrors);

                    // 4. Check for common XAML patterns
                    ValidateCommonPatterns(doc, validationResult, validationLevel, ref hasErrors);
                }
            }
            catch (XmlException ex)
            {
                validationResult.Add($"✗ XML Syntax Error: {ex.Message}");
                hasErrors = true;
            }

            // 5. Check for file extension recommendations
            if (validationLevel == "warnings" || validationLevel == "strict")
            {
                validationResult.Add("💡 Tip: AvaloniaUI XAML files should use .axaml extension instead of .xaml");
            }

            var summary = hasErrors ? "❌ XAML Validation Failed" : "✅ XAML Validation Passed";
            return $"{summary}\\n\\n{string.Join("\\n", validationResult)}";
        }
        catch (Exception ex)
        {
            return $"Error during validation: {ex.Message}";
        }
    }

    [McpServerTool, Description("Converts WPF XAML to AvaloniaUI XAML by updating namespaces and incompatible elements")]
    public static string ConvertWpfXamlToAvalonia([Description("WPF XAML content to convert")] string wpfXaml)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(wpfXaml))
            {
                return "Error: WPF XAML content cannot be empty";
            }

            var convertedXaml = wpfXaml;
            var conversionNotes = new List<string>();

            // 1. Replace WPF namespaces with AvaloniaUI namespaces
            convertedXaml = convertedXaml.Replace(
                "http://schemas.microsoft.com/winfx/2006/xaml/presentation",
                "https://github.com/avaloniaui");
            conversionNotes.Add("✓ Replaced WPF presentation namespace with AvaloniaUI namespace");

            // 2. Replace common WPF-specific elements with AvaloniaUI equivalents
            var replacements = new Dictionary<string, string>
            {
                // Window properties
                { "WindowState=\"Maximized\"", "WindowState=\"Maximized\"" }, // Same in Avalonia
                { "WindowStartupLocation=\"CenterScreen\"", "WindowStartupLocation=\"CenterScreen\"" }, // Same in Avalonia
                
                // Replace WPF-specific controls
                { "<DataGrid", "<DataGrid" }, // Available in Avalonia
                { "<TabControl", "<TabControl" }, // Available in Avalonia
                { "<TreeView", "<TreeView" }, // Available in Avalonia
                
                // Replace bindings (mostly compatible)
                { "RelativeSource={RelativeSource Self}", "RelativeSource={RelativeSource Self}" },
            };

            foreach (var replacement in replacements)
            {
                if (convertedXaml.Contains(replacement.Key))
                {
                    convertedXaml = convertedXaml.Replace(replacement.Key, replacement.Value);
                }
            }

            // 3. Check for potential issues that need manual attention
            var manualAttentionItems = new List<string>();
            
            if (convertedXaml.Contains("DependencyProperty"))
            {
                manualAttentionItems.Add("⚠ DependencyProperty usage detected - may need conversion to AvaloniaProperty");
            }
            
            if (convertedXaml.Contains("RoutedCommand"))
            {
                manualAttentionItems.Add("⚠ RoutedCommand usage detected - consider using ReactiveCommand instead");
            }
            
            if (convertedXaml.Contains("Trigger"))
            {
                manualAttentionItems.Add("⚠ Trigger usage detected - AvaloniaUI uses different styling approach");
            }
            
            if (convertedXaml.Contains("ControlTemplate"))
            {
                manualAttentionItems.Add("⚠ ControlTemplate detected - verify compatibility with AvaloniaUI templating");
            }

            // 4. Validate the converted XAML
            var validationResult = ValidateXaml(convertedXaml, "normal");

            var result = "🔄 WPF to AvaloniaUI XAML Conversion Complete\\n\\n";
            result += "Conversion Notes:\\n" + string.Join("\\n", conversionNotes) + "\\n\\n";
            
            if (manualAttentionItems.Any())
            {
                result += "Items requiring manual attention:\\n" + string.Join("\\n", manualAttentionItems) + "\\n\\n";
            }
            
            result += "Converted XAML:\\n" + convertedXaml + "\\n\\n";
            result += "Validation Result:\\n" + validationResult;

            return result;
        }
        catch (Exception ex)
        {
            return $"Error during conversion: {ex.Message}";
        }
    }

    private static void ValidateAvaloniaSpecificIssues(XDocument doc, List<string> validationResult, string validationLevel, ref bool hasErrors)
    {
        // Check for unsupported WPF elements
        var wpfOnlyElements = new[] { "DockPanel", "UniformGrid", "Viewbox" };
        foreach (var element in wpfOnlyElements)
        {
            if (doc.Descendants().Any(e => e.Name.LocalName == element))
            {
                validationResult.Add($"⚠ Warning: {element} is not available in AvaloniaUI - consider alternatives");
                if (validationLevel == "strict")
                {
                    hasErrors = true;
                }
            }
        }

        // Check for proper data binding syntax
        var elementsWithBinding = doc.Descendants().Where(e => 
            e.Attributes().Any(a => a.Value.Contains("{Binding")));
        
        foreach (var element in elementsWithBinding)
        {
            var bindingAttrs = element.Attributes().Where(a => a.Value.Contains("{Binding"));
            foreach (var attr in bindingAttrs)
            {
                if (attr.Value.Contains("RelativeSource={RelativeSource FindAncestor"))
                {
                    validationResult.Add("⚠ Warning: FindAncestor RelativeSource may not work as expected in AvaloniaUI");
                }
            }
        }

        // Check for x:Name usage (should be Name in AvaloniaUI)
        var xNameUsages = doc.Descendants().Where(e => e.Attribute("{http://schemas.microsoft.com/winfx/2006/xaml}Name") != null);
        if (xNameUsages.Any())
        {
            validationResult.Add($"✓ Found {xNameUsages.Count()} x:Name attributes (compatible with AvaloniaUI)");
        }
    }

    private static void ValidateCommonPatterns(XDocument doc, List<string> validationResult, string validationLevel, ref bool hasErrors)
    {
        // Check for proper Window structure
        if (doc.Root?.Name.LocalName == "Window")
        {
            validationResult.Add("✓ Root element is Window");
            
            // Check for recommended Window properties
            var window = doc.Root;
            if (window.Attribute("Title") != null)
            {
                validationResult.Add("✓ Window has Title property");
            }
            
            if (window.Attribute("Width") != null && window.Attribute("Height") != null)
            {
                validationResult.Add("✓ Window has Width and Height properties");
            }
        }

        // Check for proper UserControl structure
        if (doc.Root?.Name.LocalName == "UserControl")
        {
            validationResult.Add("✓ Root element is UserControl");
        }

        // Check for proper data context usage
        var dataContextUsages = doc.Descendants().Where(e => 
            e.Attribute("DataContext") != null || 
            e.Attribute("{http://schemas.microsoft.com/winfx/2006/xaml}DataType") != null);
        
        if (dataContextUsages.Any())
        {
            validationResult.Add($"✓ Found {dataContextUsages.Count()} DataContext/DataType usage(s)");
        }

        // Check for resource usage
        var resources = doc.Descendants().Where(e => e.Name.LocalName.EndsWith("Resources"));
        if (resources.Any())
        {
            validationResult.Add($"✓ Found {resources.Count()} resource section(s)");
        }
    }
}