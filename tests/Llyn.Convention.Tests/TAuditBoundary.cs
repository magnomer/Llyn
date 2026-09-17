using Xunit;

namespace Convention.Tests;

public sealed class TAuditBoundary
{
    private static readonly string[] TAuditBoundaryForbidden =
    [
        "LStateValueRead(",
        "LStateValueCreate(",
        "LStateWrittenResolve(",
        "LStateAnchorRead(",
        "LStateAnchorCreate(",
    ];

    private static readonly string[] TAuditBoundaryState =
    [
        "LState.LStateUnknown",
        "LState.LStateSpecified",
        "LState.LStateUnspecified",
    ];

    private static readonly string[] TAuditBoundaryConverter =
    [
        "PStateConverter.cs",
        "PSentenceConverter.cs",
    ];

    private const string TAuditBoundaryTimer = "CancellationTokenSource";

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
                || !path.EndsWith(".cs", StringComparison.Ordinal)
                || !chosen(Path.GetFileName(path)))
            {
                continue;
            }

            string[] lines = File.ReadAllLines(path);
            for (int index = 0; index < lines.Length; index++)
            {
                foreach (string pattern in forbidden)
                {
                    if (lines[index].Contains(pattern, StringComparison.Ordinal))
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
