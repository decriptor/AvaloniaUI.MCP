using System.ComponentModel;

using ModelContextProtocol.Server;

namespace AvaloniaUI.MCP.Tools;

[McpServerToolType]
public static class LocalizationTool
{
    [McpServerTool, Description("Generates localization infrastructure and resource files for AvaloniaUI applications")]
    public static string GenerateLocalizationSystem(
        [Description("Primary language code (e.g., 'en-US')")] string primaryLanguage,
        [Description("Additional languages (comma-separated, e.g., 'es-ES,fr-FR,de-DE')")] string additionalLanguages = "",
        [Description("Include pluralization: 'true' or 'false'")] string includePluralization = "true",
        [Description("Include formatting: 'true' or 'false'")] string includeFormatting = "true")
    {
        try
        {
            var config = new LocalizationConfiguration
            {
                PrimaryLanguage = primaryLanguage,
                AdditionalLanguages = [.. additionalLanguages.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(lang => lang.Trim())],
                IncludePluralization = bool.Parse(includePluralization),
                IncludeFormatting = bool.Parse(includeFormatting)
            };

            string localizationService = GenerateLocalizationService(config);
            string resourceFiles = GenerateResourceFiles(config);
            string markupExtension = GenerateMarkupExtension(config);
            string setupInstructions = GenerateSetupInstructions(config);

            return $@"# Localization System for AvaloniaUI

## Configuration
- **Primary Language**: {config.PrimaryLanguage}
- **Additional Languages**: {string.Join(", ", config.AdditionalLanguages)}
- **Pluralization**: {config.IncludePluralization}
- **Formatting**: {config.IncludeFormatting}

## Localization Service
```csharp
{localizationService}
```

## Resource Files Structure
{resourceFiles}

## XAML Markup Extension
```csharp
{markupExtension}
```

## Setup Instructions
{setupInstructions}

## Usage Examples

### In XAML
```xml
<TextBlock Text=""{{loc:Localize Key='WelcomeMessage'}}"" />
<Button Content=""{{loc:Localize Key='SaveButton'}}"" />
```

### In Code-Behind
```csharp
var message = _localizationService.GetString(""WelcomeMessage"");
var formatted = _localizationService.GetFormattedString(""UserGreeting"", userName);
```

### In ViewModels
```csharp
public string WelcomeText => _localizationService.GetString(""WelcomeMessage"");
```";
        }
        catch (Exception ex)
        {
            return $"Error generating localization system: {ex.Message}";
        }
    }

