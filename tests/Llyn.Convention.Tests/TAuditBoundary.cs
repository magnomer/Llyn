using System.Text.RegularExpressions;
using Xunit;

namespace Convention.Tests;

public sealed class TAuditBoundary
{
    private static readonly string[] TAuditBoundaryForbidden =
    [
        @"\bLStateValueRead\s*\(",
        @"\bLStateValueCreate\s*\(",
        @"\bLStateWrittenResolve\s*\(",
        @"\bLStateAnchorRead\s*\(",
        @"\bLStateAnchorCreate\s*\(",
        @"\busing\s+static\s+Llyn\.",
    ];

    private static readonly string[] TAuditBoundaryState =
    [
        @"\bLStateUnknown\b(?!\s*[:=])",
        @"\bLStateSpecified\b(?!\s*[:=])",
        @"\bLStateUnspecified\b(?!\s*[:=])",
    ];

    private static readonly string[] TAuditBoundaryConverter =
    [
        "PStateConverter.cs",
        "PSentenceConverter.cs",
    ];

    private static readonly string[] TAuditBoundaryHidden =
    [
        @"^\s*#\s*if\b",
        @"\bdynamic\b",
        @"\bType\.GetType\s*\(",
        @"\bActivator\.",
        @"\.GetMethod\s*\(",
        @"\.GetProperty\s*\(",
        @"\.GetField\s*\(",
        @"<x:Code\b",
        @"\bEnum\.(Try)?Parse\b",
        @"^\s*(global\s+)?using\s+\w+\s*=",
        @"^\s*extern\s+alias\b",
    ];

    private static readonly string[] TAuditBoundaryLoader =
    [
        "PHeadquarter.cs",
        "PLocalizationLoader.cs",
        "PThemeLoader.cs",
    ];

    private const string TAuditBoundaryReflection = @"\bSystem\.Reflection\b";

    private const string TAuditBoundaryTimer = @"\bCancellationTokenSource\b";

