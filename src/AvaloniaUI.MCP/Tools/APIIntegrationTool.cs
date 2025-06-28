using System.ComponentModel;

using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Tools;

[McpServerToolType]
public static class APIIntegrationTool
{
    [McpServerTool, Description("Generates HTTP client services with retry policies and error handling")]
    public static string GenerateHttpClientService(
        [Description("API type: 'rest', 'graphql', 'grpc', 'webhook'")] string apiType,
        [Description("Base URL of the API")] string baseUrl,
        [Description("Include authentication: 'true' or 'false'")] string includeAuth = "true",
        [Description("Include retry policies: 'true' or 'false'")] string includeRetry = "true")
    {
        try
        {
            var config = new ApiConfiguration
            {
                ApiType = apiType.ToLowerInvariant(),
                BaseUrl = baseUrl,
                IncludeAuthentication = bool.Parse(includeAuth),
                IncludeRetryPolicies = bool.Parse(includeRetry)
            };

            string clientService = GenerateClientService(config);
            string authService = config.IncludeAuthentication ? GenerateAuthenticationService(config) : "";
            string retryPolicy = config.IncludeRetryPolicies ? GenerateRetryPolicyCode() : "";
            string setupInstructions = GenerateSetupInstructions(config);

            return $@"# API Integration: {apiType}

## Configuration
- **API Type**: {config.ApiType}
- **Base URL**: {config.BaseUrl}
- **Authentication**: {config.IncludeAuthentication}
- **Retry Policies**: {config.IncludeRetryPolicies}

## HTTP Client Service
```csharp
{clientService}
```

{(config.IncludeAuthentication ? $@"## Authentication Service
```csharp
{authService}
```" : "")}

{(config.IncludeRetryPolicies ? $@"## Retry Policy Configuration
```csharp
{retryPolicy}
```" : "")}

## Setup Instructions
{setupInstructions}

## Best Practices
- **Timeout Configuration**: Set appropriate timeout values
- **Error Handling**: Implement comprehensive error handling
- **Circuit Breaker**: Use circuit breaker pattern for external dependencies
- **Caching**: Implement response caching where appropriate
- **Rate Limiting**: Respect API rate limits
- **Monitoring**: Log and monitor API calls for debugging";
        }
        catch (Exception ex)
        {
            return $"Error generating HTTP client service: {ex.Message}";
        }
    }

    [McpServerTool, Description("Creates data transfer objects (DTOs) and model classes for API integration")]
    public static string GenerateApiModels(
        [Description("Model type: 'request', 'response', 'entity', 'dto'")] string modelType,
        [Description("Entity name (e.g., User, Product, Order)")] string entityName,
        [Description("Include validation attributes: 'true' or 'false'")] string includeValidation = "true",
        [Description("Include JSON serialization: 'true' or 'false'")] string includeJsonSerialization = "true")
    {
        try
        {
            var config = new ModelConfiguration
            {
                ModelType = modelType.ToLowerInvariant(),
                EntityName = entityName,
                IncludeValidation = bool.Parse(includeValidation),
                IncludeJsonSerialization = bool.Parse(includeJsonSerialization)
            };

            string modelClasses = GenerateModelClasses(config);
            string validationCode = config.IncludeValidation ? GenerateValidationCode(config) : "";
            string serializationConfig = config.IncludeJsonSerialization ? GenerateSerializationConfig() : "";

            return $@"# API Models: {entityName}

## Configuration
- **Model Type**: {config.ModelType}
- **Entity Name**: {config.EntityName}
- **Validation**: {config.IncludeValidation}
- **JSON Serialization**: {config.IncludeJsonSerialization}

## Model Classes
```csharp
{modelClasses}
```

{(config.IncludeValidation ? $@"## Validation Code
```csharp
{validationCode}
```" : "")}

{(config.IncludeJsonSerialization ? $@"## Serialization Configuration
```csharp
{serializationConfig}
```" : "")}

## Usage Examples
```csharp
// Creating a request
var request = new Create{config.EntityName}Request
{{
    // Set properties
}};

// Sending request
var response = await _apiClient.Create{config.EntityName}Async(request);

// Handling response
if (response.IsSuccess)
{{
    var {config.EntityName.ToLowerInvariant()} = response.Data;
    // Process successful response
}}
else
{{
    // Handle error
    Console.WriteLine(response.ErrorMessage);
}}
```";
        }
        catch (Exception ex)
        {
            return $"Error generating API models: {ex.Message}";
        }
    }

    private sealed class ApiConfiguration
    {
        public string ApiType { get; set; } = "";
        public string BaseUrl { get; set; } = "";
        public bool IncludeAuthentication { get; set; }
        public bool IncludeRetryPolicies { get; set; }
    }

    private sealed class ModelConfiguration
    {
        public string ModelType { get; set; } = "";
        public string EntityName { get; set; } = "";
        public bool IncludeValidation { get; set; }
        public bool IncludeJsonSerialization { get; set; }
    }

    private static string GenerateClientService(ApiConfiguration config)
    {
        return config.ApiType switch
        {
            "rest" => GenerateRestClient(config),
            "graphql" => GenerateGraphQLClient(config),
            "grpc" => GenerateGrpcClient(config),
            "webhook" => GenerateWebhookClient(config),
            _ => GenerateGenericHttpClient(config)
        };
    }

    private static string GenerateRestClient(ApiConfiguration config)
    {
        return $@"// REST API Client Service
public interface IApiClientService
{{
    Task<ApiResponse<T>> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken cancellationToken = default);
}}

public class ApiClientService : IApiClientService
{{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClientService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    {(config.IncludeAuthentication ? "private readonly IApiAuthenticationService _authService;" : "")}

    public ApiClientService(
        HttpClient httpClient,
        ILogger<ApiClientService> logger{(config.IncludeAuthentication ? ",\n        IApiAuthenticationService authService" : "")})
    {{
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        {(config.IncludeAuthentication ? "_authService = authService ?? throw new ArgumentNullException(nameof(authService));" : "")}
        
        _httpClient.BaseAddress = new Uri(""{config.BaseUrl}"");
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(""application/json""));
        
        _jsonOptions = new JsonSerializerOptions
        {{
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = {{ new JsonStringEnumConverter() }}
        }};
    }}

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {{
        try
        {{
            {(config.IncludeAuthentication ? "await EnsureAuthenticatedAsync();" : "")}
            
            _logger.LogDebug(""Making GET request to: {{Endpoint}}"", endpoint);
            
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            return await ProcessResponseAsync<T>(response);
        }}
        catch (HttpRequestException ex)
        {{
            _logger.LogError(ex, ""HTTP request error for GET {{Endpoint}}"", endpoint);
            return ApiResponse<T>.Failure($""Request failed: {{ex.Message}}"");
        }}
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {{
            _logger.LogError(ex, ""Request timeout for GET {{Endpoint}}"", endpoint);
            return ApiResponse<T>.Failure(""Request timed out"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Unexpected error for GET {{Endpoint}}"", endpoint);
            return ApiResponse<T>.Failure($""Unexpected error: {{ex.Message}}"");
        }}
    }}

    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {{
        try
        {{
            {(config.IncludeAuthentication ? "await EnsureAuthenticatedAsync();" : "")}
            
            _logger.LogDebug(""Making POST request to: {{Endpoint}}"", endpoint);
            
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");
            
            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            return await ProcessResponseAsync<T>(response);
        }}
        catch (HttpRequestException ex)
        {{
            _logger.LogError(ex, ""HTTP request error for POST {{Endpoint}}"", endpoint);
            return ApiResponse<T>.Failure($""Request failed: {{ex.Message}}"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Unexpected error for POST {{Endpoint}}"", endpoint);
            return ApiResponse<T>.Failure($""Unexpected error: {{ex.Message}}"");
        }}
    }}

    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {{
        try
        {{
            {(config.IncludeAuthentication ? "await EnsureAuthenticatedAsync();" : "")}
            
            _logger.LogDebug(""Making PUT request to: {{Endpoint}}"", endpoint);
            
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");
            
            var response = await _httpClient.PutAsync(endpoint, content, cancellationToken);
            return await ProcessResponseAsync<T>(response);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Error for PUT {{Endpoint}}: {{Message}}"", endpoint, ex.Message);
            return ApiResponse<T>.Failure($""Request failed: {{ex.Message}}"");
        }}
    }}

    public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {{
        try
        {{
            {(config.IncludeAuthentication ? "await EnsureAuthenticatedAsync();" : "")}
            
            _logger.LogDebug(""Making DELETE request to: {{Endpoint}}"", endpoint);
            
            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            return await ProcessResponseAsync<T>(response);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Error for DELETE {{Endpoint}}: {{Message}}"", endpoint, ex.Message);
            return ApiResponse<T>.Failure($""Request failed: {{ex.Message}}"");
        }}
    }}

    public async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(
        string endpoint, 
        TRequest data, 
        CancellationToken cancellationToken = default)
    {{
        try
        {{
            {(config.IncludeAuthentication ? "await EnsureAuthenticatedAsync();" : "")}
            
            _logger.LogDebug(""Making typed POST request to: {{Endpoint}}"", endpoint);
            
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");
            
            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            return await ProcessResponseAsync<TResponse>(response);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Error for typed POST {{Endpoint}}: {{Message}}"", endpoint, ex.Message);
            return ApiResponse<TResponse>.Failure($""Request failed: {{ex.Message}}"");
        }}
    }}

    private async Task<ApiResponse<T>> ProcessResponseAsync<T>(HttpResponseMessage response)
    {{
        var content = await response.Content.ReadAsStringAsync();
        
        if (response.IsSuccessStatusCode)
        {{
            try
            {{
                var data = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                return ApiResponse<T>.Success(data);
            }}
            catch (JsonException ex)
            {{
                _logger.LogError(ex, ""Failed to deserialize response: {{Content}}"", content);
                return ApiResponse<T>.Failure(""Failed to parse response"");
            }}
        }}
        else
        {{
            _logger.LogWarning(""API request failed with status {{StatusCode}}: {{Content}}"", 
                response.StatusCode, content);
            
            var errorMessage = await ExtractErrorMessageAsync(content);
            return ApiResponse<T>.Failure(errorMessage);
        }}
    }}

    private async Task<string> ExtractErrorMessageAsync(string content)
    {{
        try
        {{
            // Try to parse error response
            var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(content, _jsonOptions);
            return errorResponse?.Message ?? ""An error occurred"";
        }}
        catch
        {{
            return ""An error occurred"";
        }}
    }}

    {(config.IncludeAuthentication ? @"private async Task EnsureAuthenticatedAsync()
    {
        var token = await _authService.GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue(""Bearer"", token);
        }
    }" : "")}
}}

// API Response wrapper
public class ApiResponse<T>
{{
    public bool IsSuccess {{ get; private set; }}
    public T? Data {{ get; private set; }}
    public string ErrorMessage {{ get; private set; }} = string.Empty;
    public int StatusCode {{ get; private set; }}

    public static ApiResponse<T> Success(T? data, int statusCode = 200)
    {{
        return new ApiResponse<T>
        {{
            IsSuccess = true,
            Data = data,
            StatusCode = statusCode
        }};
    }}

    public static ApiResponse<T> Failure(string errorMessage, int statusCode = 0)
    {{
        return new ApiResponse<T>
        {{
            IsSuccess = false,
            ErrorMessage = errorMessage,
            StatusCode = statusCode
        }};
    }}
}}

// Error response model
public class ApiErrorResponse
{{
    public string Message {{ get; set; }} = string.Empty;
    public string Code {{ get; set; }} = string.Empty;
    public Dictionary<string, object> Details {{ get; set; }} = new();
}}";
    }

    private static string GenerateGraphQLClient(ApiConfiguration config)
    {
        return $@"// GraphQL Client Service
public interface IGraphQLClientService
{{
    Task<GraphQLResponse<T>> QueryAsync<T>(string query, object? variables = null, CancellationToken cancellationToken = default);
    Task<GraphQLResponse<T>> MutationAsync<T>(string mutation, object? variables = null, CancellationToken cancellationToken = default);
    Task<IObservable<GraphQLResponse<T>>> SubscriptionAsync<T>(string subscription, object? variables = null);
}}

public class GraphQLClientService : IGraphQLClientService
{{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GraphQLClientService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public GraphQLClientService(HttpClient httpClient, ILogger<GraphQLClientService> logger)
    {{
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _httpClient.BaseAddress = new Uri(""{config.BaseUrl}"");
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(""application/json""));
        
        _jsonOptions = new JsonSerializerOptions
        {{
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        }};
    }}

    public async Task<GraphQLResponse<T>> QueryAsync<T>(
        string query, 
        object? variables = null, 
        CancellationToken cancellationToken = default)
    {{
        try
        {{
            var request = new GraphQLRequest
            {{
                Query = query,
                Variables = variables
            }};

            _logger.LogDebug(""Executing GraphQL query: {{Query}}"", query);
            
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");
            
            var response = await _httpClient.PostAsync(""graphql"", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {{
                var graphqlResponse = JsonSerializer.Deserialize<GraphQLResponse<T>>(responseContent, _jsonOptions);
                return graphqlResponse ?? GraphQLResponse<T>.Failure(""Failed to parse response"");
            }}
            else
            {{
                _logger.LogWarning(""GraphQL request failed: {{StatusCode}} - {{Content}}"", 
                    response.StatusCode, responseContent);
                return GraphQLResponse<T>.Failure($""Request failed: {{response.StatusCode}}"");
            }}
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Error executing GraphQL query"");
            return GraphQLResponse<T>.Failure($""Query failed: {{ex.Message}}"");
        }}
    }}

    public async Task<GraphQLResponse<T>> MutationAsync<T>(
        string mutation, 
        object? variables = null, 
        CancellationToken cancellationToken = default)
    {{
        try
        {{
            var request = new GraphQLRequest
            {{
                Query = mutation,
                Variables = variables
            }};

            _logger.LogDebug(""Executing GraphQL mutation: {{Mutation}}"", mutation);
            
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");
            
            var response = await _httpClient.PostAsync(""graphql"", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {{
                var graphqlResponse = JsonSerializer.Deserialize<GraphQLResponse<T>>(responseContent, _jsonOptions);
                return graphqlResponse ?? GraphQLResponse<T>.Failure(""Failed to parse response"");
            }}
            else
            {{
                return GraphQLResponse<T>.Failure($""Mutation failed: {{response.StatusCode}}"");
            }}
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Error executing GraphQL mutation"");
            return GraphQLResponse<T>.Failure($""Mutation failed: {{ex.Message}}"");
        }}
    }}

    public Task<IObservable<GraphQLResponse<T>>> SubscriptionAsync<T>(string subscription, object? variables = null)
    {{
        // GraphQL subscription implementation using WebSockets
        // This is a simplified version - in production, use a proper GraphQL subscription client
        throw new NotImplementedException(""GraphQL subscriptions require WebSocket implementation"");
    }}
}}

// GraphQL Models
public class GraphQLRequest
{{
    public string Query {{ get; set; }} = string.Empty;
    public object? Variables {{ get; set; }}
    public string? OperationName {{ get; set; }}
}}

public class GraphQLResponse<T>
{{
    public T? Data {{ get; set; }}
    public GraphQLError[]? Errors {{ get; set; }}
    public Dictionary<string, object>? Extensions {{ get; set; }}

    public bool IsSuccess => Errors == null || Errors.Length == 0;
    public string ErrorMessage => Errors?.FirstOrDefault()?.Message ?? string.Empty;

    public static GraphQLResponse<T> Failure(string errorMessage)
    {{
        return new GraphQLResponse<T>
        {{
            Errors = new[] {{ new GraphQLError {{ Message = errorMessage }} }}
        }};
    }}
}}

public class GraphQLError
{{
    public string Message {{ get; set; }} = string.Empty;
    public GraphQLLocation[]? Locations {{ get; set; }}
    public string[]? Path {{ get; set; }}
    public Dictionary<string, object>? Extensions {{ get; set; }}
}}

public class GraphQLLocation
{{
    public int Line {{ get; set; }}
    public int Column {{ get; set; }}
}}";
    }

    private static string GenerateGrpcClient(ApiConfiguration config)
    {
        return @"// gRPC Client Service
public interface IGrpcClientService
{
    Task<TResponse> CallAsync<TRequest, TResponse>(
        Func<TRequest, CallOptions, AsyncUnaryCall<TResponse>> method,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class;

    Task<TResponse> CallWithRetryAsync<TRequest, TResponse>(
        Func<TRequest, CallOptions, AsyncUnaryCall<TResponse>> method,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class;
}

public class GrpcClientService : IGrpcClientService
{
    private readonly ILogger<GrpcClientService> _logger;
    private readonly GrpcChannel _channel;

    public GrpcClientService(ILogger<GrpcClientService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Configure gRPC channel
        var channelOptions = new GrpcChannelOptions
        {
            MaxReceiveMessageSize = 4 * 1024 * 1024, // 4MB
            MaxSendMessageSize = 4 * 1024 * 1024,    // 4MB
        };
        
        _channel = GrpcChannel.ForAddress(""" + config.BaseUrl + @""", channelOptions);
    }

    public async Task<TResponse> CallAsync<TRequest, TResponse>(
        Func<TRequest, CallOptions, AsyncUnaryCall<TResponse>> method,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class
    {
        try
        {
            var callOptions = new CallOptions(
                deadline: DateTime.UtcNow.AddSeconds(30),
                cancellationToken: cancellationToken);

            _logger.LogDebug(""Making gRPC call for {RequestType}"", typeof(TRequest).Name);
            
            var call = method(request, callOptions);
            var response = await call.ResponseAsync;
            
            _logger.LogDebug(""gRPC call completed for {RequestType}"", typeof(TRequest).Name);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, ""gRPC call failed for {RequestType}: {StatusCode} - {Detail}"", 
                typeof(TRequest).Name, ex.StatusCode, ex.Status.Detail);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Unexpected error in gRPC call for {RequestType}"", typeof(TRequest).Name);
            throw;
        }
    }

    public async Task<TResponse> CallWithRetryAsync<TRequest, TResponse>(
        Func<TRequest, CallOptions, AsyncUnaryCall<TResponse>> method,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class
    {
        const int maxRetries = 3;
        var delay = TimeSpan.FromMilliseconds(500);

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await CallAsync(method, request, cancellationToken);
            }
            catch (RpcException ex) when (attempt < maxRetries && IsRetryableStatus(ex.StatusCode))
            {
                _logger.LogWarning(""gRPC call failed on attempt {Attempt}/{MaxRetries}, retrying in {Delay}ms"", 
                    attempt, maxRetries, delay.TotalMilliseconds);
                
                await Task.Delay(delay, cancellationToken);
                delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2); // Exponential backoff
            }
        }

        // This line should not be reached due to the exception handling above
        throw new InvalidOperationException(""Retry logic failed unexpectedly"");
    }

    private static bool IsRetryableStatus(StatusCode statusCode)
    {
        return statusCode == StatusCode.Unavailable ||
               statusCode == StatusCode.DeadlineExceeded ||
               statusCode == StatusCode.ResourceExhausted ||
               statusCode == StatusCode.Aborted;
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}";
    }

    private static string GenerateWebhookClient(ApiConfiguration config)
    {
        return @"// Webhook Client Service
public interface IWebhookClientService
{
    Task<bool> SendWebhookAsync<T>(string webhookUrl, T payload, CancellationToken cancellationToken = default);
    Task<bool> SendWebhookWithSignatureAsync<T>(string webhookUrl, T payload, string secret, CancellationToken cancellationToken = default);
    Task<WebhookDeliveryResult> SendWebhookWithRetryAsync<T>(string webhookUrl, T payload, CancellationToken cancellationToken = default);
}

public class WebhookClientService : IWebhookClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebhookClientService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public WebhookClientService(HttpClient httpClient, ILogger<WebhookClientService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task<bool> SendWebhookAsync<T>(string webhookUrl, T payload, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");
            
            // Add webhook headers
            content.Headers.Add(""X-Webhook-Timestamp"", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            content.Headers.Add(""X-Webhook-Event"", typeof(T).Name);
            
            _logger.LogDebug(""Sending webhook to {Url}"", webhookUrl);
            
            var response = await _httpClient.PostAsync(webhookUrl, content, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(""Webhook delivered successfully to {Url}"", webhookUrl);
                return true;
            }
            else
            {
                _logger.LogWarning(""Webhook delivery failed to {Url}: {StatusCode}"", webhookUrl, response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error sending webhook to {Url}"", webhookUrl);
            return false;
        }
    }

    public async Task<bool> SendWebhookWithSignatureAsync<T>(
        string webhookUrl, 
        T payload, 
        string secret, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");
            
            // Generate HMAC signature
            var signature = GenerateHmacSignature(json, secret);
            
            // Add webhook headers
            content.Headers.Add(""X-Webhook-Timestamp"", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            content.Headers.Add(""X-Webhook-Event"", typeof(T).Name);
            content.Headers.Add(""X-Webhook-Signature"", $""sha256={signature}"");
            
            _logger.LogDebug(""Sending signed webhook to {Url}"", webhookUrl);
            
            var response = await _httpClient.PostAsync(webhookUrl, content, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(""Signed webhook delivered successfully to {Url}"", webhookUrl);
                return true;
            }
            else
            {
                _logger.LogWarning(""Signed webhook delivery failed to {Url}: {StatusCode}"", webhookUrl, response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error sending signed webhook to {Url}"", webhookUrl);
            return false;
        }
    }

    public async Task<WebhookDeliveryResult> SendWebhookWithRetryAsync<T>(
        string webhookUrl, 
        T payload, 
        CancellationToken cancellationToken = default)
    {
        const int maxRetries = 3;
        var delays = new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15) };
        
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                var success = await SendWebhookAsync(webhookUrl, payload, cancellationToken);
                
                if (success)
                {
                    return new WebhookDeliveryResult
                    {
                        IsSuccess = true,
                        AttemptCount = attempt + 1,
                        DeliveredAt = DateTime.UtcNow
                    };
                }
                
                if (attempt < maxRetries - 1)
                {
                    var delay = delays[attempt];
                    _logger.LogInformation(""Webhook delivery failed, retrying in {Delay} seconds (attempt {Attempt}/{MaxRetries})"", 
                        delay.TotalSeconds, attempt + 1, maxRetries);
                    
                    await Task.Delay(delay, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ""Error on webhook delivery attempt {Attempt}"", attempt + 1);
                
                if (attempt == maxRetries - 1)
                {
                    return new WebhookDeliveryResult
                    {
                        IsSuccess = false,
                        AttemptCount = maxRetries,
                        ErrorMessage = ex.Message,
                        LastAttemptAt = DateTime.UtcNow
                    };
                }
            }
        }
        
        return new WebhookDeliveryResult
        {
            IsSuccess = false,
            AttemptCount = maxRetries,
            ErrorMessage = ""All retry attempts failed"",
            LastAttemptAt = DateTime.UtcNow
        };
    }

    private string GenerateHmacSignature(string payload, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        
        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(payloadBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}

public class WebhookDeliveryResult
{
    public bool IsSuccess { get; set; }
    public int AttemptCount { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? LastAttemptAt { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}";
    }

    private static string GenerateGenericHttpClient(ApiConfiguration config)
    {
        return @"// Generic HTTP Client Service
public interface IGenericHttpClientService
{
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
    Task<T?> GetJsonAsync<T>(string url, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PostJsonAsync<T>(string url, T data, CancellationToken cancellationToken = default);
}

public class GenericHttpClientService : IGenericHttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GenericHttpClientService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public GenericHttpClientService(HttpClient httpClient, ILogger<GenericHttpClientService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug(""Sending {Method} request to {Url}"", request.Method, request.RequestUri);
            
            var response = await _httpClient.SendAsync(request, cancellationToken);
            
            _logger.LogDebug(""Received response {StatusCode} from {Url}"", response.StatusCode, request.RequestUri);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error sending request to {Url}"", request.RequestUri);
            throw;
        }
    }

    public async Task<T?> GetJsonAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error getting JSON from {Url}"", url);
            throw;
        }
    }

    public async Task<HttpResponseMessage> PostJsonAsync<T>(string url, T data, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");
            
            return await _httpClient.PostAsync(url, content, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error posting JSON to {Url}"", url);
            throw;
        }
    }
}";
    }

    private static string GenerateAuthenticationService(ApiConfiguration config)
    {
        return @"// API Authentication Service
public interface IApiAuthenticationService
{
    Task<string?> GetAccessTokenAsync();
    Task<bool> RefreshTokenAsync();
    Task<bool> AuthenticateAsync(string username, string password);
    Task LogoutAsync();
}

public class ApiAuthenticationService : IApiAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ISecureStorage _secureStorage;
    private readonly ILogger<ApiAuthenticationService> _logger;
    private readonly AuthenticationSettings _settings;

    public ApiAuthenticationService(
        HttpClient httpClient,
        ISecureStorage secureStorage,
        ILogger<ApiAuthenticationService> logger,
        IOptions<AuthenticationSettings> settings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        try
        {
            var token = await _secureStorage.GetAsync(""access_token"");
            
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogDebug(""No access token found in secure storage"");
                return null;
            }

            // Check if token is expired
            if (IsTokenExpired(token))
            {
                _logger.LogDebug(""Access token is expired, attempting refresh"");
                
                var refreshSuccess = await RefreshTokenAsync();
                if (refreshSuccess)
                {
                    token = await _secureStorage.GetAsync(""access_token"");
                }
                else
                {
                    _logger.LogWarning(""Token refresh failed"");
                    return null;
                }
            }

            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error getting access token"");
            return null;
        }
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var refreshToken = await _secureStorage.GetAsync(""refresh_token"");
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning(""No refresh token available"");
                return false;
            }

            var request = new
            {
                grant_type = ""refresh_token"",
                refresh_token = refreshToken,
                client_id = _settings.ClientId,
                client_secret = _settings.ClientSecret
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");

            var response = await _httpClient.PostAsync(""/oauth/token"", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

                if (tokenResponse?.AccessToken != null)
                {
                    await _secureStorage.SetAsync(""access_token"", tokenResponse.AccessToken);
                    
                    if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
                    {
                        await _secureStorage.SetAsync(""refresh_token"", tokenResponse.RefreshToken);
                    }

                    _logger.LogInformation(""Token refreshed successfully"");
                    return true;
                }
            }

            _logger.LogWarning(""Token refresh failed with status: {StatusCode}"", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error refreshing token"");
            return false;
        }
    }

