using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRequestGloss
{
    [Fact]
    public void RequestApply_GlossAddition_MintsNegativeIdWithLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        (long card, long sentence) = TGlossSentenceAdd(engine, started.LDraftId, "she knelt to kindle the damp logs");

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TGlossAdditionCreate(started.LDraftId, card, sentence, "Korean", 0));

        LGlossDraft gloss = Assert.Single(TGlossRead(answered, card));
        Assert.True(gloss.LGlossDraftId < 0);
        Assert.Equal("Korean", gloss.LGlossDraftLanguage);
        Assert.True(gloss.LGlossDraftText.LStateValueEmpty);
    }

    [Fact]
    public void RequestApply_GlossTextAndLanguage_ChangesOnlyThatRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        (long card, long sentence) = TGlossSentenceAdd(engine, started.LDraftId, "she knelt to kindle the damp logs");
        engine.TEngineRequestApply(TInterface.TGlossAdditionCreate(started.LDraftId, card, sentence, "Korean", 0));
        LDraft added = engine.TEngineRequestApply(
            TInterface.TGlossAdditionCreate(started.LDraftId, card, sentence, "French", 1));
        long second = TGlossRead(added, card)[1].LGlossDraftId;

        engine.TEngineRequestApply(TInterface.TGlossTextCreate(
            started.LDraftId, card, sentence, second, TInterface.TStateValueCreate("elle s'agenouilla")));
        LDraft answered = engine.TEngineRequestApply(
            TInterface.TGlossLanguageCreate(started.LDraftId, card, sentence, second, "German"));

        IReadOnlyList<LGlossDraft> glosses = TGlossRead(answered, card);
        Assert.Equal(["Korean", "German"], glosses.Select(gloss => gloss.LGlossDraftLanguage));
        Assert.True(glosses[0].LGlossDraftText.LStateValueEmpty);
        Assert.Equal("elle s'agenouilla", glosses[1].LGlossDraftText.TStateValueShow());
    }

    [Fact]
    public void RequestApply_GlossRemoval_DropsTheRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        (long card, long sentence) = TGlossSentenceAdd(engine, started.LDraftId, "she knelt to kindle the damp logs");
        engine.TEngineRequestApply(TInterface.TGlossAdditionCreate(started.LDraftId, card, sentence, "Korean", 0));
        LDraft added = engine.TEngineRequestApply(
            TInterface.TGlossAdditionCreate(started.LDraftId, card, sentence, "French", 1));
        long first = TGlossRead(added, card)[0].LGlossDraftId;

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TGlossRemovalCreate(started.LDraftId, card, sentence, first));

        LGlossDraft kept = Assert.Single(TGlossRead(answered, card));
        Assert.Equal("French", kept.LGlossDraftLanguage);
        Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TGlossRemovalCreate(started.LDraftId, card, sentence, first)));
    }

    [Fact]
    public void DraftCommit_SentenceGlosses_StoresRowsInOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        (long card, long sentence) = TGlossSentenceAdd(engine, started.LDraftId, "she knelt to kindle the damp logs");
        TGlossWrite(engine, started.LDraftId, card, sentence, "Korean", "그녀는 무릎을 꿇었다");
        TGlossWrite(engine, started.LDraftId, card, sentence, "French", "elle s'agenouilla");

        LEntry entry = engine.TEngineDraftCommit(started.LDraftId);

        LExampleDraft example = TGlossExampleRead(engine, entry.LEntryId);
        Assert.Equal(["Korean", "French"], example.LExampleDraftGloss.Select(gloss => gloss.LGlossDraftLanguage));
        Assert.All(example.LExampleDraftGloss, gloss => Assert.True(gloss.LGlossDraftId > 0));
        LExample stored = engine.TEngineExampleRead(example.LExampleDraftId)!;
        Assert.Equal(
            ["그녀는 무릎을 꿇었다", "elle s'agenouilla"],
            stored.LExampleGloss.Select(gloss => gloss.LGlossText.TStateValueShow()));
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example_translation;"));
    }

    [Fact]
    public void DraftCommit_SharedExampleGlossChanged_UpdatesInPlaceWithoutForking()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LExample stored = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0, "English", "she knelt to kindle the damp logs", "그녀는 무릎을 꿇었다", null));
        long first = TGlossSharedCommit(engine, stored.LExampleId, "ember");

        LDraft started = engine.TEngineDraftStart("Input", null);
        (long card, long sentence) = TGlossSentenceAdd(engine, started.LDraftId, "flame");
        LDraft picked = engine.TEngineRequestApply(
            TInterface.TSentenceExampleCreate(started.LDraftId, card, sentence, stored.LExampleId));
        long gloss = Assert.Single(TGlossRead(picked, card)).LGlossDraftId;
        engine.TEngineRequestApply(TInterface.TGlossTextCreate(
            started.LDraftId, card, sentence, gloss, TInterface.TStateValueCreate("그녀는 불을 지폈다")));
        TGlossWrite(engine, started.LDraftId, card, sentence, "French", "elle s'agenouilla");
        LEntry second = engine.TEngineDraftCommit(started.LDraftId);

        LExampleDraft kept = TGlossExampleRead(engine, first);
        LExampleDraft shared = TGlossExampleRead(engine, second.LEntryId);
        Assert.Equal(kept.LExampleDraftId, shared.LExampleDraftId);
        Assert.Equal(
            ["그녀는 불을 지폈다", "elle s'agenouilla"],
            kept.LExampleDraftGloss.Select(row => row.LGlossDraftText.TStateValueShow()));
        Assert.Equal(gloss, kept.LExampleDraftGloss[0].LGlossDraftId);
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
    }

    [Fact]
    public void ExampleCommit_CorpusGlosses_HoldsRowsOnTheExample()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineExampleStart("Corpus", null);
        engine.TEngineRequestApply(TInterface.TExampleBodyCreate(
            started.LDraftId,
            TInterface.TExampleCreate(0, "English", TInterface.TStateValueCreate("a line"), null, null)));
        LDraft added = engine.TEngineRequestApply(
            TInterface.TGlossAdditionCreate(started.LDraftId, 0, 0, "Korean", 0));
        long gloss = Assert.Single(added.LDraftExample!.LExampleGloss).LGlossId;
        engine.TEngineRequestApply(TInterface.TGlossTextCreate(
            started.LDraftId, 0, 0, gloss, TInterface.TStateValueCreate("한 줄")));
        engine.TEngineRequestApply(TInterface.TGlossLanguageCreate(started.LDraftId, 0, 0, gloss, "Japanese"));

        LExample settled = engine.TEngineExampleCommit(started.LDraftId);

        LGloss stored = Assert.Single(engine.TEngineExampleRead(settled.LExampleId)!.LExampleGloss);
        Assert.True(stored.LGlossId > 0);
        Assert.Equal("Japanese", stored.LGlossLanguage);
        Assert.Equal("한 줄", stored.LGlossText.TStateValueShow());
    }

    private static void TGlossWrite(
        LEngine engine, long draftId, long card, long sentence, string language, string text)
    {
        LDraft added = engine.TEngineRequestApply(
            TInterface.TGlossAdditionCreate(draftId, card, sentence, language, int.MaxValue));
        long gloss = TGlossRead(added, card)[^1].LGlossDraftId;
        engine.TEngineRequestApply(
            TInterface.TGlossTextCreate(draftId, card, sentence, gloss, TInterface.TStateValueCreate(text)));
    }

    private static long TGlossSharedCommit(LEngine engine, long exampleId, string headword)
    {
        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, headword));
        (long card, long sentence) = TGlossSentenceAdd(engine, started.LDraftId, headword);
        engine.TEngineRequestApply(TInterface.TSentenceExampleCreate(started.LDraftId, card, sentence, exampleId));
        return engine.TEngineDraftCommit(started.LDraftId).LEntryId;
    }

    private static (long TRequestGlossCard, long TRequestGlossSentence) TGlossSentenceAdd(
        LEngine engine, long draftId, string text)
    {
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(draftId, "kindle"));
        LDraft carded = engine.TEngineRequestApply(
            TInterface.TRequestAdditionCreate(draftId, LCardKind.LCardKindMeaning, 0, int.MaxValue));
        long card = carded.LDraftContent.LEntryDraftMeanings[^1].LCardDraftId;
        LDraft rowed = engine.TEngineRequestApply(TInterface.TSentenceAdditionCreate(draftId, card, 0));
        long sentence = TInterface.TRequestCardFind(rowed.LDraftContent, card).LCardDraftSentence[0].LSentenceDraftId;
        engine.TEngineRequestApply(
            TInterface.TSentenceTextCreate(draftId, card, sentence, TInterface.TStateValueCreate(text)));
        return (card, sentence);
    }

    private static IReadOnlyList<LGlossDraft> TGlossRead(LDraft draft, long card)
    {
        LSentenceDraft sentence = TInterface.TRequestCardFind(draft.LDraftContent, card).LCardDraftSentence[0];
        return sentence.LSentenceDraftExample?.LExampleDraftGloss ?? [];
    }

    private static LExampleDraft TGlossExampleRead(LEngine engine, long entryId)
    {
        LEntryDraft loaded = engine.TEngineEntryLoad(entryId)!;
        return loaded.LEntryDraftMeanings[0].LCardDraftSentence[0].LSentenceDraftExample!;
    }
}
