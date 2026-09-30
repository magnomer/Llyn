using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QRepertoire : QChronicleHost
{
    private readonly UserControl _qRepertoireSurface;

    private PWindow _qRepertoireHost = null!;

    private CRepertoire _cRepertoire = null!;

    internal QRepertoire(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qRepertoireSurface = surface;
        QLook.QLookStyleAttach(surface.Resources);
        QChronicle.QChronicleIntroduce(surface, this);

        surface.Resources.MergedDictionaries.Add(new PImageTemplate());
        surface.Resources.MergedDictionaries.Add(new PVideoTemplate());

        surface.CommandBindings.Add(new CommandBinding(
            ApplicationCommands.Print, QRepertoirePressObserve, QRepertoirePressRefine));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QRepertoirePortraitObserve, QRepertoirePortraitRefine));
        QRepertoirePortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        QRepertoirePress.Command = ApplicationCommands.Print;

        QChoice.QChoiceDropperAttach(QTierDropper, QTierDropdown, QTier);
        QChoice.QChoiceDropperAttach(QMeshDropper, QMeshDropdown, QMeshDropper);

        QTierIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        QMeshIcon.QIconSource = QIcon.QIconResolve("filter", 24);
        QScenarioPictureIcon.QIconSource = QIcon.QIconResolve("image", 24);
        QScenarioFilmIcon.QIconSource = QIcon.QIconResolve("video", 24);
        QRepertoireBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        QInquest.SetResourceReference(QField.QFieldHintProperty, "Situation.Search");
        QSortie.SetResourceReference(QField.QFieldHintProperty, "Sortie.Search");
        QRepertoireFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        QRepertoireStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        QRepertoireEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        QRepertoireLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        QRepertoireBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        QRepertoireForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        QRepertoirePortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        QRepertoirePress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        QRepertoireViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        QRepertoireScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        QInquest.TextChanged += QInquestObserve;
        QSortie.TextChanged += QSortieObserve;
        QRepertoireFresh.Click += QRepertoireFreshObserve;
        QRepertoireStore.Click += QRepertoireStoreObserve;
        QRepertoireEarlier.Click += QRepertoireRetreatObserve;
        QRepertoireLater.Click += QRepertoireAdvanceObserve;
        QRepertoireBackward.Click += QRepertoireUndoObserve;
        QRepertoireForward.Click += QRepertoireRedoObserve;
        QRepertoireViewer.Click += QRepertoireViewerObserve;
        QRepertoireScribe.Click += QRepertoireScribeObserve;
        QRepertoireBin.Click += QRepertoireBinObserve;
        QScenarioIntroduce();
        QScenarioPicture.Click += QImageAddObserve;
        QScenarioFilm.Click += QVideoAddObserve;
    }

    private Border QTier => QContract.QContractFind<Border>(_qRepertoireSurface, "PTier");

    private ToggleButton QTierDropper => QContract.QContractFind<ToggleButton>(_qRepertoireSurface, "PTierDropper");

    private QIconImage QTierIcon => QContract.QContractFind<QIconImage>(_qRepertoireSurface, "PTierIcon");

    private Popup QTierDropdown => QContract.QContractFind<Popup>(_qRepertoireSurface, "PTierDropdown");

    private StackPanel QTierList => QContract.QContractFind<StackPanel>(_qRepertoireSurface, "PTierList");

    private TextBox QInquest => QContract.QContractFind<TextBox>(_qRepertoireSurface, "PInquest");

    private TextBox QSortie => QContract.QContractFind<TextBox>(_qRepertoireSurface, "PSortie");

    private ToggleButton QMeshDropper => QContract.QContractFind<ToggleButton>(_qRepertoireSurface, "PMeshDropper");

    private QIconImage QMeshIcon => QContract.QContractFind<QIconImage>(_qRepertoireSurface, "PMeshIcon");

    private Ellipse QMeshMark => QContract.QContractFind<Ellipse>(_qRepertoireSurface, "PMeshMark");

    private Popup QMeshDropdown => QContract.QContractFind<Popup>(_qRepertoireSurface, "PMeshDropdown");

    private StackPanel QMeshList => QContract.QContractFind<StackPanel>(_qRepertoireSurface, "PMeshList");

    private Button QRepertoireFresh => QContract.QContractFind<Button>(_qRepertoireSurface, "PRepertoireFresh");

    private Button QRepertoireStore => QContract.QContractFind<Button>(_qRepertoireSurface, "PRepertoireStore");

    private StackPanel QRepertoireVoyage =>
        QContract.QContractFind<StackPanel>(_qRepertoireSurface, "PRepertoireVoyage");

    private Button QRepertoireEarlier => QContract.QContractFind<Button>(_qRepertoireSurface, "PRepertoireEarlier");

    private Button QRepertoireLater => QContract.QContractFind<Button>(_qRepertoireSurface, "PRepertoireLater");

    private StackPanel QRepertoireChronicle =>
        QContract.QContractFind<StackPanel>(_qRepertoireSurface, "PRepertoireChronicle");

    private Button QRepertoireBackward => QContract.QContractFind<Button>(_qRepertoireSurface, "PRepertoireBackward");

    private Button QRepertoireForward => QContract.QContractFind<Button>(_qRepertoireSurface, "PRepertoireForward");

    private Button QRepertoirePortrait => QContract.QContractFind<Button>(_qRepertoireSurface, "PRepertoirePortrait");

    private Button QRepertoirePress => QContract.QContractFind<Button>(_qRepertoireSurface, "PRepertoirePress");

    private Border QRepertoireMode => QContract.QContractFind<Border>(_qRepertoireSurface, "PRepertoireMode");

    private RadioButton QRepertoireViewer =>
        QContract.QContractFind<RadioButton>(_qRepertoireSurface, "PRepertoireViewer");

    private RadioButton QRepertoireScribe =>
        QContract.QContractFind<RadioButton>(_qRepertoireSurface, "PRepertoireScribe");

    private ItemsControl QAtlas => QContract.QContractFind<ItemsControl>(_qRepertoireSurface, "PAtlas");

    private TextBlock QAtlasEmpty => QContract.QContractFind<TextBlock>(_qRepertoireSurface, "PAtlasEmpty");

    private ItemsControl QOccurrence => QContract.QContractFind<ItemsControl>(_qRepertoireSurface, "POccurrence");

    private TextBlock QOccurrenceEmpty => QContract.QContractFind<TextBlock>(_qRepertoireSurface, "POccurrenceEmpty");

    private PDisplay QRepertoireDisplay => QContract.QContractFind<PDisplay>(_qRepertoireSurface, "PDisplay");

    private PEditor QRepertoireEditor => QContract.QContractFind<PEditor>(_qRepertoireSurface, "PEditor");

    private Grid QVignette => QContract.QContractFind<Grid>(_qRepertoireSurface, "PVignette");

    private StackPanel QVignetteBody => QContract.QContractFind<StackPanel>(_qRepertoireSurface, "PVignetteBody");

    private TextBlock QVignetteTitle => QContract.QContractFind<TextBlock>(_qRepertoireSurface, "PVignetteTitle");

    private Border QVignetteChip => QContract.QContractFind<Border>(_qRepertoireSurface, "PVignetteChip");

    private TextBlock QVignetteKind => QContract.QContractFind<TextBlock>(_qRepertoireSurface, "PVignetteKind");

    private TextBlock QVignetteTally => QContract.QContractFind<TextBlock>(_qRepertoireSurface, "PVignetteTally");

    private StackPanel QVignetteDescriptionSection =>
        QContract.QContractFind<StackPanel>(_qRepertoireSurface, "PVignetteDescriptionSection");

    private StackPanel QVignetteDescription =>
        QContract.QContractFind<StackPanel>(_qRepertoireSurface, "PVignetteDescription");

    private ItemsControl QVignettePicture =>
        QContract.QContractFind<ItemsControl>(_qRepertoireSurface, "PVignettePicture");

    private ItemsControl QVignetteVideo => QContract.QContractFind<ItemsControl>(_qRepertoireSurface, "PVignetteVideo");

    private TextBlock QVignetteUnselected =>
        QContract.QContractFind<TextBlock>(_qRepertoireSurface, "PVignetteUnselected");

    private Grid QScenario => QContract.QContractFind<Grid>(_qRepertoireSurface, "PScenario");

    private TextBlock QScenarioHint => QContract.QContractFind<TextBlock>(_qRepertoireSurface, "PScenarioHint");

    private TextBlock QScenarioGhost => QContract.QContractFind<TextBlock>(_qRepertoireSurface, "PScenarioGhost");

    private TextBox QScenarioTitle => QContract.QContractFind<TextBox>(_qRepertoireSurface, "PScenarioTitle");

    private TextBlock QScenarioMeasure => QContract.QContractFind<TextBlock>(_qRepertoireSurface, "PScenarioMeasure");

    private TextBox QScenarioKind => QContract.QContractFind<TextBox>(_qRepertoireSurface, "PScenarioKind");

    private TextBlock QScenarioTally => QContract.QContractFind<TextBlock>(_qRepertoireSurface, "PScenarioTally");

    private TextBox QScenarioDescription =>
        QContract.QContractFind<TextBox>(_qRepertoireSurface, "PScenarioDescription");

    private ItemsControl QScenarioImage => QContract.QContractFind<ItemsControl>(_qRepertoireSurface, "PScenarioImage");

    private ItemsControl QScenarioVideo => QContract.QContractFind<ItemsControl>(_qRepertoireSurface, "PScenarioVideo");

    private Button QScenarioPicture => QContract.QContractFind<Button>(_qRepertoireSurface, "PScenarioPicture");

    private QIconImage QScenarioPictureIcon =>
        QContract.QContractFind<QIconImage>(_qRepertoireSurface, "PScenarioPictureIcon");

    private Button QScenarioFilm => QContract.QContractFind<Button>(_qRepertoireSurface, "PScenarioFilm");

    private QIconImage QScenarioFilmIcon =>
        QContract.QContractFind<QIconImage>(_qRepertoireSurface, "PScenarioFilmIcon");

    private Button QRepertoireBin => QContract.QContractFind<Button>(_qRepertoireSurface, "PRepertoireBin");

    private QIconImage QRepertoireBinIcon =>
        QContract.QContractFind<QIconImage>(_qRepertoireSurface, "PRepertoireBinIcon");

    internal void QRepertoireIntroduce(PWindow host)
    {
        _qRepertoireHost = host;
        _cRepertoire = CRepertoire.CRepertoireCreate(
            host.PWindowAtelier,
            QRepertoireShownCheck,
            host.PWindowEnvoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        QLectern lectern = new(
            _cRepertoire.CRepertoireEditor.CEditorDisplay, _cRepertoire.CRepertoireOccurrence.COccurrencePanel);
        QScenarioDeskIntroduce();

        QChoice.QChoiceOrderBuild(QTierList, "Tier", QTierObserve, CAtlas.CAtlasOrderRead());
        QAtlas.ItemsSource = _qAtlasList;
        QOccurrence.ItemsSource = _qOccurrenceList;
        QScenarioImage.ItemsSource = _qScenarioImage;
        QScenarioVideo.ItemsSource = _qScenarioVideo;
        QLookItem.QLookItemAttach(QAtlas, QAtlasItemRefine);
        QLookItem.QLookItemAttach(QOccurrence, QOccurrenceItemRefine);
        QLookItem.QLookItemAttach(QScenarioImage, QImageItemRefine);
        QLookItem.QLookItemAttach(QScenarioVideo, QVideoItemRefine);
        QLookItem.QLookItemAttach(QVignettePicture, PImage.PImageLineApply);
        QLookItem.QLookItemAttach(QVignetteVideo, PVideo.PVideoLineApply);

        PMedia.PMediaAttach(_qRepertoireSurface);
        QRepertoireDisplay.PDisplayAttach(host, lectern);
        QRepertoireEditor.PEditorIntroduce(host, new QEditor(_cRepertoire.CRepertoireEditor));

        CPanel atlas = _cRepertoire.CRepertoireAtlas.CAtlasPanel;
        CPanel occurrence = _cRepertoire.CRepertoireOccurrence.COccurrencePanel;
        _cRepertoire.CRepertoireChanged += QRepertoireModeRefine;
        _cRepertoire.CRepertoireScenarioChanged += QScenarioRefine;
        _cRepertoire.CRepertoireSituationChanged += QVignetteRefine;
        _cRepertoire.CRepertoireQueryCleared += QInquestRefine;
        _cRepertoire.CRepertoireWorkspaceChanged += QRepertoireWorkspaceRefine;
        atlas.CPanelChanged += QRepertoireModeRefine;
        atlas.CPanelRowsChanged += QAtlasRefine;
        atlas.CPanelRowsChanged += QRepertoireTallyRefine;
        atlas.CPanelCleared += QVignetteClearRefine;
        occurrence.CPanelChanged += QRepertoireModeRefine;
        occurrence.CPanelRowsChanged += QOccurrenceRefine;
    }

    private bool QRepertoireShownCheck()
    {
        return _qRepertoireSurface.IsVisible;
    }

    private void QRepertoireModeRefine()
    {
        QScenario.Visibility = QLook.QLookVisibleRead(_cRepertoire.CRepertoireScenarioShown);
        QVignette.Visibility = QLook.QLookVisibleRead(_cRepertoire.CRepertoireVignetteShown);
        QRepertoireDisplay.Visibility = QLook.QLookVisibleRead(_cRepertoire.CRepertoireDisplayShown);
        QRepertoireEditor.Visibility = QLook.QLookVisibleRead(_cRepertoire.CRepertoireEditorShown);
        QVignetteBody.Visibility = QLook.QLookVisibleRead(_cRepertoire.CRepertoireVignetteHeld);
        QVignetteUnselected.Visibility = QLook.QLookVisibleRead(_cRepertoire.CRepertoireVignetteBlank);
        QRepertoireViewer.IsChecked = _cRepertoire.CRepertoireViewerChecked;
        QRepertoireScribe.IsChecked = _cRepertoire.CRepertoireScribeChecked;
        QRepertoireVoyage.Visibility = QLook.QLookVisibleRead(_cRepertoire.CRepertoireViewerChecked);
        QRepertoireChronicle.Visibility = QLook.QLookVisibleRead(_cRepertoire.CRepertoireScribeChecked);
        QRepertoireMode.IsEnabled = _cRepertoire.CRepertoireModeEnabled;
        QRepertoireBin.IsEnabled = _cRepertoire.CRepertoireBinEnabled;
        QRepertoireStore.IsEnabled = _cRepertoire.CRepertoireStoreEnabled;
        QScenario.IsEnabled = _cRepertoire.CRepertoireDesk.CDeskRunning;
        QRepertoireChronicleRefine();
    }

    internal void QRepertoireExitRefine()
    {
        QRepertoireEditor.PEditorPlayerRefine();
        QTierDropdown.IsOpen = false;
        QMeshDropdown.IsOpen = false;
    }

    private void QRepertoirePressRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cRepertoire?.CRepertoirePressAllowed ?? false;
    }

    private async void QRepertoirePressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cRepertoire.CRepertoirePortraitPrint();
    }

    private void QRepertoirePortraitRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cRepertoire?.CRepertoirePortraitAllowed ?? false;
    }

    private async void QRepertoirePortraitObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cRepertoire.CRepertoirePortraitExport();
    }
}
