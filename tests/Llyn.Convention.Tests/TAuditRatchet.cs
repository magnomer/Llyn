using System.Diagnostics;
using System.Text.RegularExpressions;
using Xunit;

namespace Convention.Tests;

public sealed class TAuditRatchet
{
    private static readonly string TAuditSettingFolder =
        $"tests/{TAuditNameSetting.TAuditProject}.Convention.Tests/";

    private static readonly string TAuditTruthPath = TAuditSettingFolder + "TAuditTruthSetting.cs";

    private static readonly string TAuditStrictPath = TAuditSettingFolder + "TAuditStrictSetting.cs";

    private static readonly string TAuditRingPath = TAuditSettingFolder + "TAuditRingSetting.cs";

    private static readonly string TAuditObjectPath = TAuditSettingFolder + "TAuditObjectSetting.cs";

    private static readonly string[] TAuditWaiverPaths =
    [
        TAuditSettingFolder + "TAuditWaiverArgument.cs",
        TAuditSettingFolder + "TAuditWaiverGuard.cs",
        TAuditSettingFolder + "TAuditWaiverField.cs",
    ];

    private static readonly Regex TAuditCeilingPattern = new(@"\[""(\w+)""\] = (\d+),", RegexOptions.Compiled);

    private static readonly Regex TAuditWaiverPattern = new(@"^\s*""([^""]+:[^""]+:\w+)"",", RegexOptions.Multiline);

    private static readonly Regex TAuditRingPattern =
        new(@"^\s*""([^"":]+:[\w.]+)"",", RegexOptions.Multiline);

    private static readonly Regex TAuditRolePattern =
        new(@"\[""(?<role>Llyn\.\w+)""\]\s*=\s*\[(?<names>[^\]]*)\]", RegexOptions.Singleline);

    private static readonly Regex TAuditQuotedPattern = new(@"""([^""]+)""", RegexOptions.Compiled);

    private static readonly Regex TAuditEnforcedPattern = new(@"Enforced = (true|false);", RegexOptions.Compiled);

    private static readonly Regex TAuditGenerationPattern = new(@"TAuditGeneration = (\d+);", RegexOptions.Compiled);

    [Fact]
    public void AuditRatchet_TruthCeiling_NeverRises()
    {
        TAuditCeilingCheck(TAuditTruthPath, TAuditTruthSetting.TAuditTruthCeiling);
    }

    [Fact]
    public void AuditRatchet_StrictCeiling_NeverRises()
    {
        TAuditCeilingCheck(TAuditStrictPath, TAuditStrictSetting.TAuditStrictCeiling);
    }

    [Fact]
    public void AuditRatchet_RingCeiling_NeverRises()
    {
        TAuditCeilingCheck(TAuditRingPath, TAuditRingSetting.TAuditRingCeiling);
    }

    [Fact]
    public void AuditRatchet_ObjectCeiling_NeverRises()
    {
        TAuditCeilingCheck(TAuditObjectPath, TAuditObjectSetting.TAuditObjectCeiling);
    }

