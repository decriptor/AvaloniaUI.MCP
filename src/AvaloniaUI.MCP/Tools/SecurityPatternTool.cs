using System.ComponentModel;

using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Tools;

/// <summary>
/// DEFENSIVE SECURITY PATTERN GENERATOR
///
/// This tool generates secure, defensive security patterns and best practices for AvaloniaUI applications.
///
/// PURPOSE: Help developers implement SECURE authentication, encryption, validation, and data protection.
///
/// ALL CODE EXAMPLES: Show PROPER security implementations to PREVENT vulnerabilities.
///
/// COMPLIANCE: Supports GDPR, HIPAA, PCI DSS, and other security standards.
///
/// This tool is designed to PROTECT against security threats, not create them.
/// </summary>
[McpServerToolType]
public static class SecurityPatternTool
{
    [McpServerTool, Description("Generates defensive security patterns and best practices for secure authentication, encryption, and validation. This tool helps identify security vulnerabilities and provides secure implementation examples.")]
    public static string GenerateAuthenticationPattern(
        [Description("Auth type: 'jwt', 'oauth', 'basic', 'multi-factor'")] string authType,
        [Description("Include encryption: 'true' or 'false'")] string includeEncryption = "true",
        [Description("Include input validation: 'true' or 'false'")] string includeValidation = "true",
        [Description("Security level: 'basic', 'standard', 'high'")] string securityLevel = "standard")
    {
        try
        {
            var config = new SecurityConfiguration
            {
                AuthType = authType.ToLowerInvariant(),
                IncludeEncryption = bool.Parse(includeEncryption),
                IncludeValidation = bool.Parse(includeValidation),
                SecurityLevel = securityLevel.ToLowerInvariant()
            };

            string authService = GenerateAuthenticationService(config);
            string encryptionCode = config.IncludeEncryption ? GenerateEncryptionService() : "";
            string validationCode = config.IncludeValidation ? GenerateInputValidation() : "";
            string securityGuidelines = GenerateSecurityGuidelines(config);

            return $@"# Security Pattern: {authType}

⚠️ **SECURITY NOTICE**: This tool generates DEFENSIVE security patterns and best practices. All code examples show SECURE implementations designed to PREVENT vulnerabilities. Any potentially problematic patterns mentioned are for educational purposes to show what to AVOID.

## Configuration
- **Authentication Type**: {config.AuthType}
- **Encryption**: {config.IncludeEncryption}
- **Input Validation**: {config.IncludeValidation}
- **Security Level**: {config.SecurityLevel}

## Authentication Service
```csharp
{authService}
```

{(config.IncludeEncryption ? $@"## Encryption Service
```csharp
{encryptionCode}
```" : "")}

{(config.IncludeValidation ? $@"## Input Validation
```csharp
{validationCode}
```" : "")}

## Security Guidelines
{securityGuidelines}

## Security Best Practices
- **Never store passwords in plain text**
- **Use HTTPS for all authentication endpoints**
- **Implement proper session management**
- **Validate all user inputs**
- **Use secure random number generation**
- **Implement rate limiting for login attempts**";
        }
        catch (Exception ex)
        {
            return $"Error generating security pattern: {ex.Message}";
        }
    }

    [McpServerTool, Description("Creates defensive data security patterns with proper encryption, sanitization, and audit logging to protect against data breaches and unauthorized access.")]
    public static string GenerateDataSecurityPattern(
        [Description("Data type: 'personal', 'financial', 'medical', 'general'")] string dataType,
        [Description("Encryption method: 'aes', 'rsa', 'hybrid'")] string encryptionMethod = "aes",
        [Description("Include data sanitization: 'true' or 'false'")] string includeSanitization = "true",
        [Description("Include audit logging: 'true' or 'false'")] string includeAuditLog = "true")
    {
        try
        {
            var config = new DataSecurityConfiguration
            {
                DataType = dataType.ToLowerInvariant(),
                EncryptionMethod = encryptionMethod.ToLowerInvariant(),
                IncludeSanitization = bool.Parse(includeSanitization),
                IncludeAuditLog = bool.Parse(includeAuditLog)
            };

            string dataService = GenerateSecureDataService(config);
            string sanitizationCode = config.IncludeSanitization ? GenerateDataSanitization() : "";
            string auditCode = config.IncludeAuditLog ? GenerateAuditLogging() : "";
            string complianceInfo = GenerateComplianceInformation(config);

            return $@"# Data Security Pattern: {dataType}

🛡️ **DEFENSIVE SECURITY TOOL**: This generates secure data handling patterns to PROTECT against threats. All examples show proper security implementations that follow industry best practices and compliance requirements.

## Configuration
- **Data Type**: {config.DataType}
- **Encryption Method**: {config.EncryptionMethod}
- **Data Sanitization**: {config.IncludeSanitization}
- **Audit Logging**: {config.IncludeAuditLog}

## Secure Data Service
```csharp
{dataService}
```

{(config.IncludeSanitization ? $@"## Data Sanitization
```csharp
{sanitizationCode}
```" : "")}

{(config.IncludeAuditLog ? $@"## Audit Logging
```csharp
{auditCode}
```" : "")}

## Compliance Information
{complianceInfo}

## Data Protection Principles
- **Data Minimization**: Only collect necessary data
- **Purpose Limitation**: Use data only for stated purposes
- **Storage Limitation**: Keep data only as long as needed
- **Accuracy**: Ensure data is accurate and up-to-date
- **Security**: Implement appropriate technical safeguards";
        }
        catch (Exception ex)
        {
            return $"Error generating data security pattern: {ex.Message}";
        }
    }

    sealed class SecurityConfiguration
    {
        public string AuthType { get; set; } = "";
        public bool IncludeEncryption { get; set; }
        public bool IncludeValidation { get; set; }
        public string SecurityLevel { get; set; } = "";
    }

    sealed class DataSecurityConfiguration
    {
        public string DataType { get; set; } = "";
        public string EncryptionMethod { get; set; } = "";
        public bool IncludeSanitization { get; set; }
        public bool IncludeAuditLog { get; set; }
    }

    static string GenerateAuthenticationService(SecurityConfiguration config)
    {
        return config.AuthType switch
        {
            "jwt" => GenerateJwtAuthentication(config),
            "oauth" => GenerateOAuthAuthentication(config),
            "basic" => GenerateBasicAuthentication(config),
            "multi-factor" => GenerateMultiFactorAuthentication(config),
            _ => GenerateGenericAuthentication(config)
        };
    }

    static string GenerateJwtAuthentication(SecurityConfiguration config)
    {
        return @"// JWT Authentication Service
public interface IJwtAuthenticationService
{
    Task<AuthenticationResult> AuthenticateAsync(string username, string password);
    Task<AuthenticationResult> RefreshTokenAsync(string refreshToken);
    Task<bool> ValidateTokenAsync(string token);
    Task RevokeTokenAsync(string token);
}

public class JwtAuthenticationService : IJwtAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<JwtAuthenticationService> _logger;
    private readonly JwtSettings _jwtSettings;

    public JwtAuthenticationService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<JwtAuthenticationService> logger,
        IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
    }

    public async Task<AuthenticationResult> AuthenticateAsync(string username, string password)
    {
        try
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return AuthenticationResult.Failure(""Username and password are required"");
            }

            // Rate limiting check
            if (!await CheckRateLimitAsync(username))
            {
                _logger.LogWarning(""Rate limit exceeded for user: {Username}"", username);
                return AuthenticationResult.Failure(""Too many login attempts. Please try again later."");
            }

            // Find user
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                _logger.LogWarning(""Authentication failed for non-existent user: {Username}"", username);
                await RecordFailedAttemptAsync(username);
                return AuthenticationResult.Failure(""Invalid credentials"");
            }

            // Verify password
            if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            {
                _logger.LogWarning(""Authentication failed for user: {Username}"", username);
                await RecordFailedAttemptAsync(username);
                return AuthenticationResult.Failure(""Invalid credentials"");
            }

            // Check if account is locked
            if (user.IsLocked)
            {
                _logger.LogWarning(""Authentication attempted for locked account: {Username}"", username);
                return AuthenticationResult.Failure(""Account is locked"");
            }

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Store refresh token
            await _userRepository.StoreRefreshTokenAsync(user.Id, refreshToken);

            // Reset failed attempts
            await ResetFailedAttemptsAsync(username);

            _logger.LogInformation(""User authenticated successfully: {Username}"", username);

            return AuthenticationResult.Success(accessToken, refreshToken, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error during authentication for user: {Username}"", username);
            return AuthenticationResult.Failure(""Authentication failed"");
        }
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return AuthenticationResult.Failure(""Refresh token is required"");
            }

            // Validate refresh token
            var storedToken = await _userRepository.GetRefreshTokenAsync(refreshToken);
            if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
            {
                return AuthenticationResult.Failure(""Invalid or expired refresh token"");
            }

            var user = await _userRepository.GetByIdAsync(storedToken.UserId);
            if (user == null || user.IsLocked)
            {
                return AuthenticationResult.Failure(""User not found or account locked"");
            }

            // Generate new tokens
            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Replace refresh token
            await _userRepository.ReplaceRefreshTokenAsync(refreshToken, newRefreshToken);

            return AuthenticationResult.Success(newAccessToken, newRefreshToken, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error during token refresh"");
            return AuthenticationResult.Failure(""Token refresh failed"");
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            return _tokenService.ValidateToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error validating token"");
            return false;
        }
    }

    public async Task RevokeTokenAsync(string token)
    {
        try
        {
            await _tokenService.RevokeTokenAsync(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error revoking token"");
        }
    }

    private async Task<bool> CheckRateLimitAsync(string username)
    {
        // Implement rate limiting logic
        // Check failed attempts in last X minutes
        var recentAttempts = await _userRepository.GetRecentFailedAttemptsAsync(username, TimeSpan.FromMinutes(15));
        return recentAttempts < 5; // Max 5 attempts per 15 minutes
    }

    private async Task RecordFailedAttemptAsync(string username)
    {
        await _userRepository.RecordFailedAttemptAsync(username, DateTime.UtcNow);
    }

    private async Task ResetFailedAttemptsAsync(string username)
    {
        await _userRepository.ResetFailedAttemptsAsync(username);
    }
}

// Authentication Result
public class AuthenticationResult
{
    public bool IsSuccess { get; private set; }
    public string AccessToken { get; private set; } = string.Empty;
    public string RefreshToken { get; private set; } = string.Empty;
    public User? User { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    public static AuthenticationResult Success(string accessToken, string refreshToken, User user)
    {
        return new AuthenticationResult
        {
            IsSuccess = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = user
        };
    }

    public static AuthenticationResult Failure(string errorMessage)
    {
        return new AuthenticationResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

// JWT Settings
public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 15;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}";
    }

    static string GenerateOAuthAuthentication(SecurityConfiguration config)
    {
        return @"// OAuth 2.0 Authentication Service
public interface IOAuthAuthenticationService
{
    Task<OAuthResult> GetAuthorizationUrlAsync(string clientId, string redirectUri, string[] scopes);
    Task<OAuthResult> ExchangeCodeForTokenAsync(string code, string clientId, string clientSecret, string redirectUri);
    Task<OAuthResult> RefreshTokenAsync(string refreshToken, string clientId, string clientSecret);
    Task<UserInfo> GetUserInfoAsync(string accessToken);
}

public class OAuthAuthenticationService : IOAuthAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OAuthAuthenticationService> _logger;
    private readonly OAuthSettings _settings;

    public OAuthAuthenticationService(
        HttpClient httpClient,
        ILogger<OAuthAuthenticationService> logger,
        IOptions<OAuthSettings> settings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task<OAuthResult> GetAuthorizationUrlAsync(string clientId, string redirectUri, string[] scopes)
    {
        try
        {
            // Generate state parameter for CSRF protection
            var state = GenerateSecureRandomString(32);

            // Generate PKCE code verifier and challenge
            var codeVerifier = GenerateSecureRandomString(128);
            var codeChallenge = GenerateCodeChallenge(codeVerifier);

            var authUrl = $""{_settings.AuthorizationEndpoint}?"" +
                         $""client_id={Uri.EscapeDataString(clientId)}&"" +
                         $""redirect_uri={Uri.EscapeDataString(redirectUri)}&"" +
                         $""response_type=code&"" +
                         $""scope={Uri.EscapeDataString(string.Join("" "", scopes))}&"" +
                         $""state={state}&"" +
                         $""code_challenge={codeChallenge}&"" +
                         $""code_challenge_method=S256"";

            return OAuthResult.Success(authUrl, state, codeVerifier);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error generating authorization URL"");
            return OAuthResult.Failure(""Failed to generate authorization URL"");
        }
    }

    public async Task<OAuthResult> ExchangeCodeForTokenAsync(string code, string clientId, string clientSecret, string redirectUri)
    {
        try
        {
            var tokenRequest = new Dictionary<string, string>
            {
                [""grant_type""] = ""authorization_code"",
                [""code""] = code,
                [""client_id""] = clientId,
                [""client_secret""] = clientSecret,
                [""redirect_uri""] = redirectUri
            };

            var response = await _httpClient.PostAsync(_settings.TokenEndpoint,
                new FormUrlEncodedContent(tokenRequest));

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning(""Token exchange failed: {StatusCode} - {Content}"",
                    response.StatusCode, errorContent);
                return OAuthResult.Failure(""Token exchange failed"");
            }

            var tokenResponse = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<TokenResponse>(tokenResponse);

            return OAuthResult.Success(tokenData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error exchanging code for token"");
            return OAuthResult.Failure(""Token exchange failed"");
        }
    }

    public async Task<UserInfo> GetUserInfoAsync(string accessToken)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(""Bearer"", accessToken);

            var response = await _httpClient.GetAsync(_settings.UserInfoEndpoint);

            if (!response.IsSuccessStatusCode)
            {
                throw new UnauthorizedAccessException(""Invalid access token"");
            }

            var userInfoJson = await response.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<UserInfo>(userInfoJson);

            return userInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error getting user info"");
            throw;
        }
    }

    private string GenerateSecureRandomString(int length)
    {
        const string chars = ""ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~"";
        var random = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(random);
        }

        return new string(random.Select(b => chars[b % chars.Length]).ToArray());
    }

    private string GenerateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
        return Convert.ToBase64String(challengeBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}

