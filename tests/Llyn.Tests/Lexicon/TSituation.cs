using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TSituation
{
    [Fact]
    public void SituationUpdate_OnManyCards_ShowsNewWordingOnEach()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LSituation situation = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "at a funeral", null, null));
        LEntry entry = TSituationEntryCreate(engine, [situation], [situation]);

        engine.TEngineSituationUpdate(situation with
        {
            LSituationTitle = "at a memorial",
            LSituationDescription = "spoken to the bereaved",
            LSituationKind = "register",
        });

        LSituationDraft read =
            Assert.Single(TSituationCardRead(engine, entry.LEntryId, false).LCardDraftSituation);
        Assert.Equal("at a memorial", read.LSituationDraftTitle);
        Assert.Equal(
            "at a memorial",
            Assert.Single(TSituationCardRead(engine, entry.LEntryId, true).LCardDraftSituation)
                .LSituationDraftTitle);
        Assert.Equal("spoken to the bereaved", engine.TEngineSituationRead(
            situation.LSituationId)?.LSituationDescription);
    }

    [Fact]
    public void SituationPick_CollocationCard_LandsOnThatCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSituationEntryCreate(engine);
        LSituation situation = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "in court", null, null));

        engine.TRequestEntryApply(entry.LEntryId, draft => TInterface.TSituationPickCreate(
            draft.LDraftId, draft.LDraftContent.LEntryDraftCollocations[0].LCardDraftId, situation.LSituationId, 0));

        Assert.NotEqual(
            TSituationCardRead(engine, entry.LEntryId, false).LCardDraftId,
            TSituationCardRead(engine, entry.LEntryId, true).LCardDraftId);
        Assert.Empty(TSituationCardRead(engine, entry.LEntryId, false).LCardDraftSituation);
        Assert.Equal(
            situation.LSituationId,
            Assert.Single(TSituationCardRead(engine, entry.LEntryId, true).LCardDraftSituation).LSituationDraftId);
    }

    [Fact]
    public void SituationRead_CardHoldingSeveral_ReturnsCardOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSituationEntryCreate(engine);

        LSituation first = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "in court", null, null));
        LSituation second = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "at home", null, null));

        TSituationPickApply(engine, entry.LEntryId, first.LSituationId, 0);
        TSituationPickApply(engine, entry.LEntryId, second.LSituationId, 0);

        Assert.Equal(
            ["at home", "in court"],
            TSituationCardRead(engine, entry.LEntryId, false).LCardDraftSituation
                .Select(row => row.LSituationDraftTitle));
    }

    [Fact]
    public void SituationDelete_StillReferenced_RefusesAndDetachKeepsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSituationEntryCreate(engine);

        LSituation situation = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "in court", null, null));
        TSituationPickApply(engine, entry.LEntryId, situation.LSituationId, 0);

        Assert.Throws<InvalidOperationException>(() =>
            engine.TEngineSituationDelete(situation.LSituationId, false));

        engine.TRequestEntryApply(entry.LEntryId, draft => TInterface.TSituationRemovalCreate(
            draft.LDraftId, draft.LDraftContent.LEntryDraftMeanings[0].LCardDraftId, situation.LSituationId));
        Assert.Empty(TSituationCardRead(engine, entry.LEntryId, false).LCardDraftSituation);
        Assert.NotNull(engine.TEngineSituationRead(situation.LSituationId));
    }

    [Fact]
    public void EntryUpdate_SituationEdited_KeepsId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty,
                string.Empty,
                "a unit of language",
                [],
                [
                    TInterface.TSituationDraftCreate("in conversation"),
                    TInterface.TSituationDraftCreate("in court"),
                    TInterface.TSituationDraftCreate("at home"),
                ],
                [],
                [], [], 1)],
            []));

        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        Assert.Equal(
            ["in conversation", "in court", "at home"],
            TSituationCardRead(engine, entry.LEntryId, false).LCardDraftSituation
                .Select(row => row.LSituationDraftTitle));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LCardDraft card = loaded.LEntryDraftMeanings[0];
        Assert.Equal(3, card.LCardDraftSituation.Count);
        foreach (LSituationDraft draft in card.LCardDraftSituation)
        {
            Assert.NotEqual(0, draft.LSituationDraftId);
        }

        LReference reference = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            TInterface.TStateValueCreate("A Dictionary"),
            LStateValue.LStateValueUnspecified,
            LReferenceKind.LReferenceKindUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateMark.LStateMarkUnspecified));

        engine.TEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftMeanings =
            [
                card with
                {
                    LCardDraftSituation =
                    [
                        card.LCardDraftSituation[1] with
                        {
                            LSituationDraftTitle = "in a courtroom",
                        },
                        card.LCardDraftSituation[0],
                        TInterface.TSituationDraftCreate("in a letter"),
                    ],
                },
            ],
        });

        long citedId = card.LCardDraftSituation[1].LSituationDraftId;
        IReadOnlyList<LSituationDraft> attached =
            TSituationCardRead(engine, entry.LEntryId, false).LCardDraftSituation;
        Assert.Equal(
            ["in a courtroom", "in conversation", "in a letter"],
            attached.Select(row => row.LSituationDraftTitle));
        Assert.Equal(citedId, attached[0].LSituationDraftId);
        Assert.Equal(card.LCardDraftSituation[0].LSituationDraftId, attached[1].LSituationDraftId);

        Assert.NotNull(engine.TEngineSituationRead(card.LCardDraftSituation[2].LSituationDraftId));
        Assert.Equal(4, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM situation;"));
        Assert.Equal(3, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_situation;"));
    }

    [Fact]
    public void SituationRead_WorkspaceShelf_ReturnsReferenceCounts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LSituation shared = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "in court", null, null));
        LSituation lonely = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "at home", null, null));
        TSituationEntryCreate(engine, [shared], [shared]);

        Assert.Equal(
            ["in court", "at home"],
            engine.TEngineSituationFind(string.Empty, LCatalogOrder.LCatalogOrderEarliest)
                .OrderBy(row => row.LCatalogSituationStored.LSituationId)
                .Select(row => row.LCatalogSituationStored.LSituationTitle.TStateValueShow()));


        IReadOnlyDictionary<long, int> counts = engine.TEngineUsageRead(LOwner.LOwnerSituation);
        Assert.Equal(2, counts[shared.LSituationId]);
        Assert.DoesNotContain(lonely.LSituationId, counts);
    }

    [Fact]
    public void UsageRead_SituationOnCard_NamesSideAndEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LSituation situation = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "in court", null, null));
        LEntry entry = TSituationEntryCreate(engine, [situation], [situation]);

        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        long collocationId = TSituationCardRead(engine, entry.LEntryId, true).LCardDraftId;
        IReadOnlyList<LUsage> usage = engine.TEngineUsageRead(situation.LSituationId, LOwner.LOwnerSituation);
        Assert.Equal(2, usage.Count);

        LUsage meaning = usage.Single(row => row.LUsageOwner == LOwner.LOwnerMeaning);
        Assert.Equal(meaningId, meaning.LUsageId);
        Assert.Equal(entry.LEntryId, meaning.LUsageEntry);
        Assert.Equal("word", meaning.LUsageHeadword);
        Assert.Equal("English", meaning.LUsageLanguage);
        Assert.Equal("a meaning", meaning.LUsageTitle.TStateValueShow());

        LUsage collocation = usage.Single(row => row.LUsageOwner == LOwner.LOwnerCollocation);
        Assert.Equal(collocationId, collocation.LUsageId);
        Assert.Equal("in a word", collocation.LUsageTitle.TStateValueShow());

        Assert.Empty(engine.TEngineUsageRead(
            engine.TEngineSituationCreate(TInterface.TSituationCreate(0, "at home", null, null))
                .LSituationId,
            LOwner.LOwnerSituation));
    }

    [Fact]
    public void SituationDelete_DetachingDelete_DropsAndRenumbers()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LSituation first = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "in court", null, null));
        LSituation second = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "at home", null, null));
        LEntry entry = TSituationEntryCreate(engine, [first, second], [first]);

        engine.TEngineSituationDelete(first.LSituationId, true);

        Assert.Null(engine.TEngineSituationRead(first.LSituationId));
        Assert.Equal(
            ["at home"],
            TSituationCardRead(engine, entry.LEntryId, false).LCardDraftSituation
                .Select(row => row.LSituationDraftTitle.TStateValueShow()));
        Assert.Empty(TSituationCardRead(engine, entry.LEntryId, true).LCardDraftSituation);
        Assert.Equal(0, workspace.TWorkspaceCountRead(
            "SELECT COUNT(*) FROM sense_situation WHERE position <> 0;"));

        LSituation third = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "in a letter", null, null));
        TSituationPickApply(engine, entry.LEntryId, third.LSituationId, 1);
        Assert.Equal(
            ["at home", "in a letter"],
            TSituationCardRead(engine, entry.LEntryId, false).LCardDraftSituation
                .Select(row => row.LSituationDraftTitle.TStateValueShow()));
    }

    private static LCardDraft TSituationCardRead(LEngine engine, long entryId, bool collocation)
    {
        LEntryDraft loaded = engine.TEngineEntryLoad(entryId)!;
        return collocation ? loaded.LEntryDraftCollocations[0] : loaded.LEntryDraftMeanings[0];
    }

    private static void TSituationPickApply(LEngine engine, long entryId, long situationId, int position)
    {
        engine.TRequestEntryApply(entryId, draft => TInterface.TSituationPickCreate(
            draft.LDraftId, draft.LDraftContent.LEntryDraftMeanings[0].LCardDraftId, situationId, position));
    }

    private static LEntry TSituationEntryCreate(LEngine engine)
    {
        return TSituationEntryCreate(engine, [], []);
    }

    private static LEntry TSituationEntryCreate(
        LEngine engine, IReadOnlyList<LSituation> meaning, IReadOnlyList<LSituation> collocation)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty, string.Empty, "a meaning", [], TSituationDraftRead(meaning), [], [], [], 1)],
            [TInterface.TCardDraftCreate(
                string.Empty, "in a word", "briefly", [], TSituationDraftRead(collocation), [], [], [], 1)]));
    }

    private static List<LSituationDraft> TSituationDraftRead(IReadOnlyList<LSituation> situations)
    {
        return [.. situations.Select(row => TInterface.TSituationDraftCreate(row.LSituationTitle, row.LSituationId))];
    }
}
