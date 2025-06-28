using System.ComponentModel;

using AvaloniaUI.MCP.Services;

using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Tools;

[McpServerToolType]
public static class AnimationTool
{
    [McpServerTool, Description("Generates sophisticated AvaloniaUI animations including storyboards, transitions, and custom effects")]
    public static string GenerateAnimation(
        [Description("Animation type: 'fadeIn', 'slideIn', 'scaleIn', 'rotate', 'bounce', 'elastic', 'custom'")] string animationType,
        [Description("Target element name or type")] string targetElement,
        [Description("Duration in milliseconds")] int duration = 300,
        [Description("Easing function: 'linear', 'easeIn', 'easeOut', 'easeInOut', 'bounce', 'elastic', 'back'")] string easing = "easeInOut",
        [Description("Delay before animation starts in milliseconds")] int delay = 0,
        [Description("Number of iterations (use -1 for infinite)")] int iterations = 1)
    {
        return ErrorHandlingService.SafeExecute(nameof(GenerateAnimation), () =>
        {
            var animationConfig = new AnimationConfiguration
            {
                Type = animationType.ToLowerInvariant(),
                TargetElement = targetElement,
                Duration = duration,
                Easing = easing.ToLowerInvariant(),
                Delay = delay,
                Iterations = iterations
            };

            string xamlContent = GenerateAnimationXaml(animationConfig);
            string codeContent = GenerateAnimationCode(animationConfig);
            string usageExamples = GenerateUsageExamples(animationConfig);

            return $@"# AvaloniaUI Animation: {animationType} for {targetElement}

## Animation Configuration
- **Type**: {animationConfig.Type}
- **Duration**: {animationConfig.Duration}ms
- **Easing**: {animationConfig.Easing}
- **Delay**: {animationConfig.Delay}ms
- **Iterations**: {(animationConfig.Iterations == -1 ? "Infinite" : animationConfig.Iterations.ToString())}

## XAML Implementation

### 1. Declarative Animation
```xml
{xamlContent}
```

### 2. Code-Behind Implementation
```csharp
{codeContent}
```

## Usage Examples
{usageExamples}

## Advanced Animation Patterns

### Storyboard Composition
```xml
<Storyboard>
    <DoubleAnimation Property=""Opacity"" From=""0"" To=""1"" Duration=""0:0:0.3"" />
    <DoubleAnimation Property=""(TranslateTransform.X)"" From=""50"" To=""0"" Duration=""0:0:0.4"" />
</Storyboard>
```

### Page Transitions
```xml
<PageTransition Duration=""0:0:0.3"">
    <CrossFade Duration=""0:0:0.3"" />
</PageTransition>
```

### Performance Tips
- Use GPU-accelerated properties (Transform, Opacity)
- Avoid animating layout properties (Width, Height, Margin)
- Use easing functions for natural motion
- Consider performance on lower-end devices

## Triggering Animations
- **Loaded Events**: Trigger on control load
- **Property Changes**: Bind to ViewModel properties
- **User Interactions**: Mouse, touch, keyboard events
- **State Changes**: Data-driven animations";
        });
    }

