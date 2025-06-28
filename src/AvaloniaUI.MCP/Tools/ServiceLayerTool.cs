using System.ComponentModel;
using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Tools;

[McpServerToolType]
public static class ServiceLayerTool
{
    [McpServerTool, Description("Generates business service layer with validation and error handling")]
    public static string GenerateBusinessService(
        [Description("Service name (e.g., UserService, OrderService)")] string serviceName,
        [Description("Entity name it manages")] string entityName,
        [Description("Include validation: 'true' or 'false'")] string includeValidation = "true",
        [Description("Include mapping: 'true' or 'false'")] string includeMapping = "true",
        [Description("Include caching: 'true' or 'false'")] string includeCaching = "false")
    {
        try
        {
            var config = new ServiceConfiguration
            {
                ServiceName = serviceName,
                EntityName = entityName,
                IncludeValidation = bool.Parse(includeValidation),
                IncludeMapping = bool.Parse(includeMapping),
                IncludeCaching = bool.Parse(includeCaching)
            };

            var serviceInterface = GenerateServiceInterface(config);
            var serviceImplementation = GenerateServiceImplementation(config);
            var dtoClasses = GenerateDataTransferObjects(config);
            var validationCode = config.IncludeValidation ? GenerateValidationCode(config) : "";

            return $@"# Business Service Layer: {serviceName}

## Configuration
- **Service**: {config.ServiceName}
- **Entity**: {config.EntityName}
- **Validation**: {config.IncludeValidation}
- **Mapping**: {config.IncludeMapping}
- **Caching**: {config.IncludeCaching}

## Service Interface
```csharp
{serviceInterface}
```

## Service Implementation
```csharp
{serviceImplementation}
```

## Data Transfer Objects
```csharp
{dtoClasses}
```

{(config.IncludeValidation ? $@"## Validation Rules
```csharp
{validationCode}
```" : "")}

## Usage Example
```csharp
// In ViewModel or Controller
public class {config.EntityName}ViewModel
{{
    private readonly I{config.ServiceName} _{config.ServiceName.ToLowerInvariant()};
    
    public {config.EntityName}ViewModel(I{config.ServiceName} {config.ServiceName.ToLowerInvariant()})
    {{
        _{config.ServiceName.ToLowerInvariant()} = {config.ServiceName.ToLowerInvariant()};
    }}
    
    public async Task Load{config.EntityName}sAsync()
    {{
        var result = await _{config.ServiceName.ToLowerInvariant()}.GetAllAsync();
        if (result.IsSuccess)
        {{
            // Handle successful result
            var {config.EntityName.ToLowerInvariant()}s = result.Data;
        }}
        else
        {{
            // Handle error
            var error = result.Error;
        }}
    }}
}}
```";
        }
        catch (Exception ex)
        {
            return $"Error generating business service: {ex.Message}";
        }
    }

    [McpServerTool, Description("Creates domain service patterns for complex business logic")]
    public static string GenerateDomainService(
        [Description("Domain service name")] string serviceName,
        [Description("Business domain (e.g., Order Management, User Authentication)")] string businessDomain,
        [Description("Include domain events: 'true' or 'false'")] string includeDomainEvents = "true",
        [Description("Include business rules engine: 'true' or 'false'")] string includeRulesEngine = "false")
    {
        try
        {
            var config = new DomainServiceConfiguration
            {
                ServiceName = serviceName,
                BusinessDomain = businessDomain,
                IncludeDomainEvents = bool.Parse(includeDomainEvents),
                IncludeRulesEngine = bool.Parse(includeRulesEngine)
            };

            var domainServiceCode = GenerateDomainServiceCode(config);
            var businessRulesCode = config.IncludeRulesEngine ? GenerateBusinessRulesEngine(config) : "";
            var domainEventsCode = config.IncludeDomainEvents ? GenerateDomainEventsCode(config) : "";

            return $@"# Domain Service: {serviceName}

## Business Domain: {businessDomain}

## Configuration
- **Domain Events**: {config.IncludeDomainEvents}
- **Business Rules Engine**: {config.IncludeRulesEngine}

## Domain Service Implementation
```csharp
{domainServiceCode}
```

{(config.IncludeRulesEngine ? $@"## Business Rules Engine
```csharp
{businessRulesCode}
```" : "")}

{(config.IncludeDomainEvents ? $@"## Domain Events
```csharp
{domainEventsCode}
```" : "")}

## Domain Service Principles
- **Stateless**: Domain services should be stateless
- **Pure Business Logic**: Focus only on business rules and coordination
- **Single Responsibility**: Each service handles one business concern
- **Testable**: Easy to unit test with mocked dependencies";
        }
        catch (Exception ex)
        {
            return $"Error generating domain service: {ex.Message}";
        }
    }

    private class ServiceConfiguration
    {
        public string ServiceName { get; set; } = "";
        public string EntityName { get; set; } = "";
        public bool IncludeValidation { get; set; }
        public bool IncludeMapping { get; set; }
        public bool IncludeCaching { get; set; }
    }

    private class DomainServiceConfiguration
    {
        public string ServiceName { get; set; } = "";
        public string BusinessDomain { get; set; } = "";
        public bool IncludeDomainEvents { get; set; }
        public bool IncludeRulesEngine { get; set; }
    }

    private static string GenerateServiceInterface(ServiceConfiguration config)
    {
        return $@"public interface I{config.ServiceName}
{{
    Task<ServiceResult<{config.EntityName}Dto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<IEnumerable<{config.EntityName}Dto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult<PagedResult<{config.EntityName}Dto>>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<ServiceResult<{config.EntityName}Dto>> CreateAsync(Create{config.EntityName}Request request, CancellationToken cancellationToken = default);
    Task<ServiceResult<{config.EntityName}Dto>> UpdateAsync(int id, Update{config.EntityName}Request request, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> ExistsAsync(int id, CancellationToken cancellationToken = default);
    
    // Business-specific methods
    Task<ServiceResult<IEnumerable<{config.EntityName}Dto>>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<ServiceResult<{config.EntityName}Dto>> ActivateAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<{config.EntityName}Dto>> DeactivateAsync(int id, CancellationToken cancellationToken = default);
}}

// Generic service result wrapper
public class ServiceResult<T>
{{
    public bool IsSuccess {{ get; private set; }}
    public T? Data {{ get; private set; }}
    public string Error {{ get; private set; }} = string.Empty;
    public List<string> ValidationErrors {{ get; private set; }} = new();

    private ServiceResult() {{ }}

    public static ServiceResult<T> Success(T data)
    {{
        return new ServiceResult<T>
        {{
            IsSuccess = true,
            Data = data
        }};
    }}

    public static ServiceResult<T> Failure(string error)
    {{
        return new ServiceResult<T>
        {{
            IsSuccess = false,
            Error = error
        }};
    }}

    public static ServiceResult<T> ValidationFailure(List<string> validationErrors)
    {{
        return new ServiceResult<T>
        {{
            IsSuccess = false,
            ValidationErrors = validationErrors
        }};
    }}
}}";
    }

    private static string GenerateServiceImplementation(ServiceConfiguration config)
    {
        var cachingField = config.IncludeCaching ? "private readonly ICacheService _cacheService;" : "";
        var cachingParam = config.IncludeCaching ? ", ICacheService cacheService" : "";
        var cachingInit = config.IncludeCaching ? "_cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));" : "";
        var validationField = config.IncludeValidation ? $"private readonly IValidator<Create{config.EntityName}Request> _createValidator;\n    private readonly IValidator<Update{config.EntityName}Request> _updateValidator;" : "";
        var validationParam = config.IncludeValidation ? $", IValidator<Create{config.EntityName}Request> createValidator, IValidator<Update{config.EntityName}Request> updateValidator" : "";
        var validationInit = config.IncludeValidation ? $"_createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));\n        _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));" : "";

        return $@"public class {config.ServiceName} : I{config.ServiceName}
{{
    private readonly I{config.EntityName}Repository _{config.EntityName.ToLowerInvariant()}Repository;
    private readonly IMapper _mapper;
    private readonly ILogger<{config.ServiceName}> _logger;
    {validationField}
    {cachingField}

    public {config.ServiceName}(
        I{config.EntityName}Repository {config.EntityName.ToLowerInvariant()}Repository,
        IMapper mapper,
        ILogger<{config.ServiceName}> logger{validationParam}{cachingParam})
    {{
        _{config.EntityName.ToLowerInvariant()}Repository = {config.EntityName.ToLowerInvariant()}Repository ?? throw new ArgumentNullException(nameof({config.EntityName.ToLowerInvariant()}Repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        {validationInit}
        {cachingInit}
    }}

    public async Task<ServiceResult<{config.EntityName}Dto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {{
        try
        {{
            _logger.LogInformation(""Getting {config.EntityName} with ID {{Id}}"", id);

            {(config.IncludeCaching ? $@"// Check cache first
            var cacheKey = $""{config.EntityName.ToLowerInvariant()}:{{id}}"";
            var cached = await _cacheService.GetAsync<{config.EntityName}Dto>(cacheKey, cancellationToken);
            if (cached != null)
            {{
                return ServiceResult<{config.EntityName}Dto>.Success(cached);
            }}" : "")}

            var entity = await _{config.EntityName.ToLowerInvariant()}Repository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {{
                return ServiceResult<{config.EntityName}Dto>.Failure($""{config.EntityName} with ID {{id}} not found"");
            }}

            var dto = _mapper.Map<{config.EntityName}Dto>(entity);

            {(config.IncludeCaching ? $@"// Cache the result
            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15), cancellationToken);" : "")}

            return ServiceResult<{config.EntityName}Dto>.Success(dto);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Error getting {config.EntityName} with ID {{Id}}"", id);
            return ServiceResult<{config.EntityName}Dto>.Failure($""An error occurred while retrieving the {config.EntityName.ToLowerInvariant()}"");
        }}
    }}

    public async Task<ServiceResult<IEnumerable<{config.EntityName}Dto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {{
        try
        {{
            _logger.LogInformation(""Getting all {config.EntityName}s"");

            var entities = await _{config.EntityName.ToLowerInvariant()}Repository.GetAllActiveAsync(cancellationToken);
            var dtos = _mapper.Map<IEnumerable<{config.EntityName}Dto>>(entities);

            return ServiceResult<IEnumerable<{config.EntityName}Dto>>.Success(dtos);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Error getting all {config.EntityName}s"");
            return ServiceResult<IEnumerable<{config.EntityName}Dto>>.Failure($""An error occurred while retrieving {config.EntityName.ToLowerInvariant()}s"");
        }}
    }}

    public async Task<ServiceResult<{config.EntityName}Dto>> CreateAsync(Create{config.EntityName}Request request, CancellationToken cancellationToken = default)
    {{
        try
        {{
            _logger.LogInformation(""Creating new {config.EntityName}"");

            {(config.IncludeValidation ? $@"// Validate request
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {{
                return ServiceResult<{config.EntityName}Dto>.ValidationFailure(
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }}" : "")}

            var entity = _mapper.Map<{config.EntityName}>(request);
            var createdEntity = await _{config.EntityName.ToLowerInvariant()}Repository.AddAsync(entity, cancellationToken);
            var dto = _mapper.Map<{config.EntityName}Dto>(createdEntity);

            {(config.IncludeCaching ? $@"// Invalidate cache
            await _cacheService.RemovePatternAsync(""{config.EntityName.ToLowerInvariant()}:*"", cancellationToken);" : "")}

            _logger.LogInformation(""{config.EntityName} created with ID {{Id}}"", dto.Id);
            return ServiceResult<{config.EntityName}Dto>.Success(dto);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Error creating {config.EntityName}"");
            return ServiceResult<{config.EntityName}Dto>.Failure($""An error occurred while creating the {config.EntityName.ToLowerInvariant()}"");
        }}
    }}

    public async Task<ServiceResult<{config.EntityName}Dto>> UpdateAsync(int id, Update{config.EntityName}Request request, CancellationToken cancellationToken = default)
    {{
        try
        {{
            _logger.LogInformation(""Updating {config.EntityName} with ID {{Id}}"", id);

            {(config.IncludeValidation ? $@"// Validate request
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {{
                return ServiceResult<{config.EntityName}Dto>.ValidationFailure(
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }}" : "")}

            var existingEntity = await _{config.EntityName.ToLowerInvariant()}Repository.GetByIdAsync(id, cancellationToken);
            if (existingEntity == null)
            {{
                return ServiceResult<{config.EntityName}Dto>.Failure($""{config.EntityName} with ID {{id}} not found"");
            }}

            _mapper.Map(request, existingEntity);
            await _{config.EntityName.ToLowerInvariant()}Repository.UpdateAsync(existingEntity, cancellationToken);
            var dto = _mapper.Map<{config.EntityName}Dto>(existingEntity);

            {(config.IncludeCaching ? $@"// Update cache
            var cacheKey = $""{config.EntityName.ToLowerInvariant()}:{{id}}"";
            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15), cancellationToken);" : "")}

            _logger.LogInformation(""{config.EntityName} updated with ID {{Id}}"", id);
            return ServiceResult<{config.EntityName}Dto>.Success(dto);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Error updating {config.EntityName} with ID {{Id}}"", id);
            return ServiceResult<{config.EntityName}Dto>.Failure($""An error occurred while updating the {config.EntityName.ToLowerInvariant()}"");
        }}
    }}

    public async Task<ServiceResult<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {{
        try
        {{
            _logger.LogInformation(""Deleting {config.EntityName} with ID {{Id}}"", id);

            var exists = await _{config.EntityName.ToLowerInvariant()}Repository.ExistsAsync(id, cancellationToken);
            if (!exists)
            {{
                return ServiceResult<bool>.Failure($""{config.EntityName} with ID {{id}} not found"");
            }}

            await _{config.EntityName.ToLowerInvariant()}Repository.DeleteByIdAsync(id, cancellationToken);

            {(config.IncludeCaching ? $@"// Remove from cache
            var cacheKey = $""{config.EntityName.ToLowerInvariant()}:{{id}}"";
            await _cacheService.RemoveAsync(cacheKey, cancellationToken);" : "")}

            _logger.LogInformation(""{config.EntityName} deleted with ID {{Id}}"", id);
            return ServiceResult<bool>.Success(true);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Error deleting {config.EntityName} with ID {{Id}}"", id);
            return ServiceResult<bool>.Failure($""An error occurred while deleting the {config.EntityName.ToLowerInvariant()}"");
        }}
    }}

    // Additional business methods
    public async Task<ServiceResult<{config.EntityName}Dto>> ActivateAsync(int id, CancellationToken cancellationToken = default)
    {{
        return await UpdateStatusAsync(id, true, ""activated"", cancellationToken);
    }}

    public async Task<ServiceResult<{config.EntityName}Dto>> DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {{
        return await UpdateStatusAsync(id, false, ""deactivated"", cancellationToken);
    }}

    private async Task<ServiceResult<{config.EntityName}Dto>> UpdateStatusAsync(int id, bool isActive, string action, CancellationToken cancellationToken)
    {{
        try
        {{
            var entity = await _{config.EntityName.ToLowerInvariant()}Repository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {{
                return ServiceResult<{config.EntityName}Dto>.Failure($""{config.EntityName} with ID {{id}} not found"");
            }}

            entity.IsActive = isActive;
            entity.UpdatedAt = DateTime.UtcNow;
            
            await _{config.EntityName.ToLowerInvariant()}Repository.UpdateAsync(entity, cancellationToken);
            var dto = _mapper.Map<{config.EntityName}Dto>(entity);

            _logger.LogInformation(""{config.EntityName} {{Action}} with ID {{Id}}"", action, id);
            return ServiceResult<{config.EntityName}Dto>.Success(dto);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Error {{Action}} {config.EntityName} with ID {{Id}}"", action, id);
            return ServiceResult<{config.EntityName}Dto>.Failure($""An error occurred while {{action}} the {config.EntityName.ToLowerInvariant()}"");
        }}
    }}
}}";
    }

    private static string GenerateDataTransferObjects(ServiceConfiguration config)
    {
        return $@"// Read DTO
public class {config.EntityName}Dto
{{
    public int Id {{ get; set; }}
    public string Name {{ get; set; }} = string.Empty;
    public string Description {{ get; set; }} = string.Empty;
    public DateTime CreatedAt {{ get; set; }}
    public DateTime? UpdatedAt {{ get; set; }}
    public string CreatedBy {{ get; set; }} = string.Empty;
    public string? UpdatedBy {{ get; set; }}
    public bool IsActive {{ get; set; }}
}}

// Create request DTO
public class Create{config.EntityName}Request
{{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name {{ get; set; }} = string.Empty;
    
    [StringLength(500)]
    public string Description {{ get; set; }} = string.Empty;
    
    [Required]
    public string CreatedBy {{ get; set; }} = string.Empty;
}}

// Update request DTO
public class Update{config.EntityName}Request
{{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name {{ get; set; }} = string.Empty;
    
    [StringLength(500)]
    public string Description {{ get; set; }} = string.Empty;
    
    [Required]
    public string UpdatedBy {{ get; set; }} = string.Empty;
}}

// AutoMapper Profile
public class {config.EntityName}MappingProfile : Profile
{{
    public {config.EntityName}MappingProfile()
    {{
        CreateMap<{config.EntityName}, {config.EntityName}Dto>();
        CreateMap<Create{config.EntityName}Request, {config.EntityName}>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            
        CreateMap<Update{config.EntityName}Request, {config.EntityName}>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());
    }}
}}";
    }

    private static string GenerateValidationCode(ServiceConfiguration config)
    {
        return $@"// FluentValidation validators
public class Create{config.EntityName}RequestValidator : AbstractValidator<Create{config.EntityName}Request>
{{
    public Create{config.EntityName}RequestValidator()
    {{
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(""Name is required"")
            .Length(1, 100).WithMessage(""Name must be between 1 and 100 characters"")
            .MustAsync(BeUniqueName).WithMessage(""Name must be unique"");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage(""Description cannot exceed 500 characters"");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage(""CreatedBy is required"")
            .MaximumLength(50).WithMessage(""CreatedBy cannot exceed 50 characters"");
    }}

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {{
        // Implement uniqueness check against repository
        // In a real implementation, this would query the database
        
        await Task.Yield(); // Allow for async behavior
        
        if (string.IsNullOrWhiteSpace(name))
            return false;
            
        // Mock implementation - in production, replace with actual repository call
        // Example: return !await _repository.ExistsAsync(x => x.Name.ToLower() == name.ToLower(), cancellationToken);
        
        // Simulate common reserved names that should not be allowed
        var reservedNames = new[] {{ ""admin"", ""system"", ""root"", ""administrator"", ""guest"", ""null"", ""default"" }};
        return !reservedNames.Contains(name.ToLowerInvariant());
    }}
}}

public class Update{config.EntityName}RequestValidator : AbstractValidator<Update{config.EntityName}Request>
{{
    public Update{config.EntityName}RequestValidator()
    {{
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(""Name is required"")
            .Length(1, 100).WithMessage(""Name must be between 1 and 100 characters"");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage(""Description cannot exceed 500 characters"");

        RuleFor(x => x.UpdatedBy)
            .NotEmpty().WithMessage(""UpdatedBy is required"")
            .MaximumLength(50).WithMessage(""UpdatedBy cannot exceed 50 characters"");
    }}
}}

// Service registration for validation
public static class ValidationServiceExtensions
{{
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {{
        services.AddScoped<IValidator<Create{config.EntityName}Request>, Create{config.EntityName}RequestValidator>();
        services.AddScoped<IValidator<Update{config.EntityName}Request>, Update{config.EntityName}RequestValidator>();
        
        return services;
    }}
}}";
    }

    private static string GenerateDomainServiceCode(DomainServiceConfiguration config)
    {
        return $@"// Domain service for {config.BusinessDomain}
public interface I{config.ServiceName}
{{
    Task<DomainResult> ExecuteBusinessOperationAsync(BusinessOperationRequest request, CancellationToken cancellationToken = default);
    Task<DomainResult<T>> ProcessDomainLogicAsync<T>(DomainProcessRequest<T> request, CancellationToken cancellationToken = default);
    Task<bool> ValidateBusinessRulesAsync(object domainObject, CancellationToken cancellationToken = default);
}}

public class {config.ServiceName} : I{config.ServiceName}
{{
    private readonly ILogger<{config.ServiceName}> _logger;
    {(config.IncludeRulesEngine ? "private readonly IBusinessRulesEngine _rulesEngine;" : "")}
    {(config.IncludeDomainEvents ? "private readonly IDomainEventPublisher _eventPublisher;" : "")}

    public {config.ServiceName}(
        ILogger<{config.ServiceName}> logger
        {(config.IncludeRulesEngine ? ", IBusinessRulesEngine rulesEngine" : "")}
        {(config.IncludeDomainEvents ? ", IDomainEventPublisher eventPublisher" : "")})
    {{
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        {(config.IncludeRulesEngine ? "_rulesEngine = rulesEngine ?? throw new ArgumentNullException(nameof(rulesEngine));" : "")}
        {(config.IncludeDomainEvents ? "_eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));" : "")}
    }}

    public async Task<DomainResult> ExecuteBusinessOperationAsync(BusinessOperationRequest request, CancellationToken cancellationToken = default)
    {{
        try
        {{
            _logger.LogInformation(""Executing business operation: {{OperationType}}"", request.OperationType);

            // Validate business rules
            {(config.IncludeRulesEngine ? @"var rulesValid = await _rulesEngine.ValidateAsync(request, cancellationToken);
            if (!rulesValid.IsValid)
            {
                return DomainResult.Failure(rulesValid.Errors);
            }" : @"var isValid = await ValidateBusinessRulesAsync(request, cancellationToken);
            if (!isValid)
            {
                return DomainResult.Failure(""Business rules validation failed"");
            }")}

            // Execute core business logic
            var result = await ProcessBusinessLogicAsync(request, cancellationToken);

            {(config.IncludeDomainEvents ? @"// Publish domain events
            if (result.IsSuccess && result.DomainEvents.Any())
            {
                foreach (var domainEvent in result.DomainEvents)
                {
                    await _eventPublisher.PublishAsync(domainEvent, cancellationToken);
                }
            }" : "")}

            _logger.LogInformation(""Business operation completed: {{OperationType}}"", request.OperationType);
            return result;
        }}
        catch (DomainException ex)
        {{
            _logger.LogWarning(ex, ""Domain exception in business operation: {{OperationType}}"", request.OperationType);
            return DomainResult.Failure(ex.Message);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Error executing business operation: {{OperationType}}"", request.OperationType);
            return DomainResult.Failure(""An unexpected error occurred"");
        }}
    }}

    public async Task<DomainResult<T>> ProcessDomainLogicAsync<T>(DomainProcessRequest<T> request, CancellationToken cancellationToken = default)
    {{
        // Implement specific domain logic processing
        // This is where complex business rules and workflows are handled
        
        _logger.LogInformation(""Processing domain logic for type {{Type}}"", typeof(T).Name);
        
        // Example domain logic processing
        var result = await ApplyDomainRulesAsync(request.Data, cancellationToken);
        
        return DomainResult<T>.Success(result);
    }}

    public async Task<bool> ValidateBusinessRulesAsync(object domainObject, CancellationToken cancellationToken = default)
    {{
        // Implement business rules validation
        // This could include complex multi-entity validations, business constraints, etc.
        
        {(config.IncludeRulesEngine ? @"var validationResult = await _rulesEngine.ValidateAsync(domainObject, cancellationToken);
        return validationResult.IsValid;" : @"// Example validation logic
        if (domainObject == null)
            return false;
            
        // Add specific business rule validations here
        return true;")}
    }}

    private async Task<DomainResult> ProcessBusinessLogicAsync(BusinessOperationRequest request, CancellationToken cancellationToken)
    {{
        try
        {{
            // Core business logic implementation
            // This is where the main business operations are performed
            
            // Step 1: Validate business rules
            var validationResult = await ValidateBusinessRulesAsync(request, cancellationToken);
            if (!validationResult.IsSuccess)
                return validationResult;
            
            // Step 2: Process the operation based on type
            switch (request.OperationType.ToLowerInvariant())
            {{
                case ""create"":
                    return await ProcessCreateOperationAsync(request, cancellationToken);
                case ""update"":
                    return await ProcessUpdateOperationAsync(request, cancellationToken);
                case ""delete"":
                    return await ProcessDeleteOperationAsync(request, cancellationToken);
                case ""validate"":
                    return await ProcessValidationOperationAsync(request, cancellationToken);
                default:
                    return DomainResult.Failure($""Unknown operation type: {{request.OperationType}}"");
            }}
        }}
        catch (OperationCanceledException)
        {{
            return DomainResult.Failure(""Operation was cancelled"");
        }}
        catch (Exception ex)
        {{
            return DomainResult.Failure($""Business logic processing failed: {{ex.Message}}"");
        }}
    }}
    
    private async Task<DomainResult> ValidateBusinessRulesAsync(BusinessOperationRequest request, CancellationToken cancellationToken)
    {{
        await Task.Yield();
        
        // Example business rule validations
        if (string.IsNullOrWhiteSpace(request.OperationType))
            return DomainResult.Failure(""Operation type is required"");
            
        if (request.Data == null)
            return DomainResult.Failure(""Operation data is required"");
            
        return DomainResult.Success();
    }}
    
    private async Task<DomainResult> ProcessCreateOperationAsync(BusinessOperationRequest request, CancellationToken cancellationToken)
    {{
        await Task.Yield();
        // Mock create operation - in production, implement actual create logic
        return DomainResult.Success(""Entity created successfully"");
    }}
    
    private async Task<DomainResult> ProcessUpdateOperationAsync(BusinessOperationRequest request, CancellationToken cancellationToken)
    {{
        await Task.Yield();
        // Mock update operation - in production, implement actual update logic
        return DomainResult.Success(""Entity updated successfully"");
    }}
    
    private async Task<DomainResult> ProcessDeleteOperationAsync(BusinessOperationRequest request, CancellationToken cancellationToken)
    {{
        await Task.Yield();
        // Mock delete operation - in production, implement actual delete logic
        return DomainResult.Success(""Entity deleted successfully"");
    }}
    
    private async Task<DomainResult> ProcessValidationOperationAsync(BusinessOperationRequest request, CancellationToken cancellationToken)
    {{
        await Task.Yield();
        // Mock validation operation - in production, implement actual validation logic
        return DomainResult.Success(""Validation completed successfully"");
    }}

    private async Task<T> ApplyDomainRulesAsync<T>(T data, CancellationToken cancellationToken)
    {{
        await Task.Yield(); // Allow for async behavior
        
        if (data == null)
            return data;
            
        try
        {{
            // Apply domain-specific rules and transformations
            switch (data)
            {{
                case string stringData:
                    // Apply string-specific domain rules
                    var processedString = ApplyStringDomainRules(stringData);
                    return (T)(object)processedString;
                    
                case Dictionary<string, object> dictData:
                    // Apply dictionary-specific domain rules
                    var processedDict = ApplyDictionaryDomainRules(dictData);
                    return (T)(object)processedDict;
                    
                case IEnumerable<object> listData:
                    // Apply list-specific domain rules
                    var processedList = ApplyListDomainRules(listData);
                    return (T)(object)processedList;
                    
                default:
                    // For other types, apply generic transformations
                    return ApplyGenericDomainRules(data);
            }}
        }}
        catch (Exception)
        {{
            // If transformation fails, return original data
            return data;
        }}
    }}
    
    private string ApplyStringDomainRules(string input)
    {{
        if (string.IsNullOrWhiteSpace(input))
            return input;
            
        // Example domain rules for strings
        return input.Trim() // Remove leading/trailing whitespace
                   .Replace(""  "", "" ""); // Replace multiple spaces with single space
    }}
    
    private Dictionary<string, object> ApplyDictionaryDomainRules(Dictionary<string, object> input)
    {{
        var result = new Dictionary<string, object>();
        
        foreach (var kvp in input)
        {{
            // Example domain rules for dictionaries
            var key = kvp.Key?.Trim()?.ToLowerInvariant() ?? string.Empty;
            if (!string.IsNullOrEmpty(key))
            {{
                result[key] = kvp.Value;
            }}
        }}
        
        return result;
    }}
    
    private IEnumerable<object> ApplyListDomainRules(IEnumerable<object> input)
    {{
        // Example domain rules for lists
        return input.Where(item => item != null) // Remove null items
                   .Distinct() // Remove duplicates
                   .ToList();
    }}
    
    private T ApplyGenericDomainRules<T>(T input)
    {{
        // Apply generic domain rules for unknown types
        // In a real implementation, this might involve reflection or type-specific logic
        return input;
    }}
}}

// Domain result types
public class DomainResult
{{
    public bool IsSuccess {{ get; protected set; }}
    public string Error {{ get; protected set; }} = string.Empty;
    public List<string> Errors {{ get; protected set; }} = new();
    {(config.IncludeDomainEvents ? "public List<IDomainEvent> DomainEvents { get; protected set; } = new();" : "")}

    protected DomainResult() {{ }}

    public static DomainResult Success()
    {{
        return new DomainResult {{ IsSuccess = true }};
    }}

    public static DomainResult Failure(string error)
    {{
        return new DomainResult {{ IsSuccess = false, Error = error }};
    }}

    public static DomainResult Failure(List<string> errors)
    {{
        return new DomainResult {{ IsSuccess = false, Errors = errors }};
    }}
}}

public class DomainResult<T> : DomainResult
{{
    public T? Data {{ get; private set; }}

    private DomainResult() {{ }}

    public static DomainResult<T> Success(T data)
    {{
        return new DomainResult<T> {{ IsSuccess = true, Data = data }};
    }}

    public new static DomainResult<T> Failure(string error)
    {{
        return new DomainResult<T> {{ IsSuccess = false, Error = error }};
    }}
}}";
    }

    private static string GenerateBusinessRulesEngine(DomainServiceConfiguration config)
    {
        return $@"// Business Rules Engine
public interface IBusinessRulesEngine
{{
    Task<RuleValidationResult> ValidateAsync(object target, CancellationToken cancellationToken = default);
    Task<RuleExecutionResult> ExecuteRulesAsync(object target, string ruleSet = ""default"", CancellationToken cancellationToken = default);
    void RegisterRule<T>(IBusinessRule<T> rule);
    void RegisterRuleSet(string name, IEnumerable<IBusinessRule> rules);
}}

public class BusinessRulesEngine : IBusinessRulesEngine
{{
    private readonly Dictionary<Type, List<IBusinessRule>> _rules = new();
    private readonly Dictionary<string, List<IBusinessRule>> _ruleSets = new();
    private readonly ILogger<BusinessRulesEngine> _logger;

    public BusinessRulesEngine(ILogger<BusinessRulesEngine> logger)
    {{
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }}

    public async Task<RuleValidationResult> ValidateAsync(object target, CancellationToken cancellationToken = default)
    {{
        if (target == null)
            return RuleValidationResult.Invalid(""Target object is null"");

        var targetType = target.GetType();
        var result = new RuleValidationResult();

        if (_rules.TryGetValue(targetType, out var rules))
        {{
            foreach (var rule in rules)
            {{
                var ruleResult = await rule.ValidateAsync(target, cancellationToken);
                result.AddResult(ruleResult);
            }}
        }}

        return result;
    }}

    public async Task<RuleExecutionResult> ExecuteRulesAsync(object target, string ruleSet = ""default"", CancellationToken cancellationToken = default)
    {{
        var result = new RuleExecutionResult();

        if (_ruleSets.TryGetValue(ruleSet, out var rules))
        {{
            foreach (var rule in rules)
            {{
                var ruleResult = await rule.ExecuteAsync(target, cancellationToken);
                result.AddResult(ruleResult);
            }}
        }}

        return result;
    }}

    public void RegisterRule<T>(IBusinessRule<T> rule)
    {{
        var targetType = typeof(T);
        if (!_rules.ContainsKey(targetType))
        {{
            _rules[targetType] = new List<IBusinessRule>();
        }}
        _rules[targetType].Add(rule);
    }}

    public void RegisterRuleSet(string name, IEnumerable<IBusinessRule> rules)
    {{
        _ruleSets[name] = rules.ToList();
    }}
}}

// Business rule interfaces
public interface IBusinessRule
{{
    string Name {{ get; }}
    string Description {{ get; }}
    Task<BusinessRuleResult> ValidateAsync(object target, CancellationToken cancellationToken = default);
    Task<BusinessRuleResult> ExecuteAsync(object target, CancellationToken cancellationToken = default);
}}

public interface IBusinessRule<T> : IBusinessRule
{{
    Task<BusinessRuleResult> ValidateAsync(T target, CancellationToken cancellationToken = default);
    Task<BusinessRuleResult> ExecuteAsync(T target, CancellationToken cancellationToken = default);
}}

// Example business rule
public class SampleBusinessRule : IBusinessRule<BusinessOperationRequest>
{{
    public string Name => ""Sample Business Rule"";
    public string Description => ""Validates sample business conditions"";

    public async Task<BusinessRuleResult> ValidateAsync(BusinessOperationRequest target, CancellationToken cancellationToken = default)
    {{
        await Task.Delay(1, cancellationToken);
        
        // Implement specific validation logic
        if (string.IsNullOrEmpty(target.OperationType))
        {{
            return BusinessRuleResult.Invalid(""Operation type is required"");
        }}

        return BusinessRuleResult.Valid();
    }}

    public async Task<BusinessRuleResult> ExecuteAsync(BusinessOperationRequest target, CancellationToken cancellationToken = default)
    {{
        await Task.Delay(1, cancellationToken);
        
        // Implement rule execution logic
        return BusinessRuleResult.Executed(""Rule executed successfully"");
    }}

    // Implement non-generic interface methods
    public async Task<BusinessRuleResult> ValidateAsync(object target, CancellationToken cancellationToken = default)
    {{
        if (target is BusinessOperationRequest request)
            return await ValidateAsync(request, cancellationToken);
        
        return BusinessRuleResult.Invalid(""Invalid target type"");
    }}

    public async Task<BusinessRuleResult> ExecuteAsync(object target, CancellationToken cancellationToken = default)
    {{
        if (target is BusinessOperationRequest request)
            return await ExecuteAsync(request, cancellationToken);
        
        return BusinessRuleResult.Invalid(""Invalid target type"");
    }}
}}";
    }

    private static string GenerateDomainEventsCode(DomainServiceConfiguration config)
    {
        return $@"// Domain Events
public interface IDomainEvent
{{
    Guid Id {{ get; }}
    DateTime OccurredOn {{ get; }}
    string EventType {{ get; }}
}}

public interface IDomainEventPublisher
{{
    Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : IDomainEvent;
    Task PublishAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}}

public interface IDomainEventHandler<T> where T : IDomainEvent
{{
    Task HandleAsync(T domainEvent, CancellationToken cancellationToken = default);
}}

// Base domain event
public abstract class DomainEvent : IDomainEvent
{{
    public Guid Id {{ get; }} = Guid.NewGuid();
    public DateTime OccurredOn {{ get; }} = DateTime.UtcNow;
    public abstract string EventType {{ get; }}
}}

// Example domain events for {config.BusinessDomain}
public class BusinessOperationStartedEvent : DomainEvent
{{
    public override string EventType => nameof(BusinessOperationStartedEvent);
    public string OperationType {{ get; set; }} = string.Empty;
    public string InitiatedBy {{ get; set; }} = string.Empty;
    public Dictionary<string, object> Context {{ get; set; }} = new();
}}

public class BusinessOperationCompletedEvent : DomainEvent
{{
    public override string EventType => nameof(BusinessOperationCompletedEvent);
    public string OperationType {{ get; set; }} = string.Empty;
    public bool IsSuccessful {{ get; set; }}
    public TimeSpan Duration {{ get; set; }}
    public Dictionary<string, object> Results {{ get; set; }} = new();
}}

public class BusinessRuleViolatedEvent : DomainEvent
{{
    public override string EventType => nameof(BusinessRuleViolatedEvent);
    public string RuleName {{ get; set; }} = string.Empty;
    public string ViolationDescription {{ get; set; }} = string.Empty;
    public object TargetObject {{ get; set; }} = new();
}}

// Domain event publisher implementation
public class DomainEventPublisher : IDomainEventPublisher
{{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainEventPublisher> _logger;

    public DomainEventPublisher(IServiceProvider serviceProvider, ILogger<DomainEventPublisher> logger)
    {{
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }}

    public async Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : IDomainEvent
    {{
        try
        {{
            _logger.LogInformation(""Publishing domain event: {{EventType}}"", domainEvent.EventType);

            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(typeof(T));
            var handlers = _serviceProvider.GetServices(handlerType);

            var tasks = handlers.Cast<IDomainEventHandler<T>>()
                               .Select(handler => handler.HandleAsync(domainEvent, cancellationToken));

            await Task.WhenAll(tasks);

            _logger.LogInformation(""Domain event published successfully: {{EventType}}"", domainEvent.EventType);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Error publishing domain event: {{EventType}}"", domainEvent.EventType);
            throw;
        }}
    }}

    public async Task PublishAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {{
        var tasks = domainEvents.Select(evt => PublishAsync(evt, cancellationToken));
        await Task.WhenAll(tasks);
    }}
}}

// Example domain event handler
public class BusinessOperationEventHandler : 
    IDomainEventHandler<BusinessOperationStartedEvent>,
    IDomainEventHandler<BusinessOperationCompletedEvent>
{{
    private readonly ILogger<BusinessOperationEventHandler> _logger;

    public BusinessOperationEventHandler(ILogger<BusinessOperationEventHandler> logger)
    {{
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }}

    public async Task HandleAsync(BusinessOperationStartedEvent domainEvent, CancellationToken cancellationToken = default)
    {{
        _logger.LogInformation(""Business operation started: {{OperationType}} by {{User}}"", 
            domainEvent.OperationType, domainEvent.InitiatedBy);

        // Handle the event (e.g., send notifications, update metrics, etc.)
        await Task.CompletedTask;
    }}

    public async Task HandleAsync(BusinessOperationCompletedEvent domainEvent, CancellationToken cancellationToken = default)
    {{
        _logger.LogInformation(""Business operation completed: {{OperationType}}, Success: {{Success}}, Duration: {{Duration}}"", 
            domainEvent.OperationType, domainEvent.IsSuccessful, domainEvent.Duration);

        // Handle the event (e.g., update analytics, trigger workflows, etc.)
        await Task.CompletedTask;
    }}
}}";
    }
}