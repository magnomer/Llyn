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
            null,
            null,
            [new LTranslation(string.Empty, "Korean", "그가 그 말을 했다", 0)]));

        Assert.NotEmpty(example.LExampleId);
        Assert.Equal(
            "그가 그 말을 했다",
            Assert.Single(engine.LEngineExampleRead(example.LExampleId)!.LExampleTranslations)
                .LTranslationText);

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
            new LExample(string.Empty, "English", "he said the word", null, null, []));
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
            new LExample(string.Empty, "English", "he said the word", null, null, []));
        LReference reference = engine.LEngineReferenceCreate(new LReference(
            string.Empty,
            LReferenceValue.LReferenceValueCreate("A Dictionary"),
            LReferenceValue.LReferenceValueUnspecified,
            LReferenceValue.LReferenceValueUnspecified,
            LReferenceValue.LReferenceValueCreate("1998"),
            LReferenceValue.LReferenceValueUnspecified,
            LState.LStateUnspecified));

        engine.LEngineExampleUpdate(example.LExampleId, reference.LReferenceId);
        Assert.Equal(
            reference.LReferenceId,
            engine.LEngineExampleRead(example.LExampleId)?.LExampleSourceId);
        Assert.Equal(
            reference.LReferenceId,
            Assert.Single(engine.LEngineReferenceRead(example.LExampleId, LOwner.LOwnerExample))
                .LReferenceId);

        engine.LEngineExampleUpdate(example.LExampleId, null);
        Assert.Null(engine.LEngineExampleRead(example.LExampleId)?.LExampleSourceId);
        Assert.NotNull(engine.LEngineReferenceRead(reference.LReferenceId));
        Assert.NotNull(engine.LEngineExampleRead(example.LExampleId));
    }

    private static LEntry TExampleEntryCreate(LEngine engine)
    {
        return engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(string.Empty, string.Empty, "a meaning", [], [], string.Empty, [])],
            [new LCardDraft(string.Empty, "in a word", "briefly", [], [], string.Empty, [])]));
    }
}
