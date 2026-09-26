using System.Globalization;
using System.Text.Json;
using Xunit;

namespace Convention.Tests;

public sealed class TAuditParity
{
    private const string TAuditParityName = "AUDITPARITY";

    private const string TAuditScriptFolder = "scripts";

    private const string TAuditScriptPattern = "audit*.json";

    [Fact]
    public void AuditParity_Scripts_ShareGeneration()
    {
        string folder = Path.Combine(TAuditSource.TAuditRootRead(), TAuditScriptFolder);
        List<string> drift = [];
        foreach (string path in Directory.EnumerateFiles(folder, TAuditScriptPattern).Order(StringComparer.Ordinal))
        {
            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path));
            if (document.RootElement.ValueKind == JsonValueKind.Object
                && document.RootElement.TryGetProperty("generation", out JsonElement generation)
                && generation.GetInt32() != TAuditConvention.TAuditGeneration)
            {
                drift.Add($"  {Path.GetFileName(path)}: generation {generation.GetInt32()}");
            }
        }

        TAuditDriftCheck(
            drift, $"audit configuration(s) sit at another generation than {TAuditConvention.TAuditGeneration}");
    }

    [Fact]
    public void AuditParity_Binder_MatchesScript()
    {
        JsonElement script = TAuditScriptRead("auditbinder");
        List<string> drift = [];
        TAuditValueMatch(drift, "project", script.GetProperty("project").GetString(), TAuditNameSetting.TAuditProject);
        TAuditValueMatch(drift, "configuration", script.GetProperty("configuration").GetString(),
            TAuditTruthSetting.TAuditConfiguration);
        TAuditValueMatch(drift, "reference", script.GetProperty("reference").GetString(),
            TAuditTruthSetting.TAuditReferenceRoot);
        TAuditListMatch(drift, "packs", TAuditListRead(script, "packs"), TAuditTruthSetting.TAuditFrameworkPacks);
        TAuditListMatch(drift, "excludeSegments", TAuditListRead(script, "excludeSegments"),
            TAuditNameSetting.TAuditExcludedSegments);
        TAuditListMatch(drift, "excludeSuffixes", TAuditListRead(script, "excludeSuffixes"),
            TAuditNameSetting.TAuditExcludedSuffixes);
        TAuditListMatch(drift, "excludePrefixes", TAuditListRead(script, "excludePrefixes"),
            TAuditNameSetting.TAuditExcludedPrefixes);
        TAuditDriftCheck(drift, "binder setting(s) differ from scripts/auditbinder.json");
    }

    [Fact]
    public void AuditParity_Lines_MatchScript()
    {
        JsonElement script = TAuditScriptRead("auditlines");
        JsonElement sources = script.GetProperty("sources");
        List<string> drift = [];
        JsonElement thresholds = script.GetProperty("thresholds");
        TAuditValueMatch(drift, "thresholds.limit", thresholds.GetProperty("limit").GetInt32(),
            TAuditLineSetting.TAuditLineLimit);
        TAuditValueMatch(drift, "thresholds.warning", thresholds.GetProperty("warning").GetInt32(),
            TAuditLineSetting.TAuditLineWarning);
        TAuditListMatch(drift, "sources.roots", TAuditListRead(sources, "roots"), TAuditLineSetting.TAuditLineRoots);
        TAuditListMatch(drift, "sources.extensions", TAuditListRead(sources, "extensions").Select(item => "*" + item),
            TAuditLineSetting.TAuditLineInclude);
        TAuditListMatch(drift, "sources.excludeSegments", TAuditListRead(sources, "excludeSegments"),
            TAuditLineSetting.TAuditLineSegments);
        TAuditMapMatch(drift, "width", TAuditTableRead(script.GetProperty("width")),
            TAuditLineSetting.TAuditWidthLimit.ToDictionary(pair => pair.Key, pair => new[] { $"{pair.Value}" }));
        TAuditDriftCheck(drift, "line setting(s) differ from scripts/auditlines.json");
    }

    [Fact]
    public void AuditParity_Comments_MatchScript()
    {
        JsonElement script = TAuditScriptRead("auditcomments");
        JsonElement sources = script.GetProperty("sources");
        JsonElement rules = script.GetProperty("rules");
        JsonElement remark = script.GetProperty("remark");
        List<string> drift = [];
        TAuditListMatch(drift, "sources.roots", TAuditListRead(sources, "roots"),
            TAuditCommentSetting.TAuditCommentRoots);
        TAuditListMatch(drift, "sources.files", TAuditListRead(sources, "files"),
            TAuditCommentSetting.TAuditCommentFiles);
        TAuditValueMatch(drift, "sources.commentPattern", sources.GetProperty("commentPattern").GetString(),
            TAuditCommentSetting.TAuditCommentPattern);
        TAuditListMatch(drift, "sources.excludeSegments", TAuditListRead(sources, "excludeSegments"),
            TAuditCommentSetting.TAuditCommentSegments);
        TAuditListMatch(drift, "sources.excludeSuffixes", TAuditListRead(sources, "excludeSuffixes"),
            TAuditCommentSetting.TAuditCommentSuffixes);
        TAuditValueMatch(drift, "rules.maxWords", rules.GetProperty("maxWords").GetInt32(),
            TAuditCommentSetting.TAuditCommentWords);
        TAuditListMatch(drift, "rules.forbidden", TAuditListRead(rules, "forbidden"),
            TAuditCommentSetting.TAuditCommentForbidden);
        TAuditListMatch(drift, "rules.sentenceMarks", TAuditListRead(rules, "sentenceMarks"),
            TAuditCommentSetting.TAuditCommentMarks);
        TAuditListMatch(drift, "rules.abbreviations", TAuditListRead(rules, "abbreviations"),
            TAuditCommentSetting.TAuditCommentAbbreviations);
        TAuditListMatch(drift, "remark.exemptFiles", TAuditListRead(remark, "exemptFiles"),
            TAuditCommentSetting.TAuditCommentExempt);
        TAuditMapMatch(drift, "remark.markers", TAuditTableRead(remark.GetProperty("markers")),
            TAuditCommentSetting.TAuditCommentMarkers);
        TAuditMapMatch(drift, "remark.closers", TAuditTableRead(remark.GetProperty("closers")),
            TAuditCommentSetting.TAuditCommentClosers.ToDictionary(pair => pair.Key, pair => new[] { pair.Value }));
        TAuditDriftCheck(drift, "comment setting(s) differ from scripts/auditcomments.json");
    }

    [Fact]
    public void AuditParity_Platform_MatchesScript()
    {
        JsonElement script = TAuditScriptRead("auditplatform");
        JsonElement windows = script.GetProperty("windows");
        JsonElement framework = script.GetProperty("framework");
        JsonElement analyzer = script.GetProperty("analyzer");
        List<string> drift = [];
        TAuditValueMatch(drift, "framework.portable", framework.GetProperty("portable").GetString(),
            TAuditPlatformSetting.TAuditPlatformPortable);
        TAuditValueMatch(drift, "framework.twin", framework.GetProperty("twin").GetString(),
            TAuditPlatformSetting.TAuditPlatformTwin);
        TAuditValueMatch(drift, "analyzer.rule", analyzer.GetProperty("rule").GetString(),
            TAuditPlatformSetting.TAuditPlatformRule);
        TAuditMapMatch(drift, "analyzer.silencers", TAuditTableRead(analyzer.GetProperty("silencers")),
            TAuditPlatformSetting.TAuditPlatformSilencers.ToDictionary(pair => pair.Key, pair => new[] { pair.Value }));
        TAuditListMatch(drift, "domain.exempt", TAuditListRead(script.GetProperty("domain"), "exempt"),
            TAuditPlatformSetting.TAuditPlatformShell);
        TAuditListMatch(drift, "windows.properties", TAuditListRead(windows, "properties"),
            TAuditPlatformSetting.TAuditPlatformProperties);
        TAuditListMatch(drift, "windows.packages", TAuditListRead(windows, "packages"),
            TAuditPlatformSetting.TAuditPlatformPackages);
        TAuditListMatch(drift, "windows.patterns", TAuditListRead(windows, "patterns"),
            TAuditPlatformSetting.TAuditPlatformPatterns);
        Dictionary<string, string[]> table = script.GetProperty("projects").GetProperty("table").EnumerateArray()
            .ToDictionary(
                row => row.GetProperty("name").GetString()!,
                row => new[]
                {
                    row.GetProperty("role").GetString() switch
                    {
                        "twin" => row.GetProperty("half").GetString()!,
                        "portable" => row.GetProperty("name").GetString()!,
                        _ => "",
                    },
                });
        TAuditMapMatch(drift, "projects.table", table,
            TAuditPlatformSetting.TAuditPlatformColumn.ToDictionary(pair => pair.Key, pair => new[] { pair.Value }));
        TAuditDriftCheck(drift, "platform setting(s) differ from scripts/auditplatform.json");
    }

    [Fact]
    public void AuditParity_Structure_MatchesScript()
    {
        JsonElement script = TAuditScriptRead("auditstructure");
        JsonElement[] rings = script.GetProperty("rings").EnumerateArray().ToArray();
        List<string> drift = [];
        TAuditMapMatch(drift, "rings.reach",
            rings.ToDictionary(ring => ring.GetProperty("name").GetString()!, ring => TAuditListRead(ring, "reach")),
            TAuditChainSetting.TAuditChainReach);
        TAuditListMatch(drift, "rings.cut",
            rings.Where(ring => ring.GetProperty("cut").GetBoolean())
                .Select(ring => ring.GetProperty("name").GetString()!),
            TAuditChainSetting.TAuditChainCut);
        JsonElement[] pure = rings.Where(ring => ring.GetProperty("pure").GetBoolean()).ToArray();
        TAuditListMatch(drift, "rings.pure", pure.Select(ring => ring.GetProperty("name").GetString()!),
            TAuditFrameSetting.TAuditFramePure);
        TAuditMapMatch(drift, "rings.frame",
            pure.ToDictionary(ring => ring.GetProperty("name").GetString()!, ring => TAuditListRead(ring, "frame")),
            TAuditFrameSetting.TAuditFrameExtra.ToDictionary(
                pair => pair.Key, pair => TAuditFrameSetting.TAuditFrameAllowed.Concat(pair.Value).ToArray()));
        TAuditListMatch(drift, "ambient", TAuditListRead(script, "ambient"), TAuditFrameSetting.TAuditFrameAmbient);
        TAuditMapMatch(drift, "ceilings", TAuditTableRead(script.GetProperty("ceilings")),
            TAuditChainSetting.TAuditChainCeiling.ToDictionary(pair => pair.Key, pair => new[] { $"{pair.Value}" }));
        TAuditMapMatch(drift, "surface", TAuditTableRead(script.GetProperty("surface")),
            TAuditChainSetting.TAuditChainSurface);
        TAuditMapMatch(drift, "floor", TAuditTableRead(script.GetProperty("floor")),
            TAuditChainSetting.TAuditChainFloor.ToDictionary(pair => pair.Key, pair => new[] { $"{pair.Value}" }));
        TAuditMapMatch(drift, "stray", TAuditTableRead(script.GetProperty("stray")),
            TAuditChainSetting.TAuditChainStray);
        TAuditMapMatch(drift, "banned", TAuditTableRead(script.GetProperty("banned")),
            TAuditChainSetting.TAuditChainBanned);
        TAuditDriftCheck(drift, "structure setting(s) differ from scripts/auditstructure.json");
    }

    [Fact]
    public void AuditParity_Object_MatchesScript()
    {
        JsonElement script = TAuditScriptRead("auditobject");
        JsonElement thresholds = script.GetProperty("thresholds");
        List<string> drift = [];
        TAuditValueMatch(drift, "thresholds.parts", thresholds.GetProperty("parts").GetInt32(),
            TAuditObjectSetting.TAuditPartLimit);
        TAuditValueMatch(drift, "thresholds.span", thresholds.GetProperty("span").GetInt32(),
            TAuditObjectSetting.TAuditSpanLimit);
        TAuditValueMatch(drift, "thresholds.hub", thresholds.GetProperty("hub").GetInt32(),
            TAuditObjectSetting.TAuditHubReach);
        TAuditValueMatch(drift, "thresholds.weave", thresholds.GetProperty("weave").GetDouble(),
            TAuditObjectSetting.TAuditWeaveLimit);
        TAuditValueMatch(drift, "thresholds.density", thresholds.GetProperty("density").GetDouble(),
            TAuditObjectSetting.TAuditDensityLimit);
        TAuditValueMatch(drift, "thresholds.lines", thresholds.GetProperty("lines").GetInt32(),
            TAuditObjectSetting.TAuditLargeLines);
        TAuditValueMatch(drift, "thresholds.members", thresholds.GetProperty("members").GetInt32(),
            TAuditObjectSetting.TAuditLargeMembers);
        TAuditValueMatch(drift, "thresholds.state", thresholds.GetProperty("state").GetInt32(),
            TAuditObjectSetting.TAuditLargeState);
        TAuditMapMatch(drift, "ceiling", TAuditTableRead(script.GetProperty("ceiling")),
            TAuditObjectSetting.TAuditObjectCeiling.ToDictionary(
                pair => pair.Key.ToLowerInvariant(), pair => new[] { $"{pair.Value}" }));
        TAuditMapMatch(drift, "parts", TAuditTableRead(script.GetProperty("parts")),
            TAuditObjectSetting.TAuditPartCeiling.ToDictionary(pair => pair.Key, pair => new[] { $"{pair.Value}" }));
        TAuditDriftCheck(drift, "object setting(s) differ from scripts/auditobject.json");
    }

    [Fact]
    public void AuditParity_Fake_MatchesScript()
    {
        JsonElement script = TAuditScriptRead("auditfake");
        List<string> drift = [];
        TAuditListMatch(drift, "implicitUsings", TAuditListRead(script, "implicitUsings"),
            TAuditFakeSetting.TAuditFakeUsing);
        TAuditListMatch(drift, "sources.tests",
            TAuditListRead(script.GetProperty("sources"), "tests").Select(root => root + "/*.cs"),
            TAuditFakeSetting.TAuditFakeInclude);
        TAuditDriftCheck(drift, "fake setting(s) differ from scripts/auditfake.json");
    }

    private static JsonElement TAuditScriptRead(string name)
    {
        string path = Path.Combine(TAuditSource.TAuditRootRead(), TAuditScriptFolder, name + ".json");
        using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path));
        return document.RootElement.Clone();
    }

    private static string[] TAuditListRead(JsonElement parent, string key)
    {
        return parent.GetProperty(key).EnumerateArray().Select(item => item.GetString()!).ToArray();
    }

    private static Dictionary<string, string[]> TAuditTableRead(JsonElement map)
    {
        return map.EnumerateObject().ToDictionary(
            item => item.Name,
            item => item.Value.ValueKind == JsonValueKind.Array
                ? item.Value.EnumerateArray().Select(entry => entry.GetString()!).ToArray()
                : [item.Value.ValueKind == JsonValueKind.String
                    ? item.Value.GetString()!
                    : item.Value.GetRawText()],
            StringComparer.Ordinal);
    }

    private static void TAuditValueMatch<TAuditValue>(
        List<string> drift, string key, TAuditValue script, TAuditValue test)
    {
        if (!EqualityComparer<TAuditValue>.Default.Equals(script, test))
        {
            drift.Add(string.Create(CultureInfo.InvariantCulture, $"  {key}: script {script}, test {test}"));
        }
    }

    private static void TAuditListMatch(
        List<string> drift, string key, IEnumerable<string> script, IEnumerable<string> test)
    {
        string[] left = script.Order(StringComparer.Ordinal).ToArray();
        string[] right = test.Order(StringComparer.Ordinal).ToArray();
        if (!left.SequenceEqual(right, StringComparer.Ordinal))
        {
            drift.Add($"  {key}: script [{string.Join(", ", left)}], test [{string.Join(", ", right)}]");
        }
    }

    private static void TAuditMapMatch(
        List<string> drift,
        string key,
        IReadOnlyDictionary<string, string[]> script,
        IReadOnlyDictionary<string, string[]> test)
    {
        foreach (string name in script.Keys.Union(test.Keys).Order(StringComparer.Ordinal))
        {
            TAuditListMatch(drift, $"{key}.{name}", script.GetValueOrDefault(name, ["(absent)"]),
                test.GetValueOrDefault(name, ["(absent)"]));
        }
    }

    private static void TAuditDriftCheck(List<string> drift, string summary)
    {
        Assert.True(drift.Count == 0, TAuditConvention.TAuditReportFormat(
            TAuditParityName, $"{drift.Count} {summary}.\n{string.Join('\n', drift)}"));
    }
}
