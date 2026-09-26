using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PRepertoire : UserControl, PImageHost, PVideoHost, PChronicleHost
{
    private PWindow _pRepertoireHost = null!;

    private LRepertoire _lRepertoire = null!;

    public PRepertoire()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Repertoire/PRepertoire.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));
        PLook.PLookStyleAttach(surface.Resources);

        _pImageTemplate = new PImageTemplate(this);
        Resources.MergedDictionaries.Add(_pImageTemplate);
        _pVideoTemplate = new PVideoTemplate(this);
        Resources.MergedDictionaries.Add(_pVideoTemplate);

        CommandBindings.Add(new CommandBinding(
            ApplicationCommands.Print, PRepertoirePressHandle, PRepertoirePressCheck));
        CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, PRepertoirePortraitHandle, PRepertoirePortraitCheck));
        PRepertoirePortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        PRepertoirePress.Command = ApplicationCommands.Print;

        PChoice.PChoiceDropperAttach(PTierDropper, PTierDropdown, PTier);
        PChoice.PChoiceDropperAttach(PMeshDropper, PMeshDropdown, PMeshDropper);

        PTierIcon.PIconSource = PIcon.PIconResolve("sort", 24);
        PMeshIcon.PIconSource = PIcon.PIconResolve("filter", 24);
        PScenarioPictureIcon.PIconSource = PIcon.PIconResolve("image", 24);
        PScenarioFilmIcon.PIconSource = PIcon.PIconResolve("video", 24);
        PRepertoireBinIcon.PIconSource = PIcon.PIconResolve("delete", 24);
        PRepertoireFresh.Tag = PIcon.PIconResolve("new", 24);
        PRepertoireStore.Tag = PIcon.PIconResolve("save", 24);
        PRepertoireEarlier.Tag = PIcon.PIconResolve("retreat", 24);
        PRepertoireLater.Tag = PIcon.PIconResolve("advance", 24);
        PRepertoireBackward.Tag = PIcon.PIconResolve("undo", 24);
        PRepertoireForward.Tag = PIcon.PIconResolve("redo", 24);
        PRepertoirePortrait.Tag = PIcon.PIconResolve("export", 24);
        PRepertoirePress.Tag = PIcon.PIconResolve("print", 24);
        PRepertoireViewer.Tag = PIcon.PIconResolve("view", 24);
        PRepertoireScribe.Tag = PIcon.PIconResolve("edit", 24);

        PInquest.TextChanged += PInquestHandle;
        PSortie.TextChanged += PSortieHandle;
        PRepertoireFresh.Click += PRepertoireFreshHandle;
        PRepertoireStore.Click += PRepertoireStoreHandle;
        PRepertoireEarlier.Click += PRepertoireRetreatHandle;
        PRepertoireLater.Click += PRepertoireAdvanceHandle;
        PRepertoireBackward.Click += PRepertoireUndoHandle;
        PRepertoireForward.Click += PRepertoireRedoHandle;
        PRepertoireViewer.Click += PRepertoireScribeHandle;
        PRepertoireScribe.Click += PRepertoireScribeHandle;
        PRepertoireBin.Click += PRepertoireBinHandle;
        PScenarioTitle.TextChanged += PScenarioTitleHandle;
        PScenarioKind.TextChanged += PScenarioKindHandle;
        PScenarioDescription.TextChanged += PScenarioDescriptionHandle;
        PScenarioImage.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(PScenarioImageChange));
        PScenarioVideo.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(PScenarioVideoChange));
        PScenarioPicture.Click += PImageAddHandle;
        PScenarioFilm.Click += PVideoAddHandle;
    }

    private Border PTier => (Border)FindName(nameof(PTier));

    private ToggleButton PTierDropper => (ToggleButton)FindName(nameof(PTierDropper));

    private PIconImage PTierIcon => (PIconImage)FindName(nameof(PTierIcon));

    private Popup PTierDropdown => (Popup)FindName(nameof(PTierDropdown));

    private StackPanel PTierList => (StackPanel)FindName(nameof(PTierList));

    private TextBox PInquest => (TextBox)FindName(nameof(PInquest));

    private TextBox PSortie => (TextBox)FindName(nameof(PSortie));

    private ToggleButton PMeshDropper => (ToggleButton)FindName(nameof(PMeshDropper));

    private PIconImage PMeshIcon => (PIconImage)FindName(nameof(PMeshIcon));

    private Ellipse PMeshMark => (Ellipse)FindName(nameof(PMeshMark));

    private Popup PMeshDropdown => (Popup)FindName(nameof(PMeshDropdown));

    private StackPanel PMeshList => (StackPanel)FindName(nameof(PMeshList));

    private Button PRepertoireFresh => (Button)FindName(nameof(PRepertoireFresh));

    private Button PRepertoireStore => (Button)FindName(nameof(PRepertoireStore));

    private StackPanel PRepertoireVoyage => (StackPanel)FindName(nameof(PRepertoireVoyage));

    private Button PRepertoireEarlier => (Button)FindName(nameof(PRepertoireEarlier));

    private Button PRepertoireLater => (Button)FindName(nameof(PRepertoireLater));

    private StackPanel PRepertoireChronicle => (StackPanel)FindName(nameof(PRepertoireChronicle));

    private Button PRepertoireBackward => (Button)FindName(nameof(PRepertoireBackward));

    private Button PRepertoireForward => (Button)FindName(nameof(PRepertoireForward));

    private Button PRepertoirePortrait => (Button)FindName(nameof(PRepertoirePortrait));

    private Button PRepertoirePress => (Button)FindName(nameof(PRepertoirePress));

    private Border PRepertoireMode => (Border)FindName(nameof(PRepertoireMode));

    private RadioButton PRepertoireViewer => (RadioButton)FindName(nameof(PRepertoireViewer));

    private RadioButton PRepertoireScribe => (RadioButton)FindName(nameof(PRepertoireScribe));

    private ItemsControl PAtlas => (ItemsControl)FindName(nameof(PAtlas));

    private TextBlock PAtlasEmpty => (TextBlock)FindName(nameof(PAtlasEmpty));

    private ItemsControl POccurrence => (ItemsControl)FindName(nameof(POccurrence));

    private TextBlock POccurrenceEmpty => (TextBlock)FindName(nameof(POccurrenceEmpty));

    private PDisplay PDisplay => (PDisplay)FindName(nameof(PDisplay));

    private PEditor PEditor => (PEditor)FindName(nameof(PEditor));

    private Grid PVignette => (Grid)FindName(nameof(PVignette));

    private StackPanel PVignetteBody => (StackPanel)FindName(nameof(PVignetteBody));

    private TextBlock PVignetteTitle => (TextBlock)FindName(nameof(PVignetteTitle));

    private Border PVignetteChip => (Border)FindName(nameof(PVignetteChip));

    private TextBlock PVignetteKind => (TextBlock)FindName(nameof(PVignetteKind));

    private TextBlock PVignetteTally => (TextBlock)FindName(nameof(PVignetteTally));

    private StackPanel PVignetteDescriptionSection => (StackPanel)FindName(nameof(PVignetteDescriptionSection));

    private StackPanel PVignetteDescription => (StackPanel)FindName(nameof(PVignetteDescription));

    private ItemsControl PVignettePicture => (ItemsControl)FindName(nameof(PVignettePicture));

    private ItemsControl PVignetteVideo => (ItemsControl)FindName(nameof(PVignetteVideo));

    private TextBlock PVignetteUnselected => (TextBlock)FindName(nameof(PVignetteUnselected));

    private Grid PScenario => (Grid)FindName(nameof(PScenario));

    private TextBlock PScenarioHint => (TextBlock)FindName(nameof(PScenarioHint));

    private TextBlock PScenarioGhost => (TextBlock)FindName(nameof(PScenarioGhost));

    private TextBox PScenarioTitle => (TextBox)FindName(nameof(PScenarioTitle));

    private TextBlock PScenarioMeasure => (TextBlock)FindName(nameof(PScenarioMeasure));

    private TextBox PScenarioKind => (TextBox)FindName(nameof(PScenarioKind));

    private TextBlock PScenarioTally => (TextBlock)FindName(nameof(PScenarioTally));

    private TextBox PScenarioDescription => (TextBox)FindName(nameof(PScenarioDescription));

    private ItemsControl PScenarioImage => (ItemsControl)FindName(nameof(PScenarioImage));

    private ItemsControl PScenarioVideo => (ItemsControl)FindName(nameof(PScenarioVideo));

    private Button PScenarioPicture => (Button)FindName(nameof(PScenarioPicture));

    private PIconImage PScenarioPictureIcon => (PIconImage)FindName(nameof(PScenarioPictureIcon));

    private Button PScenarioFilm => (Button)FindName(nameof(PScenarioFilm));

    private PIconImage PScenarioFilmIcon => (PIconImage)FindName(nameof(PScenarioFilmIcon));

    private Button PRepertoireBin => (Button)FindName(nameof(PRepertoireBin));

    private PIconImage PRepertoireBinIcon => (PIconImage)FindName(nameof(PRepertoireBinIcon));

    internal void PRepertoireAttach(PWindow host)
    {
        _pRepertoireHost = host;
        LEditor editor = host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm);
        _lRepertoire = host.PWindowDeportment.LWindowRepertoireCreate(
            editor,
            PRepertoireShownCheck,
            PRepertoireDiscardConfirm,
            PRepertoireRemovalConfirm,
            host.PWindowUnreadableConfirm);
        PScenarioDeskAttach();

        PAtlas.ItemsSource = _pAtlasList;
        POccurrence.ItemsSource = _pOccurrenceList;
        PScenarioImage.ItemsSource = _pScenarioImage;
        PScenarioVideo.ItemsSource = _pScenarioVideo;
        PLookItem.PLookItemAttach(PAtlas, PAtlasApply);
        PLookItem.PLookItemAttach(POccurrence, POccurrenceApply);
        PLookItem.PLookItemAttach(PScenarioImage, PImageApply);
        PLookItem.PLookItemAttach(PScenarioVideo, PVideoApply);
        PLookItem.PLookItemAttach(PVignettePicture, PImage.PImageLineApply);
        PLookItem.PLookItemAttach(PVignetteVideo, PVideo.PVideoLineApply);

        PMedia.PMediaAttach(this, host.PWindowDeportment);
        PDisplay.PDisplayAttach(host, editor.LEditorLectern);
        PEditor.PEditorAttach(host, editor);

        LPanel atlas = _lRepertoire.LRepertoireAtlas.LAtlasPanel;
        LPanel occurrence = _lRepertoire.LRepertoireOccurrence.LOccurrencePanel;
        _lRepertoire.LRepertoireChanged += PRepertoireModeUpdate;
        _lRepertoire.LRepertoireScenarioChanged += PScenarioApply;
        _lRepertoire.LRepertoireSituationChanged += PVignetteShow;
        _lRepertoire.LRepertoireFailed += host.PWindowFailureShow;
        _lRepertoire.LRepertoireInquestCleared += PInquestClear;
        atlas.LPanelChanged += PRepertoireModeUpdate;
        atlas.LPanelRowsChanged += PAtlasFind;
        atlas.LPanelCleared += PVignetteClear;
        atlas.LPanelFailed += host.PWindowFailureShow;
        occurrence.LPanelChanged += PRepertoireModeUpdate;
        occurrence.LPanelRowsChanged += POccurrenceFind;
        occurrence.LPanelFailed += host.PWindowFailureShow;
    }

    private bool PRepertoireShownCheck()
    {
        return IsVisible;
    }

    private bool PRepertoireDiscardConfirm(Func<bool, bool> finish)
    {
        return _pRepertoireHost.PWindowDiscardConfirm(true, finish);
    }

    private bool PRepertoireRemovalConfirm(int usage)
    {
        return _pRepertoireHost.PWindowRemovalConfirm(usage);
    }

    private void PRepertoireModeUpdate()
    {
        PScenario.Visibility = PLook.PLookVisibleRead(_lRepertoire.LRepertoireScenarioShown);
        PVignette.Visibility = PLook.PLookVisibleRead(_lRepertoire.LRepertoireVignetteShown);
        PDisplay.Visibility = PLook.PLookVisibleRead(_lRepertoire.LRepertoireDisplayShown);
        PEditor.Visibility = PLook.PLookVisibleRead(_lRepertoire.LRepertoireEditorShown);
        PVignetteBody.Visibility = PLook.PLookVisibleRead(_lRepertoire.LRepertoireVignetteHeld);
        PVignetteUnselected.Visibility = PLook.PLookVisibleRead(_lRepertoire.LRepertoireVignetteBlank);
        PRepertoireViewer.IsChecked = PLook.PLookCheckedRead(_lRepertoire.LRepertoireViewerChecked);
        PRepertoireScribe.IsChecked = PLook.PLookCheckedRead(_lRepertoire.LRepertoireScribeChecked);
        PRepertoireVoyage.Visibility = PLook.PLookVisibleRead(_lRepertoire.LRepertoireViewerChecked);
        PRepertoireChronicle.Visibility = PLook.PLookVisibleRead(_lRepertoire.LRepertoireScribeChecked);
        PRepertoireMode.IsEnabled = _lRepertoire.LRepertoireModeEnabled;
        PRepertoireBin.IsEnabled = _lRepertoire.LRepertoireBinEnabled;
        PRepertoireStore.IsEnabled = _lRepertoire.LRepertoireStoreEnabled;
        PScenario.IsEnabled = _lRepertoire.LRepertoireDesk.LDeskRunning;
        PChronicleUpdate();
    }

    internal void PRepertoireReset()
    {
        _lRepertoire.LRepertoireClear();
    }

    internal bool PRepertoireChangeCheck()
    {
        return _lRepertoire.LRepertoireChangeCheck();
    }

    internal bool PRepertoireDraftFinish(bool store)
    {
        return _lRepertoire.LRepertoireDraftFinish(store);
    }

    internal void PRepertoireClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
        PTierDropdown.IsOpen = false;
        PMeshDropdown.IsOpen = false;
    }

    private void PRepertoirePressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lRepertoire?.LRepertoirePressAllowed ?? false;
    }

    private async void PRepertoirePressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pRepertoireHost.PWindowPressRun(
            (label, ticket) => _lRepertoire.LRepertoirePortraitPrint(
                label, _pRepertoireHost.PWindowLegendRead("Situation"), ticket));
    }

    private void PRepertoirePortraitCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lRepertoire?.LRepertoirePortraitAllowed ?? false;
    }

    private async void PRepertoirePortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pRepertoireHost.PWindowPortraitExport(
            _lRepertoire.LRepertoireOccurrence.LOccurrenceFileRead(), _lRepertoire.LRepertoirePortraitExport);
    }
}
