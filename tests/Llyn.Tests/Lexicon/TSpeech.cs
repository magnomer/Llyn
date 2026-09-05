using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TSpeech
{
    [Fact]
    public void BindingToAWorkspaceWritesTheLanguagePacksVocabularyIntoIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Equal("Noun", engine.TEngineSpeechRead("English", "noun"));
        Assert.Equal("Verb", engine.TEngineSpeechRead("English", "verb"));

        Assert.Equal("Verb, transitive", engine.TEngineSpeechRead("English", "verb_transitive"));
        Assert.Null(engine.TEngineSpeechRead("English", "nosuchpartofspeech"));

        LMorphology? plural = engine.TEngineMorphologyRead("English", "noun", "number", "plural");
        Assert.Equal("number", plural?.LMorphologyFeatureName);
        Assert.Equal("plural", plural?.LMorphologyValueName);

        Assert.Equal(0, engine.TEngineMorphologyRead("English", "noun", "number", "singular")?.LMorphologyPosition);
        Assert.Equal(1, plural?.LMorphologyPosition);

        Assert.Equal("Classifier", engine.TEngineSpeechRead("Vietnamese", "classifier"));
    }

    [Fact]
    public void WritingTheSameVocabularyRowAgainRewritesItRatherThanAddingOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        long parts = workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM part_of_speech_value;");
        long rows = workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM morphology_value;");
        Assert.True(parts > 0);
        Assert.True(rows > 0);

        engine.TEngineSpeechCreate(TInterface.TSpeechValueCreate("English", "noun", "substantive", 0));
        engine.TEngineMorphologyCreate(
            TInterface.TMorphologyCreate("English", "noun", "number", "number", "plural", "plural form", 1));

        Assert.Equal("substantive", engine.TEngineSpeechRead("English", "noun"));
        Assert.Equal(
            "plural form",
            engine.TEngineMorphologyRead("English", "noun", "number", "plural")?.LMorphologyValueName);
        Assert.Equal(parts, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM part_of_speech_value;"));
        Assert.Equal(rows, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM morphology_value;"));

        using LEngine reopened = workspace.TWorkspaceEngineStart();
        Assert.Equal("Noun", reopened.TEngineSpeechRead("English", "noun"));
    }

    [Fact]
    public void AnEntrysInflectionsAreSetAppendedMovedAndDeleted()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSpeechEntryCreate(engine);
        Assert.Empty(engine.TEngineInflectionRead(entry.LEntryId));

        engine.TEngineInflectionSet(entry.LEntryId, [
            TInterface.TInflectionCreate(entry.LEntryId, 0, "word", null, "noun", []),
            TInterface.TInflectionCreate(entry.LEntryId, 1, "words", null, "noun",
                [TInterface.TFeatureCreate("number", "plural")]),
        ]);
        engine.TEngineInflectionAppend(entry.LEntryId, [
            TInterface.TInflectionCreate(entry.LEntryId, 0, "word's", null, "noun", []),
        ]);

        IReadOnlyList<LInflection> stored = engine.TEngineInflectionRead(entry.LEntryId);
        Assert.Equal(["word", "words", "word's"], stored.Select(row => row.LInflectionText));
        Assert.Equal(
            "plural",
            Assert.Single(stored[1].LInflectionFeatures).LFeatureValueId);

        engine.TEngineInflectionMove(entry.LEntryId, 2, 0);
        Assert.Equal(
            ["word's", "word", "words"],
            engine.TEngineInflectionRead(entry.LEntryId).Select(row => row.LInflectionText));

        engine.TEngineInflectionDelete(entry.LEntryId, 0);
        Assert.Equal(
            ["word", "words"],
            engine.TEngineInflectionRead(entry.LEntryId).Select(row => row.LInflectionText));

        engine.TEngineInflectionSet(entry.LEntryId, []);
        Assert.Empty(engine.TEngineInflectionRead(entry.LEntryId));
    }

    [Fact]
    public void APartOfSpeechNamingAPresetIsStoredAsThatPresetsIdAndReadsBackAsItsName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TSpeechDraftCreate("Verb, transitive"));

        LSpeech speech = Assert.Single(
            TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase).TEntrySpeechRead(entry.LEntryId));
        Assert.Equal("verb_transitive", speech.LSpeechValueId);
        Assert.Null(speech.LSpeechCustom);

        Assert.Equal(
            "Verb, transitive",
            Assert.Single(engine.TEngineEntryLoad(entry.LEntryId)!.LEntryDraftSpeeches!));
    }

    [Fact]
    public void APartOfSpeechNoPresetNamesIsStoredAsTypedAndReadsBackUnchanged()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TSpeechDraftCreate("  Verb, ergative  "));

        LSpeech speech = Assert.Single(
            TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase).TEntrySpeechRead(entry.LEntryId));
        Assert.Null(speech.LSpeechValueId);
        Assert.Equal("Verb, ergative", speech.LSpeechCustom);
        Assert.Equal(
            "Verb, ergative",
            Assert.Single(engine.TEngineEntryLoad(entry.LEntryId)!.LEntryDraftSpeeches!));

        Assert.Equal("verb_transitive", engine.TEngineSpeechFind("English", "verb, TRANSITIVE"));
        Assert.Null(engine.TEngineSpeechFind("English", "Verb, ergative"));
    }

    [Fact]
    public void AnEmptyPartOfSpeechFieldWritesNoAssignmentAtAll()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TSpeechDraftCreate("   "));

        Assert.Empty(TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase).TEntrySpeechRead(entry.LEntryId));
        Assert.Empty(engine.TEngineEntryLoad(entry.LEntryId)!.LEntryDraftSpeeches!);
    }

    [Fact]
    public void ChangingThePartOfSpeechReplacesTheAssignmentAndClearingItRemovesIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = engine.TEngineEntrySave(TSpeechDraftCreate("Noun"));

        engine.TEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate("Verb, intransitive"));
        Assert.Equal(
            "verb_intransitive",
            Assert.Single(entries.TEntrySpeechRead(entry.LEntryId)).LSpeechValueId);

        engine.TEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate("Verb, ergative"));
        LSpeech custom = Assert.Single(entries.TEntrySpeechRead(entry.LEntryId));
        Assert.Null(custom.LSpeechValueId);
        Assert.Equal("Verb, ergative", custom.LSpeechCustom);

        engine.TEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate(string.Empty));
        Assert.Empty(entries.TEntrySpeechRead(entry.LEntryId));

        engine.TEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate("Noun"));
        LRevision? revision = engine.TEngineRevisionRead();
        Assert.Equal(
            "speech",
            Assert.Single(engine.TEngineChangeRead(revision!.LRevisionId),
                change => change.LRevisionChangeType == "speech").LRevisionChangeType);

        engine.TEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate("Noun"));
        revision = engine.TEngineRevisionRead();
        Assert.DoesNotContain(
            engine.TEngineChangeRead(revision!.LRevisionId),
            change => change.LRevisionChangeType == "speech");
    }

    [Fact]
    public void ManyPartsOfSpeechAreStoredInTheOrderTheyWereAdded()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(
            TSpeechDraftCreate("Noun", "Verb, transitive", "Verb, ergative"));

        IReadOnlyList<LSpeech> speeches =
            TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase).TEntrySpeechRead(entry.LEntryId);
        Assert.Equal(3, speeches.Count);
        Assert.Equal([0, 1, 2], speeches.Select(row => row.LSpeechPosition));
        Assert.Equal("verb_transitive", speeches[1].LSpeechValueId);
        Assert.Equal("Verb, ergative", speeches[2].LSpeechCustom);

        Assert.Equal(
            ["Noun", "Verb, transitive", "Verb, ergative"],
            engine.TEngineEntryLoad(entry.LEntryId)!.LEntryDraftSpeeches!);
    }

    private static LEntryDraft TSpeechDraftCreate(params string[] speeches)
    {
        return TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], string.Empty, [], [], 1)],
            [],
            string.Empty,
            null,
            speeches);
    }

    private static LEntry TSpeechEntryCreate(LEngine engine)
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
