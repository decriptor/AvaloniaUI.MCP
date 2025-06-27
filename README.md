# AvaloniaUI MCP Server

A comprehensive Model Context Protocol (MCP) server that provides extensive AvaloniaUI knowledge and development assistance. This server delivers tools, resources, and prompts to help developers build, migrate, and optimize AvaloniaUI applications.

## 🚀 Features

### 🛠️ Tools (Actions)
- **Project Generation**: Create new AvaloniaUI projects with MVVM, basic, or cross-platform templates
- **XAML Validation**: Validate AvaloniaUI XAML syntax and detect common issues
- **WPF Migration**: Convert WPF XAML to AvaloniaUI format automatically
- **Server Information**: Get server capabilities and status

### 📚 Resources (Knowledge Base)
- **Controls Reference**: Comprehensive catalog of AvaloniaUI controls with examples
- **XAML Patterns**: Common patterns for layouts, MVVM, styling, and data binding
- **Migration Guide**: Complete WPF to AvaloniaUI migration documentation
- **Best Practices**: Development guidelines and cross-platform considerations

### 📝 Prompts (Templates)
- **App Creation**: Template for new AvaloniaUI application development
- **WPF Migration**: Structured approach for migrating WPF applications
- **Issue Debugging**: Troubleshooting guidance for common problems
- **Responsive Design**: Cross-platform UI implementation guidance

## 🔧 Requirements

- **.NET 9.0 SDK** (version 9.0.300 or later)
- **MCP-compatible client** (Claude Desktop, VS Code with MCP extension, etc.)

## 📦 Installation

### 1. Clone the Repository
```bash
git clone <repository-url>
cd AvaloniaUI.MCP
```

### 2. Build the Project
```bash
dotnet build
```

### 3. Test the Installation
```bash
dotnet test
```

### 4. Run the MCP Server
```bash
dotnet run --project src/AvaloniaUI.MCP/AvaloniaUI.MCP.csproj
```

## ⚙️ MCP Client Setup

### Claude Desktop Configuration

