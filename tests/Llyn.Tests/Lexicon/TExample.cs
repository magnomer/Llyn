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
    public void OneExampleIsQuotedFromAllThreeSidesAndReadBackFromEach()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TExampleEntryCreate(engine);
        string senseId = engine.TEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        string collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LExample example = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            string.Empty,
            "English",
            "he said the word",
            "그가 그 말을 했다",
            null));

        Assert.NotEmpty(example.LExampleId);
        Assert.Equal(
            "그가 그 말을 했다",
            engine.TEngineExampleRead(example.LExampleId)!.LExampleTranslation.TStateValueShow());

        engine.TEngineExampleAttach(entry.LEntryId, example.LExampleId, 0, LOwner.LOwnerEntry);
        engine.TEngineExampleAttach(senseId, example.LExampleId, 0, LOwner.LOwnerSense);
        engine.TEngineExampleAttach(collocationId, example.LExampleId, 0, LOwner.LOwnerCollocation);

        Assert.Equal(
            example.LExampleId,
            Assert.Single(engine.TEngineExampleRead(entry.LEntryId, LOwner.LOwnerEntry)).LExampleId);
        Assert.Single(engine.TEngineExampleRead(senseId, LOwner.LOwnerSense));
        Assert.Single(engine.TEngineExampleRead(collocationId, LOwner.LOwnerCollocation));

        engine.TEngineExampleUpdate(example with { LExampleText = "he spoke the word" });
        Assert.Equal("he spoke the word", engine.TEngineExampleRead(example.LExampleId)?.LExampleText);
    }

    [Fact]
    public void AnExampleNothingQuotesAnyMoreGoesWithItsLastReference()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TExampleEntryCreate(engine);
        string senseId = engine.TEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;

        LExample example = engine.TEngineExampleCreate(
            TInterface.TExampleCreate(string.Empty, "English", "he said the word", null, null));
        engine.TEngineExampleAttach(entry.LEntryId, example.LExampleId, 0, LOwner.LOwnerEntry);
        engine.TEngineExampleAttach(senseId, example.LExampleId, 0, LOwner.LOwnerSense);

        Assert.Throws<InvalidOperationException>(() => engine.TEngineExampleDelete(example.LExampleId));
        engine.TEngineExampleDetach(senseId, example.LExampleId, LOwner.LOwnerSense);
        Assert.NotNull(engine.TEngineExampleRead(example.LExampleId));

        engine.TEngineExampleRemove(entry.LEntryId, example.LExampleId, LOwner.LOwnerEntry);
        Assert.Null(engine.TEngineExampleRead(example.LExampleId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
    }

    [Fact]
    public void CitingAReferenceMovesThePointerAndNothingElse()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LExample example = engine.TEngineExampleCreate(
            TInterface.TExampleCreate(string.Empty, "English", "he said the word", null, null));
        LReference reference = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            string.Empty,
            TInterface.TStateValueCreate("A Dictionary"),
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            TInterface.TStateValueCreate("1998"),
            LStateValue.LStateValueUnspecified,
            LState.LStateUnspecified));

        engine.TEngineExampleUpdate(example.LExampleId, reference.LReferenceId);
        Assert.Equal(
            reference.LReferenceId,
            engine.TEngineExampleRead(example.LExampleId)?.LExampleSource.TStateValueShow());
        Assert.Equal(
            reference.LReferenceId,
            Assert.Single(engine.TEngineReferenceRead(example.LExampleId, LOwner.LOwnerExample))
                .LReferenceId);

        engine.TEngineExampleUpdate(example.LExampleId, LStateValue.LStateValueUnspecified);
        Assert.Equal(
            LStateValue.LStateValueUnspecified,
            engine.TEngineExampleRead(example.LExampleId)?.LExampleSource);
        Assert.NotNull(engine.TEngineReferenceRead(reference.LReferenceId));
        Assert.NotNull(engine.TEngineExampleRead(example.LExampleId));
    }

    [Fact]
    public void AMeaningKeepsEveryExampleItRefersToAndEditsOneWithoutLosingItsIdOrSource()
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
                    TInterface.TExampleDraftCreate("he said a word"),
                    TInterface.TExampleDraftCreate("not a word was spoken"),
                    TInterface.TExampleDraftCreate("a word of advice"),
                ],
                [],
                [],
                string.Empty,
                [], [], 1)],
            []));

        string senseId = engine.TEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        Assert.Equal(
            ["he said a word", "not a word was spoken", "a word of advice"],
            engine.TEngineExampleRead(senseId, LOwner.LOwnerSense).Select(example => example.LExampleText));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LCardDraft card = loaded.LEntryDraftSenses[0];
        Assert.Equal(3, card.LCardDraftExample.Count);
        foreach (LExampleDraft draft in card.LCardDraftExample)
        {
            Assert.NotEmpty(draft.LExampleDraftId);
        }

        LReference reference = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            string.Empty,
            TInterface.TStateValueCreate("A Dictionary"),
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LState.LStateUnspecified));
        string citedId = card.LCardDraftExample[1].LExampleDraftId;
        engine.TEngineExampleUpdate(citedId, reference.LReferenceId);

        loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        card = loaded.LEntryDraftSenses[0];
        Assert.Equal(reference.LReferenceId, card.LCardDraftExample[1].LExampleDraftReference);

        engine.TEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftSenses =
            [
                card with
                {
                    LCardDraftExample =
                    [
                        card.LCardDraftExample[1] with { LExampleDraftText = "not one word was spoken" },
                        card.LCardDraftExample[0],
                        TInterface.TExampleDraftCreate("in a word"),
                    ],
                },
            ],
        });

        IReadOnlyList<LExample> attached = engine.TEngineExampleRead(senseId, LOwner.LOwnerSense);
        Assert.Equal(
            ["not one word was spoken", "he said a word", "in a word"],
            attached.Select(example => example.LExampleText));
        Assert.Equal(citedId, attached[0].LExampleId);
        Assert.Equal(reference.LReferenceId, attached[0].LExampleSource.TStateValueShow());
        Assert.Equal(card.LCardDraftExample[0].LExampleDraftId, attached[1].LExampleId);

        Assert.NotNull(engine.TEngineExampleRead(card.LCardDraftExample[2].LExampleDraftId));
        Assert.Equal(4, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
        Assert.Equal(3, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));
    }

    [Fact]
    public void TheWorkspaceStockOfExamplesIsReadWithItsUsageCounted()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TExampleEntryCreate(engine);
        string senseId = engine.TEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;

        LExample shared = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            string.Empty,
            "English",
            "he said the word",
            "그가 그 말을 했다",
            null));
        LExample lonely = engine.TEngineExampleCreate(
            TInterface.TExampleCreate(string.Empty, "English", "nobody quotes me", null, null));

        engine.TEngineExampleAttach(entry.LEntryId, shared.LExampleId, 0, LOwner.LOwnerEntry);
        engine.TEngineExampleAttach(senseId, shared.LExampleId, 0, LOwner.LOwnerSense);

        Assert.Equal(
            ["he said the word", "nobody quotes me"],
            engine.TEngineExampleRead().Select(row => row.LExampleText.TStateValueShow()));
        Assert.Equal(
            "그가 그 말을 했다",
            engine.TEngineExampleRead()[0].LExampleTranslation.TStateValueShow());

        IReadOnlyDictionary<string, int> counts = engine.TEngineUsageRead(LOwner.LOwnerExample);
        Assert.Equal(2, counts[shared.LExampleId]);
        Assert.DoesNotContain(lonely.LExampleId, counts);
    }

    [Fact]
    public void UsageNamesEveryQuotingSideAndDetachingDeleteDropsThemAll()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TExampleEntryCreate(engine);
        string senseId = engine.TEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        string collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LExample example = engine.TEngineExampleCreate(
            TInterface.TExampleCreate(string.Empty, "English", "he said the word", null, null));
        engine.TEngineExampleAttach(entry.LEntryId, example.LExampleId, 0, LOwner.LOwnerEntry);
        engine.TEngineExampleAttach(senseId, example.LExampleId, 0, LOwner.LOwnerSense);
        engine.TEngineExampleAttach(collocationId, example.LExampleId, 0, LOwner.LOwnerCollocation);

        IReadOnlyList<LUsage> usage = engine.TEngineUsageRead(example.LExampleId, LOwner.LOwnerExample);
        Assert.Equal(3, usage.Count);
        Assert.Equal(entry.LEntryId, usage.Single(row => row.LUsageOwner == LOwner.LOwnerEntry).LUsageEntry);
        Assert.Equal(senseId, usage.Single(row => row.LUsageOwner == LOwner.LOwnerSense).LUsageId);
        Assert.Equal(
            "in a word",
            usage.Single(row => row.LUsageOwner == LOwner.LOwnerCollocation).LUsageTitle.TStateValueShow());

        Assert.Throws<InvalidOperationException>(() => engine.TEngineExampleDelete(example.LExampleId));

        engine.TEngineExampleDelete(example.LExampleId, true);

        Assert.Null(engine.TEngineExampleRead(example.LExampleId));
        Assert.Empty(engine.TEngineExampleRead(entry.LEntryId, LOwner.LOwnerEntry));
        Assert.Empty(engine.TEngineExampleRead(senseId, LOwner.LOwnerSense));
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
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], string.Empty, [], [], 1)],
            [TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], string.Empty, [], [], 1)]));
    }
}