    [Fact]
    public void AuditBoundary_ShellSources_BuildNoStateValue()
    {
        List<string> hits = TAuditBoundaryScan(static _ => true, TAuditBoundaryForbidden);

        Assert.True(hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITBOUNDARY",
            $"{hits.Count} shell line(s) resolve a state the engine owns:\n{string.Join('\n', hits)}"));
    }

    [Fact]
    public void AuditBoundary_UIShell_ComparesNoState()
    {
        List<string> hits = TAuditBoundaryScan(
            static name => !TAuditBoundaryConverter.Contains(name, StringComparer.Ordinal),
            TAuditBoundaryState);

        Assert.True(hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITBOUNDARY",
            $"{hits.Count} shell line(s) compare a state outside the converters:\n{string.Join('\n', hits)}"));
    }

    [Fact]
    public void AuditBoundary_ShellSources_HideNothing()
    {
        List<string> hits = TAuditBoundaryScan(static _ => true, TAuditBoundaryHidden);

        Assert.True(hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITBOUNDARY",
            $"{hits.Count} shell line(s) hide code from the walkers:\n{string.Join('\n', hits)}"));
    }

    [Fact]
    public void AuditBoundary_PanelSources_ReflectNothing()
    {
        List<string> hits = TAuditBoundaryScan(
            static name => !TAuditBoundaryLoader.Contains(name, StringComparer.Ordinal),
            [TAuditBoundaryReflection]);

        Assert.True(hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITBOUNDARY",
            $"{hits.Count} shell line(s) reflect outside the loaders:\n{string.Join('\n', hits)}"));
    }

    [Fact]
    public void AuditBoundary_Tracked_SkipNoSource()
    {
        string repoRoot = TAuditSource.TAuditRootRead();
        TAuditScope scope = new([], ["src/*", "tests/*"], [], [], [], []);
        List<string> hits = TAuditSource.TAuditFileRead(repoRoot, scope)
            .Select(path => Path.GetRelativePath(repoRoot, path).Replace('\\', '/'))
            .Where(path => path.Split('/').Any(segment =>
                               TAuditNameSetting.TAuditExcludedSegments.Contains(segment, StringComparer.Ordinal))
                           || TAuditNameSetting.TAuditExcludedSuffixes.Any(suffix =>
                               !suffix.Equals(".md", StringComparison.Ordinal)
                               && path.EndsWith(suffix, StringComparison.Ordinal))
                           || TAuditNameSetting.TAuditExcludedPrefixes.Any(prefix =>
                               Path.GetFileName(path).StartsWith(prefix, StringComparison.Ordinal)))
            .Select(path => $"  {path}")
            .ToList();

        Assert.True(hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITBOUNDARY",
            $"{hits.Count} tracked file(s) sit where the audits never look:\n{string.Join('\n', hits)}"));
    }

    [Fact]
    public void AuditBoundary_LogicSources_HoldNoPanel()
    {
        string repoRoot = TAuditSource.TAuditRootRead();
        TAuditScope scope = new([], ["src/*.cs"], TAuditNameSetting.TAuditExcludedSegments, [], [], []);
        string shell = Path.Combine(repoRoot, "src", "Llyn.UIShell") + Path.DirectorySeparatorChar;
        List<string> hits = [];
        foreach (string path in TAuditSource.TAuditFileRead(repoRoot, scope))
        {
            if (path.StartsWith(shell, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string[] lines = File.ReadAllLines(path);
            for (int index = 0; index < lines.Length; index++)
            {
                if (Regex.IsMatch(lines[index], @"\b(class|struct|record)\s+PS?[A-Z]"))
                {
                    hits.Add($"  {Path.GetRelativePath(repoRoot, path).Replace('\\', '/')}:{index + 1}");
                }
            }
        }

        Assert.True(hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITBOUNDARY",
            $"{hits.Count} panel type(s) declared outside the shell:\n{string.Join('\n', hits)}"));
    }

    [Fact]
    public void AuditBoundary_HoldSources_KeepNoTimer()
    {
        List<string> hits = TAuditBoundaryScan(
            static name => name.EndsWith("Hold.cs", StringComparison.Ordinal)
                || name.StartsWith("PEditor", StringComparison.Ordinal),
            [TAuditBoundaryTimer]);

        Assert.True(hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITBOUNDARY",
            $"{hits.Count} hold line(s) keep a timer the tenure owns:\n{string.Join('\n', hits)}"));
    }

    private static List<string> TAuditBoundaryScan(Func<string, bool> chosen, IReadOnlyList<string> forbidden)
    {
        string repoRoot = TAuditSource.TAuditRootRead();
        TAuditScope scope = new(
            [],
            TAuditNameSetting.TAuditSourceInclude,
            TAuditNameSetting.TAuditExcludedSegments,
            TAuditNameSetting.TAuditExcludedSuffixes,
            TAuditNameSetting.TAuditExcludedPrefixes,
            []);
        IReadOnlyList<string> sources = TAuditSource.TAuditFileRead(repoRoot, scope);
        string shell = Path.Combine(repoRoot, "src", "Llyn.UIShell") + Path.DirectorySeparatorChar;

        List<string> hits = [];
        foreach (string path in sources)
        {
            if (!path.StartsWith(shell, StringComparison.OrdinalIgnoreCase)
                || !chosen(Path.GetFileName(path)))
            {
                continue;
            }

            string[] lines = File.ReadAllLines(path);
            for (int index = 0; index < lines.Length; index++)
            {
                foreach (string pattern in forbidden)
                {
                    if (Regex.IsMatch(lines[index], pattern))
                    {
                        string relative = Path.GetRelativePath(repoRoot, path).Replace('\\', '/');
                        hits.Add($"  {relative}:{index + 1} {pattern}");
                    }
                }
            }
        }

        return hits;
    }
}
