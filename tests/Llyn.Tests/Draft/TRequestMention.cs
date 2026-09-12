using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRequestMention
{
    [Fact]
    public void RequestApply_MentionAddition_MintsNegativeIdAndSortsByOffset()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (long entryId, _) = TMentionEntryCreate(engine, "logs");
        LDraft started = engine.TEngineDraftStart("Input", null);
        (long card, long sentence) = TMentionSentenceAdd(engine, started.LDraftId, "she knelt to kindle the damp logs");

        engine.TEngineRequestApply(TInterface.TMentionAdditionCreate(started.LDraftId, card, sentence, 29, 4, entryId));
        LDraft answered = engine.TEngineRequestApply(
            TInterface.TMentionAdditionCreate(started.LDraftId, card, sentence, 13, 6, 0));

        IReadOnlyList<LMentionDraft> mentions = TMentionRead(answered, card);
        Assert.Equal([13, 29], mentions.Select(mention => mention.LMentionDraftOffset));
        Assert.All(mentions, mention => Assert.True(mention.LMentionDraftId < 0));
        Assert.Equal(0, mentions[0].LMentionDraftEntry);
        Assert.Equal(entryId, mentions[1].LMentionDraftEntry);
    }

    [Fact]
    public void RequestApply_MentionAdditionOverExistingSpan_ReplacesIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (long entryId, long senseId) = TMentionEntryCreate(engine, "kindle");
        LDraft started = engine.TEngineDraftStart("Input", null);
        (long card, long sentence) = TMentionSentenceAdd(engine, started.LDraftId, "she knelt to kindle the damp logs");

        engine.TEngineRequestApply(TInterface.TMentionAdditionCreate(started.LDraftId, card, sentence, 13, 6, 0));
        LDraft answered = engine.TEngineRequestApply(
            TInterface.TMentionAdditionCreate(started.LDraftId, card, sentence, 10, 9, entryId, senseId));

        LMentionDraft mention = Assert.Single(TMentionRead(answered, card));
        Assert.Equal(10, mention.LMentionDraftOffset);
        Assert.Equal(entryId, mention.LMentionDraftEntry);
        Assert.Equal(senseId, mention.LMentionDraftSense);
    }

    [Fact]
    public void RequestApply_MentionAdditionSenseOfAnotherEntry_RefusesLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (long entryId, _) = TMentionEntryCreate(engine, "kindle");
        (_, long otherSense) = TMentionEntryCreate(engine, "logs");
        LDraft started = engine.TEngineDraftStart("Input", null);
        (long card, long sentence) = TMentionSentenceAdd(engine, started.LDraftId, "she knelt to kindle the damp logs");

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TMentionAdditionCreate(started.LDraftId, card, sentence, 13, 6, entryId, otherSense)));
        LRefusal past = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TMentionAdditionCreate(started.LDraftId, card, sentence, 30, 6, entryId)));

        Assert.Equal(LRefusal.LRefusalLink, refusal.LRefusalReason);
        Assert.Equal(LRefusal.LRefusalItem, past.LRefusalReason);
        Assert.Empty(TMentionRead(engine.TEngineDraftRead(started.LDraftId)!, card));
    }

    [Fact]
    public void RequestApply_MentionAdditionWithZeroCard_LandsOnTheDraftExample()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (long entryId, _) = TMentionEntryCreate(engine, "kindle");
        LDraft started = engine.TEngineExampleStart("Corpus", null);
        engine.TEngineRequestApply(TInterface.TExampleTextCreate(
            started.LDraftId, TInterface.TStateValueCreate("she knelt to kindle the damp logs")));

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TMentionAdditionCreate(started.LDraftId, 0, 0, 13, 6, entryId));

        LMention mention = Assert.Single(answered.LDraftExample!.LExampleMention);
        Assert.True(mention.LMentionId < 0);
        Assert.Equal(entryId, mention.LMentionEntryId);
        Assert.True(engine.TEngineDraftCheck(started.LDraftId));

        LExample stored = engine.TEngineExampleCommit(started.LDraftId);
        LMention written = Assert.Single(engine.TEngineExampleRead(stored.LExampleId)!.LExampleMention);
        Assert.True(written.LMentionId > 0);
        Assert.Equal(13, written.LMentionOffset);
    }

    [Fact]
    public void RequestApply_MentionSense_ClearsWithZeroAndRefusesOnNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (long entryId, long senseId) = TMentionEntryCreate(engine, "kindle");
        LDraft started = engine.TEngineDraftStart("Input", null);
        (long card, long sentence) = TMentionSentenceAdd(engine, started.LDraftId, "she knelt to kindle the damp logs");
        engine.TEngineRequestApply(TInterface.TMentionAdditionCreate(started.LDraftId, card, sentence, 13, 6, entryId, senseId));
        LDraft held = engine.TEngineRequestApply(
            TInterface.TMentionAdditionCreate(started.LDraftId, card, sentence, 29, 4, 0));
        long linked = TMentionRead(held, card)[0].LMentionDraftId;
        long nothing = TMentionRead(held, card)[1].LMentionDraftId;

        LDraft cleared = engine.TEngineRequestApply(
            TInterface.TMentionSenseCreate(started.LDraftId, card, sentence, linked, 0));
        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TMentionSenseCreate(started.LDraftId, card, sentence, nothing, senseId)));

        Assert.Equal(0, TMentionRead(cleared, card)[0].LMentionDraftSense);
        Assert.Equal(LRefusal.LRefusalLink, refusal.LRefusalReason);

        LDraft removed = engine.TEngineRequestApply(
            TInterface.TMentionRemovalCreate(started.LDraftId, card, sentence, nothing));
        Assert.Equal(linked, Assert.Single(TMentionRead(removed, card)).LMentionDraftId);
    }

    [Fact]
    public void RequestApply_SentenceTextEdited_ShiftsKeepsOrDropsMentions()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (long entryId, _) = TMentionEntryCreate(engine, "kindle");
        LDraft started = engine.TEngineDraftStart("Input", null);
        (long card, long sentence) = TMentionSentenceAdd(engine, started.LDraftId, "she knelt to kindle the damp logs");
        engine.TEngineRequestApply(TInterface.TMentionAdditionCreate(started.LDraftId, card, sentence, 13, 6, entryId));
        engine.TEngineRequestApply(TInterface.TMentionAdditionCreate(started.LDraftId, card, sentence, 29, 4, 0));

        LDraft inserted = TMentionTextApply(engine, started.LDraftId, card, sentence, "then she knelt to kindle the damp logs");
        Assert.Equal([18, 34], TMentionRead(inserted, card).Select(mention => mention.LMentionDraftOffset));

        LDraft appended = TMentionTextApply(engine, started.LDraftId, card, sentence, "then she knelt to kindle the damp logs again");
        Assert.Equal([18, 34], TMentionRead(appended, card).Select(mention => mention.LMentionDraftOffset));

        LDraft edited = TMentionTextApply(engine, started.LDraftId, card, sentence, "then she knelt to candle the damp logs again");
        Assert.Equal(34, Assert.Single(TMentionRead(edited, card)).LMentionDraftOffset);

        LDraft shortened = TMentionTextApply(engine, started.LDraftId, card, sentence, "then she knelt to candle the");
        Assert.Empty(TMentionRead(shortened, card));
        engine.TEngineDraftCommit(started.LDraftId);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example_mention;"));
    }

    [Fact]
    public void DraftCommit_MentionsHeld_WritesRowsAndMapsIds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (long entryId, long senseId) = TMentionEntryCreate(engine, "kindle");
        LDraft started = engine.TEngineDraftStart("Input", null);
        (long card, long sentence) = TMentionSentenceAdd(engine, started.LDraftId, "she knelt to kindle the damp logs");
        engine.TEngineRequestApply(TInterface.TMentionAdditionCreate(started.LDraftId, card, sentence, 13, 6, entryId));
        LDraft held = engine.TEngineRequestApply(
            TInterface.TMentionAdditionCreate(started.LDraftId, card, sentence, 29, 4, 0));
        IReadOnlyList<LMentionDraft> minted = TMentionRead(held, card);

        LOutcome outcome = engine.TEngineOutcomeCommit(started.LDraftId);

        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example_mention;"));
        LExampleDraft example = TMentionExampleRead(engine, outcome.LOutcomeEntry.LEntryId);
        Assert.Equal(outcome.LOutcomeIdentity[minted[0].LMentionDraftId], example.LExampleDraftMention[0].LMentionDraftId);
        Assert.Equal(outcome.LOutcomeIdentity[minted[1].LMentionDraftId], example.LExampleDraftMention[1].LMentionDraftId);
        Assert.Equal(0, example.LExampleDraftMention[1].LMentionDraftEntry);

        LDraft again = engine.TEngineDraftStart("Input", outcome.LOutcomeEntry.LEntryId);
        Assert.False(engine.TEngineDraftCheck(again.LDraftId));
        long cardId = again.LDraftContent.LEntryDraftMeanings[0].LCardDraftId;
        long sentenceId = again.LDraftContent.LEntryDraftMeanings[0].LCardDraftSentence[0].LSentenceDraftId;
        engine.TEngineRequestApply(TInterface.TMentionSenseCreate(
            again.LDraftId, cardId, sentenceId, example.LExampleDraftMention[0].LMentionDraftId, senseId));
        Assert.True(engine.TEngineDraftCheck(again.LDraftId));
        engine.TEngineDraftCommit(again.LDraftId);

        LExampleDraft rewritten = TMentionExampleRead(engine, outcome.LOutcomeEntry.LEntryId);
        Assert.Equal(example.LExampleDraftId, rewritten.LExampleDraftId);
        Assert.Equal(
            example.LExampleDraftMention.Select(mention => mention.LMentionDraftId),
            rewritten.LExampleDraftMention.Select(mention => mention.LMentionDraftId));
        Assert.Equal(senseId, rewritten.LExampleDraftMention[0].LMentionDraftSense);
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example_mention;"));
    }

    [Fact]
    public void DraftCommit_SharedExampleTextChanged_ForksWithTheDraftMentions()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (long entryId, _) = TMentionEntryCreate(engine, "kindle");
        LExample stored = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0, "English", "she knelt to kindle the damp logs", null, null) with
        {
            LExampleMention = [TInterface.TMentionCreate(0, 29, 4, entryId)],
        });
        long first = TMentionSharedCommit(engine, stored.LExampleId, "ember");

        LDraft started = engine.TEngineDraftStart("Input", null);
        (long card, long sentence) = TMentionSentenceAdd(engine, started.LDraftId, "flame");
        engine.TEngineRequestApply(TInterface.TSentenceExampleCreate(started.LDraftId, card, sentence, stored.LExampleId));
        engine.TEngineRequestApply(TInterface.TMentionAdditionCreate(started.LDraftId, card, sentence, 13, 6, entryId));
        TMentionTextApply(engine, started.LDraftId, card, sentence, "she knelt to kindle the damp logs again");
        LEntry second = engine.TEngineDraftCommit(started.LDraftId);

        LExampleDraft kept = TMentionExampleRead(engine, first);
        LExampleDraft forked = TMentionExampleRead(engine, second.LEntryId);
        Assert.NotEqual(kept.LExampleDraftId, forked.LExampleDraftId);
        Assert.Equal([29], kept.LExampleDraftMention.Select(mention => mention.LMentionDraftOffset));
        Assert.Equal([13, 29], forked.LExampleDraftMention.Select(mention => mention.LMentionDraftOffset));
        Assert.All(forked.LExampleDraftMention, mention => Assert.True(mention.LMentionDraftId > 0));
        Assert.Equal(3, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example_mention;"));
    }

    [Fact]
    public void DraftArchiveSave_MentionWithNegativeIdAndNoEntry_RoundTrips()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        (long card, long sentence) = TMentionSentenceAdd(engine, started.LDraftId, "she knelt to kindle the damp logs");
        LDraft held = engine.TEngineRequestApply(
            TInterface.TMentionAdditionCreate(started.LDraftId, card, sentence, 13, 6, 0));

        LDraft read = TInterface.TDraftArchiveRead(workspace.TWorkspaceFolder, started.LDraftId)!;

        LMentionDraft mention = Assert.Single(TMentionRead(read, card));
        Assert.Equal(TMentionRead(held, card)[0], mention);
        Assert.True(mention.LMentionDraftId < 0);
        Assert.Equal(0, mention.LMentionDraftEntry);
    }

    [Fact]
    public void RequestApply_SelectionSpanningSurrogatePair_CarriesCodePointOffsets()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (long entryId, _) = TMentionEntryCreate(engine, "kindle");
        LDraft started = engine.TEngineDraftStart("Input", null);
        const string text = "🔥 she knelt to kindle the damp logs";
        (long card, long sentence) = TMentionSentenceAdd(engine, started.LDraftId, text);
        int unit = text.IndexOf("kindle", StringComparison.Ordinal);
        int offset = TInterface.TMentionOffsetRead(text, unit);
        int length = TInterface.TMentionOffsetRead(text, unit + 6) - offset;

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TMentionAdditionCreate(started.LDraftId, card, sentence, offset, length, entryId));

        LMentionDraft mention = Assert.Single(TMentionRead(answered, card));
        Assert.Equal(15, mention.LMentionDraftOffset);
        Assert.Equal(6, mention.LMentionDraftLength);
        Assert.Equal(unit, TInterface.TMentionUnitRead(text, mention.LMentionDraftOffset));
    }

    [Fact]
    public void RequestApply_LinkChooseUnlinkSequence_LeavesNoMentionAndSilenceRefusesSense()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (long entryId, long senseId) = TMentionEntryCreate(engine, "kindle");
        LDraft started = engine.TEngineDraftStart("Input", null);
        (long card, long sentence) = TMentionSentenceAdd(engine, started.LDraftId, "she knelt to kindle the damp logs");

        LDraft linked = engine.TEngineRequestApply(
            TInterface.TMentionAdditionCreate(started.LDraftId, card, sentence, 13, 6, entryId));
        long mentionId = Assert.Single(TMentionRead(linked, card)).LMentionDraftId;
        LDraft chosen = engine.TEngineRequestApply(
            TInterface.TMentionSenseCreate(started.LDraftId, card, sentence, mentionId, senseId));
        Assert.Equal(senseId, Assert.Single(TMentionRead(chosen, card)).LMentionDraftSense);
        LDraft unlinked = engine.TEngineRequestApply(
            TInterface.TMentionRemovalCreate(started.LDraftId, card, sentence, mentionId));
        Assert.Empty(TMentionRead(unlinked, card));

        LDraft silenced = engine.TEngineRequestApply(
            TInterface.TMentionAdditionCreate(started.LDraftId, card, sentence, 13, 6, 0));
        long silent = Assert.Single(TMentionRead(silenced, card)).LMentionDraftId;
        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TMentionSenseCreate(started.LDraftId, card, sentence, silent, senseId)));
        Assert.Equal(LRefusal.LRefusalLink, refusal.LRefusalReason);
    }

    [Fact]
    public void ExampleCommit_MentionLinkedInTheScribe_IsFoundAtItsOffset()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (long entryId, long senseId) = TMentionEntryCreate(engine, "kindle");
        LDraft started = engine.TEngineExampleStart("Corpus", null);
        engine.TEngineRequestApply(TInterface.TExampleTextCreate(
            started.LDraftId, TInterface.TStateValueCreate("she knelt to kindle the damp logs")));
        LDraft held = engine.TEngineRequestApply(
            TInterface.TMentionAdditionCreate(started.LDraftId, 0, 0, 13, 6, entryId));
        long mentionId = Assert.Single(held.LDraftExample!.LExampleMention).LMentionId;
        engine.TEngineRequestApply(TInterface.TMentionSenseCreate(started.LDraftId, 0, 0, mentionId, senseId));

        LExample stored = engine.TEngineExampleCommit(started.LDraftId);
        LMentionResult found = engine.TEngineMentionFind(stored.LExampleId, 15);

        LMention mention = Assert.IsType<LMention>(found.LMentionResultStored);
        Assert.True(mention.LMentionId > 0);
        Assert.Equal(13, mention.LMentionOffset);
        Assert.Equal(6, mention.LMentionLength);
        Assert.Equal(entryId, mention.LMentionEntryId);
        Assert.Equal(senseId, mention.LMentionSenseId);
    }

    private static long TMentionSharedCommit(LEngine engine, long exampleId, string headword)
    {
        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, headword));
        (long card, long sentence) = TMentionSentenceAdd(engine, started.LDraftId, headword);
        engine.TEngineRequestApply(TInterface.TSentenceExampleCreate(started.LDraftId, card, sentence, exampleId));
        return engine.TEngineDraftCommit(started.LDraftId).LEntryId;
    }

    private static (long TRequestMentionEntry, long TRequestMentionSense) TMentionEntryCreate(
        LEngine engine, string headword)
    {
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(
                    TInterface.TStateValueCreate("set alight"),
                    LStateValue.LStateValueUnspecified,
                    TInterface.TStateValueCreate("to set something burning"),
                    [],
                    [],
                    [],
                    [],
                    [],
                    1,
                    TInterface.TIdentityCreate()),
            ],
            []));

        return (entry.LEntryId, Assert.Single(engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)).LMeaningId);
    }

    private static (long TRequestMentionCard, long TRequestMentionSentence) TMentionSentenceAdd(
        LEngine engine, long draftId, string text)
    {
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(draftId, "kindle"));
        LDraft carded = engine.TEngineRequestApply(
            TInterface.TRequestAdditionCreate(draftId, LCardKind.LCardKindMeaning, 0, int.MaxValue));
        long card = carded.LDraftContent.LEntryDraftMeanings[^1].LCardDraftId;
        LDraft rowed = engine.TEngineRequestApply(TInterface.TSentenceAdditionCreate(draftId, card, 0));
        long sentence = TInterface.TRequestCardFind(rowed.LDraftContent, card).LCardDraftSentence[0].LSentenceDraftId;
        TMentionTextApply(engine, draftId, card, sentence, text);
        return (card, sentence);
    }

    private static LDraft TMentionTextApply(LEngine engine, long draftId, long card, long sentence, string text)
    {
        return engine.TEngineRequestApply(
            TInterface.TSentenceTextCreate(draftId, card, sentence, TInterface.TStateValueCreate(text)));
    }

    private static IReadOnlyList<LMentionDraft> TMentionRead(LDraft draft, long card)
    {
        LSentenceDraft sentence = TInterface.TRequestCardFind(draft.LDraftContent, card).LCardDraftSentence[0];
        return sentence.LSentenceDraftExample?.LExampleDraftMention ?? [];
    }

    private static LExampleDraft TMentionExampleRead(LEngine engine, long entryId)
    {
        LEntryDraft loaded = engine.TEngineEntryLoad(entryId)!;
        return loaded.LEntryDraftMeanings[0].LCardDraftSentence[0].LSentenceDraftExample!;
    }
}
