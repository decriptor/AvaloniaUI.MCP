using System.Text.RegularExpressions;

namespace AvaloniaUI.MCP.Services;

/// <summary>
/// Provides comprehensive input validation for all MCP tools
/// </summary>
public static partial class InputValidationService
{
    private static readonly Regex ValidProjectNameRegex = MyRegex();
    private static readonly Regex ValidColorHexRegex = MyRegex1();
    private static readonly Regex ValidIdentifierRegex = new(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);
    private static readonly Regex ValidCssClassRegex = new(@"^[a-zA-Z_-][a-zA-Z0-9_-]*$", RegexOptions.Compiled);

    private static readonly HashSet<string> ValidTemplateTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "mvvm", "basic", "crossplatform", "cross-platform"
    };

    private static readonly HashSet<string> ValidThemeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "light", "dark", "auto"
    };

    private static readonly HashSet<string> ValidControlTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "templated", "usercontrol", "panel", "attached-property"
    };

    private static readonly HashSet<string> ValidComponentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "form", "navigation", "data-table", "modal", "notification"
    };

    public static ValidationResult ValidateProjectName(string projectName)
    {
        if (string.IsNullOrWhiteSpace(projectName))
        {
            return ValidationResult.Failure("Project name cannot be empty or whitespace");
        }

        if (projectName.Length > 128)
        {
            return ValidationResult.Failure("Project name cannot exceed 128 characters");
        }

        if (!ValidProjectNameRegex.IsMatch(projectName))
        {
            return ValidationResult.Failure("Project name must start with a letter and contain only letters, numbers, underscores, dots, and hyphens");
        }

        // Check for reserved names
        string[] reservedNames = ["CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"];
        return reservedNames.Contains(projectName.ToUpperInvariant())
            ? ValidationResult.Failure($"'{projectName}' is a reserved name and cannot be used")
            : ValidationResult.Success();
    }

    public static ValidationResult ValidateTemplateType(string template)
    {
        return string.IsNullOrWhiteSpace(template)
            ? ValidationResult.Failure("Template type cannot be empty")
            : !ValidTemplateTypes.Contains(template)
            ? ValidationResult.Failure($"Invalid template type '{template}'. Valid types are: {string.Join(", ", ValidTemplateTypes)}")
            : ValidationResult.Success();
    }

    public static ValidationResult ValidateColorHex(string color)
    {
        return string.IsNullOrWhiteSpace(color)
            ? ValidationResult.Failure("Color cannot be empty")
            : !ValidColorHexRegex.IsMatch(color)
            ? ValidationResult.Failure("Color must be a valid hex color (e.g., #FF0000, #F00, or #FFAA0080)")
            : ValidationResult.Success();
    }

    public static ValidationResult ValidateIdentifier(string identifier, string parameterName = "identifier")
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return ValidationResult.Failure($"{parameterName} cannot be empty");
        }

        if (identifier.Length > 200)
        {
            return ValidationResult.Failure($"{parameterName} cannot exceed 200 characters");
        }

        if (!ValidIdentifierRegex.IsMatch(identifier))
        {
            return ValidationResult.Failure($"{parameterName} must be a valid identifier (start with letter or underscore, contain only letters, numbers, and underscores)");
        }

        // Check for C# keywords
        string[] keywords =
        [
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is",
            "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override",
            "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte",
            "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch",
            "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
            "ushort", "using", "virtual", "void", "volatile", "while"
        ];

        return keywords.Contains(identifier.ToLowerInvariant())
            ? ValidationResult.Failure($"{parameterName} '{identifier}' is a reserved C# keyword")
            : ValidationResult.Success();
    }

    public static ValidationResult ValidateControlType(string controlType)
    {
        return string.IsNullOrWhiteSpace(controlType)
            ? ValidationResult.Failure("Control type cannot be empty")
            : !ValidControlTypes.Contains(controlType)
            ? ValidationResult.Failure($"Invalid control type '{controlType}'. Valid types are: {string.Join(", ", ValidControlTypes)}")
            : ValidationResult.Success();
    }

    public static ValidationResult ValidateComponentType(string componentType)
    {
        return string.IsNullOrWhiteSpace(componentType)
            ? ValidationResult.Failure("Component type cannot be empty")
            : !ValidComponentTypes.Contains(componentType)
            ? ValidationResult.Failure($"Invalid component type '{componentType}'. Valid types are: {string.Join(", ", ValidComponentTypes)}")
            : ValidationResult.Success();
    }

    public static ValidationResult ValidateThemeType(string themeType)
    {
        return string.IsNullOrWhiteSpace(themeType)
            ? ValidationResult.Failure("Theme type cannot be empty")
            : !ValidThemeTypes.Contains(themeType)
            ? ValidationResult.Failure($"Invalid theme type '{themeType}'. Valid types are: {string.Join(", ", ValidThemeTypes)}")
            : ValidationResult.Success();
    }

    public static ValidationResult ValidateCssClasses(string cssClasses)
    {
        if (string.IsNullOrWhiteSpace(cssClasses))
        {
            return ValidationResult.Success(); // Empty is valid
        }

        string[] classes = cssClasses.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (string cssClass in classes)
        {
            string trimmedClass = cssClass.Trim();
            if (!ValidCssClassRegex.IsMatch(trimmedClass))
            {
                return ValidationResult.Failure($"Invalid CSS class '{trimmedClass}'. CSS classes must start with a letter, underscore, or hyphen and contain only letters, numbers, underscores, and hyphens");
            }
        }

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateXamlContent(string xamlContent)
    {
        if (string.IsNullOrWhiteSpace(xamlContent))
        {
            return ValidationResult.Failure("XAML content cannot be empty");
        }

        if (xamlContent.Length > 1_000_000) // 1MB limit
        {
            return ValidationResult.Failure("XAML content cannot exceed 1MB");
        }

        // Basic XML structure validation
        if (!xamlContent.TrimStart().StartsWith('<'))
        {
            return ValidationResult.Failure("XAML content must start with an XML element");
        }

        // Check for potentially dangerous content
        string[] dangerousPatterns =
        [
            "<script", "javascript:", "vbscript:", "data:", "file:", "eval(", "expression("
        ];

        foreach (string? pattern in dangerousPatterns)
        {
            if (xamlContent.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            {
                return ValidationResult.Failure($"XAML content contains potentially dangerous pattern: {pattern}");
            }
        }

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateBooleanString(string value, string parameterName = "parameter")
    {
        return string.IsNullOrWhiteSpace(value)
            ? ValidationResult.Failure($"{parameterName} cannot be empty")
            : !bool.TryParse(value, out _)
            ? ValidationResult.Failure($"{parameterName} must be 'true' or 'false'")
            : ValidationResult.Success();
    }

    public static ValidationResult ValidateIntegerString(string value, int min = int.MinValue, int max = int.MaxValue, string parameterName = "parameter")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationResult.Failure($"{parameterName} cannot be empty");
        }

        return !int.TryParse(value, out int intValue)
            ? ValidationResult.Failure($"{parameterName} must be a valid integer")
            : intValue < min || intValue > max
            ? ValidationResult.Failure($"{parameterName} must be between {min} and {max}")
            : ValidationResult.Success();
    }

    public static ValidationResult ValidateEnumString<T>(string value, string parameterName = "parameter") where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationResult.Failure($"{parameterName} cannot be empty");
        }

        if (!Enum.TryParse<T>(value, true, out _))
        {
            string[] validValues = Enum.GetNames<T>();
            return ValidationResult.Failure($"{parameterName} must be one of: {string.Join(", ", validValues)}");
        }

        return ValidationResult.Success();
    }

    public static ValidationResult ValidatePropertyList(string properties)
    {
        if (string.IsNullOrWhiteSpace(properties))
        {
            return ValidationResult.Success(); // Empty is valid
        }

        string[] propertyNames = properties.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var seenProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (string property in propertyNames)
        {
            string trimmedProperty = property.Trim();

            ValidationResult identifierValidation = ValidateIdentifier(trimmedProperty, "property name");
            if (!identifierValidation.IsValid)
            {
                return identifierValidation;
            }

            if (!seenProperties.Add(trimmedProperty))
            {
                return ValidationResult.Failure($"Duplicate property name: {trimmedProperty}");
            }
        }

        return seenProperties.Count > 50
            ? ValidationResult.Failure("Too many properties specified (maximum 50)")
            : ValidationResult.Success();
    }

    public static ValidationResult ValidateFilePath(string filePath, bool mustExist = false)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return ValidationResult.Failure("File path cannot be empty");
        }

        try
        {
            // Check for invalid path characters
            char[] invalidChars = Path.GetInvalidPathChars();
            if (filePath.Any(c => invalidChars.Contains(c)))
            {
                return ValidationResult.Failure("File path contains invalid characters");
            }

            // Check for dangerous patterns
            return filePath.Contains("..") || filePath.Contains('~')
                ? ValidationResult.Failure("File path cannot contain relative navigation patterns")
                : mustExist && !File.Exists(filePath)
                ? ValidationResult.Failure($"File does not exist: {filePath}")
                : ValidationResult.Success();
        }
        catch (Exception ex)
        {
            return ValidationResult.Failure($"Invalid file path: {ex.Message}");
        }
    }

    public static ValidationResult ValidateUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return ValidationResult.Failure("URL cannot be empty");
        }

        return !Uri.TryCreate(url, UriKind.Absolute, out Uri? uri)
            ? ValidationResult.Failure("URL is not a valid absolute URI")
            : uri.Scheme is not "http" and not "https"
            ? ValidationResult.Failure("URL must use HTTP or HTTPS protocol")
            : ValidationResult.Success();
    }

    /// <summary>
    /// Sanitizes input by removing or encoding potentially dangerous characters
    /// </summary>
    public static string SanitizeInput(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        // Remove null characters
        input = input.Replace("\0", string.Empty);

        // Remove control characters except tab, newline, and carriage return
        input = new string([.. input.Where(c =>
            !char.IsControl(c) || c == '\t' || c == '\n' || c == '\r')]);

        // Trim excessive whitespace
        input = input.Trim();

        // Limit length to prevent DoS
        if (input.Length > 10000)
        {
            input = input[..10000];
        }

        return input;
    }

    /// <summary>
    /// Validates multiple parameters and returns the first validation failure
    /// </summary>
    public static ValidationResult ValidateAll(params ValidationResult[] validations)
    {
        foreach (ValidationResult validation in validations)
        {
            if (!validation.IsValid)
            {
                return validation;
            }
        }
        return ValidationResult.Success();
    }

    [GeneratedRegex(@"^[a-zA-Z][a-zA-Z0-9_.-]{0,127}$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
    [GeneratedRegex(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{8}|[A-Fa-f0-9]{3})$", RegexOptions.Compiled)]
    private static partial Regex MyRegex1();
}

/// <summary>
/// Represents the result of a validation operation
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; }
    public string ErrorMessage { get; }

    private ValidationResult(bool isValid, string errorMessage = "")
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success()
    {
        return new(true);
    }

    public static ValidationResult Failure(string errorMessage)
    {
        return new(false, errorMessage);
    }

    public override string ToString()
    {
        return IsValid ? "Valid" : $"Invalid: {ErrorMessage}";
    }
}
