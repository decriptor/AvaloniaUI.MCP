using System.ComponentModel;
using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Tools;

[McpServerToolType]
public static class UIUXDesignTool
{
    [McpServerTool, Description("Generates responsive design patterns with adaptive layouts and breakpoints")]
    public static string GenerateResponsiveDesign(
        [Description("Layout type: 'fluid-grid', 'adaptive-panels', 'responsive-navigation', 'flexible-forms'")] string layoutType,
        [Description("Target devices: 'mobile,tablet,desktop' (comma-separated)")] string targetDevices = "mobile,tablet,desktop",
        [Description("Include touch gestures: 'true' or 'false'")] string includeTouchGestures = "true",
        [Description("Breakpoint strategy: 'content-based', 'device-based', 'hybrid'")] string breakpointStrategy = "content-based")
    {
        try
        {
            var devices = targetDevices.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(d => d.Trim()).ToList();

            var config = new ResponsiveConfiguration
            {
                LayoutType = layoutType.ToLowerInvariant(),
                TargetDevices = devices,
                IncludeTouchGestures = bool.Parse(includeTouchGestures),
                BreakpointStrategy = breakpointStrategy.ToLowerInvariant()
            };

            var responsiveXaml = GenerateResponsiveLayout(config);
            var breakpoints = GenerateBreakpointSystem(config);
            var touchHandlers = config.IncludeTouchGestures ? GenerateTouchGestureHandlers() : "";
            var deviceAdaptation = GenerateDeviceAdaptation(config);

            return $@"# Responsive Design: {layoutType}

## Configuration
- **Layout Type**: {config.LayoutType}
- **Target Devices**: {string.Join(", ", config.TargetDevices)}
- **Touch Gestures**: {config.IncludeTouchGestures}
- **Breakpoint Strategy**: {config.BreakpointStrategy}

## Responsive Layout
```xml
{responsiveXaml}
```

## Breakpoint System
```csharp
{breakpoints}
```

{(config.IncludeTouchGestures ? $@"## Touch Gesture Handlers
```csharp
{touchHandlers}
```" : "")}

## Device Adaptation
```csharp
{deviceAdaptation}
```

## Responsive Design Principles
- **Mobile First**: Design for smallest screen first, then enhance
- **Flexible Layouts**: Use relative units and flexible containers
- **Touch-Friendly**: Minimum 44px touch targets
- **Performance**: Optimize for mobile network conditions
- **Content Priority**: Show most important content first";
        }
        catch (Exception ex)
        {
            return $"Error generating responsive design: {ex.Message}";
        }
    }

    [McpServerTool, Description("Generates UX patterns for improved user experience and interaction design")]
    public static string GenerateUXPatterns(
        [Description("Pattern type: 'loading-states', 'error-handling', 'feedback-system', 'progressive-disclosure', 'micro-interactions'")] string patternType,
        [Description("Animation style: 'subtle', 'moderate', 'bold'")] string animationStyle = "moderate",
        [Description("Include user guidance: 'true' or 'false'")] string includeGuidance = "true",
        [Description("Feedback timing: 'immediate', 'delayed', 'contextual'")] string feedbackTiming = "immediate")
    {
        try
        {
            var config = new UXConfiguration
            {
                PatternType = patternType.ToLowerInvariant(),
                AnimationStyle = animationStyle.ToLowerInvariant(),
                IncludeUserGuidance = bool.Parse(includeGuidance),
                FeedbackTiming = feedbackTiming.ToLowerInvariant()
            };

            var patternImplementation = GenerateUXPatternImplementation(config);
            var userGuidance = config.IncludeUserGuidance ? GenerateUserGuidanceSystem(config) : "";
            var microInteractions = GenerateMicroInteractions(config);
            var usabilityGuidelines = GenerateUsabilityGuidelines(config);

            return $@"# UX Pattern: {patternType}

## Configuration
- **Pattern Type**: {config.PatternType}
- **Animation Style**: {config.AnimationStyle}
- **User Guidance**: {config.IncludeUserGuidance}
- **Feedback Timing**: {config.FeedbackTiming}

## Pattern Implementation
```xml
{patternImplementation}
```

{(config.IncludeUserGuidance ? $@"## User Guidance System
```csharp
{userGuidance}
```" : "")}

## Micro-Interactions
```csharp
{microInteractions}
```

## Usability Guidelines
{usabilityGuidelines}

## UX Best Practices
- **Clarity**: Make interactions obvious and predictable
- **Feedback**: Provide immediate response to user actions
- **Consistency**: Use familiar patterns and conventions
- **Efficiency**: Minimize cognitive load and steps to completion
- **Forgiveness**: Allow users to undo actions and recover from errors";
        }
        catch (Exception ex)
        {
            return $"Error generating UX patterns: {ex.Message}";
        }
    }

    [McpServerTool, Description("Creates design systems with consistent visual components and guidelines")]
    public static string GenerateDesignSystem(
        [Description("Brand style: 'corporate', 'modern', 'playful', 'minimal'")] string brandStyle,
        [Description("Color palette: 'monochrome', 'vibrant', 'pastel', 'dark', 'custom'")] string colorPalette = "modern",
        [Description("Typography scale: 'compact', 'standard', 'large'")] string typographyScale = "standard",
        [Description("Include component library: 'true' or 'false'")] string includeComponents = "true")
    {
        try
        {
            var config = new DesignSystemConfiguration
            {
                BrandStyle = brandStyle.ToLowerInvariant(),
                ColorPalette = colorPalette.ToLowerInvariant(),
                TypographyScale = typographyScale.ToLowerInvariant(),
                IncludeComponents = bool.Parse(includeComponents)
            };

            var colorSystem = GenerateColorSystem(config);
            var typography = GenerateTypographySystem(config);
            var spacing = GenerateSpacingSystem(config);
            var components = config.IncludeComponents ? GenerateComponentLibrary(config) : "";

            return $@"# Design System: {brandStyle}

## Configuration
- **Brand Style**: {config.BrandStyle}
- **Color Palette**: {config.ColorPalette}
- **Typography Scale**: {config.TypographyScale}
- **Component Library**: {config.IncludeComponents}

## Color System
```xml
{colorSystem}
```

## Typography System
```xml
{typography}
```

## Spacing System
```xml
{spacing}
```

{(config.IncludeComponents ? $@"## Component Library
```xml
{components}
```" : "")}

## Design Principles
- **Consistency**: Maintain visual and functional consistency
- **Hierarchy**: Clear information hierarchy and visual structure
- **Simplicity**: Remove unnecessary elements and complexity
- **Accessibility**: Ensure usability for all users
- **Scalability**: Design components that work at any scale";
        }
        catch (Exception ex)
        {
            return $"Error generating design system: {ex.Message}";
        }
    }

    private class ResponsiveConfiguration
    {
        public string LayoutType { get; set; } = "";
        public List<string> TargetDevices { get; set; } = new();
        public bool IncludeTouchGestures { get; set; }
        public string BreakpointStrategy { get; set; } = "";
    }

    private class UXConfiguration
    {
        public string PatternType { get; set; } = "";
        public string AnimationStyle { get; set; } = "";
        public bool IncludeUserGuidance { get; set; }
        public string FeedbackTiming { get; set; } = "";
    }

    private class DesignSystemConfiguration
    {
        public string BrandStyle { get; set; } = "";
        public string ColorPalette { get; set; } = "";
        public string TypographyScale { get; set; } = "";
        public bool IncludeComponents { get; set; }
    }

    private static string GenerateResponsiveLayout(ResponsiveConfiguration config)
    {
        return config.LayoutType switch
        {
            "fluid-grid" => GenerateFluidGrid(config),
            "adaptive-panels" => GenerateAdaptivePanels(config),
            "responsive-navigation" => GenerateResponsiveNavigation(config),
            "flexible-forms" => GenerateFlexibleForms(config),
            _ => GenerateGenericResponsiveLayout(config)
        };
    }

    private static string GenerateFluidGrid(ResponsiveConfiguration config)
    {
        return @"<Grid>
    <Grid.Styles>
        <Style Selector=""Grid"">
            <Setter Property=""ColumnDefinitions"" Value=""*,*,*,*"" />
            <Setter Property=""RowDefinitions"" Value=""Auto,*,Auto"" />
        </Style>
        
        <!-- Mobile Breakpoint -->
        <Style Selector=""Grid"" x:Key=""Mobile"">
            <Style.Setters>
                <Setter Property=""ColumnDefinitions"" Value=""*"" />
                <Setter Property=""RowDefinitions"" Value=""Auto,Auto,Auto,Auto,*,Auto"" />
            </Style.Setters>
        </Style>
        
        <!-- Tablet Breakpoint -->
        <Style Selector=""Grid"" x:Key=""Tablet"">
            <Style.Setters>
                <Setter Property=""ColumnDefinitions"" Value=""*,*"" />
                <Setter Property=""RowDefinitions"" Value=""Auto,*,*,Auto"" />
            </Style.Setters>
        </Style>
    </Grid.Styles>
    
    <!-- Header -->
    <Border Grid.Column=""0"" Grid.ColumnSpan=""4"" Grid.Row=""0""
            Background=""{StaticResource HeaderBrush}""
            Padding=""20,16"">
        <TextBlock Text=""Responsive Header"" 
                   Classes=""heading1""
                   HorizontalAlignment=""Center"" />
    </Border>
    
    <!-- Main Content Grid -->
    <Grid Grid.Column=""0"" Grid.ColumnSpan=""4"" Grid.Row=""1""
          Margin=""20"" RowDefinitions=""*"" ColumnDefinitions=""2*,*"">
        
        <ScrollViewer Grid.Column=""0"" Padding=""0,0,20,0"">
            <StackPanel Spacing=""16"">
                <TextBlock Text=""Main Content Area"" Classes=""heading2"" />
                <TextBlock Text=""This content adapts to different screen sizes using fluid grids and responsive breakpoints."" 
                           TextWrapping=""Wrap"" />
            </StackPanel>
        </ScrollViewer>
        
        <Border Grid.Column=""1"" 
                Background=""{StaticResource SidebarBrush}""
                Padding=""16"">
            <StackPanel Spacing=""12"">
                <TextBlock Text=""Sidebar"" Classes=""heading3"" />
                <TextBlock Text=""Additional content"" TextWrapping=""Wrap"" />
            </StackPanel>
        </Border>
    </Grid>
    
    <!-- Footer -->
    <Border Grid.Column=""0"" Grid.ColumnSpan=""4"" Grid.Row=""2""
            Background=""{StaticResource FooterBrush}""
            Padding=""20,12"">
        <TextBlock Text=""Footer Content"" 
                   HorizontalAlignment=""Center"" />
    </Border>
</Grid>";
    }

    private static string GenerateAdaptivePanels(ResponsiveConfiguration config)
    {
        return @"<Panel>
    <!-- Adaptive Panel Container -->
    <AdaptiveContainer x:Name=""MainContainer"">
        <AdaptiveContainer.Breakpoints>
            <Breakpoint MinWidth=""0"" MaxWidth=""640"">
                <Setter Property=""Orientation"" Value=""Vertical"" />
                <Setter Property=""Spacing"" Value=""12"" />
            </Breakpoint>
            <Breakpoint MinWidth=""641"" MaxWidth=""1024"">
                <Setter Property=""Orientation"" Value=""Horizontal"" />
                <Setter Property=""Spacing"" Value=""20"" />
            </Breakpoint>
            <Breakpoint MinWidth=""1025"">
                <Setter Property=""Orientation"" Value=""Horizontal"" />
                <Setter Property=""Spacing"" Value=""32"" />
            </Breakpoint>
        </AdaptiveContainer.Breakpoints>
        
        <!-- Primary Panel -->
        <Panel Classes=""primary-panel"">
            <Panel.Styles>
                <Style Selector=""Panel.primary-panel"">
                    <Setter Property=""Background"" Value=""{StaticResource PrimaryBrush}"" />
                    <Setter Property=""Padding"" Value=""24"" />
                    <Setter Property=""MinHeight"" Value=""300"" />
                </Style>
            </Panel.Styles>
            
            <StackPanel Spacing=""16"">
                <TextBlock Text=""Primary Content"" Classes=""heading2"" />
                <TextBlock Text=""This panel adapts its layout based on available screen space."" 
                           TextWrapping=""Wrap"" />
                <Button Content=""Action Button"" Classes=""primary"" />
            </StackPanel>
        </Panel>
        
        <!-- Secondary Panel -->
        <Panel Classes=""secondary-panel"">
            <Panel.Styles>
                <Style Selector=""Panel.secondary-panel"">
                    <Setter Property=""Background"" Value=""{StaticResource SecondaryBrush}"" />
                    <Setter Property=""Padding"" Value=""20"" />
                    <Setter Property=""MinHeight"" Value=""200"" />
                </Style>
            </Panel.Styles>
            
            <StackPanel Spacing=""12"">
                <TextBlock Text=""Secondary Content"" Classes=""heading3"" />
                <TextBlock Text=""Supporting information and actions."" 
                           TextWrapping=""Wrap"" />
                <Button Content=""Secondary Action"" Classes=""secondary"" />
            </StackPanel>
        </Panel>
    </AdaptiveContainer>
</Panel>";
    }

    private static string GenerateResponsiveNavigation(ResponsiveConfiguration config)
    {
        return @"<DockPanel LastChildFill=""True"">
    <!-- Navigation Header -->
    <Border DockPanel.Dock=""Top"" 
            Background=""{StaticResource NavigationBrush}""
            Padding=""16,12"">
        <Grid ColumnDefinitions=""*,Auto"">
            <TextBlock Grid.Column=""0"" 
                       Text=""Application Name"" 
                       Classes=""logo""
                       VerticalAlignment=""Center"" />
            
            <!-- Mobile Menu Toggle -->
            <Button Grid.Column=""1""
                    x:Name=""MenuToggle""
                    Classes=""hamburger""
                    Content=""☰""
                    IsVisible=""{Binding IsMobileView}""
                    Command=""{Binding ToggleMenuCommand}"" />
        </Grid>
    </Border>
    
    <!-- Navigation Menu -->
    <Border DockPanel.Dock=""Left""
            x:Name=""NavigationPanel""
            Background=""{StaticResource NavigationBrush}""
            Width=""250""
            IsVisible=""{Binding IsMenuVisible}"">
        <Border.Styles>
            <!-- Desktop Navigation -->
            <Style Selector=""Border"" x:Key=""Desktop"">
                <Setter Property=""Width"" Value=""250"" />
                <Setter Property=""IsVisible"" Value=""True"" />
            </Style>
            
            <!-- Mobile Navigation Overlay -->
            <Style Selector=""Border"" x:Key=""Mobile"">
                <Setter Property=""Width"" Value=""280"" />
                <Setter Property=""IsVisible"" Value=""{Binding IsMenuOpen}"" />
                <Setter Property=""ZIndex"" Value=""1000"" />
            </Style>
        </Border.Styles>
        
        <ScrollViewer>
            <StackPanel Spacing=""8"" Margin=""16"">
                <Button Content=""Dashboard"" Classes=""nav-item"" />
                <Button Content=""Projects"" Classes=""nav-item"" />
                <Button Content=""Settings"" Classes=""nav-item"" />
                <Button Content=""Profile"" Classes=""nav-item"" />
                <Separator />
                <Button Content=""Help"" Classes=""nav-item secondary"" />
                <Button Content=""Logout"" Classes=""nav-item secondary"" />
            </StackPanel>
        </ScrollViewer>
    </Border>
    
    <!-- Main Content Area -->
    <ScrollViewer Padding=""24"">
        <StackPanel Spacing=""20"">
            <TextBlock Text=""Main Content"" Classes=""heading1"" />
            <TextBlock Text=""The navigation adapts to screen size - showing as a sidebar on desktop and a slide-out menu on mobile."" 
                       TextWrapping=""Wrap"" />
        </StackPanel>
    </ScrollViewer>
</DockPanel>";
    }

    private static string GenerateFlexibleForms(ResponsiveConfiguration config)
    {
        return @"<ScrollViewer>
    <Panel Margin=""20"" MaxWidth=""800"">
        <StackPanel Spacing=""24"">
            <!-- Form Header -->
            <TextBlock Text=""Responsive Form"" Classes=""heading1"" 
                       HorizontalAlignment=""Center"" />
            
            <!-- Form Grid Container -->
            <Grid x:Name=""FormGrid"">
                <Grid.Styles>
                    <!-- Desktop Layout: 2 columns -->
                    <Style Selector=""Grid"" x:Key=""Desktop"">
                        <Setter Property=""ColumnDefinitions"" Value=""*,20,*"" />
                        <Setter Property=""RowDefinitions"" Value=""Auto,Auto,Auto,Auto,Auto"" />
                    </Style>
                    
                    <!-- Mobile Layout: 1 column -->
                    <Style Selector=""Grid"" x:Key=""Mobile"">
                        <Setter Property=""ColumnDefinitions"" Value=""*"" />
                        <Setter Property=""RowDefinitions"" Value=""Auto,Auto,Auto,Auto,Auto,Auto,Auto,Auto,Auto"" />
                    </Style>
                </Grid.Styles>
                
                <!-- First Name -->
                <StackPanel Grid.Row=""0"" Grid.Column=""0"" Spacing=""8"">
                    <TextBlock Text=""First Name"" Classes=""form-label"" />
                    <TextBox Watermark=""Enter first name"" 
                             Classes=""form-input"" />
                </StackPanel>
                
                <!-- Last Name -->
                <StackPanel Grid.Row=""0"" Grid.Column=""2"" Spacing=""8"">
                    <TextBlock Text=""Last Name"" Classes=""form-label"" />
                    <TextBox Watermark=""Enter last name"" 
                             Classes=""form-input"" />
                </StackPanel>
                
                <!-- Email (Full Width) -->
                <StackPanel Grid.Row=""1"" Grid.Column=""0"" Grid.ColumnSpan=""3"" 
                            Spacing=""8"" Margin=""0,16,0,0"">
                    <TextBlock Text=""Email Address"" Classes=""form-label"" />
                    <TextBox Watermark=""Enter email address"" 
                             Classes=""form-input"" />
                </StackPanel>
                
                <!-- Phone -->
                <StackPanel Grid.Row=""2"" Grid.Column=""0"" Spacing=""8"" Margin=""0,16,0,0"">
                    <TextBlock Text=""Phone Number"" Classes=""form-label"" />
                    <TextBox Watermark=""(555) 123-4567"" 
                             Classes=""form-input"" />
                </StackPanel>
                
                <!-- Country -->
                <StackPanel Grid.Row=""2"" Grid.Column=""2"" Spacing=""8"" Margin=""0,16,0,0"">
                    <TextBlock Text=""Country"" Classes=""form-label"" />
                    <ComboBox Classes=""form-input"">
                        <ComboBoxItem Content=""United States"" />
                        <ComboBoxItem Content=""Canada"" />
                        <ComboBoxItem Content=""United Kingdom"" />
                    </ComboBox>
                </StackPanel>
                
                <!-- Message (Full Width) -->
                <StackPanel Grid.Row=""3"" Grid.Column=""0"" Grid.ColumnSpan=""3"" 
                            Spacing=""8"" Margin=""0,16,0,0"">
                    <TextBlock Text=""Message"" Classes=""form-label"" />
                    <TextBox AcceptsReturn=""True"" 
                             Height=""120"" 
                             TextWrapping=""Wrap""
                             Watermark=""Enter your message..."" 
                             Classes=""form-input"" />
                </StackPanel>
                
                <!-- Form Actions -->
                <StackPanel Grid.Row=""4"" Grid.Column=""0"" Grid.ColumnSpan=""3"" 
                            Orientation=""Horizontal"" 
                            HorizontalAlignment=""Right""
                            Spacing=""12"" 
                            Margin=""0,24,0,0"">
                    <Button Content=""Cancel"" Classes=""secondary"" />
                    <Button Content=""Submit"" Classes=""primary"" />
                </StackPanel>
            </Grid>
        </StackPanel>
    </Panel>
</ScrollViewer>";
    }

    private static string GenerateGenericResponsiveLayout(ResponsiveConfiguration config)
    {
        return @"<Grid>
    <!-- Generic responsive layout with flexible columns -->
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width=""*"" />
        <ColumnDefinition Width=""Auto"" />
        <ColumnDefinition Width=""*"" />
    </Grid.ColumnDefinitions>
    
    <ContentPresenter Grid.Column=""0"" Content=""{Binding MainContent}"" />
    <GridSplitter Grid.Column=""1"" Width=""4"" />
    <ContentPresenter Grid.Column=""2"" Content=""{Binding SideContent}"" />
</Grid>";
    }

    private static string GenerateBreakpointSystem(ResponsiveConfiguration config)
    {
        return @"// Responsive Breakpoint Service
public class BreakpointService : INotifyPropertyChanged
{
    private Size _currentSize;
    private BreakpointType _currentBreakpoint;

    public event PropertyChangedEventHandler? PropertyChanged;

    public BreakpointType CurrentBreakpoint
    {
        get => _currentBreakpoint;
        private set
        {
            if (_currentBreakpoint != value)
            {
                _currentBreakpoint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentBreakpoint)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMobile)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsTablet)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDesktop)));
            }
        }
    }

    public bool IsMobile => CurrentBreakpoint == BreakpointType.Mobile;
    public bool IsTablet => CurrentBreakpoint == BreakpointType.Tablet;
    public bool IsDesktop => CurrentBreakpoint == BreakpointType.Desktop;

    public void UpdateSize(Size newSize)
    {
        _currentSize = newSize;
        CurrentBreakpoint = DetermineBreakpoint(newSize.Width);
    }

    private BreakpointType DetermineBreakpoint(double width)
    {
        return width switch
        {
            <= 640 => BreakpointType.Mobile,
            <= 1024 => BreakpointType.Tablet,
            _ => BreakpointType.Desktop
        };
    }
}

public enum BreakpointType
{
    Mobile,
    Tablet,
    Desktop
}

// Breakpoint-aware UserControl base class
public abstract class ResponsiveUserControl : UserControl
{
    protected BreakpointService BreakpointService { get; }

    protected ResponsiveUserControl()
    {
        BreakpointService = new BreakpointService();
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        BreakpointService.UpdateSize(e.NewSize);
        OnBreakpointChanged(BreakpointService.CurrentBreakpoint);
    }

    protected virtual void OnBreakpointChanged(BreakpointType breakpoint)
    {
        // Override in derived classes to handle breakpoint changes
    }
}";
    }

