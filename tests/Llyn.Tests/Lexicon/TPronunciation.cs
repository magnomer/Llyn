using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Llyn.Tests;

public sealed class TPronunciation
{
    [Fact]
    public void EntrySave_WorkspaceAudio_StoresRelativeLoadsResolved()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string file = Path.Combine(workspace.TWorkspaceFolder, "audio", "English", "word.mp3");
        LEntry entry = engine.TEngineEntrySave(TPronunciationDraftCreate(
            [TInterface.TPronunciationDraftCreate("/wɜːd/", "UK", file, "Wikipedia")]));

        Assert.Equal(
            Path.Combine("audio", "English", "word.mp3"),
            TPronunciationFileRead(workspace));
        LPronunciationDraft loaded = Assert.Single(engine.TEntryPronunciationRead(entry.LEntryId));
        Assert.Equal(file, loaded.LPronunciationDraftAudio);
        Assert.Equal("Wikipedia", loaded.LPronunciationDraftSource);

        engine.TEngineEntryUpdate(
            entry.LEntryId, engine.TEngineEntryLoad(entry.LEntryId)! with { LEntryDraftPronunciations = [] });
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM pronunciation_audio;"));
    }

    [Fact]
    public void NoteUpdate_CreateOrRewrite_KeepsExactlyOneNote()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TPronunciationEntryCreate(engine);
        Assert.Equal(string.Empty, engine.TEngineEntryLoad(entry.LEntryId)!.LEntryDraftNote);

        engine.TRequestEntryApply(
            entry.LEntryId, draft => TInterface.TRequestNoteCreate(draft.LDraftId, "first thoughts"));
        engine.TRequestEntryApply(
            entry.LEntryId, draft => TInterface.TRequestNoteCreate(draft.LDraftId, "second thoughts"));
        Assert.Equal("second thoughts", engine.TEngineEntryLoad(entry.LEntryId)!.LEntryDraftNote);
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM note;"));

        engine.TRequestEntryApply(
            entry.LEntryId, draft => TInterface.TRequestNoteCreate(draft.LDraftId, string.Empty));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM note;"));
    }

    [Fact]
    public void EntrySave_TwoPronunciations_KeepsOrderAndVariety()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TPronunciationDraftCreate(
            [
                TInterface.TPronunciationDraftCreate("təˈmɑːtəʊ", "UK"),
                TInterface.TPronunciationDraftCreate("təˈmeɪtoʊ", "US"),
            ]));

        Assert.Equal(
            [(0, "UK", "təˈmɑːtəʊ"), (1, "US", "təˈmeɪtoʊ")],
            engine.TEntryPronunciationRead(entry.LEntryId)
                .Select((row, index) => (index, row.LPronunciationDraftVariety, row.LPronunciationDraftIpa)));

        LEntryDraft? loaded = engine.TEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(loaded);
        Assert.Equal("təˈmɑːtəʊ", loaded.LEntryDraftIpa);
        Assert.Equal(["UK", "US"], loaded.LEntryDraftPronunciations.Select(row => row.LPronunciationDraftVariety));
    }

    [Fact]
    public void EntryUpdate_ReorderedPronunciations_KeepsIdsAndAudio()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string file = Path.Combine(workspace.TWorkspaceFolder, "audio", "English", "tomato.mp3");
        LEntry entry = engine.TEngineEntrySave(TPronunciationDraftCreate(
            [
                TInterface.TPronunciationDraftCreate("təˈmɑːtəʊ", "UK", file, "Wikipedia"),
                TInterface.TPronunciationDraftCreate("təˈmeɪtoʊ", "US"),
            ]));
        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;
        long british = loaded.LEntryDraftPronunciations[0].LPronunciationDraftId;
        long american = loaded.LEntryDraftPronunciations[1].LPronunciationDraftId;

        engine.TEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftPronunciations =
            [
                loaded.LEntryDraftPronunciations[1],
                loaded.LEntryDraftPronunciations[0] with { LPronunciationDraftIpa = "təˈmɑːtoʊ" },
            ],
        });

        Assert.Equal(
            [(american, 0, "təˈmeɪtoʊ"), (british, 1, "təˈmɑːtoʊ")],
            engine.TEntryPronunciationRead(entry.LEntryId)
                .Select((row, index) => (row.LPronunciationDraftId, index, row.LPronunciationDraftIpa)));
        Assert.Equal(
            file,
            engine.TEntryPronunciationRead(entry.LEntryId)
                .Single(row => row.LPronunciationDraftId == british).LPronunciationDraftAudio);
        Assert.Equal("təˈmeɪtoʊ", engine.TEngineEntryLoad(entry.LEntryId)!.LEntryDraftIpa);
    }

    [Fact]
    public void EntryUpdate_DroppedPronunciation_ClosesTheGap()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TPronunciationDraftCreate(
            [
                TInterface.TPronunciationDraftCreate("təˈmɑːtəʊ", "UK"),
                TInterface.TPronunciationDraftCreate("təˈmeɪtoʊ", "US"),
            ]));
        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;

        engine.TEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftPronunciations = [loaded.LEntryDraftPronunciations[1]],
        });

        LPronunciationDraft kept = Assert.Single(engine.TEntryPronunciationRead(entry.LEntryId));
        Assert.Equal("US", kept.LPronunciationDraftVariety);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM pronunciation WHERE position <> 0;"));
    }

    [Fact]
    public void EntryUpdate_StalePronunciationId_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TPronunciationDraftCreate([]));

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineEntryUpdate(
            entry.LEntryId,
            TPronunciationDraftCreate([TInterface.TPronunciationDraftCreate("wɜːd", id: 99)])));

        Assert.Equal(LRefusal.LRefusalLink, refusal.LRefusalReason);
    }

    [Fact]
    public void RequestApply_Ipa_EditsThePrimaryRowOnly()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        LDraft answered = engine.TEngineRequestApply(TInterface.TRequestIpaCreate(started.LDraftId, "wɜːd"));
        LPronunciationDraft primary = Assert.Single(answered.LDraftContent.LEntryDraftPronunciations);
        Assert.True(primary.LPronunciationDraftId < 0);

        answered = engine.TEngineRequestApply(
            TInterface.TPronunciationAdditionCreate(started.LDraftId, "wɝd", 1));
        answered = engine.TEngineRequestApply(TInterface.TRequestIpaCreate(started.LDraftId, "wɜːɖ"));

        Assert.Equal(
            [
                (primary.LPronunciationDraftId, "wɜːɖ"),
                (answered.LDraftContent.LEntryDraftPronunciations[1].LPronunciationDraftId, "wɝd"),
            ],
            answered.LDraftContent.LEntryDraftPronunciations
                .Select(row => (row.LPronunciationDraftId, row.LPronunciationDraftIpa)));
    }

    [Fact]
    public void RequestApply_RemovalOfZeroId_RemovesThePrimaryRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        engine.TEngineRequestApply(TInterface.TRequestIpaCreate(started.LDraftId, "wɜːd"));
        engine.TEngineRequestApply(TInterface.TPronunciationAdditionCreate(started.LDraftId, "wɝd", 1));
        LDraft answered = engine.TEngineRequestApply(TInterface.TPronunciationRemovalCreate(started.LDraftId, 0));

        LPronunciationDraft left = Assert.Single(answered.LDraftContent.LEntryDraftPronunciations);
        Assert.Equal("wɝd", left.LPronunciationDraftIpa);
    }

    [Fact]
    public void DraftCheck_PrimaryClearedAgain_IsNoChangeWhereAddedBlankRowIs()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        engine.TEngineRequestApply(TInterface.TRequestIpaCreate(started.LDraftId, "wɜːd"));
        Assert.True(engine.TEngineDraftCheck(started.LDraftId));
        engine.TEngineRequestApply(TInterface.TRequestIpaCreate(started.LDraftId, string.Empty));
        Assert.False(engine.TEngineDraftCheck(started.LDraftId));

        engine.TEngineRequestApply(TInterface.TPronunciationAdditionCreate(started.LDraftId, string.Empty, 1));
        Assert.True(engine.TEngineDraftCheck(started.LDraftId));
    }

    [Fact]
    public void EntrySave_AddedBlankRow_StoresItEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "word"));
        engine.TEngineRequestApply(TInterface.TPronunciationAdditionCreate(started.LDraftId, string.Empty, 0));
        LEntry entry = engine.TEngineDraftCommit(started.LDraftId);

        LPronunciationDraft kept = Assert.Single(engine.TEntryPronunciationRead(entry.LEntryId));
        Assert.True(string.IsNullOrEmpty(kept.LPronunciationDraftIpa));
    }

    [Fact]
    public void RequestApply_AdditionOnEmptyList_SeedsThePrimaryFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TPronunciationAdditionCreate(started.LDraftId, "wɝd", 1));

        Assert.Equal(
            [string.Empty, "wɝd"],
            answered.LDraftContent.LEntryDraftPronunciations.Select(row => row.LPronunciationDraftIpa));
    }

    [Fact]
    public void RequestApply_PronunciationShift_MakesTheMovedRowPrimary()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TPronunciationAdditionCreate(started.LDraftId, "təˈmɑːtəʊ", 0));
        answered = engine.TEngineRequestApply(
            TInterface.TPronunciationAdditionCreate(started.LDraftId, "təˈmeɪtoʊ", 1));
        long american = answered.LDraftContent.LEntryDraftPronunciations[1].LPronunciationDraftId;
        answered = engine.TEngineRequestApply(
            TInterface.TPronunciationVarietyCreate(started.LDraftId, american, "US"));

        answered = engine.TEngineRequestApply(
            TInterface.TPronunciationShiftCreate(started.LDraftId, american, 0));

        Assert.Equal("təˈmeɪtoʊ", answered.LDraftContent.LEntryDraftIpa);
        Assert.Equal("US", answered.LDraftContent.LEntryDraftPronunciation?.LPronunciationDraftVariety);
    }

    [Fact]
    public void PronunciationSync_VarietyRequest_StoresVariety()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TPronunciationDraftCreate(
            [TInterface.TPronunciationDraftCreate("təˈmɑːtəʊ", "UK")]));
        long stored = Assert.Single(engine.TEntryPronunciationRead(entry.LEntryId)).LPronunciationDraftId;
        LDraft started = engine.TEngineDraftStart("Input", entry.LEntryId);

        engine.TEngineRequestApply(TInterface.TPronunciationVarietyCreate(started.LDraftId, stored, "British"));
        engine.TEngineDraftCommit(started.LDraftId);

        LPronunciationDraft kept = Assert.Single(engine.TEntryPronunciationRead(entry.LEntryId));
        Assert.Equal(stored, kept.LPronunciationDraftId);
        Assert.Equal("British", kept.LPronunciationDraftVariety);
        Assert.Equal("təˈmɑːtəʊ", kept.LPronunciationDraftIpa);
    }

    [Fact]
    public void SoundMatch_VarietyOnlyChange_ReportsUpdate()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TPronunciationDraftCreate(
            [TInterface.TPronunciationDraftCreate("təˈmɑːtəʊ", "UK")]));
        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;
        long stored = loaded.LEntryDraftPronunciations[0].LPronunciationDraftId;

        engine.TEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftPronunciations =
                [loaded.LEntryDraftPronunciations[0] with { LPronunciationDraftVariety = "British" }],
        });

        Assert.Equal(
            "British", Assert.Single(engine.TEntryPronunciationRead(entry.LEntryId)).LPronunciationDraftVariety);
        long revision = Assert.IsType<long>(engine.TEngineRevisionRead());
        Assert.Contains(workspace.TRevisionChangeRead(revision), change =>
            change.LRevisionChangeKind == "update" && change.LRevisionChangeTarget == stored);
    }

    [Fact]
    public void DraftCheck_VarietyOnlyChange_ReportsChanged()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TPronunciationDraftCreate(
            [TInterface.TPronunciationDraftCreate("təˈmɑːtəʊ", "UK")]));
        long stored = Assert.Single(engine.TEntryPronunciationRead(entry.LEntryId)).LPronunciationDraftId;
        LDraft started = engine.TEngineDraftStart("Input", entry.LEntryId);
        Assert.False(engine.TEngineDraftCheck(started.LDraftId));

        engine.TEngineRequestApply(TInterface.TPronunciationVarietyCreate(started.LDraftId, stored, "British"));

        Assert.True(engine.TEngineDraftCheck(started.LDraftId));
    }

    private static LEntryDraft TPronunciationDraftCreate(IReadOnlyList<LPronunciationDraft> pronunciations)
    {
        return TInterface.TEntryDraftCreate(
            "tomato",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a red fruit", [], [], [], [], [], 1)],
            []) with
        {
            LEntryDraftPronunciations = pronunciations,
        };
    }

    private static string TPronunciationFileRead(TWorkspace workspace)
    {
        using SqliteConnection connection = workspace.TWorkspaceConnectionRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT file FROM pronunciation_audio;";
        return (string)command.ExecuteScalar()!;
    }

    private static LEntry TPronunciationEntryCreate(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)],
            [TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], [], [], 1)]));
    }
}
