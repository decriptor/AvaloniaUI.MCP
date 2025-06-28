using System.ComponentModel;

using AvaloniaUI.MCP.Services;

using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Tools;

[McpServerToolType]
public static class AccessibilityTool
{
    static readonly string[] Value =
                [
                    "- **Contrast Ratio**: Minimum 4.5:1 for normal text, 3:1 for large text",
                    "- **Keyboard Navigation**: All interactive elements must be keyboard accessible",
                    "- **Screen Reader**: Proper ARIA labels and roles for assistive technology",
                    "- **Focus Management**: Clear visual focus indicators and logical tab order"
                ];

    [McpServerTool, Description("Generates WCAG compliant accessible UI components with proper ARIA labels and keyboard support")]
    public static string GenerateAccessibleComponent(
        [Description("Component type: 'form', 'navigation', 'data-table', 'modal', 'notification'")] string componentType,
        [Description("WCAG compliance level: 'AA', 'AAA'")] string wcagLevel = "AA",
        [Description("Include keyboard navigation: 'true' or 'false'")] string includeKeyboardNav = "true",
        [Description("Include screen reader support: 'true' or 'false'")] string includeScreenReader = "true")
    {
        return ErrorHandlingService.SafeExecute(nameof(GenerateAccessibleComponent), () =>
        {
            var config = new AccessibilityConfiguration
            {
                ComponentType = componentType.ToLowerInvariant(),
                WcagLevel = wcagLevel.ToUpperInvariant(),
                IncludeKeyboardNavigation = bool.Parse(includeKeyboardNav),
                IncludeScreenReaderSupport = bool.Parse(includeScreenReader)
            };

            string componentXaml = GenerateAccessibleComponentXaml(config);
            string accessibilityHelpers = GenerateAccessibilityHelpers(config);
            string keyboardHandler = config.IncludeKeyboardNavigation ? GenerateKeyboardNavigationCode(config) : "";
            string testingChecklist = GenerateAccessibilityTestingChecklist(config);

            return MarkdownOutputBuilder
                .Create($"Accessible Component: {componentType}")
                .AddConfiguration(
                    ("Component Type", config.ComponentType),
                    ("WCAG Level", config.WcagLevel),
                    ("Keyboard Navigation", config.IncludeKeyboardNavigation),
                    ("Screen Reader Support", config.IncludeScreenReaderSupport))
                .AddCodeSection("Accessible Component XAML", "xml", componentXaml)
                .AddCodeSection("Accessibility Helper Classes", "csharp", accessibilityHelpers)
                .AddIf(config.IncludeKeyboardNavigation, builder =>
                    builder.AddCodeSection("Keyboard Navigation Handler", "csharp", keyboardHandler))
                .AddSection("Accessibility Testing Checklist", testingChecklist)
                .AddSection($"WCAG {config.WcagLevel} Compliance Notes", string.Join("\n", Value))
                .Build();
        });
    }

    sealed class AccessibilityConfiguration
    {
        public string ComponentType { get; set; } = "";
        public string WcagLevel { get; set; } = "";
        public bool IncludeKeyboardNavigation { get; set; }
        public bool IncludeScreenReaderSupport { get; set; }
    }

    static string GenerateAccessibleComponentXaml(AccessibilityConfiguration config)
    {
        return config.ComponentType switch
        {
            "form" => GenerateAccessibleForm(config),
            "navigation" => GenerateAccessibleNavigation(config),
            "data-table" => GenerateAccessibleDataTable(config),
            "modal" => GenerateAccessibleModal(config),
            "notification" => GenerateAccessibleNotification(config),
            _ => GenerateGenericAccessibleComponent(config)
        };
    }