1. **Locate Claude Desktop Config File**:
   - **Windows**: `%APPDATA%\Claude\claude_desktop_config.json`
   - **macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`
   - **Linux**: `~/.config/Claude/claude_desktop_config.json`

2. **Add AvaloniaUI MCP Server Configuration**:
```json
{
  "mcpServers": {
    "avaloniaui-mcp": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/path/to/AvaloniaUI.MCP/src/AvaloniaUI.MCP/AvaloniaUI.MCP.csproj"
      ],
      "env": {}
    }
  }
}
```

3. **Replace the Path**: Update `/path/to/AvaloniaUI.MCP/` with the actual path to your cloned repository.

4. **Restart Claude Desktop**: Close and reopen Claude Desktop to load the new configuration.

### VS Code with MCP Extension

1. **Install MCP Extension**: Install an MCP-compatible extension in VS Code
2. **Configure Server**: Add the AvaloniaUI MCP server to your extension settings
3. **Server Command**: 
   ```bash
   dotnet run --project /path/to/AvaloniaUI.MCP/src/AvaloniaUI.MCP/AvaloniaUI.MCP.csproj
   ```

## 🎯 Usage Examples

### Creating a New AvaloniaUI Project

**Using the Tool:**
```
Use the CreateAvaloniaProject tool with:
- projectName: "MyAvaloniaApp"
- template: "mvvm" (or "basic", "crossplatform")
- platforms: "desktop" (or "mobile", "all")
- outputDirectory: "/path/to/create/project"
```

**Using the Prompt:**
```
Use the CreateAvaloniaAppPrompt with:
- appName: "MyAvaloniaApp"
- appDescription: "A cross-platform task management application"
- targetPlatforms: "desktop"
```

### Validating XAML

**Basic Validation:**
```
Use ValidateXaml tool with your XAML content to check for:
- XML syntax errors
- AvaloniaUI namespace issues
- Control compatibility
- Binding syntax problems
```

**Strict Validation:**
```
Use ValidateXaml with validationLevel: "strict" for comprehensive checking
```

### Converting WPF to AvaloniaUI

**Automatic Conversion:**
```
Use ConvertWpfXamlToAvalonia tool with your WPF XAML to:
- Update namespaces automatically
- Identify compatibility issues
- Get conversion notes and manual steps
- Validate the converted XAML
```

### Getting AvaloniaUI Information

**Controls Reference:**
```
Use GetControlsReference resource for complete controls catalog
Use GetControlInfo with controlName: "Button" for specific control details
```

**XAML Patterns:**
```
Use GetXamlPatterns for all patterns
Use GetMvvmPatterns for MVVM-specific patterns
Use GetXamlPattern with patternName: "mvvm_window" for specific patterns
```

**Migration Guidance:**
```
Use GetMigrationGuide for complete WPF migration documentation
Use GetControlMappings for WPF to AvaloniaUI control compatibility
Use GetMigrationSteps for step-by-step migration process
```

## 🔍 Available Tools

| Tool | Description | Parameters |
|------|-------------|------------|
| `Echo` | Test server connectivity | `message: string` |
| `GetServerInfo` | Server information and capabilities | None |
| `CreateAvaloniaProject` | Generate new AvaloniaUI project | `projectName`, `template`, `platforms`, `outputDirectory` |
| `ValidateXaml` | Validate AvaloniaUI XAML | `xamlContent`, `validationLevel` |
| `ConvertWpfXamlToAvalonia` | Convert WPF XAML to AvaloniaUI | `wpfXaml` |

## 📖 Available Resources

| Resource | Description | Parameters |
|----------|-------------|------------|
| `GetControlsReference` | Complete AvaloniaUI controls catalog | None |
| `GetControlInfo` | Specific control information | `controlName: string` |
| `GetXamlPatterns` | All XAML patterns and templates | None |
| `GetXamlPattern` | Specific XAML pattern | `patternName: string` |
| `GetMvvmPatterns` | MVVM-specific patterns | None |
| `GetMigrationGuide` | Complete WPF migration guide | None |
| `GetControlMappings` | WPF to AvaloniaUI control mappings | None |
| `GetNamespaceAndBindingChanges` | Migration namespace updates | None |
| `GetMigrationSteps` | Step-by-step migration process | None |

## 📋 Available Prompts

| Prompt | Description | Parameters |
|--------|-------------|------------|
| `CreateAvaloniaAppPrompt` | New application creation template | `appName`, `appDescription`, `targetPlatforms` |
| `MigrateFromWpfPrompt` | WPF migration assistance template | `wpfAppName`, `wpfFeatures`, `wpfComponents` |
| `DebugAvaloniaIssuePrompt` | Troubleshooting guidance template | `issueDescription`, `issueType` |
| `ResponsiveDesignPrompt` | Responsive design implementation | `targetDevices`, `layoutRequirements` |

## 🧪 Development

### Project Structure
```
AvaloniaUI.MCP/
├── src/AvaloniaUI.MCP/           # Main MCP server project
│   ├── Tools/                    # MCP tools implementation
│   ├── Resources/                # MCP resources (knowledge base)
│   ├── Prompts/                  # MCP prompt templates
│   └── Data/                     # JSON knowledge base files
├── tests/AvaloniaUI.MCP.Tests/   # Unit tests
└── docs/                         # Documentation
```

### Running Tests
```bash
dotnet test
```

### Building for Release
```bash
dotnet build --configuration Release
```

### Adding New Knowledge
1. **Update JSON files** in `src/AvaloniaUI.MCP/Data/`
2. **Modify Resource classes** in `src/AvaloniaUI.MCP/Resources/`
3. **Add tests** in `tests/AvaloniaUI.MCP.Tests/`
4. **Build and test** the changes

## 🐛 Troubleshooting

### Server Won't Start
- Verify .NET 9.0 SDK is installed: `dotnet --version`
- Check for build errors: `dotnet build`
- Ensure all dependencies are restored: `dotnet restore`

### Claude Desktop Not Recognizing Server
- Verify the config file path is correct
- Check JSON syntax in `claude_desktop_config.json`
- Ensure the path to the project is absolute and correct
- Restart Claude Desktop after configuration changes

### Tools Not Working
- Check server logs for errors
- Verify XAML syntax if using validation tools
- Ensure proper parameters are provided to tools
- Test with the Echo tool to verify connectivity

### Resource Data Not Found
- Verify JSON files are copied to output directory
- Check file paths in Resource implementations
- Ensure project builds successfully
- Verify `Data/*.json` files exist in the output directory

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/new-feature`
3. Commit your changes: `git commit -am 'Add new feature'`
4. Push to the branch: `git push origin feature/new-feature`
5. Submit a pull request

## 📚 Resources

- [AvaloniaUI Documentation](https://docs.avaloniaui.net/)
- [Model Context Protocol Specification](https://modelcontextprotocol.io/)
- [Microsoft MCP SDK for .NET](https://github.com/modelcontextprotocol/csharp-sdk)
- [Claude Desktop MCP Setup](https://docs.anthropic.com/en/docs/build-with-claude/computer-use)

## 🆘 Support

For issues and questions:
1. Check the [troubleshooting section](#-troubleshooting)
2. Search existing [GitHub issues](link-to-issues)
3. Create a new issue with detailed information
4. Use the `DebugAvaloniaIssuePrompt` for AvaloniaUI-specific problems

---

**Happy coding with AvaloniaUI! 🎉**