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
    public void WorkspaceEngineStart_NewWorkspace_WritesPackVocabulary()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LSpeechValue? noun = engine.TEngineSpeechFind("English", "Noun");
        Assert.NotNull(noun);
        Assert.Equal(1, noun.LSpeechValueCode);
        Assert.Equal("Noun", engine.TEngineSpeechRead(noun.LSpeechValueId)?.LSpeechValueName);
        Assert.NotNull(engine.TEngineSpeechFind("English", "Verb, transitive"));
        Assert.Null(engine.TEngineSpeechFind("English", "nosuchpartofspeech"));

        LFeature? number = engine.TEngineFeatureFind(noun.LSpeechValueId, "number");
        Assert.NotNull(number);
        LMorphology? plural = engine.TEngineMorphologyFind(number.LFeatureId, "plural");
        Assert.NotNull(plural);
        Assert.Equal("plural", engine.TEngineMorphologyRead(plural.LMorphologyId)?.LMorphologyName);

        Assert.Equal(
            ["singular", "plural"],
            engine.TEngineMorphologyScan(number.LFeatureId).Select(row => row.LMorphologyName));

        Assert.NotNull(engine.TEngineSpeechFind("Vietnamese", "Classifier"));
    }

    [Fact]
    public void SpeechCreate_SameRowAgain_RewritesNotAdds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        long parts = workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM speech_value;");
        long features = workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM morphology_feature;");
        long rows = workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM morphology_value;");
        Assert.True(parts > 0);
        Assert.True(features > 0);
        Assert.True(rows > 0);

        LSpeechValue noun = engine.TEngineSpeechFind("English", "Noun")!;
        LFeature number = engine.TEngineFeatureFind(noun.LSpeechValueId, "number")!;
        LMorphology plural = engine.TEngineMorphologyFind(number.LFeatureId, "plural")!;

        LSpeechValue renamed = engine.TEngineSpeechCreate(
            TInterface.TSpeechValueCreate("English", noun.LSpeechValueCode, "substantive", 0));
        LMorphology relabeled = engine.TEngineMorphologyCreate(TInterface.TMorphologyCreate(
            number.LFeatureId, plural.LMorphologyCode, "plural form", 1));

        Assert.Equal(noun.LSpeechValueId, renamed.LSpeechValueId);
        Assert.Equal(plural.LMorphologyId, relabeled.LMorphologyId);
        Assert.Equal("substantive", engine.TEngineSpeechRead(noun.LSpeechValueId)?.LSpeechValueName);
        Assert.Equal("plural form", engine.TEngineMorphologyRead(plural.LMorphologyId)?.LMorphologyName);
        Assert.Equal(parts, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM speech_value;"));
        Assert.Equal(features, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM morphology_feature;"));
        Assert.Equal(rows, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM morphology_value;"));

        using LEngine reopened = workspace.TWorkspaceEngineStart();
        Assert.Equal("Noun", reopened.TEngineSpeechRead(noun.LSpeechValueId)?.LSpeechValueName);
        Assert.Equal(parts, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM speech_value;"));
    }

    [Fact]
    public void InflectionSet_AppendedMovedDeleted_ReadsBackEachStep()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSpeechEntryCreate(engine);
        Assert.Empty(engine.TEngineInflectionRead(entry.LEntryId));

        LSpeechValue noun = engine.TEngineSpeechFind("English", "Noun")!;
        LFeature number = engine.TEngineFeatureFind(noun.LSpeechValueId, "number")!;
        LMorphology plural = engine.TEngineMorphologyFind(number.LFeatureId, "plural")!;

        engine.TEngineInflectionSet(entry.LEntryId, [
            TInterface.TInflectionCreate(entry.LEntryId, 0, "word", null, noun.LSpeechValueId, []),
            TInterface.TInflectionCreate(entry.LEntryId, 1, "words", null, noun.LSpeechValueId,
                [plural.LMorphologyId]),
        ]);
        engine.TEngineInflectionAppend(entry.LEntryId, [
            TInterface.TInflectionCreate(entry.LEntryId, 0, "word's", null, noun.LSpeechValueId, []),
        ]);

        IReadOnlyList<LInflection> stored = engine.TEngineInflectionRead(entry.LEntryId);
        Assert.Equal(["word", "words", "word's"], stored.Select(row => row.LInflectionText));
        Assert.Equal(
            plural.LMorphologyId,
            Assert.Single(stored[1].LInflectionMorphology));
        Assert.Equal(noun.LSpeechValueId, stored[0].LInflectionSpeechId);
        Assert.True(stored[0].LInflectionId > 0);

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
    public void EntrySave_SpeechNamingPreset_StoresIdReadsBackName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TSpeechDraftCreate("Verb, transitive"));

        LSpeech speech = Assert.Single(
            TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase).TEntrySpeechRead(entry.LEntryId));
        Assert.Equal(engine.TEngineSpeechFind("English", "Verb, transitive")!.LSpeechValueId, speech.LSpeechValueId);
        Assert.Null(speech.LSpeechCustom);

        Assert.Equal(
            "Verb, transitive",
            Assert.Single(TInterface.TSpeechNameRead(engine.TEngineEntryLoad(entry.LEntryId)!)));
    }

    [Fact]
    public void EntrySave_SpeechWithoutPreset_StoresTextAsTyped()
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
            Assert.Single(TInterface.TSpeechNameRead(engine.TEngineEntryLoad(entry.LEntryId)!)));

        Assert.Equal("Verb, transitive", engine.TEngineSpeechFind("English", "verb, TRANSITIVE")?.LSpeechValueName);
        Assert.Null(engine.TEngineSpeechFind("English", "Verb, ergative"));
    }

    [Fact]
    public void EntrySave_EmptySpeechField_WritesNoAssignment()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TSpeechDraftCreate("   "));

        Assert.Empty(TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase).TEntrySpeechRead(entry.LEntryId));
        Assert.Empty(engine.TEngineEntryLoad(entry.LEntryId)!.LEntryDraftSpeeches);
    }

    [Fact]
    public void EntryUpdate_SpeechCleared_RemovesAssignment()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = engine.TEngineEntrySave(TSpeechDraftCreate("Noun"));

        engine.TEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate("Verb, intransitive"));
        Assert.Equal(
            engine.TEngineSpeechFind("English", "Verb, intransitive")!.LSpeechValueId,
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
                change => change.LRevisionChangeSubject == "speech").LRevisionChangeSubject);

        engine.TEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate("Noun"));
        revision = engine.TEngineRevisionRead();
        Assert.DoesNotContain(
            engine.TEngineChangeRead(revision!.LRevisionId),
            change => change.LRevisionChangeSubject == "speech");
    }

    [Fact]
    public void EntrySave_ManySpeeches_StoresInAddedOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(
            TSpeechDraftCreate("Noun", "Verb, transitive", "Verb, ergative"));

        IReadOnlyList<LSpeech> speeches =
            TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase).TEntrySpeechRead(entry.LEntryId);
        Assert.Equal(3, speeches.Count);
        Assert.Equal([0, 1, 2], speeches.Select(row => row.LSpeechPosition));
        Assert.Equal(
            engine.TEngineSpeechFind("English", "Verb, transitive")!.LSpeechValueId,
            speeches[1].LSpeechValueId);
        Assert.Equal("Verb, ergative", speeches[2].LSpeechCustom);

        Assert.Equal(
            ["Noun", "Verb, transitive", "Verb, ergative"],
            TInterface.TSpeechNameRead(engine.TEngineEntryLoad(entry.LEntryId)!));
    }

    private static LEntryDraft TSpeechDraftCreate(params string[] speeches)
    {
        return TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)],
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
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)],
            [TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], [], [], 1)]));
    }
}
