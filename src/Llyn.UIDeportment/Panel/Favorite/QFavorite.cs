using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QFavorite
{
    private readonly UserControl _qFavoriteSurface;

    private PWindow _qFavoriteHost = null!;

    private CFavorite _cFavorite = null!;

    internal QFavorite(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qFavoriteSurface = surface;

        surface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QFavoritePressObserve, QFavoritePressCheck));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QFavoritePortraitObserve, QFavoritePressCheck));
        QFavoritePortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        QFavoritePress.Command = ApplicationCommands.Print;

        QChoice.QChoiceDropperAttach(QSeriesDropper, QSeriesDropdown, QSeries);
        QChoice.QChoiceDropperAttach(QStrainerDropper, QStrainerDropdown, QStrainerDropper);

        QSeriesIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        QStrainerIcon.QIconSource = QIcon.QIconResolve("filter", 24);
        QFavoriteBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        QRecall.SetResourceReference(QField.QFieldHintProperty, "Favorite.Search");
        QFavoriteStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        QFavoriteEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        QFavoriteLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        QFavoriteBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        QFavoriteForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        QFavoritePortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        QFavoritePress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        QFavoriteViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        QFavoriteScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        QRecall.TextChanged += QRecallObserve;
        QFavoriteStore.Click += QFavoriteStoreObserve;
        QFavoriteEarlier.Click += QFavoriteRetreatObserve;
        QFavoriteLater.Click += QFavoriteAdvanceObserve;
        QFavoriteBackward.Click += QFavoriteUndoObserve;
        QFavoriteForward.Click += QFavoriteRedoObserve;
        QFavoriteViewer.Click += QFavoriteViewerObserve;
        QFavoriteScribe.Click += QFavoriteScribeObserve;
        QFavoriteBin.Click += QFavoriteBinObserve;
    }

    private Border QSeries => QContract.QContractFind<Border>(_qFavoriteSurface, "PSeries");

    private ToggleButton QSeriesDropper => QContract.QContractFind<ToggleButton>(_qFavoriteSurface, "PSeriesDropper");

    private QIconImage QSeriesIcon => QContract.QContractFind<QIconImage>(_qFavoriteSurface, "PSeriesIcon");

    private Popup QSeriesDropdown => QContract.QContractFind<Popup>(_qFavoriteSurface, "PSeriesDropdown");

    private StackPanel QSeriesList => QContract.QContractFind<StackPanel>(_qFavoriteSurface, "PSeriesList");

    private TextBox QRecall => QContract.QContractFind<TextBox>(_qFavoriteSurface, "PRecall");

    private ToggleButton QStrainerDropper =>
        QContract.QContractFind<ToggleButton>(_qFavoriteSurface, "PStrainerDropper");

    private QIconImage QStrainerIcon => QContract.QContractFind<QIconImage>(_qFavoriteSurface, "PStrainerIcon");

    private FrameworkElement QStrainerMark =>
        QContract.QContractFind<FrameworkElement>(_qFavoriteSurface, "PStrainerMark");

    private Popup QStrainerDropdown => QContract.QContractFind<Popup>(_qFavoriteSurface, "PStrainerDropdown");

    private StackPanel QStrainerList => QContract.QContractFind<StackPanel>(_qFavoriteSurface, "PStrainerList");

    private ItemsControl QRoster => QContract.QContractFind<ItemsControl>(_qFavoriteSurface, "PRoster");

    private TextBlock QRosterEmpty => QContract.QContractFind<TextBlock>(_qFavoriteSurface, "PRosterEmpty");

    private Button QFavoriteStore => QContract.QContractFind<Button>(_qFavoriteSurface, "PFavoriteStore");

    private StackPanel QFavoriteVoyage => QContract.QContractFind<StackPanel>(_qFavoriteSurface, "PFavoriteVoyage");

    private Button QFavoriteEarlier => QContract.QContractFind<Button>(_qFavoriteSurface, "PFavoriteEarlier");

    private Button QFavoriteLater => QContract.QContractFind<Button>(_qFavoriteSurface, "PFavoriteLater");

    private StackPanel QFavoriteChronicle =>
        QContract.QContractFind<StackPanel>(_qFavoriteSurface, "PFavoriteChronicle");

    private Button QFavoriteBackward => QContract.QContractFind<Button>(_qFavoriteSurface, "PFavoriteBackward");

    private Button QFavoriteForward => QContract.QContractFind<Button>(_qFavoriteSurface, "PFavoriteForward");

    private Button QFavoritePortrait => QContract.QContractFind<Button>(_qFavoriteSurface, "PFavoritePortrait");

    private Button QFavoritePress => QContract.QContractFind<Button>(_qFavoriteSurface, "PFavoritePress");

    private Border QFavoriteMode => QContract.QContractFind<Border>(_qFavoriteSurface, "PFavoriteMode");

    private RadioButton QFavoriteViewer => QContract.QContractFind<RadioButton>(_qFavoriteSurface, "PFavoriteViewer");

    private RadioButton QFavoriteScribe => QContract.QContractFind<RadioButton>(_qFavoriteSurface, "PFavoriteScribe");

    private PDisplay QFavoriteDisplay => QContract.QContractFind<PDisplay>(_qFavoriteSurface, "PDisplay");

    private PEditor QFavoriteEditor => QContract.QContractFind<PEditor>(_qFavoriteSurface, "PEditor");

    private Button QFavoriteBin => QContract.QContractFind<Button>(_qFavoriteSurface, "PFavoriteBin");

    private QIconImage QFavoriteBinIcon => QContract.QContractFind<QIconImage>(_qFavoriteSurface, "PFavoriteBinIcon");

    internal void QFavoriteIntroduce(PWindow host)
    {
        _qFavoriteHost = host;
        _cFavorite = host.PWindowForge.QForgeFavoriteCreate(QFavoriteShownCheck, host.PWindowEnvoy);
        CPanel panel = _cFavorite.CFavoritePanel;
        QLectern lectern = new(_cFavorite.CFavoriteEditor.CEditorDisplay, panel);
        QChoice.QChoiceOrderBuild(QSeriesList, "Series", QSeriesObserve, CFavorite.CFavoriteOrderRead());
        panel.CPanelChanged += QFavoriteModeUpdate;
        panel.CPanelRowsChanged += QRosterRefine;
        _cFavorite.CFavoriteWorkspaceChanged += QFavoriteWorkspaceRefine;

        QRoster.ItemsSource = _qRosterList;
        QLookItem.QLookItemAttach(QRoster, QRosterApply);

        QFavoriteDisplay.PDisplayAttach(host, lectern);
        _cFavorite.CFavoriteEditor.CEditorDesk.CDeskStateChanged += QFavoriteStoreUpdate;
        QFavoriteEditor.PEditorIntroduce(host, new QEditor(_cFavorite.CFavoriteEditor));
        QFavoriteEditor.PEditorChronicleChanged += QFavoriteChronicleUpdate;
    }

    private void QFavoriteStoreUpdate()
    {
        QFavoriteStore.IsEnabled = _cFavorite.CFavoriteEditor.CEditorDesk.CDeskStorable;
    }

    internal void QFavoriteExitRefine()
    {
        QFavoriteEditor.PEditorPlayerRefine();
    }

    private void QFavoritePressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cFavorite?.CFavoritePanel.CPanelPressAllowed ?? false;
    }

    private async void QFavoritePressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cFavorite.CFavoritePortraitPrint();
    }

    private async void QFavoritePortraitObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cFavorite.CFavoritePortraitExport();
    }

    internal void QFavoriteVoyageShow(bool past, bool future)
    {
        QFavoriteEarlier.IsEnabled = past;
        QFavoriteLater.IsEnabled = future;
    }

    private void QFavoriteRetreatObserve(object sender, RoutedEventArgs e)
    {
        _qFavoriteHost.PWindowAtelier.CAtelierNavigation.CNavigationStationUndo();
    }

    private void QFavoriteAdvanceObserve(object sender, RoutedEventArgs e)
    {
        _qFavoriteHost.PWindowAtelier.CAtelierNavigation.CNavigationStationRedo();
    }

    private void QFavoriteUndoObserve(object sender, RoutedEventArgs e)
    {
        QFavoriteEditor.QChronicleUndoObserve();
    }

    private void QFavoriteRedoObserve(object sender, RoutedEventArgs e)
    {
        QFavoriteEditor.QChronicleRedoObserve();
    }

    private void QFavoriteChronicleUpdate()
    {
        (bool undo, bool redo) = _cFavorite.CFavoriteEditor.CEditorDesk.CDeskChronicleRead();
        QFavoriteBackward.IsEnabled = undo;
        QFavoriteForward.IsEnabled = redo;
    }

    private bool QFavoriteShownCheck()
    {
        return _qFavoriteSurface.IsVisible;
    }

    private void QFavoriteModeUpdate()
    {
        CPanel panel = _cFavorite.CFavoritePanel;
        QFavoriteEditor.Visibility = QLook.QLookVisibleRead(panel.CPanelEditing);
        QFavoriteDisplay.Visibility = QLook.QLookVisibleRead(panel.CPanelViewerChecked);
        QFavoriteViewer.IsChecked = panel.CPanelViewerChecked;
        QFavoriteScribe.IsChecked = panel.CPanelScribeChecked;
        QFavoriteVoyage.Visibility = QLook.QLookVisibleRead(panel.CPanelViewerChecked);
        QFavoriteChronicle.Visibility = QLook.QLookVisibleRead(panel.CPanelScribeChecked);
        QFavoriteMode.IsEnabled = panel.CPanelModeEnabled;
        QFavoriteBin.IsEnabled = panel.CPanelBinEnabled;
    }
}
