using System.Text;
using Xunit;

namespace Convention.Tests;

public sealed class TAuditModule
{
    private const string TAuditModuleAudit = "AUDITMODULE";

    private static readonly Lazy<IReadOnlyList<TAuditModuleFinding>> TAuditModuleFindings =
        new(() => TAuditModuleWalker.TAuditRun(TAuditModuleSetting.TAuditModules));

    [Fact]
    public void AuditModule_Configuration_ResolvesCompletely()
    {
        _ = TAuditModuleFindings.Value;
    }

    [Fact]
    public void AuditModule_Findings_HoldWithinCeiling()
    {
        IReadOnlyDictionary<string, int> counts = TAuditCountRead();
        List<string> above = counts
            .Where(pair => pair.Value > TAuditModuleSetting.TAuditModuleCeiling.GetValueOrDefault(pair.Key))
            .Select(pair => $"  {pair.Key}: {pair.Value} > "
                + TAuditModuleSetting.TAuditModuleCeiling.GetValueOrDefault(pair.Key))
            .Order(StringComparer.Ordinal)
            .ToList();
        string report = TAuditReportSave();
        Assert.True(!TAuditModuleSetting.TAuditModuleEnforced || above.Count == 0,
            TAuditConvention.TAuditReportFormat(
                TAuditModuleAudit,
                $"{above.Count} module finding pair(s) exceed their ceiling. {report}\n"
                + string.Join('\n', above)));
    }

    [Fact]
    public void AuditModule_Ceiling_MatchesFindings()
    {
        IReadOnlyDictionary<string, int> counts = TAuditCountRead();
        HashSet<string> recognized = TAuditModuleSetting.TAuditModules
            .SelectMany(row => new[]
            {
                $"Bypassing:{row.TAuditKey}",
                $"Exposing:{row.TAuditKey}",
            })
            .Concat(counts.Keys.Where(key => key.StartsWith("Cycling:", StringComparison.Ordinal)))
            .ToHashSet(StringComparer.Ordinal);
        List<string> stale = TAuditModuleSetting.TAuditModuleCeiling
            .Where(pair => !recognized.Contains(pair.Key) || counts.GetValueOrDefault(pair.Key) < pair.Value)
            .Select(pair => $"  {pair.Key}: {pair.Value} > {counts.GetValueOrDefault(pair.Key)}")
            .Order(StringComparer.Ordinal)
            .ToList();
        Assert.True(stale.Count == 0, TAuditConvention.TAuditReportFormat(
            TAuditModuleAudit,
            $"{stale.Count} module ceiling(s) are stale or unrecognized.\n" + string.Join('\n', stale)));
    }

    [Fact]
    public void AuditModule_TrackedSources_ReachWalker()
    {
        TAuditScope scope = new(
            [],
            TAuditModuleSetting.TAuditModuleInclude,
            TAuditNameSetting.TAuditExcludedSegments,
            TAuditNameSetting.TAuditExcludedSuffixes,
            TAuditNameSetting.TAuditExcludedPrefixes,
            []);
        IReadOnlyList<string> sources = TAuditSource.TAuditFileRead(TAuditBinder.TAuditRoot, scope);
        Assert.True(sources.Count > 0, TAuditConvention.TAuditReportFormat(
            TAuditModuleAudit, "No tracked C# source was enumerated; the module audit would pass vacuously."));
        TAuditBinder.TAuditCoverCheck(TAuditModuleAudit, sources);
    }

    private static IReadOnlyDictionary<string, int> TAuditCountRead()
    {
        return TAuditModuleFindings.Value
            .GroupBy(finding => finding.TAuditPair, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);
    }

    private static string TAuditReportSave()
    {
        string version = TAuditSource.TAuditVersionRead(TAuditBinder.TAuditRoot);
        string relative = string.Format(TAuditModuleSetting.TAuditModulePath, version);
        string path = Path.Combine(TAuditBinder.TAuditRoot, relative);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        StringBuilder text = new();
        text.AppendLine("# Module audit");
        text.AppendLine();
        text.AppendLine($"- Generation: {TAuditConvention.TAuditGeneration}");
        text.AppendLine($"- Announced modules: {TAuditModuleSetting.TAuditModules.Count}");
        text.AppendLine($"- Findings: {TAuditModuleFindings.Value.Count}");
        text.AppendLine();
        text.AppendLine("## Findings");
        text.AppendLine();
        foreach (TAuditModuleFinding finding in TAuditModuleFindings.Value)
        {
            text.AppendLine($"- `{finding.TAuditKey}`");
        }
        File.WriteAllText(path, text.ToString());
        return relative.Replace('\\', '/');
    }
}