    public async Task<bool> AuthenticateAsync(string username, string password)
    {
        try
        {
            var request = new
            {
                grant_type = ""password"",
                username = username,
                password = password,
                client_id = _settings.ClientId,
                client_secret = _settings.ClientSecret,
                scope = _settings.Scope
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");

            var response = await _httpClient.PostAsync(""/oauth/token"", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

                if (tokenResponse?.AccessToken != null)
                {
                    await _secureStorage.SetAsync(""access_token"", tokenResponse.AccessToken);
                    
                    if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
                    {
                        await _secureStorage.SetAsync(""refresh_token"", tokenResponse.RefreshToken);
                    }

                    _logger.LogInformation(""Authentication successful for user: {Username}"", username);
                    return true;
                }
            }

            _logger.LogWarning(""Authentication failed for user: {Username}"", username);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error during authentication for user: {Username}"", username);
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _secureStorage.RemoveAsync(""access_token"");
            await _secureStorage.RemoveAsync(""refresh_token"");
            
            _logger.LogInformation(""User logged out successfully"");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error during logout"");
        }
    }

    private bool IsTokenExpired(string token)
    {
        try
        {
            // Decode JWT token to check expiration
            var parts = token.Split('.');
            if (parts.Length != 3) return true;

            var payload = parts[1];
            // Add padding if necessary
            while (payload.Length % 4 != 0)
            {
                payload += ""="";
            }

            var jsonBytes = Convert.FromBase64String(payload);
            var json = Encoding.UTF8.GetString(jsonBytes);
            var tokenData = JsonSerializer.Deserialize<JsonElement>(json);

            if (tokenData.TryGetProperty(""exp"", out var expElement))
            {
                var exp = expElement.GetInt64();
                var expDateTime = DateTimeOffset.FromUnixTimeSeconds(exp);
                return expDateTime <= DateTimeOffset.UtcNow.AddMinutes(-5); // 5 minute buffer
            }

            return true; // If no expiration found, consider expired
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error checking token expiration"");
            return true; // If error parsing, consider expired
        }
    }
}

