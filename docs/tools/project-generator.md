# ProjectGeneratorTool

Generate complete AvaloniaUI projects with professional structure and best practices.

## Overview

The ProjectGeneratorTool creates production-ready AvaloniaUI projects with various templates and configurations. It supports MVVM architecture, cross-platform development, and includes all necessary project files.

## Features

- **Multiple Templates**: MVVM, Basic, Cross-platform
- **Platform Support**: Desktop, Mobile, Universal
- **Best Practices**: Industry-standard project structure
- **Performance Optimized**: Async file generation
- **Validation**: Comprehensive input checking

## Usage

### Basic Project Creation

```
"Create a new AvaloniaUI project called MyApp"
```

This creates a basic project with:
- Standard project structure
- Modern .NET 9.0 targeting
- Essential AvaloniaUI packages
- Basic window and app setup

### MVVM Project

```
"Create an MVVM AvaloniaUI project called MyMvvmApp"
```

Generates:
- ViewModels with ReactiveUI
- Proper MVVM structure
- Data binding examples
- Command patterns

### Cross-Platform Project

```
"Create a cross-platform AvaloniaUI project called MyCrossPlatformApp for desktop and mobile"
```

Creates:
- Shared UI components
- Platform-specific projects
- Conditional compilation
- Mobile optimizations

## Parameters

### Required
- **projectName**: Name of the project (validates against naming conventions)

### Optional
- **template**: Project template type
  - `"mvvm"` (default) - Full MVVM with ReactiveUI
  - `"basic"` - Simple application structure
  - `"crossplatform"` - Multi-platform setup

- **platforms**: Target platforms
  - `"desktop"` (default) - Windows, macOS, Linux
  - `"mobile"` - Android, iOS
  - `"all"` - Desktop and mobile

- **outputDirectory**: Where to create the project (defaults to current directory)

## Generated Structure

### MVVM Template
```
MyApp/
├── MyApp.csproj
├── App.axaml
├── App.axaml.cs
├── MainWindow.axaml
├── MainWindow.axaml.cs
├── Program.cs
└── ViewModels/
    ├── MainWindowViewModel.cs
    └── ViewModelBase.cs
```

### Cross-Platform Template
```
MyApp/
├── MyApp.Desktop.csproj
├── MyApp.Mobile.csproj (if mobile enabled)
├── App.axaml
├── App.axaml.cs
├── MainWindow.axaml
├── MainWindow.axaml.cs
├── Program.Desktop.cs
└── ViewModels/
    ├── MainWindowViewModel.cs
    └── ViewModelBase.cs
```

## Package References

### Desktop Projects
- Avalonia 11.3.2
- Avalonia.Desktop 11.3.2
- Avalonia.Fonts.Inter 11.3.2

### MVVM Projects (Additional)
- Avalonia.ReactiveUI 11.3.2

### Mobile Projects (Additional)
- Avalonia.Android 11.3.2
- Avalonia.iOS 11.3.2

## Code Examples

### Generated MainWindow (MVVM)
```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="using:MyApp.ViewModels"
        x:Class="MyApp.MainWindow"
        x:DataType="vm:MainWindowViewModel"
        Title="MyApp"
        Width="800"
        Height="600">

    <Design.DataContext>
        <vm:MainWindowViewModel/>
    </Design.DataContext>

    <StackPanel Margin="20" Spacing="10">
        <TextBlock Text="Welcome to AvaloniaUI!" FontSize="24" FontWeight="Bold"/>
        <TextBlock Text="{Binding Greeting}" FontSize="16"/>

        <StackPanel Orientation="Horizontal" Spacing="10">
            <TextBox Text="{Binding Name}" Watermark="Enter your name" Width="200"/>
            <Button Command="{Binding GreetCommand}" Content="Greet"/>
        </StackPanel>

        <TextBlock Text="{Binding GreetingMessage}" FontWeight="Bold" Foreground="Blue"/>
    </StackPanel>
</Window>
```

### Generated ViewModel
```csharp
using System.Reactive;
using ReactiveUI;

namespace MyApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string _name = string.Empty;
    private string _greetingMessage = string.Empty;

    public string Greeting => "Welcome to Avalonia!";

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string GreetingMessage
    {
        get => _greetingMessage;
        set => this.RaiseAndSetIfChanged(ref _greetingMessage, value);
    }

    public ReactiveCommand<Unit, Unit> GreetCommand { get; }

    public MainWindowViewModel()
    {
        GreetCommand = ReactiveCommand.Create(ExecuteGreet);
    }

    private void ExecuteGreet()
    {
        GreetingMessage = string.IsNullOrWhiteSpace(Name)
            ? "Please enter your name!"
            : $"Hello, {Name}! Welcome to AvaloniaUI!";
    }
}
```

## Building and Running

After project generation:

```bash
cd MyApp
dotnet run
```

For cross-platform projects:
```bash
# Run desktop version
dotnet run --project MyApp.Desktop.csproj

# Build for mobile (requires additional setup)
dotnet build MyApp.Mobile.csproj
```

## Validation

The tool validates:
- **Project names** - Valid C# identifiers, no reserved words
- **Directory paths** - Valid and accessible locations
- **Template types** - Supported template options
- **Platform combinations** - Valid platform specifications

## Error Handling

Common errors and solutions:

### Invalid Project Name
```
Error: Project name 'class' is a reserved keyword
Solution: Use a different name like 'MyClass' or 'ClassManager'
```

### Directory Already Exists
```
Error: Directory 'MyApp' already exists
Solution: Choose a different name or remove the existing directory
```

### Invalid Platform
```
Error: Invalid platforms 'web'. Valid platforms are: desktop, mobile, all
Solution: Use one of the supported platform options
```

## Performance

- **File Generation**: Parallel async operations for faster creation
- **Validation**: Early validation prevents unnecessary work
- **Caching**: Template caching for repeated operations
- **Memory**: Efficient string handling for large projects

## Best Practices

### Project Naming
- Use PascalCase (e.g., "MyAwesomeApp")
- Avoid spaces and special characters
- Keep names descriptive but concise
- Consider namespace implications

### Template Selection
- **Basic**: Simple apps, prototypes, learning
- **MVVM**: Production apps, complex UIs, testability
- **Cross-platform**: Multi-device deployment

### Platform Considerations
- **Desktop only**: Traditional desktop applications
- **Mobile included**: Touch-optimized interfaces
- **All platforms**: Maximum reach, consider responsive design

## Integration

Works seamlessly with other tools:
- Use **XamlValidationTool** to validate generated XAML
- Apply **SecurityPatternTool** for authentication
- Add **ThemingTool** for custom styling
- Use **DiagnosticTool** for monitoring