using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TExample
{
    [Fact]
    public void ExampleRead_FortyThousandExamples_ReadsEveryGlossAndMention()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);
        workspace.TWorkspaceScriptRun(
            "WITH RECURSIVE tally(n) AS (SELECT 1 UNION ALL SELECT n + 1 FROM tally WHERE n < 40000) " +
            "INSERT INTO example (language, text_state, text) " +
            "SELECT 'English', 'specified', 'word ' || n FROM tally; " +
            "INSERT INTO example_translation (example_parent, position, language, text_state, text) " +
            "SELECT example_id, 0, 'Korean', 'specified', '말' FROM example; " +
            "INSERT INTO example_mention (example_parent, start, length) SELECT example_id, 0, 4 FROM example;");

        IReadOnlyList<LExample> read = examples.TExampleRead();

        Assert.Equal(40_000, read.Count);
        Assert.All(read, example => Assert.Single(example.LExampleGloss));
        Assert.All(read, example => Assert.Single(example.LExampleMention));
    }

    [Fact]
    public void ExampleCite_BothCardSides_ReadsBackFromEach()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LExample glossed = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0,
            "English",
            "he said the word",
            "그가 그 말을 했다",
            null));

        Assert.NotEqual(0, glossed.LExampleId);
        Assert.Equal(
            "그가 그 말을 했다",
            Assert.Single(engine.TEngineExampleRead(glossed.LExampleId)!.LExampleGloss).LGlossText.TStateValueShow());

        LExample example = engine.TEngineExampleCreate(
            TInterface.TExampleCreate(0, "English", "he spoke a word", null, null));
        LEntry entry = TExampleEntryCreate(engine, [TExampleCiteCreate(example)], [TExampleCiteCreate(example)]);

        Assert.Equal(
            example.LExampleId,
            TExampleDraftRead(Assert.Single(TExampleCardRead(engine, entry.LEntryId, false).LCardDraftSentence))
                .LExampleDraftId);
        Assert.Equal(
            example.LExampleId,
            TExampleDraftRead(Assert.Single(TExampleCardRead(engine, entry.LEntryId, true).LCardDraftSentence))
                .LExampleDraftId);

        engine.TEngineExampleUpdate(example with { LExampleText = "he spoke the word" });
        Assert.Equal("he spoke the word", engine.TEngineExampleRead(example.LExampleId)?.LExampleText);
    }

    [Fact]
    public void EntryUpdate_ExampleEdited_KeepsIdAndSource()
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
                [
                    TInterface.TSentenceDraftCreate("he said a word"),
                    TInterface.TSentenceDraftCreate("not a word was spoken"),
                    TInterface.TSentenceDraftCreate("a word of advice"),
                ],
                [],
                [],
                [], [], 1)],
            []));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LCardDraft card = loaded.LEntryDraftMeanings[0];
        Assert.Equal(
            ["he said a word", "not a word was spoken", "a word of advice"],
            card.LCardDraftSentence.Select(sentence => TExampleDraftRead(sentence).LExampleDraftText));
        foreach (LSentenceDraft draft in card.LCardDraftSentence)
        {
            Assert.NotEqual(0, TExampleDraftRead(draft).LExampleDraftId);
        }

        LReference reference = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            TInterface.TStateValueCreate("A Dictionary"),
            LStateValue.LStateValueUnspecified,
            LReferenceKind.LReferenceKindUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateMark.LStateMarkUnspecified));
        long citedId = TExampleDraftRead(card.LCardDraftSentence[1]).LExampleDraftId;
        LSentenceDraft sourced = card.LCardDraftSentence[1] with
        {
            LSentenceDraftExample = TExampleDraftRead(card.LCardDraftSentence[1]) with
            {
                LExampleDraftReference = TInterface.TStateAnchorRead(reference.LReferenceId),
            },
        };
        engine.TEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftMeanings =
            [
                card with
                {
                    LCardDraftSentence = [card.LCardDraftSentence[0], sourced, card.LCardDraftSentence[2]],
                },
            ],
        });

        loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        card = loaded.LEntryDraftMeanings[0];
        Assert.Equal(citedId, TExampleDraftRead(card.LCardDraftSentence[1]).LExampleDraftId);
        Assert.Equal(
            TInterface.TStateAnchorRead(reference.LReferenceId),
            TExampleDraftRead(card.LCardDraftSentence[1]).LExampleDraftReference);

        LSentenceDraft cited = card.LCardDraftSentence[1];

        engine.TEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftMeanings =
            [
                card with
                {
                    LCardDraftSentence =
                    [
                        cited with
                        {
                            LSentenceDraftExample = TExampleDraftRead(cited) with
                            {
                                LExampleDraftText = "not one word was spoken",
                            },
                        },
                        card.LCardDraftSentence[0],
                        TInterface.TSentenceDraftCreate("in a word"),
                    ],
                },
            ],
        });

        IReadOnlyList<LExampleDraft> attached =
        [
            .. TExampleCardRead(engine, entry.LEntryId, false).LCardDraftSentence.Select(TExampleDraftRead),
        ];
        Assert.Equal(
            ["not one word was spoken", "he said a word", "in a word"],
            attached.Select(example => example.LExampleDraftText));
        Assert.Equal(citedId, attached[0].LExampleDraftId);
        Assert.Equal(reference.LReferenceId, attached[0].LExampleDraftReference.TStateAnchorShow());
        Assert.Equal(TExampleDraftRead(card.LCardDraftSentence[0]).LExampleDraftId, attached[1].LExampleDraftId);

        Assert.NotNull(engine.TEngineExampleRead(
            TExampleDraftRead(card.LCardDraftSentence[2]).LExampleDraftId));
        Assert.Equal(4, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
        Assert.Equal(3, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));
    }

    [Fact]
    public void EntryUpdate_SharedExampleEdited_LeavesOtherCardsSentence()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty, string.Empty, "a unit of language",
                [TInterface.TSentenceDraftCreate("he said a word")],
                [], [], [], [], 1)],
            []));

        LSentenceDraft quoted = Assert.Single(TExampleCardRead(engine, entry.LEntryId, false).LCardDraftSentence);
        long exampleId = TExampleDraftRead(quoted).LExampleDraftId;

        LEntry other = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "say", "English", string.Empty, string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty, string.Empty, "to utter",
                [TInterface.TSentenceDraftCreate("he said a word", exampleId, LStateAnchor.LStateAnchorUnspecified)],
                [], [], [], [], 1)],
            []));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LCardDraft card = loaded.LEntryDraftMeanings[0];
        LSentenceDraft cited = card.LCardDraftSentence[0];
        engine.TEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftMeanings =
            [
                card with
                {
                    LCardDraftSentence =
                    [
                        cited with
                        {
                            LSentenceDraftExample = TExampleDraftRead(cited) with
                            {
                                LExampleDraftText = "he said one word",
                            },
                        },
                    ],
                },
            ],
        });

        LExampleDraft edited =
            TExampleDraftRead(Assert.Single(TExampleCardRead(engine, entry.LEntryId, false).LCardDraftSentence));
        LExampleDraft kept =
            TExampleDraftRead(Assert.Single(TExampleCardRead(engine, other.LEntryId, false).LCardDraftSentence));
        Assert.Equal("he said one word", edited.LExampleDraftText.TStateValueShow());
        Assert.Equal("he said a word", kept.LExampleDraftText.TStateValueShow());
        Assert.Equal(exampleId, kept.LExampleDraftId);
        Assert.NotEqual(exampleId, edited.LExampleDraftId);
    }

    [Fact]
    public void ExampleRead_WorkspaceStock_ReturnsUsageCounts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LExample shared = engine.TEngineExampleCreate(
            TInterface.TExampleCreate(0, "English", "he said the word", null, null));
        LExample lonely = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0,
            "English",
            "nobody quotes me",
            "아무도 나를 인용하지 않는다",
            null));

        TExampleEntryCreate(engine, [TExampleCiteCreate(shared)], [TExampleCiteCreate(shared)]);

        IReadOnlyList<LCatalogExample> stock =
        [
            .. engine.TEngineExampleFind(string.Empty, LCatalogOrder.LCatalogOrderEarliest)
                .OrderBy(row => row.LCatalogExampleStored.LExampleId),
        ];

        Assert.Equal(
            ["he said the word", "nobody quotes me"],
            stock.Select(row => row.LCatalogExampleStored.LExampleText.TStateValueShow()));
        Assert.Equal(
            "아무도 나를 인용하지 않는다",
            Assert.Single(stock[1].LCatalogExampleStored.LExampleGloss).LGlossText.TStateValueShow());

        IReadOnlyDictionary<long, int> counts = engine.TEngineUsageRead(LOwner.LOwnerExample);
        Assert.Equal(2, counts[shared.LExampleId]);
        Assert.DoesNotContain(lonely.LExampleId, counts);
    }

    [Fact]
    public void UsageRead_ExampleQuotedBothCardSides_NamesEachSide()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LExample example = engine.TEngineExampleCreate(
            TInterface.TExampleCreate(0, "English", "he said the word", null, null));
        LEntry entry = TExampleEntryCreate(engine, [TExampleCiteCreate(example)], [TExampleCiteCreate(example)]);
        long meaningId = TExampleCardRead(engine, entry.LEntryId, false).LCardDraftId;

        IReadOnlyList<LUsage> usage = engine.TEngineUsageRead(example.LExampleId, LOwner.LOwnerExample);
        Assert.Equal(2, usage.Count);
        Assert.Equal(entry.LEntryId, usage.Single(row => row.LUsageOwner == LOwner.LOwnerMeaning).LUsageEntry);
        Assert.Equal(meaningId, usage.Single(row => row.LUsageOwner == LOwner.LOwnerMeaning).LUsageId);
        Assert.Equal(
            "in a word",
            usage.Single(row => row.LUsageOwner == LOwner.LOwnerCollocation).LUsageTitle.TStateValueShow());

        Assert.Throws<InvalidOperationException>(() => engine.TEngineExampleDelete(example.LExampleId, false));

        engine.TEngineExampleDelete(example.LExampleId, true);

        Assert.Null(engine.TEngineExampleRead(example.LExampleId));
        Assert.DoesNotContain(
            TExampleCardRead(engine, entry.LEntryId, false).LCardDraftSentence,
            sentence => sentence.LSentenceDraftExample?.LExampleDraftId == example.LExampleId);
        Assert.DoesNotContain(
            TExampleCardRead(engine, entry.LEntryId, true).LCardDraftSentence,
            sentence => sentence.LSentenceDraftExample?.LExampleDraftId == example.LExampleId);
        Assert.Empty(engine.TEngineUsageRead(example.LExampleId, LOwner.LOwnerExample));
    }

    private static LEntry TExampleEntryCreate(
        LEngine engine,
        IReadOnlyList<LSentenceDraft> meaningSentences,
        IReadOnlyList<LSentenceDraft> collocationSentences)

    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", meaningSentences, [], [], [], [], 1)],
            [TInterface.TCardDraftCreate(
                string.Empty, "in a word", "briefly", collocationSentences, [], [], [], [], 1)]));
    }

    private static LSentenceDraft TExampleCiteCreate(LExample example)
    {
        return TInterface.TSentenceDraftCreate(example.LExampleText, example.LExampleId, example.LExampleSource);
    }

    private static LCardDraft TExampleCardRead(LEngine engine, long entryId, bool collocation)
    {
        LEntryDraft loaded = engine.TEngineEntryLoad(entryId)!;
        return collocation ? loaded.LEntryDraftCollocations[0] : loaded.LEntryDraftMeanings[0];
    }

    private static LExampleDraft TExampleDraftRead(LSentenceDraft sentence)
    {
        return Assert.IsType<LExampleDraft>(sentence.LSentenceDraftExample);
    }
}
