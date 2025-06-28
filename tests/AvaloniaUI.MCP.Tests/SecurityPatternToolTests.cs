using AvaloniaUI.MCP.Tools;
using Xunit;

namespace AvaloniaUI.MCP.Tests;

public class SecurityPatternToolTests
{
    [Theory]
    [InlineData("jwt")]
    [InlineData("oauth")]
    [InlineData("basic")]
    [InlineData("multi-factor")]
    public void GenerateAuthenticationPattern_ValidAuthTypes_ReturnsSecurePattern(string authType)
    {
        // Act
        var result = SecurityPatternTool.GenerateAuthenticationPattern(authType);
        
        // Assert
        Assert.Contains($"# Security Pattern: {authType}", result);
        Assert.Contains("SECURITY NOTICE", result);
        Assert.Contains("DEFENSIVE security patterns", result);
        Assert.Contains("PREVENT vulnerabilities", result);
        Assert.Contains("Security Best Practices", result);
        Assert.Contains("Never store passwords in plain text", result);
        Assert.Contains("Use HTTPS for all authentication endpoints", result);
    }

    [Fact]
    public void GenerateAuthenticationPattern_JWT_ContainsJWTSpecificElements()
    {
        // Act
        var result = SecurityPatternTool.GenerateAuthenticationPattern("jwt", "true", "true", "standard");
        
        // Assert
        Assert.Contains("JWT Authentication Service", result);
        Assert.Contains("IJwtAuthenticationService", result);
        Assert.Contains("AuthenticateAsync", result);
        Assert.Contains("RefreshTokenAsync", result);
        Assert.Contains("ValidateTokenAsync", result);
        Assert.Contains("RevokeTokenAsync", result);
        Assert.Contains("Rate limiting", result);
    }

    [Fact]
    public void GenerateAuthenticationPattern_OAuth_ContainsOAuthSpecificElements()
    {
        // Act
        var result = SecurityPatternTool.GenerateAuthenticationPattern("oauth");
        
        // Assert
        Assert.Contains("OAuth 2.0 Authentication Service", result);
        Assert.Contains("IOAuthAuthenticationService", result);
        Assert.Contains("GetAuthorizationUrlAsync", result);
        Assert.Contains("ExchangeCodeForTokenAsync", result);
        Assert.Contains("PKCE", result);
        Assert.Contains("state parameter for CSRF protection", result);
    }

    [Fact]
    public void GenerateAuthenticationPattern_BasicAuth_ContainsTimingAttackPrevention()
    {
        // Act
        var result = SecurityPatternTool.GenerateAuthenticationPattern("basic");
        
        // Assert
        Assert.Contains("Basic Authentication Service", result);
        Assert.Contains("consistent timing to prevent username enumeration", result);
        Assert.Contains("dummy", result);
    }

    [Fact]
    public void GenerateAuthenticationPattern_MultiFactor_ContainsMFAElements()
    {
        // Act
        var result = SecurityPatternTool.GenerateAuthenticationPattern("multi-factor");
        
        // Assert
        Assert.Contains("Multi-Factor Authentication Service", result);
        Assert.Contains("IMultiFactorAuthenticationService", result);
        Assert.Contains("SetupMfaAsync", result);
        Assert.Contains("VerifyMfaAsync", result);
        Assert.Contains("GenerateBackupCodesAsync", result);
        Assert.Contains("TOTP", result);
    }

    [Fact]
    public void GenerateAuthenticationPattern_WithEncryption_IncludesEncryptionService()
    {
        // Act
        var result = SecurityPatternTool.GenerateAuthenticationPattern("jwt", "true");
        
        // Assert
        Assert.Contains("## Encryption Service", result);
        Assert.Contains("IEncryptionService", result);
        Assert.Contains("EncryptString", result);
        Assert.Contains("DecryptString", result);
        Assert.Contains("GenerateKey", result);
        Assert.Contains("HashPassword", result);
        Assert.Contains("BCrypt", result);
    }

    [Fact]
    public void GenerateAuthenticationPattern_WithValidation_IncludesInputValidation()
    {
        // Act
        var result = SecurityPatternTool.GenerateAuthenticationPattern("jwt", "false", "true");
        
        // Assert
        Assert.Contains("## Input Validation", result);
        Assert.Contains("IInputValidationService", result);
        Assert.Contains("ValidateEmail", result);
        Assert.Contains("ValidatePassword", result);
        Assert.Contains("SanitizeInput", result);
        Assert.Contains("IsValidSqlInput", result);
    }

    [Theory]
    [InlineData("personal")]
    [InlineData("financial")]
    [InlineData("medical")]
    [InlineData("general")]
    public void GenerateDataSecurityPattern_ValidDataTypes_ReturnsSecurePattern(string dataType)
    {
        // Act
        var result = SecurityPatternTool.GenerateDataSecurityPattern(dataType);
        
        // Assert
        Assert.Contains($"# Data Security Pattern: {dataType}", result);
        Assert.Contains("DEFENSIVE SECURITY TOOL", result);
        Assert.Contains("PROTECT against threats", result);
        Assert.Contains("industry best practices", result);
        Assert.Contains("compliance requirements", result);
    }

