using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditorCardWalk
{
    [Fact]
    public void EditorCardRefine_OpenedEntry_ListsTheCardStepsOfTheEditorWalk()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a liquid", 1), TInterface.TCardCreate("a sea", 2)],
            []));
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.TEditorFixtureOpen(water.LEntryId);
        IReadOnlyList<IReadOnlyList<CStep>> walk = editor.TEditorFixtureEditor.CEditorWalk;
        List<long>? meanings = null;
        List<long>? collocations = null;

        TCardHinge.TCardHingeRun(() =>
        {
            StackPanel surface = TCardWalkCreate();
            QEditorCard card = new(surface);
            CEditor gate = editor.TEditorFixtureEditor;
            card.QEditorCardIntroduce(
                atelier,
                TEnvoyFake.TEnvoyCreate(false, []),
                null!,
                gate.CEditorEntry,
                gate.CEditorCard,
                gate.CEditorCaret,
                gate.CEditorSentence,
                gate.CEditorList,
                gate.CEditorField,
                gate.CEditorImage,
                gate.CEditorVideo,
                gate.CEditorEtymology);

            card.QEditorMeaningRefine(walk[0], gate.CEditorWalkCard[0]);
            card.QEditorCollocationRefine(walk[1], gate.CEditorWalkCard[1]);

            meanings = TCardWalkRead(surface, "PMeaningList");
            collocations = TCardWalkRead(surface, "PCollocationList");
        });

        Assert.Equal(
            editor.TEditorFixtureEditor.CEditorWalkCard[0].Select(at => walk[0][at].CStepCard!.CCardDraftId),
            meanings!);
        Assert.Equal(2, meanings!.Count);
        Assert.Equal(
            editor.TEditorFixtureEditor.CEditorWalkCard[1].Select(at => walk[1][at].CStepCard!.CCardDraftId),
            collocations!);
        Assert.NotEmpty(collocations!);
    }

    [Fact]
    public void EditorWalk_CaretMovedWithoutADraftChange_RestoresTheStepCaret()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.TEditorFixtureOpen(water.LEntryId);
        CEditor gate = editor.TEditorFixtureEditor;
        long card = gate.CEditorWalk[0][2].CStepCard!.CCardDraftId;
        gate.CEditorCaret.CCaretRegisterSettle(false, card, "slang, wild");
        Assert.Null(gate.CEditorWalk[0][2].CStepCaret![gate.CEditorCaret.CCaretRegister]);

        (bool moved, long? anchor) = gate.CEditorCaret.CCaretRegister.CCaretTrailMove(false, card, -1);

        Assert.True(moved);
        Assert.NotNull(anchor);
        Assert.Equal(anchor, gate.CEditorWalk[0][2].CStepCaret![gate.CEditorCaret.CCaretRegister]);
    }

    [Fact]
    public void EditorIntroduce_EntryWithSteps_EnablesTheUndoButton()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        CEditor gate = editor.TEditorFixtureEditor;
        gate.CEditorEntryOpen(null);
        gate.CEditorEntry.CEntryHeadwordSet("water");
        gate.CEditorDesk.CDeskDraft.CDeskDraftPersist();
        bool? enabled = null;

        TCardHinge.TCardHingeRun(() =>
        {
            StackPanel surface = TCardWalkCreate();
            TCardWalkOpen(surface, atelier, gate);
            enabled = ((Button)surface.Children.OfType<FrameworkElement>()
                .Single(child => child.Name == "PEditorBackward")).IsEnabled;
        });

        Assert.True(gate.CEditorDesk.CDeskChronicle.CDeskChronicleRead().CDeskBackward);
        Assert.True(enabled);
    }

    [Fact]
    public void EditorWalkChanged_DraftChanged_ListsTheCardsOnTheGui()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        CEditor gate = editor.TEditorFixtureEditor;
        List<long>? before = null;
        List<long>? after = null;

        TCardHinge.TCardHingeRun(() =>
        {
            StackPanel surface = TCardWalkCreate();
            TCardWalkOpen(surface, atelier, gate);
            before = TCardWalkRead(surface, "PMeaningList");
            gate.CEditorEntryOpen(water.LEntryId);
            after = TCardWalkRead(surface, "PMeaningList");
        });

        Assert.Empty(before!);
        Assert.Equal(
            gate.CEditorWalkCard[0].Select(at => gate.CEditorWalk[0][at].CStepCard!.CCardDraftId), after!);
        Assert.Single(after!);
    }

    [Fact]
    public void EditorWalkChanged_CaretMovedWithoutADraftChange_RebuildsTheCardLists()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        CEditor gate = editor.TEditorFixtureEditor;
        long? anchor = null;
        long? shown = null;

        TCardHinge.TCardHingeRun(() =>
        {
            StackPanel surface = TCardWalkCreate();
            TCardWalkOpen(surface, atelier, gate);
            gate.CEditorEntryOpen(water.LEntryId);
            ItemsControl list = surface.Children.OfType<ItemsControl>().Single(child => child.Name == "PMeaningList");
            long card = gate.CEditorWalk[0][2].CStepCard!.CCardDraftId;
            gate.CEditorCaret.CCaretRegisterSettle(false, card, "slang, wild");

            anchor = gate.CEditorCaret.CCaretRegister.CCaretTrailMove(false, card, -1).CCaretAnchor;
            QPlacard placard = ((IEnumerable)list.ItemsSource).Cast<QPlacard>().Single();
            shown = placard.QPlacardStep!.CStepCaret![gate.CEditorCaret.CCaretRegister];
        });

        Assert.NotNull(anchor);
        Assert.Equal(anchor, shown);
    }

    private static StackPanel TCardWalkCreate()
    {
        StackPanel surface = new();
        surface.Children.Add(new ItemsControl { Name = "PMeaningList" });
        surface.Children.Add(new Button { Name = "PMeaningAddition" });
        surface.Children.Add(new ItemsControl { Name = "PCollocationList" });
        surface.Children.Add(new Button { Name = "PCollocationAddition" });
        surface.Children.Add(new Rectangle { Name = "PEtymologyTrunk" });
        surface.Children.Add(new StackPanel { Name = "PEtymology" });
        surface.Children.Add(new ItemsControl { Name = "PEtymologySources" });
        surface.Children.Add(new TextBlock { Name = "PEtymologyHeadword" });
        surface.Children.Add(new Button { Name = "PEtymologyInsertion" });
        surface.Children.Add(new Border { Name = "PContents" });
        surface.Children.Add(new Popup { Name = "PProspect" });
        surface.Children.Add(new ListBox { Name = "PProspectList" });
        surface.Children.Add(new ToggleButton { Name = "PSpeaker" });
        surface.Children.Add(new Image { Name = "PSpeakerFlag" });
        surface.Children.Add(new Ellipse { Name = "PSpeakerGlobe" });
        surface.Children.Add(new TextBlock { Name = "PSpeakerName" });
        surface.Children.Add(new Popup { Name = "PLanguage" });
        surface.Children.Add(new ItemsControl { Name = "PLanguageList" });
        surface.Children.Add(new Popup { Name = "PProffer" });
        surface.Children.Add(new Border { Name = "PProfferSheet" });
        surface.Children.Add(new ListBox { Name = "PProfferList" });
        surface.Children.Add(new Popup { Name = "PSlate" });
        surface.Children.Add(new Border { Name = "PSlateSheet" });
        surface.Children.Add(new ListBox { Name = "PSlateList" });
        return surface;
    }

    private static QEditor TCardWalkOpen(StackPanel surface, CAtelier atelier, CEditor gate)
    {
        System.Windows.Application application =
            System.Windows.Application.Current ?? new System.Windows.Application();
        application.Resources["PIconRoot"] = "pack://application:,,,/Llyn.Tests.Windows;component/icons/";
        application.Resources["Theme.Popup.ProgressBar.Sweep"] = new DoubleAnimation().GetAsFrozen();
        application.Resources["Theme.Sound.Rebuild.Spin"] = new DoubleAnimation().GetAsFrozen();
        QEditor face = new(TCardWalkCreate(surface));
        face.QEditorIntroduce(atelier, TEnvoyFake.TEnvoyCreate(false, []), new QVolume(atelier), null!, gate);
        return face;
    }

    private static StackPanel TCardWalkCreate(StackPanel surface)
    {
        string[] blocks =
        [
            "PHeadwordGhost", "PEditorReading", "PSpeakerName", "PEditorGraspLabel", "PReflexLoading",
            "PAnchorEmpty", "PPronunciationLabel", "PPronunciationOpener", "PPronunciationMeasure",
            "PPronunciationCloser", "PUnitName", "PFreedomName", "PCategoryNotice", "PEditorFrequency",
            "PEditorFrequencyBand",
        ];
        string[] boxes = ["PHeadword", "PPronunciationField", "PMarkerField", "PNoteContents"];
        string[] toggles =
        [
            "PSpeaker", "PEditorFavorite", "PReflexHinge", "PUnitDropper", "PFreedomDropper", "PMarkerSwitch",
        ];
        string[] popups =
        [
            "PLanguage", "PAnchor", "PUnitDropdown", "PFreedomDropdown", "PCategory", "PNotation", "PClip",
        ];
        string[] lists =
        [
            "PLanguageList", "PReflex", "PAccent", "PTranscription", "PGlyph", "PUnitList", "PFreedomList",
            "PMarkerList", "PCategoryList", "PNotationList", "PClipList",
        ];
        string[] borders =
        [
            "PEditorCommand", "PPronunciation", "PPlayback", "PMarkerSurface", "PEditorFrequencyChip", "PStackPill",
            "PNote", "PNotationProgress", "PClipProgress",
        ];
        string[] buttons =
        [
            "PEditorBackward", "PEditorForward", "PEditorDiscard", "PEditorStore", "PPlaybackAction", "PPhonetician",
            "PDownloader", "PReflexRenewal", "PStackMeaning", "PStackCollocation", "PStackEtymology", "PStackNote",
        ];
        string[] panels =
        [
            "PEditorSound", "PReflexTable", "PAnchorList", "PEditorFrequencySection", "PStackTabs", "PMeaning",
            "PCollocation",
        ];
        Action<string[], Func<FrameworkElement>> add = (names, create) =>
        {
            foreach (string name in names)
            {
                FrameworkElement part = create();
                part.Name = name;
                surface.Children.Add(part);
            }
        };
        add(blocks, static () => new TextBlock());
        add(boxes, static () => new TextBox());
        add(toggles, static () => new ToggleButton());
        add(popups, static () => new Popup());
        add(lists, static () => new ItemsControl());
        add(borders, static () => new Border());
        add(buttons, static () => new Button { IsEnabled = false });
        add(panels, static () => new StackPanel());
        add(["PSpeakerFlag", "PPronunciationFlag"], static () => new Image());
        add(["PFavoriteMark", "PMarkerIcon"], static () => new QIconImage());
        add(["PEditorGrasp"], static () => new QGrasp());
        add(["PContour"], static () => new QContour());
        add(["PEditorParadigm"], static () => new QParadigm());
        add(["PEditorFanqie"], static () => new QFanqie());
        add(["PEditorScript"], static () => new QScript());
        add(["PMarker"], static () => new WrapPanel());
        add(["PReflexBlock", "PStack"], static () => new Grid());
        add(["PVolume"], static () => new Slider());
        add(["PNotationNotice", "PClipNotice"], static () => new TextBlock());
        surface.Children.Add(new ScrollViewer { Name = "PEditorContents" });
        return surface;
    }

    private static List<long> TCardWalkRead(StackPanel surface, string name)
    {
        ItemsControl list = surface.Children.OfType<ItemsControl>().Single(child => child.Name == name);
        return [.. ((IEnumerable)list.ItemsSource).Cast<QPlacard>().Select(static card => card.QPlacardId)];
    }
}
