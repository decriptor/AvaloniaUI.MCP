using System.ComponentModel;

using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Tools;

[McpServerToolType]
public static class DataAccessPatternTool
{
    [McpServerTool, Description("Generates Entity Framework Core repository patterns with Unit of Work")]
    public static string GenerateEFCoreRepository(
        [Description("Entity name (e.g., User, Product)")] string entityName,
        [Description("Include Unit of Work: 'true' or 'false'")] string includeUnitOfWork = "true",
        [Description("Include specifications pattern: 'true' or 'false'")] string includeSpecifications = "true",
        [Description("Database provider: 'sqlserver', 'sqlite', 'postgresql', 'inmemory'")] string dbProvider = "sqlserver")
    {
        try
        {
            var config = new RepositoryConfiguration
            {
                EntityName = entityName,
                IncludeUnitOfWork = bool.Parse(includeUnitOfWork),
                IncludeSpecifications = bool.Parse(includeSpecifications),
                DatabaseProvider = dbProvider.ToLowerInvariant()
            };

            var entityCode = GenerateEntity(config);
            var repositoryInterface = GenerateRepositoryInterface(config);
            var repositoryImplementation = GenerateRepositoryImplementation(config);
            var dbContextCode = GenerateDbContext(config);
            var setupInstructions = GenerateEFSetupInstructions(config);

            return $@"# Entity Framework Core Repository Pattern: {entityName}

## Configuration
- **Entity**: {config.EntityName}
- **Unit of Work**: {config.IncludeUnitOfWork}
- **Specifications**: {config.IncludeSpecifications}
- **Database Provider**: {config.DatabaseProvider}

## Entity Definition
```csharp
{entityCode}
```

## Repository Interface
```csharp
{repositoryInterface}
```

## Repository Implementation
```csharp
{repositoryImplementation}
```

## DbContext Configuration
```csharp
{dbContextCode}
```

## Setup Instructions
{setupInstructions}";
        }
        catch (Exception ex)
        {
            return $"Error generating EF Core repository: {ex.Message}";
        }
    }

