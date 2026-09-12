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

    [Fact]
    public void AuditRun_ShellSources_BuildNoStateValue()
    {
        string repoRoot = TAuditSource.TAuditRootRead();
        IReadOnlyList<string> sources = TAuditSource.TAuditFileRead(repoRoot);
        string shell = Path.Combine(repoRoot, "src", "Llyn.UIShell") + Path.DirectorySeparatorChar;

        List<string> hits = [];
        foreach (string path in sources)
        {
            if (!path.StartsWith(shell, StringComparison.OrdinalIgnoreCase)
                || !path.EndsWith(".cs", StringComparison.Ordinal))
            {
                continue;
            }

            string[] lines = File.ReadAllLines(path);
            for (int index = 0; index < lines.Length; index++)
            {
                foreach (string forbidden in TAuditBoundaryForbidden)
                {
                    if (lines[index].Contains(forbidden, StringComparison.Ordinal))
                    {
                        string relative = Path.GetRelativePath(repoRoot, path).Replace('\\', '/');
                        hits.Add($"  {relative}:{index + 1} {forbidden}");
                    }
                }
            }
        }

        Assert.True(
            hits.Count == 0,
            $"{hits.Count} shell line(s) resolve a state the engine owns:\n{string.Join('\n', hits)}");
    }
}