public class OAuthSettings
{
    public string AuthorizationEndpoint { get; set; } = string.Empty;
    public string TokenEndpoint { get; set; } = string.Empty;
    public string UserInfoEndpoint { get; set; } = string.Empty;
}";
    }

    static string GenerateBasicAuthentication(SecurityConfiguration config)
    {
        return @"// Basic Authentication Service
public interface IBasicAuthenticationService
{
    Task<AuthenticationResult> AuthenticateAsync(string username, string password);
    Task<bool> ValidateCredentialsAsync(string authHeader);
    Task<User?> GetUserFromAuthHeaderAsync(string authHeader);
}

public class BasicAuthenticationService : IBasicAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<BasicAuthenticationService> _logger;

    public BasicAuthenticationService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ILogger<BasicAuthenticationService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AuthenticationResult> AuthenticateAsync(string username, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return AuthenticationResult.Failure(""Username and password are required"");
            }

            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                // SECURITY BEST PRACTICE: Use consistent timing to prevent username enumeration attacks
                // This prevents attackers from determining if usernames exist by measuring response times
                _passwordHasher.VerifyPassword(""dummy"", ""$2a$12$dummy.hash.to.prevent.timing.attacks"");
                _logger.LogWarning(""Authentication failed for non-existent user: {Username}"", username);
                return AuthenticationResult.Failure(""Invalid credentials"");
            }

            if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            {
                _logger.LogWarning(""Authentication failed for user: {Username}"", username);
                return AuthenticationResult.Failure(""Invalid credentials"");
            }

            if (user.IsLocked)
            {
                _logger.LogWarning(""Authentication attempted for locked account: {Username}"", username);
                return AuthenticationResult.Failure(""Account is locked"");
            }

            _logger.LogInformation(""User authenticated successfully: {Username}"", username);
            return AuthenticationResult.Success(string.Empty, string.Empty, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error during authentication for user: {Username}"", username);
            return AuthenticationResult.Failure(""Authentication failed"");
        }
    }

    public async Task<bool> ValidateCredentialsAsync(string authHeader)
    {
        try
        {
            var credentials = ExtractCredentials(authHeader);
            if (credentials == null) return false;

            var result = await AuthenticateAsync(credentials.Username, credentials.Password);
            return result.IsSuccess;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error validating credentials"");
            return false;
        }
    }

    public async Task<User?> GetUserFromAuthHeaderAsync(string authHeader)
    {
        try
        {
            var credentials = ExtractCredentials(authHeader);
            if (credentials == null) return null;

            var result = await AuthenticateAsync(credentials.Username, credentials.Password);
            return result.IsSuccess ? result.User : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error getting user from auth header"");
            return null;
        }
    }

    private BasicCredentials? ExtractCredentials(string authHeader)
    {
        if (string.IsNullOrWhiteSpace(authHeader))
            return null;

        if (!authHeader.StartsWith(""Basic "", StringComparison.OrdinalIgnoreCase))
            return null;

        try
        {
            var encodedCredentials = authHeader.Substring(6); // Remove ""Basic ""
            var decodedBytes = Convert.FromBase64String(encodedCredentials);
            var decodedCredentials = Encoding.UTF8.GetString(decodedBytes);

            var colonIndex = decodedCredentials.IndexOf(':');
            if (colonIndex <= 0 || colonIndex == decodedCredentials.Length - 1)
                return null;

            return new BasicCredentials
            {
                Username = decodedCredentials.Substring(0, colonIndex),
                Password = decodedCredentials.Substring(colonIndex + 1)
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, ""Invalid Basic Auth header format"");
            return null;
        }
    }

    private class BasicCredentials
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}";
    }

    static string GenerateMultiFactorAuthentication(SecurityConfiguration config)
    {
        return @"// Multi-Factor Authentication Service
public interface IMultiFactorAuthenticationService
{
    Task<MfaSetupResult> SetupMfaAsync(string userId, MfaMethod method);
    Task<MfaVerificationResult> VerifyMfaAsync(string userId, string code, MfaMethod method);
    Task<bool> IsMfaEnabledAsync(string userId);
    Task<bool> DisableMfaAsync(string userId, string verificationCode);
    Task<string[]> GenerateBackupCodesAsync(string userId);
}

public class MultiFactorAuthenticationService : IMultiFactorAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ITotpService _totpService;
    private readonly ISmsService _smsService;
    private readonly IEmailService _emailService;
    private readonly ILogger<MultiFactorAuthenticationService> _logger;

    public MultiFactorAuthenticationService(
        IUserRepository userRepository,
        ITotpService totpService,
        ISmsService smsService,
        IEmailService emailService,
        ILogger<MultiFactorAuthenticationService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _totpService = totpService ?? throw new ArgumentNullException(nameof(totpService));
        _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<MfaSetupResult> SetupMfaAsync(string userId, MfaMethod method)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return MfaSetupResult.Failure(""User not found"");
            }

            return method switch
            {
                MfaMethod.Totp => await SetupTotpAsync(user),
                MfaMethod.Sms => await SetupSmsAsync(user),
                MfaMethod.Email => await SetupEmailAsync(user),
                _ => MfaSetupResult.Failure(""Unsupported MFA method"")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error setting up MFA for user: {UserId}"", userId);
            return MfaSetupResult.Failure(""MFA setup failed"");
        }
    }

    public async Task<MfaVerificationResult> VerifyMfaAsync(string userId, string code, MfaMethod method)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return MfaVerificationResult.Failure(""User not found"");
            }

            // Check for backup codes first
            if (await IsBackupCodeAsync(userId, code))
            {
                await ConsumeBackupCodeAsync(userId, code);
                return MfaVerificationResult.Success();
            }

            return method switch
            {
                MfaMethod.Totp => await VerifyTotpAsync(user, code),
                MfaMethod.Sms => await VerifySmsAsync(user, code),
                MfaMethod.Email => await VerifyEmailAsync(user, code),
                _ => MfaVerificationResult.Failure(""Unsupported MFA method"")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error verifying MFA for user: {UserId}"", userId);
            return MfaVerificationResult.Failure(""MFA verification failed"");
        }
    }

    private async Task<MfaSetupResult> SetupTotpAsync(User user)
    {
        var secret = _totpService.GenerateSecret();
        var qrCodeUrl = _totpService.GenerateQrCodeUrl(user.Email, secret, ""Your App Name"");

        // Store secret temporarily - it will be confirmed when user verifies
        await _userRepository.StoreTempMfaSecretAsync(user.Id, secret);

        return MfaSetupResult.Success(secret, qrCodeUrl);
    }

    private async Task<MfaVerificationResult> VerifyTotpAsync(User user, string code)
    {
        var secret = await _userRepository.GetMfaSecretAsync(user.Id);
        if (string.IsNullOrEmpty(secret))
        {
            return MfaVerificationResult.Failure(""TOTP not configured"");
        }

        var isValid = _totpService.VerifyCode(secret, code);
        return isValid
            ? MfaVerificationResult.Success()
            : MfaVerificationResult.Failure(""Invalid TOTP code"");
    }

    public async Task<string[]> GenerateBackupCodesAsync(string userId)
    {
        var backupCodes = new string[8];
        for (int i = 0; i < backupCodes.Length; i++)
        {
            backupCodes[i] = GenerateBackupCode();
        }

        await _userRepository.StoreBackupCodesAsync(userId, backupCodes);
        return backupCodes;
    }

    private string GenerateBackupCode()
    {
        const string chars = ""ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"";
        var random = new byte[8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(random);
        }

        return new string(random.Select(b => chars[b % chars.Length]).ToArray());
    }

    private async Task<bool> IsBackupCodeAsync(string userId, string code)
    {
        var backupCodes = await _userRepository.GetBackupCodesAsync(userId);
        return backupCodes.Contains(code, StringComparer.OrdinalIgnoreCase);
    }

    private async Task ConsumeBackupCodeAsync(string userId, string code)
    {
        await _userRepository.ConsumeBackupCodeAsync(userId, code);
    }
}