    [McpServerTool, Description("Creates culture-specific formatting and validation utilities")]
    public static string GenerateCultureFormatting(
        [Description("Culture codes (comma-separated, e.g., 'en-US,es-ES,ja-JP')")] string cultures,
        [Description("Include date/time formatting: 'true' or 'false'")] string includeDateTimeFormatting = "true",
        [Description("Include number formatting: 'true' or 'false'")] string includeNumberFormatting = "true",
        [Description("Include currency formatting: 'true' or 'false'")] string includeCurrencyFormatting = "true")
    {
        try
        {
            var cultureList = cultures.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim()).ToList();

            string formattingService = GenerateFormattingService(cultureList,
                bool.Parse(includeDateTimeFormatting),
                bool.Parse(includeNumberFormatting),
                bool.Parse(includeCurrencyFormatting));

            string validators = GenerateCultureValidators(cultureList);
            string converters = GenerateCultureConverters(cultureList);

            return $@"# Culture-Specific Formatting

## Supported Cultures
{string.Join("\n", cultureList.Select(c => $"- {c}"))}

## Formatting Service
```csharp
{formattingService}
```

## Culture Validators
```csharp
{validators}
```

## Value Converters
```csharp
{converters}
```

## Usage Examples
```csharp
// Format numbers
var price = _formattingService.FormatCurrency(99.99m, ""en-US""); // $99.99
var price_es = _formattingService.FormatCurrency(99.99m, ""es-ES""); // 99,99 €

// Format dates
var date = _formattingService.FormatDate(DateTime.Now, ""en-US""); // 12/25/2024
var date_ja = _formattingService.FormatDate(DateTime.Now, ""ja-JP""); // 2024年12月25日
```";
        }
        catch (Exception ex)
        {
            return $"Error generating culture formatting: {ex.Message}";
        }
    }

    private sealed class LocalizationConfiguration
    {
        public string PrimaryLanguage { get; set; } = "";
        public List<string> AdditionalLanguages { get; set; } = new();
        public bool IncludePluralization { get; set; }
        public bool IncludeFormatting { get; set; }
    }

    private static string GenerateLocalizationService(LocalizationConfiguration config)
    {
        string pluralizationCode = config.IncludePluralization ? GeneratePluralizationCode() : "";
        string formattingCode = config.IncludeFormatting ? GenerateFormattingHelpers() : "";

        return $@"public interface ILocalizationService
{{
    string GetString(string key, string? culture = null);
    string GetFormattedString(string key, params object[] args);
    string GetPluralString(string key, int count, string? culture = null);
    void SetCulture(string culture);
    string CurrentCulture {{ get; }}
    IEnumerable<string> AvailableCultures {{ get; }}
    event EventHandler<CultureChangedEventArgs>? CultureChanged;
}}

public class LocalizationService : ILocalizationService, INotifyPropertyChanged
{{
    private readonly Dictionary<string, ResourceManager> _resourceManagers = new();
    private CultureInfo _currentCulture;
    private readonly string _defaultCulture;

    public LocalizationService(string defaultCulture = ""{config.PrimaryLanguage}"")
    {{
        _defaultCulture = defaultCulture;
        _currentCulture = new CultureInfo(defaultCulture);
        
        // Initialize resource managers
        InitializeResourceManagers();
    }}

    public string CurrentCulture => _currentCulture.Name;

    public IEnumerable<string> AvailableCultures => new[]
    {{
        ""{config.PrimaryLanguage}""{(config.AdditionalLanguages.Count != 0 ? ",\n        " + string.Join(",\n        ", config.AdditionalLanguages.Select(lang => $"\"{lang}\"")) : "")}
    }};

    public event EventHandler<CultureChangedEventArgs>? CultureChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    public string GetString(string key, string? culture = null)
    {{
        var targetCulture = culture ?? _currentCulture.Name;
        
        if (_resourceManagers.TryGetValue(targetCulture, out var resourceManager))
        {{
            var value = resourceManager.GetString(key, new CultureInfo(targetCulture));
            if (!string.IsNullOrEmpty(value))
                return value;
        }}

        // Fallback to default culture
        if (targetCulture != _defaultCulture && _resourceManagers.TryGetValue(_defaultCulture, out var defaultManager))
        {{
            var fallbackValue = defaultManager.GetString(key, new CultureInfo(_defaultCulture));
            if (!string.IsNullOrEmpty(fallbackValue))
                return fallbackValue;
        }}

        return $""[{{key}}]""; // Return key if not found
    }}

    public string GetFormattedString(string key, params object[] args)
    {{
        var template = GetString(key);
        try
        {{
            return string.Format(_currentCulture, template, args);
        }}
        catch (FormatException)
        {{
            return template; // Return unformatted if formatting fails
        }}
    }}

    {(config.IncludePluralization ? @"public string GetPluralString(string key, int count, string? culture = null)
    {
        var rules = GetPluralRules(culture ?? _currentCulture.Name);
        var pluralForm = rules.GetPluralForm(count);
        var pluralKey = $""{key}.{pluralForm}"";
        
        var result = GetString(pluralKey, culture);
        if (result.StartsWith(""["") && result.EndsWith(""]""))
        {
            // Fallback to base key if plural form not found
            result = GetString(key, culture);
        }
        
        return string.Format(_currentCulture, result, count);
    }" : @"public string GetPluralString(string key, int count, string? culture = null)
    {
        // Simple pluralization without rules
        var template = GetString(key, culture);
        return string.Format(_currentCulture, template, count);
    }")}

    public void SetCulture(string culture)
    {{
        var newCulture = new CultureInfo(culture);
        if (newCulture.Name == _currentCulture.Name)
            return;

        var oldCulture = _currentCulture.Name;
        _currentCulture = newCulture;
        
        // Set thread culture
        CultureInfo.CurrentCulture = newCulture;
        CultureInfo.CurrentUICulture = newCulture;

        // Notify changes
        CultureChanged?.Invoke(this, new CultureChangedEventArgs(oldCulture, culture));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCulture)));
    }}

    private void InitializeResourceManagers()
    {{
        var assembly = Assembly.GetExecutingAssembly();
        
        foreach (var culture in AvailableCultures)
        {{
            try
            {{
                var resourceName = $""{{assembly.GetName().Name}}.Resources.Strings"";
                var resourceManager = new ResourceManager(resourceName, assembly);
                _resourceManagers[culture] = resourceManager;
            }}
            catch (Exception ex)
            {{
                Console.WriteLine($""Failed to load resources for culture {{culture}}: {{ex.Message}}"");
            }}
        }}
    }}

    {pluralizationCode}
    {formattingCode}
}}

