using System.IO;
using System.Net;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEnsign
{
    [Fact]
    public void EnsignMissingRead_UnaskedKeys_ReturnsEachOnce()
    {
        LEnsign ensign = TInterface.TEnsignCreate(new TUsherFake());

        string[] missing = ensign.TEnsignMissingRead(["English", "Korean", "English"], out _);

        Assert.Equal(["English", "Korean"], missing);
    }

    [Fact]
    public void EnsignPathAdd_PresentFile_KeepsPath()
    {
        TUsherFake usher = new();
        usher.TUsherPresent.Add("C:/flags/en.svg");
        LEnsign ensign = TInterface.TEnsignCreate(usher);
        string[] missing = ensign.TEnsignMissingRead(["English"], out int age);

        IReadOnlyList<LEnsignRow> kept = ensign.TEnsignPathAdd(age, missing, ["C:/flags/en.svg"]);

        Assert.Equal([TInterface.TEnsignRowCreate("English", "C:/flags/en.svg")], kept);
        Assert.Empty(ensign.TEnsignMissingRead(["English"], out _));
    }

    [Fact]
    public void EnsignPathAdd_MissingFile_RecordsNothingForKey()
    {
        LEnsign ensign = TInterface.TEnsignCreate(new TUsherFake());
        string[] missing = ensign.TEnsignMissingRead(["Korean"], out int age);

        IReadOnlyList<LEnsignRow> kept = ensign.TEnsignPathAdd(age, missing, ["C:/flags/ko.svg"]);

        Assert.Empty(kept);
        Assert.Empty(ensign.TEnsignMissingRead(["Korean"], out _));
    }

    [Fact]
    public void EnsignPathAdd_StaleAge_RecordsNothing()
    {
        TUsherFake usher = new();
        usher.TUsherPresent.Add("C:/flags/en.svg");
        LEnsign ensign = TInterface.TEnsignCreate(usher);
        string[] missing = ensign.TEnsignMissingRead(["English"], out int age);
        ensign.TEnsignClear();

        IReadOnlyList<LEnsignRow> kept = ensign.TEnsignPathAdd(age, missing, ["C:/flags/en.svg"]);

        Assert.Empty(kept);
        Assert.Equal(["English"], ensign.TEnsignMissingRead(["English"], out _));
    }

    [Fact]
    public void EnsignKeyFormat_LanguageAndVariety_JoinsWithSlash()
    {
        Assert.Equal("English/Scottish", TInterface.TEnsignKeyFormat("English", "Scottish"));
    }

    [Fact]
    public void EnsignFormat_NamedOrBlankVariety_KeysTheFlagOrNothing()
    {
        Assert.Equal("English/Scottish", TInterface.TEngineEnsignFormat("English", "Scottish"));
        Assert.Equal(string.Empty, TInterface.TEngineEnsignFormat("English", string.Empty));
    }

    [Fact]
    public async Task EnsignLoad_UnflaggedVariety_ReturnsNoRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        IReadOnlyList<LEnsignRow> kept = await engine.TEngineEnsignLoad("English", ["Nowhere"]);

        Assert.Empty(kept);
    }

    [Fact]
    public void EnsignPathDelete_BrokenFile_ForwardsOnce()
    {
        TUsherFake usher = new();
        LEnsign ensign = TInterface.TEnsignCreate(usher);

        ensign.TEnsignPathDelete("C:/flags/broken.svg", new FormatException());

        Assert.Equal(["C:/flags/broken.svg"], usher.TUsherDeleted);
    }

    [Fact]
    public void EnsignPathDelete_LockedFile_DeletesNothing()
    {
        TUsherFake usher = new();
        LEnsign ensign = TInterface.TEnsignCreate(usher);

        ensign.TEnsignPathDelete("C:/flags/locked.svg", new IOException());

        Assert.Empty(usher.TUsherDeleted);
    }

    [Fact]
    public void EnsignPathAdd_ClearedDuringStore_CommitsNothing()
    {
        TUsherFake usher = new();
        usher.TUsherPresent.Add("C:/flags/en.svg");
        LEnsign ensign = TInterface.TEnsignCreate(usher);
        string[] missing = ensign.TEnsignMissingRead(["English"], out int age);

        IReadOnlyList<LEnsignRow> kept = ensign.TEnsignClearAdd(age, missing, ["C:/flags/en.svg"]);

        Assert.Empty(kept);
    }

    [Fact]
    public void EnsignPathAdd_UnevenPaths_Throws()
    {
        LEnsign ensign = TInterface.TEnsignCreate(new TUsherFake());
        string[] missing = ensign.TEnsignMissingRead(["English", "Korean"], out int age);

        Assert.Throws<ArgumentException>(() => ensign.TEnsignPathAdd(age, missing, ["C:/flags/en.svg"]));
    }

    [Fact]
    public void EnsignPathDelete_KeptPath_AsksAgain()
    {
        TUsherFake usher = new();
        usher.TUsherPresent.Add("C:/flags/en.svg");
        LEnsign ensign = TInterface.TEnsignCreate(usher);
        string[] missing = ensign.TEnsignMissingRead(["English"], out int age);
        ensign.TEnsignPathAdd(age, missing, ["C:/flags/en.svg"]);

        ensign.TEnsignPathDelete("C:/flags/en.svg", new IOException());

        Assert.Equal(["English"], ensign.TEnsignMissingRead(["English"], out _));
    }

    [Fact]
    public async Task EnsignLoad_OverlappingFills_FetchesAndStoresEachFlagOnce()
    {
        TUsherFake usher = new();
        usher.TUsherPresent.Add("C:/flags/gb.svg");
        TLanguageFake languages = new();
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild(languages, usher));

        Task<IReadOnlyList<LEnsignRow>> first = engine.TEngineEnsignLoad("English", ["Scottish"]);
        Task<IReadOnlyList<LEnsignRow>> second = engine.TEngineEnsignLoad("English", ["Scottish"]);
        languages.TLanguageFakeFetch.SetResult("C:/flags/gb.svg");

        Assert.Equal([TInterface.TEnsignRowCreate("English/Scottish", "C:/flags/gb.svg")], await first);
        Assert.Empty(await second);
        Assert.Equal(1, languages.TLanguageFakeAsked);
    }

    [Fact]
    public async Task SettingsEnsignLoad_FlagFileFails_AnswersTheLanguages()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{ \"flag\": \"gb\" }");
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "flags"), string.Empty);
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("<svg/>", HttpStatusCode.OK));
        List<LEnsignRow> stored = [];

        IReadOnlyList<string> answered = await engine.TEngineEnsignLoad((rows, _) => () => stored.AddRange(rows));

        Assert.Contains(pack.TLanguageFixtureName, answered);
        Assert.Equal(engine.TEngineLanguageRead(), answered);
        Assert.DoesNotContain(stored, row => row.LEnsignRowKey == pack.TLanguageFixtureName);
    }

    private sealed class TLanguageFake : LLanguageVault
    {
        internal TaskCompletionSource<string?> TLanguageFakeFetch { get; } = new();

        internal int TLanguageFakeAsked { get; private set; }

        public IReadOnlyList<string> LLanguageScan() => [];

        public LLanguage LLanguageRead(string language) =>
            TInterface.TLanguageCreate([TInterface.TVarietyCreate("Scottish", "gb-sct")]);

        public bool LLanguageNameValidate(string? language) => true;

        public Task<string?> LLanguageFlagRead(string code, CancellationToken cancellation)
        {
            TLanguageFakeAsked++;
            return TLanguageFakeFetch.Task;
        }

        public string? LLanguageFlagFind(string code) => null;
    }

    private sealed class TUsherFake : LUsher
    {
        internal HashSet<string> TUsherPresent { get; } = new(StringComparer.Ordinal);

        internal List<string> TUsherDeleted { get; } = [];

        public bool LUsherPathExist(string? path) => path is not null && TUsherPresent.Contains(path);

        public void LUsherPathDelete(string path) => TUsherDeleted.Add(path);

        public bool LUsherLockCheck(Exception exception) => exception is IOException;

        public void LUsherOpen(string target) => throw new NotSupportedException();
    }
}
