using Llyn.Core;
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

        LEntry entry = TSituationEntryCreate(engine);
        string meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        string collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LSituation situation = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(string.Empty, "at a funeral", null, null));
        engine.TEngineSituationAttach(meaningId, situation.LSituationId, 0, LOwner.LOwnerMeaning);
        engine.TEngineSituationAttach(
            collocationId, situation.LSituationId, 0, LOwner.LOwnerCollocation);

        engine.TEngineSituationUpdate(situation with
        {
            LSituationTitle = "at a memorial",
            LSituationDescription = "spoken to the bereaved",
            LSituationKind = "register",
        });

        LSituation read = Assert.Single(engine.TEngineSituationRead(meaningId, LOwner.LOwnerMeaning));
        Assert.Equal("at a memorial", read.LSituationTitle);
        Assert.Equal(
            "at a memorial",
            Assert.Single(engine.TEngineSituationRead(collocationId, LOwner.LOwnerCollocation))
                .LSituationTitle);
        Assert.Equal("spoken to the bereaved", engine.TEngineSituationRead(
            situation.LSituationId)?.LSituationDescription);
    }

    [Fact]
    public void SituationRead_CardHoldingSeveral_ReturnsCardOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSituationEntryCreate(engine);
        string meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;

        LSituation first = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(string.Empty, "in court", null, null));
        LSituation second = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(string.Empty, "at home", null, null));

        engine.TEngineSituationAttach(meaningId, first.LSituationId, 0, LOwner.LOwnerMeaning);
        engine.TEngineSituationAttach(meaningId, second.LSituationId, 0, LOwner.LOwnerMeaning);

        Assert.Equal(
            ["at home", "in court"],
            engine.TEngineSituationRead(meaningId, LOwner.LOwnerMeaning)
                .Select(row => row.LSituationTitle));
    }

    [Fact]
    public void SituationRemove_LastReferenceGone_DeletesSituation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSituationEntryCreate(engine);
        string meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;

        LSituation situation = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(string.Empty, "in court", null, null));
        engine.TEngineSituationAttach(meaningId, situation.LSituationId, 0, LOwner.LOwnerMeaning);

        Assert.Throws<InvalidOperationException>(() =>
            engine.TEngineSituationDelete(situation.LSituationId));

        engine.TEngineSituationDetach(meaningId, situation.LSituationId, LOwner.LOwnerMeaning);
        Assert.NotNull(engine.TEngineSituationRead(situation.LSituationId));

        engine.TEngineSituationAttach(meaningId, situation.LSituationId, 0, LOwner.LOwnerMeaning);
        engine.TEngineSituationRemove(meaningId, situation.LSituationId, LOwner.LOwnerMeaning);
        Assert.Null(engine.TEngineSituationRead(situation.LSituationId));
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
                string.Empty,
                [], [], 1)],
            []));

        string meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        Assert.Equal(
            ["in conversation", "in court", "at home"],
            engine.TEngineSituationRead(meaningId, LOwner.LOwnerMeaning).Select(row => row.LSituationTitle));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LCardDraft card = loaded.LEntryDraftMeanings[0];
        Assert.Equal(3, card.LCardDraftSituation.Count);
        foreach (LSituationDraft draft in card.LCardDraftSituation)
        {
            Assert.NotEmpty(draft.LSituationDraftId);
        }

        LReference reference = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            string.Empty,
            TInterface.TStateValueCreate("A Dictionary"),
            LStateValue.LStateValueUnspecified,
            LReferenceKind.LReferenceKindUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LState.LStateUnspecified));

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
                            LSituationDraftText = "in a courtroom",
                        },
                        card.LCardDraftSituation[0],
                        TInterface.TSituationDraftCreate("in a letter"),
                    ],
                },
            ],
        });

        string citedId = card.LCardDraftSituation[1].LSituationDraftId;
        IReadOnlyList<LSituation> attached = engine.TEngineSituationRead(meaningId, LOwner.LOwnerMeaning);
        Assert.Equal(
            ["in a courtroom", "in conversation", "in a letter"],
            attached.Select(row => row.LSituationTitle));
        Assert.Equal(citedId, attached[0].LSituationId);
        Assert.Equal(card.LCardDraftSituation[0].LSituationDraftId, attached[1].LSituationId);

        Assert.NotNull(engine.TEngineSituationRead(card.LCardDraftSituation[2].LSituationDraftId));
        Assert.Equal(4, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM situation;"));
        Assert.Equal(3, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_situation;"));
    }

    [Fact]
    public void SituationRead_WorkspaceShelf_ReturnsReferenceCounts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSituationEntryCreate(engine);
        string meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        string collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LSituation shared = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(string.Empty, "in court", null, null));
        LSituation lonely = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(string.Empty, "at home", null, null));

        engine.TEngineSituationAttach(meaningId, shared.LSituationId, 0, LOwner.LOwnerMeaning);
        engine.TEngineSituationAttach(collocationId, shared.LSituationId, 0, LOwner.LOwnerCollocation);

        Assert.Equal(
            ["in court", "at home"],
            engine.TEngineSituationRead().Select(row => row.LSituationTitle.TStateValueShow()));

        IReadOnlyDictionary<string, int> counts = engine.TEngineUsageRead(LOwner.LOwnerSituation);
        Assert.Equal(2, counts[shared.LSituationId]);
        Assert.DoesNotContain(lonely.LSituationId, counts);
    }

    [Fact]
    public void UsageRead_SituationOnCard_NamesSideAndEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSituationEntryCreate(engine);
        string meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        string collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LSituation situation = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(string.Empty, "in court", null, null));
        engine.TEngineSituationAttach(meaningId, situation.LSituationId, 0, LOwner.LOwnerMeaning);
        engine.TEngineSituationAttach(collocationId, situation.LSituationId, 0, LOwner.LOwnerCollocation);

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
            engine.TEngineSituationCreate(TInterface.TSituationCreate(string.Empty, "at home", null, null))
                .LSituationId,
            LOwner.LOwnerSituation));
    }

    [Fact]
    public void SituationDelete_DetachingDelete_DropsAndRenumbers()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSituationEntryCreate(engine);
        string meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        string collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LSituation first = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(string.Empty, "in court", null, null));
        LSituation second = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(string.Empty, "at home", null, null));

        engine.TEngineSituationAttach(meaningId, first.LSituationId, 0, LOwner.LOwnerMeaning);
        engine.TEngineSituationAttach(meaningId, second.LSituationId, 1, LOwner.LOwnerMeaning);
        engine.TEngineSituationAttach(collocationId, first.LSituationId, 0, LOwner.LOwnerCollocation);

        engine.TEngineSituationDelete(first.LSituationId, true);

        Assert.Null(engine.TEngineSituationRead(first.LSituationId));
        Assert.Equal(
            ["at home"],
            engine.TEngineSituationRead(meaningId, LOwner.LOwnerMeaning)
                .Select(row => row.LSituationTitle.TStateValueShow()));
        Assert.Empty(engine.TEngineSituationRead(collocationId, LOwner.LOwnerCollocation));
        Assert.Equal(0, workspace.TWorkspaceCountRead(
            "SELECT COUNT(*) FROM sense_situation WHERE position <> 0;"));

        LSituation third = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(string.Empty, "in a letter", null, null));
        engine.TEngineSituationAttach(meaningId, third.LSituationId, 1, LOwner.LOwnerMeaning);
        Assert.Equal(
            ["at home", "in a letter"],
            engine.TEngineSituationRead(meaningId, LOwner.LOwnerMeaning)
                .Select(row => row.LSituationTitle.TStateValueShow()));
    }

    private static LEntry TSituationEntryCreate(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], string.Empty, [], [], 1)],
            [TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], string.Empty, [], [], 1)]));
    }
}