    private static string GenerateTouchGestureHandlers()
    {
        return @"// Touch Gesture Handler Service
public class TouchGestureService
{
    private readonly Dictionary<Control, GestureConfiguration> _configurations = new();

    public void RegisterSwipeGesture(Control control, Action<SwipeDirection> onSwipe)
    {
        var config = GetOrCreateConfiguration(control);
        config.SwipeHandler = onSwipe;
        
        control.PointerPressed += OnPointerPressed;
        control.PointerMoved += OnPointerMoved;
        control.PointerReleased += OnPointerReleased;
    }

    public void RegisterPinchGesture(Control control, Action<double> onPinch)
    {
        var config = GetOrCreateConfiguration(control);
        config.PinchHandler = onPinch;
        
        // Register multi-touch events
        control.PointerPressed += OnPointerPressed;
        control.PointerMoved += OnPointerMoved;
        control.PointerReleased += OnPointerReleased;
    }

    public void RegisterTapGesture(Control control, Action onTap, Action? onDoubleTap = null)
    {
        var config = GetOrCreateConfiguration(control);
        config.TapHandler = onTap;
        config.DoubleTapHandler = onDoubleTap;
        
        control.Tapped += (s, e) => onTap?.Invoke();
        control.DoubleTapped += (s, e) => onDoubleTap?.Invoke();
    }

    private GestureConfiguration GetOrCreateConfiguration(Control control)
    {
        if (!_configurations.TryGetValue(control, out var config))
        {
            config = new GestureConfiguration();
            _configurations[control] = config;
        }
        return config;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control control && _configurations.TryGetValue(control, out var config))
        {
            config.StartPoint = e.GetPosition(control);
            config.StartTime = DateTime.UtcNow;
            config.IsTracking = true;
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is Control control && _configurations.TryGetValue(control, out var config) && config.IsTracking)
        {
            var currentPoint = e.GetPosition(control);
            var distance = CalculateDistance(config.StartPoint, currentPoint);
            
            // Handle swipe detection
            if (distance > 50) // Minimum swipe distance
            {
                var direction = DetermineSwipeDirection(config.StartPoint, currentPoint);
                config.SwipeHandler?.Invoke(direction);
                config.IsTracking = false;
            }
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Control control && _configurations.TryGetValue(control, out var config))
        {
            config.IsTracking = false;
        }
    }

    private double CalculateDistance(Point start, Point end)
    {
        var dx = end.X - start.X;
        var dy = end.Y - start.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private SwipeDirection DetermineSwipeDirection(Point start, Point end)
    {
        var dx = end.X - start.X;
        var dy = end.Y - start.Y;

        if (Math.Abs(dx) > Math.Abs(dy))
        {
            return dx > 0 ? SwipeDirection.Right : SwipeDirection.Left;
        }
        else
        {
            return dy > 0 ? SwipeDirection.Down : SwipeDirection.Up;
        }
    }
}

public class GestureConfiguration
{
    public Point StartPoint { get; set; }
    public DateTime StartTime { get; set; }
    public bool IsTracking { get; set; }
    public Action<SwipeDirection>? SwipeHandler { get; set; }
    public Action<double>? PinchHandler { get; set; }
    public Action? TapHandler { get; set; }
    public Action? DoubleTapHandler { get; set; }
}

public enum SwipeDirection
{
    Left,
    Right,
    Up,
    Down
}";
    }

