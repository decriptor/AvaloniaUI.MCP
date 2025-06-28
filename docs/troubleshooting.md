# Troubleshooting Guide

Common issues and solutions for AvaloniaUI.MCP server.

## 🚨 Server Issues

### Server Won't Start

#### Symptom
```
Error: The server failed to start or connection refused
```

#### Solutions

**Check .NET Version**
```bash
dotnet --version
# Should show 9.0.x or later
```

**Verify Project Build**
```bash
cd AvaloniaUI.MCP
dotnet clean
dotnet build
```

**Check for Port Conflicts**
```bash
# Kill any existing processes
pkill -f "AvaloniaUI.MCP"

# Start with verbose logging
export AVALONIA_MCP_LOG_LEVEL=Debug
dotnet run --project src/AvaloniaUI.MCP/AvaloniaUI.MCP.csproj
```

**Validate Configuration**
```bash
# Test server manually
echo '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05","capabilities":{},"clientInfo":{"name":"test","version":"1.0.0"}}}' | dotnet run --project src/AvaloniaUI.MCP/AvaloniaUI.MCP.csproj
```

### Server Crashes

#### Symptom
```
Unhandled exception: System.OutOfMemoryException
```

#### Solutions

**Memory Management**
```bash
# Enable server GC
export DOTNET_gcServer=1
export DOTNET_gcConcurrent=1

# Set memory limits
export DOTNET_GCHeapHardLimit=400000000  # 400MB limit
```

**Check Resource Usage**
```
Use the DiagnosticTool: "Perform a health check"
Use the DiagnosticTool: "Force garbage collection"
```

**Review Logs**
```bash
export AVALONIA_MCP_LOG_LEVEL=Information
dotnet run --project src/AvaloniaUI.MCP/AvaloniaUI.MCP.csproj 2>&1 | tee server.log
```

### Connection Issues

#### Symptom
```
MCP client cannot connect to server
```

#### Solutions

**Verify STDIO Configuration**
```json
{
  "mcpServers": {
    "avalonia": {
      "command": "dotnet",
      "args": [
        "run", 
        "--project", 
        "/absolute/path/to/AvaloniaUI.MCP/src/AvaloniaUI.MCP/AvaloniaUI.MCP.csproj"
      ],
      "cwd": "/absolute/path/to/AvaloniaUI.MCP"
    }
  }
}
```

**Test Connection**
```bash
# Manual test
echo '{"jsonrpc":"2.0","id":1,"method":"tools/list","params":{}}' | dotnet run --project src/AvaloniaUI.MCP/AvaloniaUI.MCP.csproj
```

**Check Permissions**
```bash
# Ensure executable permissions
chmod +x src/AvaloniaUI.MCP/bin/Debug/net9.0/AvaloniaUI.MCP

# Check file access
ls -la src/AvaloniaUI.MCP/AvaloniaUI.MCP.csproj
```

## 🛠️ Tool Issues

### Tool Execution Failures

#### Symptom
```
Error: Tool execution failed with status code 1
```

#### Solutions

**Check Tool Metrics**
```
Use DiagnosticTool: "Get server metrics"
```

**Validate Input Parameters**
```
"Validate my project name 'My-Invalid@Name'"
# Should suggest valid alternatives
```

**Test Individual Tools**
```
"Test the echo tool with message 'hello world'"
# Should respond with the echoed message
```

### Validation Errors

#### Symptom
```
Error: Invalid XAML syntax
```

#### Solutions

**Get Detailed Validation**
```
"Validate this XAML and provide detailed error information"
```

**Check Common Issues**
- Missing namespace declarations
- Unclosed tags
- Invalid property names
- Binding syntax errors

**Use Incremental Validation**
```
"Validate just the Window tag first"
"Now validate with the StackPanel added"
```

### Generation Failures

#### Symptom
```
Error: Project generation failed
```

#### Solutions

**Check Directory Permissions**
```bash
# Ensure write access
touch test-file.txt && rm test-file.txt
```

**Validate Project Name**
```
"Is 'MyApp123' a valid project name?"
```

**Try Simplified Generation**
```
"Create a basic project instead of MVVM"
"Generate only the essential files"
```

## 📊 Performance Issues

### Slow Response Times

#### Symptom
```
Tool responses taking > 5 seconds
```

#### Solutions

**Check Cache Performance**
```
Use DiagnosticTool: "Get server metrics"
# Look for cache hit rate < 80%
```

**Clear Cache**
```
Use DiagnosticTool: "Clear resource cache"
```

