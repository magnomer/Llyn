using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

public sealed class TSpeech
{
    [Fact]
    public void BindingToAWorkspaceWritesTheLanguagePacksVocabularyIntoIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        Assert.Equal("Noun", engine.LEngineSpeechRead("English", "noun"));
        Assert.Equal("Verb", engine.LEngineSpeechRead("English", "verb"));

        Assert.Equal("Verb, transitive", engine.LEngineSpeechRead("English", "verb_transitive"));
        Assert.Null(engine.LEngineSpeechRead("English", "nosuchpartofspeech"));

        LMorphology? plural = engine.LEngineMorphologyRead("English", "noun", "number", "plural");
        Assert.Equal("number", plural?.LMorphologyFeatureName);
        Assert.Equal("plural", plural?.LMorphologyValueName);

        Assert.Equal(0, engine.LEngineMorphologyRead("English", "noun", "number", "singular")?.LMorphologyPosition);
        Assert.Equal(1, plural?.LMorphologyPosition);

        Assert.Equal("Classifier", engine.LEngineSpeechRead("Vietnamese", "classifier"));
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

        using LEngine reopened = new(workspace.TWorkspaceFolder);
        Assert.Equal("Noun", reopened.LEngineSpeechRead("English", "noun"));
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

        engine.LEngineInflectionSet(entry.LEntryId, []);
        Assert.Empty(engine.LEngineInflectionRead(entry.LEntryId));
    }

    [Fact]
    public void APartOfSpeechNamingAPresetIsStoredAsThatPresetsIdAndReadsBackAsItsName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = engine.LEngineEntrySave(TSpeechDraftCreate("Verb, transitive"));

        LSpeech speech = Assert.Single(
            new LEntryArchive(workspace.TWorkspaceDatabase).LEntrySpeechRead(entry.LEntryId));
        Assert.Equal("verb_transitive", speech.LSpeechValueId);
        Assert.Null(speech.LSpeechCustom);

        Assert.Equal(
            "Verb, transitive",
            Assert.Single(engine.LEngineEntryLoad(entry.LEntryId)!.LEntryDraftSpeeches!));
    }

    [Fact]
    public void APartOfSpeechNoPresetNamesIsStoredAsTypedAndReadsBackUnchanged()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = engine.LEngineEntrySave(TSpeechDraftCreate("  Verb, ergative  "));

        LSpeech speech = Assert.Single(
            new LEntryArchive(workspace.TWorkspaceDatabase).LEntrySpeechRead(entry.LEntryId));
        Assert.Null(speech.LSpeechValueId);
        Assert.Equal("Verb, ergative", speech.LSpeechCustom);
        Assert.Equal(
            "Verb, ergative",
            Assert.Single(engine.LEngineEntryLoad(entry.LEntryId)!.LEntryDraftSpeeches!));

        Assert.Equal("verb_transitive", engine.LEngineSpeechFind("English", "verb, TRANSITIVE"));
        Assert.Null(engine.LEngineSpeechFind("English", "Verb, ergative"));
    }

    [Fact]
    public void AnEmptyPartOfSpeechFieldWritesNoAssignmentAtAll()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = engine.LEngineEntrySave(TSpeechDraftCreate("   "));

        Assert.Empty(new LEntryArchive(workspace.TWorkspaceDatabase).LEntrySpeechRead(entry.LEntryId));
        Assert.Empty(engine.LEngineEntryLoad(entry.LEntryId)!.LEntryDraftSpeeches!);
    }

    [Fact]
    public void ChangingThePartOfSpeechReplacesTheAssignmentAndClearingItRemovesIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);

        LEntry entry = engine.LEngineEntrySave(TSpeechDraftCreate("Noun"));

        engine.LEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate("Verb, intransitive"));
        Assert.Equal(
            "verb_intransitive",
            Assert.Single(entries.LEntrySpeechRead(entry.LEntryId)).LSpeechValueId);

        engine.LEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate("Verb, ergative"));
        LSpeech custom = Assert.Single(entries.LEntrySpeechRead(entry.LEntryId));
        Assert.Null(custom.LSpeechValueId);
        Assert.Equal("Verb, ergative", custom.LSpeechCustom);

        engine.LEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate(string.Empty));
        Assert.Empty(entries.LEntrySpeechRead(entry.LEntryId));

        engine.LEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate("Noun"));
        LRevision? revision = engine.LEngineRevisionRead();
        Assert.Equal(
            "speech",
            Assert.Single(engine.LEngineChangeRead(revision!.LRevisionId),
                change => change.LRevisionChangeType == "speech").LRevisionChangeType);

        engine.LEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate("Noun"));
        revision = engine.LEngineRevisionRead();
        Assert.DoesNotContain(
            engine.LEngineChangeRead(revision!.LRevisionId),
            change => change.LRevisionChangeType == "speech");
    }

    [Fact]
    public void ManyPartsOfSpeechAreStoredInTheOrderTheyWereAdded()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = engine.LEngineEntrySave(
            TSpeechDraftCreate("Noun", "Verb, transitive", "Verb, ergative"));

        IReadOnlyList<LSpeech> speeches =
            new LEntryArchive(workspace.TWorkspaceDatabase).LEntrySpeechRead(entry.LEntryId);
        Assert.Equal(3, speeches.Count);
        Assert.Equal([0, 1, 2], speeches.Select(row => row.LSpeechPosition));
        Assert.Equal("verb_transitive", speeches[1].LSpeechValueId);
        Assert.Equal("Verb, ergative", speeches[2].LSpeechCustom);

        Assert.Equal(
            ["Noun", "Verb, transitive", "Verb, ergative"],
            engine.LEngineEntryLoad(entry.LEntryId)!.LEntryDraftSpeeches!);
    }

    private static LEntryDraft TSpeechDraftCreate(params string[] speeches)
    {
        return new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(string.Empty, string.Empty, "a meaning", [], [], [], string.Empty, [], [])],
            [],
            string.Empty,
            null,
            speeches);
    }

    private static LEntry TSpeechEntryCreate(LEngine engine)
    {
        return engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(string.Empty, string.Empty, "a meaning", [], [], [], string.Empty, [], [])],
            [new LCardDraft(string.Empty, "in a word", "briefly", [], [], [], string.Empty, [], [])]));
    }
}