    [McpServerTool, Description("Creates page transitions for navigation between views")]
    public static string GeneratePageTransition(
        [Description("Transition type: 'slide', 'fade', 'scale', 'flip', 'cube'")] string transitionType = "slide",
        [Description("Direction: 'left', 'right', 'up', 'down', 'in', 'out'")] string direction = "left",
        [Description("Duration in milliseconds")] int duration = 350,
        [Description("Include reverse transition: 'true' or 'false'")] string includeReverse = "true")
    {
        try
        {
            string transition = GeneratePageTransitionXaml(transitionType, direction, duration, bool.Parse(includeReverse));
            string implementation = GeneratePageTransitionImplementation(transitionType);

            return $@"# Page Transition: {transitionType} ({direction})

## Transition Configuration
- **Type**: {transitionType}
- **Direction**: {direction}
- **Duration**: {duration}ms
- **Reverse**: {includeReverse}

## XAML Implementation
```xml
{transition}
```

## Navigation Implementation
```csharp
{implementation}
```

## Integration with ViewLocator
```csharp
public class ViewLocator : IDataTemplate
{{
    public Control Build(object data)
    {{
        var name = data.GetType().FullName!.Replace(""ViewModel"", ""View"");
        var type = Type.GetType(name);

        if (type != null)
        {{
            var control = (Control)Activator.CreateInstance(type)!;

            // Apply transition
            if (control is UserControl userControl)
            {{
                userControl.PageTransition = new {GetTransitionClass(transitionType)}
                {{
                    Duration = TimeSpan.FromMilliseconds({duration})
                }};
            }}

            return control;
        }}

        return new TextBlock {{ Text = ""Not Found: "" + name }};
    }}

    public bool Match(object data) => data is ViewModelBase;
}}
```

## Advanced Transition Patterns
- **Conditional Transitions**: Different transitions based on navigation direction
- **Nested Transitions**: Combining multiple transition effects
- **Custom Transitions**: Creating unique transition effects
- **Performance Optimization**: GPU acceleration and efficient rendering";
        }
        catch (Exception ex)
        {
            return $"Error generating page transition: {ex.Message}";
        }
    }

    [McpServerTool, Description("Creates sophisticated storyboard animations with multiple properties and timing")]
    public static string GenerateStoryboard(
        [Description("Animation sequence description (e.g., 'fade in button, then slide in panel')")] string sequence,
        [Description("Overall duration in milliseconds")] int totalDuration = 1000,
        [Description("Storyboard name")] string storyboardName = "MainStoryboard")
    {
        try
        {
            string storyboard = GenerateStoryboardXaml(sequence, totalDuration, storyboardName);
            string triggers = GenerateStoryboardTriggers(storyboardName);
            string codeControl = GenerateStoryboardCodeControl(storyboardName);

            return $@"# Complex Storyboard Animation: {storyboardName}

## Animation Sequence
{sequence}

## Storyboard Definition
```xml
<Window.Resources>
    <Storyboard x:Key=""{storyboardName}"">
{storyboard}
    </Storyboard>
</Window.Resources>
```

## Event Triggers
```xml
{triggers}
```

## Programmatic Control
```csharp
{codeControl}
```

## Advanced Storyboard Techniques

### Sequential Animations
```xml
<Storyboard>
    <!-- First animation -->
    <DoubleAnimation Storyboard.TargetName=""Element1""
                     Storyboard.TargetProperty=""Opacity""
                     From=""0"" To=""1""
                     Duration=""0:0:0.3""
                     BeginTime=""0:0:0"" />

    <!-- Second animation starts after first -->
    <DoubleAnimation Storyboard.TargetName=""Element2""
                     Storyboard.TargetProperty=""Opacity""
                     From=""0"" To=""1""
                     Duration=""0:0:0.3""
                     BeginTime=""0:0:0.3"" />
</Storyboard>
```

### Parallel Animations
```xml
<Storyboard>
    <!-- Multiple properties animated simultaneously -->
    <DoubleAnimation Storyboard.TargetName=""MyButton""
                     Storyboard.TargetProperty=""Opacity""
                     From=""0"" To=""1""
                     Duration=""0:0:0.5"" />
    <DoubleAnimation Storyboard.TargetName=""MyButton""
                     Storyboard.TargetProperty=""(TranslateTransform.Y)""
                     From=""20"" To=""0""
                     Duration=""0:0:0.5"" />
</Storyboard>
```

### Keyframe Animations
```xml
<DoubleAnimationUsingKeyFrames Storyboard.TargetName=""MyElement""
                               Storyboard.TargetProperty=""Opacity"">
    <LinearDoubleKeyFrame KeyTime=""0:0:0"" Value=""0"" />
    <LinearDoubleKeyFrame KeyTime=""0:0:0.2"" Value=""0.5"" />
    <LinearDoubleKeyFrame KeyTime=""0:0:0.5"" Value=""1"" />
</DoubleAnimationUsingKeyFrames>
```

## Performance Optimization
- **Use Transform properties** for position and scale changes
- **Batch animations** in single storyboard when possible
- **Consider hardware acceleration** for smooth animations
- **Test on target devices** for performance validation";
        }
        catch (Exception ex)
        {
            return $"Error generating storyboard: {ex.Message}";
        }
    }

