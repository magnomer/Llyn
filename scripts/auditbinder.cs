using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

internal sealed class AuditBinder
{
    private static readonly CSharpParseOptions ParseOptions = new(
        languageVersion: LanguageVersion.Preview,
        documentationMode: DocumentationMode.None,
        kind: SourceCodeKind.Regular);

    private AuditBinder(string root, CSharpCompilation compilation, IReadOnlyList<SyntaxTree> tracked)
    {
        Root = root;
        Compilation = compilation;
        Tracked = tracked;
    }

    public string Root { get; }

    public CSharpCompilation Compilation { get; }

    public IReadOnlyList<SyntaxTree> Tracked { get; }

    public string Relative(string path) => Path.GetRelativePath(Root, path).Replace('\\', '/');

    public static AuditBinder Bind(string root, string configPath)
    {
        JsonElement config = JsonDocument.Parse(File.ReadAllText(configPath)).RootElement;
        string source = config.GetProperty("source").GetString()!;
        string configuration = config.GetProperty("configuration").GetString()!;
        string reference = config.GetProperty("reference").GetString()!;
        string owned = config.GetProperty("project").GetString()!;
        string[] packs = Strings(config, "packs");
        string[] segments = Strings(config, "excludeSegments");
        string[] suffixes = Strings(config, "excludeSuffixes");
        string[] prefixes = Strings(config, "excludePrefixes");

        List<string> sources = Files(root, [source + "*.cs"], [], segments, suffixes, prefixes);
        if (sources.Count == 0)
        {
            throw new InvalidOperationException("No tracked source file was enumerated, so the binder would compile nothing.");
        }

        List<string> projects = Files(root, [source + "*.csproj"], [], [], [], []);
        if (projects.Count == 0)
        {
            throw new InvalidOperationException("No tracked project file was enumerated, so the binder would compile nothing.");
        }

        List<SyntaxTree> trees = sources
            .Concat(Generated(root, projects, configuration))
            .AsParallel()
            .AsOrdered()
            .Select(path => CSharpSyntaxTree.ParseText(File.ReadAllText(path), ParseOptions, path))
            .ToList();
        CSharpCompilation compilation = CSharpCompilation.Create(
            "AuditBinder",
            trees,
            References(root, packs, reference, configuration, owned),
            new CSharpCompilationOptions(
                OutputKind.ConsoleApplication,
                allowUnsafe: true,
                nullableContextOptions: NullableContextOptions.Enable));
        List<string> errors = compilation.GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .Select(diagnostic => $"  {diagnostic}")
            .ToList();
        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                $"{errors.Count} compile error(s) in the bound sources, so an unbound name would slip past the audit.\n"
                + string.Join('\n', errors.Take(40)));
        }

        string prefix = Path.GetFullPath(Path.Combine(root, source.TrimEnd('/'))) + Path.DirectorySeparatorChar;
        List<SyntaxTree> tracked = compilation.SyntaxTrees
            .Where(tree => !Path.GetRelativePath(root, tree.FilePath).Replace('\\', '/').Split('/')
                .Contains("obj", StringComparer.Ordinal))
            .Where(tree => Path.GetFullPath(tree.FilePath).StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .ToList();
        return new AuditBinder(root, compilation, tracked);
    }

    public static List<string> Files(
        string root, IEnumerable<string> patterns, string[] roots, string[] segments, string[] suffixes, string[] prefixes)
    {
        ProcessStartInfo info = new("git")
        {
            WorkingDirectory = root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        foreach (string argument in new[]
                 {
                     "-c", "core.quotePath=false", "ls-files", "--cached", "--others", "--exclude-standard", "--"
                 })
        {
            info.ArgumentList.Add(argument);
        }

        foreach (string pattern in patterns)
        {
            info.ArgumentList.Add(":(icase)" + pattern);
        }

        using Process process = Process.Start(info)
            ?? throw new InvalidOperationException("Git could not be started to enumerate source files.");
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Git failed to enumerate source files.\n{error}");
        }

        List<string> files = [];
        foreach (string line in output.Split('\n'))
        {
            string relative = line.Trim();
            if (relative.Length == 0 || Excluded(relative, roots, segments, suffixes, prefixes))
            {
                continue;
            }

            string full = Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(full))
            {
                files.Add(full);
            }
        }

        files.Sort(StringComparer.OrdinalIgnoreCase);
        return files;
    }

    private static bool Excluded(string relative, string[] roots, string[] segments, string[] suffixes, string[] prefixes)
    {
        string normalized = relative.Replace('\\', '/');
        if (roots.Length > 0 && !roots.Any(root =>
                normalized.StartsWith(root.Trim('/') + "/", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        string[] parts = normalized.Split('/');
        if (parts.Any(part => segments.Contains(part, StringComparer.OrdinalIgnoreCase)))
        {
            return true;
        }

        string name = parts[^1];
        return suffixes.Any(suffix => name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            || prefixes.Any(prefix => name.StartsWith(prefix, StringComparison.Ordinal));
    }

    private static string Target(string project, string output, string configuration)
    {
        string framework = XDocument.Load(project).Descendants()
            .Where(node => node.Name.LocalName == "TargetFramework")
            .Select(node => node.Value.Trim())
            .FirstOrDefault() ?? string.Empty;
        return Path.Combine(Path.GetDirectoryName(project)!, output, configuration, framework);
    }

    private static List<string> Generated(string root, IEnumerable<string> projects, string configuration)
    {
        List<string> files = [];
        foreach (string project in projects)
        {
            string folder = Target(project, "obj", configuration);
            bool markup = Directory.EnumerateFiles(Path.GetDirectoryName(project)!, "*.xaml", SearchOption.AllDirectories)
                .Any(path => !Path.GetRelativePath(root, path).Replace('\\', '/').Split('/')
                    .Contains("obj", StringComparer.Ordinal));
            if (markup && !Directory.Exists(folder))
            {
                throw new InvalidOperationException(
                    $"No generated markup code under {folder}; build the solution before the audit.");
            }

            if (!Directory.Exists(folder))
            {
                continue;
            }

            files.AddRange(Directory.EnumerateFiles(folder, "*.g.cs", SearchOption.AllDirectories)
                .Where(path => !Path.GetFileName(path).Contains("_wpftmp", StringComparison.Ordinal)));
        }

        return files;
    }

    private static List<MetadataReference> References(
        string root, string[] packs, string reference, string configuration, string owned)
    {
        Dictionary<string, (Version Version, string Path)> chosen = new(StringComparer.OrdinalIgnoreCase);
        string runtime = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        string shared = Path.GetDirectoryName(Path.GetDirectoryName(runtime)!)!;
        foreach (string pack in packs)
        {
            string folder = Path.Combine(shared, pack, Path.GetFileName(runtime));
            if (!Directory.Exists(folder))
            {
                throw new InvalidOperationException(
                    $"The shared framework '{pack}' is not installed beside the binder runtime at {folder}.");
            }

            foreach (string path in Directory.EnumerateFiles(folder, "*.dll"))
            {
                Choose(chosen, path);
            }
        }

        string project = Path.Combine(
            root, reference.Replace('/', Path.DirectorySeparatorChar), Path.GetFileName(reference) + ".csproj");
        string built = Target(project, "bin", configuration);
        if (!Directory.Exists(built))
        {
            throw new InvalidOperationException($"No build output under {built}; build the solution before the audit.");
        }

        foreach (string path in Directory.EnumerateFiles(built, "*.dll"))
        {
            string name = Path.GetFileNameWithoutExtension(path);
            if (!name.Equals(owned, StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith(owned + ".", StringComparison.OrdinalIgnoreCase))
            {
                Choose(chosen, path);
            }
        }

        return chosen.Values.Select(entry => (MetadataReference)MetadataReference.CreateFromFile(entry.Path)).ToList();
    }

    private static void Choose(Dictionary<string, (Version Version, string Path)> chosen, string path)
    {
        Version version;
        try
        {
            version = AssemblyName.GetAssemblyName(path).Version ?? new Version();
        }
        catch (BadImageFormatException)
        {
            return;
        }

        string name = Path.GetFileNameWithoutExtension(path);
        if (!chosen.TryGetValue(name, out (Version Version, string Path) known) || known.Version < version)
        {
            chosen[name] = (version, path);
        }
    }

    private static string[] Strings(JsonElement config, string key)
    {
        return config.GetProperty(key).EnumerateArray().Select(item => item.GetString()!).ToArray();
    }
}
