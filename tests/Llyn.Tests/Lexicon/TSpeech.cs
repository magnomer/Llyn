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

        LSpeechValue? noun = engine.TSpeechValueFind("English", "Noun");
        Assert.NotNull(noun);
        Assert.Equal(1, noun.LSpeechValueCode);
        Assert.Equal("Noun", engine.TEngineSpeechRead("English", noun.LSpeechValueId)?.LSpeechValueName);
        Assert.NotNull(engine.TSpeechValueFind("English", "Verb, transitive"));
        Assert.Null(engine.TSpeechValueFind("English", "nosuchpartofspeech"));

        Assert.Equal(
            ["singular", "plural"],
            workspace.TWorkspaceRowRead(
                "SELECT value.name FROM morphology_value value " +
                "JOIN morphology_feature feature ON value.morphology_feature_parent = feature.morphology_feature_id " +
                $"WHERE feature.speech_value_parent = {noun.LSpeechValueId} AND feature.name = 'number' " +
                "ORDER BY value.position;"));

        Assert.NotNull(engine.TSpeechValueFind("Vietnamese", "Classifier"));
    }

    [Fact]
    public void SpeechValueCreate_SameRowAgain_RewritesNotAdds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        long parts = workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM speech_value;");
        long features = workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM morphology_feature;");
        long rows = workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM morphology_value;");
        Assert.True(parts > 0);
        Assert.True(features > 0);
        Assert.True(rows > 0);

        LEntry entry = engine.TEngineEntrySave(TSpeechDraftCreate("Noun"));
        LSpeechValue noun = engine.TSpeechValueFind("English", "Noun")!;
        LMorphology plural = engine.TParadigmMorphologyRead(entry.LEntryId, "plural");

        LSpeechValue renamed = TInterface.TSpeechArchiveCreate(workspace.TWorkspaceDatabase).TSpeechValueCreate(
            TInterface.TSpeechValueCreate("English", noun.LSpeechValueCode, "substantive", 0));
        LMorphology relabeled = TInterface.TMorphologyArchiveCreate(workspace.TWorkspaceDatabase).TMorphologyCreate(
            TInterface.TMorphologyCreate(plural.LMorphologyFeatureId, plural.LMorphologyCode, "plural form", 1));

        Assert.Equal(noun.LSpeechValueId, renamed.LSpeechValueId);
        Assert.Equal(plural.LMorphologyId, relabeled.LMorphologyId);
        Assert.Equal("substantive", engine.TEngineSpeechRead("English", noun.LSpeechValueId)?.LSpeechValueName);
        Assert.Equal(
            ["plural form"],
            workspace.TWorkspaceRowRead(
                $"SELECT name FROM morphology_value WHERE morphology_value_id = {plural.LMorphologyId};"));
        Assert.Equal(parts, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM speech_value;"));
        Assert.Equal(features, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM morphology_feature;"));
        Assert.Equal(rows, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM morphology_value;"));

        using LEngine reopened = workspace.TWorkspaceEngineStart();
        Assert.Equal("Noun", reopened.TEngineSpeechRead("English", noun.LSpeechValueId)?.LSpeechValueName);
        Assert.Equal(parts, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM speech_value;"));
    }

    [Fact]
    public void InflectionUpdate_AppendedMovedDeleted_ReadsBackEachStep()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TSpeechDraftCreate("Noun"));
        Assert.Empty(engine.TEntryInflectionRead(entry.LEntryId));

        LSpeechValue noun = engine.TSpeechValueFind("English", "Noun")!;
        LMorphology plural = engine.TParadigmMorphologyRead(entry.LEntryId, "plural");

        engine.TEntryInflectionSave(entry.LEntryId, [
            TInterface.TInflectionCreate(entry.LEntryId, 0, "word", null, noun.LSpeechValueId, []),
            TInterface.TInflectionCreate(entry.LEntryId, 1, "words", null, noun.LSpeechValueId,
                [plural.LMorphologyId]),
        ]);
        engine.TEntryInflectionSave(entry.LEntryId, [
            .. engine.TEntryInflectionRead(entry.LEntryId),
            TInterface.TInflectionCreate(entry.LEntryId, 2, "word's", null, noun.LSpeechValueId, []),
        ]);

        IReadOnlyList<LInflection> stored = engine.TEntryInflectionRead(entry.LEntryId);
        Assert.Equal(["word", "words", "word's"], stored.Select(row => row.LInflectionText));
        Assert.Equal(
            plural.LMorphologyId,
            Assert.Single(stored[1].LInflectionMorphology));
        Assert.Equal(noun.LSpeechValueId, stored[0].LInflectionSpeechId);
        Assert.True(stored[0].LInflectionId > 0);

        engine.TEntryInflectionSave(entry.LEntryId, [stored[2], stored[0], stored[1]]);
        IReadOnlyList<LInflection> moved = engine.TEntryInflectionRead(entry.LEntryId);
        Assert.Equal(["word's", "word", "words"], moved.Select(row => row.LInflectionText));

        engine.TEntryInflectionSave(entry.LEntryId, [moved[1], moved[2]]);
        Assert.Equal(
            ["word", "words"],
            engine.TEntryInflectionRead(entry.LEntryId).Select(row => row.LInflectionText));

        engine.TEntryInflectionSave(entry.LEntryId, []);
        Assert.Empty(engine.TEntryInflectionRead(entry.LEntryId));
    }

    [Fact]
    public void EntrySave_SpeechNamingPreset_StoresIdReadsBackName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TSpeechDraftCreate("Verb, transitive"));

        LSpeech speech = Assert.Single(
            TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase).TEntrySpeechRead(entry.LEntryId));
        Assert.Equal(engine.TSpeechValueFind("English", "Verb, transitive")!.LSpeechValueId, speech.LSpeechValueId);
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

        Assert.Null(engine.TSpeechValueFind("English", "Verb, ergative"));
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
            engine.TSpeechValueFind("English", "Verb, intransitive")!.LSpeechValueId,
            Assert.Single(entries.TEntrySpeechRead(entry.LEntryId)).LSpeechValueId);

        engine.TEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate("Verb, ergative"));
        LSpeech custom = Assert.Single(entries.TEntrySpeechRead(entry.LEntryId));
        Assert.Null(custom.LSpeechValueId);
        Assert.Equal("Verb, ergative", custom.LSpeechCustom);

        engine.TEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate(string.Empty));
        Assert.Empty(entries.TEntrySpeechRead(entry.LEntryId));

        engine.TEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate("Noun"));
        long? revision = engine.TEngineRevisionRead();
        Assert.Equal(
            "speech",
            Assert.Single(workspace.TRevisionChangeRead(revision!.Value),
                change => change.LRevisionDeltaSubject == "speech").LRevisionDeltaSubject);

        engine.TEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate("Noun"));
        revision = engine.TEngineRevisionRead();
        Assert.DoesNotContain(
            workspace.TRevisionChangeRead(revision!.Value),
            change => change.LRevisionDeltaSubject == "speech");
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
        Assert.Equal(
            engine.TSpeechValueFind("English", "Verb, transitive")!.LSpeechValueId,
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
}
