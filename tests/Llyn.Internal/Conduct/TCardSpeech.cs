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
        CEditor editor = TCardSpeechPrepare(engine);

        bool offered = editor.CEditorSpeech.CCardSpeechSet(" verb");
        editor.CEditorDesk.CDeskPersist();

        Assert.True(offered);
        Assert.Equal(["verb"], TSpeechNamesRead(editor));
        CMarker marker = editor.CEditorSpeech.CCardSpeechRead();
        Assert.Empty(marker.CMarkerSpeeches);
        Assert.Equal(" verb", marker.CMarkerTyped);
        Assert.False(editor.CEditorSpeech.CCardSpeechSet("  "));
    }

    [Fact]
    public void SpeechAdd_CommittedName_BecomesAChipAndClearsTheText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TCardSpeechPrepare(engine);
        editor.CEditorSpeech.CCardSpeechSet("nou");

        editor.CEditorSpeech.CCardSpeechAdd(" Noun ");
        editor.CEditorSpeech.CCardSpeechAdd("noun");

        CMarker marker = editor.CEditorSpeech.CCardSpeechRead();
        Assert.Equal(["Noun"], marker.CMarkerSpeeches);
        Assert.Equal(string.Empty, marker.CMarkerTyped);
        Assert.Equal(["Noun"], TSpeechNamesRead(editor));
    }

    [Fact]
    public void SpeechAdd_BlankName_SendsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TCardSpeechPrepare(engine);

        editor.CEditorSpeech.CCardSpeechAdd("   ");

        Assert.False(editor.CEditorDesk.CDeskChanged);
        Assert.Empty(editor.CEditorSpeech.CCardSpeechRead().CMarkerSpeeches);
    }

    [Fact]
    public void SpeechRemove_Chip_DropsItAndKeepsTheTypedText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TCardSpeechPrepare(engine);
        editor.CEditorSpeech.CCardSpeechAdd("Noun");
        editor.CEditorSpeech.CCardSpeechAdd("Verb");
        editor.CEditorSpeech.CCardSpeechSet("adj");

        editor.CEditorSpeech.CCardSpeechRemove("Noun");

        CMarker marker = editor.CEditorSpeech.CCardSpeechRead();
        Assert.Equal(["Verb"], marker.CMarkerSpeeches);
        Assert.Equal("adj", marker.CMarkerTyped);
        Assert.Equal(["Verb", "adj"], TSpeechNamesRead(editor));
    }

    [Fact]
    public void SpeechRead_DraftChangedElsewhere_ClearsTheTypedText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TCardSpeechPrepare(engine);
        editor.CEditorSpeech.CCardSpeechAdd("Noun");
        editor.CEditorSpeech.CCardSpeechSet("adj");

        editor.CEditorEntryOpen(null);

        CMarker marker = editor.CEditorSpeech.CCardSpeechRead();
        Assert.Empty(marker.CMarkerSpeeches);
        Assert.Equal(string.Empty, marker.CMarkerTyped);
    }

    [Fact]
    public void SpeechRead_DraftUndone_ShowsTheDraftsOwnList()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TCardSpeechPrepare(engine);
        editor.CEditorSpeech.CCardSpeechAdd("Noun");

        editor.CEditorDesk.CDeskUndo();
        CMarker undone = editor.CEditorSpeech.CCardSpeechRead();
        editor.CEditorSpeech.CCardSpeechAdd("Verb");

        Assert.Empty(undone.CMarkerSpeeches);
        Assert.Equal(["Verb"], editor.CEditorSpeech.CCardSpeechRead().CMarkerSpeeches);
        Assert.Equal(["Verb"], TSpeechNamesRead(editor));
    }

    private static CEditor TCardSpeechPrepare(LEngine engine)
    {
        engine.TEngineDelaySet(0);
        CEditor editor = TInterfaceConduct.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("input", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(null);
        editor.CEditorLanguageSet("English");
        editor.CEditorSpeech.CCardSpeechRead();
        return editor;
    }

    private static string[] TSpeechNamesRead(CEditor editor)
    {
        editor.CEditorDesk.CDeskPersist();
        return editor.CEditorDesk.TDeskRead()!.LDraftContent.LEntryDraftSpeeches
            .Select(static speech => speech.LSpeechDraftName)
            .ToArray();
    }
}
