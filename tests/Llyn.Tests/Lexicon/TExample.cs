using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TExample
{
    [Fact]
    public void ExampleAttach_BothCardSides_ReadsBackFromEach()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TExampleEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        long collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LExample example = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0,
            "English",
            "he said the word",
            "그가 그 말을 했다",
            null));

        Assert.NotEqual(0, example.LExampleId);
        Assert.Equal(
            "그가 그 말을 했다",
            engine.TEngineExampleRead(example.LExampleId)!.LExampleTranslation.TStateValueShow());

        engine.TEngineExampleAttach(meaningId, example.LExampleId, 0, LOwner.LOwnerMeaning);
        engine.TEngineExampleAttach(collocationId, example.LExampleId, 0, LOwner.LOwnerCollocation);

        Assert.Equal(
            example.LExampleId,
            Assert.Single(engine.TEngineExampleRead(meaningId, LOwner.LOwnerMeaning)).LExampleId);
        Assert.Single(engine.TEngineExampleRead(collocationId, LOwner.LOwnerCollocation));

        engine.TEngineExampleUpdate(example with { LExampleText = "he spoke the word" });
        Assert.Equal("he spoke the word", engine.TEngineExampleRead(example.LExampleId)?.LExampleText);
    }

    [Fact]
    public void ExampleRemove_LastReferenceGone_DeletesExample()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TExampleEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        long collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LExample example = engine.TEngineExampleCreate(
            TInterface.TExampleCreate(
            0, "English", "he said the word", null, null));
        engine.TEngineExampleAttach(collocationId, example.LExampleId, 0, LOwner.LOwnerCollocation);
        engine.TEngineExampleAttach(meaningId, example.LExampleId, 0, LOwner.LOwnerMeaning);

        Assert.Throws<InvalidOperationException>(() => engine.TEngineExampleDelete(example.LExampleId));
        engine.TEngineExampleDetach(meaningId, example.LExampleId, LOwner.LOwnerMeaning);
        Assert.NotNull(engine.TEngineExampleRead(example.LExampleId));

        engine.TEngineExampleRemove(collocationId, example.LExampleId, LOwner.LOwnerCollocation);
        Assert.Null(engine.TEngineExampleRead(example.LExampleId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
    }

    [Fact]
    public void ExampleUpdate_CitingReference_MovesPointerOnly()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LExample example = engine.TEngineExampleCreate(
            TInterface.TExampleCreate(
            0, "English", "he said the word", null, null));
        LReference reference = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            TInterface.TStateValueCreate("A Dictionary"),
            TInterface.TStateValueCreate("1998"),
            LReferenceKind.LReferenceKindUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LState.LStateUnspecified));

        engine.TEngineExampleUpdate(example.LExampleId, TInterface.TStateAnchorRead(reference.LReferenceId));
        Assert.Equal(
            reference.LReferenceId,
            engine.TEngineExampleRead(example.LExampleId)?.LExampleSource.TStateAnchorShow());
        Assert.Equal(
            reference.LReferenceId,
            Assert.Single(engine.TEngineReferenceRead(example.LExampleId, LOwner.LOwnerExample))
                .LReferenceId);

        engine.TEngineExampleUpdate(example.LExampleId, LStateAnchor.LStateAnchorUnspecified);
        Assert.Equal(
            LStateAnchor.LStateAnchorUnspecified,
            engine.TEngineExampleRead(example.LExampleId)?.LExampleSource);
        Assert.NotNull(engine.TEngineReferenceRead(reference.LReferenceId));
        Assert.NotNull(engine.TEngineExampleRead(example.LExampleId));
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

        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        Assert.Equal(
            ["he said a word", "not a word was spoken", "a word of advice"],
            engine.TEngineExampleRead(meaningId, LOwner.LOwnerMeaning).Select(example => example.LExampleText));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LCardDraft card = loaded.LEntryDraftMeanings[0];
        Assert.Equal(3, card.LCardDraftSentence.Count);
        foreach (LSentenceDraft draft in card.LCardDraftSentence)
        {
            Assert.NotEqual(0, Assert.IsType<LExampleDraft>(draft.LSentenceDraftExample).LExampleDraftId);
        }

        LReference reference = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            TInterface.TStateValueCreate("A Dictionary"),
            LStateValue.LStateValueUnspecified,
            LReferenceKind.LReferenceKindUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LState.LStateUnspecified));
        long citedId = TExampleDraftRead(card.LCardDraftSentence[1]).LExampleDraftId;
        engine.TEngineExampleUpdate(citedId, TInterface.TStateAnchorRead(reference.LReferenceId));

        loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        card = loaded.LEntryDraftMeanings[0];
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

        IReadOnlyList<LExample> attached = engine.TEngineExampleRead(meaningId, LOwner.LOwnerMeaning);
        Assert.Equal(
            ["not one word was spoken", "he said a word", "in a word"],
            attached.Select(example => example.LExampleText));
        Assert.Equal(citedId, attached[0].LExampleId);
        Assert.Equal(reference.LReferenceId, attached[0].LExampleSource.TStateAnchorShow());
        Assert.Equal(
            TExampleDraftRead(card.LCardDraftSentence[0]).LExampleDraftId, attached[1].LExampleId);

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

        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        long exampleId = Assert.Single(engine.TEngineExampleRead(meaningId, LOwner.LOwnerMeaning)).LExampleId;

        LEntry other = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "say", "English", string.Empty, string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty, string.Empty, "to utter", [], [], [], [], [], 1)],
            []));
        long otherMeaningId = engine.TEngineMeaningRead(other.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        engine.TEngineExampleAttach(otherMeaningId, exampleId, 0, LOwner.LOwnerMeaning);

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

        LExample edited = Assert.Single(engine.TEngineExampleRead(meaningId, LOwner.LOwnerMeaning));
        LExample kept = Assert.Single(engine.TEngineExampleRead(otherMeaningId, LOwner.LOwnerMeaning));
        Assert.Equal("he said one word", edited.LExampleText.TStateValueShow());
        Assert.Equal("he said a word", kept.LExampleText.TStateValueShow());
        Assert.Equal(exampleId, kept.LExampleId);
        Assert.NotEqual(exampleId, edited.LExampleId);
    }

    [Fact]
    public void ExampleRead_WorkspaceStock_ReturnsUsageCounts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TExampleEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        long collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LExample shared = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0,
            "English",
            "he said the word",
            "그가 그 말을 했다",
            null));
        LExample lonely = engine.TEngineExampleCreate(
            TInterface.TExampleCreate(
            0, "English", "nobody quotes me", null, null));

        engine.TEngineExampleAttach(collocationId, shared.LExampleId, 0, LOwner.LOwnerCollocation);
        engine.TEngineExampleAttach(meaningId, shared.LExampleId, 0, LOwner.LOwnerMeaning);

        Assert.Equal(
            ["he said the word", "nobody quotes me"],
            engine.TEngineExampleRead().Select(row => row.LExampleText.TStateValueShow()));
        Assert.Equal(
            "그가 그 말을 했다",
            engine.TEngineExampleRead()[0].LExampleTranslation.TStateValueShow());

        IReadOnlyDictionary<long, int> counts = engine.TEngineUsageRead(LOwner.LOwnerExample);
        Assert.Equal(2, counts[shared.LExampleId]);
        Assert.DoesNotContain(lonely.LExampleId, counts);
    }

    [Fact]
    public void UsageRead_ExampleQuotedBothCardSides_NamesEachSide()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TExampleEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        long collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LExample example = engine.TEngineExampleCreate(
            TInterface.TExampleCreate(
            0, "English", "he said the word", null, null));
        engine.TEngineExampleAttach(meaningId, example.LExampleId, 0, LOwner.LOwnerMeaning);
        engine.TEngineExampleAttach(collocationId, example.LExampleId, 0, LOwner.LOwnerCollocation);

        IReadOnlyList<LUsage> usage = engine.TEngineUsageRead(example.LExampleId, LOwner.LOwnerExample);
        Assert.Equal(2, usage.Count);
        Assert.Equal(entry.LEntryId, usage.Single(row => row.LUsageOwner == LOwner.LOwnerMeaning).LUsageEntry);
        Assert.Equal(meaningId, usage.Single(row => row.LUsageOwner == LOwner.LOwnerMeaning).LUsageId);
        Assert.Equal(
            "in a word",
            usage.Single(row => row.LUsageOwner == LOwner.LOwnerCollocation).LUsageTitle.TStateValueShow());

        Assert.Throws<InvalidOperationException>(() => engine.TEngineExampleDelete(example.LExampleId));

        engine.TEngineExampleDelete(example.LExampleId, true);

        Assert.Null(engine.TEngineExampleRead(example.LExampleId));
        Assert.Empty(engine.TEngineExampleRead(meaningId, LOwner.LOwnerMeaning));
        Assert.Empty(engine.TEngineExampleRead(collocationId, LOwner.LOwnerCollocation));
        Assert.Empty(engine.TEngineUsageRead(example.LExampleId, LOwner.LOwnerExample));
    }

    private static LEntry TExampleEntryCreate(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)],
            [TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], [], [], 1)]));
    }

    private static LExampleDraft TExampleDraftRead(LSentenceDraft sentence)
    {
        return Assert.IsType<LExampleDraft>(sentence.LSentenceDraftExample);
    }
}
