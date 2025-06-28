using System.Text.RegularExpressions;

namespace AvaloniaUI.MCP.Services;

/// <summary>
/// Provides comprehensive input validation for all MCP tools
/// </summary>
public static class InputValidationService
{
    private static readonly Regex ValidProjectNameRegex = new(@"^[a-zA-Z][a-zA-Z0-9_.-]{0,127}$", RegexOptions.Compiled);
    private static readonly Regex ValidColorHexRegex = new(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{8}|[A-Fa-f0-9]{3})$", RegexOptions.Compiled);
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
            return ValidationResult.Failure("Project name cannot be empty or whitespace");
            
        if (projectName.Length > 128)
            return ValidationResult.Failure("Project name cannot exceed 128 characters");
            
        if (!ValidProjectNameRegex.IsMatch(projectName))
            return ValidationResult.Failure("Project name must start with a letter and contain only letters, numbers, underscores, dots, and hyphens");
            
        // Check for reserved names
        var reservedNames = new[] { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };
        if (reservedNames.Contains(projectName.ToUpperInvariant()))
            return ValidationResult.Failure($"'{projectName}' is a reserved name and cannot be used");
            
        return ValidationResult.Success();
    }
    
    public static ValidationResult ValidateTemplateType(string template)
    {
        if (string.IsNullOrWhiteSpace(template))
            return ValidationResult.Failure("Template type cannot be empty");
            
        if (!ValidTemplateTypes.Contains(template))
            return ValidationResult.Failure($"Invalid template type '{template}'. Valid types are: {string.Join(", ", ValidTemplateTypes)}");
            
        return ValidationResult.Success();
    }
    
    public static ValidationResult ValidateColorHex(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            return ValidationResult.Failure("Color cannot be empty");
            
        if (!ValidColorHexRegex.IsMatch(color))
            return ValidationResult.Failure("Color must be a valid hex color (e.g., #FF0000, #F00, or #FFAA0080)");
            
        return ValidationResult.Success();
    }
    
    public static ValidationResult ValidateIdentifier(string identifier, string parameterName = "identifier")
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return ValidationResult.Failure($"{parameterName} cannot be empty");
            
        if (identifier.Length > 200)
            return ValidationResult.Failure($"{parameterName} cannot exceed 200 characters");
            
        if (!ValidIdentifierRegex.IsMatch(identifier))
            return ValidationResult.Failure($"{parameterName} must be a valid identifier (start with letter or underscore, contain only letters, numbers, and underscores)");
            
        // Check for C# keywords
        var keywords = new[]
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is",
            "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override",
            "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte",
            "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch",
            "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
            "ushort", "using", "virtual", "void", "volatile", "while"
        };
        
        if (keywords.Contains(identifier.ToLowerInvariant()))
            return ValidationResult.Failure($"{parameterName} '{identifier}' is a reserved C# keyword");
            
        return ValidationResult.Success();
    }
    
    public static ValidationResult ValidateControlType(string controlType)
    {
        if (string.IsNullOrWhiteSpace(controlType))
            return ValidationResult.Failure("Control type cannot be empty");
            
        if (!ValidControlTypes.Contains(controlType))
            return ValidationResult.Failure($"Invalid control type '{controlType}'. Valid types are: {string.Join(", ", ValidControlTypes)}");
            
        return ValidationResult.Success();
    }
    
    public static ValidationResult ValidateComponentType(string componentType)
    {
        if (string.IsNullOrWhiteSpace(componentType))
            return ValidationResult.Failure("Component type cannot be empty");
            
        if (!ValidComponentTypes.Contains(componentType))
            return ValidationResult.Failure($"Invalid component type '{componentType}'. Valid types are: {string.Join(", ", ValidComponentTypes)}");
            
        return ValidationResult.Success();
    }
    
    public static ValidationResult ValidateThemeType(string themeType)
    {
        if (string.IsNullOrWhiteSpace(themeType))
            return ValidationResult.Failure("Theme type cannot be empty");
            
        if (!ValidThemeTypes.Contains(themeType))
            return ValidationResult.Failure($"Invalid theme type '{themeType}'. Valid types are: {string.Join(", ", ValidThemeTypes)}");
            
        return ValidationResult.Success();
    }
    
    public static ValidationResult ValidateCssClasses(string cssClasses)
    {
        if (string.IsNullOrWhiteSpace(cssClasses))
            return ValidationResult.Success(); // Empty is valid
            
        var classes = cssClasses.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var cssClass in classes)
        {
            var trimmedClass = cssClass.Trim();
            if (!ValidCssClassRegex.IsMatch(trimmedClass))
                return ValidationResult.Failure($"Invalid CSS class '{trimmedClass}'. CSS classes must start with a letter, underscore, or hyphen and contain only letters, numbers, underscores, and hyphens");
        }
        
        return ValidationResult.Success();
    }
    
    public static ValidationResult ValidateXamlContent(string xamlContent)
    {
        if (string.IsNullOrWhiteSpace(xamlContent))
            return ValidationResult.Failure("XAML content cannot be empty");
            
        if (xamlContent.Length > 1_000_000) // 1MB limit
            return ValidationResult.Failure("XAML content cannot exceed 1MB");
            
        // Basic XML structure validation
        if (!xamlContent.TrimStart().StartsWith('<'))
            return ValidationResult.Failure("XAML content must start with an XML element");
            
        // Check for potentially dangerous content
        var dangerousPatterns = new[]
        {
            "<script", "javascript:", "vbscript:", "data:", "file:", "eval(", "expression("
        };
        
        foreach (var pattern in dangerousPatterns)
        {
            if (xamlContent.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                return ValidationResult.Failure($"XAML content contains potentially dangerous pattern: {pattern}");
        }
        
        return ValidationResult.Success();
    }
    
    public static ValidationResult ValidateBooleanString(string value, string parameterName = "parameter")
    {
        if (string.IsNullOrWhiteSpace(value))
            return ValidationResult.Failure($"{parameterName} cannot be empty");
            
        if (!bool.TryParse(value, out _))
            return ValidationResult.Failure($"{parameterName} must be 'true' or 'false'");
            
        return ValidationResult.Success();
    }
    
    public static ValidationResult ValidateIntegerString(string value, int min = int.MinValue, int max = int.MaxValue, string parameterName = "parameter")
    {
        if (string.IsNullOrWhiteSpace(value))
            return ValidationResult.Failure($"{parameterName} cannot be empty");
            
        if (!int.TryParse(value, out var intValue))
            return ValidationResult.Failure($"{parameterName} must be a valid integer");
            
        if (intValue < min || intValue > max)
            return ValidationResult.Failure($"{parameterName} must be between {min} and {max}");
            
        return ValidationResult.Success();
    }
    
    public static ValidationResult ValidateEnumString<T>(string value, string parameterName = "parameter") where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return ValidationResult.Failure($"{parameterName} cannot be empty");
            
        if (!Enum.TryParse<T>(value, true, out _))
        {
            var validValues = Enum.GetNames<T>();
            return ValidationResult.Failure($"{parameterName} must be one of: {string.Join(", ", validValues)}");
        }
        
        return ValidationResult.Success();
    }
    
    public static ValidationResult ValidatePropertyList(string properties)
    {
        if (string.IsNullOrWhiteSpace(properties))
            return ValidationResult.Success(); // Empty is valid
            
        var propertyNames = properties.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var seenProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
        foreach (var property in propertyNames)
        {
            var trimmedProperty = property.Trim();
            
            var identifierValidation = ValidateIdentifier(trimmedProperty, "property name");
            if (!identifierValidation.IsValid)
                return identifierValidation;
                
            if (!seenProperties.Add(trimmedProperty))
                return ValidationResult.Failure($"Duplicate property name: {trimmedProperty}");
        }
        
        if (seenProperties.Count > 50)
            return ValidationResult.Failure("Too many properties specified (maximum 50)");
            
        return ValidationResult.Success();
    }
    
    public static ValidationResult ValidateFilePath(string filePath, bool mustExist = false)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return ValidationResult.Failure("File path cannot be empty");
            
        try
        {
            // Check for invalid path characters
            var invalidChars = Path.GetInvalidPathChars();
            if (filePath.Any(c => invalidChars.Contains(c)))
                return ValidationResult.Failure("File path contains invalid characters");
                
            // Check for dangerous patterns
            if (filePath.Contains("..") || filePath.Contains("~"))
                return ValidationResult.Failure("File path cannot contain relative navigation patterns");
                
            if (mustExist && !File.Exists(filePath))
                return ValidationResult.Failure($"File does not exist: {filePath}");
                
            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            return ValidationResult.Failure($"Invalid file path: {ex.Message}");
        }
    }
    
    public static ValidationResult ValidateUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return ValidationResult.Failure("URL cannot be empty");
            
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return ValidationResult.Failure("URL is not a valid absolute URI");
            
        if (uri.Scheme != "http" && uri.Scheme != "https")
            return ValidationResult.Failure("URL must use HTTP or HTTPS protocol");
            
        return ValidationResult.Success();
    }
    
    /// <summary>
    /// Sanitizes input by removing or encoding potentially dangerous characters
    /// </summary>
    public static string SanitizeInput(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
            
        // Remove null characters
        input = input.Replace("\0", string.Empty);
        
        // Remove control characters except tab, newline, and carriage return
        input = new string(input.Where(c => 
            !char.IsControl(c) || c == '\t' || c == '\n' || c == '\r').ToArray());
            
        // Trim excessive whitespace
        input = input.Trim();
        
        // Limit length to prevent DoS
        if (input.Length > 10000)
            input = input.Substring(0, 10000);
            
        return input;
    }
    
    /// <summary>
    /// Validates multiple parameters and returns the first validation failure
    /// </summary>
    public static ValidationResult ValidateAll(params ValidationResult[] validations)
    {
        foreach (var validation in validations)
        {
            if (!validation.IsValid)
                return validation;
        }
        return ValidationResult.Success();
    }
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
    
    public static ValidationResult Success() => new(true);
    public static ValidationResult Failure(string errorMessage) => new(false, errorMessage);
    
    public override string ToString() => IsValid ? "Valid" : $"Invalid: {ErrorMessage}";
}