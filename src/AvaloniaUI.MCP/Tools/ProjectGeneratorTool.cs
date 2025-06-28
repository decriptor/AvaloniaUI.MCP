using System.ComponentModel;
using ModelContextProtocol.Server;
using AvaloniaUI.MCP.Services;

namespace AvaloniaUI.MCP.Tools;

[McpServerToolType]
public static class ProjectGeneratorTool
{
    [McpServerTool, Description("Creates a new AvaloniaUI project with the specified template and configuration")]
    public static string CreateAvaloniaProject(
        [Description("Name of the project")] string projectName,
        [Description("Project template: 'mvvm' (default), 'basic', or 'crossplatform'")] string template = "mvvm",
        [Description("Target platforms: 'desktop' (default), 'mobile', or 'all'")] string platforms = "desktop",
        [Description("Directory where the project should be created")] string? outputDirectory = null)
    {
        return ErrorHandlingService.SafeExecute("CreateAvaloniaProject", () =>
        {
            // Validate inputs
            var validation = ErrorHandlingService.ValidateCommonParameters(
                projectName: projectName,
                templateType: template.ToLowerInvariant()
            );
            
            if (!validation.IsValid)
                return ErrorHandlingService.CreateValidationError("CreateAvaloniaProject", validation);

            // Additional platform validation
            var validPlatforms = new[] { "desktop", "mobile", "all" };
            if (!validPlatforms.Contains(platforms.ToLowerInvariant()))
            {
                var platformValidation = ValidationResult.Failure($"Invalid platforms '{platforms}'. Valid platforms are: {string.Join(", ", validPlatforms)}");
                return ErrorHandlingService.CreateValidationError("CreateAvaloniaProject", platformValidation);
            }

            // Use current directory if not specified
            outputDirectory ??= Directory.GetCurrentDirectory();

            var projectPath = Path.Combine(outputDirectory, projectName);

            // Check if directory already exists
            if (Directory.Exists(projectPath))
            {
                throw new InvalidOperationException($"Directory '{projectPath}' already exists");
            }

            // Create project directory
            Directory.CreateDirectory(projectPath);

            // Generate project files based on template
            var result = template.ToLowerInvariant() switch
            {
                "mvvm" => GenerateMvvmProject(projectPath, projectName, platforms),
                "basic" => GenerateBasicProject(projectPath, projectName, platforms),
                "crossplatform" => GenerateCrossPlatformProject(projectPath, projectName, platforms),
                _ => throw new ArgumentException($"Unknown template: {template}")
            };

            return result;
        });
    }

    private static string GenerateMvvmProject(string projectPath, string projectName, string platforms)
    {
        // Generate all file contents first
        var filesToCreate = new List<(string FilePath, string Content)>
        {
            (Path.Combine(projectPath, $"{projectName}.csproj"), GenerateProjectFile(projectName, platforms, true)),
            (Path.Combine(projectPath, "App.axaml"), GenerateAppXaml(projectName)),
            (Path.Combine(projectPath, "App.axaml.cs"), GenerateAppCode(projectName)),
            (Path.Combine(projectPath, "MainWindow.axaml"), GenerateMainWindowXaml(projectName)),
            (Path.Combine(projectPath, "MainWindow.axaml.cs"), GenerateMainWindowCode(projectName)),
            (Path.Combine(projectPath, "Program.cs"), GenerateProgramCs(projectName))
        };

        // Create ViewModels directory
        var viewModelsDir = Path.Combine(projectPath, "ViewModels");
        
        // Add ViewModel files
        filesToCreate.AddRange(new[]
        {
            (Path.Combine(viewModelsDir, "MainWindowViewModel.cs"), GenerateMainWindowViewModel(projectName)),
            (Path.Combine(viewModelsDir, "ViewModelBase.cs"), GenerateViewModelBase(projectName))
        });

        // Write all files asynchronously in parallel for better performance
        var writeTask = AsyncFileService.WriteAllFilesAsync(filesToCreate);
        writeTask.Wait(); // Wait for completion since MCP tools must be synchronous

        return $"Successfully created MVVM AvaloniaUI project '{projectName}' at '{projectPath}'\\n" +
               $"Platform support: {platforms}\\n" +
               $"Files created:\\n" +
               $"- {projectName}.csproj\\n" +
               $"- App.axaml & App.axaml.cs\\n" +
               $"- MainWindow.axaml & MainWindow.axaml.cs\\n" +
               $"- ViewModels/MainWindowViewModel.cs\\n" +
               $"- ViewModels/ViewModelBase.cs\\n" +
               $"- Program.cs\\n\\n" +
               $"To build and run:\\n" +
               $"cd \"{projectPath}\"\\n" +
               $"dotnet run";
    }