public class CultureChangedEventArgs : EventArgs
{{
    public string OldCulture {{ get; }}
    public string NewCulture {{ get; }}

    public CultureChangedEventArgs(string oldCulture, string newCulture)
    {{
        OldCulture = oldCulture;
        NewCulture = newCulture;
    }}
}}";
    }

    private static string GeneratePluralizationCode()
    {
        return @"
    private IPluralRules GetPluralRules(string culture)
    {
        return culture switch
        {
            ""en"" or ""en-US"" or ""en-GB"" => new EnglishPluralRules(),
            ""es"" or ""es-ES"" or ""es-MX"" => new SpanishPluralRules(),
            ""fr"" or ""fr-FR"" or ""fr-CA"" => new FrenchPluralRules(),
            ""de"" or ""de-DE"" or ""de-AT"" => new GermanPluralRules(),
            ""ru"" or ""ru-RU"" => new RussianPluralRules(),
            ""ja"" or ""ja-JP"" => new JapanesePluralRules(),
            ""zh"" or ""zh-CN"" or ""zh-TW"" => new ChinesePluralRules(),
            _ => new DefaultPluralRules()
        };
    }
}

public interface IPluralRules
{
    string GetPluralForm(int count);
}

public class EnglishPluralRules : IPluralRules
{
    public string GetPluralForm(int count)
    {
        return count == 1 ? ""one"" : ""other"";
    }
}

public class SpanishPluralRules : IPluralRules
{
    public string GetPluralForm(int count)
    {
        return count == 1 ? ""one"" : ""other"";
    }
}

public class RussianPluralRules : IPluralRules
{
    public string GetPluralForm(int count)
    {
        var mod10 = count % 10;
        var mod100 = count % 100;
        
        if (mod10 == 1 && mod100 != 11)
            return ""one"";
        if (mod10 >= 2 && mod10 <= 4 && (mod100 < 10 || mod100 >= 20))
            return ""few"";
        
        return ""many"";
    }
}

