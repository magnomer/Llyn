using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

/// <summary>
/// Covers the controlled vocabularies and the inflections that use them: the parts of speech and
/// morphology the engine writes into a workspace from the language packs on disk, the seams that add
/// and resolve one row, and an Entry's inflected forms set, appended to, moved and deleted.
/// </summary>
public sealed class TSpeech
{
    [Fact]
    public void BindingToAWorkspaceWritesTheLanguagePacksVocabularyIntoIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        // The tables are no longer empty on a fresh workspace: what a language declares on disk is what
        // the workspace holds, so an entry can carry a part of speech from the first save.
        Assert.Equal("noun", engine.LEngineSpeechRead("English", "noun"));
        Assert.Equal("verb", engine.LEngineSpeechRead("English", "verb"));
        Assert.Null(engine.LEngineSpeechRead("English", "nosuchpartofspeech"));

        LMorphology? plural = engine.LEngineMorphologyRead("English", "noun", "number", "plural");
        Assert.Equal("number", plural?.LMorphologyFeatureName);
        Assert.Equal("plural", plural?.LMorphologyValueName);

        // Order is the order the pack lists the values in, and it is stored, not inferred.
        Assert.Equal(0, engine.LEngineMorphologyRead("English", "noun", "number", "singular")?.LMorphologyPosition);
        Assert.Equal(1, plural?.LMorphologyPosition);

        // A language declaring no morphology is an ordinary language, not a broken pack.
        Assert.Equal("classifier", engine.LEngineSpeechRead("Vietnamese", "classifier"));
    }

    [Fact]
    public void WritingTheSameVocabularyRowAgainRewritesItRatherThanAddingOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        long parts = workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM part_of_speech_value;");
        long rows = workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM morphology_value;");
        Assert.True(parts > 0);
        Assert.True(rows > 0);

        engine.LEngineSpeechCreate(new LSpeechValue("English", "noun", "substantive", 0));
        engine.LEngineMorphologyCreate(
            new LMorphology("English", "noun", "number", "number", "plural", "plural form", 1));

        Assert.Equal("substantive", engine.LEngineSpeechRead("English", "noun"));
        Assert.Equal(
            "plural form",
            engine.LEngineMorphologyRead("English", "noun", "number", "plural")?.LMorphologyValueName);
        Assert.Equal(parts, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM part_of_speech_value;"));
        Assert.Equal(rows, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM morphology_value;"));

        // A second engine over the same workspace writes the packs again, so the pack's wording is what
        // the workspace ends up holding.
        using LEngine reopened = new(workspace.TWorkspaceFolder);
        Assert.Equal("noun", reopened.LEngineSpeechRead("English", "noun"));
    }

    [Fact]
    public void AnEntrysInflectionsAreSetAppendedMovedAndDeleted()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TSpeechEntryCreate(engine);
        Assert.Empty(engine.LEngineInflectionRead(entry.LEntryId));

        engine.LEngineInflectionSet(entry.LEntryId, [
            new LInflection(entry.LEntryId, 0, "word", null, "noun", []),
            new LInflection(entry.LEntryId, 1, "words", null, "noun",
                [new LFeature("number", "plural")]),
        ]);
        engine.LEngineInflectionAppend(entry.LEntryId, [
            new LInflection(entry.LEntryId, 0, "word's", null, "noun", []),
        ]);

        IReadOnlyList<LInflection> stored = engine.LEngineInflectionRead(entry.LEntryId);
        Assert.Equal(["word", "words", "word's"], stored.Select(row => row.LInflectionText));
        Assert.Equal(
            "plural",
            Assert.Single(stored[1].LInflectionFeatures).LFeatureValueId);

        engine.LEngineInflectionMove(entry.LEntryId, 2, 0);
        Assert.Equal(
            ["word's", "word", "words"],
            engine.LEngineInflectionRead(entry.LEntryId).Select(row => row.LInflectionText));

        engine.LEngineInflectionDelete(entry.LEntryId, 0);
        Assert.Equal(
            ["word", "words"],
            engine.LEngineInflectionRead(entry.LEntryId).Select(row => row.LInflectionText));

        // Setting the list makes it the list: an empty one clears the forms.
        engine.LEngineInflectionSet(entry.LEntryId, []);
        Assert.Empty(engine.LEngineInflectionRead(entry.LEntryId));
    }

    private static LEntry TSpeechEntryCreate(LEngine engine)
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