    static string GenerateAccessibleForm(AccessibilityConfiguration config)
    {
        return @"<UserControl xmlns=""https://github.com/avaloniaui""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             AutomationProperties.Name=""User Registration Form""
             AutomationProperties.Description=""Complete this form to create your account"">

    <ScrollViewer>
        <StackPanel Margin=""20"" Spacing=""16"">
            <!-- Form Header -->
            <TextBlock Text=""Create Account""
                       Classes=""heading1""
                       AutomationProperties.HeadingLevel=""1""
                       AutomationProperties.Name=""Create Account Form"" />

            <!-- Required Fields Notice -->
            <TextBlock Text=""Fields marked with * are required""
                       Classes=""form-notice""
                       AutomationProperties.LiveSetting=""Polite"" />

            <!-- First Name Field -->
            <StackPanel Spacing=""8"">
                <TextBlock Text=""First Name *""
                          Classes=""form-label""
                          Target=""{Binding ElementName=FirstNameInput}"" />
                <TextBox x:Name=""FirstNameInput""
                        Watermark=""Enter your first name""
                        AutomationProperties.Name=""First Name""
                        AutomationProperties.IsRequiredForForm=""True""
                        AutomationProperties.HelpText=""Your given name as it appears on official documents""
                        Classes=""form-input"" />
                <TextBlock x:Name=""FirstNameError""
                          Text=""First name is required""
                          IsVisible=""False""
                          Classes=""error-message""
                          AutomationProperties.LiveSetting=""Assertive"" />
            </StackPanel>

            <!-- Email Field -->
            <StackPanel Spacing=""8"">
                <TextBlock Text=""Email Address *""
                          Classes=""form-label""
                          Target=""{Binding ElementName=EmailInput}"" />
                <TextBox x:Name=""EmailInput""
                        Watermark=""Enter your email address""
                        AutomationProperties.Name=""Email Address""
                        AutomationProperties.IsRequiredForForm=""True""
                        AutomationProperties.HelpText=""We'll use this to send you account information""
                        Classes=""form-input"" />
                <TextBlock x:Name=""EmailError""
                          Text=""Please enter a valid email address""
                          IsVisible=""False""
                          Classes=""error-message""
                          AutomationProperties.LiveSetting=""Assertive"" />
            </StackPanel>

            <!-- Password Field -->
            <StackPanel Spacing=""8"">
                <TextBlock Text=""Password *""
                          Classes=""form-label""
                          Target=""{Binding ElementName=PasswordInput}"" />
                <TextBox x:Name=""PasswordInput""
                        PasswordChar=""•""
                        Watermark=""Create a strong password""
                        AutomationProperties.Name=""Password""
                        AutomationProperties.IsRequiredForForm=""True""
                        AutomationProperties.HelpText=""Password must be at least 8 characters with uppercase, lowercase, and numbers""
                        Classes=""form-input"" />
                <TextBlock Text=""Password must be at least 8 characters with uppercase, lowercase, and numbers""
                          Classes=""form-help""
                          AutomationProperties.LiveSetting=""Polite"" />
                <TextBlock x:Name=""PasswordError""
                          IsVisible=""False""
                          Classes=""error-message""
                          AutomationProperties.LiveSetting=""Assertive"" />
            </StackPanel>

            <!-- Terms Checkbox -->
            <CheckBox x:Name=""TermsCheckbox""
                     Content=""I agree to the Terms of Service and Privacy Policy""
                     AutomationProperties.Name=""Accept Terms and Conditions""
                     AutomationProperties.HelpText=""You must accept the terms to create an account""
                     Classes=""form-checkbox"" />

            <!-- Form Actions -->
            <StackPanel Orientation=""Horizontal""
                       HorizontalAlignment=""Right""
                       Spacing=""12""
                       Margin=""0,24,0,0"">
                <Button Content=""Cancel""
                       AutomationProperties.Name=""Cancel Registration""
                       AutomationProperties.HelpText=""Cancel account creation and return to login""
                       Classes=""secondary""
                       Command=""{Binding CancelCommand}"" />
                <Button Content=""Create Account""
                       AutomationProperties.Name=""Create Account""
                       AutomationProperties.HelpText=""Submit form to create your new account""
                       Classes=""primary""
                       IsDefault=""True""
                       Command=""{Binding CreateAccountCommand}"" />
            </StackPanel>

            <!-- Success/Error Messages -->
            <Border x:Name=""MessageContainer""
                   IsVisible=""False""
                   Classes=""message-container""
                   AutomationProperties.LiveSetting=""Assertive"">
                <TextBlock x:Name=""MessageText""
                          Classes=""message-text""
                          TextWrapping=""Wrap"" />
            </Border>
        </StackPanel>
    </ScrollViewer>
</UserControl>";
    }

