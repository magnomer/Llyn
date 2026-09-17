using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Convention.Tests;

public sealed class TAuditStrict
{
    private static readonly Lazy<(IReadOnlyList<TViolation> TAuditHits, IReadOnlyList<string> TAuditVeneers)>
        TAuditStrictHits = new(TAuditStrictRead);

    private static readonly Lazy<string> TAuditStrictWritten = new(TAuditReportSave);

    private readonly ITestOutputHelper _tAuditOutput;

    public TAuditStrict(ITestOutputHelper output)
    {
        _tAuditOutput = output;
    }

    [Fact]
    public void AuditStrict_VeneerFields_HoldNothing()
    {
        TAuditStrictCheck("Storage", "veneer field(s) hold state");
    }

    [Fact]
    public void AuditStrict_VeneerMembers_BranchNever()
    {
        TAuditStrictCheck("Flow", "veneer member(s) branch or compute");
    }

    [Fact]
    public void AuditStrict_ShellSources_TreatNoData()
    {
        TAuditStrictCheck("Treat", "shell line(s) compute over logic values");
    }

    private void TAuditStrictCheck(string kind, string summary)
    {
        List<TViolation> hits = TAuditStrictHits.Value.TAuditHits
            .Where(hit => string.Equals(hit.TViolationKind, kind, StringComparison.Ordinal))
            .ToList();
        string report = TAuditStrictWritten.Value;
        _tAuditOutput.WriteLine($"AUDITSTRICT {kind}: {hits.Count} {summary}. Report: {report}");

        Assert.True(!TAuditStrictSetting.TAuditStrictEnforced || hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITSTRICT",
            $"{hits.Count} {summary}. See {report}"));
    }

    private static (IReadOnlyList<TViolation> TAuditHits, IReadOnlyList<string> TAuditVeneers) TAuditStrictRead()
    {
        string repoRoot = TAuditSource.TAuditRootRead();
        TAuditScope scope = new(
            [],
            TAuditTruthSetting.TAuditTruthInclude,
            TAuditNameSetting.TAuditExcludedSegments,
            TAuditNameSetting.TAuditExcludedSuffixes,
            TAuditNameSetting.TAuditExcludedPrefixes,
            []);
        IReadOnlyList<string> sources = TAuditSource.TAuditFileRead(repoRoot, scope);
        IReadOnlyList<TViolation> hits = TAuditStrictWalker.TAuditRun(sources, out List<string> veneers)
            .Select(hit => hit with
            {
                TViolationPath = Path.GetRelativePath(repoRoot, hit.TViolationPath).Replace('\\', '/')
            })
            .OrderBy(hit => hit.TViolationPath, StringComparer.Ordinal)
            .ThenBy(hit => hit.TViolationLine)
            .ToList();
        veneers.Sort(StringComparer.Ordinal);
        return (hits, veneers);
    }

    private static string TAuditReportSave()
    {
        string repoRoot = TAuditSource.TAuditRootRead();
        (IReadOnlyList<TViolation> hits, IReadOnlyList<string> veneers) = TAuditStrictHits.Value;
        string version = TAuditSource.TAuditVersionRead(repoRoot);
        string path = Path.Combine(repoRoot, string.Format(TAuditStrictSetting.TAuditStrictReport, version));
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        StringBuilder text = new();
        text.AppendLine($"# Strict shell audit {version}");
        text.AppendLine();
        text.AppendLine($"- Generation: {TAuditConvention.TAuditGeneration}");
        text.AppendLine($"- Enforced: {TAuditStrictSetting.TAuditStrictEnforced}");
        text.AppendLine($"- Veneer classes: {veneers.Count}");
        foreach (string kind in new[] { "Storage", "Flow", "Treat" })
        {
            int count = hits.Count(hit => string.Equals(hit.TViolationKind, kind, StringComparison.Ordinal));
            text.AppendLine($"- {kind}: {count}");
        }

        text.AppendLine();
        text.AppendLine("## Veneer classes by member");
        text.AppendLine();
        text.AppendLine("| Class | Storage | Flow | Treat |");
        text.AppendLine("|---|---|---|---|");
        foreach (string veneer in veneers)
        {
            int storage = TAuditClassRead(hits, veneer, "Storage");
            int flow = TAuditClassRead(hits, veneer, "Flow");
            int treat = hits.Count(hit => string.Equals(hit.TViolationKind, "Treat", StringComparison.Ordinal)
                && Path.GetFileName(hit.TViolationPath).StartsWith(veneer, StringComparison.Ordinal));
            text.AppendLine($"| {veneer} | {storage} | {flow} | {treat} |");
        }

        foreach (string kind in new[] { "Storage", "Flow", "Treat" })
        {
            text.AppendLine();
            text.AppendLine($"## {kind}");
            text.AppendLine();
            foreach (TViolation hit in hits)
            {
                if (!string.Equals(hit.TViolationKind, kind, StringComparison.Ordinal))
                {
                    continue;
                }

                text.AppendLine(
                    $"- `{hit.TViolationPath}:{hit.TViolationLine}` `{hit.TViolationName}` {hit.TViolationReason}");
            }
        }

        File.WriteAllText(path, text.ToString());
        return Path.GetRelativePath(repoRoot, path).Replace('\\', '/');
    }

    private static int TAuditClassRead(IReadOnlyList<TViolation> hits, string type, string kind)
    {
        return hits.Count(hit => string.Equals(hit.TViolationKind, kind, StringComparison.Ordinal)
            && hit.TViolationName.StartsWith(type + ".", StringComparison.Ordinal));
    }
}