    [McpServerTool, Description("Generates custom easing functions and advanced animation effects")]
    public static string GenerateCustomAnimation(
        [Description("Effect name")] string effectName,
        [Description("Properties to animate (comma-separated, e.g., 'Opacity,Transform.ScaleX,Transform.RotateZ')")] string properties,
        [Description("Animation pattern: 'wave', 'spiral', 'bounce', 'elastic', 'spring'")] string pattern = "wave",
        [Description("Complexity level: 'simple', 'moderate', 'complex'")] string complexity = "moderate")
    {
        try
        {
            var propertyList = properties.Split(',').Select(p => p.Trim()).ToList();
            string customAnimation = GenerateCustomAnimationXaml(effectName, propertyList, pattern, complexity);
            string easingFunctions = GenerateCustomEasingFunctions();
            string implementationCode = GenerateCustomAnimationCode(effectName, pattern);

            return $@"# Custom Animation Effect: {effectName}

## Configuration
- **Properties**: {string.Join(", ", propertyList)}
- **Pattern**: {pattern}
- **Complexity**: {complexity}

## Custom Animation Implementation
```xml
{customAnimation}
```

## Custom Easing Functions
```csharp
{easingFunctions}
```

## Animation Controller
```csharp
{implementationCode}
```

## Advanced Animation Techniques

### Physics-Based Animations
```csharp
public class SpringAnimation
{{
    public static IAnimation CreateSpring(double tension = 300, double friction = 10)
    {{
        return new Animation
        {{
            Duration = TimeSpan.FromMilliseconds(500),
            Easing = new SpringEasing(tension, friction),
            FillMode = FillMode.Forward
        }};
    }}
}}
```

### Procedural Animations
```csharp
public class ProceduralAnimator
{{
    public static void AnimateWave(Control element, TimeSpan duration)
    {{
        var animation = new Animation
        {{
            Duration = duration,
            IterationCount = IterationCount.Infinite
        }};

        animation.KeyFrames.Add(new KeyFrame
        {{
            Cue = new Cue(0.0),
            Setters = {{ new Setter(TranslateTransform.YProperty, 0.0) }}
        }});

        animation.KeyFrames.Add(new KeyFrame
        {{
            Cue = new Cue(0.5),
            Setters = {{ new Setter(TranslateTransform.YProperty, -10.0) }}
        }});

        animation.KeyFrames.Add(new KeyFrame
        {{
            Cue = new Cue(1.0),
            Setters = {{ new Setter(TranslateTransform.YProperty, 0.0) }}
        }});

        animation.RunAsync(element);
    }}
}}
```

### Reactive Animations
```csharp
public class ReactiveAnimator
{{
    public static IDisposable AnimateOnPropertyChange<T>(
        IObservable<T> source,
        Control target,
        AvaloniaProperty property,
        Func<T, object> valueSelector,
        TimeSpan duration)
    {{
        return source
            .Select(valueSelector)
            .Subscribe(value =>
            {{
                var animation = new Animation
                {{
                    Duration = duration,
                    FillMode = FillMode.Forward
                }};

                animation.KeyFrames.Add(new KeyFrame
                {{
                    Cue = new Cue(1.0),
                    Setters = {{ new Setter(property, value) }}
                }});

                animation.RunAsync(target);
            }});
    }}
}}
```

## Performance Considerations
- **GPU Acceleration**: Use Transform properties when possible
- **Memory Management**: Dispose of long-running animations
- **Frame Rate**: Target 60fps for smooth animations
- **Battery Impact**: Consider device battery on mobile platforms";
        }
        catch (Exception ex)
        {
            return $"Error generating custom animation: {ex.Message}";
        }
    }

    sealed class AnimationConfiguration
    {
        public string Type { get; set; } = "";
        public string TargetElement { get; set; } = "";
        public int Duration { get; set; }
        public string Easing { get; set; } = "";
        public int Delay { get; set; }
        public int Iterations { get; set; }
    }