**Optimize Environment**
```bash
# Production settings
export ENVIRONMENT=production
export AVALONIA_MCP_LOG_LEVEL=Warning
export DOTNET_gcServer=1
```

**Monitor Resource Usage**
```
Use DiagnosticTool: "Force garbage collection and show memory stats"
```

### High Memory Usage

#### Symptom
```
Server memory usage > 500MB
```

#### Solutions

**Check Memory Pressure**
```
Use DiagnosticTool: "Perform health check"
# Will show memory usage warnings
```

**Restart Server Periodically**
```bash
# Schedule periodic restarts for long-running instances
# Add to cron: 0 */6 * * * pkill -f AvaloniaUI.MCP
```

**Optimize Caching**
```bash
# Reduce cache size
export AVALONIA_MCP_CACHE_SIZE=50  # Default 100
```

## 🐛 Development Issues

### Build Errors

#### Symptom
```
CS0246: The type or namespace name could not be found
```

#### Solutions

**Restore Packages**
```bash
dotnet clean
dotnet restore
dotnet build
```

**Check Package Versions**
```bash
dotnet list package --outdated
dotnet list package --vulnerable
```

**Verify SDK Version**
```bash
dotnet --list-sdks
# Should include 9.0.x
```

### Test Failures

#### Symptom
```
Test run failed: X tests failed
```

#### Solutions

**Run Specific Test Categories**
```bash
# Run only unit tests
dotnet test --filter Category=Unit

# Run only integration tests
dotnet test --filter Category=Integration
```

**Check Test Output**
```bash
dotnet test --verbosity detailed
```

**Reset Test Environment**
```bash
# Clean test artifacts
rm -rf tests/*/bin tests/*/obj
dotnet test
```

### Runtime Exceptions

#### Symptom
```
System.ArgumentNullException: Value cannot be null
```

#### Solutions

**Enable Detailed Logging**
```bash
export AVALONIA_MCP_LOG_LEVEL=Trace
```

**Check Input Validation**
```
"Test input validation with empty parameters"
```

**Use Error Handling Tools**
```
"Show me the last 10 error events"
Use DiagnosticTool: "Test logging at error level"
```

## 📋 Diagnostic Commands

### Health Monitoring

```
"Perform a comprehensive health check"
"Get current server metrics"
"Show memory usage and GC statistics"
"Test all service components"
```

### Performance Analysis

```
"Force garbage collection"
"Clear all caches"
"Show cache hit rates"
"Display tool execution statistics"
```

### Logging and Debug

```
"Test logging functionality at debug level"
"Show recent error events"
"Display server configuration"
"Check file system permissions"
```

## 🔧 Environment Variables

### Logging Configuration
```bash
export AVALONIA_MCP_LOG_LEVEL=Debug          # Trace|Debug|Information|Warning|Error|Critical
export ENVIRONMENT=development               # development|staging|production
```

### Performance Tuning
```bash
export DOTNET_gcServer=1                     # Enable server GC
export DOTNET_gcConcurrent=1                 # Enable concurrent GC
export DOTNET_GCHeapHardLimit=400000000      # 400MB memory limit
```

### Cache Configuration
```bash
export AVALONIA_MCP_CACHE_SIZE=100           # Number of cached items
export AVALONIA_MCP_CACHE_TTL=3600           # Cache TTL in seconds
```

## 📞 Getting Help

### Self-Diagnosis
1. Use DiagnosticTool for health checks
2. Review server logs with debug level
3. Test individual components
4. Check environment configuration

### Community Support
- [GitHub Issues](https://github.com/decriptor/AvaloniaUI.MCP/issues)
- [Discussions](https://github.com/decriptor/AvaloniaUI.MCP/discussions)
- [Documentation](./README.md)

### Reporting Bugs

When reporting issues, include:
- Server version and .NET version
- Operating system and architecture
- Complete error message and stack trace
- Steps to reproduce
- Server configuration and environment variables
- Output from health check diagnostic

### Example Bug Report Template

```markdown
## Bug Report

**Environment:**
- OS: Ubuntu 22.04 / Windows 11 / macOS 13
- .NET Version: 9.0.x
- Server Version: 1.0.0

**Issue:**
Brief description of the problem

**Steps to Reproduce:**
1. Start server with: `dotnet run...`
2. Execute command: "Create project..."
3. Error occurs

**Expected Behavior:**
What should happen

**Actual Behavior:**
What actually happens

**Logs:**
```
[Paste relevant log output]
```

**Health Check Output:**
```
[Output from diagnostic health check]
```

**Additional Context:**
Any other relevant information
```