using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditorUnit
{
    [Fact]
    public void CardSpeechRead_SpacedLanguage_OffersMorphemeWordAndExpression()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CCardSpeech speech = TEditorUnitPrepare(engine, "English").TEditorFixtureSpeech;

        Assert.Equal(
            ["Unit.Morpheme", "Unit.Word", "Unit.Expression"],
            speech.CCardSpeechRead().CMarkerUnits.Select(static row => row.Item1));
        Assert.DoesNotContain(speech.CCardSpeechRead().CMarkerUnits, static row => row.Item2);
    }

    [Fact]
    public void CardSpeechRead_UnspacedLanguage_OffersMorphemeWordAndExpression()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CCardSpeech speech = TEditorUnitPrepare(engine, "Thai").TEditorFixtureSpeech;

        Assert.Equal(
            ["Unit.Morpheme", "Unit.Word", "Unit.Expression"],
            speech.CCardSpeechRead().CMarkerUnits.Select(static row => row.Item1));
    }

    [Fact]
    public void CardSpeechRead_AnyLanguage_OffersDependentAndStandalone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CCardSpeech speech = TEditorUnitPrepare(engine, "Thai").TEditorFixtureSpeech;

        Assert.Equal(
            ["Freedom.Dependent", "Freedom.Standalone"],
            speech.CCardSpeechRead().CMarkerFreedoms.Select(static row => row.Item1));
        Assert.DoesNotContain(speech.CCardSpeechRead().CMarkerFreedoms, static row => row.Item2);
    }

    [Fact]
    public void EntryUnitSet_OfferedKey_MarksTheRowAndStoresTheUnit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorUnitPrepare(engine, "English");

        editor.TEditorFixtureEntry.CEntryUnitSet("Unit.Expression");

        Assert.Equal(
            ("Unit.Expression", true),
            Assert.Single(editor.TEditorFixtureSpeech.CCardSpeechRead().CMarkerUnits, static row => row.Item2));
        Assert.Equal(LUnit.LUnitExpression, editor.TEditorFixtureDesk.TDeskRead()!.LDraftContent.LEntryDraftUnit);
    }

    [Fact]
    public void EntryUnitSet_TakenKey_ClearsTheUnit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorUnitPrepare(engine, "English");
        editor.TEditorFixtureEntry.CEntryUnitSet("Unit.Morpheme");

        editor.TEditorFixtureEntry.CEntryUnitSet("Unit.Morpheme");

        Assert.DoesNotContain(editor.TEditorFixtureSpeech.CCardSpeechRead().CMarkerUnits, static row => row.Item2);
    }

    [Fact]
    public void EntryFreedomSet_OfferedKey_MarksTheRowAndStoresTheFreedom()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorUnitPrepare(engine, "English");

        editor.TEditorFixtureEntry.CEntryFreedomSet("Freedom.Standalone");

        Assert.Equal(
            ("Freedom.Standalone", true),
            Assert.Single(editor.TEditorFixtureSpeech.CCardSpeechRead().CMarkerFreedoms, static row => row.Item2));
        Assert.Equal(
            LFreedom.LFreedomStandalone,
            editor.TEditorFixtureDesk.TDeskRead()!.LDraftContent.LEntryDraftFreedom);
    }

    [Fact]
    public void EntryFreedomSet_TakenKey_ClearsTheFreedom()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorUnitPrepare(engine, "English");
        editor.TEditorFixtureEntry.CEntryFreedomSet("Freedom.Dependent");

        editor.TEditorFixtureEntry.CEntryFreedomSet("Freedom.Dependent");

        Assert.DoesNotContain(editor.TEditorFixtureSpeech.CCardSpeechRead().CMarkerFreedoms, static row => row.Item2);
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