    private static string GenerateDeviceAdaptation(ResponsiveConfiguration config)
    {
        return @"// Device Adaptation Service
public class DeviceAdaptationService
{
    private readonly Dictionary<string, DeviceProfile> _deviceProfiles;

    public DeviceAdaptationService()
    {
        _deviceProfiles = new Dictionary<string, DeviceProfile>
        {
            [""mobile""] = new DeviceProfile
            {
                MinTouchTarget = 44,
                PreferredFontSize = 16,
                MaxColumns = 1,
                UseCompactLayout = true,
                ShowSecondaryActions = false
            },
            [""tablet""] = new DeviceProfile
            {
                MinTouchTarget = 44,
                PreferredFontSize = 16,
                MaxColumns = 2,
                UseCompactLayout = false,
                ShowSecondaryActions = true
            },
            [""desktop""] = new DeviceProfile
            {
                MinTouchTarget = 32,
                PreferredFontSize = 14,
                MaxColumns = 4,
                UseCompactLayout = false,
                ShowSecondaryActions = true
            }
        };
    }

    public DeviceProfile GetCurrentProfile(double screenWidth)
    {
        return screenWidth switch
        {
            <= 640 => _deviceProfiles[""mobile""],
            <= 1024 => _deviceProfiles[""tablet""],
            _ => _deviceProfiles[""desktop""]
        };
    }

    public void ApplyDeviceAdaptations(Control control, DeviceProfile profile)
    {
        // Apply font size adaptations
        if (control is TextBlock textBlock)
        {
            textBlock.FontSize = profile.PreferredFontSize;
        }

        // Apply touch target adaptations
        if (control is Button button)
        {
            button.MinHeight = profile.MinTouchTarget;
            button.MinWidth = profile.MinTouchTarget;
        }

        // Apply layout adaptations
        if (control is Panel panel)
        {
            panel.Classes.Set(""compact"", profile.UseCompactLayout);
        }

        // Recursively apply to child controls
        if (control is IPanel parentPanel)
        {
            foreach (var child in parentPanel.Children)
            {
                ApplyDeviceAdaptations(child, profile);
            }
        }
    }
}

public class DeviceProfile
{
    public double MinTouchTarget { get; set; }
    public double PreferredFontSize { get; set; }
    public int MaxColumns { get; set; }
    public bool UseCompactLayout { get; set; }
    public bool ShowSecondaryActions { get; set; }
}";
    }

