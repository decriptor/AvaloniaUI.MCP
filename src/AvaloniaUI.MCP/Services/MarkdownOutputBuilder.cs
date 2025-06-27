using System.Text;

namespace AvaloniaUI.MCP.Services;

/// <summary>
/// Provides a fluent API for building consistent markdown output across all MCP tools
/// </summary>
public sealed class MarkdownOutputBuilder
{
    readonly StringBuilder _builder = new();

    /// <summary>
    /// Creates a new markdown output builder with the specified title
    /// </summary>
    public static MarkdownOutputBuilder Create(string title)
    {
        var builder = new MarkdownOutputBuilder();
        builder._builder.AppendLine($"# {title}");
        builder._builder.AppendLine();
        return builder;
    }

    /// <summary>
    /// Adds a section header
    /// </summary>
    public MarkdownOutputBuilder AddHeader(string header, int level = 2)
    {
        string prefix = new('#', level);
        _builder.AppendLine($"{prefix} {header}");
        _builder.AppendLine();
        return this;
    }

    /// <summary>
    /// Adds a configuration section with key-value pairs
    /// </summary>
    public MarkdownOutputBuilder AddConfiguration(params (string key, object value)[] configurations)
    {
        if (configurations.Length > 0)
        {
            AddHeader("Configuration");
            foreach ((string key, object value) in configurations)
            {
                _builder.AppendLine($"- **{key}**: {value}");
            }
            _builder.AppendLine();
        }
        return this;
    }

    /// <summary>
    /// Adds a code block with the specified language and content
    /// </summary>
    public MarkdownOutputBuilder AddCodeBlock(string language, string code)
    {
        if (!string.IsNullOrWhiteSpace(code))
        {
            _builder.AppendLine($"```{language}");
            _builder.AppendLine(code);
            _builder.AppendLine("```");
            _builder.AppendLine();
        }
        return this;
    }

    /// <summary>
    /// Adds a code section with header and code block
    /// </summary>
    public MarkdownOutputBuilder AddCodeSection(string title, string language, string code)
    {
        if (!string.IsNullOrWhiteSpace(code))
        {
            AddHeader(title);
            AddCodeBlock(language, code);
        }
        return this;
    }

    /// <summary>
    /// Adds a bulleted list
    /// </summary>
    public MarkdownOutputBuilder AddBulletList(IEnumerable<string> items)
    {
        foreach (string item in items)
        {
            _builder.AppendLine($"- {item}");
        }
        _builder.AppendLine();
        return this;
    }

    /// <summary>
    /// Adds a numbered list
    /// </summary>
    public MarkdownOutputBuilder AddNumberedList(IEnumerable<string> items)
    {
        int index = 1;
        foreach (string item in items)
        {
            _builder.AppendLine($"{index}. {item}");
            index++;
        }
        _builder.AppendLine();
        return this;
    }

    /// <summary>
    /// Adds a table with headers and rows
    /// </summary>
    public MarkdownOutputBuilder AddTable(string[] headers, string[][] rows)
    {
        if (headers.Length > 0)
        {
            // Add headers
            _builder.AppendLine($"| {string.Join(" | ", headers)} |");

            // Add separator
            string separator = string.Join(" | ", headers.Select(_ => "---"));
            _builder.AppendLine($"| {separator} |");

            // Add rows
            foreach (string[] row in rows)
            {
                string rowContent = string.Join(" | ", row.Take(headers.Length));
                _builder.AppendLine($"| {rowContent} |");
            }
            _builder.AppendLine();
        }
        return this;
    }

    /// <summary>
    /// Adds a section with custom content
    /// </summary>
    public MarkdownOutputBuilder AddSection(string title, string content)
    {
        if (!string.IsNullOrWhiteSpace(content))
        {
            AddHeader(title);
            _builder.AppendLine(content);
            _builder.AppendLine();
        }
        return this;
    }

    /// <summary>
    /// Adds raw markdown content
    /// </summary>
    public MarkdownOutputBuilder AddRaw(string content)
    {
        _builder.AppendLine(content);
        return this;
    }

    /// <summary>
    /// Adds a horizontal rule
    /// </summary>
    public MarkdownOutputBuilder AddHorizontalRule()
    {
        _builder.AppendLine("---");
        _builder.AppendLine();
        return this;
    }

    /// <summary>
    /// Adds a usage examples section
    /// </summary>
    public MarkdownOutputBuilder AddUsageExamples(IEnumerable<(string title, string description, string code)> examples)
    {
        var exampleList = examples.ToList();
        if (exampleList.Count > 0)
        {
            AddHeader("Usage Examples");

            foreach ((string title, string description, string code) in exampleList)
            {
                AddHeader(title, 3);
                if (!string.IsNullOrWhiteSpace(description))
                {
                    _builder.AppendLine(description);
                    _builder.AppendLine();
                }
                if (!string.IsNullOrWhiteSpace(code))
                {
                    AddCodeBlock("csharp", code);
                }
            }
        }
        return this;
    }

    /// <summary>
    /// Adds a notes section with important information
    /// </summary>
    public MarkdownOutputBuilder AddNotes(IEnumerable<string> notes)
    {
        var noteList = notes.ToList();
        if (noteList.Count > 0)
        {
            AddHeader("Important Notes");
            AddBulletList(noteList.Select(note => $"**Note**: {note}"));
        }
        return this;
    }

    /// <summary>
    /// Adds a best practices section
    /// </summary>
    public MarkdownOutputBuilder AddBestPractices(IEnumerable<string> practices)
    {
        var practiceList = practices.ToList();
        if (practiceList.Count > 0)
        {
            AddHeader("Best Practices");
            AddBulletList(practiceList.Select(practice => $"✅ {practice}"));
        }
        return this;
    }

    /// <summary>
    /// Adds a warnings section
    /// </summary>
    public MarkdownOutputBuilder AddWarnings(IEnumerable<string> warnings)
    {
        var warningList = warnings.ToList();
        if (warningList.Count > 0)
        {
            AddHeader("⚠️ Warnings");
            AddBulletList(warningList.Select(warning => $"⚠️ {warning}"));
        }
        return this;
    }

    /// <summary>
    /// Conditionally adds content based on a condition
    /// </summary>
    public MarkdownOutputBuilder AddIf(bool condition, Action<MarkdownOutputBuilder> builderAction)
    {
        if (condition)
        {
            builderAction(this);
        }
        return this;
    }

    /// <summary>
    /// Builds the final markdown string
    /// </summary>
    public string Build()
    {
        return _builder.ToString().TrimEnd();
    }

    /// <summary>
    /// Implicit conversion to string
    /// </summary>
    public static implicit operator string(MarkdownOutputBuilder builder)
    {
        return builder.Build();
    }

    /// <summary>
    /// ToString override
    /// </summary>
    public override string ToString()
    {
        return Build();
    }
}