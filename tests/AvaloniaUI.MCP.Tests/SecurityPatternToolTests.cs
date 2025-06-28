using AvaloniaUI.MCP.Tools;

namespace AvaloniaUI.MCP.Tests;

[TestClass]
public class SecurityPatternToolTests
{
    [DataTestMethod]
    [DataRow("jwt")]
    [DataRow("oauth")]
    [DataRow("basic")]
    [DataRow("multi-factor")]
    public void GenerateAuthenticationPattern_ValidAuthTypes_ReturnsSecurePattern(string authType)
    {
        // Act
        string result = SecurityPatternTool.GenerateAuthenticationPattern(authType);

        // Assert
        StringAssert.Contains(result, $"# Security Pattern: {authType}", "Result should contain security pattern header for the auth type");
        StringAssert.Contains(result, "SECURITY NOTICE", "Result should contain security notice");
        StringAssert.Contains(result, "DEFENSIVE security patterns", "Result should contain defensive security patterns mention");
        StringAssert.Contains(result, "PREVENT vulnerabilities", "Result should contain vulnerability prevention mention");
        StringAssert.Contains(result, "Security Best Practices", "Result should contain security best practices section");
        StringAssert.Contains(result, "Never store passwords in plain text", "Result should contain password storage best practice");
        StringAssert.Contains(result, "Use HTTPS for all authentication endpoints", "Result should contain HTTPS enforcement best practice");
    }

    [TestMethod]
    public void GenerateAuthenticationPattern_JWT_ContainsJWTSpecificElements()
    {
        // Act
        string result = SecurityPatternTool.GenerateAuthenticationPattern("jwt", "true", "true", "standard");

        // Assert
        StringAssert.Contains(result, "JWT Authentication Service", "Result should contain JWT authentication service");
        StringAssert.Contains(result, "IJwtAuthenticationService", "Result should contain JWT authentication service interface");
        StringAssert.Contains(result, "AuthenticateAsync", "Result should contain authenticate method");
        StringAssert.Contains(result, "RefreshTokenAsync", "Result should contain refresh token method");
        StringAssert.Contains(result, "ValidateTokenAsync", "Result should contain validate token method");
        StringAssert.Contains(result, "RevokeTokenAsync", "Result should contain revoke token method");
        StringAssert.Contains(result, "Rate limiting", "Result should contain rate limiting mention");
    }

    [TestMethod]
    public void GenerateAuthenticationPattern_OAuth_ContainsOAuthSpecificElements()
    {
        // Act
        string result = SecurityPatternTool.GenerateAuthenticationPattern("oauth");

        // Assert
        StringAssert.Contains(result, "OAuth 2.0 Authentication Service", "Result should contain OAuth 2.0 authentication service");
        StringAssert.Contains(result, "IOAuthAuthenticationService", "Result should contain OAuth authentication service interface");
        StringAssert.Contains(result, "GetAuthorizationUrlAsync", "Result should contain get authorization URL method");
        StringAssert.Contains(result, "ExchangeCodeForTokenAsync", "Result should contain code exchange method");
        StringAssert.Contains(result, "PKCE", "Result should contain PKCE security feature");
        StringAssert.Contains(result, "state parameter for CSRF protection", "Result should contain CSRF protection mention");
    }

    [TestMethod]
    public void GenerateAuthenticationPattern_BasicAuth_ContainsTimingAttackPrevention()
    {
        // Act
        string result = SecurityPatternTool.GenerateAuthenticationPattern("basic");

        // Assert
        StringAssert.Contains(result, "Basic Authentication Service", "Result should contain basic authentication service");
        StringAssert.Contains(result, "consistent timing to prevent username enumeration", "Result should contain timing attack prevention");
        StringAssert.Contains(result, "dummy", "Result should contain dummy operation for timing consistency");
    }

    [TestMethod]
    public void GenerateAuthenticationPattern_MultiFactor_ContainsMFAElements()
    {
        // Act
        string result = SecurityPatternTool.GenerateAuthenticationPattern("multi-factor");

        // Assert
        StringAssert.Contains(result, "Multi-Factor Authentication Service", "Result should contain multi-factor authentication service");
        StringAssert.Contains(result, "IMultiFactorAuthenticationService", "Result should contain MFA service interface");
        StringAssert.Contains(result, "SetupMfaAsync", "Result should contain MFA setup method");
        StringAssert.Contains(result, "VerifyMfaAsync", "Result should contain MFA verification method");
        StringAssert.Contains(result, "GenerateBackupCodesAsync", "Result should contain backup codes generation method");
        StringAssert.Contains(result, "TOTP", "Result should contain TOTP authentication");
    }

