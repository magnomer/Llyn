using System.Text.RegularExpressions;
using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Convention.Tests;

public sealed class TAuditPlatform
{
    private const string TAuditProjectExtension = ".csproj";

    private static readonly string[] TAuditImportNames = ["Directory.Build.props", "Directory.Build.targets"];

    private static readonly string[] TAuditConfigNames = [".editorconfig", ".globalconfig"];

    private static readonly Lazy<IReadOnlyList<TAuditHit>> TAuditPlatformHits = new(TAuditPlatformRead);

    private readonly ITestOutputHelper _tAuditOutput;

    public TAuditPlatform(ITestOutputHelper output)
    {
        _tAuditOutput = output;
    }

    [Fact]
    public void AuditPlatform_Projects_HoldWithinCeiling()
    {
        List<string> over = [];
        foreach (string kind in TAuditPlatformSetting.TAuditPlatformKinds)
        {
            TAuditHit[] hits = TAuditPlatformHits.Value.Where(hit => hit.TAuditHitKind == kind).ToArray();
            int ceiling = TAuditPlatformSetting.TAuditPlatformCeiling.GetValueOrDefault(kind);
            _tAuditOutput.WriteLine($"AUDITPLATFORM {kind}: {hits.Length} hit(s), ceiling {ceiling}");
            if (hits.Length <= ceiling)
            {
                continue;
            }

            over.Add($"  {kind}: {hits.Length} hit(s), ceiling {ceiling}");
            over.AddRange(hits.Select(hit =>
                $"    {hit.TAuditHitRing} {hit.TAuditHitPath}:{hit.TAuditHitLine} {hit.TAuditHitName}"));
        }

        bool held = !TAuditPlatformSetting.TAuditPlatformEnforced || over.Count == 0;
        Assert.True(held, TAuditConvention.TAuditReportFormat(
            "AUDITPLATFORM",
            $"Platform kind(s) count above their ceiling:\n{string.Join('\n', over)}"));
    }