// Token Response Model
public class TokenResponse
{
    [JsonPropertyName(""access_token"")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName(""refresh_token"")]
    public string? RefreshToken { get; set; }

    [JsonPropertyName(""token_type"")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName(""expires_in"")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName(""scope"")]
    public string? Scope { get; set; }
}

// Authentication Settings
public class AuthenticationSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
}";
    }

    private static string GenerateRetryPolicyCode()
    {
        return @"// Retry Policy Configuration using Polly
public static class RetryPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> CreateHttpRetryPolicy()
    {
        return Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    var logger = context.GetLogger();
                    logger?.LogWarning(""Retry {RetryCount} after {Delay}ms for {OperationKey}"", 
                        retryCount, timespan.TotalMilliseconds, context.OperationKey);
                });
    }

    public static IAsyncPolicy<T> CreateGenericRetryPolicy<T>()
    {
        return Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .Or<SocketException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timespan, retryCount, context) =>
                {
                    var logger = context.GetLogger();
                    logger?.LogWarning(exception, ""Retry {RetryCount} after {Delay}ms for {OperationKey}"", 
                        retryCount, timespan.TotalMilliseconds, context.OperationKey);
                });
    }

    public static IAsyncPolicy CreateCircuitBreakerPolicy()
    {
        return Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (exception, duration) =>
                {
                    // Log circuit breaker opened
                },
                onReset: () =>
                {
                    // Log circuit breaker closed
                });
    }

    public static IAsyncPolicy<HttpResponseMessage> CreateCombinedPolicy()
    {
        var retryPolicy = CreateHttpRetryPolicy();
        var circuitBreakerPolicy = CreateCircuitBreakerPolicy();
        
        return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
    }
}

