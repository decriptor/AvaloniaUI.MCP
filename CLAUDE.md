# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is an AvaloniaUI.MCP project - a comprehensive Model Context Protocol (MCP) server that provides extensive AvaloniaUI knowledge and development assistance. The server is built on .NET 9.0 and implements the official Microsoft MCP SDK to deliver tools, resources, and prompts for AvaloniaUI development.

## Development Environment

- **Target Framework**: .NET 9.0 (net9.0)
- **Required SDK**: 9.0.300 (specified in global.json)
- **Language Features**: C# with ImplicitUsings and Nullable reference types enabled
- **Package Management**: Central Package Management is enabled (ManagePackageVersionsCentrally=true)
- **MCP SDK**: ModelContextProtocol 0.3.0-preview.1 for server implementation

## Project Structure

```
AvaloniaUI.MCP/
├── AvaloniaUI.MCP.slnx           # Solution file
├── global.json                   # SDK version pinning
├── Directory.Build.props         # MSBuild properties
├── Directory.Build.targets       # MSBuild targets with custom build message
├── Directory.Packages.props      # Central package management
├── nuget.config                  # NuGet configuration
├── src/
│   └── AvaloniaUI.MCP/
│       ├── AvaloniaUI.MCP.csproj # Main project file
│       ├── Program.cs            # MCP server entry point
│       ├── Tools/                # MCP tools implementation
│       │   ├── EchoTool.cs
│       │   ├── ProjectGeneratorTool.cs
│       │   └── XamlValidationTool.cs
│       ├── Resources/            # MCP resources (knowledge base)
│       │   ├── AvaloniaControlsResource.cs
│       │   ├── XamlPatternsResource.cs
│       │   └── MigrationGuideResource.cs
│       ├── Prompts/              # MCP prompt templates
│       │   └── AvaloniaPrompts.cs
│       └── Data/                 # Knowledge base data files
│           ├── controls.json
│           ├── xaml-patterns.json
│           └── migration-guide.json
└── tests/
    └── AvaloniaUI.MCP.Tests/
        ├── AvaloniaUI.MCP.Tests.csproj
        ├── ToolsTests.cs
        └── XamlValidationDebugTest.cs
```

## Common Commands

### Building
```bash
dotnet build
```

### Running the MCP Server
```bash
dotnet run --project src/AvaloniaUI.MCP/AvaloniaUI.MCP.csproj
```

### Testing
```bash
dotnet test
```

### MCP Server Usage
The server implements STDIO transport and can be used with any MCP-compatible client:
- Claude Desktop (with MCP configuration)
- VS Code with MCP extensions
- Custom MCP clients

## MCP Server Capabilities

### Tools (Actions)
- **CreateAvaloniaProject**: Generate new AvaloniaUI projects with MVVM, basic, or cross-platform templates
- **ValidateXaml**: Validate AvaloniaUI XAML syntax and common issues
- **ConvertWpfXamlToAvalonia**: Convert WPF XAML to AvaloniaUI format
- **Echo**: Simple echo tool for testing server connectivity
- **GetServerInfo**: Server information and capabilities

### Resources (Knowledge Base)
- **GetControlsReference**: Comprehensive AvaloniaUI controls catalog with examples
- **GetControlInfo**: Detailed information about specific controls
- **GetXamlPatterns**: Common XAML patterns and templates
- **GetXamlPattern**: Specific XAML pattern by name
- **GetMvvmPatterns**: MVVM-specific patterns
- **GetMigrationGuide**: Complete WPF to AvaloniaUI migration guide
- **GetControlMappings**: WPF to AvaloniaUI control compatibility
- **GetNamespaceAndBindingChanges**: Migration namespace and binding updates
- **GetMigrationSteps**: Step-by-step migration process

### Prompts (Templates)
- **CreateAvaloniaAppPrompt**: Template for new application creation
- **MigrateFromWpfPrompt**: Template for WPF migration assistance
- **DebugAvaloniaIssuePrompt**: Template for troubleshooting issues
- **ResponsiveDesignPrompt**: Template for responsive design implementation

## Build Configuration

- **Custom Build Target**: The project includes a custom MSBuild target that displays "Hello from CustomAfterBuildTarget" after each build
- **Central Package Management**: All package versions are managed centrally through Directory.Packages.props
- **NuGet Sources**: Only the official NuGet feed is configured (https://api.nuget.org/v3/index.json)
- **Data Files**: JSON knowledge base files are copied to output directory

## Important Notes

- The project uses .NET 9.0 with SDK version 9.0.300
- The solution file uses the newer .slnx format instead of traditional .sln
- The project structure follows modern .NET conventions with Directory.Build.* files for shared configuration
- MCP server provides comprehensive AvaloniaUI development assistance through tools, resources, and prompts
- All knowledge base data is embedded as JSON files for offline access