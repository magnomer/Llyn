using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardSpeech
{
    [Fact]
    public void SpeechSet_TypedText_KeepsItPendingInTheDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TCardSpeechPrepare(engine);

        CCategory offered = editor.TEditorFixtureSpeech.CCardSpeechSet(" verb");
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();

        Assert.True(offered.CCategoryShown);
        Assert.Equal(["verb"], TSpeechNamesRead(editor));
        CMarker marker = editor.TEditorFixtureSpeech.CCardSpeechRead();
        Assert.Empty(marker.CMarkerSpeeches);
        Assert.Equal(" verb", marker.CMarkerTyped);
        Assert.False(editor.TEditorFixtureSpeech.CCardSpeechSet("  ").CCategoryShown);
    }

    [Fact]
    public void SpeechSet_TypedPart_OffersTheDraftLanguagesPartsMarkedWhenHeld()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CCardSpeech speech = TCardSpeechPrepare(engine).TEditorFixtureSpeech;
        speech.CCardSpeechAdd("Noun");

        CCategory held = speech.CCardSpeechSet("NOUN");
        CCategory unmatched = speech.CCardSpeechSet("zzqq");

        Assert.Contains(new CCategoryRow("Noun", true), held.CCategoryRows);
        Assert.Contains(new CCategoryRow("Pronoun", false), held.CCategoryRows);
        Assert.DoesNotContain(held.CCategoryRows, static row => row.CCategoryRowName == "Verb");
        Assert.Null(held.CCategoryHint);
        Assert.Empty(unmatched.CCategoryRows);
        Assert.False(unmatched.CCategoryShown);
        Assert.Equal("Speech.Absent", unmatched.CCategoryHint);
    }

    [Fact]
    public void SpeechRead_Draft_CarriesTheCategoryMenuForTheSettledText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CCardSpeech empty = new TEditorFixture(TInterfaceEditor.TEditorCreate(engine)).TEditorFixtureSpeech;
        CCardSpeech speech = TCardSpeechPrepare(engine).TEditorFixtureSpeech;
        speech.CCardSpeechAdd("Verb");

        CCategory shown = speech.CCardSpeechRead().CMarkerCategory;
        CCategory absent = empty.CCardSpeechRead().CMarkerCategory;

        Assert.Contains(new CCategoryRow("Verb", true), shown.CCategoryRows);
        Assert.Contains(new CCategoryRow("Noun", false), shown.CCategoryRows);
        Assert.False(shown.CCategoryShown);
        Assert.Null(shown.CCategoryHint);
        Assert.Empty(absent.CCategoryRows);
        Assert.Equal("Speech.Empty", absent.CCategoryHint);
    }

    [Fact]
    public void SpeechAdd_CommittedName_BecomesAChipAndClearsTheText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TCardSpeechPrepare(engine);
        editor.TEditorFixtureSpeech.CCardSpeechSet("nou");

        editor.TEditorFixtureSpeech.CCardSpeechAdd(" Noun ");
        editor.TEditorFixtureSpeech.CCardSpeechAdd("noun");

        CMarker marker = editor.TEditorFixtureSpeech.CCardSpeechRead();
        Assert.Equal(["Noun"], marker.CMarkerSpeeches);
        Assert.Equal(string.Empty, marker.CMarkerTyped);
        Assert.Equal(["Noun"], TSpeechNamesRead(editor));
    }

    [Fact]
    public void SpeechAdd_BlankName_SendsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TCardSpeechPrepare(engine);

        editor.TEditorFixtureSpeech.CCardSpeechAdd("   ");

        Assert.False(editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftAltered);
        Assert.Empty(editor.TEditorFixtureSpeech.CCardSpeechRead().CMarkerSpeeches);
    }

    [Fact]
    public void SpeechRemove_Chip_DropsItAndKeepsTheTypedText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TCardSpeechPrepare(engine);
        editor.TEditorFixtureSpeech.CCardSpeechAdd("Noun");
        editor.TEditorFixtureSpeech.CCardSpeechAdd("Verb");
        editor.TEditorFixtureSpeech.CCardSpeechSet("adj");

        editor.TEditorFixtureSpeech.CCardSpeechRemove("Noun");

        CMarker marker = editor.TEditorFixtureSpeech.CCardSpeechRead();
        Assert.Equal(["Verb"], marker.CMarkerSpeeches);
        Assert.Equal("adj", marker.CMarkerTyped);
        Assert.Equal(["Verb", "adj"], TSpeechNamesRead(editor));
    }

    [Fact]
    public void SpeechRead_DraftChangedElsewhere_ClearsTheTypedText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TCardSpeechPrepare(engine);
        editor.TEditorFixtureSpeech.CCardSpeechAdd("Noun");
        editor.TEditorFixtureSpeech.CCardSpeechSet("adj");

        editor.TEditorFixtureOpen(null);

        CMarker marker = editor.TEditorFixtureSpeech.CCardSpeechRead();
        Assert.Empty(marker.CMarkerSpeeches);
        Assert.Equal(string.Empty, marker.CMarkerTyped);
    }

    [Fact]
    public void SpeechRead_DraftUndone_ShowsTheDraftsOwnList()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TCardSpeechPrepare(engine);
        editor.TEditorFixtureSpeech.CCardSpeechAdd("Noun");

        editor.TEditorFixtureDesk.CDeskChronicle.CDeskChronicleUndo();
        CMarker undone = editor.TEditorFixtureSpeech.CCardSpeechRead();
        editor.TEditorFixtureSpeech.CCardSpeechAdd("Verb");

        Assert.Empty(undone.CMarkerSpeeches);
        Assert.Equal(["Verb"], editor.TEditorFixtureSpeech.CCardSpeechRead().CMarkerSpeeches);
        Assert.Equal(["Verb"], TSpeechNamesRead(editor));
    }

    private static TEditorFixture TCardSpeechPrepare(LEngine engine)
    {
        engine.TEngineDelaySet(0);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("input", LCatalogOrder.LCatalogOrderHeadword));
        editor.TEditorFixtureOpen(null);
        editor.TEditorFixtureEntry.CEntryLanguageSet("English");
        editor.TEditorFixtureSpeech.CCardSpeechRead();
        return editor;
    }

    private static string[] TSpeechNamesRead(TEditorFixture editor)
    {
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();
        return editor.TEditorFixtureDesk.TDeskRead()!.LDraftContent.LEntryDraftSpeeches
            .Select(static speech => speech.LSpeechDraftName)
            .ToArray();
    }
}
