using System.Reflection;
using System.Text.RegularExpressions;

using Xunit;

namespace Llyn.Tests;

public sealed class TDraftCoverage
{
    [Fact]
    public void DraftCoverage_Portrait_NamesEveryProperty()
    {
        TDraftCheck("portrait", TDraftCoverageSetting.TDraftPortrait, TDraftCoverageSetting.TPortraitWaiver);
    }

    [Fact]
    public void DraftCoverage_Markup_NamesEveryProperty()
    {
        TDraftCheck("markup", TDraftCoverageSetting.TDraftMarkup, TDraftCoverageSetting.TMarkupWaiver);
    }

    [Fact]
    public void DraftCoverage_Exemplar_NamesEveryProperty()
    {
        TDraftCheck("exemplar", TDraftCoverageSetting.TDraftExemplar, TDraftCoverageSetting.TExemplarWaiver);
    }

    [Fact]
    public void DraftCoverage_Setting_MatchesTheRecords()
    {
        Dictionary<string, IReadOnlyList<string>> records = TDraftRead();
        HashSet<string> properties = new(records.Values.SelectMany(names => names), StringComparer.Ordinal);

        List<string> hits = [];
        foreach (string type in TDraftCoverageSetting.TDraftTypes)
        {
            if (!records.ContainsKey(type))
            {
                hits.Add($"  {type} is no record in Llyn.Core");
            }
        }

        foreach (string name in TDraftCoverageSetting.TDraftWaiver)
        {
            if (!properties.Contains(name))
            {
                hits.Add($"  {name} is waived but is no property of a draft record");
            }
        }

        Assert.True(
            hits.Count == 0,
            $"{hits.Count} setting line(s) name nothing in the draft records.\n{string.Join('\n', hits)}");
    }

    private static void TDraftCheck(string side, IReadOnlyList<string> include, IReadOnlyList<string> waiver)
    {
        if (TDraftCoverageSetting.TDraftTypes.Length == 0)
        {
            return;
        }

        Dictionary<string, IReadOnlyList<string>> records = TDraftRead();
        string text = TSourceRead(TSourceResolve(), include);
        HashSet<string> shared = new(TDraftCoverageSetting.TDraftWaiver, StringComparer.Ordinal);
        HashSet<string> waived = new(waiver, StringComparer.Ordinal);
        HashSet<string> properties = new(StringComparer.Ordinal);

        List<string> missing = [];
        List<string> stale = [];
        foreach ((string type, IReadOnlyList<string> names) in records)
        {
            foreach (string name in names)
            {
                properties.Add(name);
                if (shared.Contains(name))
                {
                    continue;
                }

                bool named = TSourceMatch(text, name);
                if (waived.Contains(name))
                {
                    if (named)
                    {
                        stale.Add($"  {name} is waived for the {side} but the {side} names it");
                    }
                }
                else if (!named)
                {
                    missing.Add($"  {type}.{name}");
                }
            }
        }

        foreach (string name in waiver)
        {
            if (!properties.Contains(name))
            {
                stale.Add($"  {name} is waived for the {side} but is no property of a draft record");
            }
        }

        string files = string.Join(' ', include);
        Assert.True(
            missing.Count == 0 && stale.Count == 0,
            $"{missing.Count} draft propert(y/ies) unnamed in the {side} ({files}), "
            + $"{stale.Count} stale waiver line(s).\n"
            + string.Join('\n', missing.Concat(stale)));
    }

    private static Dictionary<string, IReadOnlyList<string>> TDraftRead()
    {
        HashSet<string> types = new(TDraftCoverageSetting.TDraftTypes, StringComparer.Ordinal);
        Dictionary<string, IReadOnlyList<string>> records = new(StringComparer.Ordinal);
        foreach (Type type in Assembly.Load("Llyn.Core").GetTypes())
        {
            bool record = type.GetProperty(
                "EqualityContract", BindingFlags.Instance | BindingFlags.NonPublic) is not null;
            if (!types.Contains(type.Name) || !record)
            {
                continue;
            }

            ConstructorInfo? primary = type.GetConstructors()
                .Where(constructor => constructor.GetParameters() is not [{ } only] || only.ParameterType != type)
                .OrderByDescending(constructor => constructor.GetParameters().Length)
                .FirstOrDefault();
            if (primary is null)
            {
                continue;
            }

            records[type.Name] = primary.GetParameters().Select(parameter => parameter.Name ?? string.Empty).ToList();
        }

        return records;
    }

    private static string TSourceResolve()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Llyn.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate Llyn.slnx above the test binaries.");
    }

    private static IReadOnlyList<string> TSourceFind(string root, IReadOnlyList<string> include)
    {
        HashSet<string> files = new(StringComparer.OrdinalIgnoreCase);
        foreach (string pattern in include)
        {
            int slash = pattern.LastIndexOf('/');
            string folder = Path.Combine(root, pattern[..slash].Replace('/', Path.DirectorySeparatorChar));
            string leaf = pattern[(slash + 1)..];
            if (!leaf.Contains('*'))
            {
                string path = Path.Combine(folder, leaf);
                if (File.Exists(path))
                {
                    files.Add(path);
                }

                continue;
            }

            if (!Directory.Exists(folder))
            {
                continue;
            }

            foreach (string path in Directory.EnumerateFiles(folder, leaf, SearchOption.AllDirectories))
            {
                bool built = Path.GetRelativePath(folder, path)
                    .Split(Path.DirectorySeparatorChar)
                    .Any(part => part is "bin" or "obj");
                if (!built)
                {
                    files.Add(path);
                }
            }
        }

        return files.Order(StringComparer.Ordinal).ToList();
    }

    private static string TSourceRead(string root, IReadOnlyList<string> include)
    {
        IReadOnlyList<string> files = TSourceFind(root, include);
        Assert.True(files.Count > 0, $"No file matches {string.Join(' ', include)}.");
        return string.Join('\n', files.Select(File.ReadAllText));
    }

    private static bool TSourceMatch(string text, string name)
    {
        return Regex.IsMatch(text, @"\b" + Regex.Escape(name) + @"\b", RegexOptions.CultureInvariant);
    }
}
