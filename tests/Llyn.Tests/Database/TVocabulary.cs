using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TVocabulary
{
    private const string TVocabularySpeech =
        "SELECT speech_value_id FROM speech_value ORDER BY speech_value_id;";

    private const string TVocabularyFeature =
        "SELECT morphology_feature_id FROM morphology_feature ORDER BY morphology_feature_id;";

    private const string TVocabularyValue =
        "SELECT morphology_value_id FROM morphology_value ORDER BY morphology_value_id;";

    [Fact]
    public void LanguageImport_SecondEngineStart_KeepsEveryId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        IReadOnlyList<long> parts;
        IReadOnlyList<long> features;
        IReadOnlyList<long> values;
        using (workspace.TWorkspaceEngineStart())
        {
            parts = workspace.TWorkspaceColumnRead(TVocabularySpeech);
            features = workspace.TWorkspaceColumnRead(TVocabularyFeature);
            values = workspace.TWorkspaceColumnRead(TVocabularyValue);
        }

        using LEngine reopened = workspace.TWorkspaceEngineStart();

        Assert.NotEmpty(parts);
        Assert.NotEmpty(features);
        Assert.NotEmpty(values);
        Assert.Equal(parts, workspace.TWorkspaceColumnRead(TVocabularySpeech));
        Assert.Equal(features, workspace.TWorkspaceColumnRead(TVocabularyFeature));
        Assert.Equal(values, workspace.TWorkspaceColumnRead(TVocabularyValue));
    }

    [Fact]
    public void SpeechValueCreate_RenamedPackRow_KeepsIdOnLinkedEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LSpeechValue noun = engine.TSpeechValueFind("English", "Noun")!;
        LEntry entry = engine.TEngineEntrySave(TVocabularyDraftCreate("Noun"));

        LSpeechValue renamed = TInterface.TSpeechArchiveCreate(workspace.TWorkspaceDatabase).TSpeechValueCreate(
            TInterface.TSpeechValueCreate("English", noun.LSpeechValueCode, "Substantive", noun.LSpeechValuePosition));

        Assert.Equal(noun.LSpeechValueId, renamed.LSpeechValueId);
        Assert.Null(engine.TSpeechValueFind("English", "Noun"));
        Assert.Equal(
            "Substantive",
            Assert.Single(TInterface.TSpeechNameRead(engine.TEngineEntryLoad(entry.LEntryId)!)));
    }

    [Fact]
    public void SpeechAdd_TypedName_TakesNegativePackId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LSpeechValue? first = engine.TEngineSpeechAdd("English", "Verb, ergative");
        LSpeechValue? second = engine.TEngineSpeechAdd("English", "Verb, ditransitive");
        LSpeechValue? again = engine.TEngineSpeechAdd("English", "verb, ERGATIVE");

        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.Equal(-1, first.LSpeechValueCode);
        Assert.Equal(-2, second.LSpeechValueCode);
        Assert.Equal(first.LSpeechValueId, again?.LSpeechValueId);
        Assert.Equal("Verb, ergative", engine.TEngineSpeechRead("English", first.LSpeechValueId)?.LSpeechValueName);
    }

    [Fact]
    public void EntrySave_CustomSpeech_StoresTextNoLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TVocabularyDraftCreate("Verb, ergative"));

        Assert.Equal(
            ["Verb, ergative"],
            workspace.TWorkspaceRowRead(
                $"SELECT custom_name FROM part_of_speech WHERE entry_parent = {entry.LEntryId};"));
        Assert.Equal(
            0,
            workspace.TWorkspaceCountRead(
                $"SELECT COUNT(*) FROM part_of_speech WHERE entry_parent = {entry.LEntryId} " +
                "AND speech_value_ref IS NOT NULL;"));
    }

    [Fact]
    public void EntrySave_DraftLinkingValueId_StoresLinkNoText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LSpeechValue verb = engine.TSpeechValueFind("English", "Verb")!;
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)],
            [],
            string.Empty,
            null,
            null) with { LEntryDraftSpeeches = [TInterface.TSpeechDraftCreate(verb.LSpeechValueId, "anything")] });

        Assert.Equal(
            [verb.LSpeechValueId],
            workspace.TWorkspaceColumnRead(
                $"SELECT speech_value_ref FROM part_of_speech WHERE entry_parent = {entry.LEntryId};"));
        Assert.Equal(
            "Verb",
            Assert.Single(TInterface.TSpeechNameRead(engine.TEngineEntryLoad(entry.LEntryId)!)));
    }

    [Fact]
    public void InflectionSave_FeatureLinkingValueId_ReadsBackSameId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TVocabularyDraftCreate("Noun"));
        LSpeechValue noun = engine.TSpeechValueFind("English", "Noun")!;
        LMorphology plural = engine.TParadigmMorphologyRead(entry.LEntryId, "plural");

        engine.TEntryInflectionSave(entry.LEntryId, [
            TInterface.TInflectionCreate(entry.LEntryId, 0, "words", null, noun.LSpeechValueId,
                [plural.LMorphologyId]),
        ]);

        LInflection stored = Assert.Single(engine.TEntryInflectionRead(entry.LEntryId));
        Assert.Equal(plural.LMorphologyId, Assert.Single(stored.LInflectionMorphology));
        Assert.Equal(
            [stored.LInflectionId],
            workspace.TWorkspaceColumnRead("SELECT inflection_parent FROM inflection_feature;"));
        Assert.Equal(
            ["number"],
            workspace.TWorkspaceRowRead(
                $"SELECT name FROM morphology_feature WHERE morphology_feature_id = {plural.LMorphologyFeatureId};"));
    }

    [Fact]
    public void InflectionSave_UnknownValueId_Throws()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TVocabularyDraftCreate("Noun"));

        Assert.Throws<LRefusal>(() => engine.TEntryInflectionSave(entry.LEntryId, [
            TInterface.TInflectionCreate(entry.LEntryId, 0, "words", null, null,
                [999999]),
        ]));
        Assert.Empty(engine.TEntryInflectionRead(entry.LEntryId));
    }

    private static LEntryDraft TVocabularyDraftCreate(params string[] speeches)
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