public enum MfaMethod
{
    Totp,
    Sms,
    Email
}

public class MfaSetupResult
{
    public bool IsSuccess { get; private set; }
    public string Secret { get; private set; } = string.Empty;
    public string QrCodeUrl { get; private set; } = string.Empty;
    public string ErrorMessage { get; private set; } = string.Empty;

    public static MfaSetupResult Success(string secret, string qrCodeUrl)
    {
        return new MfaSetupResult
        {
            IsSuccess = true,
            Secret = secret,
            QrCodeUrl = qrCodeUrl
        };
    }

    public static MfaSetupResult Failure(string errorMessage)
    {
        return new MfaSetupResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

public class MfaVerificationResult
{
    public bool IsSuccess { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    public static MfaVerificationResult Success()
    {
        return new MfaVerificationResult { IsSuccess = true };
    }

    public static MfaVerificationResult Failure(string errorMessage)
    {
        return new MfaVerificationResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}";
    }

    static string GenerateGenericAuthentication(SecurityConfiguration config)
    {
        return @"// Generic Authentication Service
public interface IGenericAuthenticationService
{
    Task<AuthenticationResult> AuthenticateAsync(AuthenticationRequest request);
    Task<bool> ValidateCredentialsAsync(object credentials);
}

public class GenericAuthenticationService : IGenericAuthenticationService
{
    private readonly ILogger<GenericAuthenticationService> _logger;

    public GenericAuthenticationService(ILogger<GenericAuthenticationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AuthenticationResult> AuthenticateAsync(AuthenticationRequest request)
    {
        try
        {
            // Input validation
            if (request == null)
                return AuthenticationResult.Failure(""Authentication request cannot be null"");

            if (string.IsNullOrWhiteSpace(request.Username))
                return AuthenticationResult.Failure(""Username is required"");

            if (string.IsNullOrWhiteSpace(request.Password))
                return AuthenticationResult.Failure(""Password is required"");

            // Example implementation - customize for your needs
            _logger.LogInformation(""Authenticating user: {Username}"", request.Username);

            // Simulate authentication logic
            await Task.Delay(100); // Simulate database/service call

            // Example validation logic (customize for your authentication system)
            if (request.Username.Equals(""admin"", StringComparison.OrdinalIgnoreCase) &&
                request.Password == ""SecurePassword123!"")
            {
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = request.Username,
                    Email = $""{request.Username}@example.com""
                };

                return AuthenticationResult.Success(""example-access-token"", ""example-refresh-token"", user);
            }

            _logger.LogWarning(""Authentication failed for user: {Username}"", request.Username);
            return AuthenticationResult.Failure(""Invalid credentials"");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error during authentication for user: {Username}"", request?.Username);
            return AuthenticationResult.Failure(""Authentication failed due to system error"");
        }
    }

    public async Task<bool> ValidateCredentialsAsync(object credentials)
    {
        try
        {
            // Example credential validation logic
            if (credentials == null) return false;

            // Handle basic username/password validation
            if (credentials is Dictionary<string, object> dict)
            {
                if (!dict.ContainsKey(""username"") || !dict.ContainsKey(""password""))
                    return false;

                var username = dict[""username""]?.ToString();
                var password = dict[""password""]?.ToString();

                return !string.IsNullOrWhiteSpace(username) &&
                       !string.IsNullOrWhiteSpace(password) &&
                       username.Length >= 3 && password.Length >= 8;
            }

            // Handle authentication request objects
            if (credentials is AuthenticationRequest request)
            {
                return !string.IsNullOrWhiteSpace(request.Username) &&
                       !string.IsNullOrWhiteSpace(request.Password) &&
                       request.Username.Length >= 3 && request.Password.Length >= 8;
            }

            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

public class AuthenticationRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}";
    }

    static string GenerateEncryptionService()
    {
        return @"// Encryption Service
public interface IEncryptionService
{
    string EncryptString(string plainText, string key);
    string DecryptString(string cipherText, string key);
    byte[] EncryptBytes(byte[] plainBytes, string key);
    byte[] DecryptBytes(byte[] cipherBytes, string key);
    string GenerateKey();
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public class EncryptionService : IEncryptionService
{
    private readonly ILogger<EncryptionService> _logger;

    public EncryptionService(ILogger<EncryptionService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string EncryptString(string plainText, string key)
    {
        try
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException(""Plain text cannot be null or empty"", nameof(plainText));

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException(""Key cannot be null or empty"", nameof(key));

            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = EncryptBytes(plainBytes, key);
            return Convert.ToBase64String(encryptedBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error encrypting string"");
            throw;
        }
    }

    public string DecryptString(string cipherText, string key)
    {
        try
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException(""Cipher text cannot be null or empty"", nameof(cipherText));

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException(""Key cannot be null or empty"", nameof(key));

            var cipherBytes = Convert.FromBase64String(cipherText);
            var decryptedBytes = DecryptBytes(cipherBytes, key);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error decrypting string"");
            throw;
        }
    }

    public byte[] EncryptBytes(byte[] plainBytes, string key)
    {
        try
        {
            using var aes = Aes.Create();
            aes.Key = DeriveKeyFromPassword(key, aes.KeySize / 8);
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            using var msEncrypt = new MemoryStream();

            // Prepend IV to encrypted data
            msEncrypt.Write(aes.IV, 0, aes.IV.Length);

            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                csEncrypt.Write(plainBytes, 0, plainBytes.Length);
            }

            return msEncrypt.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error encrypting bytes"");
            throw;
        }
    }

    public byte[] DecryptBytes(byte[] cipherBytes, string key)
    {
        try
        {
            using var aes = Aes.Create();
            aes.Key = DeriveKeyFromPassword(key, aes.KeySize / 8);

            // Extract IV from the beginning of cipher bytes
            var iv = new byte[aes.BlockSize / 8];
            var encryptedData = new byte[cipherBytes.Length - iv.Length];

            Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
            Array.Copy(cipherBytes, iv.Length, encryptedData, 0, encryptedData.Length);

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var msDecrypt = new MemoryStream(encryptedData);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var msPlain = new MemoryStream();

            csDecrypt.CopyTo(msPlain);
            return msPlain.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error decrypting bytes"");
            throw;
        }
    }

    public string GenerateKey()
    {
        var keyBytes = new byte[32]; // 256-bit key
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(keyBytes);
        }
        return Convert.ToBase64String(keyBytes);
    }

    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException(""Password cannot be null or empty"", nameof(password));

