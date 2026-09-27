using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Llyn.UIDeportment;

internal sealed partial class QRepertoire : PImageHost, PVideoHost, PChronicleHost
{
    private readonly UserControl _qRepertoireSurface;

    private PWindow _qRepertoireHost = null!;

    private LRepertoire _lRepertoire = null!;

    internal QRepertoire(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qRepertoireSurface = surface;
        QLook.QLookStyleAttach(surface.Resources);
        PChronicle.PChronicleAttach(surface, this);

        _qImageTemplate = new PImageTemplate(this);
        surface.Resources.MergedDictionaries.Add(_qImageTemplate);
        _qVideoTemplate = new PVideoTemplate(this);
        surface.Resources.MergedDictionaries.Add(_qVideoTemplate);

        surface.CommandBindings.Add(new CommandBinding(
            ApplicationCommands.Print, QRepertoirePressHandle, QRepertoirePressCheck));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QRepertoirePortraitHandle, QRepertoirePortraitCheck));
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

        QInquest.TextChanged += QInquestHandle;
        QSortie.TextChanged += QSortieHandle;
        QRepertoireFresh.Click += QRepertoireFreshHandle;
        QRepertoireStore.Click += QRepertoireStoreHandle;
        QRepertoireEarlier.Click += QRepertoireRetreatHandle;
        QRepertoireLater.Click += QRepertoireAdvanceHandle;
        QRepertoireBackward.Click += QRepertoireUndoHandle;
        QRepertoireForward.Click += QRepertoireRedoHandle;
        QRepertoireViewer.Click += QRepertoireViewerHandle;
        QRepertoireScribe.Click += QRepertoireScribeHandle;
        QRepertoireBin.Click += QRepertoireBinHandle;
        QScenarioAttach();
        QScenarioPicture.Click += QImageAddHandle;
        QScenarioFilm.Click += QVideoAddHandle;
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

    internal void QRepertoireAttach(PWindow host)
    {
        _qRepertoireHost = host;
        LEditor editor = host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm);
        LLectern lectern = new(editor.LEditorDisplay);
        _lRepertoire = host.PWindowDeportment.LWindowRepertoireCreate(
            editor,
            lectern,
            QRepertoireShownCheck,
            QRepertoireDiscardConfirm,
            QRepertoireRemovalConfirm,
            host.PWindowUnreadableConfirm);
        QScenarioDeskAttach();

        QAtlas.ItemsSource = _qAtlasList;
        QOccurrence.ItemsSource = _qOccurrenceList;
        QScenarioImage.ItemsSource = _qScenarioImage;
        QScenarioVideo.ItemsSource = _qScenarioVideo;
        QLookItem.QLookItemAttach(QAtlas, QAtlasApply);
        QLookItem.QLookItemAttach(QOccurrence, QOccurrenceApply);
        QLookItem.QLookItemAttach(QScenarioImage, QImageApply);
        QLookItem.QLookItemAttach(QScenarioVideo, QVideoApply);
        QLookItem.QLookItemAttach(QVignettePicture, PImage.PImageLineApply);
        QLookItem.QLookItemAttach(QVignetteVideo, PVideo.PVideoLineApply);

        PMedia.PMediaAttach(_qRepertoireSurface, host.PWindowDeportment);
        QRepertoireDisplay.PDisplayAttach(host, lectern);
        QRepertoireEditor.PEditorAttach(host, editor, lectern);

        LPanel atlas = _lRepertoire.LRepertoireAtlas.LAtlasPanel;
        LPanel occurrence = _lRepertoire.LRepertoireOccurrence.LOccurrencePanel;
        _lRepertoire.LRepertoireChanged += QRepertoireModeUpdate;
        _lRepertoire.LRepertoireScenarioChanged += QScenarioApply;
        _lRepertoire.LRepertoireSituationChanged += QVignetteShow;
        _lRepertoire.LRepertoireFailed += host.PWindowFailureShow;
        _lRepertoire.LRepertoireInquestCleared += QInquestClear;
        atlas.LPanelChanged += QRepertoireModeUpdate;
        atlas.LPanelRowsChanged += QAtlasFind;
        atlas.LPanelCleared += QVignetteClear;
        atlas.LPanelFailed += host.PWindowFailureShow;
        occurrence.LPanelChanged += QRepertoireModeUpdate;
        occurrence.LPanelRowsChanged += QOccurrenceFind;
        occurrence.LPanelFailed += host.PWindowFailureShow;
    }

    private bool QRepertoireShownCheck()
    {
        return _qRepertoireSurface.IsVisible;
    }

    private bool QRepertoireDiscardConfirm(Func<bool, bool> finish)
    {
        return _qRepertoireHost.PWindowDiscardConfirm(true, finish);
    }

    private bool QRepertoireRemovalConfirm(int usage)
    {
        return _qRepertoireHost.PWindowRemovalConfirm(usage);
    }

    private void QRepertoireModeUpdate()
    {
        QScenario.Visibility = QLook.QLookVisibleRead(_lRepertoire.LRepertoireScenarioShown);
        QVignette.Visibility = QLook.QLookVisibleRead(_lRepertoire.LRepertoireVignetteShown);
        QRepertoireDisplay.Visibility = QLook.QLookVisibleRead(_lRepertoire.LRepertoireDisplayShown);
        QRepertoireEditor.Visibility = QLook.QLookVisibleRead(_lRepertoire.LRepertoireEditorShown);
        QVignetteBody.Visibility = QLook.QLookVisibleRead(_lRepertoire.LRepertoireVignetteHeld);
        QVignetteUnselected.Visibility = QLook.QLookVisibleRead(_lRepertoire.LRepertoireVignetteBlank);
        QRepertoireViewer.IsChecked = QLook.QLookCheckedRead(_lRepertoire.LRepertoireViewerChecked);
        QRepertoireScribe.IsChecked = QLook.QLookCheckedRead(_lRepertoire.LRepertoireScribeChecked);
        QRepertoireVoyage.Visibility = QLook.QLookVisibleRead(_lRepertoire.LRepertoireViewerChecked);
        QRepertoireChronicle.Visibility = QLook.QLookVisibleRead(_lRepertoire.LRepertoireScribeChecked);
        QRepertoireMode.IsEnabled = _lRepertoire.LRepertoireModeEnabled;
        QRepertoireBin.IsEnabled = _lRepertoire.LRepertoireBinEnabled;
        QRepertoireStore.IsEnabled = _lRepertoire.LRepertoireStoreEnabled;
        QScenario.IsEnabled = _lRepertoire.LRepertoireDesk.LDeskRunning;
        PChronicleUpdate();
    }

    internal void QRepertoireReset()
    {
        _lRepertoire.LRepertoireClear();
    }

    internal bool QRepertoireChangeCheck()
    {
        return _lRepertoire.LRepertoireChangeCheck();
    }

    internal bool QRepertoireDraftFinish(bool store)
    {
        return _lRepertoire.LRepertoireDraftFinish(store);
    }

    internal void QRepertoireClose()
    {
        QRepertoireEditor.PEditorClose();
        QRepertoireDisplay.PDisplayClose();
        QTierDropdown.IsOpen = false;
        QMeshDropdown.IsOpen = false;
    }

    private void QRepertoirePressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lRepertoire?.LRepertoirePressAllowed ?? false;
    }

    private async void QRepertoirePressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _qRepertoireHost.PWindowPressRun("Situation", _lRepertoire.LRepertoirePortraitPrint);
    }

    private void QRepertoirePortraitCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lRepertoire?.LRepertoirePortraitAllowed ?? false;
    }

    private async void QRepertoirePortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _qRepertoireHost.PWindowPortraitExport(
            _lRepertoire.LRepertoireOccurrence.LOccurrenceFileRead(), _lRepertoire.LRepertoirePortraitExport);
    }
}
