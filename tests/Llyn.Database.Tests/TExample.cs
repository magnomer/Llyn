using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

public sealed class TExample
{
    [Fact]
    public void OneExampleIsQuotedFromAllThreeSidesAndReadBackFromEach()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TExampleEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        string collocationId =
            engine.LEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LExample example = engine.LEngineExampleCreate(new LExample(
            string.Empty,
            "English",
            "he said the word",
            "그가 그 말을 했다",
            null));

        Assert.NotEmpty(example.LExampleId);
        Assert.Equal(
            "그가 그 말을 했다",
            engine.LEngineExampleRead(example.LExampleId)!.LExampleTranslation.LStateValueShow());

        engine.LEngineExampleAttach(entry.LEntryId, example.LExampleId, 0, LOwner.LOwnerEntry);
        engine.LEngineExampleAttach(senseId, example.LExampleId, 0, LOwner.LOwnerSense);
        engine.LEngineExampleAttach(collocationId, example.LExampleId, 0, LOwner.LOwnerCollocation);

        Assert.Equal(
            example.LExampleId,
            Assert.Single(engine.LEngineExampleRead(entry.LEntryId, LOwner.LOwnerEntry)).LExampleId);
        Assert.Single(engine.LEngineExampleRead(senseId, LOwner.LOwnerSense));
        Assert.Single(engine.LEngineExampleRead(collocationId, LOwner.LOwnerCollocation));