        // Using BCrypt for password hashing
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error verifying password"");
            return false;
        }
    }

    private byte[] DeriveKeyFromPassword(string password, int keyLength)
    {
        const int iterations = 10000; // PBKDF2 iterations
        var salt = Encoding.UTF8.GetBytes(""YourAppSalt""); // In production, use a random salt per encryption

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(keyLength);
    }
}";
    }

    static string GenerateInputValidation()
    {
        return @"// Input Validation Service
public interface IInputValidationService
{
    ValidationResult ValidateEmail(string email);
    ValidationResult ValidatePassword(string password);
    ValidationResult ValidatePhoneNumber(string phoneNumber);
    ValidationResult ValidateUrl(string url);
    string SanitizeInput(string input);
    bool IsValidSqlInput(string input);
}

public class InputValidationService : IInputValidationService
{
    private static readonly Regex EmailRegex = new(
        @""^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PhoneRegex = new(
        @""^[\+]?[1-9][\d]{0,15}$"",
        RegexOptions.Compiled);

    private static readonly Regex UrlRegex = new(
        @""^https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)$"",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly string[] SqlKeywords = {
        ""SELECT"", ""INSERT"", ""UPDATE"", ""DELETE"", ""DROP"", ""CREATE"", ""ALTER"",
        ""EXEC"", ""EXECUTE"", ""UNION"", ""SCRIPT"", ""DECLARE"", ""CAST"", ""CONVERT""
    };

    public ValidationResult ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return ValidationResult.Failure(""Email is required"");
        }

        if (email.Length > 254)
        {
            return ValidationResult.Failure(""Email is too long"");
        }

        if (!EmailRegex.IsMatch(email))
        {
            return ValidationResult.Failure(""Invalid email format"");
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return ValidationResult.Failure(""Password is required"");
        }

        if (password.Length < 8)
        {
            return ValidationResult.Failure(""Password must be at least 8 characters long"");
        }

        if (password.Length > 128)
        {
            return ValidationResult.Failure(""Password is too long"");
        }

        var hasUpper = password.Any(char.IsUpper);
        var hasLower = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

        if (!hasUpper)
        {
            return ValidationResult.Failure(""Password must contain at least one uppercase letter"");
        }

        if (!hasLower)
        {
            return ValidationResult.Failure(""Password must contain at least one lowercase letter"");
        }

        if (!hasDigit)
        {
            return ValidationResult.Failure(""Password must contain at least one digit"");
        }

        if (!hasSpecial)
        {
            return ValidationResult.Failure(""Password must contain at least one special character"");
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return ValidationResult.Failure(""Phone number is required"");
        }

        // Remove common formatting characters
        var cleanPhone = phoneNumber.Replace("" "", """").Replace(""-"", """").Replace(""("", """").Replace("")"", """");

        if (!PhoneRegex.IsMatch(cleanPhone))
        {
            return ValidationResult.Failure(""Invalid phone number format"");
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidateUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return ValidationResult.Failure(""URL is required"");
        }

        if (!UrlRegex.IsMatch(url))
        {
            return ValidationResult.Failure(""Invalid URL format"");
        }

        // Additional security checks
        if (url.Contains(""javascript:"") || url.Contains(""data:""))
        {
            return ValidationResult.Failure(""URL contains potentially dangerous scheme"");
        }

        return ValidationResult.Success();
    }

    public string SanitizeInput(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // Remove or encode potentially dangerous characters
        return input
            .Replace(""<"", ""&lt;"")
            .Replace("">"", ""&gt;"")
            .Replace(""\"""", ""&quot;"")
            .Replace(""'"", ""&#x27;"")
            .Replace(""/"", ""&#x2F;"")
            .Replace(""\\"", ""&#x5C;"")
            .Trim();
    }

    public bool IsValidSqlInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return true;

        var upperInput = input.ToUpperInvariant();

        // Check for SQL injection patterns
        if (SqlKeywords.Any(keyword => upperInput.Contains(keyword)))
        {
            return false;
        }

        // Check for common SQL injection characters
        if (input.Contains(""--"") || input.Contains(""/*"") || input.Contains(""*/"") ||
            input.Contains(""xp_"") || input.Contains(""sp_""))
        {
            return false;
        }

        return true;
    }
}

public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    public static ValidationResult Success()
    {
        return new ValidationResult { IsValid = true };
    }

    public static ValidationResult Failure(string errorMessage)
    {
        return new ValidationResult
        {
            IsValid = false,
            ErrorMessage = errorMessage
        };
    }
}";
    }

    static string GenerateSecurityGuidelines(SecurityConfiguration config)
    {
        return $@"### Security Implementation Guidelines

#### Authentication Security
- **Password Storage**: Never store passwords in plain text - use bcrypt, scrypt, or Argon2
- **Session Management**: Use secure session tokens with proper expiration
- **Rate Limiting**: Implement rate limiting for login attempts (max 5 attempts per 15 minutes)
- **Account Lockout**: Lock accounts after repeated failed attempts
- **Two-Factor Authentication**: Implement 2FA for enhanced security

#### Data Protection
- **Encryption at Rest**: Encrypt sensitive data in the database
- **Encryption in Transit**: Use HTTPS/TLS for all communications
- **Key Management**: Store encryption keys securely (Azure Key Vault, AWS KMS)
- **Data Minimization**: Only collect and store necessary data
- **Regular Backups**: Implement secure backup procedures

#### Input Validation
- **Server-Side Validation**: Always validate on the server, never trust client input
- **SQL Injection Prevention**: Use parameterized queries or ORMs
- **XSS Prevention**: Sanitize and encode user input
- **CSRF Protection**: Implement CSRF tokens for state-changing operations
- **File Upload Security**: Validate file types, sizes, and scan for malware

#### Security Level: {config.SecurityLevel.ToUpperInvariant()}
{GetSecurityLevelRequirements(config.SecurityLevel)}

#### Compliance Considerations
- **GDPR**: Implement data protection and user rights
- **CCPA**: Provide data transparency and user control
- **SOX**: Maintain audit trails and access controls
- **HIPAA**: Protect health information with appropriate safeguards
- **PCI DSS**: Secure payment card data processing";
    }

    static string GetSecurityLevelRequirements(string level)
    {
        return level switch
        {
            "basic" => @"- Password complexity requirements
- HTTPS enforcement
- Basic input validation
- Session timeouts",

            "standard" => @"- Strong password policies
- Multi-factor authentication recommended
- Comprehensive input validation
- Rate limiting and account lockout
- Security headers implementation
- Regular security updates",

            "high" => @"- Mandatory multi-factor authentication
- Advanced threat detection
- Zero-trust architecture
- Regular penetration testing
- Security monitoring and alerting
- Incident response procedures
- Data loss prevention (DLP)
- Advanced encryption standards",

            _ => "- Standard security practices apply"
        };
    }

    static string GenerateSecureDataService(DataSecurityConfiguration config)
    {
        return @"// Secure Data Service
public interface ISecureDataService<T> where T : class
{
    Task<T> GetAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<string> CreateAsync(T entity);
    Task UpdateAsync(string id, T entity);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
}

public class SecureDataService<T> : ISecureDataService<T> where T : class, ISecureEntity
{
    private readonly IRepository<T> _repository;
    private readonly IEncryptionService _encryptionService;
    private readonly IDataSanitizationService _sanitizationService;
    private readonly IAuditLogger _auditLogger;
    private readonly ILogger<SecureDataService<T>> _logger;
    private readonly string _encryptionKey;

    public SecureDataService(
        IRepository<T> repository,
        IEncryptionService encryptionService,
        IDataSanitizationService sanitizationService,
        IAuditLogger auditLogger,
        ILogger<SecureDataService<T>> logger,
        IConfiguration configuration)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        _sanitizationService = sanitizationService ?? throw new ArgumentNullException(nameof(sanitizationService));
        _auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _encryptionKey = configuration.GetConnectionString(""EncryptionKey"") ??
            throw new InvalidOperationException(""Encryption key not configured"");
    }

    public async Task<T> GetAsync(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(""ID cannot be null or empty"", nameof(id));

            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return null;

            // Decrypt sensitive fields
            DecryptEntity(entity);

            await _auditLogger.LogDataAccessAsync(typeof(T).Name, id, ""Read"");
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error retrieving entity with ID: {Id}"", id);
            throw;
        }
    }

    public async Task<string> CreateAsync(T entity)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Sanitize input data
            SanitizeEntity(entity);

            // Encrypt sensitive fields
            EncryptEntity(entity);

            // Set audit fields
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = GetCurrentUserId();

            var id = await _repository.CreateAsync(entity);

            await _auditLogger.LogDataAccessAsync(typeof(T).Name, id, ""Create"");
            _logger.LogInformation(""Created new entity of type {Type} with ID: {Id}"", typeof(T).Name, id);

            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error creating entity of type {Type}"", typeof(T).Name);
            throw;
        }
    }

    public async Task UpdateAsync(string id, T entity)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(""ID cannot be null or empty"", nameof(id));

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity == null)
                throw new NotFoundException($""Entity with ID {id} not found"");

            // Sanitize input data
            SanitizeEntity(entity);

            // Encrypt sensitive fields
            EncryptEntity(entity);

            // Set audit fields
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = GetCurrentUserId();

            await _repository.UpdateAsync(id, entity);

            await _auditLogger.LogDataAccessAsync(typeof(T).Name, id, ""Update"");
            _logger.LogInformation(""Updated entity of type {Type} with ID: {Id}"", typeof(T).Name, id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error updating entity with ID: {Id}"", id);
            throw;
        }
    }

    public async Task DeleteAsync(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(""ID cannot be null or empty"", nameof(id));

            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new NotFoundException($""Entity with ID {id} not found"");

            await _repository.DeleteAsync(id);

            await _auditLogger.LogDataAccessAsync(typeof(T).Name, id, ""Delete"");
            _logger.LogInformation(""Deleted entity of type {Type} with ID: {Id}"", typeof(T).Name, id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error deleting entity with ID: {Id}"", id);
            throw;
        }
    }

    private void EncryptEntity(T entity)
    {
        var encryptableProperties = typeof(T).GetProperties()
            .Where(p => p.GetCustomAttribute<EncryptAttribute>() != null)
            .Where(p => p.PropertyType == typeof(string) && p.CanWrite);

        foreach (var property in encryptableProperties)
        {
            var value = property.GetValue(entity) as string;
            if (!string.IsNullOrEmpty(value))
            {
                var encryptedValue = _encryptionService.EncryptString(value, _encryptionKey);
                property.SetValue(entity, encryptedValue);
            }
        }
    }

    private void DecryptEntity(T entity)
    {
        var encryptableProperties = typeof(T).GetProperties()
            .Where(p => p.GetCustomAttribute<EncryptAttribute>() != null)
            .Where(p => p.PropertyType == typeof(string) && p.CanWrite);

        foreach (var property in encryptableProperties)
        {
            var encryptedValue = property.GetValue(entity) as string;
            if (!string.IsNullOrEmpty(encryptedValue))
            {
                try
                {
                    var decryptedValue = _encryptionService.DecryptString(encryptedValue, _encryptionKey);
                    property.SetValue(entity, decryptedValue);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ""Error decrypting property {Property}"", property.Name);
                    // Set to empty string or handle as needed
                    property.SetValue(entity, string.Empty);
                }
            }
        }
    }

    private void SanitizeEntity(T entity)
    {
        var sanitizableProperties = typeof(T).GetProperties()
            .Where(p => p.GetCustomAttribute<SanitizeAttribute>() != null)
            .Where(p => p.PropertyType == typeof(string) && p.CanWrite);

        foreach (var property in sanitizableProperties)
        {
            var value = property.GetValue(entity) as string;
            if (!string.IsNullOrEmpty(value))
            {
                var sanitizedValue = _sanitizationService.SanitizeInput(value);
                property.SetValue(entity, sanitizedValue);
            }
        }
    }

    private string GetCurrentUserId()
    {
        // Get current user ID from authentication context
        // This is a placeholder - implement based on your authentication system
        return ""system"";
    }
}

// Security Attributes
[AttributeUsage(AttributeTargets.Property)]
public class EncryptAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property)]
public class SanitizeAttribute : Attribute { }

// Secure Entity Interface
public interface ISecureEntity
{
    string Id { get; set; }
    DateTime CreatedAt { get; set; }
    string CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
}";
    }

    static string GenerateDataSanitization()
    {
        return @"// Data Sanitization Service
public interface IDataSanitizationService
{
    string SanitizeHtml(string input);
    string SanitizeInput(string input);
    string SanitizeSqlInput(string input);
    string SanitizeFileName(string fileName);
    string SanitizeUrl(string url);
    byte[] SanitizeFile(byte[] fileContent, string fileExtension);
}

public class DataSanitizationService : IDataSanitizationService
{
    private readonly ILogger<DataSanitizationService> _logger;
    private static readonly string[] DangerousExtensions =
    {
        "".exe"", "".bat"", "".cmd"", "".com"", "".pif"", "".scr"", "".vbs"", "".js"", "".jar"", "".dll""
    };

    public DataSanitizationService(ILogger<DataSanitizationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string SanitizeHtml(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // Remove script tags and their content
        input = Regex.Replace(input, @""<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>"",
            string.Empty, RegexOptions.IgnoreCase);

        // Remove dangerous HTML attributes
        input = Regex.Replace(input, @""(on\w+)=""[^""]*"""",
            string.Empty, RegexOptions.IgnoreCase);

        // Remove javascript: and vbscript: protocols
        input = Regex.Replace(input, @""(javascript|vbscript):[^""']*"",
            string.Empty, RegexOptions.IgnoreCase);

        // Encode remaining HTML entities
        input = HttpUtility.HtmlEncode(input);

        return input;
    }

    public string SanitizeInput(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // Remove null characters
        input = input.Replace(""\0"", string.Empty);

        // Remove control characters except tab, newline, and carriage return
        input = new string(input.Where(c =>
            !char.IsControl(c) || c == '\t' || c == '\n' || c == '\r').ToArray());

        // Trim whitespace
        input = input.Trim();

        // Limit length to prevent buffer overflow attacks
        if (input.Length > 10000)
        {
            input = input.Substring(0, 10000);
            _logger.LogWarning(""Input truncated due to excessive length"");
        }

        return input;
    }

    public string SanitizeSqlInput(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // Remove SQL comment sequences
        input = input.Replace(""--"", string.Empty);
        input = input.Replace(""/*"", string.Empty);
        input = input.Replace(""*/"", string.Empty);

        // Remove common SQL injection patterns
        var sqlPatterns = new[]
        {
            @""\bUNION\b"", @""\bSELECT\b"", @""\bINSERT\b"", @""\bUPDATE\b"",
            @""\bDELETE\b"", @""\bDROP\b"", @""\bEXEC\b"", @""\bEXECUTE\b""
        };

        foreach (var pattern in sqlPatterns)
        {
            input = Regex.Replace(input, pattern, string.Empty, RegexOptions.IgnoreCase);
        }

        // Escape single quotes
        input = input.Replace(""'"", ""''"");

        return input;
    }

    public string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return ""sanitized_file"";

        // Remove path characters
        var invalidChars = Path.GetInvalidFileNameChars();
        fileName = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());

        // Remove dangerous patterns
        fileName = fileName.Replace("".."", string.Empty);
        fileName = fileName.Replace(""~"", string.Empty);

        // Check for dangerous extensions
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (DangerousExtensions.Contains(extension))
        {
            fileName = Path.ChangeExtension(fileName, "".txt"");
            _logger.LogWarning(""Dangerous file extension changed: {Extension}"", extension);
        }

        // Ensure filename is not empty
        if (string.IsNullOrWhiteSpace(fileName))
            fileName = ""sanitized_file.txt"";

        // Limit length
        if (fileName.Length > 255)
            fileName = fileName.Substring(0, 255);

        return fileName;
    }

    public string SanitizeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        // Remove dangerous protocols
        var dangerousProtocols = new[] { ""javascript:"", ""data:"", ""vbscript:"", ""file:"" };
        foreach (var protocol in dangerousProtocols)
        {
            if (url.StartsWith(protocol, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(""Dangerous URL protocol removed: {Protocol}"", protocol);
                return string.Empty;
            }
        }

        // Ensure URL starts with http or https
        if (!url.StartsWith(""http://"", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith(""https://"", StringComparison.OrdinalIgnoreCase))
        {
            url = ""https://"" + url;
        }

        // Validate URL format
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            _logger.LogWarning(""Invalid URL format: {Url}"", url);
            return string.Empty;
        }

        return uri.ToString();
    }

    public byte[] SanitizeFile(byte[] fileContent, string fileExtension)
    {
        if (fileContent == null || fileContent.Length == 0)
            return Array.Empty<byte>();

        var extension = fileExtension.ToLowerInvariant();

        // Check for dangerous file types
        if (DangerousExtensions.Contains(extension))
        {
            _logger.LogWarning(""Dangerous file type blocked: {Extension}"", extension);
            throw new SecurityException($""File type '{extension}' is not allowed"");
        }

        // Check file size (example: 10MB limit)
        if (fileContent.Length > 10 * 1024 * 1024)
        {
            _logger.LogWarning(""File size exceeds limit: {Size} bytes"", fileContent.Length);
            throw new SecurityException(""File size exceeds maximum allowed limit"");
        }

        // Scan for embedded malicious content (simplified example)
        if (ContainsSuspiciousContent(fileContent))
        {
            _logger.LogWarning(""Suspicious content detected in file"");
            throw new SecurityException(""File contains suspicious content"");
        }

        return fileContent;
    }

    private bool ContainsSuspiciousContent(byte[] content)
    {
        // Simple check for suspicious patterns in binary content
        // In production, integrate with a proper malware scanner

        var contentString = Encoding.UTF8.GetString(content, 0, Math.Min(content.Length, 1024));

        var suspiciousPatterns = new[]
        {
            ""eval("", ""<script"", ""javascript:"", ""onload="", ""onerror=""
        };

        return suspiciousPatterns.Any(pattern =>
            contentString.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }
}";
    }

    static string GenerateAuditLogging()
    {
        return @"// Audit Logging Service
public interface IAuditLogger
{
    Task LogDataAccessAsync(string entityType, string entityId, string operation);
    Task LogSecurityEventAsync(string eventType, string description, string userId = null);
    Task LogLoginAttemptAsync(string username, bool success, string ipAddress);
    Task LogPermissionChangeAsync(string userId, string permission, string changedBy);
    Task<IEnumerable<AuditLog>> GetAuditLogsAsync(DateTime fromDate, DateTime toDate);
}

public class AuditLogger : IAuditLogger
{
    private readonly IAuditRepository _auditRepository;
    private readonly ILogger<AuditLogger> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogger(
        IAuditRepository auditRepository,
        ILogger<AuditLogger> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _auditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogDataAccessAsync(string entityType, string entityId, string operation)
    {
        try
        {
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid().ToString(),
                EventType = ""DataAccess"",
                EntityType = entityType,
                EntityId = entityId,
                Operation = operation,
                UserId = GetCurrentUserId(),
                UserName = GetCurrentUserName(),
                IpAddress = GetClientIpAddress(),
                UserAgent = GetUserAgent(),
                Timestamp = DateTime.UtcNow,
                Details = $""{operation} operation on {entityType} with ID {entityId}""
            };

            await _auditRepository.CreateAsync(auditLog);
            _logger.LogInformation(""Data access logged: {Operation} on {EntityType} {EntityId}"",
                operation, entityType, entityId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error logging data access audit"");
            // Don't throw - audit logging failure shouldn't break the main operation
        }
    }

    public async Task LogSecurityEventAsync(string eventType, string description, string userId = null)
    {
        try
        {
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid().ToString(),
                EventType = ""Security"",
                SubEventType = eventType,
                UserId = userId ?? GetCurrentUserId(),
                UserName = GetCurrentUserName(),
                IpAddress = GetClientIpAddress(),
                UserAgent = GetUserAgent(),
                Timestamp = DateTime.UtcNow,
                Details = description,
                Severity = GetSecurityEventSeverity(eventType)
            };

            await _auditRepository.CreateAsync(auditLog);
            _logger.LogWarning(""Security event logged: {EventType} - {Description}"", eventType, description);

            // Alert on high-severity security events
            if (auditLog.Severity == ""High"")
            {
                await SendSecurityAlert(auditLog);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error logging security audit"");
        }
    }

    public async Task LogLoginAttemptAsync(string username, bool success, string ipAddress)
    {
        try
        {
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid().ToString(),
                EventType = ""Authentication"",
                SubEventType = success ? ""LoginSuccess"" : ""LoginFailure"",
                UserName = username,
                IpAddress = ipAddress,
                UserAgent = GetUserAgent(),
                Timestamp = DateTime.UtcNow,
                Details = $""{(success ? ""Successful"" : ""Failed"")} login attempt for user {username}"",
                Severity = success ? ""Low"" : ""Medium""
            };

            await _auditRepository.CreateAsync(auditLog);

            if (!success)
            {
                _logger.LogWarning(""Failed login attempt for user: {Username} from IP: {IpAddress}"",
                    username, ipAddress);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error logging login attempt audit"");
        }
    }

    public async Task LogPermissionChangeAsync(string userId, string permission, string changedBy)
    {
        try
        {
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid().ToString(),
                EventType = ""Authorization"",
                SubEventType = ""PermissionChange"",
                UserId = userId,
                UserName = GetUserNameById(userId),
                IpAddress = GetClientIpAddress(),
                UserAgent = GetUserAgent(),
                Timestamp = DateTime.UtcNow,
                Details = $""Permission '{permission}' changed for user {userId} by {changedBy}"",
                Severity = ""High"",
                ChangedBy = changedBy
            };

            await _auditRepository.CreateAsync(auditLog);
            _logger.LogWarning(""Permission change logged: {Permission} for user {UserId} by {ChangedBy}"",
                permission, userId, changedBy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error logging permission change audit"");
        }
    }

    public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            return await _auditRepository.GetByDateRangeAsync(fromDate, toDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error retrieving audit logs"");
            throw;
        }
    }

    private string GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(""sub"")?.Value ?? ""Anonymous"";
    }

    private string GetCurrentUserName()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(""name"")?.Value ?? ""Anonymous"";
    }

    private string GetClientIpAddress()
    {
        return _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? ""Unknown"";
    }

    private string GetUserAgent()
    {
        return _httpContextAccessor.HttpContext?.Request?.Headers[""User-Agent""].ToString() ?? ""Unknown"";
    }

    private string GetUserNameById(string userId)
    {
        // Example user lookup - in production, query your user repository
        if (string.IsNullOrWhiteSpace(userId))
            return ""Unknown User"";

        // Mock implementation for demonstration
        return userId.StartsWith(""admin"") ? ""Administrator"" :
               userId.StartsWith(""user"") ? $""User {userId}"" :
               ""System User"";
    }

    private string GetSecurityEventSeverity(string eventType)
    {
        return eventType switch
        {
            ""LoginFailure"" => ""Medium"",
            ""AccountLockout"" => ""High"",
            ""PermissionEscalation"" => ""High"",
            ""SuspiciousActivity"" => ""High"",
            ""DataBreach"" => ""Critical"",
            _ => ""Low""
        };
    }

    private async Task SendSecurityAlert(AuditLog auditLog)
    {
        // Implement security alerting (email, SMS, etc.)
        _logger.LogCritical(""High-severity security event: {EventType} - {Details}"",
            auditLog.SubEventType, auditLog.Details);
    }
}

