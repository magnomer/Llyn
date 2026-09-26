using Xunit;

namespace Convention.Tests;

public sealed class TAuditName
{
    [Fact]
    public void AuditName_AllSourceNames_ReportsNoViolation()
    {
        TAuditRegistry registry = TAuditRegistry.TAuditLoad();
        string repoRoot = TAuditSource.TAuditRootRead();
        TAuditScope scope = new(
            [],
            TAuditNameSetting.TAuditSourceInclude,
            TAuditNameSetting.TAuditExcludedSegments,
            TAuditNameSetting.TAuditExcludedSuffixes,
            TAuditNameSetting.TAuditExcludedPrefixes,
            TAuditNameSetting.TAuditSelfExcluded);
        IReadOnlyList<string> sources = TAuditSource.TAuditFileRead(repoRoot, scope);

        Assert.True(sources.Count > 0, TAuditConvention.TAuditReportFormat(
            "AUDITNAMES", "No source files were enumerated; the audit would pass vacuously."));

        IReadOnlyList<TViolation> violations = TAuditNameWalker.TAuditRun(sources, registry);

        Assert.True(violations.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITNAMES", TViolationFormat(repoRoot, violations)));
    }

    [Fact]
    public void AuditName_Exempt_MatchesSource()
    {
        TAuditRegistry registry = TAuditRegistry.TAuditLoad();
        string repoRoot = TAuditSource.TAuditRootRead();
        TAuditScope scope = new(
            [],
            TAuditNameSetting.TAuditSourceInclude,
            TAuditNameSetting.TAuditExcludedSegments,
            TAuditNameSetting.TAuditExcludedSuffixes,
            TAuditNameSetting.TAuditExcludedPrefixes,
            TAuditNameSetting.TAuditSelfExcluded);
        List<TSpecimen> specimens = TAuditNameWalker.TAuditSpecimenRead(TAuditSource.TAuditFileRead(repoRoot, scope));
        List<string> stale = registry.TAuditExempt.Keys
            .Where(name => !specimens.Any(specimen =>
                string.Equals(specimen.TSpecimenName, name, StringComparison.Ordinal)
                && registry.TAuditExemptValidate(name, specimen.TSpecimenPath)))
            .Order(StringComparer.Ordinal)
            .Select(name => $"  {name}")
            .ToList();

        Assert.True(stale.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITNAMES",
            $"{stale.Count} registry exemption(s) spare no name in the sources and must be removed:\n"
            + string.Join('\n', stale)));
    }

    private static string TViolationFormat(string repoRoot, IReadOnlyList<TViolation> violations)
    {
        IEnumerable<string> lines = violations
            .OrderBy(violation => violation.TViolationPath, StringComparer.OrdinalIgnoreCase)
            .ThenBy(violation => violation.TViolationLine)
            .Select(violation =>
            {
                string relative = Path.GetRelativePath(repoRoot, violation.TViolationPath).Replace('\\', '/');
                return $"  {relative}:{violation.TViolationLine} [{violation.TViolationKind}] " +
                       $"{violation.TViolationName} — {violation.TViolationReason}";
            });

        return $"{violations.Count} non-conforming name(s):\n{string.Join('\n', lines)}";
    }
}
