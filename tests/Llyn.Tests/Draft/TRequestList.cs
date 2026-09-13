using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRequestList
{
    [Fact]
    public void RequestApply_SentenceAddition_MintsNegativeIdWithNoExample()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        long card = TRequestCardAdd(engine, started.LDraftId);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TSentenceAdditionCreate(started.LDraftId, card, 0));

        LSentenceDraft sentence = Assert.Single(answered.LDraftContent.LEntryDraftMeanings[0].LCardDraftSentence);
        Assert.True(sentence.LSentenceDraftId < 0);
        Assert.Null(sentence.LSentenceDraftExample);
        Assert.False(engine.TEngineDraftCheck(started.LDraftId));
    }

    [Fact]
    public void RequestApply_SentenceText_MintsExampleOnFirstText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        long card = TRequestCardAdd(engine, started.LDraftId);
        long sentence = TRequestSentenceAdd(engine, started.LDraftId, card, 0);

        LDraft answered = engine.TEngineRequestApply(TInterface.TSentenceTextCreate(
            started.LDraftId, card, sentence, TInterface.TStateValueCreate("she knelt to kindle the damp logs")));

        LExampleDraft example = Assert.IsType<LExampleDraft>(
            answered.LDraftContent.LEntryDraftMeanings[0].LCardDraftSentence[0].LSentenceDraftExample);
        Assert.True(example.LExampleDraftId < 0);
        Assert.Equal("she knelt to kindle the damp logs", example.LExampleDraftText.TStateValueShow());
    }

    [Fact]
    public void RequestApply_SentenceRemoval_DropsTheRowNamed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        long card = TRequestCardAdd(engine, started.LDraftId);
        long first = TRequestSentenceAdd(engine, started.LDraftId, card, 0);
        long second = TRequestSentenceAdd(engine, started.LDraftId, card, 1);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TSentenceRemovalCreate(started.LDraftId, card, first));

        LSentenceDraft kept = Assert.Single(answered.LDraftContent.LEntryDraftMeanings[0].LCardDraftSentence);
        Assert.Equal(second, kept.LSentenceDraftId);
    }

    [Fact]
    public void RequestApply_SentenceShift_ReordersRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        long card = TRequestCardAdd(engine, started.LDraftId);
        long first = TRequestSentenceAdd(engine, started.LDraftId, card, 0);
        long second = TRequestSentenceAdd(engine, started.LDraftId, card, 1);
        long third = TRequestSentenceAdd(engine, started.LDraftId, card, 2);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TSentenceShiftCreate(started.LDraftId, card, third, 0));

        Assert.Equal(
            [third, first, second],
            answered.LDraftContent.LEntryDraftMeanings[0].LCardDraftSentence.Select(row => row.LSentenceDraftId));
    }

    [Fact]
    public void RequestApply_SentenceExample_KeepsThePositiveId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LExample stored = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0,
            "English",
            TInterface.TStateValueCreate("the speech kindled a hope"),
            LStateValue.LStateValueUnspecified,
            TInterface.TStateAnchorRead(null)));
        LDraft started = engine.TEngineDraftStart("Input", null);
        long card = TRequestCardAdd(engine, started.LDraftId);
        long sentence = TRequestSentenceAdd(engine, started.LDraftId, card, 0);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TSentenceExampleCreate(started.LDraftId, card, sentence, stored.LExampleId));

        LExampleDraft example = Assert.IsType<LExampleDraft>(
            answered.LDraftContent.LEntryDraftMeanings[0].LCardDraftSentence[0].LSentenceDraftExample);
        Assert.Equal(stored.LExampleId, example.LExampleDraftId);
        Assert.Equal("the speech kindled a hope", example.LExampleDraftText.TStateValueShow());
    }

    [Fact]
    public void RequestApply_SituationAddition_MintsNegativeId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        long card = TRequestCardAdd(engine, started.LDraftId);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TSituationAdditionCreate(started.LDraftId, card, "around a hearth", 0));

        LSituationDraft situation = Assert.Single(answered.LDraftContent.LEntryDraftMeanings[0].LCardDraftSituation);
        Assert.True(situation.LSituationDraftId < 0);
        Assert.Equal("around a hearth", situation.LSituationDraftTitle.TStateValueShow());
    }

    [Fact]
    public void RequestApply_SituationPick_CopiesTheStoredRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSituation stored = engine.TEngineSituationCreate(TInterface.TSituationCreate(
            0,
            TInterface.TStateValueCreate("around a hearth"),
            TInterface.TStateValueCreate("telling stories"),
            LStateValue.LStateValueUnspecified));
        LDraft started = engine.TEngineDraftStart("Input", null);
        long card = TRequestCardAdd(engine, started.LDraftId);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TSituationPickCreate(started.LDraftId, card, stored.LSituationId, 0));
        LDraft again = engine.TEngineRequestApply(
            TInterface.TSituationPickCreate(started.LDraftId, card, stored.LSituationId, 0));

        LSituationDraft situation = Assert.Single(answered.LDraftContent.LEntryDraftMeanings[0].LCardDraftSituation);
        Assert.Equal(stored.LSituationId, situation.LSituationDraftId);
        Assert.Equal("telling stories", situation.LSituationDraftDescription.TStateValueShow());
        Assert.Single(again.LDraftContent.LEntryDraftMeanings[0].LCardDraftSituation);
    }

    [Fact]
    public void RequestApply_SituationTitle_ChangesEveryCardHoldingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSituation stored = engine.TEngineSituationCreate(TInterface.TSituationCreate(
            0,
            TInterface.TStateValueCreate("around a hearth"),
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified));
        LDraft started = engine.TEngineDraftStart("Input", null);
        long first = TRequestCardAdd(engine, started.LDraftId);
        long second = TRequestCardAdd(engine, started.LDraftId);
        engine.TEngineRequestApply(
            TInterface.TSituationPickCreate(started.LDraftId, first, stored.LSituationId, 0));
        engine.TEngineRequestApply(
            TInterface.TSituationPickCreate(started.LDraftId, second, stored.LSituationId, 0));

        LDraft answered = engine.TEngineRequestApply(TInterface.TSituationTitleCreate(
            started.LDraftId, stored.LSituationId, TInterface.TStateValueCreate("by the fire")));

        Assert.All(
            answered.LDraftContent.LEntryDraftMeanings,
            card => Assert.Equal("by the fire", card.LCardDraftSituation[0].LSituationDraftTitle.TStateValueShow()));
    }

    [Fact]
    public void RequestApply_ZeroItemId_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        long card = TRequestCardAdd(engine, started.LDraftId);

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TSituationRemovalCreate(started.LDraftId, card, 0)));

        Assert.Equal(LRefusal.LRefusalItem, refusal.LRefusalReason);
    }

    [Fact]
    public void RequestApply_UnknownItemId_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        long card = TRequestCardAdd(engine, started.LDraftId);

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TSentenceShiftCreate(started.LDraftId, card, TInterface.TIdentityCreate(), 0)));

        Assert.Equal(LRefusal.LRefusalItem, refusal.LRefusalReason);
    }

    [Fact]
    public void RequestApply_RegisterTagImageVideoAddition_MintsEachRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        long card = TRequestCardAdd(engine, started.LDraftId);

        engine.TEngineRequestApply(TInterface.TRegisterAdditionCreate(started.LDraftId, card, "gruff", 0));
        engine.TEngineRequestApply(TInterface.TTagAdditionCreate(started.LDraftId, card, "fire", 0));
        engine.TEngineRequestApply(
            TInterface.TImageAdditionCreate(started.LDraftId, card, "media/fire.jpg", 0));
        LDraft answered = engine.TEngineRequestApply(
            TInterface.TVideoAdditionCreate(started.LDraftId, card, "media/kindling.mp4", 0));

        LCardDraft held = answered.LDraftContent.LEntryDraftMeanings[0];
        Assert.True(Assert.Single(held.LCardDraftRegister).LRegisterDraftId < 0);
        Assert.True(Assert.Single(held.LCardDraftTag).LTagDraftId < 0);
        Assert.True(Assert.Single(held.LCardDraftImage).LImageDraftId < 0);
        Assert.True(Assert.Single(held.LCardDraftVideo).LVideoDraftId < 0);
    }

    [Fact]
    public void RequestApply_TranslationPick_HoldsTheEntryOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        long card = TRequestCardAdd(engine, started.LDraftId);

        engine.TEngineRequestApply(TInterface.TTranslationPickCreate(started.LDraftId, card, 42, 0));
        LDraft answered = engine.TEngineRequestApply(
            TInterface.TTranslationPickCreate(started.LDraftId, card, 42, 0));

        Assert.Equal([42L], answered.LDraftContent.LEntryDraftMeanings[0].LCardDraftTranslation);
    }

    [Fact]
    public void DraftCommit_BlankSentenceRow_StoresNoExample()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "kindle"));
        long card = TRequestCardAdd(engine, started.LDraftId);
        engine.TEngineRequestApply(
            TInterface.TRequestTitleCreate(started.LDraftId, card, TInterface.TStateValueCreate("set alight")));
        TRequestSentenceAdd(engine, started.LDraftId, card, 0);

        LEntry stored = engine.TEngineDraftCommit(started.LDraftId);

        Assert.Empty(engine.TEngineExampleRead());
        Assert.Empty(engine.TEngineEntryLoad(stored.LEntryId)!.LEntryDraftMeanings[0].LCardDraftSentence);
    }

    [Fact]
    public void DraftCommit_FullRequestSequence_StoresTheSameRowsAsTheFixture()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntryDraft fixture = TInterface.TEntryDraftCreate(
            "kindle",
            "English",
            string.Empty,
            "a note",
            [
                TInterface.TCardDraftCreate(
                    TInterface.TStateValueCreate("set alight"),
                    LStateValue.LStateValueUnspecified,
                    TInterface.TStateValueCreate("to set something burning"),
                    [TInterface.TSentenceDraftCreate("she knelt to kindle the damp logs")],
                    [TInterface.TSituationDraftCreate("around a hearth")],
                    [],
                    ["fire", "literary"],
                    [TInterface.TImageDraftCreate("media/fire.jpg")],
                    0,
                    video: [TInterface.TVideoDraftCreate("media/kindling.mp4", "00:12-00:19")],
                    register: [TInterface.TRegisterDraftCreate("gruff")]),
            ],
            []);
        LDraft started = engine.TEngineDraftStart("Input", null);

        engine.TRequestContentApply(started.LDraftId, fixture);
        LEntry stored = engine.TEngineDraftCommit(started.LDraftId);
        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(stored.LEntryId));

        Assert.Equal("kindle", loaded.LEntryDraftHeadword);
        Assert.Equal("a note", loaded.LEntryDraftNote);
        LCardDraft card = Assert.Single(loaded.LEntryDraftMeanings);
        Assert.Equal("set alight", card.LCardDraftTitle.TStateValueShow());
        Assert.Equal("to set something burning", card.LCardDraftMeaning.TStateValueShow());
        Assert.Equal(
            "she knelt to kindle the damp logs",
            Assert.Single(card.LCardDraftSentence).LSentenceDraftExample!.LExampleDraftText.TStateValueShow());
        Assert.Equal("around a hearth", Assert.Single(card.LCardDraftSituation).LSituationDraftTitle.TStateValueShow());
        Assert.Equal("gruff", Assert.Single(card.LCardDraftRegister).LRegisterDraftName.TStateValueShow());
        Assert.Equal(["fire", "literary"], TInterface.TTagDraftRead(card.LCardDraftTag));
        Assert.Equal("media/fire.jpg", Assert.Single(card.LCardDraftImage).LImageDraftLocation.TStateValueShow());
        Assert.Equal("00:12-00:19", Assert.Single(card.LCardDraftVideo).LVideoDraftSpan.TStateValueShow());
        Assert.All(card.LCardDraftSentence, row => Assert.True(row.LSentenceDraftId > 0));
        Assert.All(card.LCardDraftSituation, row => Assert.True(row.LSituationDraftId > 0));
    }

    [Fact]
    public void ReferenceCommit_AuthorsAddedAndPicked_AttachesThemInOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LAuthor known = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LDraft started = engine.TEngineReferenceStart("Reference", null);
        engine.TEngineRequestApply(
            TInterface.TReferenceTitleCreate(started.LDraftId, TInterface.TStateValueCreate("the evening news")));

        engine.TEngineRequestApply(TInterface.TAuthorAdditionCreate(started.LDraftId, "Brook", 0));
        LDraft answered = engine.TEngineRequestApply(
            TInterface.TAuthorPickCreate(started.LDraftId, known.LAuthorId, 0));

        Assert.Equal(2, answered.LDraftAuthor.Count);
        Assert.Equal(known.LAuthorId, answered.LDraftAuthor[0].LAuthorId);
        Assert.True(answered.LDraftAuthor[1].LAuthorId < 0);
        Assert.Equal(LState.LStateSpecified, answered.LDraftReference!.LReferenceAuthorState.LStateMarkState);
        Assert.True(engine.TEngineDraftCheck(started.LDraftId));

        LReference stored = engine.TEngineReferenceCommit(started.LDraftId);

        IReadOnlyList<LAuthor> credits = engine.TEngineAuthorRead(stored.LReferenceId, LOwner.LOwnerReference);
        Assert.Equal(["Ada", "Brook"], credits.Select(author => author.LAuthorName));
    }

    [Fact]
    public void RequestApply_AuthorRemovalAndShift_ReorderTheCredits()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineReferenceStart("Reference", null);
        engine.TEngineRequestApply(TInterface.TAuthorAdditionCreate(started.LDraftId, "Ada", 0));
        engine.TEngineRequestApply(TInterface.TAuthorAdditionCreate(started.LDraftId, "Brook", 1));
        LDraft held = engine.TEngineRequestApply(
            TInterface.TAuthorAdditionCreate(started.LDraftId, "Cam", 2));
        long ada = held.LDraftAuthor[0].LAuthorId;
        long cam = held.LDraftAuthor[2].LAuthorId;

        engine.TEngineRequestApply(TInterface.TAuthorShiftCreate(started.LDraftId, cam, 0));
        LDraft answered = engine.TEngineRequestApply(TInterface.TAuthorRemovalCreate(started.LDraftId, ada));

        Assert.Equal(["Cam", "Brook"], answered.LDraftAuthor.Select(author => author.LAuthorName));
    }

    [Fact]
    public void RequestApply_AuthorAdditionOfKnownName_CreditsTheStoredAuthor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LAuthor known = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LDraft started = engine.TEngineReferenceStart("Reference", null);

        engine.TEngineRequestApply(TInterface.TAuthorAdditionCreate(started.LDraftId, "brook", 0));
        engine.TEngineRequestApply(TInterface.TAuthorAdditionCreate(started.LDraftId, " ADA ", 1));
        LDraft answered = engine.TEngineRequestApply(
            TInterface.TAuthorAdditionCreate(started.LDraftId, "Brook", 2));

        Assert.Equal(2, answered.LDraftAuthor.Count);
        Assert.True(answered.LDraftAuthor[0].LAuthorId < 0);
        Assert.Equal("brook", answered.LDraftAuthor[0].LAuthorName);
        Assert.Equal(known.LAuthorId, answered.LDraftAuthor[1].LAuthorId);
        Assert.Equal("Ada", answered.LDraftAuthor[1].LAuthorName);
    }

    [Fact]
    public void AuthorFind_TypedText_ReadsTheAuthorsItMatches()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada Lovelace"));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Brook Taylor"));
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Cam Adams"));

        IReadOnlyList<LAuthor> found = engine.TEngineAuthorFind("ada");

        Assert.Equal(["Ada Lovelace", "Cam Adams"], found.Select(author => author.LAuthorName));
    }

    private static long TRequestCardAdd(LEngine engine, long draftId)
    {
        LDraft answered = engine.TEngineRequestApply(
            TInterface.TRequestAdditionCreate(draftId, LCardKind.LCardKindMeaning, 0, int.MaxValue));
        return answered.LDraftContent.LEntryDraftMeanings[^1].LCardDraftId;
    }

    private static long TRequestSentenceAdd(LEngine engine, long draftId, long cardId, int position)
    {
        LDraft answered = engine.TEngineRequestApply(
            TInterface.TSentenceAdditionCreate(draftId, cardId, position));
        return TInterface.TRequestCardFind(answered.LDraftContent, cardId).LCardDraftSentence[position].LSentenceDraftId;
    }
}