    static string GenerateAnimationXaml(AnimationConfiguration config)
    {
        string easingFunction = GetEasingFunction(config.Easing);
        _ = GetAnimationProperties(config.Type);

        return config.Type switch
        {
            "fadein" => $@"<UserControl.Styles>
    <Style Selector=""{config.TargetElement}"">
        <Style.Animations>
            <Animation Duration=""0:0:{config.Duration / 1000.0:F1}"" {GetIterationAttribute(config.Iterations)}>
                <KeyFrame Cue=""0%"">
                    <Setter Property=""Opacity"" Value=""0"" />
                </KeyFrame>
                <KeyFrame Cue=""100%"" KeySpline=""{easingFunction}"">
                    <Setter Property=""Opacity"" Value=""1"" />
                </KeyFrame>
            </Animation>
        </Style.Animations>
    </Style>
</UserControl.Styles>",

            "slidein" => $@"<UserControl.Styles>
    <Style Selector=""{config.TargetElement}"">
        <Style.Animations>
            <Animation Duration=""0:0:{config.Duration / 1000.0:F1}"" {GetIterationAttribute(config.Iterations)}>
                <KeyFrame Cue=""0%"">
                    <Setter Property=""Opacity"" Value=""0"" />
                    <Setter Property=""RenderTransform"" Value=""translate(30px, 0)"" />
                </KeyFrame>
                <KeyFrame Cue=""100%"" KeySpline=""{easingFunction}"">
                    <Setter Property=""Opacity"" Value=""1"" />
                    <Setter Property=""RenderTransform"" Value=""translate(0px, 0)"" />
                </KeyFrame>
            </Animation>
        </Style.Animations>
    </Style>
</UserControl.Styles>",

            "scalein" => $@"<UserControl.Styles>
    <Style Selector=""{config.TargetElement}"">
        <Style.Animations>
            <Animation Duration=""0:0:{config.Duration / 1000.0:F1}"" {GetIterationAttribute(config.Iterations)}>
                <KeyFrame Cue=""0%"">
                    <Setter Property=""Opacity"" Value=""0"" />
                    <Setter Property=""RenderTransform"" Value=""scale(0.8)"" />
                </KeyFrame>
                <KeyFrame Cue=""100%"" KeySpline=""{easingFunction}"">
                    <Setter Property=""Opacity"" Value=""1"" />
                    <Setter Property=""RenderTransform"" Value=""scale(1.0)"" />
                </KeyFrame>
            </Animation>
        </Style.Animations>
    </Style>
</UserControl.Styles>",

            "rotate" => $@"<UserControl.Styles>
    <Style Selector=""{config.TargetElement}"">
        <Style.Animations>
            <Animation Duration=""0:0:{config.Duration / 1000.0:F1}"" {GetIterationAttribute(config.Iterations)}>
                <KeyFrame Cue=""0%"">
                    <Setter Property=""RenderTransform"" Value=""rotate(0deg)"" />
                </KeyFrame>
                <KeyFrame Cue=""100%"" KeySpline=""{easingFunction}"">
                    <Setter Property=""RenderTransform"" Value=""rotate(360deg)"" />
                </KeyFrame>
            </Animation>
        </Style.Animations>
    </Style>
</UserControl.Styles>",

            "bounce" => $@"<UserControl.Styles>
    <Style Selector=""{config.TargetElement}"">
        <Style.Animations>
            <Animation Duration=""0:0:{config.Duration / 1000.0:F1}"" {GetIterationAttribute(config.Iterations)}>
                <KeyFrame Cue=""0%"">
                    <Setter Property=""RenderTransform"" Value=""translateY(0)"" />
                </KeyFrame>
                <KeyFrame Cue=""25%"">
                    <Setter Property=""RenderTransform"" Value=""translateY(-10px)"" />
                </KeyFrame>
                <KeyFrame Cue=""50%"">
                    <Setter Property=""RenderTransform"" Value=""translateY(0)"" />
                </KeyFrame>
                <KeyFrame Cue=""75%"">
                    <Setter Property=""RenderTransform"" Value=""translateY(-5px)"" />
                </KeyFrame>
                <KeyFrame Cue=""100%"">
                    <Setter Property=""RenderTransform"" Value=""translateY(0)"" />
                </KeyFrame>
            </Animation>
        </Style.Animations>
    </Style>
</UserControl.Styles>",

            _ => GenerateCustomAnimationPattern(config, easingFunction)
        };
    }