public class DefaultPluralRules : IPluralRules
{
    public string GetPluralForm(int count)
    {
        return count == 1 ? ""one"" : ""other"";
    }";
    }

    private static string GenerateFormattingHelpers()
    {
        return @"
    public string FormatCurrency(decimal amount, string? culture = null)
    {
        var targetCulture = new CultureInfo(culture ?? _currentCulture.Name);
        return amount.ToString(""C"", targetCulture);
    }

    public string FormatNumber(double number, string? culture = null)
    {
        var targetCulture = new CultureInfo(culture ?? _currentCulture.Name);
        return number.ToString(""N"", targetCulture);
    }

    public string FormatDate(DateTime date, string? culture = null)
    {
        var targetCulture = new CultureInfo(culture ?? _currentCulture.Name);
        return date.ToString(""d"", targetCulture);
    }

    public string FormatDateTime(DateTime dateTime, string? culture = null)
    {
        var targetCulture = new CultureInfo(culture ?? _currentCulture.Name);
        return dateTime.ToString(""F"", targetCulture);
    }";
    }

    private static string GenerateResourceFiles(LocalizationConfiguration config)
    {
        var allLanguages = new List<string> { config.PrimaryLanguage };
        allLanguages.AddRange(config.AdditionalLanguages);

        string resourceStructure = $@"```
Resources/
├── Strings.resx                    // {config.PrimaryLanguage} (default)";

        foreach (string lang in config.AdditionalLanguages)
        {
            resourceStructure += $@"
├── Strings.{lang}.resx             // {lang}";
        }

        resourceStructure += @"
└── README.md                       // Resource documentation
```

## Sample Resource Files

### Strings.resx (Default - " + config.PrimaryLanguage + @")
```xml
<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <data name=""WelcomeMessage"" xml:space=""preserve"">
    <value>Welcome to our application!</value>
  </data>
  <data name=""SaveButton"" xml:space=""preserve"">
    <value>Save</value>
  </data>
  <data name=""CancelButton"" xml:space=""preserve"">
    <value>Cancel</value>
  </data>
  <data name=""UserGreeting"" xml:space=""preserve"">
    <value>Hello, {0}!</value>
  </data>";

        if (config.IncludePluralization)
        {
            resourceStructure += @"
  <data name=""ItemCount.one"" xml:space=""preserve"">
    <value>{0} item</value>
  </data>
  <data name=""ItemCount.other"" xml:space=""preserve"">
    <value>{0} items</value>
  </data>";
        }

        resourceStructure += @"
</root>
```";

        if (config.AdditionalLanguages.Contains("es-ES"))
        {
            resourceStructure += @"

### Strings.es-ES.resx (Spanish)
```xml
<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <data name=""WelcomeMessage"" xml:space=""preserve"">
    <value>¡Bienvenido a nuestra aplicación!</value>
  </data>
  <data name=""SaveButton"" xml:space=""preserve"">
    <value>Guardar</value>
  </data>
  <data name=""CancelButton"" xml:space=""preserve"">
    <value>Cancelar</value>
  </data>
  <data name=""UserGreeting"" xml:space=""preserve"">
    <value>¡Hola, {0}!</value>
  </data>";

            if (config.IncludePluralization)
            {
                resourceStructure += @"
  <data name=""ItemCount.one"" xml:space=""preserve"">
    <value>{0} elemento</value>
  </data>
  <data name=""ItemCount.other"" xml:space=""preserve"">
    <value>{0} elementos</value>
  </data>";
            }

            resourceStructure += @"
</root>
```";
        }

        return resourceStructure;
    }

    private static string GenerateMarkupExtension(LocalizationConfiguration config)
    {
        return @"// XAML Markup Extension for Localization
public class LocalizeExtension : MarkupExtension
{
    public string Key { get; set; } = string.Empty;
    public string? Culture { get; set; }
    public object[]? Arguments { get; set; }
    public bool UseFormatting { get; set; } = true;

    private static ILocalizationService? _localizationService;

    public static void SetLocalizationService(ILocalizationService service)
    {
        _localizationService = service;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (_localizationService == null)
        {
            // Try to resolve from service provider
            if (serviceProvider.GetService(typeof(ILocalizationService)) is ILocalizationService service)
            {
                _localizationService = service;
            }
            else
            {
                return $""[{Key}]""; // Fallback if service not available
            }
        }

        try
        {
            if (Arguments != null && Arguments.Length > 0 && UseFormatting)
            {
                return _localizationService.GetFormattedString(Key, Arguments);
            }
            else
            {
                return _localizationService.GetString(Key, Culture);
            }
        }
        catch (Exception)
        {
            return $""[{Key}]""; // Fallback on error
        }
    }
}

// Reactive localization extension for dynamic updates
public class ReactiveLocalizeExtension : MarkupExtension
{
    public string Key { get; set; } = string.Empty;
    public string? Culture { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var targetService = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        var target = targetService?.TargetObject as DependencyObject;
        var property = targetService?.TargetProperty as DependencyProperty;

        if (target != null && property != null)
        {
            return new LocalizedBinding(Key, Culture, target, property);
        }

        // Fallback to static value
        return new LocalizeExtension { Key = Key, Culture = Culture }.ProvideValue(serviceProvider);
    }
}

public class LocalizedBinding : IDisposable
{
    private readonly string _key;
    private readonly string? _culture;
    private readonly WeakReference _targetRef;
    private readonly DependencyProperty _property;
    private readonly ILocalizationService _localizationService;

    public LocalizedBinding(string key, string? culture, DependencyObject target, DependencyProperty property)
    {
        _key = key;
        _culture = culture;
        _targetRef = new WeakReference(target);
        _property = property;

        // Get localization service (in real implementation, use DI)
        _localizationService = App.Current.Services.GetService<ILocalizationService>();
        _localizationService.CultureChanged += OnCultureChanged;

        UpdateValue();
    }

    private void OnCultureChanged(object? sender, CultureChangedEventArgs e)
    {
        UpdateValue();
    }

    private void UpdateValue()
    {
        if (_targetRef.Target is DependencyObject target)
        {
            var value = _localizationService.GetString(_key, _culture);
            target.SetValue(_property, value);
        }
        else
        {
            Dispose(); // Target is gone, clean up
        }
    }

    public void Dispose()
    {
        _localizationService.CultureChanged -= OnCultureChanged;
    }
}";
    }

    private static string GenerateSetupInstructions(LocalizationConfiguration config)
    {
        return $@"### 1. Install Required Packages
```xml
<PackageReference Include=""System.Resources.Extensions"" Version=""8.0.0"" />
```

### 2. Configure Services
```csharp
// In Program.cs or App.axaml.cs
services.AddSingleton<ILocalizationService>(provider => 
    new LocalizationService(""{config.PrimaryLanguage}""));

// Register markup extension
LocalizeExtension.SetLocalizationService(services.GetService<ILocalizationService>());
```

### 3. Create Resource Files
1. Add folder: `Resources/`
2. Add `Strings.resx` (primary language)
3. Add culture-specific files: `Strings.{{culture}}.resx`
4. Set Build Action to ""Embedded Resource""

### 4. Usage in XAML
```xml
<Window xmlns:loc=""clr-namespace:YourApp.Localization"">
    <StackPanel>
        <TextBlock Text=""{{loc:Localize Key='WelcomeMessage'}}"" />
        <Button Content=""{{loc:Localize Key='SaveButton'}}"" />
    </StackPanel>
</Window>
```

### 5. Culture Switching
```csharp
// In ViewModel or code-behind
public void SwitchToCulture(string culture)
{{
    _localizationService.SetCulture(culture);
    // UI will update automatically with reactive bindings
}}
```

### 6. Resource Management
- Use meaningful key names (e.g., `Dialog.Save.Button`)
- Group related resources with prefixes
- Include context comments in .resx files
- Use placeholders for dynamic content: `Welcome, {{0}}!`";
    }

    private static string GenerateFormattingService(List<string> cultures, bool includeDateTime, bool includeNumber, bool includeCurrency)
    {
        var methods = new List<string>();

        if (includeDateTime)
        {
            methods.Add(@"    public string FormatDate(DateTime date, string culture)
    {
        return date.ToString(""d"", new CultureInfo(culture));
    }

    public string FormatTime(DateTime time, string culture)
    {
        return time.ToString(""t"", new CultureInfo(culture));
    }

    public string FormatDateTime(DateTime dateTime, string culture)
    {
        return dateTime.ToString(""F"", new CultureInfo(culture));
    }");
        }

        if (includeNumber)
        {
            methods.Add(@"    public string FormatNumber(double number, string culture, int decimals = 2)
    {
        var format = $""N{decimals}"";
        return number.ToString(format, new CultureInfo(culture));
    }

    public string FormatPercentage(double percentage, string culture)
    {
        return percentage.ToString(""P"", new CultureInfo(culture));
    }");
        }

        if (includeCurrency)
        {
            methods.Add(@"    public string FormatCurrency(decimal amount, string culture)
    {
        return amount.ToString(""C"", new CultureInfo(culture));
    }

    public string FormatCurrency(decimal amount, string culture, string currencyCode)
    {
        var cultureInfo = new CultureInfo(culture);
        var regionInfo = new RegionInfo(cultureInfo.Name);
        
        // Custom currency formatting
        var numberFormat = (NumberFormatInfo)cultureInfo.NumberFormat.Clone();
        numberFormat.CurrencySymbol = GetCurrencySymbol(currencyCode);
        
        return amount.ToString(""C"", numberFormat);
    }

    private string GetCurrencySymbol(string currencyCode)
    {
        return currencyCode switch
        {
            ""USD"" => ""$"",
            ""EUR"" => ""€"",
            ""GBP"" => ""£"",
            ""JPY"" => ""¥"",
            ""CNY"" => ""¥"",
            ""CAD"" => ""C$"",
            _ => currencyCode
        };
    }");
        }

        return $@"public interface ICultureFormattingService
{{
    string FormatDate(DateTime date, string culture);
    string FormatTime(DateTime time, string culture);
    string FormatDateTime(DateTime dateTime, string culture);
    string FormatNumber(double number, string culture, int decimals = 2);
    string FormatPercentage(double percentage, string culture);
    string FormatCurrency(decimal amount, string culture);
    string FormatCurrency(decimal amount, string culture, string currencyCode);
    bool ValidateFormat(string value, string culture, FormatType formatType);
}}

public enum FormatType
{{
    Date,
    Time,
    DateTime,
    Number,
    Currency,
    Percentage
}}

public class CultureFormattingService : ICultureFormattingService
{{
    private readonly Dictionary<string, CultureInfo> _cultures;

    public CultureFormattingService()
    {{
        _cultures = new Dictionary<string, CultureInfo>();
        var supportedCultures = new[] {{ {string.Join(", ", cultures.Select(c => $"\"{c}\""))} }};
        
        foreach (var culture in supportedCultures)
        {{
            try
            {{
                _cultures[culture] = new CultureInfo(culture);
            }}
            catch (CultureNotFoundException)
            {{
                Console.WriteLine($""Culture {{culture}} not supported on this system"");
            }}
        }}
    }}

{string.Join("\n\n", methods)}

    public bool ValidateFormat(string value, string culture, FormatType formatType)
    {{
        if (!_cultures.TryGetValue(culture, out var cultureInfo))
            return false;

        try
        {{
            return formatType switch
            {{
                FormatType.Date => DateTime.TryParse(value, cultureInfo, DateTimeStyles.None, out _),
                FormatType.Time => TimeSpan.TryParse(value, cultureInfo, out _),
                FormatType.DateTime => DateTime.TryParse(value, cultureInfo, DateTimeStyles.None, out _),
                FormatType.Number => double.TryParse(value, NumberStyles.Number, cultureInfo, out _),
                FormatType.Currency => decimal.TryParse(value, NumberStyles.Currency, cultureInfo, out _),
                FormatType.Percentage => double.TryParse(value, NumberStyles.AllowPercentSymbol | NumberStyles.Number, cultureInfo, out _),
                _ => false
            }};
        }}
        catch
        {{
            return false;
        }}
    }}
}}";
    }

    private static string GenerateCultureValidators(List<string> cultures)
    {
        return $@"// Culture-specific validation rules
public static class CultureValidators
{{
    public static bool ValidatePhoneNumber(string phoneNumber, string culture)
    {{
        return culture switch
        {{
            ""en-US"" => ValidateUSPhoneNumber(phoneNumber),
            ""es-ES"" => ValidateSpanishPhoneNumber(phoneNumber),
            ""fr-FR"" => ValidateFrenchPhoneNumber(phoneNumber),
            ""de-DE"" => ValidateGermanPhoneNumber(phoneNumber),
            ""ja-JP"" => ValidateJapanesePhoneNumber(phoneNumber),
            _ => ValidateGenericPhoneNumber(phoneNumber)
        }};
    }}

    public static bool ValidatePostalCode(string postalCode, string culture)
    {{
        return culture switch
        {{
            ""en-US"" => Regex.IsMatch(postalCode, @""^\d{{5}}(-\d{{4}})?$""), // ZIP code
            ""es-ES"" => Regex.IsMatch(postalCode, @""^\d{{5}}$""), // Spanish postal code
            ""fr-FR"" => Regex.IsMatch(postalCode, @""^\d{{5}}$""), // French postal code
            ""de-DE"" => Regex.IsMatch(postalCode, @""^\d{{5}}$""), // German postal code
            ""en-GB"" => Regex.IsMatch(postalCode, @""^[A-Z]{{1,2}}\d{{1,2}}[A-Z]? \d[A-Z]{{2}}$""), // UK postcode
            ""ja-JP"" => Regex.IsMatch(postalCode, @""^\d{{3}}-\d{{4}}$""), // Japanese postal code
            _ => !string.IsNullOrWhiteSpace(postalCode)
        }};
    }}

    public static bool ValidateIdNumber(string idNumber, string culture)
    {{
        return culture switch
        {{
            ""en-US"" => ValidateSSN(idNumber),
            ""es-ES"" => ValidateSpanishDNI(idNumber),
            ""fr-FR"" => ValidateFrenchINSEE(idNumber),
            ""de-DE"" => ValidateGermanIdCard(idNumber),
            _ => !string.IsNullOrWhiteSpace(idNumber)
        }};
    }}

    private static bool ValidateUSPhoneNumber(string phone)
    {{
        // US phone number: (123) 456-7890 or 123-456-7890 or 1234567890
        return Regex.IsMatch(phone, @""^(\+1)?[-.\s]?(\(?\d{{3}}\)?[-.\s]?\d{{3}}[-.\s]?\d{{4}})$"");
    }}

    private static bool ValidateSSN(string ssn)
    {{
        // US SSN: 123-45-6789
        return Regex.IsMatch(ssn, @""^\d{{3}}-\d{{2}}-\d{{4}}$"");
    }}

    private static bool ValidateSpanishDNI(string dni)
    {{
        // Spanish DNI: 12345678A
        if (!Regex.IsMatch(dni, @""^\d{{8}}[A-Z]$""))
            return false;

        var number = int.Parse(dni.Substring(0, 8));
        var letter = dni[8];
        var expectedLetter = ""TRWAGMYFPDXBNJZSQVHLCKE""[number % 23];
        
        return letter == expectedLetter;
    }}

    private static bool ValidateGenericPhoneNumber(string phone)
    {{
        // Generic validation: at least 7 digits
        var digitsOnly = Regex.Replace(phone, @""[^\d]"", """");
        return digitsOnly.Length >= 7 && digitsOnly.Length <= 15;
    }}

    // Add more culture-specific validators as needed
    private static bool ValidateSpanishPhoneNumber(string phone) => ValidateGenericPhoneNumber(phone);
    private static bool ValidateFrenchPhoneNumber(string phone) => ValidateGenericPhoneNumber(phone);
    private static bool ValidateGermanPhoneNumber(string phone) => ValidateGenericPhoneNumber(phone);
    private static bool ValidateJapanesePhoneNumber(string phone) => ValidateGenericPhoneNumber(phone);
    private static bool ValidateFrenchINSEE(string insee) => !string.IsNullOrWhiteSpace(insee);
    private static bool ValidateGermanIdCard(string id) => !string.IsNullOrWhiteSpace(id);
}}";
    }

    private static string GenerateCultureConverters(List<string> cultures)
    {
        return @"// Avalonia value converters for culture-specific formatting
public class CultureDateConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime date)
        {
            var format = parameter as string ?? ""d"";
            return date.ToString(format, culture);
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string dateString)
        {
            if (DateTime.TryParse(dateString, culture, DateTimeStyles.None, out var result))
                return result;
        }
        return null;
    }
}

public class CultureNumberConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double number)
        {
            var format = parameter as string ?? ""N2"";
            return number.ToString(format, culture);
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string numberString)
        {
            if (double.TryParse(numberString, NumberStyles.Number, culture, out var result))
                return result;
        }
        return null;
    }
}

public class CultureCurrencyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal currency)
        {
            return currency.ToString(""C"", culture);
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string currencyString)
        {
            if (decimal.TryParse(currencyString, NumberStyles.Currency, culture, out var result))
                return result;
        }
        return null;
    }
}

// Multi-culture converter that accepts culture as parameter
public class MultiCultureConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count >= 2 && values[0] != null && values[1] is string cultureName)
        {
            var targetCulture = new CultureInfo(cultureName);
            var value = values[0];

            return value switch
            {
                DateTime date => date.ToString(""d"", targetCulture),
                double number => number.ToString(""N2"", targetCulture),
                decimal currency => currency.ToString(""C"", targetCulture),
                _ => value?.ToString()
            };
        }
        return values.FirstOrDefault();
    }
}";
    }
}
