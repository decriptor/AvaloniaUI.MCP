using System.Text;

namespace AvaloniaUI.MCP.Services;

/// <summary>
/// Provides comprehensive error handling and user-friendly error messages for MCP tools
/// </summary>
public static class ErrorHandlingService
{
    /// <summary>
    /// Wraps tool execution with comprehensive error handling
    /// </summary>
    public static string SafeExecute(string toolName, Func<string> toolOperation)
    {
        try
        {
            return toolOperation();
        }
        catch (ArgumentException ex)
        {
            return CreateUserFriendlyError(toolName, "Invalid Input", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return CreateUserFriendlyError(toolName, "Operation Error", ex.Message);
        }
        catch (FileNotFoundException ex)
        {
            return CreateUserFriendlyError(toolName, "File Not Found", $"Required file not found: {ex.FileName}");
        }
        catch (DirectoryNotFoundException ex)
        {
            return CreateUserFriendlyError(toolName, "Directory Not Found", ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return CreateUserFriendlyError(toolName, "Access Denied", "Insufficient permissions to access the requested resource");
        }
        catch (TimeoutException)
        {
            return CreateUserFriendlyError(toolName, "Operation Timeout", "The operation took too long to complete. Please try again.");
        }
        catch (OutOfMemoryException)
        {
            return CreateUserFriendlyError(toolName, "Memory Error", "The operation requires more memory than available. Try reducing the input size.");
        }
        catch (NotSupportedException ex)
        {
            return CreateUserFriendlyError(toolName, "Not Supported", ex.Message);
        }
        catch (FormatException)
        {
            return CreateUserFriendlyError(toolName, "Format Error", "The input format is incorrect. Please check your parameters.");
        }
        catch (Exception ex)
        {
            return CreateUserFriendlyError(toolName, "Unexpected Error", $"An unexpected error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Wraps async tool execution with comprehensive error handling
    /// </summary>
    public static async Task<string> SafeExecuteAsync(string toolName, Func<Task<string>> toolOperation)
    {
        try
        {
            return await toolOperation();
        }
        catch (ArgumentException ex)
        {
            return CreateUserFriendlyError(toolName, "Invalid Input", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return CreateUserFriendlyError(toolName, "Operation Error", ex.Message);
        }
        catch (FileNotFoundException ex)
        {
            return CreateUserFriendlyError(toolName, "File Not Found", $"Required file not found: {ex.FileName}");
        }
        catch (DirectoryNotFoundException ex)
        {
            return CreateUserFriendlyError(toolName, "Directory Not Found", ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return CreateUserFriendlyError(toolName, "Access Denied", "Insufficient permissions to access the requested resource");
        }
        catch (TimeoutException)
        {
            return CreateUserFriendlyError(toolName, "Operation Timeout", "The operation took too long to complete. Please try again.");
        }
        catch (OutOfMemoryException)
        {
            return CreateUserFriendlyError(toolName, "Memory Error", "The operation requires more memory than available. Try reducing the input size.");
        }
        catch (NotSupportedException ex)
        {
            return CreateUserFriendlyError(toolName, "Not Supported", ex.Message);
        }
        catch (FormatException)
        {
            return CreateUserFriendlyError(toolName, "Format Error", "The input format is incorrect. Please check your parameters.");
        }
        catch (HttpRequestException ex)
        {
            return CreateUserFriendlyError(toolName, "Network Error", $"Network request failed: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return CreateUserFriendlyError(toolName, "Request Cancelled", "The operation was cancelled or timed out");
        }
        catch (Exception ex)
        {
            return CreateUserFriendlyError(toolName, "Unexpected Error", $"An unexpected error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a user-friendly error message with helpful information
    /// </summary>
    public static string CreateUserFriendlyError(string toolName, string errorType, string errorMessage)
    {
        var sb = new StringBuilder();

        sb.AppendLine("# ❌ Error");
        sb.AppendLine();
        sb.AppendLine($"**Tool**: {toolName}");
        sb.AppendLine($"**Error Type**: {errorType}");
        sb.AppendLine($"**Details**: {errorMessage}");
        sb.AppendLine();
        sb.AppendLine("## 🔧 Troubleshooting");

        switch (errorType.ToLowerInvariant())
        {
            case "invalid input":
                sb.AppendLine("- Check that all required parameters are provided");
                sb.AppendLine("- Verify parameter formats match the expected types");
                sb.AppendLine("- Ensure text inputs don't contain special characters");
                sb.AppendLine("- Check that file paths are valid and accessible");
                break;

            case "operation error":
                sb.AppendLine("- Verify all prerequisites are met");
                sb.AppendLine("- Check that dependent resources are available");
                sb.AppendLine("- Ensure the operation is valid for the current context");
                break;

            case "file not found":
            case "directory not found":
                sb.AppendLine("- Verify the file or directory path is correct");
                sb.AppendLine("- Ensure the file or directory exists");
                sb.AppendLine("- Check that you have read permissions");
                sb.AppendLine("- Try using an absolute path instead of relative");
                break;

            case "access denied":
                sb.AppendLine("- Check file and directory permissions");
                sb.AppendLine("- Ensure the application has necessary access rights");
                sb.AppendLine("- Try running with elevated permissions if needed");
                break;

            case "operation timeout":
                sb.AppendLine("- Try reducing the input size or complexity");
                sb.AppendLine("- Check your network connection if applicable");
                sb.AppendLine("- Retry the operation after a brief wait");
                break;

            case "memory error":
                sb.AppendLine("- Reduce the size of input data");
                sb.AppendLine("- Break large operations into smaller chunks");
                sb.AppendLine("- Close other applications to free memory");
                break;

            case "format error":
                sb.AppendLine("- Check the format of input parameters");
                sb.AppendLine("- Verify color codes are valid hex values");
                sb.AppendLine("- Ensure boolean values are 'true' or 'false'");
                sb.AppendLine("- Validate that identifiers follow naming conventions");
                break;

            case "network error":
                sb.AppendLine("- Check your internet connection");
                sb.AppendLine("- Verify the target URL is accessible");
                sb.AppendLine("- Check for firewall or proxy restrictions");
                sb.AppendLine("- Try again after a brief wait");
                break;

            case "not supported":
                sb.AppendLine("- Check if the feature is available in this version");
                sb.AppendLine("- Verify the operation is supported on your platform");
                sb.AppendLine("- Review the tool documentation for alternatives");
                break;

            default:
                sb.AppendLine("- Check the input parameters for correctness");
                sb.AppendLine("- Verify all required dependencies are available");
                sb.AppendLine("- Try the operation again with different parameters");
                sb.AppendLine("- Contact support if the issue persists");
                break;
        }

        sb.AppendLine();
        sb.AppendLine("## 📚 Help Resources");
        sb.AppendLine("- Review the tool documentation for parameter requirements");
        sb.AppendLine("- Check example usage patterns");
        sb.AppendLine("- Verify your input follows the expected format");
        sb.AppendLine("- Use the Echo tool to test server connectivity");

        return sb.ToString();
    }

    /// <summary>
    /// Creates a validation error message with specific guidance
    /// </summary>
    public static string CreateValidationError(string toolName, ValidationResult validationResult)
    {
        return validationResult.IsValid ? string.Empty : CreateUserFriendlyError(toolName, "Validation Error", validationResult.ErrorMessage);
    }

    /// <summary>
    /// Creates a validation error message for multiple validation failures
    /// </summary>
    public static string CreateValidationErrors(string toolName, params ValidationResult[] validationResults)
    {
        var failures = validationResults.Where(v => !v.IsValid).ToList();
        if (failures.Count == 0)
        {
            return string.Empty;
        }

        if (failures.Count == 1)
        {
            return CreateValidationError(toolName, failures[0]);
        }

        var sb = new StringBuilder();
        sb.AppendLine("# ❌ Validation Errors");
        sb.AppendLine();
        sb.AppendLine($"**Tool**: {toolName}");
        sb.AppendLine($"**Error Count**: {failures.Count}");
        sb.AppendLine();
        sb.AppendLine("## Issues Found:");

        for (int i = 0; i < failures.Count; i++)
        {
            sb.AppendLine($"{i + 1}. {failures[i].ErrorMessage}");
        }

        sb.AppendLine();
        sb.AppendLine("## 🔧 Resolution");
        sb.AppendLine("- Fix all validation errors listed above");
        sb.AppendLine("- Ensure all parameters meet the required format");
        sb.AppendLine("- Check the tool documentation for parameter specifications");
        sb.AppendLine("- Try again after correcting the inputs");

        return sb.ToString();
    }

    /// <summary>
    /// Logs error information for debugging purposes
    /// </summary>
    public static void LogError(string toolName, Exception exception, string? additionalContext = null)
    {
        // In a real application, this would write to a proper logging system
        // For this MCP server, we'll use console output
        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC");

        Console.WriteLine($"[{timestamp}] ERROR in {toolName}:");
        Console.WriteLine($"  Type: {exception.GetType().Name}");
        Console.WriteLine($"  Message: {exception.Message}");

        if (!string.IsNullOrEmpty(additionalContext))
        {
            Console.WriteLine($"  Context: {additionalContext}");
        }

        if (exception.InnerException != null)
        {
            Console.WriteLine($"  Inner Exception: {exception.InnerException.Message}");
        }

        // Only log stack trace for unexpected errors
        if (exception is not (ArgumentException or InvalidOperationException or FormatException))
        {
            Console.WriteLine($"  Stack Trace: {exception.StackTrace}");
        }
    }

    /// <summary>
    /// Creates a warning message for non-critical issues
    /// </summary>
    public static string CreateWarning(string toolName, string warningMessage, string? suggestion = null)
    {
        var sb = new StringBuilder();

        sb.AppendLine("# ⚠️ Warning");
        sb.AppendLine();
        sb.AppendLine($"**Tool**: {toolName}");
        sb.AppendLine($"**Warning**: {warningMessage}");

        if (!string.IsNullOrEmpty(suggestion))
        {
            sb.AppendLine();
            sb.AppendLine("## 💡 Suggestion");
            sb.AppendLine(suggestion);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Validates common tool parameters and returns aggregated validation result
    /// </summary>
    public static ValidationResult ValidateCommonParameters(
        string? projectName = null,
        string? templateType = null,
        string? controlType = null,
        string? componentType = null,
        string? themeType = null,
        string? colorHex = null,
        string? identifier = null,
        string? xamlContent = null,
        string? booleanValue = null,
        string? integerValue = null,
        string? properties = null,
        string? cssClasses = null,
        string? filePath = null,
        string? url = null)
    {
        var validations = new List<ValidationResult>();

        if (projectName != null)
        {
            validations.Add(InputValidationService.ValidateProjectName(projectName));
        }

        if (templateType != null)
        {
            validations.Add(InputValidationService.ValidateTemplateType(templateType));
        }

        if (controlType != null)
        {
            validations.Add(InputValidationService.ValidateControlType(controlType));
        }

        if (componentType != null)
        {
            validations.Add(InputValidationService.ValidateComponentType(componentType));
        }

        if (themeType != null)
        {
            validations.Add(InputValidationService.ValidateThemeType(themeType));
        }

        if (colorHex != null)
        {
            validations.Add(InputValidationService.ValidateColorHex(colorHex));
        }

        if (identifier != null)
        {
            validations.Add(InputValidationService.ValidateIdentifier(identifier));
        }

        if (xamlContent != null)
        {
            validations.Add(InputValidationService.ValidateXamlContent(xamlContent));
        }

        if (booleanValue != null)
        {
            validations.Add(InputValidationService.ValidateBooleanString(booleanValue));
        }

        if (integerValue != null)
        {
            validations.Add(InputValidationService.ValidateIntegerString(integerValue));
        }

        if (properties != null)
        {
            validations.Add(InputValidationService.ValidatePropertyList(properties));
        }

        if (cssClasses != null)
        {
            validations.Add(InputValidationService.ValidateCssClasses(cssClasses));
        }

        if (filePath != null)
        {
            validations.Add(InputValidationService.ValidateFilePath(filePath));
        }

        if (url != null)
        {
            validations.Add(InputValidationService.ValidateUrl(url));
        }

        return InputValidationService.ValidateAll([.. validations]);
    }
}