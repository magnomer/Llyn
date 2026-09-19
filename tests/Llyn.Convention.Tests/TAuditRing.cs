using System.Text.RegularExpressions;
using Xunit;

namespace Convention.Tests;

public sealed class TAuditRing
{
    private const string TAuditRingSource = "src/";

    private static readonly Regex TAuditRingReference = new(
        @"<ProjectReference\s+Include=""[^""]*[\\/](?<name>[\w.]+)\.csproj""", RegexOptions.Compiled);

    private static readonly string[] TAuditRingField =
    [
        @"\bLEngine\??\s+_lEngine\b",
    ];

    private static readonly string[] TAuditRingHelper =
    [
        @"\b(LTenure|LVista|LForay)\b",
    ];

    private static readonly string[] TAuditRingAdapter =
    [
        @"\bnew L\w+(Archive|Loader)\(",
        @"\bLDatabaseSessionStart\(",
    ];

    private static readonly string[] TAuditRingRequest =
    [
        @"\bLRequest\w*\b",
    ];

    private static readonly string[] TAuditRingDatabase =
    [
        @"\b_lEngineDatabase\.",
    ];

    [Fact]
    public void AuditRing_Projects_ReferenceInward()
    {
        string repoRoot = TAuditSource.TAuditRootRead();
        TAuditScope scope = new([], [TAuditRingSource + "*.csproj"], [], [], [], []);
        Dictionary<string, string[]> found = TAuditSource.TAuditFileRead(repoRoot, scope)
            .ToDictionary(
                static path => Path.GetFileNameWithoutExtension(path), TAuditEdgeRead, StringComparer.Ordinal);

        List<string> drift = [];
        IEnumerable<string> projects = found.Keys
            .Union(TAuditRingSetting.TAuditRingEdges.Keys, StringComparer.Ordinal)
            .Order(StringComparer.Ordinal);
        foreach (string project in projects)
        {
            string[] actual = found.GetValueOrDefault(project, []);
            string[] expected = TAuditRingSetting.TAuditRingEdges.GetValueOrDefault(project, []);
            foreach (string edge in actual.Except(expected, StringComparer.Ordinal))
            {
                drift.Add($"  {project} -> {edge} is referenced but not in the ring table");
            }

            foreach (string edge in expected.Except(actual, StringComparer.Ordinal))
            {
                drift.Add($"  {project} -> {edge} is in the ring table but not referenced");
            }
        }

        Assert.True(drift.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITRING",
            $"{drift.Count} project edge(s) differ from the ring table:\n{string.Join('\n', drift)}"));
    }

    [Fact]
    public void AuditRing_Sources_UseOnlyInnerRings()
    {
        List<string> hits = TAuditRingFind()
            .Where(hit => hit.TViolationReason.Length == 0)
            .Select(hit => $"  {hit.TViolationPath}:{hit.TViolationLine} {hit.TViolationName}")
            .ToList();

        Assert.True(hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITRING",
            $"{hits.Count} source line(s) reach an outer ring without a waiver:\n{string.Join('\n', hits)}"));
    }

    [Fact]
    public void AuditRing_Waiver_MatchesSource()
    {
        HashSet<string> used = TAuditRingFind()
            .Select(hit => hit.TViolationReason)
            .Where(reason => reason.Length > 0)
            .ToHashSet(StringComparer.Ordinal);
        List<string> stale = TAuditRingSetting.TAuditRingWaiver
            .Where(row => !used.Contains(row))
            .Select(row => $"  {row}")
            .ToList();

        Assert.True(stale.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITRING",
            $"{stale.Count} waiver row(s) match no source line and must be deleted:\n{string.Join('\n', stale)}"));
    }

    [Fact]
    public void AuditRing_Veneer_HoldsEngineWithinCeiling()
    {
        TAuditRingCheck("EngineField", "Llyn.UIVeneer", TAuditRingField, "engine field(s) in the veneer");
        TAuditRingCheck("EngineHelper", "Llyn.UIVeneer", TAuditRingHelper, "engine helper(s) in the veneer");
    }

    [Fact]
    public void AuditRing_Engine_ConstructsNoAdapter()
    {
        TAuditRingCheck(
            "AdapterEngine", "Llyn.ShellEngine", TAuditRingAdapter, "adapter(s) built by the engine");
    }

    [Fact]
    public void AuditRing_Engine_ReadsNoDatabase()
    {
        Func<string, bool> engine = TAuditRoleSelect("Llyn.ShellEngine");
        List<TViolation> hits = TAuditRingScan(
            path => engine(path) && !path.EndsWith("/LEngine.cs", StringComparison.Ordinal),
            TAuditRingDatabase);
        Assert.True(hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITRING",
            $"{hits.Count} database read(s) sit outside the engine and must go through a vault:\n" + string.Join(
                '\n', hits.Select(hit => $"  {hit.TViolationPath}:{hit.TViolationLine} {hit.TViolationName}"))));
    }

    [Fact]
    public void AuditRing_Core_HoldsNoRequest()
    {
        List<TViolation> hits = TAuditRingScan(TAuditRoleSelect("Llyn.Core"), TAuditRingRequest);
        Assert.True(hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITRING",
            $"{hits.Count} request record(s) sit in Core and must live in Application:\n" + string.Join(
                '\n', hits.Select(hit => $"  {hit.TViolationPath}:{hit.TViolationLine} {hit.TViolationName}"))));
    }

    [Fact]
    public void AuditRing_Ceiling_MatchesHits()
    {
        List<string> stale = [];
        foreach ((string kind, string role, string[] patterns) in new[]
                 {
                     ("EngineField", "Llyn.UIVeneer", TAuditRingField),
                     ("EngineHelper", "Llyn.UIVeneer", TAuditRingHelper),
                     ("AdapterEngine", "Llyn.ShellEngine", TAuditRingAdapter),
                 })
        {
            int count = TAuditRingScan(TAuditRoleSelect(role), patterns).Count;
            int ceiling = TAuditRingSetting.TAuditRingCeiling.GetValueOrDefault(kind);
            if (count < ceiling)
            {
                stale.Add($"  {kind}: {count} hit(s), ceiling {ceiling}");
            }
        }

        Assert.True(stale.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITRING",
            $"{stale.Count} ceiling(s) sit above the count and must be lowered.\n{string.Join('\n', stale)}"));
    }

    private static void TAuditRingCheck(string kind, string role, string[] patterns, string summary)
    {
        List<TViolation> hits = TAuditRingScan(TAuditRoleSelect(role), patterns);
        int ceiling = TAuditRingSetting.TAuditRingCeiling.GetValueOrDefault(kind);
        Assert.True(hits.Count <= ceiling, TAuditConvention.TAuditReportFormat(
            "AUDITRING",
            $"{hits.Count} {summary}, above the ceiling of {ceiling}:\n" + string.Join(
                '\n', hits.Select(hit => $"  {hit.TViolationPath}:{hit.TViolationLine} {hit.TViolationName}"))));
    }

    private static List<TViolation> TAuditRingFind()
    {
        List<TViolation> breaks = [];
        foreach ((string role, string[] forbidden) in TAuditRingSetting.TAuditRingRoles)
        {
            string[] patterns = forbidden.Select(static ns => $@"\b{Regex.Escape(ns)}\b").ToArray();
            Dictionary<string, bool> streamed = new(StringComparer.Ordinal);
            foreach (TViolation hit in TAuditRingScan(TAuditRoleSelect(role), patterns))
            {
                string row = $"{hit.TViolationPath}:{hit.TViolationName}";
                if (TAuditRingSetting.TAuditRingExempt.Contains(row, StringComparer.Ordinal))
                {
                    continue;
                }

                if (hit.TViolationName == "System.IO" && !TAuditStreamCheck(hit.TViolationPath, streamed))
                {
                    continue;
                }

                string reason = TAuditRingSetting.TAuditRingWaiver.FirstOrDefault(
                    waiver => TAuditWaiverMatch(waiver, hit.TViolationPath, hit.TViolationName)) ?? "";
                breaks.Add(hit with { TViolationReason = reason });
            }
        }

        return breaks;
    }

    private static bool TAuditWaiverMatch(string waiver, string path, string name)
    {
        int split = waiver.LastIndexOf(':');
        string pattern = waiver[..split];
        if (!string.Equals(waiver[(split + 1)..], name, StringComparison.Ordinal))
        {
            return false;
        }

        return pattern.EndsWith("/*", StringComparison.Ordinal)
            ? path.StartsWith(pattern[..^1], StringComparison.Ordinal)
            : string.Equals(pattern, path, StringComparison.Ordinal);
    }

    private static bool TAuditStreamCheck(string path, Dictionary<string, bool> streamed)
    {
        if (!streamed.TryGetValue(path, out bool streams))
        {
            string text = File.ReadAllText(Path.Combine(TAuditSource.TAuditRootRead(), path));
            streams = TAuditRingSetting.TAuditRingStream.Any(pattern => Regex.IsMatch(text, pattern));
            streamed[path] = streams;
        }

        return streams;
    }

    private static Func<string, bool> TAuditRoleSelect(string role)
    {
        string prefix = $"{TAuditRingSource}{role}/";
        return path => path.StartsWith(prefix, StringComparison.Ordinal);
    }

    private static List<TViolation> TAuditRingScan(Func<string, bool> chosen, IReadOnlyList<string> patterns)
    {
        string repoRoot = TAuditSource.TAuditRootRead();
        TAuditScope scope = new(
            [],
            [TAuditRingSource + "*.cs"],
            TAuditNameSetting.TAuditExcludedSegments,
            TAuditNameSetting.TAuditExcludedSuffixes,
            TAuditNameSetting.TAuditExcludedPrefixes,
            []);
        IReadOnlyList<string> sources = TAuditSource.TAuditFileRead(repoRoot, scope);
        Assert.True(sources.Count > 0, TAuditConvention.TAuditReportFormat(
            "AUDITRING", "No tracked source file was enumerated; the audit would pass vacuously."));

        List<TViolation> hits = [];
        foreach (string path in sources)
        {
            string relative = Path.GetRelativePath(repoRoot, path).Replace('\\', '/');
            if (!chosen(relative))
            {
                continue;
            }

            string[] lines = File.ReadAllLines(path);
            for (int index = 0; index < lines.Length; index++)
            {
                foreach (string pattern in patterns)
                {
                    foreach (Match match in Regex.Matches(lines[index], pattern))
                    {
                        hits.Add(new TViolation(relative, index + 1, match.Value, pattern, ""));
                    }
                }
            }
        }

        return hits;
    }

    private static string[] TAuditEdgeRead(string csproj)
    {
        return TAuditRingReference.Matches(File.ReadAllText(csproj))
            .Select(match => match.Groups["name"].Value)
            .Order(StringComparer.Ordinal)
            .ToArray();
    }
}