    private static string GenerateBasicProject(string projectPath, string projectName, string platforms)
    {
        // Create project file
        var projectFile = GenerateProjectFile(projectName, platforms, false);
        File.WriteAllText(Path.Combine(projectPath, $"{projectName}.csproj"), projectFile);

        // Create App.axaml
        var appXaml = GenerateAppXaml(projectName);
        File.WriteAllText(Path.Combine(projectPath, "App.axaml"), appXaml);

        // Create App.axaml.cs
        var appCode = GenerateAppCode(projectName);
        File.WriteAllText(Path.Combine(projectPath, "App.axaml.cs"), appCode);

        // Create MainWindow.axaml (basic version)
        var mainWindowXaml = GenerateBasicMainWindowXaml(projectName);
        File.WriteAllText(Path.Combine(projectPath, "MainWindow.axaml"), mainWindowXaml);

        // Create MainWindow.axaml.cs
        var mainWindowCode = GenerateMainWindowCode(projectName);
        File.WriteAllText(Path.Combine(projectPath, "MainWindow.axaml.cs"), mainWindowCode);

        // Create Program.cs
        var programCs = GenerateProgramCs(projectName);
        File.WriteAllText(Path.Combine(projectPath, "Program.cs"), programCs);

        return $"Successfully created basic AvaloniaUI project '{projectName}' at '{projectPath}'\\n" +
               $"Platform support: {platforms}\\n" +
               $"Files created:\\n" +
               $"- {projectName}.csproj\\n" +
               $"- App.axaml & App.axaml.cs\\n" +
               $"- MainWindow.axaml & MainWindow.axaml.cs\\n" +
               $"- Program.cs\\n\\n" +
               $"To build and run:\\n" +
               $"cd \"{projectPath}\"\\n" +
               $"dotnet run";
    }

    private static string GenerateCrossPlatformProject(string projectPath, string projectName, string platforms)
    {
        // For cross-platform, create multiple project files
        var desktopProject = GenerateProjectFile($"{projectName}.Desktop", "desktop", true);
        File.WriteAllText(Path.Combine(projectPath, $"{projectName}.Desktop.csproj"), desktopProject);

        if (platforms == "mobile" || platforms == "all")
        {
            var mobileProject = GenerateProjectFile($"{projectName}.Mobile", "mobile", true);
            File.WriteAllText(Path.Combine(projectPath, $"{projectName}.Mobile.csproj"), mobileProject);
        }

        // Create shared UI files
        var appXaml = GenerateAppXaml(projectName);
        File.WriteAllText(Path.Combine(projectPath, "App.axaml"), appXaml);

        var appCode = GenerateAppCode(projectName);
        File.WriteAllText(Path.Combine(projectPath, "App.axaml.cs"), appCode);

        var mainWindowXaml = GenerateMainWindowXaml(projectName);
        File.WriteAllText(Path.Combine(projectPath, "MainWindow.axaml"), mainWindowXaml);

        var mainWindowCode = GenerateMainWindowCode(projectName);
        File.WriteAllText(Path.Combine(projectPath, "MainWindow.axaml.cs"), mainWindowCode);

        // Create ViewModels
        var viewModelsDir = Path.Combine(projectPath, "ViewModels");
        Directory.CreateDirectory(viewModelsDir);
        
        var mainViewModel = GenerateMainWindowViewModel(projectName);
        File.WriteAllText(Path.Combine(viewModelsDir, "MainWindowViewModel.cs"), mainViewModel);

        var viewModelBase = GenerateViewModelBase(projectName);
        File.WriteAllText(Path.Combine(viewModelsDir, "ViewModelBase.cs"), viewModelBase);

        // Create platform-specific entry points
        var desktopProgramCs = GenerateProgramCs($"{projectName}.Desktop");
        File.WriteAllText(Path.Combine(projectPath, "Program.Desktop.cs"), desktopProgramCs);

        return $"Successfully created cross-platform AvaloniaUI project '{projectName}' at '{projectPath}'\\n" +
               $"Platform support: {platforms}\\n" +
               $"Projects created: Desktop{(platforms == "mobile" || platforms == "all" ? ", Mobile" : "")}\\n\\n" +
               $"To build and run desktop:\\n" +
               $"cd \"{projectPath}\"\\n" +
               $"dotnet run --project {projectName}.Desktop.csproj";
    }

