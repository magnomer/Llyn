using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Llyn.Tests;

public sealed class TPronunciation
{
    [Fact]
    public void PronunciationCreate_WholeRowLife_ReadsBackEachStep()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TPronunciationEntryCreate(engine);
        LPronunciation stored = engine.TEnginePronunciationCreate(
            TInterface.TPronunciationCreate(0, entry.LEntryId, "/wɜːd/", []));

        Assert.NotEqual(0, stored.LPronunciationId);
        Assert.Equal("/wɜːd/", Assert.Single(engine.TEnginePronunciationRead(entry.LEntryId)).LPronunciationIpa);

        engine.TEnginePronunciationUpdate(stored with { LPronunciationIpa = "/wɝd/" });
        Assert.Equal("/wɝd/", Assert.Single(engine.TEnginePronunciationRead(entry.LEntryId)).LPronunciationIpa);

        engine.TEnginePronunciationDelete(stored.LPronunciationId);
        Assert.Empty(engine.TEnginePronunciationRead(entry.LEntryId));
    }

    [Fact]
    public void AudioSave_WorkspaceFile_StoresRelativeReadsResolved()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TPronunciationEntryCreate(engine);
        LPronunciation pronunciation = engine.TEnginePronunciationCreate(
            TInterface.TPronunciationCreate(0, entry.LEntryId, "/wɜːd/", []));

        string file = Path.Combine(workspace.TWorkspaceFolder, "audio", "English", "word.mp3");
        engine.TEngineAudioSave(pronunciation.LPronunciationId, file, "Wikipedia");

        Assert.Equal(
            Path.Combine("audio", "English", "word.mp3"),
            TPronunciationFileRead(workspace));
        LPronunciationAudio? audio = engine.TEngineAudioRead(pronunciation.LPronunciationId);
        Assert.Equal(file, audio?.LPronunciationAudioFile);
        Assert.Equal("Wikipedia", audio?.LPronunciationAudioSource);

        engine.TEnginePronunciationDelete(pronunciation.LPronunciationId);
        Assert.Null(engine.TEngineAudioRead(pronunciation.LPronunciationId));
    }

    [Fact]
    public void NoteSave_CreateOrRewrite_KeepsExactlyOneNote()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TPronunciationEntryCreate(engine);
        Assert.Null(engine.TEngineNoteRead(entry.LEntryId));

        engine.TEngineNoteSave(TInterface.TNoteCreate(entry.LEntryId, "first thoughts"));
        engine.TEngineNoteSave(TInterface.TNoteCreate(entry.LEntryId, "second thoughts"));
        Assert.Equal("second thoughts", engine.TEngineNoteRead(entry.LEntryId)?.LNoteText);
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM note;"));

        engine.TEngineNoteDelete(entry.LEntryId);
        Assert.Null(engine.TEngineNoteRead(entry.LEntryId));
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
            engine.TEnginePronunciationRead(entry.LEntryId)
                .Select(row => (row.LPronunciationPosition, row.LPronunciationVariety, row.LPronunciationIpa)));

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
            engine.TEnginePronunciationRead(entry.LEntryId)
                .Select(row => (row.LPronunciationId, row.LPronunciationPosition, row.LPronunciationIpa)));
        Assert.Equal(file, engine.TEngineAudioRead(british)?.LPronunciationAudioFile);
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

        LPronunciation kept = Assert.Single(engine.TEnginePronunciationRead(entry.LEntryId));
        Assert.Equal(0, kept.LPronunciationPosition);
        Assert.Equal("US", kept.LPronunciationVariety);
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
            [(primary.LPronunciationDraftId, "wɜːɖ"), (answered.LDraftContent.LEntryDraftPronunciations[1].LPronunciationDraftId, "wɝd")],
            answered.LDraftContent.LEntryDraftPronunciations
                .Select(row => (row.LPronunciationDraftId, row.LPronunciationDraftIpa)));
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
        long stored = Assert.Single(engine.TEnginePronunciationRead(entry.LEntryId)).LPronunciationId;
        LDraft started = engine.TEngineDraftStart("Input", entry.LEntryId);

        engine.TEngineRequestApply(TInterface.TPronunciationVarietyCreate(started.LDraftId, stored, "British"));
        engine.TEngineDraftCommit(started.LDraftId);

        LPronunciation kept = Assert.Single(engine.TEnginePronunciationRead(entry.LEntryId));
        Assert.Equal(stored, kept.LPronunciationId);
        Assert.Equal("British", kept.LPronunciationVariety);
        Assert.Equal("təˈmɑːtəʊ", kept.LPronunciationIpa);
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

        Assert.Equal("British", Assert.Single(engine.TEnginePronunciationRead(entry.LEntryId)).LPronunciationVariety);
        LRevision revision = Assert.IsType<LRevision>(engine.TEngineRevisionRead());
        Assert.Contains(engine.TEngineChangeRead(revision.LRevisionId), change =>
            change.LRevisionChangeKind == "update" && change.LRevisionChangeTarget == stored);
    }

    [Fact]
    public void DraftCheck_VarietyOnlyChange_ReportsChanged()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TPronunciationDraftCreate(
            [TInterface.TPronunciationDraftCreate("təˈmɑːtəʊ", "UK")]));
        long stored = Assert.Single(engine.TEnginePronunciationRead(entry.LEntryId)).LPronunciationId;
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
