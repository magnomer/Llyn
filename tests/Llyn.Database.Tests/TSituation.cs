using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

public sealed class TSituation
{
    [Fact]
    public void ASituationIsRewrittenOnceAndEveryCardReadsTheNewWording()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TSituationEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        string collocationId =
            engine.LEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LSituation situation = engine.LEngineSituationCreate(
            new LSituation(string.Empty, "at a funeral", null, null, null));
        engine.LEngineSituationAttach(senseId, situation.LSituationId, 0, LOwner.LOwnerSense);
        engine.LEngineSituationAttach(
            collocationId, situation.LSituationId, 0, LOwner.LOwnerCollocation);

        engine.LEngineSituationUpdate(situation with
        {
            LSituationTitle = "at a memorial",
            LSituationDescription = "spoken to the bereaved",
            LSituationKind = "register",
        });

        LSituation read = Assert.Single(engine.LEngineSituationRead(senseId, LOwner.LOwnerSense));
        Assert.Equal("at a memorial", read.LSituationTitle);
        Assert.Equal(
            "at a memorial",
            Assert.Single(engine.LEngineSituationRead(collocationId, LOwner.LOwnerCollocation))
                .LSituationTitle);
        Assert.Equal("spoken to the bereaved", engine.LEngineSituationRead(
            situation.LSituationId)?.LSituationDescription);
    }

    [Fact]
    public void TheOrderACardHoldsIsTheOrderTheSituationsComeBackIn()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TSituationEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;

        LSituation first = engine.LEngineSituationCreate(
            new LSituation(string.Empty, "in court", null, null, null));
        LSituation second = engine.LEngineSituationCreate(
            new LSituation(string.Empty, "at home", null, null, null));

        engine.LEngineSituationAttach(senseId, first.LSituationId, 0, LOwner.LOwnerSense);
        engine.LEngineSituationAttach(senseId, second.LSituationId, 0, LOwner.LOwnerSense);

        Assert.Equal(
            ["at home", "in court"],
            engine.LEngineSituationRead(senseId, LOwner.LOwnerSense)
                .Select(row => row.LSituationTitle));
    }

    [Fact]
    public void ARemovedLastReferenceTakesTheSituationWhileADeleteIsRefusedBeforeThat()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TSituationEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;

        LSituation situation = engine.LEngineSituationCreate(
            new LSituation(string.Empty, "in court", null, null, null));
        engine.LEngineSituationAttach(senseId, situation.LSituationId, 0, LOwner.LOwnerSense);

        Assert.Throws<InvalidOperationException>(() =>
            engine.LEngineSituationDelete(situation.LSituationId));

        engine.LEngineSituationDetach(senseId, situation.LSituationId, LOwner.LOwnerSense);
        Assert.NotNull(engine.LEngineSituationRead(situation.LSituationId));

        engine.LEngineSituationAttach(senseId, situation.LSituationId, 0, LOwner.LOwnerSense);
        engine.LEngineSituationRemove(senseId, situation.LSituationId, LOwner.LOwnerSense);
        Assert.Null(engine.LEngineSituationRead(situation.LSituationId));
    }

    [Fact]
    public void AMeaningKeepsEverySituationItRefersToAndEditsOneWithoutLosingItsIdOrSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(
                string.Empty,
                string.Empty,
                "a unit of language",
                [],
                [
                    LSituationDraft.LSituationDraftCreate("in conversation"),
                    LSituationDraft.LSituationDraftCreate("in court"),
                    LSituationDraft.LSituationDraftCreate("at home"),
                ],
                string.Empty,
                [], [])],
            []));

        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        Assert.Equal(
            ["in conversation", "in court", "at home"],
            engine.LEngineSituationRead(senseId, LOwner.LOwnerSense).Select(row => row.LSituationTitle));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.LEngineEntryLoad(entry.LEntryId));
        LCardDraft card = loaded.LEntryDraftSenses[0];
        Assert.Equal(3, card.LCardDraftSituation.Count);
        foreach (LSituationDraft draft in card.LCardDraftSituation)
        {
            Assert.NotEmpty(draft.LSituationDraftId);
        }

        LReference reference = engine.LEngineReferenceCreate(new LReference(
            string.Empty,
            LStateValue.LStateValueCreate("A Dictionary"),
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LState.LStateUnspecified));

        engine.LEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftSenses =
            [
                card with
                {
                    LCardDraftSituation =
                    [
                        card.LCardDraftSituation[1] with
                        {
                            LSituationDraftText = "in a courtroom",
                            LSituationDraftReference = reference.LReferenceId,
                        },
                        card.LCardDraftSituation[0],
                        LSituationDraft.LSituationDraftCreate("in a letter"),
                    ],
                },
            ],
        });

        string citedId = card.LCardDraftSituation[1].LSituationDraftId;
        IReadOnlyList<LSituation> attached = engine.LEngineSituationRead(senseId, LOwner.LOwnerSense);
        Assert.Equal(
            ["in a courtroom", "in conversation", "in a letter"],
            attached.Select(row => row.LSituationTitle));
        Assert.Equal(citedId, attached[0].LSituationId);
        Assert.Equal(reference.LReferenceId, attached[0].LSituationSource.LStateValueShow());
        Assert.Equal(card.LCardDraftSituation[0].LSituationDraftId, attached[1].LSituationId);

        Assert.NotNull(engine.LEngineSituationRead(card.LCardDraftSituation[2].LSituationDraftId));
        Assert.Equal(4, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM situation;"));
        Assert.Equal(3, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_situation;"));
    }

    [Fact]
    public void TheShelfListsEverySituationWithHowManyPlacesReferenceIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TSituationEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        string collocationId =
            engine.LEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LSituation shared = engine.LEngineSituationCreate(
            new LSituation(string.Empty, "in court", null, null, null));
        LSituation lonely = engine.LEngineSituationCreate(
            new LSituation(string.Empty, "at home", null, null, null));

        engine.LEngineSituationAttach(senseId, shared.LSituationId, 0, LOwner.LOwnerSense);
        engine.LEngineSituationAttach(collocationId, shared.LSituationId, 0, LOwner.LOwnerCollocation);

        Assert.Equal(
            ["in court", "at home"],
            engine.LEngineSituationRead().Select(row => row.LSituationTitle.LStateValueShow()));

        IReadOnlyDictionary<string, int> counts = engine.LEngineUsageRead(LOwner.LOwnerSituation);
        Assert.Equal(2, counts[shared.LSituationId]);
        Assert.DoesNotContain(lonely.LSituationId, counts);
    }

    [Fact]
    public void UsageNamesTheReferringSideAndTheEntryItBelongsTo()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TSituationEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        string collocationId =
            engine.LEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LSituation situation = engine.LEngineSituationCreate(
            new LSituation(string.Empty, "in court", null, null, null));
        engine.LEngineSituationAttach(senseId, situation.LSituationId, 0, LOwner.LOwnerSense);
        engine.LEngineSituationAttach(collocationId, situation.LSituationId, 0, LOwner.LOwnerCollocation);

        IReadOnlyList<LUsage> usage = engine.LEngineUsageRead(situation.LSituationId, LOwner.LOwnerSituation);
        Assert.Equal(2, usage.Count);

        LUsage sense = usage.Single(row => row.LUsageOwner == LOwner.LOwnerSense);
        Assert.Equal(senseId, sense.LUsageId);
        Assert.Equal(entry.LEntryId, sense.LUsageEntry);
        Assert.Equal("word", sense.LUsageHeadword);
        Assert.Equal("English", sense.LUsageLanguage);
        Assert.Equal("a meaning", sense.LUsageTitle.LStateValueShow());

        LUsage collocation = usage.Single(row => row.LUsageOwner == LOwner.LOwnerCollocation);
        Assert.Equal(collocationId, collocation.LUsageId);
        Assert.Equal("in a word", collocation.LUsageTitle.LStateValueShow());

        Assert.Empty(engine.LEngineUsageRead(
            engine.LEngineSituationCreate(new LSituation(string.Empty, "at home", null, null, null))
                .LSituationId,
            LOwner.LOwnerSituation));
    }

    [Fact]
    public void ADetachingDeleteDropsEveryReferenceAndRenumbersWhatEachCardHasLeft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TSituationEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        string collocationId =
            engine.LEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LSituation first = engine.LEngineSituationCreate(
            new LSituation(string.Empty, "in court", null, null, null));
        LSituation second = engine.LEngineSituationCreate(
            new LSituation(string.Empty, "at home", null, null, null));

        engine.LEngineSituationAttach(senseId, first.LSituationId, 0, LOwner.LOwnerSense);
        engine.LEngineSituationAttach(senseId, second.LSituationId, 1, LOwner.LOwnerSense);
        engine.LEngineSituationAttach(collocationId, first.LSituationId, 0, LOwner.LOwnerCollocation);

        engine.LEngineSituationDelete(first.LSituationId, true);

        Assert.Null(engine.LEngineSituationRead(first.LSituationId));
        Assert.Equal(
            ["at home"],
            engine.LEngineSituationRead(senseId, LOwner.LOwnerSense)
                .Select(row => row.LSituationTitle.LStateValueShow()));
        Assert.Empty(engine.LEngineSituationRead(collocationId, LOwner.LOwnerCollocation));
        Assert.Equal(0, workspace.TWorkspaceCountRead(
            "SELECT COUNT(*) FROM sense_situation WHERE position <> 0;"));

        LSituation third = engine.LEngineSituationCreate(
            new LSituation(string.Empty, "in a letter", null, null, null));
        engine.LEngineSituationAttach(senseId, third.LSituationId, 1, LOwner.LOwnerSense);
        Assert.Equal(
            ["at home", "in a letter"],
            engine.LEngineSituationRead(senseId, LOwner.LOwnerSense)
                .Select(row => row.LSituationTitle.LStateValueShow()));
    }

    private static LEntry TSituationEntryCreate(LEngine engine)
    {
        return engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(string.Empty, string.Empty, "a meaning", [], [], string.Empty, [], [])],
            [new LCardDraft(string.Empty, "in a word", "briefly", [], [], string.Empty, [], [])]));
    }
}