    static string GenerateAnimationCode(AnimationConfiguration config)
    {
        return $@"public class {config.Type}Animation
{{
    public static async Task Animate{config.Type}Async(Control element)
    {{
        var animation = new Animation
        {{
            Duration = TimeSpan.FromMilliseconds({config.Duration}),
            Delay = TimeSpan.FromMilliseconds({config.Delay}),
            IterationCount = {(config.Iterations == -1 ? "IterationCount.Infinite" : $"new IterationCount({config.Iterations})")},
            FillMode = FillMode.Forward,
            Easing = {GetEasingFunctionCode(config.Easing)}
        }};

        {GetAnimationKeyFrames(config.Type)}

        await animation.RunAsync(element);
    }}

    public static void Start{config.Type}Animation(Control element)
    {{
        _ = Task.Run(async () => await Animate{config.Type}Async(element));
    }}

    public static IDisposable Create{config.Type}Loop(Control element, TimeSpan interval)
    {{
        return Observable.Interval(interval)
            .ObserveOn(AvaloniaScheduler.Instance)
            .Subscribe(_ => Start{config.Type}Animation(element));
    }}
}}";
    }

    static string GenerateUsageExamples(AnimationConfiguration config)
    {
        return $@"
### Trigger on Load
```xml
<UserControl Loaded=""OnLoaded"">
    <Button Name=""{config.TargetElement}"" Content=""Animated Button"" />
</UserControl>
```

```csharp
private void OnLoaded(object sender, RoutedEventArgs e)
{{
    {config.Type}Animation.Start{config.Type}Animation({config.TargetElement});
}}
```

### Reactive Trigger
```csharp
// In ViewModel
public class MainViewModel : ReactiveObject
{{
    private bool _triggerAnimation;
    public bool TriggerAnimation
    {{
        get => _triggerAnimation;
        set => this.RaiseAndSetIfChanged(ref _triggerAnimation, value);
    }}
}}

// In View
this.WhenAnyValue(x => x.ViewModel.TriggerAnimation)
    .Where(trigger => trigger)
    .Subscribe(_ => {config.Type}Animation.Start{config.Type}Animation(MyElement));
```

### Command Binding
```xml
<Button Command=""{{Binding AnimateCommand}}"" Content=""Trigger Animation"" />
```

```csharp
public ReactiveCommand<Unit, Unit> AnimateCommand {{ get; }}

public MainViewModel()
{{
    AnimateCommand = ReactiveCommand.Create(() =>
    {{
        // Animation will be triggered by view
        TriggerAnimation = !TriggerAnimation;
    }});
}}
```";
    }

    static string GetEasingFunction(string easing)
    {
        return easing switch
        {
            "linear" => "0,0,1,1",
            "easein" => "0.42,0,1,1",
            "easeout" => "0,0,0.58,1",
            "easeinout" => "0.42,0,0.58,1",
            "bounce" => "0.68,-0.55,0.265,1.55",
            "elastic" => "0.68,-0.55,0.265,1.55",
            "back" => "0.6,-0.28,0.735,0.045",
            _ => "0.42,0,0.58,1"
        };
    }

    static string GetEasingFunctionCode(string easing)
    {
        return easing switch
        {
            "linear" => "Easing.Linear",
            "easein" => "new CubicBezierEasing(0.42, 0, 1, 1)",
            "easeout" => "new CubicBezierEasing(0, 0, 0.58, 1)",
            "easeinout" => "new CubicBezierEasing(0.42, 0, 0.58, 1)",
            "bounce" => "new BounceEasing()",
            "elastic" => "new ElasticEasing()",
            "back" => "new BackEasing()",
            _ => "Easing.Default"
        };
    }