    private static string GenerateUXPatternImplementation(UXConfiguration config)
    {
        return config.PatternType switch
        {
            "loading-states" => GenerateLoadingStates(config),
            "error-handling" => GenerateErrorHandling(config),
            "feedback-system" => GenerateFeedbackSystem(config),
            "progressive-disclosure" => GenerateProgressiveDisclosure(config),
            "micro-interactions" => GenerateMicroInteractionComponents(config),
            _ => GenerateGenericUXPattern(config)
        };
    }

    private static string GenerateLoadingStates(UXConfiguration config)
    {
        return @"<!-- Loading States Pattern -->
<UserControl xmlns=""https://github.com/avaloniaui"">
    <UserControl.Styles>
        <Style Selector=""Border.loading-container"">
            <Setter Property=""Background"" Value=""{StaticResource LoadingOverlayBrush}"" />
            <Setter Property=""Opacity"" Value=""0.95"" />
            <Setter Property=""IsVisible"" Value=""{Binding IsLoading}"" />
        </Style>
        
        <Style Selector=""ProgressBar.skeleton"">
            <Setter Property=""IsIndeterminate"" Value=""True"" />
            <Setter Property=""Height"" Value=""4"" />
            <Setter Property=""Background"" Value=""Transparent"" />
        </Style>
        
        <Style Selector=""ContentControl.shimmer"">
            <Setter Property=""Background"" Value=""{StaticResource ShimmerBrush}"" />
            <Setter Property=""Opacity"" Value=""0.7"" />
        </Style>
    </UserControl.Styles>
    
    <Grid>
        <!-- Main Content -->
        <ContentPresenter Content=""{Binding MainContent}"" 
                         IsVisible=""{Binding !IsLoading}"" />
        
        <!-- Loading Overlay -->
        <Border Classes=""loading-container"">
            <StackPanel HorizontalAlignment=""Center"" 
                       VerticalAlignment=""Center""
                       Spacing=""16"">
                <!-- Spinner -->
                <Border Width=""48"" Height=""48""
                       CornerRadius=""24""
                       Background=""{StaticResource AccentBrush}"">
                    <Border.RenderTransform>
                        <RotateTransform />
                    </Border.RenderTransform>
                    <Border.Styles>
                        <Style Selector=""Border"">
                            <Style.Animations>
                                <Animation Duration=""0:0:1"" IterationCount=""INFINITE"">
                                    <KeyFrame Cue=""0%"">
                                        <Setter Property=""RenderTransform.Angle"" Value=""0"" />
                                    </KeyFrame>
                                    <KeyFrame Cue=""100%"">
                                        <Setter Property=""RenderTransform.Angle"" Value=""360"" />
                                    </KeyFrame>
                                </Animation>
                            </Style.Animations>
                        </Style>
                    </Border.Styles>
                </Border>
                
                <!-- Loading Text -->
                <TextBlock Text=""{Binding LoadingMessage}"" 
                          HorizontalAlignment=""Center""
                          Classes=""body1"" />
                
                <!-- Progress Bar -->
                <ProgressBar Value=""{Binding LoadingProgress}""
                            Width=""200""
                            Height=""8""
                            IsVisible=""{Binding ShowProgress}"" />
            </StackPanel>
        </Border>
        
        <!-- Skeleton Loading (Alternative) -->
        <StackPanel IsVisible=""{Binding UseSkeletonLoading}"" 
                   Spacing=""12"" Margin=""20"">
            <ContentControl Classes=""shimmer"" Height=""24"" />
            <ContentControl Classes=""shimmer"" Height=""16"" Width=""200"" />
            <ContentControl Classes=""shimmer"" Height=""16"" Width=""150"" />
            <ContentControl Classes=""shimmer"" Height=""100"" />
        </StackPanel>
    </Grid>
</UserControl>";
    }

    private static string GenerateErrorHandling(UXConfiguration config)
    {
        return @"<!-- Error Handling Pattern -->
<UserControl xmlns=""https://github.com/avaloniaui"">
    <Grid>
        <!-- Main Content -->
        <ContentPresenter Content=""{Binding MainContent}"" 
                         IsVisible=""{Binding !HasError}"" />
        
        <!-- Error States -->
        <StackPanel IsVisible=""{Binding HasError}""
                   HorizontalAlignment=""Center""
                   VerticalAlignment=""Center""
                   Spacing=""20"" Margin=""40"">
            
            <!-- Error Icon -->
            <Border Width=""64"" Height=""64""
                   Background=""{StaticResource ErrorBrush}""
                   CornerRadius=""32"">
                <TextBlock Text=""⚠️"" 
                          FontSize=""32""
                          HorizontalAlignment=""Center""
                          VerticalAlignment=""Center"" />
            </Border>
            
            <!-- Error Title -->
            <TextBlock Text=""{Binding ErrorTitle}"" 
                      Classes=""heading2""
                      HorizontalAlignment=""Center""
                      TextAlignment=""Center"" />
            
            <!-- Error Description -->
            <TextBlock Text=""{Binding ErrorDescription}"" 
                      Classes=""body1""
                      HorizontalAlignment=""Center""
                      TextAlignment=""Center""
                      TextWrapping=""Wrap""
                      MaxWidth=""400"" />
            
            <!-- Error Actions -->
            <StackPanel Orientation=""Horizontal"" 
                       HorizontalAlignment=""Center""
                       Spacing=""12"">
                <Button Content=""Try Again"" 
                       Command=""{Binding RetryCommand}""
                       Classes=""primary"" />
                <Button Content=""Report Issue"" 
                       Command=""{Binding ReportCommand}""
                       Classes=""secondary"" />
            </StackPanel>
            
            <!-- Technical Details (Expandable) -->
            <Expander Header=""Technical Details""
                     IsVisible=""{Binding ShowTechnicalDetails}"">
                <Border Background=""{StaticResource CodeBackgroundBrush}""
                       Padding=""16"" CornerRadius=""4"" Margin=""0,8,0,0"">
                    <SelectableTextBlock Text=""{Binding TechnicalDetails}""
                                       FontFamily=""Consolas""
                                       FontSize=""12""
                                       TextWrapping=""Wrap"" />
                </Border>
            </Expander>
        </StackPanel>
        
        <!-- Inline Error Banner -->
        <Border DockPanel.Dock=""Top""
               Background=""{StaticResource ErrorBackgroundBrush}""
               BorderBrush=""{StaticResource ErrorBrush}""
               BorderThickness=""0,0,0,1""
               IsVisible=""{Binding ShowInlineError}"">
            <Grid Margin=""16,12"" ColumnDefinitions=""Auto,*,Auto"">
                <TextBlock Grid.Column=""0"" 
                          Text=""⚠️"" 
                          VerticalAlignment=""Center""
                          Margin=""0,0,12,0"" />
                <TextBlock Grid.Column=""1"" 
                          Text=""{Binding InlineErrorMessage}""
                          VerticalAlignment=""Center""
                          TextWrapping=""Wrap"" />
                <Button Grid.Column=""2""
                       Content=""✕""
                       Command=""{Binding DismissErrorCommand}""
                       Classes=""icon dismiss"" />
            </Grid>
        </Border>
    </Grid>
</UserControl>";
    }

    private static string GenerateFeedbackSystem(UXConfiguration config)
    {
        return @"<!-- Feedback System Pattern -->
<UserControl xmlns=""https://github.com/avaloniaui"">
    <Grid>
        <!-- Main Content -->
        <ContentPresenter Content=""{Binding MainContent}"" />
        
        <!-- Toast Notifications -->
        <ItemsControl Items=""{Binding ToastNotifications}""
                     VerticalAlignment=""Top""
                     HorizontalAlignment=""Right""
                     Margin=""20"">
            <ItemsControl.ItemTemplate>
                <DataTemplate>
                    <Border Background=""{Binding BackgroundBrush}""
                           BorderBrush=""{Binding BorderBrush}""
                           BorderThickness=""1""
                           CornerRadius=""6""
                           Padding=""16,12""
                           Margin=""0,0,0,8""
                           MaxWidth=""400"">
                        <Border.Styles>
                            <Style Selector=""Border"">
                                <Style.Animations>
                                    <Animation Duration=""0:0:0.3"" FillMode=""Forward"">
                                        <KeyFrame Cue=""0%"">
                                            <Setter Property=""Opacity"" Value=""0"" />
                                            <Setter Property=""RenderTransform"" Value=""translateX(100px)"" />
                                        </KeyFrame>
                                        <KeyFrame Cue=""100%"">
                                            <Setter Property=""Opacity"" Value=""1"" />
                                            <Setter Property=""RenderTransform"" Value=""translateX(0px)"" />
                                        </KeyFrame>
                                    </Animation>
                                </Style.Animations>
                            </Style>
                        </Border.Styles>
                        
                        <Grid ColumnDefinitions=""Auto,*,Auto"">
                            <TextBlock Grid.Column=""0"" 
                                      Text=""{Binding Icon}""
                                      VerticalAlignment=""Center""
                                      Margin=""0,0,12,0"" />
                            <StackPanel Grid.Column=""1"" Spacing=""4"">
                                <TextBlock Text=""{Binding Title}""
                                          Classes=""subtitle""
                                          IsVisible=""{Binding HasTitle}"" />
                                <TextBlock Text=""{Binding Message}""
                                          TextWrapping=""Wrap"" />
                            </StackPanel>
                            <Button Grid.Column=""2""
                                   Content=""✕""
                                   Command=""{Binding DismissCommand}""
                                   Classes=""icon dismiss""
                                   IsVisible=""{Binding IsDismissible}"" />
                        </Grid>
                    </Border>
                </DataTemplate>
            </ItemsControl.ItemTemplate>
        </ItemsControl>
        
        <!-- Progress Feedback -->
        <Border Background=""{StaticResource OverlayBrush}""
               IsVisible=""{Binding ShowProgressFeedback}"">
            <StackPanel HorizontalAlignment=""Center""
                       VerticalAlignment=""Center""
                       Spacing=""16"">
                <ProgressBar Value=""{Binding ProgressValue}""
                            Width=""300""
                            Height=""8"" />
                <TextBlock Text=""{Binding ProgressMessage}""
                          HorizontalAlignment=""Center"" />
            </StackPanel>
        </Border>
        
        <!-- Status Bar -->
        <Border DockPanel.Dock=""Bottom""
               Background=""{StaticResource StatusBarBrush}""
               BorderBrush=""{StaticResource BorderBrush}""
               BorderThickness=""0,1,0,0""
               IsVisible=""{Binding ShowStatusBar}"">
            <Grid Margin=""16,8"" ColumnDefinitions=""*,Auto"">
                <TextBlock Grid.Column=""0"" 
                          Text=""{Binding StatusMessage}""
                          VerticalAlignment=""Center"" />
                <StackPanel Grid.Column=""1"" 
                           Orientation=""Horizontal""
                           Spacing=""8"">
                    <TextBlock Text=""{Binding ConnectedUsersCount}"" 
                              VerticalAlignment=""Center"" />
                    <Ellipse Width=""8"" Height=""8""
                            Fill=""{Binding ConnectionStatusBrush}"" />
                </StackPanel>
            </Grid>
        </Border>
    </Grid>
</UserControl>";
    }

