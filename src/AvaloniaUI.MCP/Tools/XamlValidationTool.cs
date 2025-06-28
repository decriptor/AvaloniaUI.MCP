using System.ComponentModel;
using System.Xml;
using System.Xml.Linq;

using AvaloniaUI.MCP.Services;

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
        return ErrorHandlingService.SafeExecute("ValidateXaml", () =>
        {
            // Validate inputs
            ValidationResult validation = ErrorHandlingService.ValidateCommonParameters(
                xamlContent: xamlContent
            );

            if (!validation.IsValid)
            {
                return ErrorHandlingService.CreateValidationError("ValidateXaml", validation);
            }

            var validationResult = new List<string>();
            bool hasErrors = false;

            // 1. Check for basic XML validity
            try
            {
                var doc = XDocument.Parse(xamlContent);
                validationResult.Add("✓ XML syntax is valid");

                // 2. Check for required AvaloniaUI namespaces
                XElement? root = doc.Root;
                if (root != null)
                {
                    XNamespace avaloniaNamespace = root.GetDefaultNamespace();
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
                    XNamespace? xamlNamespace = root.GetNamespaceOfPrefix("x");
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

                    // 4. Check for new 11.3+ features
                    ValidateNewFeatures(doc, validationResult, validationLevel, ref hasErrors);

                    // 5. Check for common XAML patterns
                    ValidateCommonPatterns(doc, validationResult, validationLevel, ref hasErrors);

                    // 6. Check for performance optimizations
                    ValidatePerformanceOptimizations(doc, validationResult, validationLevel, ref hasErrors);
                }
            }
            catch (XmlException ex)
            {
                validationResult.Add($"✗ XML Syntax Error: {ex.Message}");
                hasErrors = true;
            }

            // 5. Check for file extension recommendations
            if (validationLevel is "warnings" or "strict")
            {
                validationResult.Add("💡 Tip: AvaloniaUI XAML files should use .axaml extension instead of .xaml");
            }

            string summary = hasErrors ? "❌ XAML Validation Failed" : "✅ XAML Validation Passed";
            return $"{summary}\\n\\n{string.Join("\\n", validationResult)}";
        });
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

            string convertedXaml = wpfXaml;
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

            foreach (KeyValuePair<string, string> replacement in replacements)
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
            string validationResult = ValidateXaml(convertedXaml, "normal");

            string result = "🔄 WPF to AvaloniaUI XAML Conversion Complete\\n\\n";
            result += "Conversion Notes:\\n" + string.Join("\\n", conversionNotes) + "\\n\\n";

            if (manualAttentionItems.Count != 0)
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
        string[] wpfOnlyElements = ["DockPanel", "UniformGrid", "Viewbox"];
        foreach (string? element in wpfOnlyElements)
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
        IEnumerable<XElement> elementsWithBinding = doc.Descendants().Where(e =>
            e.Attributes().Any(a => a.Value.Contains("{Binding")));

        foreach (XElement? element in elementsWithBinding)
        {
            IEnumerable<XAttribute> bindingAttrs = element.Attributes().Where(a => a.Value.Contains("{Binding"));
            foreach (XAttribute? attr in bindingAttrs)
            {
                if (attr.Value.Contains("RelativeSource={RelativeSource FindAncestor"))
                {
                    validationResult.Add("⚠ Warning: FindAncestor RelativeSource may not work as expected in AvaloniaUI");
                }
            }
        }

        // Check for x:Name usage (should be Name in AvaloniaUI)
        IEnumerable<XElement> xNameUsages = doc.Descendants().Where(e => e.Attribute("{http://schemas.microsoft.com/winfx/2006/xaml}Name") != null);
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
            XElement window = doc.Root;
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
        IEnumerable<XElement> dataContextUsages = doc.Descendants().Where(e =>
            e.Attribute("DataContext") != null ||
            e.Attribute("{http://schemas.microsoft.com/winfx/2006/xaml}DataType") != null);

        if (dataContextUsages.Any())
        {
            validationResult.Add($"✓ Found {dataContextUsages.Count()} DataContext/DataType usage(s)");
        }

        // Check for resource usage
        IEnumerable<XElement> resources = doc.Descendants().Where(e => e.Name.LocalName.EndsWith("Resources"));
        if (resources.Any())
        {
            validationResult.Add($"✓ Found {resources.Count()} resource section(s)");
        }
    }

    private static void ValidateNewFeatures(XDocument doc, List<string> validationResult, string validationLevel, ref bool hasErrors)
    {
        // Check for Container Queries (11.3+)
        IEnumerable<XElement> containerQueries = doc.Descendants("Style").Where(s =>
            s.Attribute("Selector")?.Value.Contains("@container") == true);

        if (containerQueries.Any())
        {
            validationResult.Add("✓ Using Container Queries (11.3+ feature) - excellent for responsive design!");

            foreach (XElement? query in containerQueries)
            {
                string selector = query.Attribute("Selector")?.Value ?? "";
                if (!selector.Contains("min-width") && !selector.Contains("max-width") &&
                    !selector.Contains("min-height") && !selector.Contains("max-height"))
                {
                    validationResult.Add("⚠ Container query without size constraints - consider adding min-width/max-width");
                }
            }
        }

        // Check for new Popup.ShouldUseOverlayLayer property (11.3+)
        IEnumerable<XElement> popupsWithOverlay = doc.Descendants("Popup").Where(p =>
            p.Attribute("ShouldUseOverlayLayer") != null);

        if (popupsWithOverlay.Any())
        {
            validationResult.Add("✓ Using ShouldUseOverlayLayer property (11.3+ feature) for better popup rendering");
        }

        // Check for enhanced spacing properties (11.3+)
        IEnumerable<XElement> elementsWithSpacing = doc.Descendants().Where(e =>
            e.Attributes().Any(a => a.Name.LocalName is "Spacing" or
                                   "RowSpacing" or
                                   "ColumnSpacing"));

        if (elementsWithSpacing.Any())
        {
            validationResult.Add("✓ Using enhanced spacing properties (11.3+ feature) - great for layout!");
        }

        // Check for ItemsControl.PreparingContainer usage
        IEnumerable<XElement> itemsControlsWithPreparing = doc.Descendants().Where(e =>
            e.Name.LocalName.Contains("ItemsControl") &&
            e.Attributes().Any(a => a.Name.LocalName == "PreparingContainer"));

        if (itemsControlsWithPreparing.Any())
        {
            validationResult.Add("✓ Using PreparingContainer event (11.3+ feature)");
        }

        // Check for TextBox.LineCount property
        IEnumerable<XElement> textBoxesWithLineCount = doc.Descendants("TextBox").Where(t =>
            t.Attribute("LineCount") != null);

        if (textBoxesWithLineCount.Any())
        {
            validationResult.Add("✓ Using TextBox.LineCount property (11.3+ feature)");
        }
    }
    private static readonly string[] sourceArray = ["ListBox", "ListView", "DataGrid", "TreeView"];
    private static readonly string[] sourceArray0 = new[] { "Background", "Foreground", "FontSize", "FontWeight", "Margin", "Padding" };

    private static void ValidatePerformanceOptimizations(XDocument doc, List<string> validationResult, string validationLevel, ref bool hasErrors)
    {
        // Check for compiled bindings (x:DataType)
        IEnumerable<XElement> elementsWithDataType = doc.Descendants().Where(e =>
            e.Attributes().Any(a => a.Name.LocalName == "DataType" && a.Name.NamespaceName.Contains("2006/xaml")));

        IEnumerable<XElement> elementsWithBindings = doc.Descendants().Where(e =>
            e.Attributes().Any(a => a.Value.Contains("{Binding")));

        if (elementsWithBindings.Any() && !elementsWithDataType.Any())
        {
            validationResult.Add("💡 Consider using x:DataType for compiled bindings (better performance)");
        }
        else if (elementsWithDataType.Any())
        {
            validationResult.Add("✓ Using compiled bindings with x:DataType - excellent performance!");
        }

        // Check for virtualization on list controls
        IEnumerable<XElement> listControls = doc.Descendants().Where(e => sourceArray.Contains(e.Name.LocalName));

        foreach (XElement? control in listControls)
        {
            bool hasVirtualization = control.Attributes().Any(a =>
                a.Value.Contains("VirtualizingStackPanel") || a.Name.LocalName.Contains("Virtualiz"));

            if (!hasVirtualization)
            {
                validationResult.Add($"💡 Consider enabling virtualization for {control.Name.LocalName} with large datasets");
            }
        }

        // Check for efficient styling patterns
        int inlineStyles = doc.Descendants().Count(e =>
            e.Attributes().Any(a => sourceArray0.Contains(a.Name.LocalName)));

        if (inlineStyles > 5)
        {
            validationResult.Add($"💡 {inlineStyles} inline style properties found - consider using styles for better maintainability");
        }

        // Check for proper resource usage
        int dynamicResources = doc.Descendants().Count(e =>
            e.Attributes().Any(a => a.Value.Contains("DynamicResource")));

        int staticResources = doc.Descendants().Count(e =>
            e.Attributes().Any(a => a.Value.Contains("StaticResource")));

        if (dynamicResources > staticResources && dynamicResources > 3)
        {
            validationResult.Add("💡 Consider using StaticResource instead of DynamicResource for better performance where possible");
        }

        // Check for modern selector usage
        IEnumerable<XElement> modernSelectors = doc.Descendants("Style").Where(s =>
            s.Attribute("Selector")?.Value.Contains(':') == true ||
            s.Attribute("Selector")?.Value.Contains('.') == true ||
            s.Attribute("Selector")?.Value.Contains('>') == true);

        if (modernSelectors.Any())
        {
            validationResult.Add("✓ Using modern CSS-like selectors - good styling practice!");
        }
    }
}