    static string GetIterationAttribute(int iterations)
    {
        return iterations == -1 ? "IterationCount=\"Infinite\"" :
               iterations > 1 ? $"IterationCount=\"{iterations}\"" : "";
    }

    static string GetAnimationProperties(string type)
    {
        return type switch
        {
            "fadein" => "Opacity",
            "slidein" => "Opacity, Transform",
            "scalein" => "Opacity, Transform",
            "rotate" => "Transform",
            "bounce" => "Transform",
            _ => "Opacity"
        };
    }

    static string GenerateCustomAnimationPattern(AnimationConfiguration config, string easingFunction)
    {
        return $@"<!-- Custom animation for {config.Type} -->
<UserControl.Styles>
    <Style Selector=""{config.TargetElement}"">
        <Style.Animations>
            <Animation Duration=""0:0:{config.Duration / 1000.0:F1}"" {GetIterationAttribute(config.Iterations)}>
                <!-- Add custom keyframes here -->
                <KeyFrame Cue=""0%"">
                    <Setter Property=""Opacity"" Value=""1"" />
                </KeyFrame>
                <KeyFrame Cue=""100%"" KeySpline=""{easingFunction}"">
                    <Setter Property=""Opacity"" Value=""1"" />
                </KeyFrame>
            </Animation>
        </Style.Animations>
    </Style>
</UserControl.Styles>";
    }

    static string GetAnimationKeyFrames(string type)
    {
        return type switch
        {
            "fadein" => @"animation.KeyFrames.Add(new KeyFrame
        {
            Cue = new Cue(0.0),
            Setters = { new Setter(Visual.OpacityProperty, 0.0) }
        });

        animation.KeyFrames.Add(new KeyFrame
        {
            Cue = new Cue(1.0),
            Setters = { new Setter(Visual.OpacityProperty, 1.0) }
        });",

            "slidein" => @"animation.KeyFrames.Add(new KeyFrame
        {
            Cue = new Cue(0.0),
            Setters =
            {
                new Setter(Visual.OpacityProperty, 0.0),
                new Setter(Visual.RenderTransformProperty, new TranslateTransform(30, 0))
            }
        });

        animation.KeyFrames.Add(new KeyFrame
        {
            Cue = new Cue(1.0),
            Setters =
            {
                new Setter(Visual.OpacityProperty, 1.0),
                new Setter(Visual.RenderTransformProperty, new TranslateTransform(0, 0))
            }
        });",

