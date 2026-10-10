using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardFold
{
    [Fact]
    public void DisplayCardRead_FoldedCards_CarryEachCardsStoredFold()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TCardFoldPrepare(engine, "water");
        LEntryDraft held = engine.TEngineEntryLoad(water.LEntryId)!;
        long entry = water.LEntryId;
        engine.TEngineFoldSave(entry, held.LEntryDraftMeanings[1].LCardDraftId);
        engine.TEngineFoldSave(entry, held.LEntryDraftCollocations[0].LCardDraftId);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(entry);

        CLecternCard card = wing.CWingDisplay.CDisplayCard.CDisplayCardRead();

        Assert.Equal(
            held.LEntryDraftMeanings.Select(static held => held.LCardDraftId),
            card.CLecternCardMeanings.Select(static leaf => leaf.CLeafId));
        Assert.Equal([false, true], card.CLecternCardMeanings.Select(static leaf => leaf.CLeafFolded));
        Assert.True(Assert.Single(card.CLecternCardCollocations).CLeafFolded);
        Assert.All(
            card.CLecternCardMeanings.Concat(card.CLecternCardCollocations),
            static leaf => Assert.True(leaf.CLeafStored));
    }

    [Fact]
    public void DisplayFoldToggle_StoredCard_FoldsThenUnfoldsAndRaisesTheDisplayFold()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TCardFoldPrepare(engine, "water");
        LEntryDraft held = engine.TEngineEntryLoad(water.LEntryId)!;
        long card = held.LEntryDraftMeanings[0].LCardDraftId;
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(water.LEntryId);
        CDisplayCard display = wing.CWingDisplay.CDisplayCard;
        List<long> heard = [];
        wing.CWingDisplay.CDisplayFoldChanged += bulletin => heard.Add(bulletin.CBulletinId);

        bool foldTaken = display.CDisplayFoldToggle(card, true);
        bool folded = display.CDisplayCardRead().CLecternCardMeanings[0].CLeafFolded;
        bool stored = engine.TEngineFoldRead(water.LEntryId).Contains(card);
        bool unfoldTaken = display.CDisplayFoldToggle(card, false);

        Assert.True(foldTaken);
        Assert.True(unfoldTaken);
        Assert.True(folded);
        Assert.True(stored);
        Assert.Equal([water.LEntryId, water.LEntryId], heard);
        Assert.DoesNotContain(card, engine.TEngineFoldRead(water.LEntryId));
        Assert.False(display.CDisplayCardRead().CLecternCardMeanings[0].CLeafFolded);
    }

    [Fact]
    public void EntryDraftChanged_FoldedCard_CarriesTheStoredFold()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry water = TCardFoldPrepare(engine, "water");
        LEntryDraft held = engine.TEngineEntryLoad(water.LEntryId)!;
        engine.TEngineFoldSave(water.LEntryId, held.LEntryDraftMeanings[0].LCardDraftId);
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        List<CEntryDraft> shown = [];
        editor.TEditorFixtureEntry.CEntryDraftChanged += shown.Add;

        editor.TEditorFixtureOpen(water.LEntryId);

        CEntryDraft draft = Assert.Single(shown);
        Assert.Equal([true, false], draft.CEntryDraftMeanings.Select(static card => card.CCardDraftFolded));
        Assert.False(Assert.Single(draft.CEntryDraftCollocations).CCardDraftFolded);
        Assert.All(draft.CEntryDraftMeanings, static card => Assert.True(card.CCardDraftStored));
    }

    [Fact]
    public void CardFoldToggle_StoredCard_FoldsTheCardById()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry water = TCardFoldPrepare(engine, "water");
        LEntryDraft held = engine.TEngineEntryLoad(water.LEntryId)!;
        long card = held.LEntryDraftCollocations[0].LCardDraftId;
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(water.LEntryId);
        List<CEntryDraft> shown = [];
        editor.TEditorFixtureEntry.CEntryDraftChanged += shown.Add;

        bool foldTaken = editor.TEditorFixtureEditor.CEditorList.CCardFoldToggle(card, true);

        Assert.True(foldTaken);
        Assert.Equal(new HashSet<long> { card }, engine.TEngineFoldRead(water.LEntryId));
        Assert.True(Assert.Single(shown[^1].CEntryDraftCollocations).CCardDraftFolded);
        Assert.False(editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftStorable);

        bool unfoldTaken = editor.TEditorFixtureEditor.CEditorList.CCardFoldToggle(card, false);

        Assert.True(unfoldTaken);
        Assert.Empty(engine.TEngineFoldRead(water.LEntryId));
        Assert.False(Assert.Single(editor.TEditorDraftRead()!.CEntryDraftCollocations).CCardDraftFolded);
    }

    [Fact]
    public void CardFoldToggle_UnsavedCard_AnswersItCannotFoldAndStoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry water = TCardFoldPrepare(engine, "water");
        LEntryDraft held = engine.TEngineEntryLoad(water.LEntryId)!;
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(water.LEntryId);
        editor.TEditorFixtureEditor.CEditorList.CCardMeaningAdd();
        CCardDraft fresh = editor.TEditorDraftRead()!.CEntryDraftMeanings
            .Single(static card => !card.CCardDraftStored);

        editor.TEditorFixtureEditor.CEditorList.CCardFoldToggle(fresh.CCardDraftId, true);

        Assert.False(fresh.CCardDraftFolded);
        Assert.Empty(engine.TEngineFoldRead(water.LEntryId));
        Assert.False(editor.TEditorDraftRead()!.CEntryDraftMeanings
            .Single(static card => !card.CCardDraftStored).CCardDraftFolded);
    }

    [Fact]
    public void DisplayFoldToggle_OneEntry_LeavesAnotherEntrysCardsUnfolded()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TCardFoldPrepare(engine, "water");
        LEntryDraft held = engine.TEngineEntryLoad(water.LEntryId)!;
        LEntry salt = TCardFoldPrepare(engine, "salt");
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(water.LEntryId);

        wing.CWingDisplay.CDisplayCard.CDisplayFoldToggle(held.LEntryDraftMeanings[0].LCardDraftId, true);
        wing.CWingEntryOpen(salt.LEntryId);

        CLecternCard card = wing.CWingDisplay.CDisplayCard.CDisplayCardRead();
        Assert.All(
            card.CLecternCardMeanings.Concat(card.CLecternCardCollocations),
            static leaf => Assert.False(leaf.CLeafFolded));
        Assert.Empty(engine.TEngineFoldRead(salt.LEntryId));
    }

    [Fact]
    public void DisplayFoldToggle_EditorOnTheSameEntry_RefreshesTheEditorCards()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TCardFoldPrepare(engine, "water");
        LEntryDraft held = engine.TEngineEntryLoad(water.LEntryId)!;
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(water.LEntryId);
        List<CEntryDraft> shown = [];
        editor.TEditorFixtureEntry.CEntryDraftChanged += shown.Add;
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(water.LEntryId);

        wing.CWingDisplay.CDisplayCard.CDisplayFoldToggle(held.LEntryDraftMeanings[1].LCardDraftId, true);

        Assert.Equal([false, true], shown[^1].CEntryDraftMeanings.Select(static card => card.CCardDraftFolded));
    }

    [Fact]
    public void CardFoldToggle_ReadingViewOnTheSameEntry_RaisesTheDisplayFold()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TCardFoldPrepare(engine, "water");
        LEntryDraft held = engine.TEngineEntryLoad(water.LEntryId)!;
        LEntry salt = TCardFoldPrepare(engine, "salt");
        LEntryDraft kept = engine.TEngineEntryLoad(salt.LEntryId)!;
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(water.LEntryId);
        List<long> heard = [];
        wing.CWingDisplay.CDisplayFoldChanged += bulletin => heard.Add(bulletin.CBulletinId);
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(salt.LEntryId);
        editor.TEditorFixtureEditor.CEditorList.CCardFoldToggle(kept.LEntryDraftMeanings[0].LCardDraftId, true);
        editor.TEditorFixtureOpen(water.LEntryId);

        editor.TEditorFixtureEditor.CEditorList.CCardFoldToggle(held.LEntryDraftMeanings[0].LCardDraftId, true);

        Assert.Equal([water.LEntryId], heard);
        Assert.True(wing.CWingDisplay.CDisplayCard.CDisplayCardRead().CLecternCardMeanings[0].CLeafFolded);
    }

    [Fact]
    public void DisplayFoldToggle_NoEntryChosen_AnswersFalseAndStoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TCardFoldPrepare(engine, "water");
        LEntryDraft held = engine.TEngineEntryLoad(water.LEntryId)!;
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        long card = held.LEntryDraftMeanings[0].LCardDraftId;

        bool taken = wing.CWingDisplay.CDisplayCard.CDisplayFoldToggle(card, true);

        Assert.False(taken);
        Assert.Empty(engine.TEngineFoldRead(water.LEntryId));
    }

    [Fact]
    public void CardFoldToggle_NoStoredEntry_AnswersFalseAndStoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry water = TCardFoldPrepare(engine, "water");
        LEntryDraft held = engine.TEngineEntryLoad(water.LEntryId)!;
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        long card = held.LEntryDraftMeanings[0].LCardDraftId;

        bool taken = editor.TEditorFixtureEditor.CEditorList.CCardFoldToggle(card, true);

        Assert.False(taken);
        Assert.Empty(engine.TEngineFoldRead(water.LEntryId));
    }

    private static LEntry TCardFoldPrepare(LEngine engine, string headword)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a liquid", 1), TInterface.TCardCreate("a drink", 2)],
            [TInterface.TCardCreate("running water", 1)]));
    }
}