    [TestMethod]
    public void GenerateAuthenticationPattern_WithEncryption_IncludesEncryptionService()
    {
        // Act
        string result = SecurityPatternTool.GenerateAuthenticationPattern("jwt", "true");

        // Assert
        StringAssert.Contains(result, "## Encryption Service", "Result should contain encryption service section");
        StringAssert.Contains(result, "IEncryptionService", "Result should contain encryption service interface");
        StringAssert.Contains(result, "EncryptString", "Result should contain string encryption method");
        StringAssert.Contains(result, "DecryptString", "Result should contain string decryption method");
        StringAssert.Contains(result, "GenerateKey", "Result should contain key generation method");
        StringAssert.Contains(result, "HashPassword", "Result should contain password hashing method");
        StringAssert.Contains(result, "BCrypt", "Result should contain BCrypt implementation");
    }

    [TestMethod]
    public void GenerateAuthenticationPattern_WithValidation_IncludesInputValidation()
    {
        // Act
        string result = SecurityPatternTool.GenerateAuthenticationPattern("jwt", "false", "true");

        // Assert
        StringAssert.Contains(result, "## Input Validation", "Result should contain input validation section");
        StringAssert.Contains(result, "IInputValidationService", "Result should contain input validation service interface");
        StringAssert.Contains(result, "ValidateEmail", "Result should contain email validation method");
        StringAssert.Contains(result, "ValidatePassword", "Result should contain password validation method");
        StringAssert.Contains(result, "SanitizeInput", "Result should contain input sanitization method");
        StringAssert.Contains(result, "IsValidSqlInput", "Result should contain SQL injection validation method");
    }

    [DataTestMethod]
    [DataRow("personal")]
    [DataRow("financial")]
    [DataRow("medical")]
    [DataRow("general")]
    public void GenerateDataSecurityPattern_ValidDataTypes_ReturnsSecurePattern(string dataType)
    {
        // Act
        string result = SecurityPatternTool.GenerateDataSecurityPattern(dataType);

        // Assert
        StringAssert.Contains(result, $"# Data Security Pattern: {dataType}", "Result should contain data security pattern header for the data type");
        StringAssert.Contains(result, "DEFENSIVE SECURITY TOOL", "Result should contain defensive security tool mention");
        StringAssert.Contains(result, "PROTECT against threats", "Result should contain threat protection mention");
        StringAssert.Contains(result, "industry best practices", "Result should contain industry best practices mention");
        StringAssert.Contains(result, "compliance requirements", "Result should contain compliance requirements mention");
    }

    [TestMethod]
    public void GenerateDataSecurityPattern_PersonalData_IncludesGDPRCompliance()
    {
        // Act
        string result = SecurityPatternTool.GenerateDataSecurityPattern("personal");

        // Assert
        StringAssert.Contains(result, "GDPR Compliance Requirements", "Result should contain GDPR compliance requirements");
        StringAssert.Contains(result, "Data Subject Rights", "Result should contain data subject rights");
        StringAssert.Contains(result, "Privacy by Design", "Result should contain privacy by design principle");
        StringAssert.Contains(result, "Breach Notification", "Result should contain breach notification requirements");
        StringAssert.Contains(result, "72 hours", "Result should contain 72 hours notification timeframe");
    }

    [TestMethod]
    public void GenerateDataSecurityPattern_FinancialData_IncludesPCIDSSCompliance()
    {
        // Act
        string result = SecurityPatternTool.GenerateDataSecurityPattern("financial");

        // Assert
        StringAssert.Contains(result, "PCI DSS Compliance Requirements", "Result should contain PCI DSS compliance requirements");
        StringAssert.Contains(result, "Cardholder Data Protection", "Result should contain cardholder data protection");
        StringAssert.Contains(result, "SOX Compliance Requirements", "Result should contain SOX compliance requirements");
        StringAssert.Contains(result, "Internal Controls", "Result should contain internal controls");
        StringAssert.Contains(result, "Audit Trails", "Result should contain audit trails");
    }

    [TestMethod]
    public void GenerateDataSecurityPattern_MedicalData_IncludesHIPAACompliance()
    {
        // Act
        string result = SecurityPatternTool.GenerateDataSecurityPattern("medical");

        // Assert
        StringAssert.Contains(result, "HIPAA Compliance Requirements", "Result should contain HIPAA compliance requirements");
        StringAssert.Contains(result, "Administrative Safeguards", "Result should contain administrative safeguards");
        StringAssert.Contains(result, "Physical Safeguards", "Result should contain physical safeguards");
        StringAssert.Contains(result, "Technical Safeguards", "Result should contain technical safeguards");
        StringAssert.Contains(result, "Business Associate Agreements", "Result should contain business associate agreements");
        StringAssert.Contains(result, "PHI", "Result should contain PHI (Protected Health Information)");
    }