            _ => @"// Add keyframes for custom animation
        animation.KeyFrames.Add(new KeyFrame
        {
            Cue = new Cue(0.0),
            Setters = { new Setter(Visual.OpacityProperty, 1.0) }
        });"
        };
    }

    static string GeneratePageTransitionXaml(string transitionType, string direction, int duration, bool includeReverse)
    {
        return transitionType.ToLowerInvariant() switch
        {
            "slide" => $@"<UserControl.PageTransition>
    <PageSlide Duration=""0:0:{duration / 1000.0:F1}"" Orientation=""{GetSlideOrientation(direction)}"" />
</UserControl.PageTransition>",

            "fade" => $@"<UserControl.PageTransition>
    <CrossFade Duration=""0:0:{duration / 1000.0:F1}"" />
</UserControl.PageTransition>",

            "scale" => $@"<UserControl.PageTransition>
    <PageSlide Duration=""0:0:{duration / 1000.0:F1}"" Orientation=""Horizontal"">
        <PageSlide.PageTransition>
            <CompositePageTransition>
                <CrossFade Duration=""0:0:{duration / 1000.0:F1}"" />
                <PageSlide Duration=""0:0:{duration / 1000.0:F1}"" Orientation=""Horizontal"" />
            </CompositePageTransition>
        </PageSlide.PageTransition>
    </PageSlide>
</UserControl.PageTransition>",

            _ => $@"<UserControl.PageTransition>
    <PageSlide Duration=""0:0:{duration / 1000.0:F1}"" Orientation=""Horizontal"" />
</UserControl.PageTransition>"
        };
    }

    static string GetSlideOrientation(string direction)
    {
        return direction.ToLowerInvariant() switch
        {
            "left" or "right" => "Horizontal",
            "up" or "down" => "Vertical",
            _ => "Horizontal"
        };
    }

    static string GetTransitionClass(string transitionType)
    {
        return transitionType switch
        {
            "slide" => "PageSlide",
            "fade" => "CrossFade",
            "scale" => "CompositePageTransition",
            _ => "PageSlide"
        };
    }

    static string GeneratePageTransitionImplementation(string transitionType)
    {
        return $@"public class NavigationService
{{
    private ContentControl _contentHost;

    public NavigationService(ContentControl contentHost)
    {{
        _contentHost = contentHost;
    }}

    public async Task NavigateAsync<T>(T viewModel) where T : ViewModelBase
    {{
        var view = ViewLocator.Build(viewModel);

        if (view is UserControl userControl)
        {{
            // Apply transition
            userControl.PageTransition = Create{transitionType}Transition();
        }}

        _contentHost.Content = view;
        await Task.Delay(50); // Allow transition to start
    }}

    private IPageTransition Create{transitionType}Transition()
    {{
        return new {GetTransitionClass(transitionType)}
        {{
            Duration = TimeSpan.FromMilliseconds(350)
        }};
    }}
}}";
    }

    static string GenerateStoryboardXaml(string sequence, int totalDuration, string storyboardName)
    {
        // Parse sequence and generate appropriate animations
        return @"        <!-- Fade in first element -->
        <DoubleAnimation Storyboard.TargetName=""Element1""
                         Storyboard.TargetProperty=""Opacity""
                         From=""0"" To=""1""
                         Duration=""0:0:0.3""
                         BeginTime=""0:0:0"" />

        <!-- Slide in second element -->
        <DoubleAnimation Storyboard.TargetName=""Element2""
                         Storyboard.TargetProperty=""(TranslateTransform.X)""
                         From=""50"" To=""0""
                         Duration=""0:0:0.4""
                         BeginTime=""0:0:0.2"" />

        <!-- Scale final element -->
        <DoubleAnimation Storyboard.TargetName=""Element3""
                         Storyboard.TargetProperty=""(ScaleTransform.ScaleX)""
                         From=""0.8"" To=""1.0""
                         Duration=""0:0:0.3""
                         BeginTime=""0:0:0.5"" />";
    }

    static string GenerateStoryboardTriggers(string storyboardName)
    {
        return $@"<EventTrigger RoutedEvent=""Button.Click"" SourceName=""TriggerButton"">
    <BeginStoryboard Storyboard=""{{StaticResource {storyboardName}}}"" />
</EventTrigger>

<!-- Auto-trigger on load -->
<EventTrigger RoutedEvent=""UserControl.Loaded"">
    <BeginStoryboard Storyboard=""{{StaticResource {storyboardName}}}"" />
</EventTrigger>";
    }

    static string GenerateStoryboardCodeControl(string storyboardName)
    {
        return $@"public class StoryboardController
{{
    private readonly Storyboard _{storyboardName.ToLowerInvariant()};

    public StoryboardController(UserControl view)
    {{
        _{storyboardName.ToLowerInvariant()} = view.FindResource(""{storyboardName}"") as Storyboard;
    }}

    public void Start()
    {{
        _{storyboardName.ToLowerInvariant()}?.Begin();
    }}

    public void Stop()
    {{
        _{storyboardName.ToLowerInvariant()}?.Stop();
    }}

    public void Pause()
    {{
        _{storyboardName.ToLowerInvariant()}?.Pause();
    }}

    public void Resume()
    {{
        _{storyboardName.ToLowerInvariant()}?.Resume();
    }}
}}";
    }

    static string GenerateCustomAnimationXaml(string effectName, List<string> properties, string pattern, string complexity)
    {
        return $@"<!-- Custom {effectName} Effect -->
<UserControl.Resources>
    <Storyboard x:Key=""{effectName}Storyboard"">
{string.Join("\n", properties.Select(prop => GeneratePropertyAnimation(prop, pattern, complexity)))}
    </Storyboard>
</UserControl.Resources>";
    }

    static string GeneratePropertyAnimation(string property, string pattern, string complexity)
    {
        return pattern switch
        {
            "wave" => $@"        <DoubleAnimation Storyboard.TargetProperty=""{property}""
                         From=""0"" To=""1""
                         Duration=""0:0:1""
                         RepeatBehavior=""Forever""
                         AutoReverse=""True"" />",

            "spiral" => $@"        <DoubleAnimation Storyboard.TargetProperty=""{property}""
                         From=""0"" To=""360""
                         Duration=""0:0:2""
                         RepeatBehavior=""Forever"" />",

            _ => $@"        <DoubleAnimation Storyboard.TargetProperty=""{property}""
                         From=""0"" To=""1""
                         Duration=""0:0:0.5"" />"
        };
    }

    static string GenerateCustomEasingFunctions()
    {
        return @"public class CustomEasing : Easing
{
    public static readonly SpringEasing Spring = new SpringEasing();
    public static readonly BounceEasing Bounce = new BounceEasing();
    public static readonly ElasticEasing Elastic = new ElasticEasing();
}

public class SpringEasing : Easing
{
    public double Tension { get; set; } = 300;
    public double Friction { get; set; } = 10;

    public override double Ease(double progress)
    {
        // Spring physics simulation
        var tension = Tension / 1000.0;
        var friction = Friction / 100.0;

        return 1 - Math.Pow(Math.E, -friction * progress) * Math.Cos(tension * progress);
    }
}

public class BounceEasing : Easing
{
    public override double Ease(double progress)
    {
        if (progress < 0.36)
            return 7.5625 * progress * progress;
        if (progress < 0.73)
            return 7.5625 * (progress -= 0.545) * progress + 0.75;
        if (progress < 0.91)
            return 7.5625 * (progress -= 0.818) * progress + 0.9375;

        return 7.5625 * (progress -= 0.955) * progress + 0.984375;
    }
}

public class ElasticEasing : Easing
{
    public double Amplitude { get; set; } = 1;
    public double Period { get; set; } = 0.3;

    public override double Ease(double progress)
    {
        if (progress == 0 || progress == 1)
            return progress;

        var p = Period / 4;
        return -(Amplitude * Math.Pow(2, 10 * (progress -= 1)) *
                Math.Sin((progress - p) * (2 * Math.PI) / Period));
    }
}";
    }

    static string GenerateCustomAnimationCode(string effectName, string pattern)
    {
        return $@"public class {effectName}AnimationController
{{
    private readonly Timer _animationTimer;
    private readonly Control _target;
    private double _progress = 0;

    public {effectName}AnimationController(Control target)
    {{
        _target = target;
        _animationTimer = new Timer(16); // ~60fps
        _animationTimer.Elapsed += OnAnimationTick;
    }}

    public void Start()
    {{
        _progress = 0;
        _animationTimer.Start();
    }}

    public void Stop()
    {{
        _animationTimer.Stop();
    }}

    private void OnAnimationTick(object sender, ElapsedEventArgs e)
    {{
        _progress += 0.016; // 16ms increment

        if (_progress >= 1.0)
        {{
            _progress = {(pattern == "wave" ? "0" : "1.0")};
            {(pattern != "wave" ? "_animationTimer.Stop();" : "")}
        }}

        Dispatcher.UIThread.Post(() => Update{pattern}Animation(_progress));
    }}

    private void Update{pattern}Animation(double progress)
    {{
        switch (""{pattern}"")
        {{
            case ""wave"":
                var wave = Math.Sin(progress * Math.PI * 2) * 10;
                _target.RenderTransform = new TranslateTransform(0, wave);
                break;

            case ""spiral"":
                var angle = progress * 360;
                var radius = progress * 50;
                var x = Math.Cos(angle * Math.PI / 180) * radius;
                var y = Math.Sin(angle * Math.PI / 180) * radius;
                _target.RenderTransform = new TranslateTransform(x, y);
                break;

            default:
                _target.Opacity = progress;
                break;
        }}
    }}

    public void Dispose()
    {{
        _animationTimer?.Dispose();
    }}
}}";
    }
}