    [McpServerTool, Description("Creates async data access patterns with caching and error handling")]
    public static string GenerateAsyncDataAccess(
        [Description("Service name (e.g., UserDataService)")] string serviceName,
        [Description("Include caching: 'true' or 'false'")] string includeCaching = "true",
        [Description("Include retry policies: 'true' or 'false'")] string includeRetry = "true",
        [Description("Caching provider: 'memory', 'redis', 'distributed'")] string cachingProvider = "memory")
    {
        try
        {
            var config = new DataAccessConfiguration
            {
                ServiceName = serviceName,
                IncludeCaching = bool.Parse(includeCaching),
                IncludeRetry = bool.Parse(includeRetry),
                CachingProvider = cachingProvider.ToLowerInvariant()
            };

            var serviceInterface = GenerateAsyncServiceInterface(config);
            var serviceImplementation = GenerateAsyncServiceImplementation(config);
            var cachingCode = config.IncludeCaching ? GenerateCachingImplementation(config) : "";
            var retryCode = config.IncludeRetry ? GenerateRetryPolicyCode() : "";

            return $@"# Async Data Access Service: {serviceName}

## Configuration
- **Service**: {config.ServiceName}
- **Caching**: {config.IncludeCaching} ({config.CachingProvider})
- **Retry Policies**: {config.IncludeRetry}

## Service Interface
```csharp
{serviceInterface}
```

## Service Implementation
```csharp
{serviceImplementation}
```

{(config.IncludeCaching ? $@"## Caching Implementation
```csharp
{cachingCode}
```" : "")}

{(config.IncludeRetry ? $@"## Retry Policy Configuration
```csharp
{retryCode}
```" : "")}

## Performance Considerations
- **Async/Await**: All database operations are async
- **Cancellation Tokens**: Support for operation cancellation
- **Connection Pooling**: Efficient database connection management
- **Query Optimization**: Use appropriate loading strategies";
        }
        catch (Exception ex)
        {
            return $"Error generating async data access: {ex.Message}";
        }
    }

    private class RepositoryConfiguration
    {
        public string EntityName { get; set; } = "";
        public bool IncludeUnitOfWork { get; set; }
        public bool IncludeSpecifications { get; set; }
        public string DatabaseProvider { get; set; } = "";
    }

    private class DataAccessConfiguration
    {
        public string ServiceName { get; set; } = "";
        public bool IncludeCaching { get; set; }
        public bool IncludeRetry { get; set; }
        public string CachingProvider { get; set; } = "";
    }

    private static string GenerateEntity(RepositoryConfiguration config)
    {
        return $@"using System.ComponentModel.DataAnnotations;

public class {config.EntityName}
{{
    public int Id {{ get; set; }}
    
    [Required]
    [StringLength(100)]
    public string Name {{ get; set; }} = string.Empty;
    
    [StringLength(500)]
    public string Description {{ get; set; }} = string.Empty;
    
    public DateTime CreatedAt {{ get; set; }} = DateTime.UtcNow;
    public DateTime? UpdatedAt {{ get; set; }}
    
    [Required]
    public string CreatedBy {{ get; set; }} = string.Empty;
    public string? UpdatedBy {{ get; set; }}
    
    public bool IsActive {{ get; set; }} = true;
    
    // Navigation properties
    // public virtual ICollection<Related{config.EntityName}> Related{{ get; set; }} = new List<Related{config.EntityName}>();
}}

// Entity configuration
public class {config.EntityName}Configuration : IEntityTypeConfiguration<{config.EntityName}>
{{
    public void Configure(EntityTypeBuilder<{config.EntityName}> builder)
    {{
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.Description)
            .HasMaxLength(500);
            
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql(""GETUTCDATE()"");
            
        builder.Property(e => e.CreatedBy)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(50);
            
        builder.HasIndex(e => e.Name)
            .IsUnique();
            
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.IsActive);
    }}
}}";
    }

    private static string GenerateRepositoryInterface(RepositoryConfiguration config)
    {
        var specificationsCode = config.IncludeSpecifications ? GenerateSpecificationsInterface(config.EntityName) : "";

        return $@"public interface I{config.EntityName}Repository : IRepository<{config.EntityName}>
{{
    Task<{config.EntityName}?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<{config.EntityName}?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<{config.EntityName}>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<IPagedResult<{config.EntityName}>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CountActiveAsync(CancellationToken cancellationToken = default);
    
    {(config.IncludeSpecifications ? $"Task<IEnumerable<{config.EntityName}>> FindAsync(ISpecification<{config.EntityName}> specification, CancellationToken cancellationToken = default);" : "")}
}}

// Base repository interface
public interface IRepository<T> where T : class
{{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
}}

{specificationsCode}";
    }

    private static string GenerateSpecificationsInterface(string entityName)
    {
        return $@"
// Specification pattern interfaces
public interface ISpecification<T>
{{
    Expression<Func<T, bool>> Criteria {{ get; }}
    List<Expression<Func<T, object>>> Includes {{ get; }}
    Expression<Func<T, object>>? OrderBy {{ get; }}
    Expression<Func<T, object>>? OrderByDescending {{ get; }}
    bool IsPagingEnabled {{ get; }}
    int Take {{ get; }}
    int Skip {{ get; }}
}}

// Example specification for {entityName}
public class Active{entityName}Specification : Specification<{entityName}>
{{
    public Active{entityName}Specification() : base(x => x.IsActive) {{ }}
}}

public class {entityName}ByNameSpecification : Specification<{entityName}>
{{
    public {entityName}ByNameSpecification(string name) : base(x => x.Name.Contains(name)) {{ }}
}}";
    }

    private static string GenerateRepositoryImplementation(RepositoryConfiguration config)
    {
        var unitOfWorkField = config.IncludeUnitOfWork ? "private readonly IUnitOfWork _unitOfWork;" : "";
        var unitOfWorkParam = config.IncludeUnitOfWork ? ", IUnitOfWork unitOfWork" : "";
        var unitOfWorkAssignment = config.IncludeUnitOfWork ? "_unitOfWork = unitOfWork;" : "";
        var specificationsMethod = config.IncludeSpecifications ? GenerateSpecificationsMethod(config.EntityName) : "";

        return $@"public class {config.EntityName}Repository : I{config.EntityName}Repository
{{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<{config.EntityName}Repository> _logger;
    {unitOfWorkField}

    public {config.EntityName}Repository(
        ApplicationDbContext context,
        ILogger<{config.EntityName}Repository> logger{unitOfWorkParam})
    {{
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        {unitOfWorkAssignment}
    }}

    public async Task<{config.EntityName}?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {{
        return await _context.Set<{config.EntityName}>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }}

    public async Task<{config.EntityName}?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {{
        return await _context.Set<{config.EntityName}>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }}

    public async Task<IEnumerable<{config.EntityName}>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {{
        return await _context.Set<{config.EntityName}>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }}

    public async Task<IPagedResult<{config.EntityName}>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {{
        var query = _context.Set<{config.EntityName}>()
            .AsNoTracking()
            .Where(x => x.IsActive);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<{config.EntityName}>(items, totalCount, pageNumber, pageSize);
    }}

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {{
        return await _context.Set<{config.EntityName}>()
            .AnyAsync(x => x.Id == id, cancellationToken);
    }}

    public async Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
    {{
        return await _context.Set<{config.EntityName}>()
            .CountAsync(x => x.IsActive, cancellationToken);
    }}

    public async Task<{config.EntityName}> AddAsync({config.EntityName} entity, CancellationToken cancellationToken = default)
    {{
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        entity.CreatedAt = DateTime.UtcNow;
        var result = await _context.Set<{config.EntityName}>().AddAsync(entity, cancellationToken);
        
        _logger.LogInformation(""Added new {config.EntityName}: {{Name}}"", entity.Name);
        return result.Entity;
    }}

    public async Task<IEnumerable<{config.EntityName}>> AddRangeAsync(IEnumerable<{config.EntityName}> entities, CancellationToken cancellationToken = default)
    {{
        if (entities == null) throw new ArgumentNullException(nameof(entities));

        var entityList = entities.ToList();
        foreach (var entity in entityList)
        {{
            entity.CreatedAt = DateTime.UtcNow;
        }}

        await _context.Set<{config.EntityName}>().AddRangeAsync(entityList, cancellationToken);
        
        _logger.LogInformation(""Added {{Count}} {config.EntityName} entities"", entityList.Count);
        return entityList;
    }}

    public Task UpdateAsync({config.EntityName} entity, CancellationToken cancellationToken = default)
    {{
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        entity.UpdatedAt = DateTime.UtcNow;
        _context.Set<{config.EntityName}>().Update(entity);
        
        _logger.LogInformation(""Updated {config.EntityName}: {{Id}}"", entity.Id);
        return Task.CompletedTask;
    }}

    public Task DeleteAsync({config.EntityName} entity, CancellationToken cancellationToken = default)
    {{
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        _context.Set<{config.EntityName}>().Remove(entity);
        
        _logger.LogInformation(""Deleted {config.EntityName}: {{Id}}"", entity.Id);
        return Task.CompletedTask;
    }}

    public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {{
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {{
            await DeleteAsync(entity, cancellationToken);
        }}
    }}

    {specificationsMethod}
}}";
    }

    private static string GenerateSpecificationsMethod(string entityName)
    {
        return $@"
    public async Task<IEnumerable<{entityName}>> FindAsync(ISpecification<{entityName}> specification, CancellationToken cancellationToken = default)
    {{
        var query = _context.Set<{entityName}>().AsQueryable();

        // Apply criteria
        if (specification.Criteria != null)
        {{
            query = query.Where(specification.Criteria);
        }}

        // Apply includes
        query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

        // Apply ordering
        if (specification.OrderBy != null)
        {{
            query = query.OrderBy(specification.OrderBy);
        }}
        else if (specification.OrderByDescending != null)
        {{
            query = query.OrderByDescending(specification.OrderByDescending);
        }}

        // Apply paging
        if (specification.IsPagingEnabled)
        {{
            query = query.Skip(specification.Skip).Take(specification.Take);
        }}

        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }}";
    }

    private static string GenerateDbContext(RepositoryConfiguration config)
    {
        var connectionString = GetConnectionString(config.DatabaseProvider);

        return $@"public class ApplicationDbContext : DbContext
{{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {{ }}

    public DbSet<{config.EntityName}> {config.EntityName}s {{ get; set; }}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {{
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new {config.EntityName}Configuration());
        
        // Global query filters
        modelBuilder.Entity<{config.EntityName}>().HasQueryFilter(e => e.IsActive);

        // Seed data
        SeedData(modelBuilder);
    }}

    private static void SeedData(ModelBuilder modelBuilder)
    {{
        modelBuilder.Entity<{config.EntityName}>().HasData(
            new {config.EntityName}
            {{
                Id = 1,
                Name = ""Sample {config.EntityName}"",
                Description = ""This is a sample {config.EntityName.ToLowerInvariant()}"",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = ""System"",
                IsActive = true
            }}
        );
    }}

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {{
        // Auto-update audit fields
        UpdateAuditFields();
        
        return await base.SaveChangesAsync(cancellationToken);
    }}

    private void UpdateAuditFields()
    {{
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {{
            if (entry.State == EntityState.Added)
            {{
                if (entry.Property(""CreatedAt"").CurrentValue == null)
                    entry.Property(""CreatedAt"").CurrentValue = DateTime.UtcNow;
            }}
            else if (entry.State == EntityState.Modified)
            {{
                entry.Property(""UpdatedAt"").CurrentValue = DateTime.UtcNow;
            }}
        }}
    }}
}}

// DbContext factory for design-time operations
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{{
    public ApplicationDbContext CreateDbContext(string[] args)
    {{
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.{GetProviderMethod(config.DatabaseProvider)}(""{connectionString}"");

        return new ApplicationDbContext(optionsBuilder.Options);
    }}
}}";
    }

    private static string GetConnectionString(string provider)
    {
        return provider switch
        {
            "sqlserver" => "Server=(localdb)\\mssqllocaldb;Database=YourAppDb;Trusted_Connection=true;MultipleActiveResultSets=true",
            "sqlite" => "Data Source=yourapp.db",
            "postgresql" => "Host=localhost;Database=yourappdb;Username=postgres;Password=password",
            "inmemory" => "InMemoryDatabase",
            _ => "DefaultConnectionString"
        };
    }

    private static string GetProviderMethod(string provider)
    {
        return provider switch
        {
            "sqlserver" => "UseSqlServer",
            "sqlite" => "UseSqlite",
            "postgresql" => "UseNpgsql",
            "inmemory" => "UseInMemoryDatabase",
            _ => "UseSqlServer"
        };
    }

    private static string GenerateEFSetupInstructions(RepositoryConfiguration config)
    {
        var packageReference = GetPackageReference(config.DatabaseProvider);

        return $@"### 1. Install Required Packages
```xml
<PackageReference Include=""Microsoft.EntityFrameworkCore"" Version=""8.0.0"" />
<PackageReference Include=""Microsoft.EntityFrameworkCore.Design"" Version=""8.0.0"" />
{packageReference}
</PackageReference>
```

### 2. Configure Services in Program.cs
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.{GetProviderMethod(config.DatabaseProvider)}(
        builder.Configuration.GetConnectionString(""DefaultConnection"")));

builder.Services.AddScoped<I{config.EntityName}Repository, {config.EntityName}Repository>();
{(config.IncludeUnitOfWork ? "builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();" : "")}
```

### 3. Connection String in appsettings.json
```json
{{
  ""ConnectionStrings"": {{
    ""DefaultConnection"": ""{GetConnectionString(config.DatabaseProvider)}""
  }}
}}
```

### 4. Create and Run Migrations
```bash
# Add migration
dotnet ef migrations add Initial{config.EntityName}

# Update database
dotnet ef database update
```";
    }

    private static string GetPackageReference(string provider)
    {
        return provider switch
        {
            "sqlserver" => "<PackageReference Include=\"Microsoft.EntityFrameworkCore.SqlServer\" Version=\"8.0.0\" />",
            "sqlite" => "<PackageReference Include=\"Microsoft.EntityFrameworkCore.Sqlite\" Version=\"8.0.0\" />",
            "postgresql" => "<PackageReference Include=\"Npgsql.EntityFrameworkCore.PostgreSQL\" Version=\"8.0.0\" />",
            "inmemory" => "<PackageReference Include=\"Microsoft.EntityFrameworkCore.InMemory\" Version=\"8.0.0\" />",
            _ => "<PackageReference Include=\"Microsoft.EntityFrameworkCore.SqlServer\" Version=\"8.0.0\" />"
        };
    }

    private static string GenerateAsyncServiceInterface(DataAccessConfiguration config)
    {
        return $@"public interface I{config.ServiceName}
{{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync<T>(CancellationToken cancellationToken = default);
    Task<IPagedResult<T>> GetPagedAsync<T>(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<T> CreateAsync<T>(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync<T>(T entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync<T>(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync<T>(string key, CancellationToken cancellationToken = default);
    
    // Bulk operations
    Task<IEnumerable<T>> CreateBulkAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<bool> DeleteBulkAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default);
    
    // Search operations
    Task<IEnumerable<T>> SearchAsync<T>(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FilterAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}}";
    }

    private static string GenerateAsyncServiceImplementation(DataAccessConfiguration config)
    {
        var cachingField = config.IncludeCaching ? GenerateCachingField(config.CachingProvider) : "";
        var cachingParam = config.IncludeCaching ? GenerateCachingParameter(config.CachingProvider) : "";
        var cachingAssignment = config.IncludeCaching ? GenerateCachingAssignment(config.CachingProvider) : "";

        return $@"public class {config.ServiceName} : I{config.ServiceName}
{{
    private readonly IRepository _repository;
    private readonly ILogger<{config.ServiceName}> _logger;
    {cachingField}
    {(config.IncludeRetry ? "private readonly IAsyncPolicy _retryPolicy;" : "")}

    public {config.ServiceName}(
        IRepository repository,
        ILogger<{config.ServiceName}> logger{cachingParam}
        {(config.IncludeRetry ? ", IAsyncPolicy retryPolicy" : "")})
    {{
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        {cachingAssignment}
        {(config.IncludeRetry ? "_retryPolicy = retryPolicy ?? throw new ArgumentNullException(nameof(retryPolicy));" : "")}
    }}

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {{
        var operation = async () =>
        {{
            {(config.IncludeCaching ? $@"// Check cache first
            var cacheKey = GenerateCacheKey<T>(key);
            var cached = await {GetCacheGetMethod(config.CachingProvider)}(cacheKey, cancellationToken);
            if (cached != null)
            {{
                _logger.LogDebug(""Cache hit for key: {{Key}}"", cacheKey);
                return cached;
            }}" : "")}

            // Get from repository
            var result = await _repository.GetByIdAsync<T>(key, cancellationToken);
            
            {(config.IncludeCaching ? $@"// Cache the result
            if (result != null)
            {{
                await {GetCacheSetMethod(config.CachingProvider)}(cacheKey, result, TimeSpan.FromMinutes(30), cancellationToken);
            }}" : "")}

            return result;
        }};

        {(config.IncludeRetry ? "return await _retryPolicy.ExecuteAsync(operation);" : "return await operation();")}
    }}

    public async Task<IEnumerable<T>> GetAllAsync<T>(CancellationToken cancellationToken = default)
    {{
        var operation = async () =>
        {{
            _logger.LogInformation(""Retrieving all entities of type {{Type}}"", typeof(T).Name);
            return await _repository.GetAllAsync<T>(cancellationToken);
        }};

        {(config.IncludeRetry ? "return await _retryPolicy.ExecuteAsync(operation);" : "return await operation();")}
    }}

    public async Task<IPagedResult<T>> GetPagedAsync<T>(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {{
        var operation = async () =>
        {{
            _logger.LogInformation(""Retrieving paged data: Page {{Page}}, Size {{Size}}"", pageNumber, pageSize);
            return await _repository.GetPagedAsync<T>(pageNumber, pageSize, cancellationToken);
        }};

        {(config.IncludeRetry ? "return await _retryPolicy.ExecuteAsync(operation);" : "return await operation();")}
    }}

    public async Task<T> CreateAsync<T>(T entity, CancellationToken cancellationToken = default)
    {{
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var operation = async () =>
        {{
            var result = await _repository.AddAsync(entity, cancellationToken);
            
            {(config.IncludeCaching ? @"// Invalidate relevant cache entries
            await InvalidateCacheAsync<T>();" : "")}
            
            _logger.LogInformation(""Created new entity of type {{Type}}"", typeof(T).Name);
            return result;
        }};

        {(config.IncludeRetry ? "return await _retryPolicy.ExecuteAsync(operation);" : "return await operation();")}
    }}

    public async Task<T> UpdateAsync<T>(T entity, CancellationToken cancellationToken = default)
    {{
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var operation = async () =>
        {{
            var result = await _repository.UpdateAsync(entity, cancellationToken);
            
            {(config.IncludeCaching ? @"// Invalidate cache
            await InvalidateCacheAsync<T>();" : "")}
            
            _logger.LogInformation(""Updated entity of type {{Type}}"", typeof(T).Name);
            return result;
        }};

        {(config.IncludeRetry ? "return await _retryPolicy.ExecuteAsync(operation);" : "return await operation();")}
    }}

    public async Task<bool> DeleteAsync<T>(string key, CancellationToken cancellationToken = default)
    {{
        var operation = async () =>
        {{
            var result = await _repository.DeleteAsync<T>(key, cancellationToken);
            
            {(config.IncludeCaching ? @"// Invalidate cache
            await InvalidateCacheAsync<T>();" : "")}
            
            _logger.LogInformation(""Deleted entity with key {{Key}}"", key);
            return result;
        }};

        {(config.IncludeRetry ? "return await _retryPolicy.ExecuteAsync(operation);" : "return await operation();")}
    }}

    {(config.IncludeCaching ? GenerateCacheHelperMethods(config.CachingProvider) : "")}
}}";
    }

    private static string GenerateCachingField(string provider)
    {
        return provider switch
        {
            "memory" => "private readonly IMemoryCache _cache;",
            "redis" => "private readonly IDistributedCache _cache;",
            "distributed" => "private readonly IDistributedCache _cache;",
            _ => "private readonly IMemoryCache _cache;"
        };
    }

    private static string GenerateCachingParameter(string provider)
    {
        return provider switch
        {
            "memory" => ", IMemoryCache cache",
            "redis" => ", IDistributedCache cache",
            "distributed" => ", IDistributedCache cache",
            _ => ", IMemoryCache cache"
        };
    }

    private static string GenerateCachingAssignment(string provider)
    {
        return "_cache = cache ?? throw new ArgumentNullException(nameof(cache));";
    }

    private static string GetCacheGetMethod(string provider)
    {
        return provider switch
        {
            "memory" => "_cache.TryGetValue",
            "redis" => "_cache.GetAsync",
            "distributed" => "_cache.GetAsync",
            _ => "_cache.TryGetValue"
        };
    }

    private static string GetCacheSetMethod(string provider)
    {
        return provider switch
        {
            "memory" => "_cache.Set",
            "redis" => "_cache.SetAsync",
            "distributed" => "_cache.SetAsync",
            _ => "_cache.Set"
        };
    }

    private static string GenerateCacheHelperMethods(string provider)
    {
        return $@"
    private string GenerateCacheKey<T>(string key)
    {{
        return $""{{typeof(T).Name}}:{{key}}"";
    }}

    private async Task InvalidateCacheAsync<T>()
    {{
        // Implementation depends on caching provider
        // For memory cache, you might need to track keys
        // For Redis, you can use pattern-based deletion
        _logger.LogDebug(""Invalidating cache for type {{Type}}"", typeof(T).Name);
    }}";
    }

    private static string GenerateCachingImplementation(DataAccessConfiguration config)
    {
        return $@"// Caching service implementation for {config.CachingProvider}
public class CachingService : ICachingService
{{
    private readonly {GetCacheInterface(config.CachingProvider)} _cache;
    private readonly ILogger<CachingService> _logger;

    public CachingService({GetCacheInterface(config.CachingProvider)} cache, ILogger<CachingService> logger)
    {{
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }}

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {{
        try
        {{
            {GetCacheGetImplementation(config.CachingProvider)}
        }}
        catch (Exception ex)
        {{
            _logger.LogWarning(ex, ""Failed to get cache value for key {{Key}}"", key);
            return default;
        }}
    }}

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {{
        try
        {{
            {GetCacheSetImplementation(config.CachingProvider)}
        }}
        catch (Exception ex)
        {{
            _logger.LogWarning(ex, ""Failed to set cache value for key {{Key}}"", key);
        }}
    }}

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {{
        try
        {{
            {GetCacheRemoveImplementation(config.CachingProvider)}
        }}
        catch (Exception ex)
        {{
            _logger.LogWarning(ex, ""Failed to remove cache value for key {{Key}}"", key);
        }}
    }}
}}";
    }

    private static string GetCacheInterface(string provider)
    {
        return provider switch
        {
            "memory" => "IMemoryCache",
            "redis" => "IDistributedCache",
            "distributed" => "IDistributedCache",
            _ => "IMemoryCache"
        };
    }

    private static string GetCacheGetImplementation(string provider)
    {
        return provider switch
        {
            "memory" => @"if (_cache.TryGetValue(key, out var cached) && cached is T typedValue)
            {
                return typedValue;
            }
            return default;",

            "redis" or "distributed" => @"var json = await _cache.GetStringAsync(key, cancellationToken);
            if (!string.IsNullOrEmpty(json))
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            return default;",

            _ => "return default;"
        };
    }

    private static string GetCacheSetImplementation(string provider)
    {
        return provider switch
        {
            "memory" => "_cache.Set(key, value, expiration);",

            "redis" or "distributed" => @"var json = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };
            await _cache.SetStringAsync(key, json, options, cancellationToken);",

            _ => "// No implementation"
        };
    }

    private static string GetCacheRemoveImplementation(string provider)
    {
        return provider switch
        {
            "memory" => "_cache.Remove(key);",
            "redis" or "distributed" => "await _cache.RemoveAsync(key, cancellationToken);",
            _ => "// No implementation"
        };
    }

    private static string GenerateRetryPolicyCode()
    {
        return @"// Retry policy configuration using Polly
public static class RetryPolicies
{
    public static IAsyncPolicy CreateDatabaseRetryPolicy()
    {
        return Policy
            .Handle<SqlException>()
            .Or<TimeoutException>()
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    var logger = context.GetLogger();
                    logger?.LogWarning(""Retry {RetryCount} after {Delay}ms"", retryCount, timespan.TotalMilliseconds);
                });
    }

    public static IAsyncPolicy CreateCircuitBreakerPolicy()
    {
        return Policy
            .Handle<SqlException>()
            .Or<TimeoutException>()
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

    public static IAsyncPolicy CreateCombinedPolicy()
    {
        var retryPolicy = CreateDatabaseRetryPolicy();
        var circuitBreakerPolicy = CreateCircuitBreakerPolicy();
        
        return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
    }
}

// Service registration
services.AddSingleton<IAsyncPolicy>(provider => 
    RetryPolicies.CreateCombinedPolicy());";
    }
}