    static string GenerateAccessibleNavigation(AccessibilityConfiguration config)
    {
        return @"<UserControl xmlns=""https://github.com/avaloniaui""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             AutomationProperties.Name=""Main Navigation""
             AutomationProperties.Description=""Primary navigation menu for the application"">

    <DockPanel>
        <!-- Navigation Header -->
        <Border DockPanel.Dock=""Top""
               Background=""{StaticResource NavigationBrush}""
               Padding=""16,12""
               AutomationProperties.LandmarkType=""Banner"">
            <Grid ColumnDefinitions=""*,Auto"">
                <TextBlock Grid.Column=""0""
                          Text=""Application Name""
                          Classes=""logo""
                          AutomationProperties.Name=""Application Logo""
                          VerticalAlignment=""Center"" />

                <!-- Skip to Main Content Link -->
                <Button Grid.Column=""1""
                       Content=""Skip to Main Content""
                       Classes=""skip-link""
                       AutomationProperties.Name=""Skip to Main Content""
                       AutomationProperties.HelpText=""Bypass navigation and go directly to main content""
                       Command=""{Binding SkipToMainCommand}"" />
            </Grid>
        </Border>

        <!-- Main Navigation Menu -->
        <Border DockPanel.Dock=""Left""
               Background=""{StaticResource NavigationBrush}""
               Width=""250""
               AutomationProperties.LandmarkType=""Navigation"">

            <ScrollViewer>
                <StackPanel Spacing=""4"" Margin=""16"">
                    <!-- Navigation Menu -->
                    <Menu AutomationProperties.Name=""Main Menu"">
                        <MenuItem Header=""Dashboard""
                                 AutomationProperties.Name=""Dashboard""
                                 AutomationProperties.HelpText=""View application dashboard""
                                 Classes=""nav-item""
                                 Command=""{Binding NavigateDashboardCommand}"">
                            <MenuItem.Icon>
                                <TextBlock Text=""📊"" AutomationProperties.Name=""Dashboard Icon"" />
                            </MenuItem.Icon>
                        </MenuItem>

                        <MenuItem Header=""Projects""
                                 AutomationProperties.Name=""Projects""
                                 AutomationProperties.HelpText=""Manage your projects""
                                 Classes=""nav-item""
                                 Command=""{Binding NavigateProjectsCommand}"">
                            <MenuItem.Icon>
                                <TextBlock Text=""📁"" AutomationProperties.Name=""Projects Icon"" />
                            </MenuItem.Icon>

                            <!-- Sub-menu -->
                            <MenuItem Header=""All Projects""
                                     AutomationProperties.Name=""All Projects""
                                     Command=""{Binding ViewAllProjectsCommand}"" />
                            <MenuItem Header=""Create New""
                                     AutomationProperties.Name=""Create New Project""
                                     Command=""{Binding CreateProjectCommand}"" />
                        </MenuItem>

                        <MenuItem Header=""Settings""
                                 AutomationProperties.Name=""Settings""
                                 AutomationProperties.HelpText=""Configure application settings""
                                 Classes=""nav-item""
                                 Command=""{Binding NavigateSettingsCommand}"">
                            <MenuItem.Icon>
                                <TextBlock Text=""⚙️"" AutomationProperties.Name=""Settings Icon"" />
                            </MenuItem.Icon>
                        </MenuItem>
                    </Menu>

                    <Separator />

                    <!-- Secondary Navigation -->
                    <TextBlock Text=""Account""
                              Classes=""nav-section-header""
                              AutomationProperties.HeadingLevel=""2"" />

                    <Menu AutomationProperties.Name=""Account Menu"">
                        <MenuItem Header=""Profile""
                                 AutomationProperties.Name=""User Profile""
                                 AutomationProperties.HelpText=""View and edit your profile""
                                 Classes=""nav-item secondary""
                                 Command=""{Binding NavigateProfileCommand}"" />

                        <MenuItem Header=""Help""
                                 AutomationProperties.Name=""Help and Support""
                                 AutomationProperties.HelpText=""Get help and support""
                                 Classes=""nav-item secondary""
                                 Command=""{Binding NavigateHelpCommand}"" />

                        <MenuItem Header=""Logout""
                                 AutomationProperties.Name=""Logout""
                                 AutomationProperties.HelpText=""Sign out of your account""
                                 Classes=""nav-item secondary""
                                 Command=""{Binding LogoutCommand}"" />
                    </Menu>

                    <!-- Breadcrumb Navigation -->
                    <StackPanel Margin=""0,16,0,0""
                               AutomationProperties.Name=""Breadcrumb Navigation""
                               AutomationProperties.LandmarkType=""Navigation"">
                        <TextBlock Text=""You are here:""
                                  Classes=""breadcrumb-label""
                                  AutomationProperties.Name=""Current Location"" />
                        <ItemsControl Items=""{Binding BreadcrumbItems}"">
                            <ItemsControl.ItemsPanel>
                                <ItemsPanelTemplate>
                                    <StackPanel Orientation=""Horizontal"" />
                                </ItemsPanelTemplate>
                            </ItemsControl.ItemsPanel>
                            <ItemsControl.ItemTemplate>
                                <DataTemplate>
                                    <StackPanel Orientation=""Horizontal"">
                                        <Button Content=""{Binding Name}""
                                               AutomationProperties.Name=""{Binding AccessibleName}""
                                               Classes=""breadcrumb-item""
                                               Command=""{Binding NavigateCommand}"" />
                                        <TextBlock Text="">""
                                                  IsVisible=""{Binding !IsLast}""
                                                  Classes=""breadcrumb-separator""
                                                  AutomationProperties.Name=""Navigate to"" />
                                    </StackPanel>
                                </DataTemplate>
                            </ItemsControl.ItemTemplate>
                        </ItemsControl>
                    </StackPanel>
                </StackPanel>
            </ScrollViewer>
        </Border>

        <!-- Main Content Area -->
        <ContentPresenter DockPanel.Dock=""Right""
                         Content=""{Binding MainContent}""
                         AutomationProperties.LandmarkType=""Main""
                         AutomationProperties.Name=""Main Content Area"" />
    </DockPanel>
</UserControl>";
    }

