using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineParadigm
{
    [Fact]
    public void ParadigmRead_ChildOfDeclaredPart_ReturnsParentSlots()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TParadigmDraftCreate("word", "Verb, transitive"));

        IReadOnlyList<LParadigmSlot> slots = engine.TEngineParadigmRead(entry.LEntryId);
        Assert.Equal(["past", "past participle"], slots.Select(slot => slot.LParadigmSlotMorphology.LMorphologyName));
        Assert.All(slots, slot => Assert.Equal(7, slot.LParadigmSlotSpeech.LSpeechValueCode));
        Assert.All(slots, slot => Assert.Null(slot.LParadigmSlotInflection));
        Assert.All(slots, slot => Assert.Equal(LState.LStateUnspecified, slot.LParadigmSlotState));
    }

    [Fact]
    public void ParadigmRead_StoredInflection_MarksSlotSpecified()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TParadigmDraftCreate("word", "Verb, transitive"));
        LParadigmSlot past = engine.TEngineParadigmRead(entry.LEntryId)[0];
        engine.TEngineInflectionAppend(entry.LEntryId, [
            TInterface.TInflectionCreate(
                entry.LEntryId, 0, "worded", null, null, [past.LParadigmSlotMorphology.LMorphologyId]),
        ]);

        IReadOnlyList<LParadigmSlot> slots = engine.TEngineParadigmRead(entry.LEntryId);
        Assert.Equal(LState.LStateSpecified, slots[0].LParadigmSlotState);
        Assert.Equal("worded", slots[0].LParadigmSlotInflection?.LInflectionText);
        Assert.Equal(LState.LStateUnspecified, slots[1].LParadigmSlotState);
        Assert.Null(slots[1].LParadigmSlotInflection);
    }

    [Fact]
    public void ParadigmRead_ExceptedPart_ReturnsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry water = engine.TEngineEntrySave(TParadigmDraftCreate("water", "Noun, uncountable"));
        LEntry cat = engine.TEngineEntrySave(TParadigmDraftCreate("cat", "Noun, countable"));

        Assert.Empty(engine.TEngineParadigmRead(water.LEntryId));
        Assert.Single(engine.TEngineParadigmRead(cat.LEntryId));
    }

    [Fact]
    public void ParadigmRead_CustomPart_ReturnsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TParadigmDraftCreate("word", "Verb, ergative"));

        Assert.Empty(engine.TEngineParadigmRead(entry.LEntryId));
        Assert.Empty(engine.TEngineParadigmRead(entry.LEntryId + 1));
    }

    [Fact]
    public void ParadigmRead_ManyParts_ReturnsSlotsInPartOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TParadigmDraftCreate("word", "Noun", "Verb"));

        IReadOnlyList<LParadigmSlot> slots = engine.TEngineParadigmRead(entry.LEntryId);
        Assert.Equal(["plural", "past", "past participle"], slots.Select(slot => slot.LParadigmSlotMorphology.LMorphologyName));
        Assert.Equal([1L, 6L, 6L], slots.Select(slot => slot.LParadigmSlotSpeech.LSpeechValueCode));
    }

    [Fact]
    public void MorphologyCodeFind_PackCode_ReturnsLanguageRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LMorphologyArchive morphologies = TInterface.TMorphologyArchiveCreate(workspace.TWorkspaceDatabase);

        Assert.Equal("past", morphologies.TMorphologyCodeFind("English", 6, 2, 5)?.LMorphologyName);
        Assert.Null(morphologies.TMorphologyCodeFind("English", 6, 2, 999));
        Assert.Null(morphologies.TMorphologyCodeFind("Klingon", 6, 2, 5));
    }

    [Fact]
    public void ParadigmMatch_NounPattern_AcceptsRegularForm()
    {
        LSpeechPack pack = TInterface.TSpeechPackLoad("English");
        LParadigm noun = Assert.Single(pack.LSpeechPackParadigms, row => row.LParadigmSpeechCode == 1);

        Assert.True(TInterface.TEngineParadigmMatch(noun, "cat", "cats"));
        Assert.True(TInterface.TEngineParadigmMatch(noun, "Box", "boxes"));
        Assert.True(TInterface.TEngineParadigmMatch(noun, "city", "cities"));
        Assert.True(TInterface.TEngineParadigmMatch(noun, "knife", "knives"));
        Assert.True(TInterface.TEngineParadigmMatch(noun, "roof", "roofs"));
        Assert.True(TInterface.TEngineParadigmMatch(noun, "hero", "heroes"));
        Assert.True(TInterface.TEngineParadigmMatch(noun, "photo", "photos"));
        Assert.False(TInterface.TEngineParadigmMatch(noun, "mouse", "mice"));
        Assert.False(TInterface.TEngineParadigmMatch(noun, "child", "children"));
        Assert.False(TInterface.TEngineParadigmMatch(noun, "cat", ""));
        Assert.False(TInterface.TEngineParadigmMatch(TInterface.TParadigmCreate(1, [2]), "cat", "cats"));
    }

    [Fact]
    public void ParadigmRead_SharedForm_FillsBothSlots()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TParadigmDraftCreate("walk", "Verb"));
        IReadOnlyList<LParadigmSlot> empty = engine.TEngineParadigmRead(entry.LEntryId);
        engine.TEngineInflectionAppend(entry.LEntryId, [
            TInterface.TInflectionCreate(
                entry.LEntryId, 0, "walked", null, null, empty.Select(slot => slot.LParadigmSlotMorphology.LMorphologyId).ToList()),
        ]);

        IReadOnlyList<LParadigmSlot> slots = engine.TEngineParadigmRead(entry.LEntryId);
        Assert.Equal(2, slots.Count);
        Assert.All(slots, slot => Assert.Equal("walked", slot.LParadigmSlotInflection?.LInflectionText));
        Assert.Single(slots.Select(slot => slot.LParadigmSlotInflection?.LInflectionId).Distinct());
    }

    [Fact]
    public void ParadigmShow_RegularNounPlural_DropsSlot()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TParadigmDraftCreate("cat", "Noun"));
        TParadigmInflectionAppend(engine, entry.LEntryId, "cats");

        Assert.Empty(engine.TEngineParadigmShow(entry.LEntryId));
        Assert.Single(engine.TEngineParadigmRead(entry.LEntryId));
    }

    [Fact]
    public void ParadigmShow_IrregularNounPlural_KeepsSlot()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TParadigmDraftCreate("mouse", "Noun"));
        TParadigmInflectionAppend(engine, entry.LEntryId, "mice");

        LParadigmSlot slot = Assert.Single(engine.TEngineParadigmShow(entry.LEntryId));
        Assert.Equal("mice", slot.LParadigmSlotInflection?.LInflectionText);
        Assert.Equal(LState.LStateSpecified, slot.LParadigmSlotState);
    }

    [Fact]
    public void ParadigmShow_VerbWithoutPattern_KeepsBothSlots()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TParadigmDraftCreate("walk", "Verb"));
        TParadigmInflectionAppend(engine, entry.LEntryId, "walked", "walked");

        IReadOnlyList<LParadigmSlot> slots = engine.TEngineParadigmShow(entry.LEntryId);
        Assert.Equal(["walked", "walked"], slots.Select(slot => slot.LParadigmSlotInflection?.LInflectionText));
        Assert.All(slots, slot => Assert.Empty(slot.LParadigmSlotParadigm.LParadigmRegular));
    }

    [Fact]
    public void ParadigmShow_UnfilledSlot_KeepsSlot()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TParadigmDraftCreate("cat", "Noun"));

        LParadigmSlot slot = Assert.Single(engine.TEngineParadigmShow(entry.LEntryId));
        Assert.Equal(LState.LStateUnspecified, slot.LParadigmSlotState);
        Assert.Empty(engine.TEngineParadigmShow(entry.LEntryId + 1));
    }

    [Fact]
    public async Task MorphologySave_Off_CancelsPendingFetch()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TaskCompletionSource gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TParadigmFetchBody, gate.Task));
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TParadigmDraftCreate("go", "Verb, transitive"));

        engine.TEngineInflectionStart(entry.LEntryId);
        engine.TEngineMorphologySave(false);
        gate.SetResult();
        await Task.Delay(300);

        Assert.False(engine.TEngineSettingsRead().LSettingsMorphology);
        Assert.Empty(engine.TEngineInflectionRead(entry.LEntryId));
        Assert.All(engine.TEngineParadigmRead(entry.LEntryId), slot => Assert.Equal(LState.LStateUnspecified, slot.LParadigmSlotState));
    }

    private const string TParadigmFetchBody =
        "<b class=\"Latn form-of lang-en spast-form-of\" lang=\"en\"><a href=\"./went\">went</a></b>" +
        "<b class=\"Latn form-of lang-en past|part-form-of\" lang=\"en\"><a href=\"./gone\">gone</a></b>";

    private static void TParadigmInflectionAppend(LEngine engine, long entryId, params string[] forms)
    {
        IReadOnlyList<LParadigmSlot> slots = engine.TEngineParadigmRead(entryId);
        List<LInflection> inflections = [];
        for (int index = 0; index < forms.Length; index++)
        {
            inflections.Add(TInterface.TInflectionCreate(
                entryId, 0, forms[index], null, null, [slots[index].LParadigmSlotMorphology.LMorphologyId]));
        }

        engine.TEngineInflectionAppend(entryId, inflections);
    }

    private static LEntryDraft TParadigmDraftCreate(string headword, params string[] speeches)
    {
        return TInterface.TEntryDraftCreate(
            headword,
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
