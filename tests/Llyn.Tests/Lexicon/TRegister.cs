using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRegister
{
    [Fact]
    public void RegisterSave_CardCarryingPresetNames_StoresBuiltinRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TRegisterDraftBuild(engine, ["Formal", "Polite"]));

        LMeaning meaning = Assert.Single(
            TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase).TMeaningRead(entry.LEntryId));
        LRegisterArchive registers = TInterface.TRegisterArchiveCreate(workspace.TWorkspaceDatabase);

        IReadOnlyList<LRegister> attached = registers.TRegisterMeaningRead(meaning.LMeaningId);
        Assert.Equal(["Formal", "Polite"], attached.Select(row => row.LRegisterName.TStateValueShow()));
        Assert.All(attached, row => Assert.True(row.LRegisterBuiltin));
    }

    [Fact]
    public void RegisterSave_TwoPacksNamingOneName_ShareOneRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TRegisterDraftBuild(engine, ["Formal"], "word", "English"));
        engine.TEngineEntrySave(TRegisterDraftBuild(engine, ["Formal"], "palabra", "Spanish"));

        LRegisterArchive registers = TInterface.TRegisterArchiveCreate(workspace.TWorkspaceDatabase);
        LRegister formal = Assert.Single(
            registers.TRegisterRead(), row => row.LRegisterName.TStateValueShow() == "Formal");
        Assert.True(formal.LRegisterBuiltin);
        Assert.Equal(
            ["palabra", "word"],
            engine.TEngineEntryFind(formal).Select(entry => entry.LEntryHeadword).Order(StringComparer.Ordinal));
    }

    [Fact]
    public void RegisterSeed_NameTheUserWroteFirst_BecomesBuiltin()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LRegister written = engine.TEngineRegisterCreate("Polite");
        Assert.False(written.LRegisterBuiltin);

        engine.TEngineEntrySave(TRegisterDraftBuild(engine, ["Polite"]));

        LRegisterArchive registers = TInterface.TRegisterArchiveCreate(workspace.TWorkspaceDatabase);
        LRegister polite = Assert.Single(
            registers.TRegisterRead(), row => row.LRegisterName.TStateValueShow() == "Polite");
        Assert.Equal(written.LRegisterId, polite.LRegisterId);
        Assert.True(polite.LRegisterBuiltin);
    }

    [Fact]
    public void RegisterSave_CardCarryingWrittenName_StoresWrittenRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TRegisterDraftBuild(engine, ["gruff"]));

        LMeaning meaning = Assert.Single(
            TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase).TMeaningRead(entry.LEntryId));
        LRegister written = Assert.Single(
            TInterface.TRegisterArchiveCreate(workspace.TWorkspaceDatabase)
                .TRegisterMeaningRead(meaning.LMeaningId));

        Assert.Equal("gruff", written.LRegisterName.TStateValueShow());
        Assert.False(written.LRegisterBuiltin);
    }

    [Fact]
    public void RegisterSave_TwoEntriesSharingName_ReferenceOneRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TRegisterDraftBuild(engine, ["gruff"], "word"));
        engine.TEngineEntrySave(TRegisterDraftBuild(engine, ["gruff"], "term"));

        LRegisterArchive registers = TInterface.TRegisterArchiveCreate(workspace.TWorkspaceDatabase);
        Assert.Single(registers.TRegisterRead(), row => !row.LRegisterBuiltin);
    }

    [Fact]
    public void RegisterSave_TwoEntriesWritingOneName_ReferenceOneRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TRegisterDraftBuild(["gruff"], "word"));
        engine.TEngineEntrySave(TRegisterDraftBuild(["Gruff "], "term"));

        LRegisterArchive registers = TInterface.TRegisterArchiveCreate(workspace.TWorkspaceDatabase);
        Assert.Single(registers.TRegisterRead(), row => !row.LRegisterBuiltin);
    }

    [Fact]
    public void RegisterDelete_LanguagePackRow_KeepsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TRegisterDraftBuild(engine, ["Formal"]));

        LRegisterArchive registers = TInterface.TRegisterArchiveCreate(workspace.TWorkspaceDatabase);
        long packed = registers.TRegisterRead()
            .First(row => row.LRegisterName.TStateValueShow() == "Informal")
            .LRegisterId;
        registers.TRegisterDelete(packed);

        Assert.Contains(registers.TRegisterRead(), row => row.LRegisterName.TStateValueShow() == "Informal");
    }

    [Fact]
    public void RegisterFind_WorkspaceShelf_ReturnsMarkCounts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TRegisterDraftBuild(engine, ["Formal"], "word"));
        engine.TEngineEntrySave(TRegisterDraftBuild(engine, ["Formal"], "term"));
        engine.TEngineEntrySave(TRegisterDraftBuild(engine, ["gruff"], "growl"));

        IReadOnlyList<LCatalogRegister> read =
            engine.TEngineRegisterFind(string.Empty, LCatalogOrder.LCatalogOrderUsage);

        Assert.Equal(
            2,
            read.First(row => row.LCatalogRegisterStored.LRegisterName.TStateValueShow() == "Formal")
                .LCatalogRegisterUsage);
        Assert.Equal(
            0,
            read.First(row => row.LCatalogRegisterStored.LRegisterName.TStateValueShow() == "Informal")
                .LCatalogRegisterUsage);
        Assert.Equal("Formal", read[0].LCatalogRegisterStored.LRegisterName.TStateValueShow());
    }

    [Fact]
    public void RegisterFind_ByLanguageName_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TRegisterDraftBuild(engine, ["gruff"]));

        Assert.Empty(engine.TEngineRegisterFind("English", LCatalogOrder.LCatalogOrderName));
    }

    [Fact]
    public void EntryFind_ByRegister_ReturnsTheEntriesMarkedWithIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TRegisterDraftBuild(engine, ["Formal"], "word"));
        engine.TEngineEntrySave(TRegisterDraftBuild(engine, ["Polite"], "term"));

        LRegister formal = Assert.Single(
            TInterface.TRegisterArchiveCreate(workspace.TWorkspaceDatabase).TRegisterRead(),
            row => row.LRegisterName.TStateValueShow() == "Formal");

        Assert.Equal(["word"], engine.TEngineEntryFind(formal).Select(entry => entry.LEntryHeadword));
        Assert.Equal(2, engine.TEngineEntryFind(TRegisterBlankCreate()).Count);
    }

    [Fact]
    public void RegisterCreate_WordingNoCardCarries_ListsItAsWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LRegister created = engine.TEngineRegisterCreate("  gruff  ");

        Assert.True(created.LRegisterId > 0);
        Assert.False(created.LRegisterBuiltin);
        Assert.Equal("gruff", created.LRegisterName.TStateValueShow());

        LCatalogRegister listed = Assert.Single(
            engine.TEngineRegisterFind("gruff", LCatalogOrder.LCatalogOrderName));
        Assert.Equal(created.LRegisterId, listed.LCatalogRegisterStored.LRegisterId);
        Assert.Equal(0, listed.LCatalogRegisterUsage);
    }

    [Fact]
    public void RegisterCreate_WordingAlreadyOnShelf_ReturnsTheStoredRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TRegisterDraftBuild(engine, ["gruff"]));
        LRegister written = Assert.Single(
            engine.TEngineRegisterFind("gruff", "English"), row => !row.LRegisterBuiltin);

        LRegister again = engine.TEngineRegisterCreate("Gruff");

        Assert.Equal(written.LRegisterId, again.LRegisterId);
        Assert.Single(engine.TEngineRegisterFind("gruff", "English"), row => !row.LRegisterBuiltin);
    }

    [Fact]
    public void RegisterChange_WrittenRow_RenamesItOnEveryCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TRegisterDraftBuild(engine, ["gruff"]));

        LRegisterArchive registers = TInterface.TRegisterArchiveCreate(workspace.TWorkspaceDatabase);
        LRegister written = Assert.Single(registers.TRegisterRead(), row => !row.LRegisterBuiltin);

        engine.TEngineRegisterChange(written.LRegisterId, "blunt");

        Assert.Equal(
            "blunt",
            registers.TRegisterRead(written.LRegisterId)?.LRegisterName.TStateValueShow());
    }

    [Fact]
    public void RegisterDelete_LanguagePackRowOnCards_KeepsEveryMark()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TRegisterDraftBuild(engine, ["Formal"]));
        LMeaning meaning = Assert.Single(
            TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase).TMeaningRead(entry.LEntryId));

        LRegisterArchive registers = TInterface.TRegisterArchiveCreate(workspace.TWorkspaceDatabase);
        long packed = registers.TRegisterRead()
            .First(row => row.LRegisterName.TStateValueShow() == "Formal")
            .LRegisterId;
        registers.TRegisterDelete(packed, true);

        Assert.Single(registers.TRegisterMeaningRead(meaning.LMeaningId));
    }

    private static LRegister TRegisterBlankCreate()
    {
        return TInterface.TRegisterCreate(0, string.Empty);
    }

    private static LEntryDraft TRegisterDraftBuild(
        LEngine engine, IReadOnlyList<string> registers, string headword = "word", string language = "English")
    {
        List<LRegisterDraft> drafts = new(registers.Count);
        foreach (string register in registers)
        {
            LRegister? stored = engine.TEngineRegisterFind(register, language)
                .FirstOrDefault(row => string.Equals(
                    row.LRegisterName.TStateValueShow(), register, StringComparison.Ordinal));
            drafts.Add(stored is null
                ? TInterface.TRegisterDraftCreate(register)
                : TInterface.TRegisterDraftCreate(stored.LRegisterName, stored.LRegisterId));
        }

        return TRegisterDraftBuild(drafts, headword, language);
    }

    private static LEntryDraft TRegisterDraftBuild(
        IReadOnlyList<string> registers, string headword = "word")
    {
        List<LRegisterDraft> drafts = new(registers.Count);
        foreach (string register in registers)
        {
            drafts.Add(TInterface.TRegisterDraftCreate(register));
        }

        return TRegisterDraftBuild(drafts, headword, "English");
    }

    private static LEntryDraft TRegisterDraftBuild(
        IReadOnlyList<LRegisterDraft> drafts, string headword, string language)
    {
        return TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(
                    string.Empty,
                    string.Empty,
                    "a meaning",
                    [],
                    [],
                    [],
                    [],
                    [],
                    1,
                    register: drafts),
            ],
            []);
    }
}