    static string GenerateAccessibleDataTable(AccessibilityConfiguration config)
    {
        return @"<UserControl xmlns=""https://github.com/avaloniaui""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             AutomationProperties.Name=""Data Table""
             AutomationProperties.Description=""Sortable and filterable data table with accessibility support"">

    <StackPanel Spacing=""16"" Margin=""20"">
        <!-- Table Header -->
        <Grid ColumnDefinitions=""*,Auto"">
            <StackPanel Grid.Column=""0"" Spacing=""8"">
                <TextBlock Text=""User Data Table""
                          Classes=""heading2""
                          AutomationProperties.HeadingLevel=""2""
                          AutomationProperties.Name=""User Data Table"" />
                <TextBlock x:Name=""TableSummary""
                          Text=""Showing 25 of 100 users""
                          Classes=""table-summary""
                          AutomationProperties.LiveSetting=""Polite"" />
            </StackPanel>

            <!-- Table Actions -->
            <StackPanel Grid.Column=""1"" Orientation=""Horizontal"" Spacing=""8"">
                <TextBox x:Name=""SearchBox""
                        Watermark=""Search users...""
                        AutomationProperties.Name=""Search Users""
                        AutomationProperties.HelpText=""Type to filter table results""
                        Classes=""search-input"" />
                <Button Content=""Export""
                       AutomationProperties.Name=""Export Table Data""
                       AutomationProperties.HelpText=""Export table data to CSV file""
                       Classes=""secondary""
                       Command=""{Binding ExportCommand}"" />
            </StackPanel>
        </Grid>

        <!-- Accessible Data Grid -->
        <DataGrid x:Name=""UsersDataGrid""
                 Items=""{Binding Users}""
                 AutomationProperties.Name=""Users Table""
                 AutomationProperties.Description=""Table containing user information with sortable columns""
                 CanUserReorderColumns=""True""
                 CanUserResizeColumns=""True""
                 CanUserSortColumns=""True""
                 GridLinesVisibility=""Horizontal""
                 HeadersVisibility=""Column""
                 RowHeight=""44""
                 SelectionMode=""Extended""
                 AutoGenerateColumns=""False"">

            <!-- Column Definitions with Accessibility -->
            <DataGrid.Columns>
                <DataGridTextColumn Header=""Name""
                                  Binding=""{Binding FullName}""
                                  Width=""200""
                                  CanUserSort=""True""
                                  SortMemberPath=""FullName"">
                    <DataGridTextColumn.HeaderStyle>
                        <Style TargetType=""DataGridColumnHeader"">
                            <Setter Property=""AutomationProperties.Name"" Value=""Name Column Header"" />
                            <Setter Property=""AutomationProperties.HelpText"" Value=""Click to sort by name"" />
                        </Style>
                    </DataGridTextColumn.HeaderStyle>
                </DataGridTextColumn>

                <DataGridTextColumn Header=""Email""
                                  Binding=""{Binding Email}""
                                  Width=""250""
                                  CanUserSort=""True""
                                  SortMemberPath=""Email"">
                    <DataGridTextColumn.HeaderStyle>
                        <Style TargetType=""DataGridColumnHeader"">
                            <Setter Property=""AutomationProperties.Name"" Value=""Email Column Header"" />
                            <Setter Property=""AutomationProperties.HelpText"" Value=""Click to sort by email address"" />
                        </Style>
                    </DataGridTextColumn.HeaderStyle>
                </DataGridTextColumn>

                <DataGridTextColumn Header=""Role""
                                  Binding=""{Binding Role}""
                                  Width=""120""
                                  CanUserSort=""True""
                                  SortMemberPath=""Role"">
                    <DataGridTextColumn.HeaderStyle>
                        <Style TargetType=""DataGridColumnHeader"">
                            <Setter Property=""AutomationProperties.Name"" Value=""Role Column Header"" />
                            <Setter Property=""AutomationProperties.HelpText"" Value=""Click to sort by user role"" />
                        </Style>
                    </DataGridTextColumn.HeaderStyle>
                </DataGridTextColumn>

                <DataGridTextColumn Header=""Last Login""
                                  Binding=""{Binding LastLogin, StringFormat='{}{0:MM/dd/yyyy HH:mm}'}""
                                  Width=""150""
                                  CanUserSort=""True""
                                  SortMemberPath=""LastLogin"">
                    <DataGridTextColumn.HeaderStyle>
                        <Style TargetType=""DataGridColumnHeader"">
                            <Setter Property=""AutomationProperties.Name"" Value=""Last Login Column Header"" />
                            <Setter Property=""AutomationProperties.HelpText"" Value=""Click to sort by last login date"" />
                        </Style>
                    </DataGridTextColumn.HeaderStyle>
                </DataGridTextColumn>

                <DataGridTemplateColumn Header=""Actions"" Width=""120"">
                    <DataGridTemplateColumn.HeaderStyle>
                        <Style TargetType=""DataGridColumnHeader"">
                            <Setter Property=""AutomationProperties.Name"" Value=""Actions Column Header"" />
                            <Setter Property=""AutomationProperties.HelpText"" Value=""Available actions for each user"" />
                        </Style>
                    </DataGridTemplateColumn.HeaderStyle>
                    <DataGridTemplateColumn.CellTemplate>
                        <DataTemplate>
                            <StackPanel Orientation=""Horizontal"" Spacing=""4"">
                                <Button Content=""Edit""
                                       AutomationProperties.Name=""{Binding ., StringFormat='Edit user {0}'}""
                                       AutomationProperties.HelpText=""Edit this user's information""
                                       Classes=""small secondary""
                                       Command=""{Binding $parent[UserControl].DataContext.EditUserCommand}""
                                       CommandParameter=""{Binding}"" />
                                <Button Content=""Delete""
                                       AutomationProperties.Name=""{Binding ., StringFormat='Delete user {0}'}""
                                       AutomationProperties.HelpText=""Remove this user from the system""
                                       Classes=""small danger""
                                       Command=""{Binding $parent[UserControl].DataContext.DeleteUserCommand}""
                                       CommandParameter=""{Binding}"" />
                            </StackPanel>
                        </DataTemplate>
                    </DataGridTemplateColumn.CellTemplate>
                </DataGridTemplateColumn>
            </DataGrid.Columns>
        </DataGrid>

        <!-- Table Pagination -->
        <Grid ColumnDefinitions=""*,Auto"">
            <TextBlock Grid.Column=""0""
                      Text=""{Binding PaginationInfo}""
                      VerticalAlignment=""Center""
                      AutomationProperties.LiveSetting=""Polite"" />

            <StackPanel Grid.Column=""1"" Orientation=""Horizontal"" Spacing=""8"">
                <Button Content=""Previous""
                       AutomationProperties.Name=""Previous Page""
                       AutomationProperties.HelpText=""Go to previous page of results""
                       IsEnabled=""{Binding CanGoPrevious}""
                       Classes=""pagination""
                       Command=""{Binding PreviousPageCommand}"" />
                <TextBlock Text=""{Binding CurrentPage}""
                          VerticalAlignment=""Center""
                          AutomationProperties.Name=""{Binding ., StringFormat='Page {0}'}"" />
                <Button Content=""Next""
                       AutomationProperties.Name=""Next Page""
                       AutomationProperties.HelpText=""Go to next page of results""
                       IsEnabled=""{Binding CanGoNext}""
                       Classes=""pagination""
                       Command=""{Binding NextPageCommand}"" />
            </StackPanel>
        </Grid>
    </StackPanel>
</UserControl>";
    }

