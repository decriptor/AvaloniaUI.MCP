# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files first for better layer caching
COPY global.json ./
COPY Directory.*.props ./
COPY *.slnx ./
COPY src/AvaloniaUI.MCP/*.csproj ./src/AvaloniaUI.MCP/
COPY tests/AvaloniaUI.MCP.Tests/*.csproj ./tests/AvaloniaUI.MCP.Tests/

# Restore dependencies (cached if project files haven't changed)
RUN dotnet restore src/AvaloniaUI.MCP/AvaloniaUI.MCP.csproj

# Copy source code
COPY src/ ./src/
COPY tests/ ./tests/

# Build and test
RUN dotnet build src/AvaloniaUI.MCP/AvaloniaUI.MCP.csproj -c Release --no-restore && \
    dotnet test tests/AvaloniaUI.MCP.Tests/AvaloniaUI.MCP.Tests.csproj -c Release --no-build --verbosity normal

# Publish the application with optimizations
RUN dotnet publish src/AvaloniaUI.MCP/AvaloniaUI.MCP.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    --self-contained false \
    -p:PublishSingleFile=false \
    -p:PublishTrimmed=false

# Runtime stage - Use Ubuntu Chiseled for minimal size (~30MB vs ~180MB)
FROM mcr.microsoft.com/dotnet/runtime:9.0-noble-chiseled AS runtime
WORKDIR /app

# Copy published app (chiseled images already have non-root user)
COPY --from=build /app/publish .

# Set environment variables
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_URLS=
ENV DOTNET_EnableDiagnostics=0
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

# Health check optimized for chiseled containers (no shell available)
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD ["dotnet", "AvaloniaUI.MCP.dll", "--help"]

# Labels for metadata
LABEL org.opencontainers.image.title="AvaloniaUI MCP Server"
LABEL org.opencontainers.image.description="Comprehensive Model Context Protocol server for AvaloniaUI development"
LABEL org.opencontainers.image.vendor="AvaloniaUI.MCP"
LABEL org.opencontainers.image.source="https://github.com/decriptor/AvaloniaUI.MCP"
LABEL org.opencontainers.image.documentation="https://github.com/decriptor/AvaloniaUI.MCP/blob/main/README.md"

# Default entrypoint
ENTRYPOINT ["dotnet", "AvaloniaUI.MCP.dll"]
