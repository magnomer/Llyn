using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Convention.Tests;

public sealed class TAuditTruth
{
    private static readonly Lazy<IReadOnlyList<TViolation>> TAuditTruthHits = new(TAuditTruthRead);

    private static readonly Lazy<string> TAuditTruthWritten = new(TAuditReportSave);

    private readonly ITestOutputHelper _tAuditOutput;

    public TAuditTruth(ITestOutputHelper output)
    {
        _tAuditOutput = output;
    }

    [Fact]
    public void AuditTruth_ShellFields_ReachNoRequest()
    {
        TAuditTruthCheck(["Argument", "Guard"], "shell field(s) carry a value into an engine request");
    }

    [Fact]
    public void AuditTruth_ShellFields_KeepOneWriter()
    {
        TAuditTruthCheck(["Fork"], "shell field(s) are written by the engine and by the shell");
    }

    [Fact]
    public void AuditTruth_ShellSources_MutateNoLogic()
    {
        TAuditTruthCheck(["Mutation"], "shell line(s) assign a logic member");
    }

    [Fact]
    public void AuditTruth_Waiver_MatchesAHit()
    {
        HashSet<string> matched = new(TAuditTruthHits.Value.Select(TAuditWaiverFormat), StringComparer.Ordinal);
        List<string> stale = TAuditTruthSetting.TAuditTruthWaiver
            .Where(waiver => !matched.Contains(waiver))
            .Select(waiver => $"  {waiver}")
            .ToList();

        Assert.True(stale.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITTRUTH",
            $"{stale.Count} waiver line(s) match no hit.\n{string.Join('\n', stale)}"));
    }

    private void TAuditTruthCheck(IReadOnlyList<string> kinds, string summary)
    {
        HashSet<string> waived = new(TAuditTruthSetting.TAuditTruthWaiver, StringComparer.Ordinal);
        List<string> hits = TAuditTruthHits.Value
            .Where(hit => kinds.Contains(hit.TViolationKind, StringComparer.Ordinal))
            .Where(hit => !waived.Contains(TAuditWaiverFormat(hit)))
            .OrderBy(hit => hit.TViolationPath, StringComparer.Ordinal)
            .ThenBy(hit => hit.TViolationLine)
            .Select(hit =>
                $"  {hit.TViolationPath}:{hit.TViolationLine} {hit.TViolationName} {hit.TViolationReason}")
            .ToList();
        string report = TAuditTruthWritten.Value;
        _tAuditOutput.WriteLine($"AUDITTRUTH: {hits.Count} {summary}. Report: {report}");

        Assert.True(!TAuditTruthSetting.TAuditTruthEnforced || hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITTRUTH",
            $"{hits.Count} {summary}:\n{string.Join('\n', hits)}"));
    }

    private static string TAuditReportSave()
    {
        string repoRoot = TAuditSource.TAuditRootRead();
        string version = TAuditSource.TAuditVersionRead(repoRoot);
        string path = Path.Combine(repoRoot, string.Format(TAuditTruthSetting.TAuditTruthReport, version));
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        HashSet<string> waived = new(TAuditTruthSetting.TAuditTruthWaiver, StringComparer.Ordinal);
        IReadOnlyList<TViolation> hits = TAuditTruthHits.Value;
        string[] kinds = ["Argument", "Guard", "Fork", "Mutation"];

        StringBuilder text = new();
        text.AppendLine($"# Custody audit {version}");
        text.AppendLine();
        text.AppendLine($"- Generation: {TAuditConvention.TAuditGeneration}");
        text.AppendLine($"- Enforced: {TAuditTruthSetting.TAuditTruthEnforced}");
        text.AppendLine($"- Waived: {hits.Count(hit => waived.Contains(TAuditWaiverFormat(hit)))}");
        foreach (string kind in kinds)
        {
            int count = hits.Count(hit => string.Equals(hit.TViolationKind, kind, StringComparison.Ordinal));
            text.AppendLine($"- {kind}: {count}");
        }

        text.AppendLine();
        text.AppendLine("## Fields by hit count");
        text.AppendLine();
        text.AppendLine("| Field | Hits |");
        text.AppendLine("|---|---|");
        IEnumerable<IGrouping<string, TViolation>> fields = hits
            .Where(hit => !string.Equals(hit.TViolationKind, "Mutation", StringComparison.Ordinal))
            .GroupBy(hit => hit.TViolationName, StringComparer.Ordinal)
            .OrderByDescending(group => group.Count())
            .ThenBy(group => group.Key, StringComparer.Ordinal);
        foreach (IGrouping<string, TViolation> field in fields)
        {
            text.AppendLine($"| `{field.Key}` | {field.Count()} |");
        }

        foreach (string kind in kinds)
        {
            text.AppendLine();
            text.AppendLine($"## {kind}");
            text.AppendLine();
            foreach (TViolation hit in hits
                         .OrderBy(hit => hit.TViolationPath, StringComparer.Ordinal)
                         .ThenBy(hit => hit.TViolationLine))
            {
                if (!string.Equals(hit.TViolationKind, kind, StringComparison.Ordinal))
                {
                    continue;
                }

                string mark = waived.Contains(TAuditWaiverFormat(hit)) ? " (waived)" : string.Empty;
                text.AppendLine(
                    $"- `{hit.TViolationPath}:{hit.TViolationLine}` `{hit.TViolationName}` "
                    + $"{hit.TViolationReason}{mark}");
            }
        }

        File.WriteAllText(path, text.ToString());
        return Path.GetRelativePath(repoRoot, path).Replace('\\', '/');
    }

    private static string TAuditWaiverFormat(TViolation hit)
    {
        return $"{Path.GetFileName(hit.TViolationPath)}:{hit.TViolationName}:{hit.TViolationKind}";
    }

    private static IReadOnlyList<TViolation> TAuditTruthRead()
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
        Assert.True(sources.Count > 0, TAuditConvention.TAuditReportFormat(
            "AUDITTRUTH", $"No tracked file matches {string.Join(' ', TAuditTruthSetting.TAuditTruthInclude)}."));

        return TAuditTruthWalker.TAuditRun(sources)
            .Select(hit => hit with
            {
                TViolationPath = Path.GetRelativePath(repoRoot, hit.TViolationPath).Replace('\\', '/')
            })
            .ToList();
    }
}