    static string GenerateAccessibleModal(AccessibilityConfiguration config)
    {
        return @"<UserControl xmlns=""https://github.com/avaloniaui""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             AutomationProperties.Name=""Accessible Modal Dialog""
             AutomationProperties.Description=""Modal dialog with proper accessibility support"">

    <!-- Modal Overlay -->
    <Border Background=""Black"" Opacity=""0.5"" />

    <!-- Modal Content -->
    <Border Background=""{StaticResource DialogBrush}""
           CornerRadius=""8""
           BorderBrush=""{StaticResource BorderBrush}""
           BorderThickness=""1""
           MaxWidth=""600""
           MaxHeight=""400""
           HorizontalAlignment=""Center""
           VerticalAlignment=""Center""
           AutomationProperties.Role=""Dialog""
           AutomationProperties.Name=""Confirmation Dialog""
           AutomationProperties.Description=""Please confirm your action"">

        <StackPanel Spacing=""16"" Margin=""24"">
            <!-- Dialog Header -->
            <Grid ColumnDefinitions=""*,Auto"">
                <TextBlock Grid.Column=""0""
                          Text=""Confirm Action""
                          Classes=""heading2""
                          AutomationProperties.HeadingLevel=""1""
                          AutomationProperties.Name=""Dialog Title"" />

                <Button Grid.Column=""1""
                       Content=""✕""
                       AutomationProperties.Name=""Close Dialog""
                       AutomationProperties.HelpText=""Close this dialog without saving""
                       Classes=""icon close""
                       Command=""{Binding CloseCommand}"" />
            </Grid>

            <!-- Dialog Content -->
            <TextBlock Text=""Are you sure you want to perform this action? This cannot be undone.""
                      TextWrapping=""Wrap""
                      AutomationProperties.Description=""Warning about permanent action"" />

            <!-- Dialog Actions -->
            <StackPanel Orientation=""Horizontal""
                       HorizontalAlignment=""Right""
                       Spacing=""12"">
                <Button Content=""Cancel""
                       AutomationProperties.Name=""Cancel Action""
                       AutomationProperties.HelpText=""Cancel and close dialog""
                       Classes=""secondary""
                       Command=""{Binding CancelCommand}"" />
                <Button Content=""Confirm""
                       AutomationProperties.Name=""Confirm Action""
                       AutomationProperties.HelpText=""Confirm and proceed with action""
                       Classes=""primary danger""
                       IsDefault=""True""
                       Command=""{Binding ConfirmCommand}"" />
            </StackPanel>
        </StackPanel>
    </Border>
</UserControl>";
    }

