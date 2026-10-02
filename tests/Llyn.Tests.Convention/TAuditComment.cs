using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace Convention.Tests;

public sealed class TAuditComment
{
    private const string TAuditMissingProblem = "no hash";

    private const string TAuditChangedProblem = "source changed";

    private const string TAuditUnstampedKind = "Unstamped";

    private static readonly Regex TAuditLiteralPattern = new(
        """"(?:"{3,})[\s\S]*?"{3,}|"""" +
        """"(?:@\$?|\$@)"(?:[^"]|"")*"|"(?:\\.|[^"\\\r\n])*"|'(?:\\.|[^'\\\r\n])'"""",
        RegexOptions.Compiled);

    private static readonly Regex TAuditSpanPattern = new("`[^`]*`", RegexOptions.Compiled);

    private static readonly Regex TAuditBulletPattern = new(@"^[-*>]\s+", RegexOptions.Compiled);

    private static readonly string[] TAuditCodeExtensions = ["", ".cs", ".xaml", ".xaml.cs"];

    private static readonly Regex TAuditHeadingPattern = new(@"^##\s+`(?<span>[^`]+)`\s*$", RegexOptions.Compiled);

    private static readonly Regex TAuditFilePattern = new(
        @"^[\w.]+\.(cs|xaml|json|csproj|props|slnx|md)$", RegexOptions.Compiled);

    private static readonly Regex TAuditGenericPattern = new(@"<[^<>]*>", RegexOptions.Compiled);

    private static readonly Regex TAuditIdentifierPattern = new(@"@?[A-Za-z_][A-Za-z0-9_]*", RegexOptions.Compiled);

    private static readonly Regex TAuditSentencePattern = new(
        "[" + Regex.Escape(string.Concat(TAuditCommentSetting.TAuditCommentMarks)) + @"]\s+\p{L}",
        RegexOptions.Compiled);

    private static readonly Regex TAuditAbbreviationPattern = new(
        @"(?<![\p{L}.])(?:"
        + string.Join('|', TAuditCommentSetting.TAuditCommentAbbreviations.Select(Regex.Escape))
        + @")\.",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex TAuditLevelPattern = new(@"^#+\s*", RegexOptions.Compiled);

    private static readonly Regex TAuditHashPattern = new("^Hash: `(?<hash>[0-9a-f]{16})`$", RegexOptions.Compiled);

    private readonly ITestOutputHelper _tAuditOutput;

    public TAuditComment(ITestOutputHelper output)
    {
        _tAuditOutput = output;
    }

    [Fact]
    public void AuditComment_CommentLines_KeepLineRules()
    {
        string repoRoot = TAuditSource.TAuditRootRead();
        List<string> hits = [];
        TAuditScope scope = new(
            TAuditCommentSetting.TAuditCommentRoots,
            [TAuditCommentSetting.TAuditCommentPattern],
            TAuditCommentSetting.TAuditCommentSegments,
            [],
            [],
            []);
        IEnumerable<string> comments = TAuditScanRead(repoRoot, scope).Concat(
            TAuditCommentSetting.TAuditCommentFiles
                .Select(file => TAuditCommentRead(Path.Combine(repoRoot, file)))
                .Where(File.Exists));
        foreach (string path in comments)
        {
            string[] lines = File.ReadAllLines(path);
            for (int index = 0; index < lines.Length; index++)
            {
                string problem = TAuditLineCheck(lines[index]);
                if (problem.Length > 0)
                {
                    string relative = Path.GetRelativePath(repoRoot, path).Replace('\\', '/');
                    hits.Add($"  {relative}:{index + 1} {problem}");
                }
            }
        }

        Assert.True(hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITCOMMENTS",
            $"{hits.Count} comment line(s) break the line rules: one sentence, "
            + $"at most {TAuditCommentSetting.TAuditCommentWords} words, "
            + $"none of {string.Join(' ', TAuditCommentSetting.TAuditCommentForbidden)}.\n"
            + string.Join('\n', hits)));
    }

