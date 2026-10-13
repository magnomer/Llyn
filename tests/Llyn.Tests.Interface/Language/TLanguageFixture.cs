using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TLanguageFixture : IDisposable
{
    private readonly string _tLanguageFixtureFolder;

    private TLanguageFixture(string name, string folder)
    {
        TLanguageFixtureName = name;
        _tLanguageFixtureFolder = folder;
    }

    internal string TLanguageFixtureName { get; }

    internal static TLanguageFixture TLanguageFixtureCreate(string json)
    {
        string name = "Fixture" + Guid.NewGuid().ToString("N");
        string folder = Path.Combine(AppContext.BaseDirectory, "languages", name);
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, "source.json"), json);
        return new TLanguageFixture(name, folder);
    }

    internal static LLanguage TLanguageFixtureLoad(string json)
    {
        using TLanguageFixture pack = TLanguageFixtureCreate(json);
        return TInterface.TLanguageLoad(pack.TLanguageFixtureName);
    }

    internal void TLanguageFixtureSave(string file, string json)
    {
        File.WriteAllText(Path.Combine(_tLanguageFixtureFolder, file), json);
    }

    public void Dispose()
    {
        Directory.Delete(_tLanguageFixtureFolder, true);
    }
}
