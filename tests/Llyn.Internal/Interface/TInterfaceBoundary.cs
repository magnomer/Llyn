using System.Text.RegularExpressions;

using Xunit;

namespace Llyn.Tests;

public sealed class TInterfaceBoundary
{
    private static readonly Regex TInterfaceDirectCall = new(
        @"\bL[A-Z][A-Za-z0-9_]*\s*\.\s*L[A-Z][A-Za-z0-9_]*\s*\(|\.\s*L[A-Z][A-Za-z0-9_]*\s*\(",
        RegexOptions.Compiled);

    private static readonly string[] TInterfaceProjects = ["Llyn.Internal", "Llyn.Windows"];

    private static readonly Regex TInterfaceDirectConstruct = new(
        @"\bnew\s+L[A-Z][A-Za-z0-9_]*\s*[({]",
        RegexOptions.Compiled);

    [Fact]
    public void InterfaceBoundary_ProductionCalls_GoThroughInterface()
    {
        string testsRoot = TInterfaceRootRead();
        string[] offenders = TInterfaceProjects
            .Select(project => Path.Combine(testsRoot, project))
            .SelectMany(folder => Directory.EnumerateFiles(folder, "*.cs", SearchOption.AllDirectories))
            .Where(path => !Path.GetRelativePath(testsRoot, path)
                .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Any(part => part is "Interface" or "bin" or "obj"))
            .Where(path =>
            {
                string source = File.ReadAllText(path);
                return TInterfaceDirectCall.IsMatch(source) || TInterfaceDirectConstruct.IsMatch(source);
            })
            .Select(path => Path.GetRelativePath(testsRoot, path))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            offenders.Length == 0,
            "Production operations must be relayed by an Interface folder under tests. Direct usage: " +
            string.Join(", ", offenders));
    }

    private static string TInterfaceRootRead()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            string candidate = Path.Combine(directory.FullName, "tests");
            if (TInterfaceProjects.All(project => Directory.Exists(Path.Combine(candidate, project))))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the tests folder.");
    }
}
