using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditorUnit
{
    [Fact]
    public void CardSpeechRead_SpacedLanguage_OffersContentFunctionAndMorpheme()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorUnitPrepare(engine, "English");

        Assert.Equal(
            ["Unit.Content", "Unit.Function", "Unit.Morpheme"],
            editor.CEditorSpeech.CCardSpeechRead().CMarkerUnits.Select(static row => row.Item1));
        Assert.DoesNotContain(editor.CEditorSpeech.CCardSpeechRead().CMarkerUnits, static row => row.Item2);
    }

    [Fact]
    public void CardSpeechRead_UnspacedLanguage_OffersWordAndMorpheme()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorUnitPrepare(engine, "Thai");

        Assert.Equal(
            ["Unit.Word", "Unit.Morpheme"],
            editor.CEditorSpeech.CCardSpeechRead().CMarkerUnits.Select(static row => row.Item1));
    }

    [Fact]
    public void EditorUnitSet_OfferedKey_MarksTheRowAndStoresTheUnit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorUnitPrepare(engine, "English");

        editor.CEditorUnitSet("Unit.Function");

        Assert.Equal(
            ("Unit.Function", true),
            Assert.Single(editor.CEditorSpeech.CCardSpeechRead().CMarkerUnits, static row => row.Item2));
        Assert.Equal(LUnit.LUnitFunction, editor.CEditorDesk.TDeskRead()!.LDraftContent.LEntryDraftUnit);
    }

    [Fact]
    public void EditorUnitSet_TakenKey_ClearsTheUnit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorUnitPrepare(engine, "English");
        editor.CEditorUnitSet("Unit.Morpheme");

        editor.CEditorUnitSet("Unit.Morpheme");

        Assert.DoesNotContain(editor.CEditorSpeech.CCardSpeechRead().CMarkerUnits, static row => row.Item2);
    }

    private static CEditor TEditorUnitPrepare(LEngine engine, string language)
    {
        engine.TEngineDelaySet(0);
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("input", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(null);
        editor.CEditorLanguageSet(language);
        return editor;
    }
}