    private static string GenerateProjectFile(string projectName, string platforms, bool includeMvvm)
    {
        var targetFramework = platforms switch
        {
            "mobile" => "net9.0",
            "all" => "net9.0",
            _ => "net9.0"
        };

        var packageReferences = new List<string>
        {
            "    <PackageReference Include=\"Avalonia\" Version=\"11.3.2\" />",
            "    <PackageReference Include=\"Avalonia.Desktop\" Version=\"11.3.2\" />",
            "    <PackageReference Include=\"Avalonia.Fonts.Inter\" Version=\"11.3.2\" />"
        };

        if (platforms == "mobile" || platforms == "all")
        {
            packageReferences.Add("    <PackageReference Include=\"Avalonia.Android\" Version=\"11.3.2\" />");
            packageReferences.Add("    <PackageReference Include=\"Avalonia.iOS\" Version=\"11.3.2\" />");
        }

        if (includeMvvm)
        {
            packageReferences.Add("    <PackageReference Include=\"Avalonia.ReactiveUI\" Version=\"11.3.2\" />");
        }

        return $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>{targetFramework}</TargetFramework>
    <Nullable>enable</Nullable>
    <BuiltInComInteropSupport>true</BuiltInComInteropSupport>
    <ApplicationManifest>app.manifest</ApplicationManifest>
    <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
  </PropertyGroup>

  <ItemGroup>
{string.Join("\\n", packageReferences)}
  </ItemGroup>

  <ItemGroup>
    <AvaloniaResource Include=""**/*.axaml"" />
  </ItemGroup>

</Project>";
    }

    private static string GenerateAppXaml(string projectName) => $@"<Application xmlns=""https://github.com/avaloniaui""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             x:Class=""{projectName}.App"">
  <Application.Styles>
    <FluentTheme />
  </Application.Styles>
</Application>";

    private static string GenerateAppCode(string projectName) => $@"using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace {projectName};

public partial class App : Application
{{
    public override void Initialize()
    {{
        AvaloniaXamlLoader.Load(this);
    }}

    public override void OnFrameworkInitializationCompleted()
    {{
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {{
            desktop.MainWindow = new MainWindow();
        }}

        base.OnFrameworkInitializationCompleted();
    }}
}}";

    private static string GenerateMainWindowXaml(string projectName) => $@"<Window xmlns=""https://github.com/avaloniaui""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
        xmlns:vm=""using:{projectName}.ViewModels""
        x:Class=""{projectName}.MainWindow""
        x:DataType=""vm:MainWindowViewModel""
        Icon=""/Assets/avalonia-logo.ico""
        Title=""{projectName}""
        Width=""800""
        Height=""600"">

    <Design.DataContext>
        <vm:MainWindowViewModel/>
    </Design.DataContext>

    <StackPanel Margin=""20"" Spacing=""10"">
        <TextBlock Text=""Welcome to AvaloniaUI!"" FontSize=""24"" FontWeight=""Bold""/>
        <TextBlock Text=""{{Binding Greeting}}"" FontSize=""16""/>
        
        <StackPanel Orientation=""Horizontal"" Spacing=""10"">
            <TextBox Text=""{{Binding Name}}"" Watermark=""Enter your name"" Width=""200""/>
            <Button Command=""{{Binding GreetCommand}}"" Content=""Greet""/>
        </StackPanel>
        
        <TextBlock Text=""{{Binding GreetingMessage}}"" FontWeight=""Bold"" Foreground=""Blue""/>
    </StackPanel>
</Window>";

    private static string GenerateBasicMainWindowXaml(string projectName) => $@"<Window xmlns=""https://github.com/avaloniaui""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
        x:Class=""{projectName}.MainWindow""
        Title=""{projectName}""
        Width=""800""
        Height=""600"">

    <StackPanel Margin=""20"" Spacing=""10"">
        <TextBlock Text=""Welcome to AvaloniaUI!"" FontSize=""24"" FontWeight=""Bold""/>
        <TextBlock Text=""This is a basic AvaloniaUI application."" FontSize=""16""/>
        
        <Button Content=""Click Me!"" Click=""OnButtonClick"" Width=""100""/>
        <TextBlock Name=""ResultText"" FontWeight=""Bold"" Foreground=""Blue""/>
    </StackPanel>
</Window>";

    private static string GenerateMainWindowCode(string projectName) => $@"using Avalonia.Controls;
using Avalonia.Interactivity;

namespace {projectName};

public partial class MainWindow : Window
{{
    public MainWindow()
    {{
        InitializeComponent();
    }}

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {{
        if (this.FindControl<TextBlock>(""ResultText"") is TextBlock textBlock)
        {{
            textBlock.Text = ""Hello from AvaloniaUI!"";
        }}
    }}
}}";

    private static string GenerateMainWindowViewModel(string projectName) => $@"using System.Reactive;
using ReactiveUI;

namespace {projectName}.ViewModels;

public class MainWindowViewModel : ViewModelBase
{{
    private string _name = string.Empty;
    private string _greetingMessage = string.Empty;

    public string Greeting => ""Welcome to Avalonia!"";

    public string Name
    {{
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }}

    public string GreetingMessage
    {{
        get => _greetingMessage;
        set => this.RaiseAndSetIfChanged(ref _greetingMessage, value);
    }}

    public ReactiveCommand<Unit, Unit> GreetCommand {{ get; }}

    public MainWindowViewModel()
    {{
        GreetCommand = ReactiveCommand.Create(ExecuteGreet);
    }}

    private void ExecuteGreet()
    {{
        GreetingMessage = string.IsNullOrWhiteSpace(Name) 
            ? ""Please enter your name!"" 
            : $""Hello, {{Name}}! Welcome to AvaloniaUI!"";
    }}
}}";

    private static string GenerateViewModelBase(string projectName) => $@"using ReactiveUI;

namespace {projectName}.ViewModels;

public class ViewModelBase : ReactiveObject
{{
}}";

    private static string GenerateProgramCs(string projectName) => $@"using Avalonia;
using System;

namespace {projectName};

class Program
{{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}}";
}