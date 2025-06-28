# Contributing to AvaloniaUI MCP Server

Thank you for your interest in contributing to the AvaloniaUI MCP Server! This document provides guidelines and information for contributors.

## 🤝 How to Contribute

### Reporting Issues

1. **Check existing issues** first to avoid duplicates
2. **Use the issue templates** when available
3. **Provide detailed information**:
   - Clear description of the problem
   - Steps to reproduce
   - Expected vs actual behavior
   - Environment details (.NET version, OS, MCP client)
   - Relevant logs or error messages

### Suggesting Enhancements

1. **Open an issue** with the enhancement request
2. **Describe the use case** and why it would be valuable
3. **Provide examples** of how it would work
4. **Consider backward compatibility** implications

### Pull Requests

1. **Fork the repository** and create a feature branch
2. **Follow the coding standards** outlined below
3. **Write tests** for new functionality
4. **Update documentation** as needed
5. **Ensure all CI checks pass** before requesting review

## 🏗️ Development Setup

### Prerequisites

- **.NET 9.0 SDK** (version 9.0.300 or later)
- **Git** for version control
- **IDE** (Visual Studio, VS Code, or JetBrains Rider)

### Local Development

1. **Clone the repository**:
   ```bash
   git clone https://github.com/your-username/AvaloniaUI.MCP.git
   cd AvaloniaUI.MCP
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Build the project**:
   ```bash
   dotnet build
   ```

4. **Run tests**:
   ```bash
   dotnet test
   ```

5. **Run the MCP server**:
   ```bash
   dotnet run --project src/AvaloniaUI.MCP/AvaloniaUI.MCP.csproj
   ```

## 📝 Coding Standards

### Code Style

- Follow the **existing code style** in the project
- Use the **EditorConfig** settings provided
- Run `dotnet format` before committing
- Follow **C# naming conventions**
- Use **XML documentation** for public APIs

### Project Structure

```
src/AvaloniaUI.MCP/
├── Tools/          # MCP tools (actions that can be performed)
├── Resources/      # MCP resources (knowledge base providers)
├── Prompts/        # MCP prompts (template generators)
└── Data/           # JSON knowledge base files
```

### Adding New Features

#### Adding a New Tool

1. Create a new class in `src/AvaloniaUI.MCP/Tools/`
2. Use the `[McpServerToolType]` attribute on the class
3. Use the `[McpServerTool]` attribute on methods
4. Add `[Description]` attributes for documentation
5. Write unit tests in `tests/AvaloniaUI.MCP.Tests/`

Example:
```csharp
[McpServerToolType]
public static class MyNewTool
{
    [McpServerTool, Description("Description of what this tool does")]
    public static string MyToolMethod(
        [Description("Parameter description")] string parameter)
    {
        // Implementation
        return "Result";
    }
}
```

#### Adding a New Resource

1. Create a new class in `src/AvaloniaUI.MCP/Resources/`
2. Use the `[McpServerResourceType]` attribute on the class
3. Use the `[McpServerResource]` attribute on methods
4. Return `Task<string>` for async resource loading
5. Add corresponding JSON data files if needed

#### Adding a New Prompt

1. Create or extend a class in `src/AvaloniaUI.MCP/Prompts/`
2. Use the `[McpServerPromptType]` attribute on the class
3. Use the `[McpServerPrompt]` attribute on methods
4. Return `Task<string>` with the generated prompt

### Testing Guidelines

- **Write unit tests** for all new functionality
- **Use descriptive test names** that explain what is being tested
- **Follow the AAA pattern** (Arrange, Act, Assert)
- **Test both success and failure scenarios**
- **Maintain high code coverage**

Example test:
```csharp
[Fact]
public void MyTool_WithValidInput_ReturnsExpectedResult()
{
    // Arrange
    var input = "test input";
    
    // Act
    var result = MyNewTool.MyToolMethod(input);
    
    // Assert
    Assert.Contains("expected content", result);
}
```

### Documentation

- **Update README.md** for major feature additions
- **Update CLAUDE.md** for development-related changes
- **Add XML documentation** for public APIs
- **Update capability tables** in documentation

## 🔄 Pull Request Process

### Before Submitting

1. **Ensure all tests pass**:
   ```bash
   dotnet test
   ```

2. **Check code formatting**:
   ```bash
   dotnet format --verify-no-changes
   ```

3. **Build in release mode**:
   ```bash
   dotnet build --configuration Release
   ```

4. **Update documentation** if needed

### PR Requirements

- **Clear title** describing the change
- **Detailed description** explaining:
  - What was changed and why
  - How to test the changes
  - Any breaking changes
  - Related issues (use "Fixes #123" syntax)
- **Small, focused changes** (prefer multiple small PRs over large ones)
- **All CI checks passing**

### Review Process

1. **Automated checks** must pass (CI, tests, formatting)
2. **Code review** by maintainers
3. **Testing** of the functionality
4. **Documentation review** if applicable
5. **Merge** after approval

## 🌟 Areas for Contribution

### High Priority

- **Additional AvaloniaUI controls** in the knowledge base
- **More XAML patterns** and examples
- **Enhanced WPF migration tools**
- **Cross-platform specific guidance**
- **Performance optimizations**

### Medium Priority

- **Additional MCP transport support** (HTTP, WebSockets)
- **More comprehensive test coverage**
- **Better error handling and validation**
- **Documentation improvements**
- **Sample applications and demos**

### Ideas for New Features

- **Interactive XAML designer assistance**
- **Automated code generation tools**
- **AvaloniaUI project analysis tools**
- **Integration with AvaloniaUI designer**
- **Custom control templates generator**

## 🐛 Bug Reports

When reporting bugs, please include:

1. **Clear title** summarizing the issue
2. **Steps to reproduce** the problem
3. **Expected behavior** vs **actual behavior**
4. **Environment information**:
   - Operating system and version
   - .NET version
   - MCP client being used
   - AvaloniaUI MCP Server version
5. **Logs or error messages** (if applicable)
6. **Minimal reproduction case** (if possible)

## 📚 Knowledge Base Contributions

The AvaloniaUI MCP Server's knowledge base is stored in JSON files in `src/AvaloniaUI.MCP/Data/`. Contributions to improve or expand this knowledge base are highly valued:

### Controls Reference (`controls.json`)
- Add new controls with examples
- Improve existing control descriptions
- Add more usage scenarios and properties

### XAML Patterns (`xaml-patterns.json`)
- Add new common patterns
- Improve existing pattern examples
- Add platform-specific patterns

### Migration Guide (`migration-guide.json`)
- Expand WPF to AvaloniaUI mappings
- Add more migration scenarios
- Include troubleshooting guidance

## 🏷️ Issue Labels

- `bug` - Something isn't working
- `enhancement` - New feature or request
- `documentation` - Improvements to documentation
- `good first issue` - Good for newcomers
- `help wanted` - Extra attention is needed
- `question` - Further information is requested
- `tools` - Related to MCP tools
- `resources` - Related to MCP resources
- `prompts` - Related to MCP prompts
- `knowledge-base` - Related to JSON data files

## 📄 License

By contributing to this project, you agree that your contributions will be licensed under the same license as the project (MIT License).

## 🆘 Getting Help

- **GitHub Discussions** for general questions
- **GitHub Issues** for bugs and feature requests
- **Code review comments** for implementation questions
- **Documentation** in README.md and CLAUDE.md

Thank you for contributing to make AvaloniaUI development better for everyone! 🎉