    static string GenerateAccessibleNotification(AccessibilityConfiguration config)
    {
        return @"<UserControl xmlns=""https://github.com/avaloniaui""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             AutomationProperties.Name=""Notification Banner""
             AutomationProperties.Description=""System notification with accessibility support"">

    <Border Classes=""notification {Binding NotificationType}""
           Background=""{StaticResource NotificationBrush}""
           BorderBrush=""{StaticResource NotificationBorderBrush}""
           BorderThickness=""1""
           CornerRadius=""4""
           Padding=""16""
           AutomationProperties.Role=""Alert""
           AutomationProperties.LiveSetting=""Assertive"">

        <Grid ColumnDefinitions=""Auto,*,Auto"">
            <!-- Notification Icon -->
            <Border Grid.Column=""0""
                   Classes=""notification-icon""
                   Width=""24""
                   Height=""24""
                   Margin=""0,0,12,0""
                   AutomationProperties.Name=""{Binding IconDescription}"">
                <TextBlock Text=""{Binding NotificationIcon}""
                          FontSize=""16""
                          HorizontalAlignment=""Center""
                          VerticalAlignment=""Center""
                          AutomationProperties.Name=""{Binding IconDescription}"" />
            </Border>

            <!-- Notification Content -->
            <StackPanel Grid.Column=""1"" Spacing=""8"">
                <TextBlock Text=""{Binding Title}""
                          Classes=""notification-title""
                          FontWeight=""Bold""
                          AutomationProperties.HeadingLevel=""2""
                          IsVisible=""{Binding HasTitle}"" />

                <TextBlock Text=""{Binding Message}""
                          Classes=""notification-message""
                          TextWrapping=""Wrap""
                          AutomationProperties.Description=""{Binding Message}"" />

                <!-- Action Buttons -->
                <StackPanel Orientation=""Horizontal""
                           Spacing=""8""
                           IsVisible=""{Binding HasActions}"">
                    <Button Content=""{Binding PrimaryActionText}""
                           AutomationProperties.Name=""{Binding PrimaryActionDescription}""
                           Classes=""notification-action primary""
                           Command=""{Binding PrimaryActionCommand}""
                           IsVisible=""{Binding HasPrimaryAction}"" />

                    <Button Content=""{Binding SecondaryActionText}""
                           AutomationProperties.Name=""{Binding SecondaryActionDescription}""
                           Classes=""notification-action secondary""
                           Command=""{Binding SecondaryActionCommand}""
                           IsVisible=""{Binding HasSecondaryAction}"" />
                </StackPanel>
            </StackPanel>

            <!-- Dismiss Button -->
            <Button Grid.Column=""2""
                   Content=""✕""
                   AutomationProperties.Name=""Dismiss Notification""
                   AutomationProperties.HelpText=""Remove this notification""
                   Classes=""icon dismiss""
                   Command=""{Binding DismissCommand}""
                   IsVisible=""{Binding IsDismissible}"" />
        </Grid>
    </Border>
</UserControl>";
    }

    static string GenerateGenericAccessibleComponent(AccessibilityConfiguration config)
    {
        return @"<UserControl xmlns=""https://github.com/avaloniaui""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             AutomationProperties.Name=""Accessible Component""
             AutomationProperties.Description=""A generic accessible component with proper ARIA support"">

    <StackPanel Spacing=""16"" Margin=""20"">
        <TextBlock Text=""Accessible Component""
                  Classes=""heading1""
                  AutomationProperties.HeadingLevel=""1""
                  AutomationProperties.Name=""Component Title"" />

        <TextBlock Text=""This is an accessible component that follows WCAG guidelines.""
                  TextWrapping=""Wrap""
                  AutomationProperties.Description=""Component description for screen readers"" />

        <Button Content=""Accessible Action""
               AutomationProperties.Name=""Perform Action""
               AutomationProperties.HelpText=""Click to perform the main action""
               Classes=""primary"" />
    </StackPanel>
</UserControl>";
    }