    [Fact]
    public void AuditComment_Sources_CarryNoRemark()
    {
        string repoRoot = TAuditSource.TAuditRootRead();
        TAuditScope scope = new(
            TAuditCommentSetting.TAuditCommentRoots,
            TAuditCommentSetting.TAuditCommentMarkers.Keys.Select(extension => "*" + extension).ToArray(),
            TAuditCommentSetting.TAuditCommentSegments,
            TAuditCommentSetting.TAuditCommentSuffixes,
            [],
            TAuditCommentSetting.TAuditCommentExempt);
        IEnumerable<string> sources = TAuditScanRead(repoRoot, scope).Concat(
            TAuditCommentSetting.TAuditCommentFiles
                .Where(file => !TAuditCommentSetting.TAuditCommentExempt.Contains(
                    Path.GetFileName(file), StringComparer.OrdinalIgnoreCase))
                .Select(file => Path.Combine(repoRoot, file)));

        List<string> hits = [];
        foreach (string path in sources)
        {
            string extension = Path.GetExtension(path);
            if (!TAuditCommentSetting.TAuditCommentMarkers.TryGetValue(extension, out string[]? markers))
            {
                continue;
            }

            bool code = extension.Equals(".cs", StringComparison.OrdinalIgnoreCase);
            string[] lines = TAuditSourceRead(path, code);
            string relative = Path.GetRelativePath(repoRoot, path).Replace('\\', '/');
            string? open = null;
            for (int index = 0; index < lines.Length; index++)
            {
                string text = lines[index];
                if (open is not null)
                {
                    hits.Add($"  {relative}:{index + 1} {open}");
                    string end = TAuditCommentSetting.TAuditCommentClosers[open];
                    open = text.Contains(end, StringComparison.Ordinal) ? null : open;
                    continue;
                }

                string? marker = markers.FirstOrDefault(entry => text.Contains(entry, StringComparison.Ordinal));
                if (marker is null)
                {
                    continue;
                }

                hits.Add($"  {relative}:{index + 1} {marker}");
                int after = text.IndexOf(marker, StringComparison.Ordinal) + marker.Length;
                if (TAuditCommentSetting.TAuditCommentClosers.TryGetValue(marker, out string? closer)
                    && text.IndexOf(closer, after, StringComparison.Ordinal) < 0)
                {
                    open = marker;
                }
            }
        }

        Assert.True(hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITCOMMENTS",
            $"{hits.Count} in-code comment(s) found. "
            + $"Prose belongs in the {TAuditCommentSetting.TAuditCommentPattern} file beside the source.\n"
            + string.Join('\n', hits)));
    }

    [Fact]
    public void AuditComment_Sources_CarryCommentFile()
    {
        string repoRoot = TAuditSource.TAuditRootRead();
        List<string> hits = TAuditOwnerRead(repoRoot)
            .Where(path => !File.Exists(TAuditCommentRead(path)))
            .Select(path => "  " + Path.GetRelativePath(repoRoot, path).Replace('\\', '/'))
            .ToList();

        Assert.True(hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITCOMMENTS",
            $"{hits.Count} source(s) have no comment file beside them.\n" + string.Join('\n', hits)));
    }

    [Fact]
    public void AuditComment_CommentFiles_HaveSource()
    {
        string repoRoot = TAuditSource.TAuditRootRead();
        HashSet<string> expected = new(
            TAuditOwnerRead(repoRoot).Select(TAuditCommentRead), StringComparer.OrdinalIgnoreCase);
        TAuditScope scope = new(
            TAuditCommentSetting.TAuditCommentRoots,
            [TAuditCommentSetting.TAuditCommentPattern],
            TAuditCommentSetting.TAuditCommentSegments,
            [],
            [],
            []);
        List<string> hits = TAuditScanRead(repoRoot, scope)
            .Where(path => !expected.Contains(path))
            .Select(path => "  " + Path.GetRelativePath(repoRoot, path).Replace('\\', '/'))
            .ToList();

        Assert.True(hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITCOMMENTS",
            $"{hits.Count} comment file(s) have no source beside them.\n" + string.Join('\n', hits)));
    }

    private static IEnumerable<string> TAuditOwnerRead(string repoRoot)
    {
        TAuditScope scope = new(
            TAuditCommentSetting.TAuditCommentRoots,
            TAuditCommentSetting.TAuditCommentSources,
            TAuditCommentSetting.TAuditCommentSegments,
            TAuditCommentSetting.TAuditCommentSuffixes,
            [],
            []);
        return TAuditScanRead(repoRoot, scope)
            .Concat(TAuditCommentSetting.TAuditCommentFiles.Select(file => Path.Combine(repoRoot, file)));
    }

    [Fact]
    public void AuditComment_Headings_NameMembers()
    {
        string repoRoot = TAuditSource.TAuditRootRead();
        TAuditScope scope = new(
            TAuditCommentSetting.TAuditCommentRoots,
            [TAuditCommentSetting.TAuditCommentPattern],
            TAuditCommentSetting.TAuditCommentSegments,
            [],
            [],
            []);
        List<string> hits = [];
        IEnumerable<string> comments = TAuditScanRead(repoRoot, scope).Concat(
            TAuditCommentSetting.TAuditCommentFiles
                .Select(file => TAuditCommentRead(Path.Combine(repoRoot, file)))
                .Where(File.Exists));
        foreach (string comment in comments)
        {
            string stem = comment[..^TAuditCommentSetting.TAuditCommentPattern.TrimStart('*').Length];
            string[] sources = [.. TAuditCodeExtensions.Select(extension => stem + extension).Where(File.Exists)];
            if (sources.Length == 0 || TAuditCommentSetting.TAuditCommentExempt.Contains(
                    Path.GetFileName(sources[0]), StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            string text = string.Concat(sources.Select(File.ReadAllText));

            string[] lines = File.ReadAllLines(comment);
            for (int index = 0; index < lines.Length; index++)
            {
                Match heading = TAuditHeadingPattern.Match(lines[index]);
                string? name = heading.Success ? TAuditMemberRead(heading.Groups["span"].Value) : null;
                if (name is not null && !Regex.IsMatch(text, $@"\b{Regex.Escape(name)}\b"))
                {
                    string relative = Path.GetRelativePath(repoRoot, comment).Replace('\\', '/');
                    hits.Add($"  {relative}:{index + 1} {name}");
                }
            }
        }

        Assert.True(hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITCOMMENTS",
            $"{hits.Count} comment heading(s) name nothing in their source.\n" + string.Join('\n', hits)));
    }

    [Fact]
    public void AuditComment_StampedFiles_MatchSource()
    {
        List<string> hits = TAuditStampRead(TAuditSource.TAuditRootRead())
            .Where(hit => hit.EndsWith(TAuditChangedProblem, StringComparison.Ordinal))
            .ToList();

        Assert.True(hits.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITCOMMENTS",
            $"{hits.Count} comment file(s) carry a hash their source no longer matches. "
            + "Reread the prose, then restamp with scripts/stampcomment.ps1.\n"
            + string.Join('\n', hits)));
    }

    [Fact]
    public void AuditComment_UnstampedFiles_HoldWithinCeiling()
    {
        List<string> hits = TAuditStampRead(TAuditSource.TAuditRootRead())
            .Where(hit => hit.EndsWith(TAuditMissingProblem, StringComparison.Ordinal))
            .ToList();
        int ceiling = TAuditCommentSetting.TAuditCommentCeiling.GetValueOrDefault(TAuditUnstampedKind);
        if (hits.Count > 0)
        {
            _tAuditOutput.WriteLine(TAuditConvention.TAuditReportFormat(
                "AUDITCOMMENTS",
                $"WARNING (not a failure while at or below the ceiling {ceiling}): "
                + $"{hits.Count} comment file(s) carry no hash. "
                + "Reread each one, then stamp it with scripts/stampcomment.ps1."));
            _tAuditOutput.WriteLine(string.Join('\n', hits));
        }

        Assert.True(hits.Count <= ceiling, TAuditConvention.TAuditReportFormat(
            "AUDITCOMMENTS",
            $"{hits.Count} comment file(s) carry no hash, above the ceiling {ceiling}. "
            + "A new comment file is stamped when it is written.\n"
            + string.Join('\n', hits)));
    }

    [Fact]
    public void AuditComment_UnstampedCeiling_MatchesCount()
    {
        int count = TAuditStampRead(TAuditSource.TAuditRootRead())
            .Count(hit => hit.EndsWith(TAuditMissingProblem, StringComparison.Ordinal));
        int ceiling = TAuditCommentSetting.TAuditCommentCeiling.GetValueOrDefault(TAuditUnstampedKind);

        Assert.True(count >= ceiling, TAuditConvention.TAuditReportFormat(
            "AUDITCOMMENTS",
            $"The ceiling {ceiling} sits above the {count} unstamped comment file(s) and must be lowered."));
    }

    private static List<string> TAuditStampRead(string repoRoot)
    {
        List<string> hits = [];
        HashSet<string> rooted = new(
            TAuditCommentSetting.TAuditCommentFiles.Select(file => TAuditCommentRead(Path.Combine(repoRoot, file))),
            StringComparer.OrdinalIgnoreCase);
        IEnumerable<IGrouping<string, string>> pairs = TAuditOwnerRead(repoRoot)
            .GroupBy(TAuditCommentRead, StringComparer.OrdinalIgnoreCase)
            .Where(group => File.Exists(group.Key))
            .OrderBy(group => rooted.Contains(group.Key))
            .ThenBy(group => group.Key, StringComparer.OrdinalIgnoreCase);
        foreach (IGrouping<string, string> pair in pairs)
        {
            string[] owners = [.. pair.OrderBy(Path.GetFileName, StringComparer.Ordinal)];
            if (owners.Any(owner => TAuditCommentSetting.TAuditCommentExempt.Contains(
                    Path.GetFileName(owner), StringComparer.OrdinalIgnoreCase)))
            {
                continue;
            }

            string[] lines = File.ReadAllLines(pair.Key);
            Match stamp = TAuditHashPattern.Match(lines.Length >= 2 ? lines[1] : string.Empty);
            string problem = !stamp.Success
                ? TAuditMissingProblem
                : stamp.Groups["hash"].Value == TAuditHashRead(owners) ? string.Empty : TAuditChangedProblem;
            if (problem.Length > 0)
            {
                string relative = Path.GetRelativePath(repoRoot, pair.Key).Replace('\\', '/');
                hits.Add($"  {relative}:2 {problem}");
            }
        }

        return hits;
    }

    private static string TAuditHashRead(IEnumerable<string> owners)
    {
        string text = string.Concat(owners.Select(owner => File.ReadAllText(owner)
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')));
        byte[] digest = SHA256.HashData(new UTF8Encoding(false).GetBytes(text));
        return Convert.ToHexStringLower(digest)[..16];
    }

    private static IReadOnlyList<string> TAuditScanRead(string repoRoot, TAuditScope scope)
    {
        List<string> missing = TAuditCommentSetting.TAuditCommentRoots
            .Where(root => !Directory.Exists(Path.Combine(repoRoot, root)))
            .Concat(TAuditCommentSetting.TAuditCommentFiles
                .Where(file => !File.Exists(Path.Combine(repoRoot, file))))
            .ToList();
        Assert.True(missing.Count == 0, TAuditConvention.TAuditReportFormat(
            "AUDITCOMMENTS",
            $"{missing.Count} configured root(s) or file(s) do not exist: {string.Join(", ", missing)}."));

        IReadOnlyList<string> paths = TAuditSource.TAuditFileRead(repoRoot, scope);
        Assert.True(paths.Count > 0, TAuditConvention.TAuditReportFormat(
            "AUDITCOMMENTS", "No tracked file lies in the comment scope, so the audit cannot judge."));
        return paths;
    }

    private static string? TAuditMemberRead(string span)
    {
        if (span.StartsWith('<') || TAuditFilePattern.IsMatch(span))
        {
            return null;
        }

        int stop = span.IndexOfAny(['(', '=', ';', '{', ':']);
        string head = stop < 0 ? span : span[..stop];
        while (TAuditGenericPattern.IsMatch(head))
        {
            head = TAuditGenericPattern.Replace(head, string.Empty);
        }

        Match last = TAuditIdentifierPattern.Matches(head).LastOrDefault() ?? Match.Empty;
        return last.Success ? last.Value : null;
    }

    private static string TAuditCommentRead(string path)
    {
        string trimmed = path.EndsWith(".xaml.cs", StringComparison.OrdinalIgnoreCase)
            ? path[..^3]
            : Path.ChangeExtension(path, null);
        return trimmed + ".comment.md";
    }

    private static string[] TAuditSourceRead(string path, bool code)
    {
        string text = File.ReadAllText(path);
        if (code)
        {
            text = TAuditLiteralPattern.Replace(
                text,
                match => "\"\"" + new string('\n', match.Value.Count(character => character == '\n')));
        }

        return text.Split('\n');
    }

    private static string TAuditLineCheck(string line)
    {
        string text = line.Trim();
        bool heading = TAuditLevelPattern.IsMatch(text);
        text = TAuditLevelPattern.Replace(text, string.Empty);
        if (text.Length == 0)
        {
            return string.Empty;
        }

        text = TAuditBulletPattern.Replace(text, string.Empty);
        string prose = TAuditSpanPattern.Replace(text, string.Empty);
        List<string> problems = [];
        int words = (heading ? prose : text).Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
        if (words > TAuditCommentSetting.TAuditCommentWords)
        {
            problems.Add($"{words} words");
        }

        foreach (string token in TAuditCommentSetting.TAuditCommentForbidden)
        {
            if (prose.Contains(token, StringComparison.Ordinal))
            {
                problems.Add($"forbidden '{token}'");
            }
        }

        int sentences = 1 + TAuditSentencePattern.Matches(TAuditAbbreviationPattern.Replace(prose, "abbr")).Count;
        if (sentences > 1)
        {
            problems.Add($"{sentences} sentences");
        }

        return string.Join(", ", problems);
    }
}
