using System.ComponentModel;

using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Tools;

[McpServerToolType]
public static class ArchitectureTemplateTool
{
    [McpServerTool, Description("Generates advanced MVVM architecture templates with dependency injection and reactive patterns")]
    public static string GenerateMvvmArchitecture(
        [Description("Project name")] string projectName,
        [Description("Architecture pattern: 'clean', 'onion', 'hexagonal', 'layered'")] string architecturePattern = "clean",
        [Description("Include dependency injection: 'true' or 'false'")] string includeDI = "true",
        [Description("Include reactive extensions: 'true' or 'false'")] string includeReactive = "true",
        [Description("Include validation: 'true' or 'false'")] string includeValidation = "true")
    {
        try
        {
            var config = new ArchitectureConfiguration
            {
                ProjectName = projectName,
                Pattern = architecturePattern.ToLowerInvariant(),
                IncludeDI = bool.Parse(includeDI),
                IncludeReactive = bool.Parse(includeReactive),
                IncludeValidation = bool.Parse(includeValidation)
            };

            string projectStructure = GenerateProjectStructure(config);
            string baseClasses = GenerateBaseClasses(config);
            string serviceLayer = GenerateServiceLayer(config);
            string viewModels = GenerateViewModelTemplates(config);
            string views = GenerateViewTemplates(config);

            return $@"# Advanced MVVM Architecture: {projectName}

## Architecture Pattern: {architecturePattern}

### Project Structure
```
{projectStructure}
```

## Core Architecture Components

### 1. Base Classes
```csharp
{baseClasses}
```

### 2. Service Layer
```csharp
{serviceLayer}
```

### 3. ViewModel Templates
```csharp
{viewModels}
```

### 4. View Templates
```xml
{views}
```

## Architecture Benefits

### {architecturePattern} Architecture
{GetArchitectureDescription(architecturePattern)}

### Design Principles
- **Separation of Concerns**: Clear separation between UI, business logic, and data
- **Dependency Inversion**: High-level modules don't depend on low-level modules
- **Single Responsibility**: Each class has one reason to change
- **Open/Closed Principle**: Open for extension, closed for modification

### Reactive Programming
{(config.IncludeReactive ? @"- **Observable Streams**: Use ReactiveUI for property changes and commands
- **Asynchronous Operations**: Handle async operations with reactive patterns
- **Event Composition**: Compose complex event flows declaratively
- **Memory Management**: Automatic subscription management" : "// Reactive programming not included")}

### Dependency Injection
{(config.IncludeDI ? @"- **IoC Container**: Microsoft.Extensions.DependencyInjection integration
- **Service Registration**: Automatic service discovery and registration
- **Lifetime Management**: Proper service lifetime management
- **Testability**: Easy mocking and testing" : "// Dependency injection not included")}

## Implementation Guidelines

### 1. ViewModels
- Inherit from `ViewModelBase` for common functionality
- Use `ReactiveCommand` for user actions
- Implement `IValidatableObject` for validation
- Follow reactive property patterns

### 2. Services
- Define interfaces for all services
- Implement repository pattern for data access
- Use async/await for I/O operations
- Handle errors gracefully

### 3. Views
- Use MVVM binding patterns
- Minimize code-behind
- Implement proper data templates
- Follow accessibility guidelines

### 4. Models
- Keep models simple and focused
- Implement `INotifyPropertyChanged` when needed
- Use data annotations for validation
- Separate DTOs from domain models";
        }
        catch (Exception ex)
        {
            return $"Error generating MVVM architecture: {ex.Message}";
        }
    }

    [McpServerTool, Description("Creates microservices architecture templates for distributed AvaloniaUI applications")]
    public static string GenerateMicroservicesArchitecture(
        [Description("Application name")] string applicationName,
        [Description("Services to generate (comma-separated, e.g., 'user,order,inventory')")] string services,
        [Description("Communication pattern: 'rest', 'grpc', 'messagebus'")] string communicationPattern = "rest",
        [Description("Include API gateway: 'true' or 'false'")] string includeGateway = "true",
        [Description("Include event sourcing: 'true' or 'false'")] string includeEventSourcing = "false")
    {
        try
        {
            var serviceList = services.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
            bool gateway = bool.Parse(includeGateway);
            bool eventSourcing = bool.Parse(includeEventSourcing);

            string architecture = GenerateMicroservicesStructure(applicationName, serviceList, communicationPattern, gateway, eventSourcing);
            string serviceTemplates = GenerateServiceTemplates(serviceList, communicationPattern);
            string clientIntegration = GenerateClientIntegration(communicationPattern);

            return $@"# Microservices Architecture: {applicationName}

## Architecture Overview
- **Services**: {string.Join(", ", serviceList)}
- **Communication**: {communicationPattern}
- **API Gateway**: {gateway}
- **Event Sourcing**: {eventSourcing}

## Project Structure
```
{architecture}
```

## Service Templates
{serviceTemplates}

## Client Integration
```csharp
{clientIntegration}
```

## Communication Patterns

### {communicationPattern.ToUpperInvariant()} Communication
{GetCommunicationDescription(communicationPattern)}

### Service Discovery
- **Consul**: Service registration and discovery
- **Health Checks**: Endpoint monitoring
- **Load Balancing**: Automatic load distribution
- **Circuit Breaker**: Fault tolerance patterns

### Event-Driven Architecture
{(eventSourcing ? @"- **Event Store**: Immutable event log
- **Event Sourcing**: Rebuild state from events
- **CQRS**: Command Query Responsibility Segregation
- **Saga Pattern**: Distributed transaction management" : "// Event sourcing not included")}

## Cross-Cutting Concerns

### Logging and Monitoring
- **Structured Logging**: JSON-formatted logs
- **Distributed Tracing**: Request correlation
- **Metrics Collection**: Performance monitoring
- **Health Endpoints**: Service health checks

### Security
- **JWT Authentication**: Token-based security
- **OAuth 2.0**: Authorization framework
- **API Rate Limiting**: Prevent abuse
- **Input Validation**: Secure data handling

### Configuration
- **Environment Variables**: Runtime configuration
- **Configuration Server**: Centralized config management
- **Feature Flags**: Dynamic feature control
- **Secrets Management**: Secure credential storage";
        }
        catch (Exception ex)
        {
            return $"Error generating microservices architecture: {ex.Message}";
        }
    }

    [McpServerTool, Description("Generates Domain-Driven Design (DDD) templates with bounded contexts and aggregates")]
    public static string GenerateDDDArchitecture(
        [Description("Domain name")] string domainName,
        [Description("Bounded contexts (comma-separated, e.g., 'sales,inventory,shipping')")] string boundedContexts,
        [Description("Include CQRS: 'true' or 'false'")] string includeCQRS = "true",
        [Description("Include event sourcing: 'true' or 'false'")] string includeEventSourcing = "false",
        [Description("Include domain events: 'true' or 'false'")] string includeDomainEvents = "true")
    {
        try
        {
            var contexts = boundedContexts.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
            bool cqrs = bool.Parse(includeCQRS);
            bool eventSourcing = bool.Parse(includeEventSourcing);
            bool domainEvents = bool.Parse(includeDomainEvents);

            string dddStructure = GenerateDDDStructure(domainName, contexts, cqrs, eventSourcing, domainEvents);
            string domainLayer = GenerateDomainLayer(contexts.First(), domainEvents);
            string applicationLayer = GenerateApplicationLayer(contexts.First(), cqrs);
            string infrastructureLayer = GenerateInfrastructureLayer(contexts.First(), eventSourcing);

            return $@"# Domain-Driven Design Architecture: {domainName}

## Domain Model
- **Bounded Contexts**: {string.Join(", ", contexts)}
- **CQRS**: {cqrs}
- **Event Sourcing**: {eventSourcing}
- **Domain Events**: {domainEvents}

## Project Structure
```
{dddStructure}
```

## Domain Layer
```csharp
{domainLayer}
```

## Application Layer
```csharp
{applicationLayer}
```

## Infrastructure Layer
```csharp
{infrastructureLayer}
```

## DDD Patterns Implementation

### Aggregates
- **Aggregate Root**: Single entry point for cluster of entities
- **Entity Identity**: Unique identification for entities
- **Value Objects**: Immutable objects with no identity
- **Invariants**: Business rules that must always be true

### Domain Services
- **Domain Logic**: Complex business operations
- **Coordination**: Orchestrate multiple aggregates
- **Policies**: Implement business policies
- **Specifications**: Encapsulate business rules

### Repositories
- **Aggregate Persistence**: Store and retrieve aggregates
- **Unit of Work**: Maintain consistency across operations
- **Specifications**: Query criteria encapsulation
- **Domain Events**: Publish side effects

## Strategic Design

### Bounded Context Mapping
{string.Join("\n", contexts.Select(context => $"- **{context} Context**: Responsible for {context} domain logic and data"))}

### Context Integration Patterns
- **Shared Kernel**: Common domain concepts
- **Customer/Supplier**: Upstream/downstream relationship
- **Conformist**: Adapt to external systems
- **Anti-Corruption Layer**: Protect domain model

### Ubiquitous Language
- **Domain Vocabulary**: Consistent terminology
- **Model Alignment**: Code reflects business language
- **Communication**: Bridge between technical and business teams
- **Documentation**: Living documentation through code";
        }
        catch (Exception ex)
        {
            return $"Error generating DDD architecture: {ex.Message}";
        }
    }

    [McpServerTool, Description("Creates plugin architecture templates with extensible module system")]
    public static string GeneratePluginArchitecture(
        [Description("Application name")] string applicationName,
        [Description("Plugin types (comma-separated, e.g., 'editor,filter,exporter')")] string pluginTypes,
        [Description("Loading strategy: 'static', 'dynamic', 'lazy'")] string loadingStrategy = "dynamic",
        [Description("Include MEF: 'true' or 'false'")] string includeMEF = "true",
        [Description("Include hot-reload: 'true' or 'false'")] string includeHotReload = "false")
    {
        try
        {
            var types = pluginTypes.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
            bool mef = bool.Parse(includeMEF);
            bool hotReload = bool.Parse(includeHotReload);

            string pluginStructure = GeneratePluginStructure(applicationName, types, loadingStrategy, mef, hotReload);
            string pluginInterface = GeneratePluginInterfaces(types);
            string pluginHost = GeneratePluginHost(loadingStrategy, mef, hotReload);
            string samplePlugin = GenerateSamplePlugin(types.First());

            return $@"# Plugin Architecture: {applicationName}

## Plugin System Configuration
- **Plugin Types**: {string.Join(", ", types)}
- **Loading Strategy**: {loadingStrategy}
- **MEF Integration**: {mef}
- **Hot Reload**: {hotReload}

## Project Structure
```
{pluginStructure}
```

## Plugin Interfaces
```csharp
{pluginInterface}
```

## Plugin Host System
```csharp
{pluginHost}
```

## Sample Plugin Implementation
```csharp
{samplePlugin}
```

## Plugin Development Guidelines

### Plugin Contract
- **Stable Interface**: Define clear plugin contracts
- **Versioning**: Handle plugin version compatibility
- **Metadata**: Provide plugin information and capabilities
- **Dependencies**: Manage plugin dependencies

### Loading Strategies
#### {loadingStrategy} Loading
{GetLoadingStrategyDescription(loadingStrategy)}

### Plugin Discovery
- **Directory Scanning**: Automatic plugin discovery
- **Manifest Files**: Plugin metadata and configuration
- **Digital Signatures**: Plugin authenticity verification
- **Sandbox Security**: Isolated plugin execution

### Communication Patterns
- **Event Bus**: Loose coupling through events
- **Service Locator**: Centralized service discovery
- **Dependency Injection**: Service composition
- **Message Passing**: Async communication

## Security Considerations

### Plugin Isolation
- **AppDomain Separation**: Runtime isolation
- **Permission Sets**: Limited plugin permissions
- **Code Access Security**: Controlled resource access
- **Assembly Loading**: Secure assembly resolution

### Trust Management
- **Code Signing**: Verify plugin authenticity
- **Permission Policies**: Define security policies
- **Runtime Verification**: Check plugin behavior
- **Audit Logging**: Track plugin activities

## Performance Optimization

### Loading Performance
- **Lazy Loading**: Load plugins on demand
- **Caching**: Cache plugin metadata
- **Parallel Loading**: Concurrent plugin initialization
- **Memory Management**: Efficient resource usage

### Runtime Performance
- **Interface Optimization**: Minimal overhead interfaces
- **Batch Operations**: Reduce cross-boundary calls
- **Connection Pooling**: Reuse expensive resources
- **Profiling Hooks**: Monitor plugin performance";
        }
        catch (Exception ex)
        {
            return $"Error generating plugin architecture: {ex.Message}";
        }
    }

    private sealed class ArchitectureConfiguration
    {
        public string ProjectName { get; set; } = "";
        public string Pattern { get; set; } = "";
        public bool IncludeDI { get; set; }
        public bool IncludeReactive { get; set; }
        public bool IncludeValidation { get; set; }
    }

    private static string GenerateProjectStructure(ArchitectureConfiguration config)
    {
        return config.Pattern switch
        {
            "clean" => GenerateCleanArchitectureStructure(config),
            "onion" => GenerateOnionArchitectureStructure(config),
            "hexagonal" => GenerateHexagonalArchitectureStructure(config),
            "layered" => GenerateLayeredArchitectureStructure(config),
            _ => GenerateCleanArchitectureStructure(config)
        };
    }

    private static string GenerateCleanArchitectureStructure(ArchitectureConfiguration config)
    {
        return $@"{config.ProjectName}/
├── src/
│   ├── {config.ProjectName}.Domain/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Aggregates/
│   │   ├── Interfaces/
│   │   └── Events/
│   ├── {config.ProjectName}.Application/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Handlers/
│   │   ├── Services/
│   │   ├── Interfaces/
│   │   └── DTOs/
│   ├── {config.ProjectName}.Infrastructure/
│   │   ├── Persistence/
│   │   ├── External/
│   │   ├── Messaging/
│   │   └── Configuration/
│   └── {config.ProjectName}.Presentation/
│       ├── Views/
│       ├── ViewModels/
│       ├── Controls/
│       ├── Converters/
│       └── Services/
└── tests/
    ├── {config.ProjectName}.Domain.Tests/
    ├── {config.ProjectName}.Application.Tests/
    ├── {config.ProjectName}.Infrastructure.Tests/
    └── {config.ProjectName}.Presentation.Tests/";
    }

    private static string GenerateOnionArchitectureStructure(ArchitectureConfiguration config)
    {
        return $@"{config.ProjectName}/
├── Core/
│   ├── {config.ProjectName}.Domain/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Enums/
│   │   └── Exceptions/
│   └── {config.ProjectName}.Application/
│       ├── Interfaces/
│       ├── Services/
│       ├── UseCases/
│       └── DTOs/
├── Infrastructure/
│   ├── {config.ProjectName}.Persistence/
│   ├── {config.ProjectName}.ExternalServices/
│   └── {config.ProjectName}.Messaging/
└── Presentation/
    └── {config.ProjectName}.UI/
        ├── Views/
        ├── ViewModels/
        └── Controls/";
    }

    private static string GenerateHexagonalArchitectureStructure(ArchitectureConfiguration config)
    {
        return $@"{config.ProjectName}/
├── {config.ProjectName}.Core/
│   ├── Domain/
│   │   ├── Aggregates/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   └── Services/
│   ├── Ports/
│   │   ├── Primary/    (Driving ports)
│   │   └── Secondary/  (Driven ports)
│   └── UseCases/
├── {config.ProjectName}.Adapters/
│   ├── Primary/
│   │   ├── UI/         (AvaloniaUI adapter)
│   │   ├── REST/       (REST API adapter)
│   │   └── GraphQL/    (GraphQL adapter)
│   └── Secondary/
│       ├── Database/   (Database adapter)
│       ├── FileSystem/ (File adapter)
│       └── External/   (External service adapters)
└── {config.ProjectName}.Configuration/
    ├── DependencyInjection/
    └── Composition/";
    }

    private static string GenerateLayeredArchitectureStructure(ArchitectureConfiguration config)
    {
        return $@"{config.ProjectName}/
├── {config.ProjectName}.Presentation/
│   ├── Views/
│   ├── ViewModels/
│   ├── Controls/
│   └── Converters/
├── {config.ProjectName}.Business/
│   ├── Services/
│   ├── Logic/
│   ├── Rules/
│   └── Workflows/
├── {config.ProjectName}.Data/
│   ├── Repositories/
│   ├── Entities/
│   ├── Context/
│   └── Migrations/
└── {config.ProjectName}.Common/
    ├── Interfaces/
    ├── Extensions/
    ├── Helpers/
    └── Constants/";
    }

    private static string GenerateBaseClasses(ArchitectureConfiguration config)
    {
        string validationMixin = config.IncludeValidation ? GenerateValidationMixin() : "";
        string reactiveMixin = config.IncludeReactive ? GenerateReactiveMixin() : "";

        return $@"// Base ViewModel with common functionality
public abstract class ViewModelBase : ReactiveObject{(config.IncludeValidation ? ", IValidatableObject" : "")}
{{
    private bool _isBusy;
    private string _title = string.Empty;

    public bool IsBusy
    {{
        get => _isBusy;
        set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }}

    public string Title
    {{
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }}

{(config.IncludeValidation ? validationMixin : "")}
{(config.IncludeReactive ? reactiveMixin : "")}
}}

// Base Service with logging and error handling
public abstract class ServiceBase
{{
    protected readonly ILogger Logger;

    protected ServiceBase(ILogger logger)
    {{
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }}

    protected async Task<TResult> ExecuteAsync<TResult>(
        Func<Task<TResult>> operation,
        [CallerMemberName] string operationName = """")
    {{
        Logger.LogInformation(""Starting operation: {{OperationName}}"", operationName);
        
        try
        {{
            var result = await operation();
            Logger.LogInformation(""Completed operation: {{OperationName}}"", operationName);
            return result;
        }}
        catch (Exception ex)
        {{
            Logger.LogError(ex, ""Error in operation: {{OperationName}}"", operationName);
            throw;
        }}
    }}
}}

// Entity base class
public abstract class Entity<TId> : IEquatable<Entity<TId>>
{{
    public TId Id {{ get; protected set; }}
    public DateTime CreatedAt {{ get; protected set; }}
    public DateTime? UpdatedAt {{ get; protected set; }}

    protected Entity(TId id)
    {{
        Id = id;
        CreatedAt = DateTime.UtcNow;
    }}

    public virtual bool Equals(Entity<TId>? other)
    {{
        return other is not null && Id?.Equals(other.Id) == true;
    }}

    public override bool Equals(object? obj)
    {{
        return Equals(obj as Entity<TId>);
    }}

    public override int GetHashCode()
    {{
        return Id?.GetHashCode() ?? 0;
    }}
}}

// Value Object base class
public abstract class ValueObject : IEquatable<ValueObject>
{{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other)
    {{
        return other is not null && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }}

    public override bool Equals(object? obj)
    {{
        return Equals(obj as ValueObject);
    }}

    public override int GetHashCode()
    {{
        return GetEqualityComponents()
            .Where(x => x != null)
            .Aggregate(1, (current, obj) => current * 23 + obj!.GetHashCode());
    }}
}}";
    }

    private static string GenerateValidationMixin()
    {
        return @"
    private readonly Dictionary<string, List<string>> _errors = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(this, validationContext, results, true);
        return results;
    }

    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return _errors.Values.SelectMany(x => x);
            
        return _errors.TryGetValue(propertyName, out var errors) ? errors : Enumerable.Empty<string>();
    }

    public bool HasErrors => _errors.Any();

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    protected void ValidateProperty(object? value, [CallerMemberName] string? propertyName = null)
    {
        if (propertyName == null) return;

        var context = new ValidationContext(this) { MemberName = propertyName };
        var results = new List<ValidationResult>();
        Validator.TryValidateProperty(value, context, results);

        if (results.Any())
        {
            _errors[propertyName] = results.Select(r => r.ErrorMessage ?? string.Empty).ToList();
        }
        else
        {
            _errors.Remove(propertyName);
        }

        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }";
    }

    private static string GenerateReactiveMixin()
    {
        return @"
    protected readonly CompositeDisposable Disposables = new();

    protected void AddDisposable(IDisposable disposable)
    {
        Disposables.Add(disposable);
    }

    protected ReactiveCommand<TParam, TResult> CreateCommand<TParam, TResult>(
        Func<TParam, Task<TResult>> execute,
        IObservable<bool>? canExecute = null)
    {
        var command = ReactiveCommand.CreateFromTask(execute, canExecute);
        AddDisposable(command);
        return command;
    }

    protected ReactiveCommand<Unit, Unit> CreateCommand(
        Func<Task> execute,
        IObservable<bool>? canExecute = null)
    {
        var command = ReactiveCommand.CreateFromTask(execute, canExecute);
        AddDisposable(command);
        return command;
    }

    public virtual void Dispose()
    {
        Disposables.Dispose();
    }";
    }

    private static string GenerateServiceLayer(ArchitectureConfiguration config)
    {
        string diRegistration = config.IncludeDI ? GenerateDIRegistration() : "";

        return $@"// Service interfaces
public interface IUserService
{{
    Task<User?> GetUserAsync(int userId);
    Task<User> CreateUserAsync(CreateUserRequest request);
    Task UpdateUserAsync(int userId, UpdateUserRequest request);
    Task DeleteUserAsync(int userId);
}}

// Service implementation
public class UserService : ServiceBase, IUserService
{{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<UserService> logger) : base(logger)
    {{
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }}

    public async Task<User?> GetUserAsync(int userId)
    {{
        return await ExecuteAsync(async () =>
        {{
            var userEntity = await _userRepository.GetByIdAsync(userId);
            return userEntity != null ? _mapper.Map<User>(userEntity) : null;
        }});
    }}

    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {{
        return await ExecuteAsync(async () =>
        {{
            var userEntity = _mapper.Map<UserEntity>(request);
            var createdEntity = await _userRepository.CreateAsync(userEntity);
            return _mapper.Map<User>(createdEntity);
        }});
    }}

    public async Task UpdateUserAsync(int userId, UpdateUserRequest request)
    {{
        await ExecuteAsync(async () =>
        {{
            var existingUser = await _userRepository.GetByIdAsync(userId);
            if (existingUser == null)
                throw new NotFoundException($""User with ID {{userId}} not found"");

            _mapper.Map(request, existingUser);
            await _userRepository.UpdateAsync(existingUser);
        }});
    }}

    public async Task DeleteUserAsync(int userId)
    {{
        await ExecuteAsync(async () =>
        {{
            await _userRepository.DeleteAsync(userId);
        }});
    }}
}}

{(config.IncludeDI ? diRegistration : "")}";
    }

    private static string GenerateDIRegistration()
    {
        return @"
// Dependency injection configuration
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IOrderService, OrderService>();
        
        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        
        // Register infrastructure services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        
        // Register AutoMapper
        services.AddAutoMapper(typeof(UserProfile));
        
        // Register validation
        services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
        
        return services;
    }
    
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString(""DefaultConnection"")));
            
        // Register external services
        services.AddHttpClient<IExternalApiService, ExternalApiService>();
        
        return services;
    }
}";
    }

    private static string GenerateViewModelTemplates(ArchitectureConfiguration config)
    {
        return $@"// Main ViewModel example
public class MainViewModel : ViewModelBase
{{
    private readonly IUserService _userService;
    private readonly INavigationService _navigationService;
    
    private ObservableCollection<UserViewModel> _users = new();
    private UserViewModel? _selectedUser;

    public MainViewModel(IUserService userService, INavigationService navigationService)
    {{
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        
        Title = ""User Management"";
        
        LoadUsersCommand = CreateCommand(LoadUsersAsync);
        CreateUserCommand = CreateCommand(CreateUserAsync);
        EditUserCommand = CreateCommand<UserViewModel>(EditUserAsync);
        DeleteUserCommand = CreateCommand<UserViewModel>(DeleteUserAsync);
        
        // Load users on initialization
        LoadUsersCommand.Execute().Subscribe();
    }}

    public ObservableCollection<UserViewModel> Users
    {{
        get => _users;
        set => this.RaiseAndSetIfChanged(ref _users, value);
    }}

    public UserViewModel? SelectedUser
    {{
        get => _selectedUser;
        set => this.RaiseAndSetIfChanged(ref _selectedUser, value);
    }}

    public ReactiveCommand<Unit, Unit> LoadUsersCommand {{ get; }}
    public ReactiveCommand<Unit, Unit> CreateUserCommand {{ get; }}
    public ReactiveCommand<UserViewModel, Unit> EditUserCommand {{ get; }}
    public ReactiveCommand<UserViewModel, Unit> DeleteUserCommand {{ get; }}

    private async Task LoadUsersAsync()
    {{
        IsBusy = true;
        try
        {{
            var users = await _userService.GetAllUsersAsync();
            Users = new ObservableCollection<UserViewModel>(
                users.Select(u => new UserViewModel(u, _userService)));
        }}
        finally
        {{
            IsBusy = false;
        }}
    }}

    private async Task CreateUserAsync()
    {{
        await _navigationService.NavigateToAsync<CreateUserViewModel>();
    }}

    private async Task EditUserAsync(UserViewModel userViewModel)
    {{
        await _navigationService.NavigateToAsync<EditUserViewModel>(userViewModel.User);
    }}

    private async Task DeleteUserAsync(UserViewModel userViewModel)
    {{
        var result = await _navigationService.ShowConfirmationAsync(
            ""Delete User"",
            $""Are you sure you want to delete {{userViewModel.Name}}?"");
            
        if (result)
        {{
            await _userService.DeleteUserAsync(userViewModel.User.Id);
            await LoadUsersAsync();
        }}
    }}
}}

// Detail ViewModel example
public class UserDetailsViewModel : ViewModelBase
{{
    private readonly IUserService _userService;
    private User _user;

    public UserDetailsViewModel(User user, IUserService userService)
    {{
        _user = user ?? throw new ArgumentNullException(nameof(user));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        
        Title = $""User Details - {{user.Name}}"";
        
        SaveCommand = CreateCommand(SaveAsync, CanSave());
        CancelCommand = CreateCommand(CancelAsync);
    }}

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name
    {{
        get => _user.Name;
        set 
        {{ 
            if (_user.Name != value)
            {{
                _user.Name = value;
                this.RaisePropertyChanged();
                {(config.IncludeValidation ? "ValidateProperty(value);" : "")}
            }}
        }}
    }}

    [Required]
    [EmailAddress]
    public string Email
    {{
        get => _user.Email;
        set 
        {{ 
            if (_user.Email != value)
            {{
                _user.Email = value;
                this.RaisePropertyChanged();
                {(config.IncludeValidation ? "ValidateProperty(value);" : "")}
            }}
        }}
    }}

    public ReactiveCommand<Unit, Unit> SaveCommand {{ get; }}
    public ReactiveCommand<Unit, Unit> CancelCommand {{ get; }}

    private IObservable<bool> CanSave()
    {{
        return this.WhenAnyValue(
            x => x.Name,
            x => x.Email,
            (name, email) => !string.IsNullOrWhiteSpace(name) && 
                           !string.IsNullOrWhiteSpace(email) &&
                           {(config.IncludeValidation ? "!HasErrors" : "true")});
    }}

    private async Task SaveAsync()
    {{
        IsBusy = true;
        try
        {{
            await _userService.UpdateUserAsync(_user.Id, new UpdateUserRequest
            {{
                Name = Name,
                Email = Email
            }});
            
            // Navigate back or show success message
        }}
        finally
        {{
            IsBusy = false;
        }}
    }}

    private Task CancelAsync()
    {{
        // Navigate back without saving
        return Task.CompletedTask;
    }}
}}";
    }

    private static string GenerateViewTemplates(ArchitectureConfiguration config)
    {
        return $@"<!-- MainView.axaml -->
<UserControl xmlns=""https://github.com/avaloniaui""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             x:Class=""YourApp.Views.MainView""
             x:DataType=""vm:MainViewModel"">

    <Grid RowDefinitions=""Auto,*,Auto"">
        <!-- Header -->
        <TextBlock Grid.Row=""0""
                   Text=""{{Binding Title}}""
                   Classes=""h1""
                   Margin=""16"" />

        <!-- Content -->
        <Grid Grid.Row=""1"" ColumnDefinitions=""2*,*"" Margin=""16"">
            <!-- Users List -->
            <ListBox Grid.Column=""0""
                     ItemsSource=""{{Binding Users}}""
                     SelectedItem=""{{Binding SelectedUser}}""
                     Margin=""0,0,8,0"">
                <ListBox.ItemTemplate>
                    <DataTemplate>
                        <Grid ColumnDefinitions=""*,Auto"" Margin=""8"">
                            <StackPanel Grid.Column=""0"">
                                <TextBlock Text=""{{Binding Name}}"" FontWeight=""Bold"" />
                                <TextBlock Text=""{{Binding Email}}"" Opacity=""0.7"" />
                            </StackPanel>
                            <StackPanel Grid.Column=""1"" Orientation=""Horizontal"" Spacing=""4"">
                                <Button Content=""Edit""
                                        Command=""{{Binding $parent[ListBox].((vm:MainViewModel)DataContext).EditUserCommand}}""
                                        CommandParameter=""{{Binding}}"" />
                                <Button Content=""Delete""
                                        Command=""{{Binding $parent[ListBox].((vm:MainViewModel)DataContext).DeleteUserCommand}}""
                                        CommandParameter=""{{Binding}}""
                                        Classes=""danger"" />
                            </StackPanel>
                        </Grid>
                    </DataTemplate>
                </ListBox.ItemTemplate>
            </ListBox>

            <!-- Details Panel -->
            <Border Grid.Column=""1""
                    Background=""{{DynamicResource SystemControlBackgroundAltHighBrush}}""
                    CornerRadius=""4""
                    Padding=""16""
                    Margin=""8,0,0,0"">
                <ContentControl Content=""{{Binding SelectedUser}}"">
                    <ContentControl.DataTemplates>
                        <DataTemplate DataType=""vm:UserViewModel"">
                            <StackPanel Spacing=""8"">
                                <TextBlock Text=""User Details"" FontWeight=""Bold"" />
                                <TextBlock Text=""{{Binding Name}}"" />
                                <TextBlock Text=""{{Binding Email}}"" />
                                <TextBlock Text=""{{Binding CreatedAt, StringFormat='Created: {0:d}'}}"" />
                            </StackPanel>
                        </DataTemplate>
                    </ContentControl.DataTemplates>
                </ContentControl>
            </Border>
        </Grid>

        <!-- Footer -->
        <StackPanel Grid.Row=""2""
                    Orientation=""Horizontal""
                    HorizontalAlignment=""Right""
                    Spacing=""8""
                    Margin=""16"">
            <Button Content=""Create User""
                    Command=""{{Binding CreateUserCommand}}""
                    Classes=""primary"" />
            <Button Content=""Refresh""
                    Command=""{{Binding LoadUsersCommand}}"" />
        </StackPanel>

        <!-- Loading overlay -->
        <Border Grid.RowSpan=""3""
                Background=""{{DynamicResource SystemControlBackgroundBaseMediumBrush}}""
                Opacity=""0.8""
                IsVisible=""{{Binding IsBusy}}"">
            <StackPanel HorizontalAlignment=""Center""
                        VerticalAlignment=""Center""
                        Spacing=""8"">
                <ProgressRing IsIndeterminate=""True"" />
                <TextBlock Text=""Loading..."" />
            </StackPanel>
        </Border>
    </Grid>
</UserControl>

<!-- UserDetailsView.axaml -->
<UserControl xmlns=""https://github.com/avaloniaui""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             x:Class=""YourApp.Views.UserDetailsView""
             x:DataType=""vm:UserDetailsViewModel"">

    <Grid RowDefinitions=""Auto,*,Auto"" Margin=""16"" MaxWidth=""400"">
        <!-- Header -->
        <TextBlock Grid.Row=""0""
                   Text=""{{Binding Title}}""
                   Classes=""h2""
                   Margin=""0,0,0,16"" />

        <!-- Form -->
        <StackPanel Grid.Row=""1"" Spacing=""12"">
            <!-- Name Field -->
            <StackPanel>
                <TextBlock Text=""Name"" Margin=""0,0,0,4"" />
                <TextBox Text=""{{Binding Name}}"" 
                         Watermark=""Enter user name"" />
                {(config.IncludeValidation ? @"<TextBlock Text=""{Binding (Validation.Errors)[0].ErrorContent, RelativeSource={RelativeSource Self}}""
                           Classes=""error""
                           IsVisible=""{Binding (Validation.HasErrors), RelativeSource={RelativeSource Self}}"" />" : "")}
            </StackPanel>

            <!-- Email Field -->
            <StackPanel>
                <TextBlock Text=""Email"" Margin=""0,0,0,4"" />
                <TextBox Text=""{{Binding Email}}"" 
                         Watermark=""Enter email address"" />
                {(config.IncludeValidation ? @"<TextBlock Text=""{Binding (Validation.Errors)[0].ErrorContent, RelativeSource={RelativeSource Self}}""
                           Classes=""error""
                           IsVisible=""{Binding (Validation.HasErrors), RelativeSource={RelativeSource Self}}"" />" : "")}
            </StackPanel>
        </StackPanel>

        <!-- Actions -->
        <StackPanel Grid.Row=""2""
                    Orientation=""Horizontal""
                    HorizontalAlignment=""Right""
                    Spacing=""8""
                    Margin=""0,16,0,0"">
            <Button Content=""Cancel""
                    Command=""{{Binding CancelCommand}}"" />
            <Button Content=""Save""
                    Command=""{{Binding SaveCommand}}""
                    Classes=""primary"" />
        </StackPanel>
    </Grid>
</UserControl>";
    }

    private static string GetArchitectureDescription(string pattern)
    {
        return pattern switch
        {
            "clean" => @"**Clean Architecture** emphasizes separation of concerns with dependency inversion:
- **Domain Layer**: Contains business entities, value objects, and domain services
- **Application Layer**: Contains use cases, application services, and interfaces
- **Infrastructure Layer**: Contains external concerns like databases and web APIs
- **Presentation Layer**: Contains UI components, ViewModels, and user interaction logic",

            "onion" => @"**Onion Architecture** organizes code in concentric layers:
- **Core**: Domain and Application layers (innermost)
- **Infrastructure**: Data access and external services (middle)
- **Presentation**: User interface and controllers (outermost)
- Dependencies point inward, with outer layers depending on inner layers",

            "hexagonal" => @"**Hexagonal Architecture** (Ports and Adapters) isolates business logic:
- **Core**: Contains pure business logic and domain model
- **Ports**: Define interfaces for external communication
- **Adapters**: Implement ports for specific technologies
- **Primary Adapters**: Drive the application (UI, REST APIs)
- **Secondary Adapters**: Driven by the application (databases, external services)",

            "layered" => @"**Layered Architecture** organizes code in horizontal layers:
- **Presentation Layer**: User interface and user interaction
- **Business Layer**: Business logic and rules
- **Data Layer**: Data access and persistence
- **Common Layer**: Shared utilities and cross-cutting concerns",

            _ => "Standard architectural pattern with separation of concerns"
        };
    }

    private static string GenerateMicroservicesStructure(string applicationName, List<string> services, string communicationPattern, bool gateway, bool eventSourcing)
    {
        string structure = $@"{applicationName}/
├── src/
│   ├── Services/";

        foreach (string service in services)
        {
            structure += $@"
│   │   ├── {service.ToTitleCase()}Service/
│   │   │   ├── {service.ToTitleCase()}Service.API/
│   │   │   ├── {service.ToTitleCase()}Service.Domain/
│   │   │   ├── {service.ToTitleCase()}Service.Infrastructure/
│   │   │   └── {service.ToTitleCase()}Service.Tests/";
        }

        if (gateway)
        {
            structure += @"
│   ├── Gateway/
│   │   ├── API.Gateway/
│   │   └── Gateway.Tests/";
        }

        structure += @"
│   ├── Shared/
│   │   ├── Shared.Contracts/
│   │   ├── Shared.Infrastructure/
│   │   └── Shared.EventBus/";

        if (eventSourcing)
        {
            structure += @"
│   ├── EventStore/
│   │   ├── EventStore.API/
│   │   └── EventStore.Infrastructure/";
        }

        structure += @"
│   └── Client/
│       ├── DesktopClient/
│       ├── Client.ViewModels/
│       └── Client.Services/
├── docker/
│   ├── docker-compose.yml
│   └── Dockerfile.*
└── k8s/
    ├── deployments/
    └── services/";

        return structure;
    }

    private static string GenerateServiceTemplates(List<string> services, string communicationPattern)
    {
        string firstService = services.First();

        return communicationPattern.ToLowerInvariant() switch
        {
            "rest" => GenerateRestServiceTemplate(firstService),
            "grpc" => GenerateGrpcServiceTemplate(firstService),
            "messagebus" => GenerateMessageBusServiceTemplate(firstService),
            _ => GenerateRestServiceTemplate(firstService)
        };
    }

    private static string GenerateRestServiceTemplate(string serviceName)
    {
        string className = serviceName.ToTitleCase();

        return $@"// {className}Service REST API
[ApiController]
[Route(""api/[controller]"")]
public class {className}Controller : ControllerBase
{{
    private readonly I{className}Service _{serviceName}Service;
    private readonly ILogger<{className}Controller> _logger;

    public {className}Controller(I{className}Service {serviceName}Service, ILogger<{className}Controller> logger)
    {{
        _{serviceName}Service = {serviceName}Service;
        _logger = logger;
    }}

    [HttpGet(""{{id}}"")]
    [ProducesResponseType(typeof({className}Dto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<{className}Dto>> GetAsync(int id)
    {{
        var result = await _{serviceName}Service.GetByIdAsync(id);
        return result != null ? Ok(result) : NotFound();
    }}

    [HttpPost]
    [ProducesResponseType(typeof({className}Dto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<{className}Dto>> CreateAsync(Create{className}Request request)
    {{
        var result = await _{serviceName}Service.CreateAsync(request);
        return CreatedAtAction(nameof(GetAsync), new {{ id = result.Id }}, result);
    }}

    [HttpPut(""{{id}}"")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(int id, Update{className}Request request)
    {{
        await _{serviceName}Service.UpdateAsync(id, request);
        return NoContent();
    }}

    [HttpDelete(""{{id}}"")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(int id)
    {{
        await _{serviceName}Service.DeleteAsync(id);
        return NoContent();
    }}
}}";
    }

    private static string GenerateGrpcServiceTemplate(string serviceName)
    {
        string className = serviceName.ToTitleCase();

        return $@"// {className}Service gRPC implementation
public class {className}GrpcService : {className}Service.{className}ServiceBase
{{
    private readonly I{className}Service _{serviceName}Service;
    private readonly IMapper _mapper;

    public {className}GrpcService(I{className}Service {serviceName}Service, IMapper mapper)
    {{
        _{serviceName}Service = {serviceName}Service;
        _mapper = mapper;
    }}

    public override async Task<{className}Response> Get{className}(Get{className}Request request, ServerCallContext context)
    {{
        var result = await _{serviceName}Service.GetByIdAsync(request.Id);
        if (result == null)
        {{
            throw new RpcException(new Status(StatusCode.NotFound, $""{className} with ID {{request.Id}} not found""));
        }}

        return _mapper.Map<{className}Response>(result);
    }}

    public override async Task<{className}Response> Create{className}(Create{className}Request request, ServerCallContext context)
    {{
        var dto = _mapper.Map<Create{className}Dto>(request);
        var result = await _{serviceName}Service.CreateAsync(dto);
        return _mapper.Map<{className}Response>(result);
    }}

    public override async Task<Empty> Update{className}(Update{className}Request request, ServerCallContext context)
    {{
        var dto = _mapper.Map<Update{className}Dto>(request);
        await _{serviceName}Service.UpdateAsync(request.Id, dto);
        return new Empty();
    }}

    public override async Task<Empty> Delete{className}(Delete{className}Request request, ServerCallContext context)
    {{
        await _{serviceName}Service.DeleteAsync(request.Id);
        return new Empty();
    }}
}}

// Proto file definition
syntax = ""proto3"";

option csharp_namespace = ""YourApp.{className}Service.Grpc"";

package {serviceName};

service {className}Service {{
  rpc Get{className} (Get{className}Request) returns ({className}Response);
  rpc Create{className} (Create{className}Request) returns ({className}Response);
  rpc Update{className} (Update{className}Request) returns (google.protobuf.Empty);
  rpc Delete{className} (Delete{className}Request) returns (google.protobuf.Empty);
}}

message {className}Response {{
  int32 id = 1;
  string name = 2;
  // Add other fields as needed
}}

message Get{className}Request {{
  int32 id = 1;
}}

message Create{className}Request {{
  string name = 1;
  // Add other fields as needed
}}

message Update{className}Request {{
  int32 id = 1;
  string name = 2;
  // Add other fields as needed
}}

message Delete{className}Request {{
  int32 id = 1;
}}";
    }

    private static string GenerateMessageBusServiceTemplate(string serviceName)
    {
        string className = serviceName.ToTitleCase();

        return $@"// {className}Service Message Bus implementation
public class {className}MessageHandler : 
    IConsumer<Create{className}Command>,
    IConsumer<Update{className}Command>,
    IConsumer<Delete{className}Command>
{{
    private readonly I{className}Service _{serviceName}Service;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<{className}MessageHandler> _logger;

    public {className}MessageHandler(
        I{className}Service {serviceName}Service,
        IPublishEndpoint publishEndpoint,
        ILogger<{className}MessageHandler> logger)
    {{
        _{serviceName}Service = {serviceName}Service;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }}

    public async Task Consume(ConsumeContext<Create{className}Command> context)
    {{
        try
        {{
            var result = await _{serviceName}Service.CreateAsync(context.Message.Data);
            
            await _publishEndpoint.Publish(new {className}CreatedEvent
            {{
                Id = result.Id,
                Name = result.Name,
                CreatedAt = DateTime.UtcNow
            }});
            
            await context.RespondAsync(new {className}CreatedResponse {{ {className} = result }});
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Error creating {serviceName}"");
            await context.RespondAsync(new {className}ErrorResponse {{ Error = ex.Message }});
        }}
    }}

    public async Task Consume(ConsumeContext<Update{className}Command> context)
    {{
        try
        {{
            await _{serviceName}Service.UpdateAsync(context.Message.Id, context.Message.Data);
            
            await _publishEndpoint.Publish(new {className}UpdatedEvent
            {{
                Id = context.Message.Id,
                UpdatedAt = DateTime.UtcNow
            }});
            
            await context.RespondAsync(new {className}UpdatedResponse());
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Error updating {serviceName} {{Id}}"", context.Message.Id);
            await context.RespondAsync(new {className}ErrorResponse {{ Error = ex.Message }});
        }}
    }}

    public async Task Consume(ConsumeContext<Delete{className}Command> context)
    {{
        try
        {{
            await _{serviceName}Service.DeleteAsync(context.Message.Id);
            
            await _publishEndpoint.Publish(new {className}DeletedEvent
            {{
                Id = context.Message.Id,
                DeletedAt = DateTime.UtcNow
            }});
            
            await context.RespondAsync(new {className}DeletedResponse());
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Error deleting {serviceName} {{Id}}"", context.Message.Id);
            await context.RespondAsync(new {className}ErrorResponse {{ Error = ex.Message }});
        }}
    }}
}}

// Commands and Events
public record Create{className}Command(Create{className}Dto Data);
public record Update{className}Command(int Id, Update{className}Dto Data);
public record Delete{className}Command(int Id);

public record {className}CreatedEvent(int Id, string Name, DateTime CreatedAt);
public record {className}UpdatedEvent(int Id, DateTime UpdatedAt);
public record {className}DeletedEvent(int Id, DateTime DeletedAt);

public record {className}CreatedResponse({className}Dto {className});
public record {className}UpdatedResponse();
public record {className}DeletedResponse();
public record {className}ErrorResponse(string Error);";
    }

    private static string GenerateClientIntegration(string communicationPattern)
    {
        return communicationPattern.ToLowerInvariant() switch
        {
            "rest" => GenerateRestClientIntegration(),
            "grpc" => GenerateGrpcClientIntegration(),
            "messagebus" => GenerateMessageBusClientIntegration(),
            _ => GenerateRestClientIntegration()
        };
    }

    private static string GenerateRestClientIntegration()
    {
        return @"// REST Client Service
public class ApiClientService : IApiClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClientService> _logger;

    public ApiClientService(HttpClient httpClient, ILogger<ApiClientService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error calling GET {Endpoint}"", endpoint);
            throw;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, JsonSerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");
            
            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseJson, JsonSerializerOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error calling POST {Endpoint}"", endpoint);
            throw;
        }
    }

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
}

// Service registration
services.AddHttpClient<IApiClientService, ApiClientService>(client =>
{
    client.BaseAddress = new Uri(configuration[""ApiBaseUrl""]);
    client.DefaultRequestHeaders.Add(""Accept"", ""application/json"");
});";
    }

    private static string GenerateGrpcClientIntegration()
    {
        return @"// gRPC Client Service
public class GrpcClientService : IGrpcClientService
{
    private readonly UserService.UserServiceClient _userClient;
    private readonly OrderService.OrderServiceClient _orderClient;

    public GrpcClientService(
        UserService.UserServiceClient userClient,
        OrderService.OrderServiceClient orderClient)
    {
        _userClient = userClient;
        _orderClient = orderClient;
    }

    public async Task<UserResponse?> GetUserAsync(int userId)
    {
        var request = new GetUserRequest { Id = userId };
        return await _userClient.GetUserAsync(request);
    }

    public async Task<UserResponse?> CreateUserAsync(CreateUserRequest request)
    {
        return await _userClient.CreateUserAsync(request);
    }
}

// Service registration
services.AddGrpcClient<UserService.UserServiceClient>(options =>
{
    options.Address = new Uri(configuration[""UserServiceUrl""]);
});

services.AddGrpcClient<OrderService.OrderServiceClient>(options =>
{
    options.Address = new Uri(configuration[""OrderServiceUrl""]);
});";
    }

    private static string GenerateMessageBusClientIntegration()
    {
        return @"// Message Bus Client Service
public class MessageBusClientService : IMessageBusClientService
{
    private readonly IRequestClient<CreateUserCommand> _createUserClient;
    private readonly IRequestClient<UpdateUserCommand> _updateUserClient;
    private readonly IPublishEndpoint _publishEndpoint;

    public MessageBusClientService(
        IRequestClient<CreateUserCommand> createUserClient,
        IRequestClient<UpdateUserCommand> updateUserClient,
        IPublishEndpoint publishEndpoint)
    {
        _createUserClient = createUserClient;
        _updateUserClient = updateUserClient;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<UserCreatedResponse> CreateUserAsync(CreateUserDto userData)
    {
        var command = new CreateUserCommand(userData);
        var response = await _createUserClient.GetResponse<UserCreatedResponse, UserErrorResponse>(command);
        
        return response.Is<UserCreatedResponse>(out var userCreated) 
            ? userCreated.Message 
            : throw new InvalidOperationException(response.Message.Error);
    }

    public async Task PublishUserEventAsync(UserEventData eventData)
    {
        await _publishEndpoint.Publish(new UserEvent
        {
            UserId = eventData.UserId,
            EventType = eventData.EventType,
            Data = eventData.Data,
            Timestamp = DateTime.UtcNow
        });
    }
}

// Service registration with MassTransit
services.AddMassTransit(x =>
{
    x.AddRequestClient<CreateUserCommand>();
    x.AddRequestClient<UpdateUserCommand>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(configuration.GetConnectionString(""RabbitMQ""));
        cfg.ConfigureEndpoints(context);
    });
});";
    }

    private static string GetCommunicationDescription(string pattern)
    {
        return pattern switch
        {
            "rest" => @"**REST Communication** provides simple HTTP-based API interaction:
- **HTTP Methods**: GET, POST, PUT, DELETE for CRUD operations
- **JSON Payload**: Standardized data exchange format
- **Stateless**: Each request contains all necessary information
- **Cacheable**: Responses can be cached for performance",

            "grpc" => @"**gRPC Communication** provides high-performance RPC framework:
- **Protocol Buffers**: Efficient binary serialization
- **HTTP/2**: Multiplexed, bidirectional communication
- **Strongly Typed**: Schema-first API design
- **Streaming**: Support for client/server/bidirectional streaming",

            "messagebus" => @"**Message Bus Communication** provides asynchronous messaging:
- **Publish/Subscribe**: Decoupled event-driven communication
- **Request/Response**: Asynchronous command handling
- **Message Durability**: Persistent message storage
- **Load Balancing**: Automatic message distribution",

            _ => "Standard communication pattern"
        };
    }

    private static string GenerateDDDStructure(string domainName, List<string> contexts, bool cqrs, bool eventSourcing, bool domainEvents)
    {
        string structure = $@"{domainName}/
├── src/
│   ├── Domain/";

        foreach (string context in contexts)
        {
            structure += $@"
│   │   ├── {context.ToTitleCase()}/
│   │   │   ├── Entities/
│   │   │   ├── ValueObjects/
│   │   │   ├── Aggregates/
│   │   │   ├── Services/
│   │   │   ├── Repositories/";

            if (domainEvents)
            {
                structure += $@"
│   │   │   ├── Events/";
            }

            structure += $@"
│   │   │   └── Specifications/";
        }

        structure += @"
│   ├── Application/
│   │   ├── Services/
│   │   ├── DTOs/
│   │   ├── Interfaces/";

        if (cqrs)
        {
            structure += @"
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Handlers/";
        }

        structure += @"
│   │   └── Validators/
│   ├── Infrastructure/
│   │   ├── Persistence/
│   │   ├── External/
│   │   ├── Configuration/";

        if (eventSourcing)
        {
            structure += @"
│   │   ├── EventStore/";
        }

        structure += @"
│   │   └── Messaging/
│   └── Presentation/
│       ├── API/
│       ├── Desktop/
│       └── Web/";

        return structure;
    }

    private static string GenerateDomainLayer(string context, bool domainEvents)
    {
        string className = context.ToTitleCase();
        string eventsCode = domainEvents ? GenerateDomainEventsCode(className) : "";

        return $@"// Aggregate Root
public class {className} : AggregateRoot<{className}Id>
{{
    private readonly List<{className}Item> _items = new();

    public {className}Name Name {{ get; private set; }}
    public {className}Status Status {{ get; private set; }}
    public Money TotalAmount {{ get; private set; }}
    public IReadOnlyList<{className}Item> Items => _items.AsReadOnly();

    private {className}() {{ }} // For ORM

    public {className}({className}Id id, {className}Name name)
        : base(id)
    {{
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Status = {className}Status.Draft;
        TotalAmount = Money.Zero;
        
        {(domainEvents ? $"AddDomainEvent(new {className}CreatedEvent(Id, Name));" : "")}
    }}

    public void AddItem(ProductId productId, Quantity quantity, Money unitPrice)
    {{
        if (Status != {className}Status.Draft)
            throw new InvalidOperationException(""Cannot modify confirmed {context}"");

        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {{
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }}
        else
        {{
            var item = new {className}Item(productId, quantity, unitPrice);
            _items.Add(item);
        }}

        RecalculateTotal();
        {(domainEvents ? $"AddDomainEvent(new {className}ItemAddedEvent(Id, productId, quantity));" : "")}
    }}

    public void Confirm()
    {{
        if (Status != {className}Status.Draft)
            throw new InvalidOperationException(""{className} is already confirmed"");

        if (!_items.Any())
            throw new DomainException(""{className} must have at least one item"");

        Status = {className}Status.Confirmed;
        {(domainEvents ? $"AddDomainEvent(new {className}ConfirmedEvent(Id, TotalAmount));" : "")}
    }}

    private void RecalculateTotal()
    {{
        TotalAmount = _items.Aggregate(Money.Zero, (sum, item) => sum + item.TotalPrice);
    }}
}}

// Value Objects
public class {className}Id : ValueObject
{{
    public Guid Value {{ get; }}

    public {className}Id(Guid value)
    {{
        if (value == Guid.Empty)
            throw new ArgumentException(""Invalid {className} ID"", nameof(value));
        Value = value;
    }}

    protected override IEnumerable<object?> GetEqualityComponents()
    {{
        yield return Value;
    }}

    public static implicit operator Guid({className}Id id) => id.Value;
    public static implicit operator {className}Id(Guid value) => new(value);
}}

public class {className}Name : ValueObject
{{
    public string Value {{ get; }}

    public {className}Name(string value)
    {{
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(""Name cannot be empty"", nameof(value));
        if (value.Length > 100)
            throw new ArgumentException(""Name cannot exceed 100 characters"", nameof(value));
        
        Value = value.Trim();
    }}

    protected override IEnumerable<object?> GetEqualityComponents()
    {{
        yield return Value;
    }}

    public static implicit operator string({className}Name name) => name.Value;
    public static implicit operator {className}Name(string value) => new(value);
}}

// Enumeration
public class {className}Status : Enumeration
{{
    public static {className}Status Draft = new(1, nameof(Draft));
    public static {className}Status Confirmed = new(2, nameof(Confirmed));
    public static {className}Status Cancelled = new(3, nameof(Cancelled));

    public {className}Status(int id, string name) : base(id, name) {{ }}
}}

// Repository Interface
public interface I{className}Repository : IRepository<{className}, {className}Id>
{{
    Task<{className}?> GetByNameAsync({className}Name name);
    Task<IEnumerable<{className}>> GetByStatusAsync({className}Status status);
}}

{eventsCode}";
    }

    private static string GenerateDomainEventsCode(string className)
    {
        return $@"
// Domain Events
public class {className}CreatedEvent : DomainEvent
{{
    public {className}Id {className}Id {{ get; }}
    public {className}Name Name {{ get; }}

    public {className}CreatedEvent({className}Id {className.ToLowerInvariant()}Id, {className}Name name)
    {{
        {className}Id = {className.ToLowerInvariant()}Id;
        Name = name;
    }}
}}

public class {className}ItemAddedEvent : DomainEvent
{{
    public {className}Id {className}Id {{ get; }}
    public ProductId ProductId {{ get; }}
    public Quantity Quantity {{ get; }}

    public {className}ItemAddedEvent({className}Id {className.ToLowerInvariant()}Id, ProductId productId, Quantity quantity)
    {{
        {className}Id = {className.ToLowerInvariant()}Id;
        ProductId = productId;
        Quantity = quantity;
    }}
}}

public class {className}ConfirmedEvent : DomainEvent
{{
    public {className}Id {className}Id {{ get; }}
    public Money TotalAmount {{ get; }}

    public {className}ConfirmedEvent({className}Id {className.ToLowerInvariant()}Id, Money totalAmount)
    {{
        {className}Id = {className.ToLowerInvariant()}Id;
        TotalAmount = totalAmount;
    }}
}}";
    }

    private static string GenerateApplicationLayer(string context, bool cqrs)
    {
        string className = context.ToTitleCase();

        return cqrs ? GenerateCQRSApplicationLayer(className) : GenerateTraditionalApplicationLayer(className);
    }

    private static string GenerateCQRSApplicationLayer(string className)
    {
        return $@"// Commands
public record Create{className}Command({className}Name Name) : IRequest<{className}Id>;
public record Add{className}ItemCommand({className}Id {className}Id, ProductId ProductId, Quantity Quantity, Money UnitPrice) : IRequest;
public record Confirm{className}Command({className}Id {className}Id) : IRequest;

// Queries
public record Get{className}Query({className}Id {className}Id) : IRequest<{className}Dto?>;
public record Get{className}sByStatusQuery({className}Status Status) : IRequest<IEnumerable<{className}Dto>>;

// Command Handlers
public class Create{className}Handler : IRequestHandler<Create{className}Command, {className}Id>
{{
    private readonly I{className}Repository _{className.ToLowerInvariant()}Repository;
    private readonly IUnitOfWork _unitOfWork;

    public Create{className}Handler(I{className}Repository {className.ToLowerInvariant()}Repository, IUnitOfWork unitOfWork)
    {{
        _{className.ToLowerInvariant()}Repository = {className.ToLowerInvariant()}Repository;
        _unitOfWork = unitOfWork;
    }}

    public async Task<{className}Id> Handle(Create{className}Command request, CancellationToken cancellationToken)
    {{
        var {className.ToLowerInvariant()} = new {className}({className}Id.New(), request.Name);
        
        await _{className.ToLowerInvariant()}Repository.AddAsync({className.ToLowerInvariant()});
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return {className.ToLowerInvariant()}.Id;
    }}
}}

public class Add{className}ItemHandler : IRequestHandler<Add{className}ItemCommand>
{{
    private readonly I{className}Repository _{className.ToLowerInvariant()}Repository;
    private readonly IUnitOfWork _unitOfWork;

    public Add{className}ItemHandler(I{className}Repository {className.ToLowerInvariant()}Repository, IUnitOfWork unitOfWork)
    {{
        _{className.ToLowerInvariant()}Repository = {className.ToLowerInvariant()}Repository;
        _unitOfWork = unitOfWork;
    }}

    public async Task Handle(Add{className}ItemCommand request, CancellationToken cancellationToken)
    {{
        var {className.ToLowerInvariant()} = await _{className.ToLowerInvariant()}Repository.GetByIdAsync(request.{className}Id);
        if ({className.ToLowerInvariant()} == null)
            throw new NotFoundException($""{className} {{request.{className}Id}} not found"");

        {className.ToLowerInvariant()}.AddItem(request.ProductId, request.Quantity, request.UnitPrice);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }}
}}

// Query Handlers
public class Get{className}Handler : IRequestHandler<Get{className}Query, {className}Dto?>
{{
    private readonly I{className}ReadModel _{className.ToLowerInvariant()}ReadModel;

    public Get{className}Handler(I{className}ReadModel {className.ToLowerInvariant()}ReadModel)
    {{
        _{className.ToLowerInvariant()}ReadModel = {className.ToLowerInvariant()}ReadModel;
    }}

    public async Task<{className}Dto?> Handle(Get{className}Query request, CancellationToken cancellationToken)
    {{
        return await _{className.ToLowerInvariant()}ReadModel.GetByIdAsync(request.{className}Id);
    }}
}}

// DTOs
public class {className}Dto
{{
    public {className}Id Id {{ get; set; }}
    public string Name {{ get; set; }} = string.Empty;
    public string Status {{ get; set; }} = string.Empty;
    public decimal TotalAmount {{ get; set; }}
    public List<{className}ItemDto> Items {{ get; set; }} = new();
}}

public class {className}ItemDto
{{
    public ProductId ProductId {{ get; set; }}
    public int Quantity {{ get; set; }}
    public decimal UnitPrice {{ get; set; }}
    public decimal TotalPrice {{ get; set; }}
}}";
    }

    private static string GenerateTraditionalApplicationLayer(string className)
    {
        return $@"// Application Service
public class {className}ApplicationService : I{className}ApplicationService
{{
    private readonly I{className}Repository _{className.ToLowerInvariant()}Repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<{className}ApplicationService> _logger;

    public {className}ApplicationService(
        I{className}Repository {className.ToLowerInvariant()}Repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<{className}ApplicationService> logger)
    {{
        _{className.ToLowerInvariant()}Repository = {className.ToLowerInvariant()}Repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }}

    public async Task<{className}Dto> CreateAsync(Create{className}Request request)
    {{
        var {className.ToLowerInvariant()} = new {className}({className}Id.New(), new {className}Name(request.Name));
        
        await _{className.ToLowerInvariant()}Repository.AddAsync({className.ToLowerInvariant()});
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation(""{className} created with ID {{Id}}"", {className.ToLowerInvariant()}.Id);
        
        return _mapper.Map<{className}Dto>({className.ToLowerInvariant()});
    }}

    public async Task<{className}Dto?> GetByIdAsync({className}Id id)
    {{
        var {className.ToLowerInvariant()} = await _{className.ToLowerInvariant()}Repository.GetByIdAsync(id);
        return {className.ToLowerInvariant()} != null ? _mapper.Map<{className}Dto>({className.ToLowerInvariant()}) : null;
    }}

    public async Task AddItemAsync({className}Id {className.ToLowerInvariant()}Id, Add{className}ItemRequest request)
    {{
        var {className.ToLowerInvariant()} = await _{className.ToLowerInvariant()}Repository.GetByIdAsync({className.ToLowerInvariant()}Id);
        if ({className.ToLowerInvariant()} == null)
            throw new NotFoundException($""{className} {{{className.ToLowerInvariant()}Id}} not found"");

        {className.ToLowerInvariant()}.AddItem(
            new ProductId(request.ProductId),
            new Quantity(request.Quantity),
            new Money(request.UnitPrice));
        
        await _unitOfWork.SaveChangesAsync();
    }}

    public async Task ConfirmAsync({className}Id id)
    {{
        var {className.ToLowerInvariant()} = await _{className.ToLowerInvariant()}Repository.GetByIdAsync(id);
        if ({className.ToLowerInvariant()} == null)
            throw new NotFoundException($""{className} {{id}} not found"");

        {className.ToLowerInvariant()}.Confirm();
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation(""{className} {{Id}} confirmed"", id);
    }}
}}

// Request/Response DTOs
public class Create{className}Request
{{
    public string Name {{ get; set; }} = string.Empty;
}}

public class Add{className}ItemRequest
{{
    public Guid ProductId {{ get; set; }}
    public int Quantity {{ get; set; }}
    public decimal UnitPrice {{ get; set; }}
}}

public class {className}Dto
{{
    public Guid Id {{ get; set; }}
    public string Name {{ get; set; }} = string.Empty;
    public string Status {{ get; set; }} = string.Empty;
    public decimal TotalAmount {{ get; set; }}
    public List<{className}ItemDto> Items {{ get; set; }} = new();
    public DateTime CreatedAt {{ get; set; }}
}}";
    }

    private static string GenerateInfrastructureLayer(string context, bool eventSourcing)
    {
        string className = context.ToTitleCase();
        string eventStoreCode = eventSourcing ? GenerateEventStoreCode(className) : "";

        return $@"// Repository Implementation
public class {className}Repository : I{className}Repository
{{
    private readonly DbContext _context;
    private readonly ILogger<{className}Repository> _logger;

    public {className}Repository(DbContext context, ILogger<{className}Repository> logger)
    {{
        _context = context;
        _logger = logger;
    }}

    public async Task<{className}?> GetByIdAsync({className}Id id)
    {{
        return await _context.Set<{className}>()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
    }}

    public async Task<{className}?> GetByNameAsync({className}Name name)
    {{
        return await _context.Set<{className}>()
            .FirstOrDefaultAsync(o => o.Name == name);
    }}

    public async Task<IEnumerable<{className}>> GetByStatusAsync({className}Status status)
    {{
        return await _context.Set<{className}>()
            .Where(o => o.Status == status)
            .ToListAsync();
    }}

    public async Task AddAsync({className} {className.ToLowerInvariant()})
    {{
        await _context.Set<{className}>().AddAsync({className.ToLowerInvariant()});
    }}

    public Task UpdateAsync({className} {className.ToLowerInvariant()})
    {{
        _context.Set<{className}>().Update({className.ToLowerInvariant()});
        return Task.CompletedTask;
    }}

    public Task DeleteAsync({className}Id id)
    {{
        var {className.ToLowerInvariant()} = _context.Set<{className}>().Find(id);
        if ({className.ToLowerInvariant()} != null)
        {{
            _context.Set<{className}>().Remove({className.ToLowerInvariant()});
        }}
        return Task.CompletedTask;
    }}
}}

// Entity Configuration
public class {className}Configuration : IEntityTypeConfiguration<{className}>
{{
    public void Configure(EntityTypeBuilder<{className}> builder)
    {{
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Id)
            .HasConversion(
                id => id.Value,
                value => new {className}Id(value));

        builder.Property(o => o.Name)
            .HasConversion(
                name => name.Value,
                value => new {className}Name(value))
            .HasMaxLength(100);

        builder.Property(o => o.Status)
            .HasConversion(
                status => status.Id,
                value => {className}Status.FromValue(value));

        builder.OwnsOne(o => o.TotalAmount, money =>
        {{
            money.Property(m => m.Amount).HasColumnName(""TotalAmount"");
            money.Property(m => m.Currency).HasColumnName(""Currency"");
        }});

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(""{className}Id"");

        builder.Navigation(o => o.Items).EnableLazyLoading(false);
    }}
}}

// Unit of Work
public class UnitOfWork : IUnitOfWork
{{
    private readonly DbContext _context;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public UnitOfWork(DbContext context, IDomainEventDispatcher domainEventDispatcher)
    {{
        _context = context;
        _domainEventDispatcher = domainEventDispatcher;
    }}

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {{
        // Collect domain events before saving
        var domainEvents = _context.ChangeTracker.Entries<IAggregateRoot>()
            .SelectMany(entry => entry.Entity.GetDomainEvents())
            .ToList();

        // Save changes
        var result = await _context.SaveChangesAsync(cancellationToken);

        // Dispatch domain events after successful save
        foreach (var domainEvent in domainEvents)
        {{
            await _domainEventDispatcher.DispatchAsync(domainEvent);
        }}

        return result;
    }}
}}

{eventStoreCode}";
    }

    private static string GenerateEventStoreCode(string className)
    {
        return $@"
// Event Store Implementation
public class EventStore : IEventStore
{{
    private readonly IEventStoreConnection _connection;
    private readonly IEventSerializer _serializer;

    public EventStore(IEventStoreConnection connection, IEventSerializer serializer)
    {{
        _connection = connection;
        _serializer = serializer;
    }}

    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<IDomainEvent> events, int expectedVersion)
    {{
        var eventData = events.Select(e => new EventData(
            Guid.NewGuid(),
            e.GetType().Name,
            true,
            _serializer.Serialize(e),
            null)).ToArray();

        await _connection.AppendToStreamAsync(
            GetStreamName(aggregateId),
            expectedVersion == 0 ? ExpectedVersion.NoStream : expectedVersion - 1,
            eventData);
    }}

    public async Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId)
    {{
        var streamEvents = await _connection.ReadStreamEventsForwardAsync(
            GetStreamName(aggregateId),
            0,
            int.MaxValue,
            false);

        return streamEvents.Events.Select(e => _serializer.Deserialize(e.Event.Data, e.Event.EventType));
    }}

    private string GetStreamName(Guid aggregateId) => $""{className.ToLowerInvariant()}-{{aggregateId}}"";
}}

// Event Sourced Repository
public class EventSourced{className}Repository : I{className}Repository
{{
    private readonly IEventStore _eventStore;

    public EventSourced{className}Repository(IEventStore eventStore)
    {{
        _eventStore = eventStore;
    }}

    public async Task<{className}?> GetByIdAsync({className}Id id)
    {{
        var events = await _eventStore.GetEventsAsync(id.Value);
        if (!events.Any())
            return null;

        var {className.ToLowerInvariant()} = new {className}();
        {className.ToLowerInvariant()}.LoadFromHistory(events);
        return {className.ToLowerInvariant()};
    }}

    public async Task AddAsync({className} {className.ToLowerInvariant()})
    {{
        var uncommittedEvents = {className.ToLowerInvariant()}.GetUncommittedEvents();
        await _eventStore.SaveEventsAsync({className.ToLowerInvariant()}.Id.Value, uncommittedEvents, 0);
        {className.ToLowerInvariant()}.MarkEventsAsCommitted();
    }}

    public async Task UpdateAsync({className} {className.ToLowerInvariant()})
    {{
        var uncommittedEvents = {className.ToLowerInvariant()}.GetUncommittedEvents();
        if (uncommittedEvents.Any())
        {{
            await _eventStore.SaveEventsAsync(
                {className.ToLowerInvariant()}.Id.Value,
                uncommittedEvents,
                {className.ToLowerInvariant()}.Version);
            {className.ToLowerInvariant()}.MarkEventsAsCommitted();
        }}
    }}
}}";
    }

    private static string GeneratePluginStructure(string applicationName, List<string> types, string loadingStrategy, bool mef, bool hotReload)
    {
        string structure = $@"{applicationName}/
├── src/
│   ├── {applicationName}.Core/
│   │   ├── Interfaces/
│   │   ├── Models/
│   │   └── Contracts/
│   ├── {applicationName}.PluginHost/
│   │   ├── Loader/
│   │   ├── Manager/
│   │   ├── Discovery/";

        if (hotReload)
        {
            structure += @"
│   │   ├── HotReload/";
        }

        structure += @"
│   │   └── Security/
│   ├── Plugins/";

        foreach (string type in types)
        {
            structure += $@"
│   │   ├── {type.ToTitleCase()}Plugins/
│   │   │   ├── Sample{type.ToTitleCase()}Plugin/
│   │   │   └── Advanced{type.ToTitleCase()}Plugin/";
        }

        structure += $@"
│   └── {applicationName}.Host/
│       ├── Views/
│       ├── ViewModels/
│       └── Services/
├── plugins/
│   ├── manifests/
│   └── assemblies/
└── sdk/
    ├── PluginSDK/
    ├── Templates/
    └── Documentation/";

        return structure;
    }

    private static string GeneratePluginInterfaces(List<string> types)
    {
        string firstType = types.First();
        string className = firstType.ToTitleCase();

        return $@"// Base Plugin Interface
public interface IPlugin
{{
    string Name {{ get; }}
    string Version {{ get; }}
    string Description {{ get; }}
    string Author {{ get; }}
    
    Task InitializeAsync(IPluginContext context);
    Task ShutdownAsync();
    
    bool IsCompatible(Version hostVersion);
}}

// Specific Plugin Interface
public interface I{className}Plugin : IPlugin
{{
    Task<{className}Result> Execute{className}Async({className}Input input);
    bool CanHandle({className}Input input);
    IEnumerable<{className}Capability> GetCapabilities();
}}

// Plugin Context
public interface IPluginContext
{{
    IServiceProvider Services {{ get; }}
    IConfiguration Configuration {{ get; }}
    ILogger Logger {{ get; }}
    string PluginDirectory {{ get; }}
    
    T GetService<T>() where T : class;
    Task<T> GetServiceAsync<T>() where T : class;
    void RegisterService<T>(T service) where T : class;
}}

// Plugin Metadata
[AttributeUsage(AttributeTargets.Class)]
public class PluginMetadataAttribute : Attribute
{{
    public string Name {{ get; set; }} = string.Empty;
    public string Version {{ get; set; }} = string.Empty;
    public string Description {{ get; set; }} = string.Empty;
    public string Author {{ get; set; }} = string.Empty;
    public string Category {{ get; set; }} = string.Empty;
    public string[] Dependencies {{ get; set; }} = Array.Empty<string>();
    public string MinHostVersion {{ get; set; }} = string.Empty;
}}

// Plugin Data Models
public class {className}Input
{{
    public string Data {{ get; set; }} = string.Empty;
    public Dictionary<string, object> Parameters {{ get; set; }} = new();
    public CancellationToken CancellationToken {{ get; set; }}
}}

public class {className}Result
{{
    public bool Success {{ get; set; }}
    public string Message {{ get; set; }} = string.Empty;
    public object? Data {{ get; set; }}
    public TimeSpan ExecutionTime {{ get; set; }}
}}

public class {className}Capability
{{
    public string Name {{ get; set; }} = string.Empty;
    public string Description {{ get; set; }} = string.Empty;
    public Dictionary<string, object> Properties {{ get; set; }} = new();
}}";
    }

    private static string GeneratePluginHost(string loadingStrategy, bool mef, bool hotReload)
    {
        string mefCode = mef ? GenerateMEFPluginHost() : "";
        string hotReloadCode = hotReload ? GenerateHotReloadCode() : "";

        return $@"// Plugin Manager
public class PluginManager : IPluginManager, IDisposable
{{
    private readonly IPluginLoader _loader;
    private readonly IPluginDiscovery _discovery;
    private readonly ILogger<PluginManager> _logger;
    private readonly ConcurrentDictionary<string, IPlugin> _loadedPlugins = new();
    {(hotReload ? "private readonly IPluginHotReloader _hotReloader;" : "")}

    public PluginManager(
        IPluginLoader loader,
        IPluginDiscovery discovery,
        ILogger<PluginManager> logger
        {(hotReload ? ", IPluginHotReloader hotReloader" : "")})
    {{
        _loader = loader;
        _discovery = discovery;
        _logger = logger;
        {(hotReload ? "_hotReloader = hotReloader;" : "")}
    }}

    public async Task LoadPluginsAsync(string pluginDirectory)
    {{
        var pluginInfos = await _discovery.DiscoverPluginsAsync(pluginDirectory);
        
        foreach (var pluginInfo in pluginInfos)
        {{
            try
            {{
                var plugin = await _loader.LoadPluginAsync(pluginInfo);
                if (plugin != null)
                {{
                    await plugin.InitializeAsync(new PluginContext(_services, pluginInfo.Directory));
                    _loadedPlugins.TryAdd(plugin.Name, plugin);
                    _logger.LogInformation(""Plugin loaded: {{PluginName}}"", plugin.Name);
                }}
            }}
            catch (Exception ex)
            {{
                _logger.LogError(ex, ""Failed to load plugin: {{PluginPath}}"", pluginInfo.AssemblyPath);
            }}
        }}

        {(hotReload ? "await _hotReloader.StartWatchingAsync(pluginDirectory);" : "")}
    }}

    public T? GetPlugin<T>(string name) where T : class, IPlugin
    {{
        return _loadedPlugins.TryGetValue(name, out var plugin) ? plugin as T : null;
    }}

    public IEnumerable<T> GetPlugins<T>() where T : class, IPlugin
    {{
        return _loadedPlugins.Values.OfType<T>();
    }}

    public async Task UnloadPluginAsync(string name)
    {{
        if (_loadedPlugins.TryRemove(name, out var plugin))
        {{
            await plugin.ShutdownAsync();
            _logger.LogInformation(""Plugin unloaded: {{PluginName}}"", name);
        }}
    }}

    public void Dispose()
    {{
        foreach (var plugin in _loadedPlugins.Values)
        {{
            try
            {{
                plugin.ShutdownAsync().Wait(TimeSpan.FromSeconds(5));
            }}
            catch (Exception ex)
            {{
                _logger.LogError(ex, ""Error shutting down plugin: {{PluginName}}"", plugin.Name);
            }}
        }}
        
        _loadedPlugins.Clear();
        {(hotReload ? "_hotReloader?.Dispose();" : "")}
    }}
}}

// Plugin Loader
public class PluginLoader : IPluginLoader
{{
    private readonly ILogger<PluginLoader> _logger;
    private readonly List<AssemblyLoadContext> _loadContexts = new();

    public PluginLoader(ILogger<PluginLoader> logger)
    {{
        _logger = logger;
    }}

    public async Task<IPlugin?> LoadPluginAsync(PluginInfo pluginInfo)
    {{
        try
        {{
            var loadContext = new PluginLoadContext(pluginInfo.AssemblyPath);
            _loadContexts.Add(loadContext);

            var assembly = loadContext.LoadFromAssemblyPath(pluginInfo.AssemblyPath);
            
            var pluginTypes = assembly.GetTypes()
                .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList();

            if (!pluginTypes.Any())
            {{
                _logger.LogWarning(""No plugin implementations found in {{Assembly}}"", pluginInfo.AssemblyPath);
                return null;
            }}

            var pluginType = pluginTypes.First();
            var plugin = Activator.CreateInstance(pluginType) as IPlugin;
            
            return plugin;
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Failed to load plugin assembly: {{Assembly}}"", pluginInfo.AssemblyPath);
            return null;
        }}
    }}
}}

// Plugin Load Context
public class PluginLoadContext : AssemblyLoadContext
{{
    private readonly AssemblyDependencyResolver _resolver;

    public PluginLoadContext(string pluginPath) : base(isCollectible: true)
    {{
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }}

    protected override Assembly? Load(AssemblyName assemblyName)
    {{
        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
    }}

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {{
        var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
    }}
}}

{mefCode}
{hotReloadCode}";
    }

    private static string GenerateMEFPluginHost()
    {
        return @"
// MEF Plugin Discovery
[Export(typeof(IPluginDiscovery))]
public class MefPluginDiscovery : IPluginDiscovery
{
    public async Task<IEnumerable<PluginInfo>> DiscoverPluginsAsync(string directory)
    {
        var catalog = new DirectoryCatalog(directory, ""*.dll"");
        var container = new CompositionContainer(catalog);
        
        var plugins = container.GetExports<IPlugin, IPluginMetadata>();
        
        return plugins.Select(p => new PluginInfo
        {
            Name = p.Metadata.Name,
            Version = p.Metadata.Version,
            AssemblyPath = p.Value.GetType().Assembly.Location,
            Directory = directory
        });
    }
}

// MEF Plugin Metadata
public interface IPluginMetadata
{
    string Name { get; }
    string Version { get; }
    string Description { get; }
    string Author { get; }
}

// MEF Export Attribute
[MetadataAttribute]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ExportPluginAttribute : ExportAttribute, IPluginMetadata
{
    public ExportPluginAttribute(string name, string version) : base(typeof(IPlugin))
    {
        Name = name;
        Version = version;
    }

    public string Name { get; }
    public string Version { get; }
    public string Description { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
}";
    }

    private static string GenerateHotReloadCode()
    {
        return @"
// Hot Reload Plugin Manager
public class PluginHotReloader : IPluginHotReloader, IDisposable
{
    private readonly FileSystemWatcher _watcher;
    private readonly IPluginManager _pluginManager;
    private readonly ILogger<PluginHotReloader> _logger;
    private readonly Dictionary<string, DateTime> _lastModified = new();

    public PluginHotReloader(IPluginManager pluginManager, ILogger<PluginHotReloader> logger)
    {
        _pluginManager = pluginManager;
        _logger = logger;
        _watcher = new FileSystemWatcher();
        _watcher.Changed += OnPluginFileChanged;
        _watcher.Created += OnPluginFileChanged;
        _watcher.Deleted += OnPluginFileDeleted;
    }

    public Task StartWatchingAsync(string directory)
    {
        _watcher.Path = directory;
        _watcher.Filter = ""*.dll"";
        _watcher.IncludeSubdirectories = true;
        _watcher.EnableRaisingEvents = true;
        
        _logger.LogInformation(""Started watching for plugin changes in {Directory}"", directory);
        return Task.CompletedTask;
    }

    private async void OnPluginFileChanged(object sender, FileSystemEventArgs e)
    {
        try
        {
            // Debounce file system events
            var lastWrite = File.GetLastWriteTime(e.FullPath);
            if (_lastModified.TryGetValue(e.FullPath, out var previousWrite) && 
                lastWrite.Subtract(previousWrite).TotalMilliseconds < 1000)
            {
                return;
            }
            _lastModified[e.FullPath] = lastWrite;

            _logger.LogInformation(""Plugin file changed: {FilePath}"", e.FullPath);

            // Wait for file to be fully written
            await Task.Delay(500);

            // Reload plugin
            await ReloadPluginAsync(e.FullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""Error handling plugin file change: {FilePath}"", e.FullPath);
        }
    }

    private async void OnPluginFileDeleted(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation(""Plugin file deleted: {FilePath}"", e.FullPath);
        // Handle plugin removal
    }

    private async Task ReloadPluginAsync(string filePath)
    {
        var pluginInfo = new PluginInfo
        {
            AssemblyPath = filePath,
            Directory = Path.GetDirectoryName(filePath) ?? string.Empty
        };

        // Unload existing plugin if it exists
        // This is simplified - in practice you'd need to track plugin names by file path
        // await _pluginManager.UnloadPluginAsync(pluginName);

        // Load the updated plugin
        var loader = new PluginLoader(new NullLogger<PluginLoader>());
        var plugin = await loader.LoadPluginAsync(pluginInfo);
        
        if (plugin != null)
        {
            await plugin.InitializeAsync(new PluginContext(null, pluginInfo.Directory));
            _logger.LogInformation(""Plugin reloaded: {PluginName}"", plugin.Name);
        }
    }

    public void Dispose()
    {
        _watcher?.Dispose();
    }
}";
    }

    private static string GenerateSamplePlugin(string type)
    {
        string className = type.ToTitleCase();

        return $@"// Sample {className} Plugin
[PluginMetadata(
    Name = ""Sample {className} Plugin"",
    Version = ""1.0.0"",
    Description = ""A sample {type} plugin demonstrating basic functionality"",
    Author = ""Plugin Developer"",
    Category = ""{className}"")]
public class Sample{className}Plugin : I{className}Plugin
{{
    private IPluginContext? _context;
    private ILogger? _logger;

    public string Name => ""Sample {className} Plugin"";
    public string Version => ""1.0.0"";
    public string Description => ""A sample {type} plugin"";
    public string Author => ""Plugin Developer"";

    public Task InitializeAsync(IPluginContext context)
    {{
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = context.Logger;
        
        _logger.LogInformation(""Initializing {{PluginName}}"", Name);
        
        // Initialize plugin resources
        return Task.CompletedTask;
    }}

    public Task ShutdownAsync()
    {{
        _logger?.LogInformation(""Shutting down {{PluginName}}"", Name);
        
        // Clean up plugin resources
        return Task.CompletedTask;
    }}

    public bool IsCompatible(Version hostVersion)
    {{
        // Check if plugin is compatible with host version
        return hostVersion >= new Version(""1.0.0"");
    }}

    public async Task<{className}Result> Execute{className}Async({className}Input input)
    {{
        _logger?.LogInformation(""Executing {type} operation"");
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {{
            // Simulate {type} processing
            await Task.Delay(100, input.CancellationToken);
            
            var result = new {className}Result
            {{
                Success = true,
                Message = ""Operation completed successfully"",
                Data = $""Processed: {{input.Data}}"",
                ExecutionTime = stopwatch.Elapsed
            }};
            
            return result;
        }}
        catch (Exception ex)
        {{
            _logger?.LogError(ex, ""Error executing {type} operation"");
            
            return new {className}Result
            {{
                Success = false,
                Message = ex.Message,
                ExecutionTime = stopwatch.Elapsed
            }};
        }}
    }}

    public bool CanHandle({className}Input input)
    {{
        // Check if this plugin can handle the input
        return !string.IsNullOrEmpty(input.Data);
    }}

    public IEnumerable<{className}Capability> GetCapabilities()
    {{
        return new[]
        {{
            new {className}Capability
            {{
                Name = ""Basic {className}"",
                Description = ""Provides basic {type} functionality"",
                Properties = new Dictionary<string, object>
                {{
                    {{ ""MaxInputSize"", 1000000 }},
                    {{ ""SupportedFormats"", new[] {{ ""text"", ""json"" }} }}
                }}
            }}
        }};
    }}
}}

// Plugin Configuration
public class Sample{className}PluginConfig
{{
    public bool EnableLogging {{ get; set; }} = true;
    public int MaxConcurrentOperations {{ get; set; }} = 5;
    public TimeSpan OperationTimeout {{ get; set; }} = TimeSpan.FromMinutes(5);
    public Dictionary<string, string> CustomSettings {{ get; set; }} = new();
}}

// Plugin Factory (if using dependency injection)
[Export(typeof(I{className}Plugin))]
[ExportPlugin(""{className}Plugin"", ""1.0.0"", Description = ""Sample {type} plugin"")]
public class Sample{className}PluginFactory : I{className}Plugin
{{
    private readonly IServiceProvider _serviceProvider;
    
    [ImportingConstructor]
    public Sample{className}PluginFactory([Import] IServiceProvider serviceProvider)
    {{
        _serviceProvider = serviceProvider;
    }}
    
    // Implement interface methods...
    public string Name => ""Sample {className} Plugin"";
    public string Version => ""1.0.0"";
    public string Description => ""Factory-created sample plugin"";
    public string Author => ""Plugin Developer"";

    // Implementation details...
}}";
    }

    private static string GetLoadingStrategyDescription(string strategy)
    {
        return strategy switch
        {
            "static" => @"**Static Loading** loads all plugins at application startup:
- **Predictable**: All plugins available immediately
- **Simple**: Straightforward loading process
- **Resource Usage**: Higher memory usage from start
- **Startup Time**: Longer application startup",

            "dynamic" => @"**Dynamic Loading** loads plugins on-demand:
- **Efficient**: Load only when needed
- **Flexible**: Runtime plugin management
- **Scalable**: Better resource utilization
- **Complex**: More sophisticated loading logic",

            "lazy" => @"**Lazy Loading** defers plugin loading until first use:
- **Fast Startup**: Minimal initial loading
- **Memory Efficient**: Load plugins as needed
- **Just-in-Time**: Plugin activation on demand
- **Delayed Errors**: Loading errors occur during use",

            _ => "Standard plugin loading strategy"
        };
    }
}

// Extension method to convert strings to title case
public static class StringExtensions
{
    public static string ToTitleCase(this string input)
    {
        return string.IsNullOrEmpty(input) ? input : char.ToUpperInvariant(input[0]) + input[1..].ToLowerInvariant();
    }
}