    [TestMethod]
    public void GenerateDataSecurityPattern_WithSanitization_IncludesDataSanitization()
    {
        // Act
        string result = SecurityPatternTool.GenerateDataSecurityPattern("general", "aes", "true");

        // Assert
        StringAssert.Contains(result, "## Data Sanitization", "Result should contain data sanitization section");
        StringAssert.Contains(result, "IDataSanitizationService", "Result should contain data sanitization service interface");
        StringAssert.Contains(result, "SanitizeHtml", "Result should contain HTML sanitization method");
        StringAssert.Contains(result, "SanitizeSqlInput", "Result should contain SQL input sanitization method");
        StringAssert.Contains(result, "SanitizeFileName", "Result should contain filename sanitization method");
        StringAssert.Contains(result, "Remove script tags", "Result should contain script tag removal");
        StringAssert.Contains(result, "SQL injection", "Result should contain SQL injection prevention");
    }

    [TestMethod]
    public void GenerateDataSecurityPattern_WithAuditLog_IncludesAuditLogging()
    {
        // Act
        string result = SecurityPatternTool.GenerateDataSecurityPattern("general", "aes", "false", "true");

        // Assert
        StringAssert.Contains(result, "## Audit Logging", "Result should contain audit logging section");
        StringAssert.Contains(result, "IAuditLogger", "Result should contain audit logger interface");
        StringAssert.Contains(result, "LogDataAccessAsync", "Result should contain data access logging method");
        StringAssert.Contains(result, "LogSecurityEventAsync", "Result should contain security event logging method");
        StringAssert.Contains(result, "LogLoginAttemptAsync", "Result should contain login attempt logging method");
        StringAssert.Contains(result, "LogPermissionChangeAsync", "Result should contain permission change logging method");
    }

    [TestMethod]
    public void GenerateDataSecurityPattern_IncludesSecureDataService()
    {
        // Act
        string result = SecurityPatternTool.GenerateDataSecurityPattern("general");

        // Assert
        StringAssert.Contains(result, "## Secure Data Service", "Result should contain secure data service section");
        StringAssert.Contains(result, "ISecureDataService", "Result should contain secure data service interface");
        StringAssert.Contains(result, "EncryptEntity", "Result should contain entity encryption method");
        StringAssert.Contains(result, "DecryptEntity", "Result should contain entity decryption method");
        StringAssert.Contains(result, "SanitizeEntity", "Result should contain entity sanitization method");
        StringAssert.Contains(result, "EncryptAttribute", "Result should contain attribute encryption");
        StringAssert.Contains(result, "SanitizeAttribute", "Result should contain attribute sanitization");
    }

    [TestMethod]
    public void GenerateAuthenticationPattern_InvalidAuthType_ReturnsGenericPattern()
    {
        // Act
        string result = SecurityPatternTool.GenerateAuthenticationPattern("invalid-type");

        // Assert
        StringAssert.Contains(result, "# Security Pattern: invalid-type", "Result should contain security pattern header for invalid auth type");
        StringAssert.Contains(result, "Generic Authentication Service", "Result should contain generic authentication service for invalid type");
    }

    [DataTestMethod]
    [DataRow("aes")]
    [DataRow("rsa")]
    [DataRow("hybrid")]
    public void GenerateDataSecurityPattern_ValidEncryptionMethods_ReturnsPattern(string encryptionMethod)
    {
        // Act
        string result = SecurityPatternTool.GenerateDataSecurityPattern("general", encryptionMethod);

        // Assert
        StringAssert.Contains(result, $"**Encryption Method**: {encryptionMethod}", "Result should contain the encryption method specification");
        StringAssert.Contains(result, "Secure Data Service", "Result should contain secure data service");
    }

    [TestMethod]
    public void GenerateAuthenticationPattern_ContainsSecurityLevelRequirements()
    {
        // Act
        string basicResult = SecurityPatternTool.GenerateAuthenticationPattern("jwt", "true", "true", "basic");
        string standardResult = SecurityPatternTool.GenerateAuthenticationPattern("jwt", "true", "true", "standard");
        string highResult = SecurityPatternTool.GenerateAuthenticationPattern("jwt", "true", "true", "high");

        // Assert - Basic level
        StringAssert.Contains(basicResult, "Password complexity requirements", "Basic level result should contain password complexity requirements");
        StringAssert.Contains(basicResult, "HTTPS enforcement", "Basic level result should contain HTTPS enforcement");

        // Assert - Standard level
        StringAssert.Contains(standardResult, "Multi-factor authentication recommended", "Standard level result should contain MFA recommendation");
        StringAssert.Contains(standardResult, "Rate limiting and account lockout", "Standard level result should contain rate limiting and lockout");

        // Assert - High level
        StringAssert.Contains(highResult, "Mandatory multi-factor authentication", "High level result should contain mandatory MFA");
        StringAssert.Contains(highResult, "Zero-trust architecture", "High level result should contain zero-trust architecture");
        StringAssert.Contains(highResult, "Advanced threat detection", "High level result should contain advanced threat detection");
    }
}