    static string GenerateAccessibilityHelpers(AccessibilityConfiguration config)
    {
        return @"// Accessibility Helper Classes
public static class AccessibilityHelpers
{
    /// <summary>
    /// Announces text to screen readers immediately
    /// </summary>
    public static void AnnounceToScreenReader(string message, Control context)
    {
        if (context == null) return;

        var announcement = new TextBlock
        {
            Text = message,
            IsVisible = false
        };

        AutomationProperties.SetLiveSetting(announcement, AutomationLiveSetting.Assertive);
        AutomationProperties.SetName(announcement, message);

        // Add to visual tree temporarily
        if (context.Parent is Panel parent)
        {
            parent.Children.Add(announcement);

            // Remove after announcement
            Dispatcher.UIThread.Post(() =>
            {
                parent.Children.Remove(announcement);
            }, DispatcherPriority.Background);
        }
    }

    /// <summary>
    /// Sets up proper form field associations
    /// </summary>
    public static void AssociateFormField(TextBlock label, Control input, string helpText = null)
    {
        if (label == null || input == null) return;

        // Associate label with input
        AutomationProperties.SetLabeledBy(input, label);

        // Set help text if provided
        if (!string.IsNullOrEmpty(helpText))
        {
            AutomationProperties.SetHelpText(input, helpText);
        }

        // Ensure input has proper name
        if (string.IsNullOrEmpty(AutomationProperties.GetName(input)))
        {
            AutomationProperties.SetName(input, label.Text?.Replace(""*"", """").Trim());
        }
    }

    /// <summary>
    /// Manages focus for modal dialogs
    /// </summary>
    public static void SetupModalFocus(Control modal, Control initialFocusElement = null)
    {
        if (modal == null) return;

        modal.Loaded += (s, e) =>
        {
            // Set focus to specified element or first focusable element
            var focusTarget = initialFocusElement ?? FindFirstFocusableElement(modal);
            focusTarget?.Focus();
        };
    }

    /// <summary>
    /// Finds the first focusable element in a container
    /// </summary>
    public static Control FindFirstFocusableElement(Control container)
    {
        if (container == null) return null;

        if (container.Focusable && container.IsEnabled && container.IsVisible)
        {
            return container;
        }

        if (container is Panel panel)
        {
            foreach (var child in panel.Children.OfType<Control>())
            {
                var focusable = FindFirstFocusableElement(child);
                if (focusable != null) return focusable;
            }
        }

        return null;
    }

    /// <summary>
    /// Creates accessible error message binding
    /// </summary>
    public static void SetupErrorMessage(Control input, TextBlock errorMessage, string fieldName)
    {
        if (input == null || errorMessage == null) return;

        // Hide error message initially
        errorMessage.IsVisible = false;

        // Set up live region for error announcements
        AutomationProperties.SetLiveSetting(errorMessage, AutomationLiveSetting.Assertive);

        // Associate error with input
        AutomationProperties.SetDescribedBy(input, errorMessage);

        // Set accessible name for error
        AutomationProperties.SetName(errorMessage, $""{fieldName} error message"");
    }

    /// <summary>
    /// Sets up keyboard navigation for custom controls
    /// </summary>
    public static void SetupKeyboardNavigation(Control control, KeyEventHandler keyHandler)
    {
        if (control == null || keyHandler == null) return;

        control.KeyDown += keyHandler;
        control.Focusable = true;

        // Ensure control is included in tab order
        if (control.TabIndex == 0)
        {
            control.TabIndex = 1;
        }
    }

    /// <summary>
    /// Validates color contrast for accessibility
    /// </summary>
    public static bool ValidateColorContrast(Color foreground, Color background, bool isLargeText = false)
    {
        var contrastRatio = CalculateContrastRatio(foreground, background);
        var minimumRatio = isLargeText ? 3.0 : 4.5; // WCAG AA standards

        return contrastRatio >= minimumRatio;
    }

    private static double CalculateContrastRatio(Color foreground, Color background)
    {
        var l1 = GetRelativeLuminance(foreground);
        var l2 = GetRelativeLuminance(background);

        var lighter = Math.Max(l1, l2);
        var darker = Math.Min(l1, l2);

        return (lighter + 0.05) / (darker + 0.05);
    }

    private static double GetRelativeLuminance(Color color)
    {
        var r = GetLuminanceComponent(color.R / 255.0);
        var g = GetLuminanceComponent(color.G / 255.0);
        var b = GetLuminanceComponent(color.B / 255.0);

        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }

    private static double GetLuminanceComponent(double component)
    {
        return component <= 0.03928
            ? component / 12.92
            : Math.Pow((component + 0.055) / 1.055, 2.4);
    }
}";
    }

    static string GenerateKeyboardNavigationCode(AccessibilityConfiguration config)
    {
        return @"// Keyboard Navigation Handler
public class KeyboardNavigationHandler
{
    private readonly List<Control> _focusableElements = new();
    private int _currentFocusIndex = -1;

    public void RegisterFocusableElements(params Control[] elements)
    {
        _focusableElements.Clear();
        _focusableElements.AddRange(elements.Where(e => e != null && e.Focusable));

        // Set up keyboard event handlers
        foreach (var element in _focusableElements)
        {
            element.KeyDown += OnElementKeyDown;
        }
    }

    private void OnElementKeyDown(object sender, KeyEventArgs e)
    {
        if (sender is not Control currentControl) return;

        _currentFocusIndex = _focusableElements.IndexOf(currentControl);

        switch (e.Key)
        {
            case Key.Tab:
                HandleTabNavigation(e);
                break;

            case Key.Enter:
            case Key.Space:
                HandleActivation(currentControl, e);
                break;

            case Key.Escape:
                HandleEscape(currentControl, e);
                break;

            case Key.Home:
                HandleHome(e);
                break;

            case Key.End:
                HandleEnd(e);
                break;

            case Key.Up:
            case Key.Down:
            case Key.Left:
            case Key.Right:
                HandleArrowNavigation(e);
                break;
        }
    }

    private void HandleTabNavigation(KeyEventArgs e)
    {
        if (_focusableElements.Count == 0) return;

        int nextIndex;
        if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            // Shift+Tab: Move to previous element
            nextIndex = _currentFocusIndex > 0 ? _currentFocusIndex - 1 : _focusableElements.Count - 1;
        }
        else
        {
            // Tab: Move to next element
            nextIndex = _currentFocusIndex < _focusableElements.Count - 1 ? _currentFocusIndex + 1 : 0;
        }

        MoveFocusTo(nextIndex);
        e.Handled = true;
    }

    private void HandleActivation(Control control, KeyEventArgs e)
    {
        // Handle Enter/Space activation for buttons and other interactive elements
        if (control is Button button && button.Command?.CanExecute(button.CommandParameter) == true)
        {
            button.Command.Execute(button.CommandParameter);
            e.Handled = true;
        }
        else if (control is CheckBox checkBox)
        {
            checkBox.IsChecked = !checkBox.IsChecked;
            e.Handled = true;
        }
        else if (control is ToggleButton toggle)
        {
            toggle.IsChecked = !toggle.IsChecked;
            e.Handled = true;
        }
    }

    private void HandleEscape(Control control, KeyEventArgs e)
    {
        // Handle Escape key - typically used to close dialogs or cancel operations
        if (control.DataContext is IDialogViewModel dialog)
        {
            dialog.CloseCommand?.Execute(null);
            e.Handled = true;
        }
    }

    private void HandleHome(KeyEventArgs e)
    {
        // Move focus to first element
        if (_focusableElements.Count > 0)
        {
            MoveFocusTo(0);
            e.Handled = true;
        }
    }

    private void HandleEnd(KeyEventArgs e)
    {
        // Move focus to last element
        if (_focusableElements.Count > 0)
        {
            MoveFocusTo(_focusableElements.Count - 1);
            e.Handled = true;
        }
    }

    private void HandleArrowNavigation(KeyEventArgs e)
    {
        // Custom arrow key navigation for grid-like controls
        // This can be customized based on the specific layout

        switch (e.Key)
        {
            case Key.Up:
                MoveFocusToPrevious();
                e.Handled = true;
                break;

            case Key.Down:
                MoveFocusToNext();
                e.Handled = true;
                break;

            case Key.Left:
                if (CultureInfo.CurrentCulture.TextInfo.IsRightToLeft)
                    MoveFocusToNext();
                else
                    MoveFocusToPrevious();
                e.Handled = true;
                break;

            case Key.Right:
                if (CultureInfo.CurrentCulture.TextInfo.IsRightToLeft)
                    MoveFocusToPrevious();
                else
                    MoveFocusToNext();
                e.Handled = true;
                break;
        }
    }

    private void MoveFocusToNext()
    {
        if (_focusableElements.Count == 0) return;

        int nextIndex = _currentFocusIndex < _focusableElements.Count - 1
            ? _currentFocusIndex + 1
            : 0;
        MoveFocusTo(nextIndex);
    }

    private void MoveFocusToPrevious()
    {
        if (_focusableElements.Count == 0) return;

        int prevIndex = _currentFocusIndex > 0
            ? _currentFocusIndex - 1
            : _focusableElements.Count - 1;
        MoveFocusTo(prevIndex);
    }

    private void MoveFocusTo(int index)
    {
        if (index >= 0 && index < _focusableElements.Count)
        {
            var element = _focusableElements[index];
            if (element.IsEnabled && element.IsVisible)
            {
                element.Focus();
                _currentFocusIndex = index;

                // Announce focus change to screen readers
                var name = AutomationProperties.GetName(element);
                if (!string.IsNullOrEmpty(name))
                {
                    AccessibilityHelpers.AnnounceToScreenReader($""Focused on {name}"", element);
                }
            }
        }
    }
}";
    }

    static string GenerateAccessibilityTestingChecklist(AccessibilityConfiguration config)
    {
        return $@"### Manual Testing Checklist

#### Keyboard Navigation
- [ ] All interactive elements are reachable with Tab key
- [ ] Tab order is logical and follows visual layout
- [ ] Shift+Tab moves focus in reverse order
- [ ] Enter/Space activates buttons and controls
- [ ] Escape key works appropriately in dialogs
- [ ] Arrow keys work for grouped controls
- [ ] No keyboard traps exist

#### Screen Reader Testing
- [ ] All elements have appropriate names/labels
- [ ] Form fields are properly associated with labels
- [ ] Required fields are announced as required
- [ ] Error messages are announced when they appear
- [ ] Live regions announce dynamic content changes
- [ ] Headings provide proper document structure
- [ ] Links have descriptive text
- [ ] Images have alternative text

#### Visual Design
- [ ] Focus indicators are clearly visible
- [ ] Color contrast meets WCAG {config.WcagLevel} standards (4.5:1 normal, 3:1 large text)
- [ ] Content is readable without color
- [ ] Text can be resized to 200% without horizontal scrolling
- [ ] No content flashes more than 3 times per second

#### Form Accessibility
- [ ] All form fields have labels
- [ ] Required fields are clearly marked
- [ ] Error messages are specific and helpful
- [ ] Instructions are provided where needed
- [ ] Form validation occurs appropriately

#### Advanced Testing
- [ ] Works with screen reader (NVDA, JAWS, VoiceOver)
- [ ] Works with voice control software
- [ ] Usable with keyboard only
- [ ] Responsive design works at different zoom levels
- [ ] No motion triggers without user control

### Automated Testing Tools
- **axe-core**: Automated accessibility testing
- **WAVE**: Web accessibility evaluation
- **Color Contrast Analyzers**: Check color combinations
- **Keyboard Navigation Testers**: Verify tab order

### WCAG {config.WcagLevel} Compliance Requirements
{GenerateWcagRequirements(config.WcagLevel)}";
    }

    static string GenerateWcagRequirements(string level)
    {
        string baseRequirements = @"
#### Level A Requirements
- Images have alternative text
- Videos have captions
- Content is keyboard accessible
- Pages have titles
- Text has sufficient contrast
- Content doesn't flash excessively

#### Level AA Requirements
- Enhanced color contrast (4.5:1 normal, 3:1 large text)
- Text can resize to 200% without assistive technology
- Content is meaningful when stylesheets are disabled
- Focus is visible and logical
- Page content is organized with headings";

        if (level == "AAA")
        {
            baseRequirements += @"

#### Level AAA Requirements
- Enhanced color contrast (7:1 normal, 4.5:1 large text)
- No keyboard trap situations
- Audio has sign language interpretation
- Context-sensitive help is available
- Error prevention for critical functions";
        }

        return baseRequirements;
    }
}