    [Fact]
    public void AuditRatchet_RingWaiver_NeverGrows()
    {
        string? committed = TAuditCommittedRead(TAuditRingPath);
        if (committed is null || !TAuditGenerationCheck(committed))
        {
            return;
        }

        HashSet<string> known = TAuditRingPattern.Matches(TAuditBlockRead(committed, "TAuditRingWaiver"))
            .Select(match => match.Groups[1].Value)
            .ToHashSet(StringComparer.Ordinal);
        List<string> added = TAuditRingSetting.TAuditRingWaiver
            .Where(waiver => !known.Contains(waiver))
            .Select(waiver => $"  {waiver}")
            .ToList();

        Assert.True(added.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITRATCHET",
            $"{added.Count} ring waiver line(s) not in the committed settings.\n{string.Join('\n', added)}"));
    }

    [Fact]
    public void AuditRatchet_RingExempt_NeverGrows()
    {
        string? committed = TAuditCommittedRead(TAuditRingPath);
        if (committed is null || !TAuditGenerationCheck(committed))
        {
            return;
        }

        HashSet<string> known = TAuditRingPattern.Matches(TAuditBlockRead(committed, "TAuditRingExempt"))
            .Select(match => match.Groups[1].Value)
            .ToHashSet(StringComparer.Ordinal);
        List<string> added = TAuditRingSetting.TAuditRingExempt
            .Where(exempt => !known.Contains(exempt))
            .Select(exempt => $"  {exempt}")
            .ToList();

        Assert.True(added.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITRATCHET",
            $"{added.Count} ring exempt line(s) not in the committed settings.\n{string.Join('\n', added)}"));
    }

    [Fact]
    public void AuditRatchet_RingRoles_NeverShrink()
    {
        string? committed = TAuditCommittedRead(TAuditRingPath);
        if (committed is null || !TAuditGenerationCheck(committed))
        {
            return;
        }

        List<string> loosened = [];
        foreach (Match match in TAuditRolePattern.Matches(TAuditBlockRead(committed, "TAuditRingRoles")))
        {
            string role = match.Groups["role"].Value;
            string[] now = TAuditRingSetting.TAuditRingRoles.GetValueOrDefault(role, []);
            foreach (Match name in TAuditQuotedPattern.Matches(match.Groups["names"].Value))
            {
                if (!now.Contains(name.Groups[1].Value, StringComparer.Ordinal))
                {
                    loosened.Add($"  {role} may now name {name.Groups[1].Value}");
                }
            }
        }

        Assert.True(loosened.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITRATCHET",
            $"{loosened.Count} ring role(s) dropped a forbidden namespace.\n{string.Join('\n', loosened)}"));
    }

    [Fact]
    public void AuditRatchet_TruthWaiver_NeverGrows()
    {
        string? committed = TAuditCommittedRead(TAuditTruthPath);
        if (committed is null || !TAuditCeilingPattern.IsMatch(committed) || !TAuditGenerationCheck(committed))
        {
            return;
        }

        HashSet<string> known = new(StringComparer.Ordinal);
        foreach (string part in TAuditWaiverPaths)
        {
            string? text = TAuditCommittedRead(part);
            if (text is not null)
            {
                known.UnionWith(TAuditWaiverPattern.Matches(text).Select(match => match.Groups[1].Value));
            }
        }

        List<string> added = TAuditTruthSetting.TAuditTruthWaiver
            .Where(waiver => !known.Contains(waiver))
            .Select(waiver => $"  {waiver}")
            .ToList();

        Assert.True(added.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITRATCHET",
            $"{added.Count} waiver line(s) not in the committed settings.\n{string.Join('\n', added)}"));
    }

    [Fact]
    public void AuditRatchet_Enforced_NeverFlipsOff()
    {
        List<string> loosened = [];
        foreach ((string path, bool enforced) in new[]
                 {
                     (TAuditTruthPath, TAuditTruthSetting.TAuditTruthEnforced),
                     (TAuditStrictPath, TAuditStrictSetting.TAuditStrictEnforced),
                     (TAuditObjectPath, TAuditObjectSetting.TAuditObjectEnforced),
                 })
        {
            string? committed = TAuditCommittedRead(path);
            Match match = committed is null || !TAuditGenerationCheck(committed)
                ? Match.Empty
                : TAuditEnforcedPattern.Match(committed);
            if (match.Success && match.Groups[1].Value == "true" && !enforced)
            {
                loosened.Add($"  {path}");
            }
        }

        Assert.True(loosened.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITRATCHET",
            $"{loosened.Count} setting(s) switched enforcement off.\n{string.Join('\n', loosened)}"));
    }

    private static void TAuditCeilingCheck(string path, IReadOnlyDictionary<string, int> current)
    {
        string? committed = TAuditCommittedRead(path);
        if (committed is null || !TAuditGenerationCheck(committed))
        {
            return;
        }

        List<string> raised = [];
        foreach (Match match in TAuditCeilingPattern.Matches(committed))
        {
            string kind = match.Groups[1].Value;
            int before = int.Parse(match.Groups[2].Value);
            if (current.TryGetValue(kind, out int now) && now > before)
            {
                raised.Add($"  {kind}: committed {before}, now {now}");
            }
        }

        Assert.True(raised.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITRATCHET",
            $"{raised.Count} ceiling(s) raised above the committed value.\n{string.Join('\n', raised)}"));
    }

    private static string TAuditBlockRead(string committed, string name)
    {
        int start = committed.IndexOf(name + " =", StringComparison.Ordinal);
        if (start < 0)
        {
            return string.Empty;
        }

        int end = committed.IndexOf("};", start, StringComparison.Ordinal);
        int close = committed.IndexOf("];", start, StringComparison.Ordinal);
        if (close >= 0 && (end < 0 || close < end))
        {
            end = close;
        }

        return end < 0 ? committed[start..] : committed[start..end];
    }

    private static bool TAuditGenerationCheck(string committed)
    {
        Match match = TAuditGenerationPattern.Match(committed);
        return match.Success && int.Parse(match.Groups[1].Value) == TAuditConvention.TAuditGeneration;
    }

    private static string? TAuditCommittedRead(string path)
    {
        ProcessStartInfo info = new("git")
        {
            WorkingDirectory = TAuditSource.TAuditRootRead(),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        info.ArgumentList.Add("show");
        info.ArgumentList.Add($"HEAD:{path}");
        using Process? process = Process.Start(info);
        if (process is null)
        {
            return null;
        }

        string output = process.StandardOutput.ReadToEnd();
        process.StandardError.ReadToEnd();
        process.WaitForExit();
        return process.ExitCode == 0 ? output : null;
    }
}
