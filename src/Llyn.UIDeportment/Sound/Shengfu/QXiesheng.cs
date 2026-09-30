using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QXiesheng
{
    private readonly ObservableCollection<QGroveItem> _qGroveList = [];

    private readonly ObservableCollection<QKindredItem> _qKindredList = [];

    private readonly UserControl _qXieshengSurface;

    private readonly QStem _qStem;

    private PWindow _qXieshengHost = null!;

    private CXiesheng _cXiesheng = null!;

    internal QXiesheng(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qXieshengSurface = surface;
        _qStem = new QStem(QXieshengStem);

        surface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QXieshengPressObserve, QXieshengPressRefine));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QXieshengPortraitObserve, QXieshengPressRefine));
        QXieshengPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        QXieshengPress.Command = ApplicationCommands.Print;

        QChoice.QChoiceDropperAttach(QRungDropper, QRungDropdown, QRungBar);

        QRungIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        QXieshengBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        QLodestar.SetResourceReference(QField.QFieldHintProperty, "Lodestar.Search");
        QSextant.SetResourceReference(QField.QFieldHintProperty, "Sextant.Search");
        QXieshengFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        QXieshengStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        QXieshengEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        QXieshengLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        QXieshengBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        QXieshengForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        QXieshengPortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        QXieshengPress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        QXieshengViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        QXieshengScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        QLookItem.QLookItemAttach(QGrove, QGroveItem.QGroveItemRefine);
        QLookItem.QLookItemAttach(QKindred, QKindredItem.QKindredItemRefine);
        QGrove.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QGroveObserve));
        QKindred.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QKindredObserve));

        QLodestar.TextChanged += QLodestarObserve;
        QSextant.TextChanged += QSextantObserve;
        QXieshengFresh.Click += QXieshengFreshObserve;
        QXieshengStore.Click += QXieshengStoreObserve;
        QXieshengEarlier.Click += QXieshengRetreatObserve;
        QXieshengLater.Click += QXieshengAdvanceObserve;
        QXieshengBackward.Click += QXieshengUndoObserve;
        QXieshengForward.Click += QXieshengRedoObserve;
        QXieshengViewer.Click += QXieshengViewerObserve;
        QXieshengScribe.Click += QXieshengScribeObserve;
        QXieshengBin.Click += QXieshengBinObserve;
    }

    private Border QRungBar => QContract.QContractFind<Border>(_qXieshengSurface, "PRungBar");

    private ToggleButton QRungDropper => QContract.QContractFind<ToggleButton>(_qXieshengSurface, "PRungDropper");

    private QIconImage QRungIcon => QContract.QContractFind<QIconImage>(_qXieshengSurface, "PRungIcon");

    private TextBox QLodestar => QContract.QContractFind<TextBox>(_qXieshengSurface, "PLodestar");

    private Popup QRungDropdown => QContract.QContractFind<Popup>(_qXieshengSurface, "PRungDropdown");

    private StackPanel QRungList => QContract.QContractFind<StackPanel>(_qXieshengSurface, "PRungList");

    private ItemsControl QGrove => QContract.QContractFind<ItemsControl>(_qXieshengSurface, "PGrove");

    private TextBlock QGroveEmpty => QContract.QContractFind<TextBlock>(_qXieshengSurface, "PGroveEmpty");

    private TextBox QSextant => QContract.QContractFind<TextBox>(_qXieshengSurface, "PSextant");

    private ItemsControl QKindred => QContract.QContractFind<ItemsControl>(_qXieshengSurface, "PKindred");

    private TextBlock QKindredEmpty => QContract.QContractFind<TextBlock>(_qXieshengSurface, "PKindredEmpty");

    private Button QXieshengFresh => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengFresh");

    private Button QXieshengStore => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengStore");

    private StackPanel QXieshengVoyage => QContract.QContractFind<StackPanel>(_qXieshengSurface, "PXieshengVoyage");

    private Button QXieshengEarlier => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengEarlier");

    private Button QXieshengLater => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengLater");

    private StackPanel QXieshengChronicle =>
        QContract.QContractFind<StackPanel>(_qXieshengSurface, "PXieshengChronicle");

    private Button QXieshengBackward => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengBackward");

    private Button QXieshengForward => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengForward");

    private Button QXieshengPortrait => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengPortrait");

    private Button QXieshengPress => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengPress");

    private Border QXieshengMode => QContract.QContractFind<Border>(_qXieshengSurface, "PXieshengMode");

    private RadioButton QXieshengViewer => QContract.QContractFind<RadioButton>(_qXieshengSurface, "PXieshengViewer");

    private RadioButton QXieshengScribe => QContract.QContractFind<RadioButton>(_qXieshengSurface, "PXieshengScribe");

    private PDisplay QXieshengDisplay => QContract.QContractFind<PDisplay>(_qXieshengSurface, "PDisplay");

    private UserControl QXieshengStem => QContract.QContractFind<UserControl>(_qXieshengSurface, "PXieshengStem");

    private PEditor QXieshengEditor => QContract.QContractFind<PEditor>(_qXieshengSurface, "PEditor");

    private Button QXieshengBin => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengBin");

    private QIconImage QXieshengBinIcon => QContract.QContractFind<QIconImage>(_qXieshengSurface, "PXieshengBinIcon");

    internal void QXieshengIntroduce(PWindow host)
    {
        _qXieshengHost = host;
        _cXiesheng = host.PWindowForge.QForgeXieshengCreate(QXieshengShownCheck, host.PWindowEnvoy);
        CPanel panel = _cXiesheng.CXieshengPanel;
        QLectern lectern = new(_cXiesheng.CXieshengEditor.CEditorDisplay, panel);
        QChoice.QChoiceOrderBuild(QRungList, "Rung", QRungObserve, CXiesheng.CXieshengOrderRead());
        _cXiesheng.CXieshengStemOpened += QLodestarRefine;
        _cXiesheng.CXieshengChanged += QGroveRefine;
        _cXiesheng.CXieshengChanged += QXieshengStemRefine;
        _cXiesheng.CXieshengChanged += QXieshengModeRefine;
        _cXiesheng.CXieshengWorkspaceChanged += QXieshengWorkspaceRefine;
        panel.CPanelChanged += QXieshengModeRefine;
        panel.CPanelRowsChanged += QKindredRefine;
        panel.CPanelCleared += QXieshengStemRefine;
        _cXiesheng.CXieshengEditor.CEditorDesk.CDeskStateChanged += QXieshengStoreRefine;

        QGrove.ItemsSource = _qGroveList;
        QKindred.ItemsSource = _qKindredList;

        QXieshengDisplay.PDisplayAttach(host, lectern);
        _qStem.QStemIntroduce(_cXiesheng);

        QXieshengEditor.PEditorIntroduce(host, new QEditor(_cXiesheng.CXieshengEditor));

        QXieshengEditor.PEditorChronicleChanged += QXieshengChronicleRefine;
    }

    private void QXieshengStoreRefine()
    {
        QXieshengStore.IsEnabled = _cXiesheng.CXieshengEditor.CEditorDesk.CDeskStorable;
    }

    internal void QXieshengVistaRefine()
    {
        QChoice.QChoiceOrderApply(QRungDropdown, _cXiesheng.CXieshengOrder);
        QXieshengModeRefine();
        QKindredRefine();
    }

    private async void QXieshengWorkspaceRefine()
    {
        await LEnsignImage.LEnsignLoad(_qXieshengHost.PWindowAtelier.CAtelierCatalog.CCatalogEnsignLoad);
        QKindredRefine();
    }

    private void QLodestarRefine()
    {
        QLodestar.Text = string.Empty;
    }

    internal void QXieshengExitRefine()
    {
        QXieshengEditor.PEditorPlayerRefine();
    }

    private bool QXieshengShownCheck()
    {
        return _qXieshengSurface.IsVisible;
    }

    internal void QGroveRefine()
    {
        LSplice.LSpliceApply(
            _qGroveList,
            QGroveItem.QGroveItemBuild(_cXiesheng.CXieshengGroveRead()),
            QGroveItem.QGroveItemMatch,
            QGroveItem.QGroveItemSync);
        QGroveEmpty.SetResourceReference(TextBlock.TextProperty, _cXiesheng.CXieshengGroveKey);
        QGroveEmpty.Visibility = QLook.QLookVisibleRead(_cXiesheng.CXieshengGroveEmpty);
    }

    private void QKindredRefine()
    {
        LSplice.LSpliceApply(
            _qKindredList,
            QKindredItem.QKindredItemBuild(_cXiesheng.CXieshengKindredRead()),
            QKindredItem.QKindredItemMatch,
            QKindredItem.QKindredItemSync);
        QKindredEmpty.SetResourceReference(TextBlock.TextProperty, _cXiesheng.CXieshengKindredKey);
        QKindredEmpty.Visibility = QLook.QLookVisibleRead(_cXiesheng.CXieshengKindredEmpty);
    }

    internal void QXieshengStemRefine()
    {
        _qStem.QStemRefine(_cXiesheng.CXieshengStemRead());
    }

    private void QXieshengModeRefine()
    {
        QXieshengEditor.Visibility = QLook.QLookVisibleRead(_cXiesheng.CXieshengEditorShown);
        QXieshengDisplay.Visibility = QLook.QLookVisibleRead(_cXiesheng.CXieshengDisplayShown);
        QXieshengStem.Visibility = QLook.QLookVisibleRead(_cXiesheng.CXieshengStemShown);
        QXieshengViewer.IsChecked = _cXiesheng.CXieshengPanel.CPanelViewerChecked;
        QXieshengScribe.IsChecked = _cXiesheng.CXieshengPanel.CPanelScribeChecked;
        QXieshengVoyage.Visibility = QLook.QLookVisibleRead(_cXiesheng.CXieshengPanel.CPanelViewerChecked);
        QXieshengChronicle.Visibility = QLook.QLookVisibleRead(_cXiesheng.CXieshengPanel.CPanelScribeChecked);
        QXieshengMode.IsEnabled = _cXiesheng.CXieshengPanel.CPanelModeEnabled;
        QXieshengBin.IsEnabled = _cXiesheng.CXieshengPanel.CPanelBinEnabled;
    }

    private void QLodestarObserve(object sender, TextChangedEventArgs e)
    {
        _cXiesheng.CXieshengGroveFind(QLodestar.Text);
    }

    private void QSextantObserve(object sender, TextChangedEventArgs e)
    {
        _cXiesheng.CXieshengKindredFind(QSextant.Text);
    }

    private void QRungObserve(object sender, RoutedEventArgs e)
    {
        _cXiesheng.CXieshengGroveSet(QChoice.QChoiceOrderRead(sender));
        QRungRefine();
    }

    private void QRungRefine()
    {
        QRungDropper.IsChecked = false;
    }

    private void QGroveObserve(object sender, RoutedEventArgs e)
    {
        _cXiesheng.CXieshengStemSelect(QSender.QSenderSourceRead<QGroveItem>(e)?.QGroveItemId);
    }

    private void QKindredObserve(object sender, RoutedEventArgs e)
    {
        _cXiesheng.CXieshengPanel.CPanelRowSelect(QSender.QSenderSourceRead<QKindredItem>(e)?.QKindredItemId);
    }

    private void QXieshengFreshObserve(object sender, RoutedEventArgs e)
    {
        _cXiesheng.CXieshengPanel.CPanelEntryCreate();
    }

    private void QXieshengViewerObserve(object sender, RoutedEventArgs e)
    {
        _cXiesheng.CXieshengPanel.CPanelScribeToggle(false);
    }

    private void QXieshengScribeObserve(object sender, RoutedEventArgs e)
    {
        _cXiesheng.CXieshengPanel.CPanelScribeToggle(true);
    }

    private void QXieshengStoreObserve(object sender, RoutedEventArgs e)
    {
        _cXiesheng.CXieshengEditor.CEditorEntrySave();
    }

    private void QXieshengBinObserve(object sender, RoutedEventArgs e)
    {
        _cXiesheng.CXieshengPanel.CPanelEntryDelete();
    }

    private void QXieshengPressRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cXiesheng?.CXieshengPanel.CPanelPressAllowed ?? false;
    }

    private async void QXieshengPressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cXiesheng.CXieshengPortraitPrint();
    }

    private async void QXieshengPortraitObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cXiesheng.CXieshengPortraitExport();
    }

    internal void QXieshengVoyageRefine(bool past, bool future)
    {
        QXieshengEarlier.IsEnabled = past;
        QXieshengLater.IsEnabled = future;
    }

    private void QXieshengRetreatObserve(object sender, RoutedEventArgs e)
    {
        _qXieshengHost.PWindowAtelier.CAtelierNavigation.CNavigationStationUndo();
    }

    private void QXieshengAdvanceObserve(object sender, RoutedEventArgs e)
    {
        _qXieshengHost.PWindowAtelier.CAtelierNavigation.CNavigationStationRedo();
    }

    private void QXieshengUndoObserve(object sender, RoutedEventArgs e)
    {
        QXieshengEditor.QChronicleUndoObserve();
    }

    private void QXieshengRedoObserve(object sender, RoutedEventArgs e)
    {
        QXieshengEditor.QChronicleRedoObserve();
    }

    private void QXieshengChronicleRefine()
    {
        (bool undo, bool redo) = _cXiesheng.CXieshengEditor.CEditorDesk.CDeskChronicleRead();
        QXieshengBackward.IsEnabled = undo;
        QXieshengForward.IsEnabled = redo;
    }
}
