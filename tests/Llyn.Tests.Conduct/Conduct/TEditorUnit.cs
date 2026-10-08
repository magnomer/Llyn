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
        CCardSpeech speech = TEditorUnitPrepare(engine, "English").TEditorFixtureSpeech;

        Assert.Equal(
            ["Unit.Content", "Unit.Function", "Unit.Morpheme"],
            speech.CCardSpeechRead().CMarkerUnits.Select(static row => row.Item1));
        Assert.DoesNotContain(speech.CCardSpeechRead().CMarkerUnits, static row => row.Item2);
    }

    [Fact]
    public void CardSpeechRead_UnspacedLanguage_OffersWordAndMorpheme()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CCardSpeech speech = TEditorUnitPrepare(engine, "Thai").TEditorFixtureSpeech;

        Assert.Equal(
            ["Unit.Word", "Unit.Morpheme"],
            speech.CCardSpeechRead().CMarkerUnits.Select(static row => row.Item1));
    }

    [Fact]
    public void EditorUnitSet_OfferedKey_MarksTheRowAndStoresTheUnit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorUnitPrepare(engine, "English");

        editor.TEditorFixtureEntry.CEntryUnitSet("Unit.Function");

        Assert.Equal(
            ("Unit.Function", true),
            Assert.Single(editor.TEditorFixtureSpeech.CCardSpeechRead().CMarkerUnits, static row => row.Item2));
        Assert.Equal(LUnit.LUnitFunction, editor.TEditorFixtureDesk.TDeskRead()!.LDraftContent.LEntryDraftUnit);
    }

    [Fact]
    public void EditorUnitSet_TakenKey_ClearsTheUnit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorUnitPrepare(engine, "English");
        editor.TEditorFixtureEntry.CEntryUnitSet("Unit.Morpheme");

        editor.TEditorFixtureEntry.CEntryUnitSet("Unit.Morpheme");

        Assert.DoesNotContain(editor.TEditorFixtureSpeech.CCardSpeechRead().CMarkerUnits, static row => row.Item2);
    }

    private static TEditorFixture TEditorUnitPrepare(LEngine engine, string language)
    {
        engine.TEngineDelaySet(0);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("input", LCatalogOrder.LCatalogOrderHeadword));
        editor.TEditorFixtureOpen(null);
        editor.TEditorFixtureEntry.CEntryLanguageSet(language);
        return editor;
    }
}