// Service registration extension
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthenticationSettings>(configuration.GetSection(""Authentication""));
        
        services.AddHttpClient<IApiClientService, ApiClientService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add(""User-Agent"", ""YourApp/1.0"");
        })
        .AddPolicyHandler(RetryPolicies.CreateCombinedPolicy());

        services.AddScoped<IApiAuthenticationService, ApiAuthenticationService>();
        
        return services;
    }
}";
    }

    private static string GenerateModelClasses(ModelConfiguration config)
    {
        return config.ModelType switch
        {
            "request" => GenerateRequestModels(config),
            "response" => GenerateResponseModels(config),
            "entity" => GenerateEntityModels(config),
            "dto" => GenerateDtoModels(config),
            _ => GenerateGenericModels(config)
        };
    }

    private static string GenerateRequestModels(ModelConfiguration config)
    {
        string validationAttributes = config.IncludeValidation ? GetValidationAttributes() : "";
        string jsonAttributes = config.IncludeJsonSerialization ? GetJsonAttributes() : "";

        return $@"// Request Models for {config.EntityName}

public class Create{config.EntityName}Request
{{
    {validationAttributes}
    {jsonAttributes}
    public string Name {{ get; set; }} = string.Empty;

    {validationAttributes}
    {jsonAttributes}
    public string Description {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public bool IsActive {{ get; set; }} = true;

    {jsonAttributes}
    public Dictionary<string, object> Metadata {{ get; set; }} = new();
}}

public class Update{config.EntityName}Request
{{
    {validationAttributes}
    {jsonAttributes}
    public string Id {{ get; set; }} = string.Empty;

    {validationAttributes}
    {jsonAttributes}
    public string Name {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string Description {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public bool IsActive {{ get; set; }}

    {jsonAttributes}
    public Dictionary<string, object> Metadata {{ get; set; }} = new();
}}

public class Get{config.EntityName}Request
{{
    {validationAttributes}
    {jsonAttributes}
    public string Id {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public bool IncludeDetails {{ get; set; }} = false;
}}

public class List{config.EntityName}Request
{{
    {jsonAttributes}
    public int PageNumber {{ get; set; }} = 1;

    {jsonAttributes}
    public int PageSize {{ get; set; }} = 20;

    {jsonAttributes}
    public string? SearchTerm {{ get; set; }}

    {jsonAttributes}
    public string? SortBy {{ get; set; }}

    {jsonAttributes}
    public string SortDirection {{ get; set; }} = ""asc"";

    {jsonAttributes}
    public Dictionary<string, object> Filters {{ get; set; }} = new();
}}

public class Delete{config.EntityName}Request
{{
    {validationAttributes}
    {jsonAttributes}
    public string Id {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string? Reason {{ get; set; }}
}}";
    }

    private static string GenerateResponseModels(ModelConfiguration config)
    {
        string jsonAttributes = config.IncludeJsonSerialization ? GetJsonAttributes() : "";

        return $@"// Response Models for {config.EntityName}

public class {config.EntityName}Response
{{
    {jsonAttributes}
    public string Id {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string Name {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string Description {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public bool IsActive {{ get; set; }}

    {jsonAttributes}
    public DateTime CreatedAt {{ get; set; }}

    {jsonAttributes}
    public DateTime? UpdatedAt {{ get; set; }}

    {jsonAttributes}
    public string CreatedBy {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string? UpdatedBy {{ get; set; }}

    {jsonAttributes}
    public Dictionary<string, object> Metadata {{ get; set; }} = new();
}}

public class List{config.EntityName}Response
{{
    {jsonAttributes}
    public IEnumerable<{config.EntityName}Response> Items {{ get; set; }} = new List<{config.EntityName}Response>();

    {jsonAttributes}
    public int TotalCount {{ get; set; }}

    {jsonAttributes}
    public int PageNumber {{ get; set; }}

    {jsonAttributes}
    public int PageSize {{ get; set; }}

    {jsonAttributes}
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    {jsonAttributes}
    public bool HasPreviousPage => PageNumber > 1;

    {jsonAttributes}
    public bool HasNextPage => PageNumber < TotalPages;
}}

public class ApiResponse<T>
{{
    {jsonAttributes}
    public bool IsSuccess {{ get; set; }}

    {jsonAttributes}
    public T? Data {{ get; set; }}

    {jsonAttributes}
    public string? ErrorMessage {{ get; set; }}

    {jsonAttributes}
    public string? ErrorCode {{ get; set; }}

    {jsonAttributes}
    public Dictionary<string, object> Metadata {{ get; set; }} = new();

    {jsonAttributes}
    public DateTime Timestamp {{ get; set; }} = DateTime.UtcNow;

    public static ApiResponse<T> Success(T data)
    {{
        return new ApiResponse<T>
        {{
            IsSuccess = true,
            Data = data
        }};
    }}

    public static ApiResponse<T> Failure(string errorMessage, string? errorCode = null)
    {{
        return new ApiResponse<T>
        {{
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode
        }};
    }}
}}

public class ErrorResponse
{{
    {jsonAttributes}
    public string Message {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string Code {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string? Details {{ get; set; }}

    {jsonAttributes}
    public Dictionary<string, string[]> ValidationErrors {{ get; set; }} = new();

    {jsonAttributes}
    public DateTime Timestamp {{ get; set; }} = DateTime.UtcNow;
}}";
    }

    private static string GenerateEntityModels(ModelConfiguration config)
    {
        string validationAttributes = config.IncludeValidation ? GetValidationAttributes() : "";
        string jsonAttributes = config.IncludeJsonSerialization ? GetJsonAttributes() : "";

        return $@"// Entity Models for {config.EntityName}

public class {config.EntityName}Entity
{{
    {jsonAttributes}
    public string Id {{ get; set; }} = string.Empty;

    {validationAttributes}
    {jsonAttributes}
    public string Name {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string Description {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public bool IsActive {{ get; set; }} = true;

    {jsonAttributes}
    public DateTime CreatedAt {{ get; set; }} = DateTime.UtcNow;

    {jsonAttributes}
    public DateTime? UpdatedAt {{ get; set; }}

    {jsonAttributes}
    public string CreatedBy {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string? UpdatedBy {{ get; set; }}

    {jsonAttributes}
    public string Version {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public Dictionary<string, object> Metadata {{ get; set; }} = new();

    // Navigation properties
    {jsonAttributes}
    public ICollection<Related{config.EntityName}> RelatedItems {{ get; set; }} = new List<Related{config.EntityName}>();
}}

public class Related{config.EntityName}
{{
    {jsonAttributes}
    public string Id {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string {config.EntityName}Id {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string Name {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string Type {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public object Value {{ get; set; }} = new();

    // Navigation property
    {jsonAttributes}
    public {config.EntityName}Entity? {config.EntityName} {{ get; set; }}
}}

public enum {config.EntityName}Status
{{
    Draft,
    Active,
    Inactive,
    Archived,
    Deleted
}}

public class {config.EntityName}Filter
{{
    public string? Name {{ get; set; }}
    public bool? IsActive {{ get; set; }}
    public {config.EntityName}Status? Status {{ get; set; }}
    public DateTime? CreatedAfter {{ get; set; }}
    public DateTime? CreatedBefore {{ get; set; }}
    public string? CreatedBy {{ get; set; }}
    public Dictionary<string, object> MetadataFilters {{ get; set; }} = new();
}}";
    }

    private static string GenerateDtoModels(ModelConfiguration config)
    {
        string validationAttributes = config.IncludeValidation ? GetValidationAttributes() : "";
        string jsonAttributes = config.IncludeJsonSerialization ? GetJsonAttributes() : "";

        return $@"// Data Transfer Objects for {config.EntityName}

public class {config.EntityName}Dto
{{
    {jsonAttributes}
    public string Id {{ get; set; }} = string.Empty;

    {validationAttributes}
    {jsonAttributes}
    public string Name {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string Description {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public bool IsActive {{ get; set; }}

    {jsonAttributes}
    public DateTime CreatedAt {{ get; set; }}

    {jsonAttributes}
    public DateTime? UpdatedAt {{ get; set; }}

    {jsonAttributes}
    public string CreatedBy {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public Dictionary<string, object> Metadata {{ get; set; }} = new();
}}

public class Create{config.EntityName}Dto
{{
    {validationAttributes}
    {jsonAttributes}
    public string Name {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string Description {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public bool IsActive {{ get; set; }} = true;

    {jsonAttributes}
    public Dictionary<string, object> Metadata {{ get; set; }} = new();
}}

public class Update{config.EntityName}Dto
{{
    {validationAttributes}
    {jsonAttributes}
    public string Name {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string Description {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public bool IsActive {{ get; set; }}

    {jsonAttributes}
    public Dictionary<string, object> Metadata {{ get; set; }} = new();
}}

public class {config.EntityName}SummaryDto
{{
    {jsonAttributes}
    public string Id {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string Name {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public bool IsActive {{ get; set; }}

    {jsonAttributes}
    public DateTime CreatedAt {{ get; set; }}
}}

public class {config.EntityName}DetailDto : {config.EntityName}Dto
{{
    {jsonAttributes}
    public ICollection<Related{config.EntityName}Dto> RelatedItems {{ get; set; }} = new List<Related{config.EntityName}Dto>();

    {jsonAttributes}
    public {config.EntityName}Statistics Statistics {{ get; set; }} = new();
}}

public class Related{config.EntityName}Dto
{{
    {jsonAttributes}
    public string Id {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string Name {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public string Type {{ get; set; }} = string.Empty;

    {jsonAttributes}
    public object Value {{ get; set; }} = new();
}}

public class {config.EntityName}Statistics
{{
    {jsonAttributes}
    public int TotalViews {{ get; set; }}

    {jsonAttributes}
    public int TotalEdits {{ get; set; }}

    {jsonAttributes}
    public DateTime LastAccessed {{ get; set; }}

    {jsonAttributes}
    public Dictionary<string, int> MetricsData {{ get; set; }} = new();
}}";
    }

    private static string GenerateGenericModels(ModelConfiguration config)
    {
        return $@"// Generic Models for {config.EntityName}

public class {config.EntityName}Model
{{
    public string Id {{ get; set; }} = string.Empty;
    public string Name {{ get; set; }} = string.Empty;
    public Dictionary<string, object> Properties {{ get; set; }} = new();
    public DateTime CreatedAt {{ get; set; }} = DateTime.UtcNow;
}}";
    }

    private static string GetValidationAttributes()
    {
        return @"[Required(ErrorMessage = ""This field is required"")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ""Length must be between 2 and 100 characters"")]";
    }

    private static string GetJsonAttributes()
    {
        return @"[JsonPropertyName(""name"")]";
    }

    private static string GenerateValidationCode(ModelConfiguration config)
    {
        return @"// Validation Extensions and Custom Validators
public static class ValidationExtensions
{
    public static bool IsValid<T>(this T model, out ICollection<ValidationResult> validationResults)
    {
        validationResults = new List<ValidationResult>();
        var context = new ValidationContext(model);
        return Validator.TryValidateObject(model, context, validationResults, true);
    }

    public static void ValidateAndThrow<T>(this T model)
    {
        if (!model.IsValid(out var validationResults))
        {
            var errors = validationResults.Select(vr => vr.ErrorMessage).ToArray();
            throw new ValidationException(string.Join(""; "", errors));
        }
    }
}

// Custom Validation Attributes
public class NotEmptyGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is Guid guid)
        {
            return guid != Guid.Empty;
        }
        if (value is string stringValue && Guid.TryParse(stringValue, out var parsedGuid))
        {
            return parsedGuid != Guid.Empty;
        }
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $""The {name} field must be a valid, non-empty GUID."";
    }
}

public class ValidEnumAttribute : ValidationAttribute
{
    private readonly Type _enumType;

    public ValidEnumAttribute(Type enumType)
    {
        _enumType = enumType;
    }

    public override bool IsValid(object? value)
    {
        if (value == null) return true; // Use Required attribute for null checks
        return Enum.IsDefined(_enumType, value);
    }

    public override string FormatErrorMessage(string name)
    {
        return $""The {name} field must be a valid {_enumType.Name} value."";
    }
}";
    }

    private static string GenerateSerializationConfig()
    {
        return @"// JSON Serialization Configuration
public static class JsonSerializationConfig
{
    public static JsonSerializerOptions DefaultOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter(),
            new DateTimeConverter(),
            new GuidConverter()
        },
        WriteIndented = false,
        PropertyNameCaseInsensitive = true
    };

    public static JsonSerializerOptions PrettyPrintOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter(),
            new DateTimeConverter(),
            new GuidConverter()
        },
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };
}

// Custom JSON Converters
public class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(""yyyy-MM-ddTHH:mm:ss.fffZ""));
    }
}

public class GuidConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Guid.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

// Extension methods for serialization
public static class SerializationExtensions
{
    public static string ToJson<T>(this T obj, bool prettyPrint = false)
    {
        var options = prettyPrint ? JsonSerializationConfig.PrettyPrintOptions : JsonSerializationConfig.DefaultOptions;
        return JsonSerializer.Serialize(obj, options);
    }

    public static T? FromJson<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json, JsonSerializationConfig.DefaultOptions);
    }

    public static byte[] ToJsonBytes<T>(this T obj)
    {
        return JsonSerializer.SerializeToUtf8Bytes(obj, JsonSerializationConfig.DefaultOptions);
    }

    public static T? FromJsonBytes<T>(this byte[] jsonBytes)
    {
        return JsonSerializer.Deserialize<T>(jsonBytes, JsonSerializationConfig.DefaultOptions);
    }
}";
    }

    private static string GenerateSetupInstructions(ApiConfiguration config)
    {
        return $@"### 1. Install Required Packages
```xml
<PackageReference Include=""Microsoft.Extensions.Http"" Version=""8.0.0"" />
<PackageReference Include=""Microsoft.Extensions.Http.Polly"" Version=""8.0.0"" />
<PackageReference Include=""System.Text.Json"" Version=""8.0.0"" />
{(config.IncludeAuthentication ? @"<PackageReference Include=""Microsoft.Extensions.Configuration.Binder"" Version=""8.0.0"" />" : "")}
</PackageReference>
```

### 2. Configure Services in Program.cs
```csharp
// Add HTTP client with retry policies
builder.Services.AddApiClient(builder.Configuration);

// Configure authentication settings
{(config.IncludeAuthentication ? @"builder.Services.Configure<AuthenticationSettings>(
    builder.Configuration.GetSection(""ApiSettings:Authentication""));" : "")}

// Add logging
builder.Services.AddLogging();
```

### 3. Configuration in appsettings.json
```json
{{
  ""ApiSettings"": {{
    ""BaseUrl"": ""{config.BaseUrl}"",
    {(config.IncludeAuthentication ? @"""Authentication"": {
      ""ClientId"": ""your-client-id"",
      ""ClientSecret"": ""your-client-secret"",
      ""Scope"": ""api.read api.write""
    }," : "")}
    ""Timeout"": ""00:00:30"",
    ""RetryAttempts"": 3
  }}
}}
```

### 4. Usage Example
```csharp
public class ExampleService
{{
    private readonly IApiClientService _apiClient;

    public ExampleService(IApiClientService apiClient)
    {{
        _apiClient = apiClient;
    }}

    public async Task<UserResponse?> GetUserAsync(string userId)
    {{
        var response = await _apiClient.GetAsync<UserResponse>($""/users/{{userId}}"");
        return response.IsSuccess ? response.Data : null;
    }}

    public async Task<bool> CreateUserAsync(CreateUserRequest request)
    {{
        var response = await _apiClient.PostAsync<UserResponse>(""/users"", request);
        return response.IsSuccess;
    }}
}}
```

### 5. Error Handling Best Practices
- Always check `IsSuccess` property of API responses
- Log errors and retry attempts appropriately
- Implement circuit breaker for external dependencies
- Use cancellation tokens for long-running operations
- Handle network timeouts gracefully";
    }
}