public class AuditLog
{
    public string Id { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string SubEventType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string ChangedBy { get; set; } = string.Empty;
}";
    }

    static string GenerateComplianceInformation(DataSecurityConfiguration config)
    {
        return config.DataType switch
        {
            "personal" => @"### GDPR Compliance Requirements
- **Lawful Basis**: Establish lawful basis for processing personal data
- **Data Subject Rights**: Implement rights to access, rectify, erase, and port data
- **Privacy by Design**: Implement data protection by design and default
- **Data Protection Impact Assessment**: Conduct DPIA for high-risk processing
- **Breach Notification**: Report breaches within 72 hours to authorities

### Data Handling Requirements
- Encrypt personal data at rest and in transit
- Implement pseudonymization where possible
- Maintain detailed processing records
- Ensure data accuracy and currency
- Limit data retention periods",

            "financial" => @"### PCI DSS Compliance Requirements
- **Secure Network**: Build and maintain secure network and systems
- **Cardholder Data Protection**: Protect stored cardholder data
- **Vulnerability Management**: Maintain vulnerability management program
- **Access Control**: Implement strong access control measures
- **Network Monitoring**: Regularly monitor and test networks

### SOX Compliance Requirements
- **Internal Controls**: Implement internal controls over financial reporting
- **Audit Trails**: Maintain comprehensive audit trails
- **Data Integrity**: Ensure accuracy and completeness of financial data
- **Access Management**: Restrict access to financial systems",

            "medical" => @"### HIPAA Compliance Requirements
- **Administrative Safeguards**: Implement policies and procedures
- **Physical Safeguards**: Protect physical access to systems
- **Technical Safeguards**: Control access to electronic PHI
- **Business Associate Agreements**: Ensure third-party compliance
- **Breach Notification**: Report breaches according to HIPAA rules

### Data Handling Requirements
- Encrypt PHI at rest and in transit
- Implement minimum necessary access
- Maintain audit logs for all PHI access
- Conduct regular risk assessments
- Train workforce on HIPAA requirements",

            _ => @"### General Data Protection Requirements
- **Data Classification**: Classify data based on sensitivity
- **Access Control**: Implement role-based access control
- **Encryption**: Encrypt sensitive data at rest and in transit
- **Audit Logging**: Log all data access and modifications
- **Backup and Recovery**: Implement secure backup procedures

### Best Practices
- Regular security assessments
- Employee training and awareness
- Incident response procedures
- Vendor risk management
- Data retention policies"
        };
    }
}