    private static string GenerateProgressiveDisclosure(UXConfiguration config)
    {
        return @"<!-- Progressive Disclosure Pattern -->
<UserControl xmlns=""https://github.com/avaloniaui"">
    <ScrollViewer>
        <StackPanel Spacing=""16"" Margin=""20"">
            <!-- Primary Information -->
            <StackPanel Spacing=""12"">
                <TextBlock Text=""Essential Information"" Classes=""heading2"" />
                <TextBlock Text=""This is the most important content that users need to see immediately."" 
                          TextWrapping=""Wrap"" />
                <Button Content=""Primary Action"" Classes=""primary"" />
            </StackPanel>
            
            <!-- Secondary Information (Expandable) -->
            <Expander Header=""Show More Details"" 
                     Classes=""progressive-disclosure"">
                <StackPanel Spacing=""12"" Margin=""0,12,0,0"">
                    <TextBlock Text=""Additional Details"" Classes=""heading3"" />
                    <TextBlock Text=""This secondary information is available when users need it, but doesn't clutter the initial view."" 
                              TextWrapping=""Wrap"" />
                    
                    <!-- Nested Progressive Disclosure -->
                    <Expander Header=""Advanced Options"">
                        <StackPanel Spacing=""8"" Margin=""0,8,0,0"">
                            <CheckBox Content=""Enable advanced feature"" />
                            <CheckBox Content=""Show debug information"" />
                            <StackPanel Orientation=""Horizontal"" Spacing=""8"">
                                <TextBlock Text=""Timeout:"" VerticalAlignment=""Center"" />
                                <NumericUpDown Value=""30"" Minimum=""1"" Maximum=""300"" />
                                <TextBlock Text=""seconds"" VerticalAlignment=""Center"" />
                            </StackPanel>
                        </StackPanel>
                    </Expander>
                </StackPanel>
            </Expander>
            
            <!-- Step-by-Step Disclosure -->
            <StackPanel Spacing=""16"">
                <TextBlock Text=""Step-by-Step Process"" Classes=""heading2"" />
                
                <!-- Step 1 -->
                <Border Classes=""step-container active"">
                    <StackPanel Spacing=""8"">
                        <Grid ColumnDefinitions=""Auto,*"">
                            <Border Grid.Column=""0"" Classes=""step-number"">
                                <TextBlock Text=""1"" />
                            </Border>
                            <TextBlock Grid.Column=""1"" 
                                      Text=""Basic Information"" 
                                      Classes=""step-title""
                                      VerticalAlignment=""Center"" />
                        </Grid>
                        <StackPanel Spacing=""8"" IsVisible=""{Binding Step1Expanded}"">
                            <TextBox Watermark=""Enter your name"" />
                            <TextBox Watermark=""Enter your email"" />
                            <Button Content=""Continue to Step 2"" 
                                   Command=""{Binding ExpandStep2Command}""
                                   Classes=""primary"" />
                        </StackPanel>
                    </StackPanel>
                </Border>
                
                <!-- Step 2 -->
                <Border Classes=""step-container"" 
                       IsEnabled=""{Binding Step2Enabled}"">
                    <StackPanel Spacing=""8"">
                        <Grid ColumnDefinitions=""Auto,*"">
                            <Border Grid.Column=""0"" Classes=""step-number"">
                                <TextBlock Text=""2"" />
                            </Border>
                            <TextBlock Grid.Column=""1"" 
                                      Text=""Preferences"" 
                                      Classes=""step-title""
                                      VerticalAlignment=""Center"" />
                        </Grid>
                        <StackPanel Spacing=""8"" IsVisible=""{Binding Step2Expanded}"">
                            <CheckBox Content=""Receive notifications"" />
                            <CheckBox Content=""Enable dark mode"" />
                            <Button Content=""Complete Setup"" 
                                   Command=""{Binding CompleteCommand}""
                                   Classes=""primary"" />
                        </StackPanel>
                    </StackPanel>
                </Border>
            </StackPanel>
        </StackPanel>
    </ScrollViewer>
</UserControl>";
    }

    private static string GenerateMicroInteractionComponents(UXConfiguration config)
    {
        return @"<!-- Micro-Interactions Pattern -->
<UserControl xmlns=""https://github.com/avaloniaui"">
    <UserControl.Styles>
        <!-- Hover Animations -->
        <Style Selector=""Button.interactive:pointerover"">
            <Style.Animations>
                <Animation Duration=""0:0:0.2"">
                    <KeyFrame Cue=""100%"">
                        <Setter Property=""RenderTransform"" Value=""scale(1.05)"" />
                        <Setter Property=""BoxShadow"" Value=""0 4 12 rgba(0,0,0,0.15)"" />
                    </KeyFrame>
                </Animation>
            </Style.Animations>
        </Style>
        
        <!-- Click Feedback -->
        <Style Selector=""Button.interactive:pressed"">
            <Style.Animations>
                <Animation Duration=""0:0:0.1"">
                    <KeyFrame Cue=""100%"">
                        <Setter Property=""RenderTransform"" Value=""scale(0.95)"" />
                    </KeyFrame>
                </Animation>
            </Style.Animations>
        </Style>
        
        <!-- Focus Animations -->
        <Style Selector=""TextBox:focus"">
            <Style.Animations>
                <Animation Duration=""0:0:0.2"">
                    <KeyFrame Cue=""100%"">
                        <Setter Property=""BorderBrush"" Value=""{StaticResource AccentBrush}"" />
                        <Setter Property=""BorderThickness"" Value=""2"" />
                    </KeyFrame>
                </Animation>
            </Style.Animations>
        </Style>
        
        <!-- Loading Pulse -->
        <Style Selector=""Border.pulse"">
            <Style.Animations>
                <Animation Duration=""0:0:1.5"" IterationCount=""INFINITE"">
                    <KeyFrame Cue=""0%"">
                        <Setter Property=""Opacity"" Value=""1"" />
                    </KeyFrame>
                    <KeyFrame Cue=""50%"">
                        <Setter Property=""Opacity"" Value=""0.5"" />
                    </KeyFrame>
                    <KeyFrame Cue=""100%"">
                        <Setter Property=""Opacity"" Value=""1"" />
                    </KeyFrame>
                </Animation>
            </Style.Animations>
        </Style>
    </UserControl.Styles>
    
    <StackPanel Spacing=""20"" Margin=""20"">
        <!-- Interactive Buttons -->
        <StackPanel Spacing=""12"">
            <TextBlock Text=""Interactive Buttons"" Classes=""heading2"" />
            <StackPanel Orientation=""Horizontal"" Spacing=""12"">
                <Button Content=""Hover Me"" Classes=""interactive primary"" />
                <Button Content=""Click Me"" Classes=""interactive secondary"" />
                <Button Content=""Loading..."" Classes=""pulse"" IsEnabled=""False"" />
            </StackPanel>
        </StackPanel>
        
        <!-- Form Interactions -->
        <StackPanel Spacing=""12"">
            <TextBlock Text=""Form Interactions"" Classes=""heading2"" />
            <TextBox Watermark=""Focus for border animation"" />
            <ToggleSwitch Content=""Toggle with smooth transition"" />
            <Slider Value=""50"" ToolTip.Tip=""Drag for instant feedback"" />
        </StackPanel>
        
        <!-- Feedback Cards -->
        <StackPanel Spacing=""12"">
            <TextBlock Text=""Card Interactions"" Classes=""heading2"" />
            <Border Classes=""card interactive""
                   Background=""{StaticResource CardBrush}""
                   Padding=""16"" CornerRadius=""8"">
                <StackPanel Spacing=""8"">
                    <TextBlock Text=""Interactive Card"" Classes=""subtitle"" />
                    <TextBlock Text=""Hover over this card to see micro-interactions in action."" 
                              TextWrapping=""Wrap"" />
                </StackPanel>
            </Border>
        </StackPanel>
        
        <!-- Progress Feedback -->
        <StackPanel Spacing=""12"">
            <TextBlock Text=""Progress Feedback"" Classes=""heading2"" />
            <ProgressBar Value=""{Binding ProgressValue}"" 
                        Height=""8"" 
                        CornerRadius=""4"">
                <ProgressBar.Styles>
                    <Style Selector=""ProgressBar"">
                        <Style.Animations>
                            <Animation Duration=""0:0:0.3"">
                                <KeyFrame Cue=""100%"">
                                    <Setter Property=""Value"" Value=""{Binding ProgressValue}"" />
                                </KeyFrame>
                            </Animation>
                        </Style.Animations>
                    </Style>
                </ProgressBar.Styles>
            </ProgressBar>
        </StackPanel>
    </StackPanel>
</UserControl>";
    }

