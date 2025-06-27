# Your First AvaloniaUI Project

Learn how to create your first AvaloniaUI application using the MCP server.

## Overview

This tutorial walks you through creating a complete AvaloniaUI application from scratch using AvaloniaUI.MCP. You'll learn the basics of project generation, XAML validation, and best practices.

## Prerequisites

- AvaloniaUI.MCP server running
- MCP-compatible client (Claude Desktop, VS Code)
- Basic understanding of C# and XAML

## Step 1: Create the Project

Start by asking your MCP client to create a new project:

```
"Create a new AvaloniaUI MVVM project called HelloAvaloniaUI for desktop"
```

The server will generate:

```
HelloAvaloniaUI/
├── HelloAvaloniaUI.csproj      # Project file with dependencies
├── App.axaml                   # Application XAML
├── App.axaml.cs               # Application code-behind
├── MainWindow.axaml           # Main window XAML
├── MainWindow.axaml.cs        # Main window code-behind
├── Program.cs                 # Application entry point
└── ViewModels/
    ├── MainWindowViewModel.cs  # Main window view model
    └── ViewModelBase.cs       # Base class for view models
```

## Step 2: Understand the Structure

### Project File (HelloAvaloniaUI.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <BuiltInComInteropSupport>true</BuiltInComInteropSupport>
    <ApplicationManifest>app.manifest</ApplicationManifest>
    <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Avalonia" Version="11.3.2" />
    <PackageReference Include="Avalonia.Desktop" Version="11.3.2" />
    <PackageReference Include="Avalonia.Fonts.Inter" Version="11.3.2" />
    <PackageReference Include="Avalonia.ReactiveUI" Version="11.3.2" />
  </ItemGroup>

  <ItemGroup>
    <AvaloniaResource Include="**/*.axaml" />
  </ItemGroup>
</Project>
```

### Application Entry Point (Program.cs)
```csharp
using Avalonia;
using System;

namespace HelloAvaloniaUI;

class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
```

### Application Class (App.axaml.cs)
```csharp
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace HelloAvaloniaUI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
```

## Step 3: Explore the Main Window

The generated main window demonstrates MVVM patterns:

### MainWindow.axaml
```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="using:HelloAvaloniaUI.ViewModels"
        x:Class="HelloAvaloniaUI.MainWindow"
        x:DataType="vm:MainWindowViewModel"
        Icon="/Assets/avalonia-logo.ico"
        Title="HelloAvaloniaUI"
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

### MainWindowViewModel.cs
```csharp
using System.Reactive;
using ReactiveUI;

namespace HelloAvaloniaUI.ViewModels;

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

## Step 4: Build and Run

Navigate to your project directory and run:

```bash
cd HelloAvaloniaUI
dotnet run
```

You should see a window with:
- A welcome message
- A text input field
- A "Greet" button
- Dynamic greeting output

## Step 5: Validate Your XAML

Use the MCP server to validate your XAML:

```
"Validate this XAML code and suggest improvements"
```

Paste your MainWindow.axaml content. The server will:
- Check syntax
- Verify property bindings
- Suggest best practices
- Identify potential issues

## Step 6: Add Features

### Add a Counter Feature

Ask the MCP server:
```
"Add a counter feature to my MainWindow with increment and decrement buttons"
```

This might generate additional properties and commands for your ViewModel:

```csharp
private int _counter = 0;

public int Counter
{
    get => _counter;
    set => this.RaiseAndSetIfChanged(ref _counter, value);
}

public ReactiveCommand<Unit, Unit> IncrementCommand { get; }
public ReactiveCommand<Unit, Unit> DecrementCommand { get; }

// In constructor:
IncrementCommand = ReactiveCommand.Create(() => Counter++);
DecrementCommand = ReactiveCommand.Create(() => Counter--);
```

And corresponding XAML:
```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <Button Command="{Binding DecrementCommand}" Content="-" Width="40"/>
    <TextBlock Text="{Binding Counter}" FontSize="18" VerticalAlignment="Center" MinWidth="40" TextAlignment="Center"/>
    <Button Command="{Binding IncrementCommand}" Content="+" Width="40"/>
</StackPanel>
```

## Step 7: Apply Styling

Ask for theming assistance:
```
"Apply a modern theme to my AvaloniaUI application"
```

The server can help you add:
- Color schemes
- Typography
- Control styles
- Layout improvements

## Common Patterns Demonstrated

### 1. MVVM Architecture
- Clear separation of concerns
- View models handle business logic
- Views handle presentation
- Commands for user interactions

### 2. Data Binding
- Two-way binding for input fields
- One-way binding for display
- Command binding for buttons
- Property change notifications

### 3. Reactive Patterns
- ReactiveUI integration
- Observable properties
- Reactive commands
- Automatic UI updates

### 4. Best Practices
- Compiled bindings for performance
- Proper namespace organization
- Design-time data context
- Null reference safety

## Next Steps

1. **Learn More Patterns**: Explore [MVVM Examples](../mvvm/)
2. **Add Validation**: Implement input validation patterns using the SecurityPatternTool
3. **Style Your App**: Use the ThemingTool to create custom themes
4. **Test Your Code**: Add unit tests for your ViewModels and business logic

## Troubleshooting

### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

### Runtime Issues
```bash
# Check for missing references
dotnet list package --vulnerable
dotnet list package --outdated
```

### XAML Errors
Use the validation tool:
```
"Check my XAML for errors and provide fixes"
```

## Summary

You've successfully created your first AvaloniaUI application with:
- ✅ MVVM architecture
- ✅ Data binding
- ✅ Reactive patterns
- ✅ Modern .NET practices
- ✅ Cross-platform compatibility

The MCP server helped generate a professional project structure with best practices built-in. You can now extend this foundation with additional features, styling, and functionality.
