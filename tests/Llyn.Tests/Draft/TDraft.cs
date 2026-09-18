using System.Collections.Generic;
using System.Text.Json;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDraft
{
    [Fact]
    public void DraftCommit_HeldDraft_StoresEntryAndDeletesFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        engine.TRequestContentApply(
            started.LDraftId,
            TInterface.TDraftNestedCreate("editor", "kindle").LDraftContent with
            {
                LEntryDraftMeanings = [TInterface.TDraftCardCreate("set alight")],
            });

        LEntry stored = engine.TEngineDraftCommit(started.LDraftId);

        Assert.Equal("kindle", stored.LEntryHeadword);
        Assert.NotEqual(0, stored.LEntryId);
        Assert.Equal("kindle", engine.TEngineEntryRead(stored.LEntryId)?.LEntryHeadword);
        Assert.Empty(engine.TEngineDraftScan());
        Assert.Null(engine.TEngineDraftRead(started.LDraftId));
    }

    [Fact]
    public void DraftStart_Fresh_TakesFirstLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        IReadOnlyList<string> languages = engine.TEngineLanguageRead();
        LDraft started = engine.TEngineDraftStart("editor", null);

        Assert.NotEmpty(languages);
        Assert.Equal(languages[0], started.LDraftContent.LEntryDraftLanguage);
        Assert.Equal(languages[0], engine.TEngineDraftRead(started.LDraftId)?.LDraftContent.LEntryDraftLanguage);
        Assert.False(engine.TEngineDraftCheck(started.LDraftId));
    }

    [Fact]
    public void DraftCommit_WriteRefused_KeepsDraftFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);

        Assert.Throws<LRefusal>(() => engine.TEngineDraftCommit(started.LDraftId));

        LDraft? held = engine.TEngineDraftRead(started.LDraftId);

        Assert.NotNull(held);
        Assert.Equal(started.LDraftId, held.LDraftId);
        Assert.Single(engine.TEngineDraftScan());
        Assert.Empty(engine.TEngineEntryFind(string.Empty));
    }

    [Fact]
    public void DraftMove_CardMoved_RenumbersAllCards()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        engine.TRequestContentApply(
            started.LDraftId,
            TInterface.TDraftNestedCreate("editor", "kindle").LDraftContent with
            {
                LEntryDraftMeanings =
                [
                    TInterface.TDraftCardCreate("first"),
                    TInterface.TDraftCardCreate("second"),
                    TInterface.TDraftCardCreate("third"),
                ],
            });

        IReadOnlyList<LCardDraft> moved = engine.TEngineDraftMove(started.LDraftId, false, 0, 2);

        Assert.Equal(
            ["second", "third", "first"],
            moved.Select(card => card.LCardDraftTitle.TStateValueShow()));
        Assert.Equal([1, 2, 3], moved.Select(card => card.LCardDraftPosition));

        LDraft? held = engine.TEngineDraftRead(started.LDraftId);

        Assert.NotNull(held);
        Assert.Equal(
            ["second", "third", "first"],
            held.LDraftContent.LEntryDraftMeanings.Select(card => card.LCardDraftTitle.TStateValueShow()));
        Assert.Equal([1, 2, 3], held.LDraftContent.LEntryDraftMeanings.Select(card => card.LCardDraftPosition));
    }

    [Fact]
    public void DraftCheck_OnlyVideoAdded_ReportsChanged()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft first = engine.TEngineDraftStart("editor", null);
        engine.TRequestContentApply(
            first.LDraftId,
            TInterface.TDraftNestedCreate("editor", "kindle").LDraftContent with
            {
                LEntryDraftMeanings = [TInterface.TDraftCardCreate("set alight")],
            });

        LEntry stored = engine.TEngineDraftCommit(first.LDraftId);
        LDraft opened = engine.TEngineDraftStart("editor", stored.LEntryId);

        Assert.False(engine.TEngineDraftCheck(opened.LDraftId));

        LCardDraft card = opened.LDraftContent.LEntryDraftMeanings[0];

        engine.TRequestContentApply(opened.LDraftId, opened.LDraftContent with
            {
                LEntryDraftMeanings =
                [
                    card with
                    {
                        LCardDraftVideo = [TInterface.TVideoDraftCreate("https://example.com/reel.mp4")],
                    },
                ],
            });

        Assert.True(engine.TEngineDraftCheck(opened.LDraftId));
    }

    [Fact]
    public void DraftCheck_OnlyLanguageChanged_ReportsChanged()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft first = engine.TEngineDraftStart("editor", null);
        engine.TRequestContentApply(
            first.LDraftId,
            TInterface.TDraftNestedCreate("editor", "kindle").LDraftContent with
            {
                LEntryDraftMeanings = [TInterface.TDraftCardCreate("set alight")],
            });

        LEntry stored = engine.TEngineDraftCommit(first.LDraftId);
        LDraft opened = engine.TEngineDraftStart("editor", stored.LEntryId);

        Assert.False(engine.TEngineDraftCheck(opened.LDraftId));

        engine.TRequestContentApply(opened.LDraftId, opened.LDraftContent with { LEntryDraftLanguage = "Korean" });

        Assert.True(engine.TEngineDraftCheck(opened.LDraftId));
    }

    [Fact]
    public void DraftCheck_NewDraftCarryingLanguage_ReportsUnchanged()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft opened = engine.TEngineDraftStart("editor", null);

        engine.TRequestContentApply(opened.LDraftId, opened.LDraftContent with { LEntryDraftLanguage = "Korean" });

        Assert.False(engine.TEngineDraftCheck(opened.LDraftId));
    }

    [Fact]
    public void DraftCommit_DraftsPointingAtEachOther_NoRecursion()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft first = engine.TEngineDraftStart("editor", null);
        LDraft second = engine.TEngineDraftStart("editor", null);

        engine.TRequestContentApply(
            first.LDraftId,
            TInterface.TDraftNestedCreate("editor", "kindle").LDraftContent with
            {
                LEntryDraftMeanings = [TInterface.TDraftCardCreate("set alight")],
            });
        engine.TRequestContentApply(
            second.LDraftId,
            TInterface.TDraftNestedCreate("editor", "ember").LDraftContent with
            {
                LEntryDraftMeanings = [TInterface.TDraftCardCreate("a glowing coal")],
            });

        engine.TEngineCourtSave(first.LDraftId, second.LDraftId, "ember", "English");
        engine.TEngineCourtSave(second.LDraftId, first.LDraftId, "kindle", "English");

        LEntry stored = engine.TEngineDraftCommit(first.LDraftId);

        Assert.Equal("kindle", stored.LEntryHeadword);
        Assert.Single(engine.TEngineEntryFind("ember"));
        Assert.Empty(engine.TEngineDraftScan());
        Assert.Empty(TInterface.TCourtArchiveScan(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void DraftCommit_DraftsTranslatingEachOther_StoresBothLinks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft first = engine.TEngineDraftStart("editor", null);
        LDraft second = engine.TEngineDraftStart("editor", null);

        engine.TRequestContentApply(
            first.LDraftId,
            TInterface.TDraftNestedCreate("editor", "kindle").LDraftContent with
            {
                LEntryDraftMeanings =
                    [TInterface.TDraftCardCreate("set alight") with { LCardDraftTranslation = [second.LDraftId] }],
            });
        engine.TRequestContentApply(
            second.LDraftId,
            TInterface.TDraftNestedCreate("editor", "ember").LDraftContent with
            {
                LEntryDraftMeanings =
                    [TInterface.TDraftCardCreate("a glowing coal") with { LCardDraftTranslation = [first.LDraftId] }],
            });

        engine.TEngineCourtSave(first.LDraftId, second.LDraftId, "ember", "English");
        engine.TEngineCourtSave(second.LDraftId, first.LDraftId, "kindle", "English");

        LEntry kindle = engine.TEngineDraftCommit(first.LDraftId);
        LEntry ember = Assert.Single(engine.TEngineEntryFind("ember"));

        LTranslationArchive translations = TInterface.TTranslationArchiveCreate(workspace.TWorkspaceDatabase);
        LMeaningArchive meanings = TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase);
        LMeaning lit = Assert.Single(meanings.TMeaningRead(kindle.LEntryId));
        LMeaning coal = Assert.Single(meanings.TMeaningRead(ember.LEntryId));

        Assert.Equal(
            ember.LEntryId, Assert.Single(translations.TTranslationMeaningRead(lit.LMeaningId)).LTranslationEntryId);
        Assert.Equal(
            kindle.LEntryId, Assert.Single(translations.TTranslationMeaningRead(coal.LMeaningId)).LTranslationEntryId);
        Assert.Empty(engine.TEngineDraftScan());
        Assert.Empty(TInterface.TCourtArchiveScan(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void DraftCommit_NamesStoredEntry_UpdatesIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("Input", null);
        LDraft written = engine.TRequestContentApply(
            started.LDraftId,
            TInterface.TDraftNestedCreate("Input", "kindle").LDraftContent with
            {
                LEntryDraftMeanings = [TInterface.TDraftCardCreate("set alight")],
            });

        LEntry stored = engine.TEngineEntrySave(written.LDraftContent);

        TInterface.TDraftArchiveSave(workspace.TWorkspaceFolder, written with { LDraftEntryId = stored.LEntryId });

        LEntry recommitted = engine.TEngineDraftCommit(started.LDraftId);

        Assert.Equal(stored.LEntryId, recommitted.LEntryId);
        Assert.Single(engine.TEngineEntryFind("kindle"));
        Assert.Empty(engine.TEngineDraftScan());
    }

    [Fact]
    public void DraftCommit_NamesDeletedEntry_StoresNewEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TRequestContentApply(
            started.LDraftId,
            TInterface.TDraftNestedCreate("Input", "kindle").LDraftContent with
            {
                LEntryDraftMeanings = [TInterface.TDraftCardCreate("set alight")],
            });

        LEntry stored = engine.TEngineDraftCommit(started.LDraftId);
        LDraft opened = engine.TEngineDraftStart("Input", stored.LEntryId);

        engine.TRequestContentApply(opened.LDraftId, opened.LDraftContent with { LEntryDraftHeadword = "ember" });

        engine.TEngineEntryDelete(stored.LEntryId);

        Assert.True(engine.TEngineDraftCheck(opened.LDraftId));

        LEntry recommitted = engine.TEngineDraftCommit(opened.LDraftId);

        Assert.Equal("ember", recommitted.LEntryHeadword);
        Assert.NotEqual(stored.LEntryId, recommitted.LEntryId);
        Assert.Equal("ember", engine.TEngineEntryRead(recommitted.LEntryId)?.LEntryHeadword);
        Assert.Empty(engine.TEngineDraftScan());
    }

    [Fact]
    public void DraftCommit_TargetAnotherEditorHolds_KeepsTargetFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using LEngine other = workspace.TWorkspaceEngineStart();

        LDraft owner = engine.TEngineDraftStart("Input", null);
        LDraft target = other.TEngineDraftStart("Library", null);

        engine.TRequestContentApply(
            owner.LDraftId,
            TInterface.TDraftNestedCreate("Input", "kindle").LDraftContent with
            {
                LEntryDraftMeanings = [TInterface.TDraftCardCreate("set alight")],
            });
        other.TRequestContentApply(target.LDraftId, TInterface.TDraftNestedCreate("Library", "ember").LDraftContent with
            {
                LEntryDraftMeanings = [TInterface.TDraftCardCreate("a glowing coal")],
            });

        engine.TEngineCourtSave(owner.LDraftId, target.LDraftId, "ember", "English");

        LEntry stored = engine.TEngineDraftCommit(owner.LDraftId);
        LDraft? kept = engine.TEngineDraftRead(target.LDraftId);

        Assert.Equal("kindle", stored.LEntryHeadword);
        Assert.Null(engine.TEngineDraftRead(owner.LDraftId));
        Assert.NotNull(kept);
        Assert.Single(engine.TEngineEntryFind("ember"));
        Assert.Equal(engine.TEngineEntryFind("ember")[0].LEntryId, kept.LDraftEntryId);
        Assert.True(TInterface.TClaimArchiveCheck(workspace.TWorkspaceFolder, target.LDraftId));
        Assert.Empty(TInterface.TCourtArchiveScan(workspace.TWorkspaceFolder));

        LEntry recommitted = other.TEngineDraftCommit(target.LDraftId);

        Assert.Equal(kept.LDraftEntryId, recommitted.LEntryId);
        Assert.Single(engine.TEngineEntryFind("ember"));
    }

    [Fact]
    public void DraftCommit_TargetHeldByAnotherEngine_LeavesItNamingTheStoredEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        using LEngine holder = workspace.TWorkspaceEngineStart();
        using LEngine writer = workspace.TWorkspaceEngineStart();

        LDraft target = holder.TEngineDraftStart("Library", null);
        holder.TRequestContentApply(target.LDraftId, TInterface.TDraftPlainCreate("ember"));

        LDraft owner = writer.TEngineDraftStart("Input", null);
        writer.TRequestContentApply(owner.LDraftId, TInterface.TDraftPlainCreate("kindle"));
        writer.TEngineCourtSave(owner.LDraftId, target.LDraftId, "ember", "English");

        writer.TEngineDraftCommit(owner.LDraftId);

        LDraft? kept = writer.TEngineDraftRead(target.LDraftId);

        Assert.NotNull(kept);
        Assert.NotEqual(0, kept.LDraftEntryId);
        Assert.NotNull(writer.TEngineEntryLoad(kept.LDraftEntryId));
        Assert.NotEqual(
            0,
            kept.LDraftContent.LEntryDraftMeanings[0].LCardDraftId);
        Assert.Empty(writer.TEngineLeftoverRead());
    }

    [Fact]
    public void DraftCommit_TranslationNamingNoEntry_DropsItInsteadOfFailing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("Input", null);
        LEntryDraft content = TInterface.TDraftPlainCreate("kindle");

        engine.TRequestContentApply(started.LDraftId, content with
            {
                LEntryDraftMeanings =
                [
                    content.LEntryDraftMeanings[0] with
                    {
                        LCardDraftTranslation = [9999],
                    },
                ],
            });

        LEntry stored = engine.TEngineDraftCommit(started.LDraftId);
        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Empty(loaded.LEntryDraftMeanings[0].LCardDraftTranslation);
    }
}
