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
        Assert.Equal(["English.formal", "English.polite"], attached.Select(row => row.LRegisterId));
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
        Assert.Single(registers.TRegisterRead().Where(row => !row.LRegisterBuiltin));
    }

    [Fact]
    public void RegisterDelete_LanguagePackRow_KeepsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TRegisterDraftBuild(["Formal"]));

        LRegisterArchive registers = TInterface.TRegisterArchiveCreate(workspace.TWorkspaceDatabase);
        registers.TRegisterDelete("English.informal");

        Assert.Contains(registers.TRegisterRead(), row => row.LRegisterId == "English.informal");
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