        engine.LEngineExampleUpdate(example with { LExampleText = "he spoke the word" });
        Assert.Equal("he spoke the word", engine.LEngineExampleRead(example.LExampleId)?.LExampleText);
    }

    [Fact]
    public void AnExampleNothingQuotesAnyMoreGoesWithItsLastReference()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TExampleEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;

        LExample example = engine.LEngineExampleCreate(
            new LExample(string.Empty, "English", "he said the word", null, null));
        engine.LEngineExampleAttach(entry.LEntryId, example.LExampleId, 0, LOwner.LOwnerEntry);
        engine.LEngineExampleAttach(senseId, example.LExampleId, 0, LOwner.LOwnerSense);

        Assert.Throws<InvalidOperationException>(() => engine.LEngineExampleDelete(example.LExampleId));
        engine.LEngineExampleDetach(senseId, example.LExampleId, LOwner.LOwnerSense);
        Assert.NotNull(engine.LEngineExampleRead(example.LExampleId));

        engine.LEngineExampleRemove(entry.LEntryId, example.LExampleId, LOwner.LOwnerEntry);
        Assert.Null(engine.LEngineExampleRead(example.LExampleId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
    }

    [Fact]
    public void CitingAReferenceMovesThePointerAndNothingElse()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LExample example = engine.LEngineExampleCreate(
            new LExample(string.Empty, "English", "he said the word", null, null));
        LReference reference = engine.LEngineReferenceCreate(new LReference(
            string.Empty,
            LStateValue.LStateValueCreate("A Dictionary"),
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueCreate("1998"),
            LStateValue.LStateValueUnspecified,
            LState.LStateUnspecified));

        engine.LEngineExampleUpdate(example.LExampleId, reference.LReferenceId);
        Assert.Equal(
            reference.LReferenceId,
            engine.LEngineExampleRead(example.LExampleId)?.LExampleSource.LStateValueShow());
        Assert.Equal(
            reference.LReferenceId,
            Assert.Single(engine.LEngineReferenceRead(example.LExampleId, LOwner.LOwnerExample))
                .LReferenceId);

        engine.LEngineExampleUpdate(example.LExampleId, LStateValue.LStateValueUnspecified);
        Assert.Equal(
            LStateValue.LStateValueUnspecified,
            engine.LEngineExampleRead(example.LExampleId)?.LExampleSource);
        Assert.NotNull(engine.LEngineReferenceRead(reference.LReferenceId));
        Assert.NotNull(engine.LEngineExampleRead(example.LExampleId));
    }

    [Fact]
    public void AMeaningKeepsEveryExampleItRefersToAndEditsOneWithoutLosingItsIdOrSource()
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
                [
                    LExampleDraft.LExampleDraftCreate("he said a word"),
                    LExampleDraft.LExampleDraftCreate("not a word was spoken"),
                    LExampleDraft.LExampleDraftCreate("a word of advice"),
                ],
                [],
                [],
                string.Empty,
                [], [], 1)],
            []));

        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        Assert.Equal(
            ["he said a word", "not a word was spoken", "a word of advice"],
            engine.LEngineExampleRead(senseId, LOwner.LOwnerSense).Select(example => example.LExampleText));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.LEngineEntryLoad(entry.LEntryId));
        LCardDraft card = loaded.LEntryDraftSenses[0];
        Assert.Equal(3, card.LCardDraftExample.Count);
        foreach (LExampleDraft draft in card.LCardDraftExample)
        {
            Assert.NotEmpty(draft.LExampleDraftId);
        }

        LReference reference = engine.LEngineReferenceCreate(new LReference(
            string.Empty,
            LStateValue.LStateValueCreate("A Dictionary"),
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LState.LStateUnspecified));
        string citedId = card.LCardDraftExample[1].LExampleDraftId;
        engine.LEngineExampleUpdate(citedId, reference.LReferenceId);

        loaded = Assert.IsType<LEntryDraft>(engine.LEngineEntryLoad(entry.LEntryId));
        card = loaded.LEntryDraftSenses[0];
        Assert.Equal(reference.LReferenceId, card.LCardDraftExample[1].LExampleDraftReference);

        engine.LEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftSenses =
            [
                card with
                {
                    LCardDraftExample =
                    [
                        card.LCardDraftExample[1] with { LExampleDraftText = "not one word was spoken" },
                        card.LCardDraftExample[0],
                        LExampleDraft.LExampleDraftCreate("in a word"),
                    ],
                },
            ],
        });

        IReadOnlyList<LExample> attached = engine.LEngineExampleRead(senseId, LOwner.LOwnerSense);
        Assert.Equal(
            ["not one word was spoken", "he said a word", "in a word"],
            attached.Select(example => example.LExampleText));
        Assert.Equal(citedId, attached[0].LExampleId);
        Assert.Equal(reference.LReferenceId, attached[0].LExampleSource.LStateValueShow());
        Assert.Equal(card.LCardDraftExample[0].LExampleDraftId, attached[1].LExampleId);

        Assert.NotNull(engine.LEngineExampleRead(card.LCardDraftExample[2].LExampleDraftId));
        Assert.Equal(4, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
        Assert.Equal(3, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));
    }

    [Fact]
    public void TheWorkspaceStockOfExamplesIsReadWithItsUsageCounted()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TExampleEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;

        LExample shared = engine.LEngineExampleCreate(new LExample(
            string.Empty,
            "English",
            "he said the word",
            "그가 그 말을 했다",
            null));
        LExample lonely = engine.LEngineExampleCreate(
            new LExample(string.Empty, "English", "nobody quotes me", null, null));

        engine.LEngineExampleAttach(entry.LEntryId, shared.LExampleId, 0, LOwner.LOwnerEntry);
        engine.LEngineExampleAttach(senseId, shared.LExampleId, 0, LOwner.LOwnerSense);

        Assert.Equal(
            ["he said the word", "nobody quotes me"],
            engine.LEngineExampleRead().Select(row => row.LExampleText.LStateValueShow()));
        Assert.Equal(
            "그가 그 말을 했다",
            engine.LEngineExampleRead()[0].LExampleTranslation.LStateValueShow());

        IReadOnlyDictionary<string, int> counts = engine.LEngineUsageRead(LOwner.LOwnerExample);
        Assert.Equal(2, counts[shared.LExampleId]);
        Assert.DoesNotContain(lonely.LExampleId, counts);
    }

    [Fact]
    public void UsageNamesEveryQuotingSideAndDetachingDeleteDropsThemAll()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TExampleEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        string collocationId =
            engine.LEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LExample example = engine.LEngineExampleCreate(
            new LExample(string.Empty, "English", "he said the word", null, null));
        engine.LEngineExampleAttach(entry.LEntryId, example.LExampleId, 0, LOwner.LOwnerEntry);
        engine.LEngineExampleAttach(senseId, example.LExampleId, 0, LOwner.LOwnerSense);
        engine.LEngineExampleAttach(collocationId, example.LExampleId, 0, LOwner.LOwnerCollocation);

        IReadOnlyList<LUsage> usage = engine.LEngineUsageRead(example.LExampleId, LOwner.LOwnerExample);
        Assert.Equal(3, usage.Count);
        Assert.Equal(entry.LEntryId, usage.Single(row => row.LUsageOwner == LOwner.LOwnerEntry).LUsageEntry);
        Assert.Equal(senseId, usage.Single(row => row.LUsageOwner == LOwner.LOwnerSense).LUsageId);
        Assert.Equal(
            "in a word",
            usage.Single(row => row.LUsageOwner == LOwner.LOwnerCollocation).LUsageTitle.LStateValueShow());

        Assert.Throws<InvalidOperationException>(() => engine.LEngineExampleDelete(example.LExampleId));

        engine.LEngineExampleDelete(example.LExampleId, true);

        Assert.Null(engine.LEngineExampleRead(example.LExampleId));
        Assert.Empty(engine.LEngineExampleRead(entry.LEntryId, LOwner.LOwnerEntry));
        Assert.Empty(engine.LEngineExampleRead(senseId, LOwner.LOwnerSense));
        Assert.Empty(engine.LEngineExampleRead(collocationId, LOwner.LOwnerCollocation));
        Assert.Empty(engine.LEngineUsageRead(example.LExampleId, LOwner.LOwnerExample));
    }

    private static LEntry TExampleEntryCreate(LEngine engine)
    {
        return engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(string.Empty, string.Empty, "a meaning", [], [], [], string.Empty, [], [], 1)],
            [new LCardDraft(string.Empty, "in a word", "briefly", [], [], [], string.Empty, [], [], 1)]));
    }
}