    private static string GenerateGenericUXPattern(UXConfiguration config)
    {
        return @"<!-- Generic UX Pattern -->
<UserControl xmlns=""https://github.com/avaloniaui"">
    <StackPanel Spacing=""16"" Margin=""20"">
        <TextBlock Text=""UX Pattern Component"" Classes=""heading1"" />
        <TextBlock Text=""This is a generic UX pattern that can be customized for specific use cases."" 
                  TextWrapping=""Wrap"" />
        <Button Content=""Interact"" Classes=""primary"" />
    </StackPanel>
</UserControl>";
    }

    private static string GenerateUserGuidanceSystem(UXConfiguration config)
    {
        return @"// User Guidance Service
public class UserGuidanceService
{
    private readonly Dictionary<string, GuidanceStep> _guidanceSteps = new();
    private int _currentStepIndex = 0;
    private List<string> _currentFlow = new();

    public event Action<GuidanceStep>? GuidanceStepChanged;

    public void StartGuidedFlow(string flowName, List<GuidanceStep> steps)
    {
        _currentFlow = steps.Select(s => s.Id).ToList();
        _currentStepIndex = 0;
        
        foreach (var step in steps)
        {
            _guidanceSteps[step.Id] = step;
        }
        
        ShowCurrentStep();
    }

    public void NextStep()
    {
        if (_currentStepIndex < _currentFlow.Count - 1)
        {
            _currentStepIndex++;
            ShowCurrentStep();
        }
    }

    public void PreviousStep()
    {
        if (_currentStepIndex > 0)
        {
            _currentStepIndex--;
            ShowCurrentStep();
        }
    }

    public void EndGuidance()
    {
        _currentFlow.Clear();
        _currentStepIndex = 0;
        GuidanceStepChanged?.Invoke(null);
    }

    private void ShowCurrentStep()
    {
        if (_currentStepIndex < _currentFlow.Count)
        {
            var stepId = _currentFlow[_currentStepIndex];
            if (_guidanceSteps.TryGetValue(stepId, out var step))
            {
                step.IsActive = true;
                GuidanceStepChanged?.Invoke(step);
            }
        }
    }
}

public class GuidanceStep
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TargetElement { get; set; } = string.Empty;
    public GuidancePosition Position { get; set; } = GuidancePosition.Bottom;
    public bool IsActive { get; set; }
    public Action? OnComplete { get; set; }
}

public enum GuidancePosition
{
    Top,
    Bottom,
    Left,
    Right,
    Center
}

// Usage Example:
public void StartOnboarding()
{
    var steps = new List<GuidanceStep>
    {
        new() { Id = ""welcome"", Title = ""Welcome!"", Description = ""Let's get you started."" },
        new() { Id = ""navigation"", Title = ""Navigation"", Description = ""Use this menu to navigate."", TargetElement = ""MainMenu"" },
        new() { Id = ""actions"", Title = ""Actions"", Description = ""Click here for main actions."", TargetElement = ""ActionButton"" }
    };
    
    _guidanceService.StartGuidedFlow(""onboarding"", steps);
}";
    }

