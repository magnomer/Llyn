using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTranscription
{
    [Fact]
    public void EntrySave_TwoSchemes_ReadsBackInOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TTranscriptionDraftCreate(
            [
                TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1 gong2"),
                TInterface.TTranscriptionDraftCreate("Yale", "hēung góng"),
            ]));

        Assert.Equal(
            ["Jyutping", "Yale"],
            engine.TEngineTranscriptionRead(entry.LEntryId).Select(row => row.LTranscriptionScheme));

        LEntryDraft? loaded = engine.TEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(loaded);
        Assert.Equal(
            ["hoeng1 gong2", "hēung góng"],
            loaded.LEntryDraftTranscriptions.Select(row => row.LTranscriptionDraftText));
        Assert.All(loaded.LEntryDraftTranscriptions, row => Assert.True(row.LTranscriptionDraftId > 0));
    }

    [Fact]
    public void EntryUpdate_SwappedSchemes_KeepsEveryId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TTranscriptionDraftCreate(
            [
                TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1 gong2"),
                TInterface.TTranscriptionDraftCreate("Yale", "hēung góng"),
            ]));
        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;
        long first = loaded.LEntryDraftTranscriptions[0].LTranscriptionDraftId;
        long second = loaded.LEntryDraftTranscriptions[1].LTranscriptionDraftId;

        engine.TEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftTranscriptions =
            [
                loaded.LEntryDraftTranscriptions[1] with { LTranscriptionDraftScheme = "Jyutping" },
                loaded.LEntryDraftTranscriptions[0] with { LTranscriptionDraftScheme = "Yale" },
            ],
        });

        Assert.Equal(
            [(second, "Jyutping", "hēung góng"), (first, "Yale", "hoeng1 gong2")],
            engine.TEngineTranscriptionRead(entry.LEntryId)
                .Select(row => (row.LTranscriptionId, row.LTranscriptionScheme, row.LTranscriptionText)));
    }

    [Fact]
    public void EntryUpdate_BlankText_DropsTheRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TTranscriptionDraftCreate(
            [TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1 gong2")]));
        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;

        engine.TEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftTranscriptions = [loaded.LEntryDraftTranscriptions[0] with { LTranscriptionDraftText = " " }],
        });

        Assert.Empty(engine.TEngineTranscriptionRead(entry.LEntryId));
    }

    [Fact]
    public void EntryUpdate_DoubledScheme_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TTranscriptionDraftCreate([]));

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineEntryUpdate(
            entry.LEntryId,
            TTranscriptionDraftCreate(
                [
                    TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1 gong2"),
                    TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1 gong2"),
                ])));

        Assert.Equal(LRefusal.LRefusalScheme, refusal.LRefusalReason);
    }

    [Fact]
    public void EntryUpdate_StaleTranscriptionId_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TTranscriptionDraftCreate([]));

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineEntryUpdate(
            entry.LEntryId,
            TTranscriptionDraftCreate([TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1 gong2", 99)])));

        Assert.Equal(LRefusal.LRefusalLink, refusal.LRefusalReason);
    }

    [Fact]
    public void RequestApply_TranscriptionAddition_MintsRowAndRefusesDoubledScheme()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TTranscriptionAdditionCreate(started.LDraftId, "Jyutping", 0));
        LTranscriptionDraft added = Assert.Single(answered.LDraftContent.LEntryDraftTranscriptions);
        Assert.True(added.LTranscriptionDraftId < 0);
        Assert.Equal("Jyutping", added.LTranscriptionDraftScheme);

        answered = engine.TEngineRequestApply(
            TInterface.TTranscriptionTextCreate(started.LDraftId, added.LTranscriptionDraftId, "hoeng1 gong2"));
        Assert.Equal("hoeng1 gong2", answered.LDraftContent.LEntryDraftTranscriptions[0].LTranscriptionDraftText);

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TTranscriptionAdditionCreate(started.LDraftId, "Jyutping", 1)));
        Assert.Equal(LRefusal.LRefusalScheme, refusal.LRefusalReason);
    }

    [Fact]
    public void RequestApply_TranscriptionShiftAndRemoval_KeepsOnlyTheRowsNamed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TTranscriptionAdditionCreate(started.LDraftId, "Jyutping", 0));
        answered = engine.TEngineRequestApply(
            TInterface.TTranscriptionAdditionCreate(started.LDraftId, "Yale", 1));
        long jyutping = answered.LDraftContent.LEntryDraftTranscriptions[0].LTranscriptionDraftId;
        long yale = answered.LDraftContent.LEntryDraftTranscriptions[1].LTranscriptionDraftId;

        answered = engine.TEngineRequestApply(TInterface.TTranscriptionShiftCreate(started.LDraftId, yale, 0));
        Assert.Equal(
            [yale, jyutping],
            answered.LDraftContent.LEntryDraftTranscriptions.Select(row => row.LTranscriptionDraftId));

        answered = engine.TEngineRequestApply(TInterface.TTranscriptionRemovalCreate(started.LDraftId, jyutping));
        Assert.Equal(yale, Assert.Single(answered.LDraftContent.LEntryDraftTranscriptions).LTranscriptionDraftId);
    }

    [Fact]
    public void SchemeRead_LanguagePack_ListsDeclaredSchemesOnly()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Equal(["Jyutping", "Yale"], engine.TEngineSchemeRead("Cantonese"));
        Assert.Empty(engine.TEngineSchemeRead("English"));
    }

    private static LEntryDraft TTranscriptionDraftCreate(IReadOnlyList<LTranscriptionDraft> transcriptions)
    {
        return TInterface.TEntryDraftCreate(
            "香港",
            "Cantonese",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "Hong Kong", [], [], [], string.Empty, [], [], 1)],
            [],
            transcriptions: transcriptions);
    }
}
