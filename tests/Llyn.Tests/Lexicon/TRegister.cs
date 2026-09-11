using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRegister
{
    [Fact]
    public void RegisterSave_CardCarryingPresetNames_StoresLanguagePackRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TRegisterDraftBuild(["Formal", "Polite"]));

        LMeaning meaning = Assert.Single(
            TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase).TMeaningRead(entry.LEntryId));
        LRegisterArchive registers = TInterface.TRegisterArchiveCreate(workspace.TWorkspaceDatabase);

        IReadOnlyList<LRegister> attached = registers.TRegisterMeaningRead(meaning.LMeaningId);
        Assert.Equal(["Formal", "Polite"], attached.Select(row => row.LRegisterName.TStateValueShow()));
        Assert.Equal(["formal", "polite"], attached.Select(row => row.LRegisterPackKey));
        Assert.All(attached, row => Assert.True(row.LRegisterBuiltin));
        Assert.All(attached, row => Assert.Equal("English", row.LRegisterLanguage));
    }

    [Fact]
    public void RegisterSave_CardCarryingWrittenName_StoresWrittenRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TRegisterDraftBuild(["gruff"]));

        LMeaning meaning = Assert.Single(
            TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase).TMeaningRead(entry.LEntryId));
        LRegister written = Assert.Single(
            TInterface.TRegisterArchiveCreate(workspace.TWorkspaceDatabase)
                .TRegisterMeaningRead(meaning.LMeaningId));

        Assert.Equal("gruff", written.LRegisterName.TStateValueShow());
        Assert.False(written.LRegisterBuiltin);
        Assert.Equal(string.Empty, written.LRegisterLanguage);
    }

    [Fact]
    public void RegisterSave_TwoEntriesSharingName_ReferenceOneRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TRegisterDraftBuild(["gruff"], "word"));
        engine.TEngineEntrySave(TRegisterDraftBuild(["gruff"], "term"));

        LRegisterArchive registers = TInterface.TRegisterArchiveCreate(workspace.TWorkspaceDatabase);
        Assert.Single(registers.TRegisterRead(), row => !row.LRegisterBuiltin);
    }

    [Fact]
    public void RegisterDelete_LanguagePackRow_KeepsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TRegisterDraftBuild(["Formal"]));

        LRegisterArchive registers = TInterface.TRegisterArchiveCreate(workspace.TWorkspaceDatabase);
        long packed = registers.TRegisterRead()
            .First(row => row.LRegisterPackKey == "informal")
            .LRegisterId;
        registers.TRegisterDelete(packed);

        Assert.Contains(registers.TRegisterRead(), row => row.LRegisterPackKey == "informal");
    }

    [Fact]
    public void RegisterFind_WorkspaceShelf_ReturnsMarkCounts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TRegisterDraftBuild(["Formal"], "word"));
        engine.TEngineEntrySave(TRegisterDraftBuild(["Formal"], "term"));
        engine.TEngineEntrySave(TRegisterDraftBuild(["gruff"], "growl"));

        IReadOnlyList<LCatalogRegister> read =
            engine.TEngineRegisterFind(string.Empty, LCatalogOrder.LCatalogOrderUsage);

        Assert.Equal(
            2,
            read.First(row => row.LCatalogRegisterStored.LRegisterPackKey == "formal")
                .LCatalogRegisterUsage);
        Assert.Equal(
            0,
            read.First(row => row.LCatalogRegisterStored.LRegisterPackKey == "informal")
                .LCatalogRegisterUsage);
        Assert.Equal("formal", read[0].LCatalogRegisterStored.LRegisterPackKey);
    }

    [Fact]
    public void RegisterFind_ByLanguage_AnswersThePackName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TRegisterDraftBuild(["gruff"]));

        IReadOnlyList<LCatalogRegister> read =
            engine.TEngineRegisterFind("English", LCatalogOrder.LCatalogOrderName);

        Assert.Equal(4, read.Count);
        Assert.All(read, row => Assert.True(row.LCatalogRegisterStored.LRegisterBuiltin));
    }

    [Fact]
    public void EntryFind_ByRegister_ReturnsTheEntriesMarkedWithIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TRegisterDraftBuild(["Formal"], "word"));
        engine.TEngineEntrySave(TRegisterDraftBuild(["Polite"], "term"));

        LRegister formal = Assert.Single(
            TInterface.TRegisterArchiveCreate(workspace.TWorkspaceDatabase).TRegisterRead(),
            row => row.LRegisterPackKey == "formal");

        Assert.Equal(["word"], engine.TEngineEntryFind(formal).Select(entry => entry.LEntryHeadword));
        Assert.Equal(2, engine.TEngineEntryFind(TRegisterBlankCreate()).Count);
    }

    [Fact]
    public void RegisterChange_WrittenRow_RenamesItOnEveryCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TRegisterDraftBuild(["gruff"]));

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

        LEntry entry = engine.TEngineEntrySave(TRegisterDraftBuild(["Formal"]));
        LMeaning meaning = Assert.Single(
            TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase).TMeaningRead(entry.LEntryId));

        LRegisterArchive registers = TInterface.TRegisterArchiveCreate(workspace.TWorkspaceDatabase);
        long packed = registers.TRegisterRead()
            .First(row => row.LRegisterPackKey == "formal")
            .LRegisterId;
        registers.TRegisterDelete(packed, true);

        Assert.Single(registers.TRegisterMeaningRead(meaning.LMeaningId));
    }

    private static LRegister TRegisterBlankCreate()
    {
        return TInterface.TRegisterCreate(0, string.Empty);
    }

    private static LEntryDraft TRegisterDraftBuild(
        IReadOnlyList<string> registers, string headword = "word")
    {
        List<LRegisterDraft> drafts = new(registers.Count);
        foreach (string register in registers)
        {
            drafts.Add(TInterface.TRegisterDraftCreate(register));
        }

        return TInterface.TEntryDraftCreate(
            headword,
            "English",
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
                    string.Empty,
                    [],
                    [],
                    1,
                    register: drafts),
            ],
            []);
    }
}