    private static string GenerateMicroInteractions(UXConfiguration config)
    {
        return @"// Micro-Interactions Service
public class MicroInteractionsService
{
    public void AttachHoverEffect(Control control, TimeSpan duration = default)
    {
        if (duration == default) duration = TimeSpan.FromMilliseconds(200);
        
        control.PointerEnter += (s, e) => 
        {
            var animation = new Animation
            {
                Duration = duration,
                Children =
                {
                    new KeyFrame { Cue = Cue.Parse(""100%"") }
                        .Setters = { new Setter(Visual.OpacityProperty, 0.8) }
                }
            };
            animation.RunAsync(control);
        };
        
        control.PointerLeave += (s, e) => 
        {
            var animation = new Animation
            {
                Duration = duration,
                Children =
                {
                    new KeyFrame { Cue = Cue.Parse(""100%"") }
                        .Setters = { new Setter(Visual.OpacityProperty, 1.0) }
                }
            };
            animation.RunAsync(control);
        };
    }

    public void AttachClickFeedback(Button button)
    {
        button.Click += async (s, e) =>
        {
            // Scale down animation
            var scaleDown = new Animation
            {
                Duration = TimeSpan.FromMilliseconds(100),
                Children =
                {
                    new KeyFrame { Cue = Cue.Parse(""100%"") }
                        .Setters = { new Setter(Visual.RenderTransformProperty, new ScaleTransform(0.95, 0.95)) }
                }
            };
            
            await scaleDown.RunAsync(button);
            
            // Scale back up
            var scaleUp = new Animation
            {
                Duration = TimeSpan.FromMilliseconds(100),
                Children =
                {
                    new KeyFrame { Cue = Cue.Parse(""100%"") }
                        .Setters = { new Setter(Visual.RenderTransformProperty, new ScaleTransform(1.0, 1.0)) }
                }
            };
            
            await scaleUp.RunAsync(button);
        };
    }

    public void AttachLoadingState(Control control, bool isLoading)
    {
        if (isLoading)
        {
            var pulseAnimation = new Animation
            {
                Duration = TimeSpan.FromSeconds(1),
                IterationCount = IterationCount.Infinite,
                Children =
                {
                    new KeyFrame { Cue = Cue.Parse(""0%"") }
                        .Setters = { new Setter(Visual.OpacityProperty, 1.0) },
                    new KeyFrame { Cue = Cue.Parse(""50%"") }
                        .Setters = { new Setter(Visual.OpacityProperty, 0.5) },
                    new KeyFrame { Cue = Cue.Parse(""100%"") }
                        .Setters = { new Setter(Visual.OpacityProperty, 1.0) }
                }
            };
            
            pulseAnimation.RunAsync(control);
        }
        else
        {
            // Stop animation and reset opacity
            control.Opacity = 1.0;
        }
    }

    public void AttachSuccessFeedback(Control control, Action? onComplete = null)
    {
        var successAnimation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(600),
            Children =
            {
                new KeyFrame { Cue = Cue.Parse(""0%"") }
                    .Setters = { 
                        new Setter(Visual.RenderTransformProperty, new ScaleTransform(1.0, 1.0)),
                        new Setter(Border.BackgroundProperty, Brushes.Transparent)
                    },
                new KeyFrame { Cue = Cue.Parse(""30%"") }
                    .Setters = { 
                        new Setter(Visual.RenderTransformProperty, new ScaleTransform(1.1, 1.1)),
                        new Setter(Border.BackgroundProperty, Brushes.LightGreen)
                    },
                new KeyFrame { Cue = Cue.Parse(""100%"") }
                    .Setters = { 
                        new Setter(Visual.RenderTransformProperty, new ScaleTransform(1.0, 1.0)),
                        new Setter(Border.BackgroundProperty, Brushes.Transparent)
                    }
            }
        };
        
        successAnimation.RunAsync(control).ContinueWith(_ => onComplete?.Invoke());
    }
}";
    }

    private static string GenerateUsabilityGuidelines(UXConfiguration config)
    {
        return @"## Usability Guidelines

### Interaction Design
- **Immediate Feedback**: Every user action should have immediate visual feedback
- **Clear Affordances**: Interactive elements should look clickable/touchable
- **Consistent Behavior**: Similar interactions should behave the same way
- **Error Prevention**: Design to prevent errors before they occur

### Animation Timing
- **Subtle**: 100-200ms for micro-interactions
- **Moderate**: 200-500ms for transitions and state changes  
- **Bold**: 500ms+ for dramatic state changes and onboarding

### Touch Targets
- **Minimum Size**: 44x44px for touch interfaces
- **Spacing**: 8px minimum between interactive elements
- **Hit Areas**: Extend beyond visual boundaries for small controls

### Loading States
- **Under 100ms**: No feedback needed
- **100ms - 1s**: Show loading indicator
- **1s - 10s**: Show progress with percentage
- **Over 10s**: Show progress with time estimates

### Error Handling
- **Validation**: Real-time validation with clear messaging
- **Recovery**: Always provide a way to fix or undo errors
- **Context**: Explain what went wrong and how to fix it
- **Tone**: Use helpful, not blame-focused language";
    }

    private static string GenerateColorSystem(DesignSystemConfiguration config)
    {
        return config.ColorPalette switch
        {
            "monochrome" => GenerateMonochromeColors(),
            "vibrant" => GenerateVibrantColors(),
            "pastel" => GeneratePastelColors(),
            "dark" => GenerateDarkColors(),
            _ => GenerateModernColors()
        };
    }

    private static string GenerateMonochromeColors()
    {
        return @"<!-- Monochrome Color System -->
<ResourceDictionary xmlns=""https://github.com/avaloniaui"">
    <!-- Primary Colors -->
    <SolidColorBrush x:Key=""PrimaryBrush"" Color=""#000000"" />
    <SolidColorBrush x:Key=""PrimaryLightBrush"" Color=""#333333"" />
    <SolidColorBrush x:Key=""PrimaryDarkBrush"" Color=""#000000"" />
    
    <!-- Secondary Colors -->
    <SolidColorBrush x:Key=""SecondaryBrush"" Color=""#666666"" />
    <SolidColorBrush x:Key=""SecondaryLightBrush"" Color=""#999999"" />
    <SolidColorBrush x:Key=""SecondaryDarkBrush"" Color=""#333333"" />
    
    <!-- Neutral Colors -->
    <SolidColorBrush x:Key=""BackgroundBrush"" Color=""#FFFFFF"" />
    <SolidColorBrush x:Key=""SurfaceBrush"" Color=""#F8F8F8"" />
    <SolidColorBrush x:Key=""BorderBrush"" Color=""#E0E0E0"" />
    
    <!-- Text Colors -->
    <SolidColorBrush x:Key=""TextPrimaryBrush"" Color=""#000000"" />
    <SolidColorBrush x:Key=""TextSecondaryBrush"" Color=""#666666"" />
    <SolidColorBrush x:Key=""TextDisabledBrush"" Color=""#CCCCCC"" />
    
    <!-- Accent Colors -->
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#000000"" />
    <SolidColorBrush x:Key=""SuccessBrush"" Color=""#4A4A4A"" />
    <SolidColorBrush x:Key=""WarningBrush"" Color=""#666666"" />
    <SolidColorBrush x:Key=""ErrorBrush"" Color=""#2A2A2A"" />
</ResourceDictionary>";
    }

    private static string GenerateVibrantColors()
    {
        return @"<!-- Vibrant Color System -->
<ResourceDictionary xmlns=""https://github.com/avaloniaui"">
    <!-- Primary Colors -->
    <SolidColorBrush x:Key=""PrimaryBrush"" Color=""#FF6B35"" />
    <SolidColorBrush x:Key=""PrimaryLightBrush"" Color=""#FF8A65"" />
    <SolidColorBrush x:Key=""PrimaryDarkBrush"" Color=""#D84315"" />
    
    <!-- Secondary Colors -->
    <SolidColorBrush x:Key=""SecondaryBrush"" Color=""#2196F3"" />
    <SolidColorBrush x:Key=""SecondaryLightBrush"" Color=""#64B5F6"" />
    <SolidColorBrush x:Key=""SecondaryDarkBrush"" Color=""#1976D2"" />
    
    <!-- Neutral Colors -->
    <SolidColorBrush x:Key=""BackgroundBrush"" Color=""#FFFFFF"" />
    <SolidColorBrush x:Key=""SurfaceBrush"" Color=""#FAFAFA"" />
    <SolidColorBrush x:Key=""BorderBrush"" Color=""#E0E0E0"" />
    
    <!-- Accent Colors -->
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#9C27B0"" />
    <SolidColorBrush x:Key=""SuccessBrush"" Color=""#4CAF50"" />
    <SolidColorBrush x:Key=""WarningBrush"" Color=""#FF9800"" />
    <SolidColorBrush x:Key=""ErrorBrush"" Color=""#F44336"" />
</ResourceDictionary>";
    }

    private static string GeneratePastelColors()
    {
        return @"<!-- Pastel Color System -->
<ResourceDictionary xmlns=""https://github.com/avaloniaui"">
    <!-- Primary Colors -->
    <SolidColorBrush x:Key=""PrimaryBrush"" Color=""#E8A87C"" />
    <SolidColorBrush x:Key=""PrimaryLightBrush"" Color=""#F4C2A1"" />
    <SolidColorBrush x:Key=""PrimaryDarkBrush"" Color=""#D4956B"" />
    
    <!-- Secondary Colors -->
    <SolidColorBrush x:Key=""SecondaryBrush"" Color=""#B8E6B8"" />
    <SolidColorBrush x:Key=""SecondaryLightBrush"" Color=""#D4F1D4"" />
    <SolidColorBrush x:Key=""SecondaryDarkBrush"" Color=""#A3D9A3"" />
    
    <!-- Neutral Colors -->
    <SolidColorBrush x:Key=""BackgroundBrush"" Color=""#FEFEFE"" />
    <SolidColorBrush x:Key=""SurfaceBrush"" Color=""#F9F9F9"" />
    <SolidColorBrush x:Key=""BorderBrush"" Color=""#E8E8E8"" />
    
    <!-- Accent Colors -->
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#C8A2C8"" />
    <SolidColorBrush x:Key=""SuccessBrush"" Color=""#A8E6CF"" />
    <SolidColorBrush x:Key=""WarningBrush"" Color=""#FFD3A5"" />
    <SolidColorBrush x:Key=""ErrorBrush"" Color=""#FFAAA5"" />
</ResourceDictionary>";
    }

    private static string GenerateDarkColors()
    {
        return @"<!-- Dark Color System -->
<ResourceDictionary xmlns=""https://github.com/avaloniaui"">
    <!-- Primary Colors -->
    <SolidColorBrush x:Key=""PrimaryBrush"" Color=""#BB86FC"" />
    <SolidColorBrush x:Key=""PrimaryLightBrush"" Color=""#CBA6FC"" />
    <SolidColorBrush x:Key=""PrimaryDarkBrush"" Color=""#9965F4"" />
    
    <!-- Secondary Colors -->
    <SolidColorBrush x:Key=""SecondaryBrush"" Color=""#03DAC6"" />
    <SolidColorBrush x:Key=""SecondaryLightBrush"" Color=""#66FFF9"" />
    <SolidColorBrush x:Key=""SecondaryDarkBrush"" Color=""#00A693"" />
    
    <!-- Background Colors -->
    <SolidColorBrush x:Key=""BackgroundBrush"" Color=""#121212"" />
    <SolidColorBrush x:Key=""SurfaceBrush"" Color=""#1E1E1E"" />
    <SolidColorBrush x:Key=""BorderBrush"" Color=""#333333"" />
    
    <!-- Text Colors -->
    <SolidColorBrush x:Key=""TextPrimaryBrush"" Color=""#FFFFFF"" />
    <SolidColorBrush x:Key=""TextSecondaryBrush"" Color=""#B3B3B3"" />
    <SolidColorBrush x:Key=""TextDisabledBrush"" Color=""#666666"" />
    
    <!-- Accent Colors -->
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#BB86FC"" />
    <SolidColorBrush x:Key=""SuccessBrush"" Color=""#4CAF50"" />
    <SolidColorBrush x:Key=""WarningBrush"" Color=""#FF9800"" />
    <SolidColorBrush x:Key=""ErrorBrush"" Color=""#CF6679"" />
</ResourceDictionary>";
    }

    private static string GenerateModernColors()
    {
        return @"<!-- Modern Color System -->
<ResourceDictionary xmlns=""https://github.com/avaloniaui"">
    <!-- Primary Colors -->
    <SolidColorBrush x:Key=""PrimaryBrush"" Color=""#6366F1"" />
    <SolidColorBrush x:Key=""PrimaryLightBrush"" Color=""#8B5CF6"" />
    <SolidColorBrush x:Key=""PrimaryDarkBrush"" Color=""#4F46E5"" />
    
    <!-- Secondary Colors -->
    <SolidColorBrush x:Key=""SecondaryBrush"" Color=""#06B6D4"" />
    <SolidColorBrush x:Key=""SecondaryLightBrush"" Color=""#22D3EE"" />
    <SolidColorBrush x:Key=""SecondaryDarkBrush"" Color=""#0891B2"" />
    
    <!-- Neutral Colors -->
    <SolidColorBrush x:Key=""BackgroundBrush"" Color=""#FFFFFF"" />
    <SolidColorBrush x:Key=""SurfaceBrush"" Color=""#F8FAFC"" />
    <SolidColorBrush x:Key=""BorderBrush"" Color=""#E2E8F0"" />
    
    <!-- Text Colors -->
    <SolidColorBrush x:Key=""TextPrimaryBrush"" Color=""#1E293B"" />
    <SolidColorBrush x:Key=""TextSecondaryBrush"" Color=""#64748B"" />
    <SolidColorBrush x:Key=""TextDisabledBrush"" Color=""#CBD5E1"" />
    
    <!-- Accent Colors -->
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#8B5CF6"" />
    <SolidColorBrush x:Key=""SuccessBrush"" Color=""#10B981"" />
    <SolidColorBrush x:Key=""WarningBrush"" Color=""#F59E0B"" />
    <SolidColorBrush x:Key=""ErrorBrush"" Color=""#EF4444"" />
</ResourceDictionary>";
    }

    private static string GenerateTypographySystem(DesignSystemConfiguration config)
    {
        var baseSize = config.TypographyScale switch
        {
            "compact" => 14,
            "large" => 18,
            _ => 16
        };

        return $@"<!-- Typography System -->
<ResourceDictionary xmlns=""https://github.com/avaloniaui"">
    <!-- Font Families -->
    <FontFamily x:Key=""PrimaryFont"">Inter, Segoe UI, Arial, sans-serif</FontFamily>
    <FontFamily x:Key=""HeadingFont"">Inter, Segoe UI, Arial, sans-serif</FontFamily>
    <FontFamily x:Key=""MonospaceFont"">Consolas, Monaco, monospace</FontFamily>
    
    <!-- Typography Styles -->
    <Style Selector=""TextBlock.heading1"" x:Key=""Heading1"">
        <Setter Property=""FontFamily"" Value=""{{StaticResource HeadingFont}}"" />
        <Setter Property=""FontSize"" Value=""{baseSize * 2.5}"" />
        <Setter Property=""FontWeight"" Value=""Bold"" />
        <Setter Property=""LineHeight"" Value=""{baseSize * 3}"" />
        <Setter Property=""Foreground"" Value=""{{StaticResource TextPrimaryBrush}}"" />
    </Style>
    
    <Style Selector=""TextBlock.heading2"" x:Key=""Heading2"">
        <Setter Property=""FontFamily"" Value=""{{StaticResource HeadingFont}}"" />
        <Setter Property=""FontSize"" Value=""{baseSize * 2}"" />
        <Setter Property=""FontWeight"" Value=""SemiBold"" />
        <Setter Property=""LineHeight"" Value=""{baseSize * 2.5}"" />
        <Setter Property=""Foreground"" Value=""{{StaticResource TextPrimaryBrush}}"" />
    </Style>
    
    <Style Selector=""TextBlock.heading3"" x:Key=""Heading3"">
        <Setter Property=""FontFamily"" Value=""{{StaticResource HeadingFont}}"" />
        <Setter Property=""FontSize"" Value=""{baseSize * 1.5}"" />
        <Setter Property=""FontWeight"" Value=""SemiBold"" />
        <Setter Property=""LineHeight"" Value=""{baseSize * 2}"" />
        <Setter Property=""Foreground"" Value=""{{StaticResource TextPrimaryBrush}}"" />
    </Style>
    
    <Style Selector=""TextBlock.subtitle"" x:Key=""Subtitle"">
        <Setter Property=""FontFamily"" Value=""{{StaticResource PrimaryFont}}"" />
        <Setter Property=""FontSize"" Value=""{baseSize * 1.25}"" />
        <Setter Property=""FontWeight"" Value=""Medium"" />
        <Setter Property=""LineHeight"" Value=""{baseSize * 1.75}"" />
        <Setter Property=""Foreground"" Value=""{{StaticResource TextPrimaryBrush}}"" />
    </Style>
    
    <Style Selector=""TextBlock.body1"" x:Key=""Body1"">
        <Setter Property=""FontFamily"" Value=""{{StaticResource PrimaryFont}}"" />
        <Setter Property=""FontSize"" Value=""{baseSize}"" />
        <Setter Property=""FontWeight"" Value=""Normal"" />
        <Setter Property=""LineHeight"" Value=""{baseSize * 1.5}"" />
        <Setter Property=""Foreground"" Value=""{{StaticResource TextPrimaryBrush}}"" />
    </Style>
    
    <Style Selector=""TextBlock.body2"" x:Key=""Body2"">
        <Setter Property=""FontFamily"" Value=""{{StaticResource PrimaryFont}}"" />
        <Setter Property=""FontSize"" Value=""{baseSize * 0.875}"" />
        <Setter Property=""FontWeight"" Value=""Normal"" />
        <Setter Property=""LineHeight"" Value=""{baseSize * 1.25}"" />
        <Setter Property=""Foreground"" Value=""{{StaticResource TextSecondaryBrush}}"" />
    </Style>
    
    <Style Selector=""TextBlock.caption"" x:Key=""Caption"">
        <Setter Property=""FontFamily"" Value=""{{StaticResource PrimaryFont}}"" />
        <Setter Property=""FontSize"" Value=""{baseSize * 0.75}"" />
        <Setter Property=""FontWeight"" Value=""Normal"" />
        <Setter Property=""LineHeight"" Value=""{baseSize}"" />
        <Setter Property=""Foreground"" Value=""{{StaticResource TextSecondaryBrush}}"" />
    </Style>
    
    <Style Selector=""TextBlock.code"" x:Key=""Code"">
        <Setter Property=""FontFamily"" Value=""{{StaticResource MonospaceFont}}"" />
        <Setter Property=""FontSize"" Value=""{baseSize * 0.875}"" />
        <Setter Property=""FontWeight"" Value=""Normal"" />
        <Setter Property=""Background"" Value=""{{StaticResource SurfaceBrush}}"" />
        <Setter Property=""Padding"" Value=""4,2"" />
    </Style>
</ResourceDictionary>";
    }

    private static string GenerateSpacingSystem(DesignSystemConfiguration config)
    {
        return @"<!-- Spacing System -->
<ResourceDictionary xmlns=""https://github.com/avaloniaui"">
    <!-- Base Spacing Unit (8px) -->
    <x:Double x:Key=""SpacingUnit"">8</x:Double>
    
    <!-- Spacing Scale -->
    <Thickness x:Key=""Spacing0"">0</Thickness>
    <Thickness x:Key=""Spacing1"">4</Thickness>
    <Thickness x:Key=""Spacing2"">8</Thickness>
    <Thickness x:Key=""Spacing3"">12</Thickness>
    <Thickness x:Key=""Spacing4"">16</Thickness>
    <Thickness x:Key=""Spacing5"">20</Thickness>
    <Thickness x:Key=""Spacing6"">24</Thickness>
    <Thickness x:Key=""Spacing8"">32</Thickness>
    <Thickness x:Key=""Spacing10"">40</Thickness>
    <Thickness x:Key=""Spacing12"">48</Thickness>
    <Thickness x:Key=""Spacing16"">64</Thickness>
    <Thickness x:Key=""Spacing20"">80</Thickness>
    
    <!-- Border Radius -->
    <CornerRadius x:Key=""RadiusNone"">0</CornerRadius>
    <CornerRadius x:Key=""RadiusSmall"">4</CornerRadius>
    <CornerRadius x:Key=""RadiusMedium"">8</CornerRadius>
    <CornerRadius x:Key=""RadiusLarge"">12</CornerRadius>
    <CornerRadius x:Key=""RadiusXLarge"">16</CornerRadius>
    <CornerRadius x:Key=""RadiusFull"">9999</CornerRadius>
    
    <!-- Shadows -->
    <BoxShadow x:Key=""ShadowSmall"">0 1 3 rgba(0,0,0,0.12)</BoxShadow>
    <BoxShadow x:Key=""ShadowMedium"">0 4 6 rgba(0,0,0,0.1)</BoxShadow>
    <BoxShadow x:Key=""ShadowLarge"">0 10 15 rgba(0,0,0,0.1)</BoxShadow>
    <BoxShadow x:Key=""ShadowXLarge"">0 20 25 rgba(0,0,0,0.15)</BoxShadow>
</ResourceDictionary>";
    }

    private static string GenerateComponentLibrary(DesignSystemConfiguration config)
    {
        return @"<!-- Component Library -->
<ResourceDictionary xmlns=""https://github.com/avaloniaui"">
    <!-- Button Styles -->
    <Style Selector=""Button.primary"" x:Key=""PrimaryButton"">
        <Setter Property=""Background"" Value=""{StaticResource PrimaryBrush}"" />
        <Setter Property=""Foreground"" Value=""White"" />
        <Setter Property=""Padding"" Value=""{StaticResource Spacing4}"" />
        <Setter Property=""CornerRadius"" Value=""{StaticResource RadiusMedium}"" />
        <Setter Property=""FontWeight"" Value=""Medium"" />
        <Setter Property=""BorderThickness"" Value=""0"" />
        <Setter Property=""MinHeight"" Value=""44"" />
    </Style>
    
    <Style Selector=""Button.secondary"" x:Key=""SecondaryButton"">
        <Setter Property=""Background"" Value=""Transparent"" />
        <Setter Property=""Foreground"" Value=""{StaticResource PrimaryBrush}"" />
        <Setter Property=""BorderBrush"" Value=""{StaticResource PrimaryBrush}"" />
        <Setter Property=""BorderThickness"" Value=""1"" />
        <Setter Property=""Padding"" Value=""{StaticResource Spacing4}"" />
        <Setter Property=""CornerRadius"" Value=""{StaticResource RadiusMedium}"" />
        <Setter Property=""FontWeight"" Value=""Medium"" />
        <Setter Property=""MinHeight"" Value=""44"" />
    </Style>
    
    <!-- Card Style -->
    <Style Selector=""Border.card"" x:Key=""Card"">
        <Setter Property=""Background"" Value=""{StaticResource SurfaceBrush}"" />
        <Setter Property=""BorderBrush"" Value=""{StaticResource BorderBrush}"" />
        <Setter Property=""BorderThickness"" Value=""1"" />
        <Setter Property=""CornerRadius"" Value=""{StaticResource RadiusMedium}"" />
        <Setter Property=""Padding"" Value=""{StaticResource Spacing6}"" />
        <Setter Property=""BoxShadow"" Value=""{StaticResource ShadowSmall}"" />
    </Style>
    
    <!-- Form Input Styles -->
    <Style Selector=""TextBox.form-input"" x:Key=""FormInput"">
        <Setter Property=""Padding"" Value=""{StaticResource Spacing3}"" />
        <Setter Property=""CornerRadius"" Value=""{StaticResource RadiusSmall}"" />
        <Setter Property=""BorderThickness"" Value=""1"" />
        <Setter Property=""BorderBrush"" Value=""{StaticResource BorderBrush}"" />
        <Setter Property=""MinHeight"" Value=""44"" />
    </Style>
    
    <Style Selector=""TextBlock.form-label"" x:Key=""FormLabel"">
        <Setter Property=""FontWeight"" Value=""Medium"" />
        <Setter Property=""Foreground"" Value=""{StaticResource TextPrimaryBrush}"" />
        <Setter Property=""Margin"" Value=""0,0,0,4"" />
    </Style>
</ResourceDictionary>";
    }
}