    [Fact]
    public void AuditPlatform_Ceiling_MatchesHits()
    {
        List<string> stale = TAuditPlatformSetting.TAuditPlatformCeiling
            .Select(pair => (pair.Key, pair.Value,
                Count: TAuditPlatformHits.Value.Count(hit => hit.TAuditHitKind == pair.Key)))
            .Where(pair => pair.Count < pair.Value)
            .Select(pair => $"  {pair.Key}: {pair.Count} hit(s), ceiling {pair.Value}")
            .ToList();

        Assert.True(stale.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITPLATFORM",
            $"{stale.Count} ceiling(s) sit above the count and must be lowered.\n{string.Join('\n', stale)}"));
    }

    private static IReadOnlyList<TAuditHit> TAuditPlatformRead()
    {
        string repoRoot = TAuditSource.TAuditRootRead();
        string root = TAuditPlatformSetting.TAuditPlatformRoot;
        TAuditScope scope = new(
            [],
            [root + "*" + TAuditProjectExtension, root + "*.cs", root + "*.xaml"],
            TAuditNameSetting.TAuditExcludedSegments,
            TAuditNameSetting.TAuditExcludedSuffixes,
            TAuditNameSetting.TAuditExcludedPrefixes,
            []);
        IReadOnlyList<string> files = TAuditSource.TAuditFileRead(repoRoot, scope);
        Dictionary<string, string> projects = files
            .Where(path => path.EndsWith(TAuditProjectExtension, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(path => Path.GetFileNameWithoutExtension(path), StringComparer.Ordinal);
        Assert.True(projects.Count > 0, TAuditConvention.TAuditReportFormat(
            "AUDITPLATFORM", "No project file was enumerated; the audit would pass vacuously."));
        string[] sources = files
            .Where(path => !path.EndsWith(TAuditProjectExtension, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        IReadOnlyDictionary<string, string> table = TAuditPlatformSetting.TAuditPlatformColumn;

        List<TAuditHit> hits = [];
        foreach ((string name, string path) in projects.OrderBy(pair => pair.Key, StringComparer.Ordinal))
        {
            if (!table.ContainsKey(name))
            {
                hits.Add(new TAuditHit(
                    TAuditRelativeRead(repoRoot, path), 0, name, "Unmapped", "", "not in the platform table"));
            }
        }

        foreach (string name in table.Keys.Where(name => !projects.ContainsKey(name)))
        {
            hits.Add(new TAuditHit("", 0, name, "Absent", "", "in the platform table but not on disk"));
        }

        foreach ((string name, string path) in projects.Where(pair => table.ContainsKey(pair.Key)))
        {
            string column = table[name];
            string relative = TAuditRelativeRead(repoRoot, path);
            XDocument document = XDocument.Load(path);
            string[] frameworks = TAuditFrameworkRead(document);
            string[] references = TAuditIncludeRead(document, "ProjectReference")
                .Select(include => Path.GetFileNameWithoutExtension(include.Replace('\\', '/').Split('/')[^1]))
                .Distinct(StringComparer.Ordinal)
                .ToArray();
            string folder = Path.GetDirectoryName(path)! + Path.DirectorySeparatorChar;
            string[] held = sources.Where(source => source.StartsWith(folder, StringComparison.Ordinal)).ToArray();
            string targets = $"targets '{string.Join(';', frameworks)}'";

            if (column == name)
            {
                if (frameworks.Length != 1 || frameworks[0] != TAuditPlatformSetting.TAuditPlatformPortable)
                {
                    hits.Add(new TAuditHit(relative, 0, name, "Framework", "", targets));
                }

                hits.AddRange(references
                    .Where(reference => TAuditWindowsCheck(reference, projects))
                    .Select(reference => new TAuditHit(
                        relative, 0, name, "Reference", reference, $"references the Windows project {reference}")));

                if (!TAuditAnalyzerCheck(repoRoot, path))
                {
                    hits.Add(new TAuditHit(
                        relative, 0, name, "Analyzer", "",
                        $"{TAuditPlatformSetting.TAuditPlatformRule} is not an error"));
                }

                hits.AddRange(TAuditPlatformSetting.TAuditPlatformProperties
                    .Where(property => TAuditValueRead(document, property).Any(value => value == "true"))
                    .Select(property => new TAuditHit(relative, 0, name, "Windows", property, $"enables {property}")));
                hits.AddRange(TAuditIncludeRead(document, "PackageReference")
                    .Where(package => TAuditPlatformSetting.TAuditPlatformPackages.Contains(package))
                    .Select(package => new TAuditHit(
                        relative, 0, name, "Windows", package, $"references the Windows package {package}")));
                hits.AddRange(held.SelectMany(source => TAuditSourceScan(repoRoot, source, name)));
            }
            else if (column.Length > 0)
            {
                string twin = TAuditPlatformSetting.TAuditPlatformTwin;
                if (frameworks.Length != 1 || !frameworks[0].StartsWith(twin, StringComparison.Ordinal))
                {
                    hits.Add(new TAuditHit(relative, 0, name, "Framework", "", targets));
                }

                hits.AddRange(references
                    .Where(reference => table.GetValueOrDefault(reference) != column)
                    .Select(reference => new TAuditHit(
                        relative, 0, name, "Column", reference,
                        $"references {reference} outside the {column} column")));

                if (held.Length == 0)
                {
                    hits.Add(new TAuditHit(relative, 0, name, "Empty", "", "the twin holds no source file"));
                }
            }
        }

        return hits;
    }

    private static IEnumerable<TAuditHit> TAuditSourceScan(string repoRoot, string source, string project)
    {
        Regex[] patterns = TAuditPlatformSetting.TAuditPlatformPatterns.Select(pattern => new Regex(pattern)).ToArray();
        string relative = TAuditRelativeRead(repoRoot, source);
        string[] lines = File.ReadAllLines(source);
        for (int index = 0; index < lines.Length; index++)
        {
            Match? match = patterns
                .Select(pattern => pattern.Match(lines[index]))
                .FirstOrDefault(found => found.Success);
            if (match is not null)
            {
                yield return new TAuditHit(
                    relative, index + 1, project, "Windows", match.Value, $"names {match.Value}");
            }
        }
    }

    private static bool TAuditWindowsCheck(string project, IReadOnlyDictionary<string, string> projects)
    {
        string column = TAuditPlatformSetting.TAuditPlatformColumn.GetValueOrDefault(project, project);
        if (column.Length > 0 && column != project)
        {
            return true;
        }

        return projects.TryGetValue(project, out string? path)
            && TAuditFrameworkRead(XDocument.Load(path))
                .Any(framework => framework.Contains("-windows", StringComparison.Ordinal));
    }

    private static bool TAuditAnalyzerCheck(string repoRoot, string project)
    {
        string rule = TAuditPlatformSetting.TAuditPlatformRule;
        Regex severity = new(@"^\s*dotnet_diagnostic\." + Regex.Escape(rule) + @"\.severity\s*=\s*(?<level>\w+)");
        foreach (string config in TAuditChainRead(repoRoot, project, TAuditConfigNames))
        {
            Match? match = File.ReadLines(config)
                .Select(line => severity.Match(line))
                .FirstOrDefault(found => found.Success);
            if (match is not null)
            {
                return match.Groups["level"].Value == "error";
            }
        }

        XDocument[] documents = TAuditChainRead(repoRoot, project, TAuditImportNames)
            .Append(project)
            .Select(path => XDocument.Load(path))
            .ToArray();
        bool listed = documents.Any(document => TAuditListCheck(document, "WarningsAsErrors", rule));
        bool spared = documents.Any(document => TAuditListCheck(document, "WarningsNotAsErrors", rule));
        bool every = documents.Any(document =>
            TAuditValueRead(document, "TreatWarningsAsErrors").Any(value => value == "true"));
        return listed || (every && !spared);
    }

    private static bool TAuditListCheck(XDocument document, string element, string rule)
    {
        return TAuditValueRead(document, element)
            .SelectMany(value => value.Split(
                [';', ',', ' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Contains(rule, StringComparer.Ordinal);
    }

    private static IEnumerable<string> TAuditChainRead(string repoRoot, string project, string[] names)
    {
        string root = Path.TrimEndingDirectorySeparator(repoRoot);
        DirectoryInfo? folder = new FileInfo(project).Directory;
        while (folder is not null)
        {
            foreach (string name in names)
            {
                string candidate = Path.Combine(folder.FullName, name);
                if (File.Exists(candidate))
                {
                    yield return candidate;
                }
            }

            string current = Path.TrimEndingDirectorySeparator(folder.FullName);
            if (string.Equals(current, root, StringComparison.OrdinalIgnoreCase))
            {
                yield break;
            }

            folder = folder.Parent;
        }
    }

    private static string[] TAuditFrameworkRead(XDocument document)
    {
        return TAuditValueRead(document, "TargetFramework")
            .Concat(TAuditValueRead(document, "TargetFrameworks"))
            .SelectMany(value => value.Split(
                ';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .ToArray();
    }

    private static IEnumerable<string> TAuditValueRead(XDocument document, string element)
    {
        return document.Descendants()
            .Where(node => node.Name.LocalName == element)
            .Select(node => node.Value.Trim());
    }

    private static IEnumerable<string> TAuditIncludeRead(XDocument document, string element)
    {
        return document.Descendants()
            .Where(node => node.Name.LocalName == element)
            .Select(node => (string?)node.Attribute("Include") ?? "")
            .Where(include => include.Length > 0);
    }

    private static string TAuditRelativeRead(string repoRoot, string path)
    {
        return Path.GetRelativePath(repoRoot, path).Replace('\\', '/');
    }
}