    [Fact]
    public void GenerateDataSecurityPattern_PersonalData_IncludesGDPRCompliance()
    {
        // Act
        var result = SecurityPatternTool.GenerateDataSecurityPattern("personal");
        
        // Assert
        Assert.Contains("GDPR Compliance Requirements", result);
        Assert.Contains("Data Subject Rights", result);
        Assert.Contains("Privacy by Design", result);
        Assert.Contains("Breach Notification", result);
        Assert.Contains("72 hours", result);
    }

    [Fact]
    public void GenerateDataSecurityPattern_FinancialData_IncludesPCIDSSCompliance()
    {
        // Act
        var result = SecurityPatternTool.GenerateDataSecurityPattern("financial");
        
        // Assert
        Assert.Contains("PCI DSS Compliance Requirements", result);
        Assert.Contains("Cardholder Data Protection", result);
        Assert.Contains("SOX Compliance Requirements", result);
        Assert.Contains("Internal Controls", result);
        Assert.Contains("Audit Trails", result);
    }

    [Fact]
    public void GenerateDataSecurityPattern_MedicalData_IncludesHIPAACompliance()
    {
        // Act
        var result = SecurityPatternTool.GenerateDataSecurityPattern("medical");
        
        // Assert
        Assert.Contains("HIPAA Compliance Requirements", result);
        Assert.Contains("Administrative Safeguards", result);
        Assert.Contains("Physical Safeguards", result);
        Assert.Contains("Technical Safeguards", result);
        Assert.Contains("Business Associate Agreements", result);
        Assert.Contains("PHI", result);
    }

    [Fact]
    public void GenerateDataSecurityPattern_WithSanitization_IncludesDataSanitization()
    {
        // Act
        var result = SecurityPatternTool.GenerateDataSecurityPattern("general", "aes", "true");
        
        // Assert
        Assert.Contains("## Data Sanitization", result);
        Assert.Contains("IDataSanitizationService", result);
        Assert.Contains("SanitizeHtml", result);
        Assert.Contains("SanitizeSqlInput", result);
        Assert.Contains("SanitizeFileName", result);
        Assert.Contains("Remove script tags", result);
        Assert.Contains("SQL injection", result);
    }

    [Fact]
    public void GenerateDataSecurityPattern_WithAuditLog_IncludesAuditLogging()
    {
        // Act
        var result = SecurityPatternTool.GenerateDataSecurityPattern("general", "aes", "false", "true");
        
        // Assert
        Assert.Contains("## Audit Logging", result);
        Assert.Contains("IAuditLogger", result);
        Assert.Contains("LogDataAccessAsync", result);
        Assert.Contains("LogSecurityEventAsync", result);
        Assert.Contains("LogLoginAttemptAsync", result);
        Assert.Contains("LogPermissionChangeAsync", result);
    }

    [Fact]
    public void GenerateDataSecurityPattern_IncludesSecureDataService()
    {
        // Act
        var result = SecurityPatternTool.GenerateDataSecurityPattern("general");
        
        // Assert
        Assert.Contains("## Secure Data Service", result);
        Assert.Contains("ISecureDataService", result);
        Assert.Contains("EncryptEntity", result);
        Assert.Contains("DecryptEntity", result);
        Assert.Contains("SanitizeEntity", result);
        Assert.Contains("EncryptAttribute", result);
        Assert.Contains("SanitizeAttribute", result);
    }

    [Fact]
    public void GenerateAuthenticationPattern_InvalidAuthType_ReturnsGenericPattern()
    {
        // Act
        var result = SecurityPatternTool.GenerateAuthenticationPattern("invalid-type");
        
        // Assert
        Assert.Contains("# Security Pattern: invalid-type", result);
        Assert.Contains("Generic Authentication Service", result);
    }

    [Theory]
    [InlineData("aes")]
    [InlineData("rsa")]
    [InlineData("hybrid")]
    public void GenerateDataSecurityPattern_ValidEncryptionMethods_ReturnsPattern(string encryptionMethod)
    {
        // Act
        var result = SecurityPatternTool.GenerateDataSecurityPattern("general", encryptionMethod);
        
        // Assert
        Assert.Contains($"**Encryption Method**: {encryptionMethod}", result);
        Assert.Contains("Secure Data Service", result);
    }

    [Fact]
    public void GenerateAuthenticationPattern_ContainsSecurityLevelRequirements()
    {
        // Act
        var basicResult = SecurityPatternTool.GenerateAuthenticationPattern("jwt", "true", "true", "basic");
        var standardResult = SecurityPatternTool.GenerateAuthenticationPattern("jwt", "true", "true", "standard");
        var highResult = SecurityPatternTool.GenerateAuthenticationPattern("jwt", "true", "true", "high");
        
        // Assert - Basic level
        Assert.Contains("Password complexity requirements", basicResult);
        Assert.Contains("HTTPS enforcement", basicResult);
        
        // Assert - Standard level
        Assert.Contains("Multi-factor authentication recommended", standardResult);
        Assert.Contains("Rate limiting and account lockout", standardResult);
        
        // Assert - High level
        Assert.Contains("Mandatory multi-factor authentication", highResult);
        Assert.Contains("Zero-trust architecture", highResult);
        Assert.Contains("Advanced threat detection", highResult);
    }
}