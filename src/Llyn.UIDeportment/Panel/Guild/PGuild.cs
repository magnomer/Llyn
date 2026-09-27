using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public class PGuild : UserControl
{
    private readonly ObservableCollection<PRollItem> _pRollList = [];

    private readonly ObservableCollection<PShelfItem> _pOeuvreList = [];

    private PWindow _pGuildHost = null!;

    private LGuild _lGuild = null!;

    public PGuild()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Guild/PGuild.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        PGuildPress.Command = ApplicationCommands.Print;

        QChoice.QChoiceDropperAttach(PEchelonDropper, PEchelonDropdown, PEchelon);
        QChoice.QChoiceDropperAttach(PLouverDropper, PLouverDropdown, PLouverDropper);

        PEchelonIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        PLouverIcon.QIconSource = QIcon.QIconResolve("filter", 24);
        PGuildBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        PMuster.SetResourceReference(QField.QFieldHintProperty, "Muster.Search");
        PComb.SetResourceReference(QField.QFieldHintProperty, "Comb.Search");
        PGuildFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        PGuildStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        PGuildEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        PGuildLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        PGuildBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        PGuildForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        PGuildPress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        PGuildViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        PGuildScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        PMuster.TextChanged += PMusterHandle;
        PComb.TextChanged += PCombHandle;
        PGuildFresh.Click += PGuildFreshHandle;
        PGuildStore.Click += PGuildStoreHandle;
        PGuildEarlier.Click += PGuildRetreatHandle;
        PGuildLater.Click += PGuildAdvanceHandle;
        PGuildBackward.Click += PGuildUndoHandle;
        PGuildForward.Click += PGuildRedoHandle;
        PGuildViewer.Click += PGuildScribeHandle;
        PGuildScribe.Click += PGuildScribeHandle;
        PGuildBin.Click += PGuildBinHandle;

        PRoll.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PRollHandle));
        POeuvre.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(POeuvreHandle));

        QLookItem.QLookItemAttach(PRoll, PRollItem.PRollItemApply);
        QLookItem.QLookItemAttach(POeuvre, PShelfItem.PShelfItemApply);
    }

    private Border PEchelon => (Border)FindName(nameof(PEchelon));

    private ToggleButton PEchelonDropper => (ToggleButton)FindName(nameof(PEchelonDropper));

    private QIconImage PEchelonIcon => (QIconImage)FindName(nameof(PEchelonIcon));

    private TextBox PMuster => (TextBox)FindName(nameof(PMuster));

    private Popup PEchelonDropdown => (Popup)FindName(nameof(PEchelonDropdown));

    private StackPanel PEchelonList => (StackPanel)FindName(nameof(PEchelonList));

    private TextBox PComb => (TextBox)FindName(nameof(PComb));

    private ToggleButton PLouverDropper => (ToggleButton)FindName(nameof(PLouverDropper));

    private QIconImage PLouverIcon => (QIconImage)FindName(nameof(PLouverIcon));

    private FrameworkElement PLouverMark => (FrameworkElement)FindName(nameof(PLouverMark));

    private Popup PLouverDropdown => (Popup)FindName(nameof(PLouverDropdown));

    private StackPanel PLouverList => (StackPanel)FindName(nameof(PLouverList));

    private Button PGuildFresh => (Button)FindName(nameof(PGuildFresh));

    private Button PGuildStore => (Button)FindName(nameof(PGuildStore));

    private StackPanel PGuildVoyage => (StackPanel)FindName(nameof(PGuildVoyage));

    private Button PGuildEarlier => (Button)FindName(nameof(PGuildEarlier));

    private Button PGuildLater => (Button)FindName(nameof(PGuildLater));

    private StackPanel PGuildChronicle => (StackPanel)FindName(nameof(PGuildChronicle));

    private Button PGuildBackward => (Button)FindName(nameof(PGuildBackward));

    private Button PGuildForward => (Button)FindName(nameof(PGuildForward));

    private Button PGuildPress => (Button)FindName(nameof(PGuildPress));

    private Border PGuildMode => (Border)FindName(nameof(PGuildMode));

    private RadioButton PGuildViewer => (RadioButton)FindName(nameof(PGuildViewer));

    private RadioButton PGuildScribe => (RadioButton)FindName(nameof(PGuildScribe));

    private ItemsControl PRoll => (ItemsControl)FindName(nameof(PRoll));

    private TextBlock PRollEmpty => (TextBlock)FindName(nameof(PRollEmpty));

    private ItemsControl POeuvre => (ItemsControl)FindName(nameof(POeuvre));

    private TextBlock POeuvreEmpty => (TextBlock)FindName(nameof(POeuvreEmpty));

    private PColophon PColophon => (PColophon)FindName(nameof(PColophon));

    private PVita PVita => (PVita)FindName(nameof(PVita));

    private PAutograph PAutograph => (PAutograph)FindName(nameof(PAutograph));

    private Button PGuildBin => (Button)FindName(nameof(PGuildBin));

    private QIconImage PGuildBinIcon => (QIconImage)FindName(nameof(PGuildBinIcon));

    internal void PGuildAttach(PWindow host)
    {
        _pGuildHost = host;
        _lGuild = host.PWindowDeportment.LWindowGuildCreate(
            PGuildShownCheck,
            PGuildDiscardConfirm,
            PGuildRemovalConfirm,
            host.PWindowUnionConfirm,
            host.PWindowUnreadableConfirm);
        _lGuild.LGuildChanged += PGuildModeUpdate;
        _lGuild.LGuildRefused += host.PWindowFailureShow;
        _lGuild.LGuildFailed += host.PWindowFailureShow;
        _lGuild.LGuildPanel.LPanelChanged += PGuildModeUpdate;
        _lGuild.LGuildPanel.LPanelRowsChanged += PRollUpdate;
        _lGuild.LGuildPanel.LPanelFailed += host.PWindowFailureShow;
        _lGuild.LGuildOeuvre.LOeuvrePanel.LPanelChanged += PGuildModeUpdate;
        _lGuild.LGuildOeuvre.LOeuvrePanel.LPanelRowsChanged += POeuvreUpdate;
        _lGuild.LGuildOeuvre.LOeuvrePanel.LPanelCleared += PColophon.PColophonClear;
        _lGuild.LGuildOeuvre.LOeuvreColophonChanged += PColophon.PColophonShow;
        _lGuild.LGuildOeuvre.LOeuvrePanel.LPanelFailed += host.PWindowFailureShow;
        PVita.PVitaAttach(host, _lGuild);
        PAutograph.PAutographAttach(_lGuild);
        _lGuild.LGuildAutograph.LDeskFailed += host.PWindowFailureShow;
        _lGuild.LGuildAutograph.LDeskStateChanged += PGuildChronicleUpdate;

        PRoll.ItemsSource = _pRollList;
        POeuvre.ItemsSource = _pOeuvreList;

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PGuildPressHandle, PGuildPressCheck));
    }

    internal void PGuildVistaRestore()
    {
        _lGuild.LGuildPanel.LPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(this, _lGuild.LGuildPanel.LPanelRowsUpdate));
        _lGuild.LGuildOeuvre.LOeuvrePanel.LPanelObserverAttach(
            CSubject.CSubjectVista,
            LObserver.LObserverCreate<CBulletin>(this, _lGuild.LGuildOeuvre.LOeuvrePanel.LPanelRowsUpdate));
        _lGuild.LGuildPanel.LPanelObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(this, _lGuild.LGuildReset));
        _lGuild.LGuildPanel.LPanelObserverAttach(
            CSubject.CSubjectAuthor, LObserver.LObserverCreate<CBulletin>(this, _lGuild.LGuildCatalogUpdate));
        _lGuild.LGuildPanel.LPanelObserverAttach(
            CSubject.CSubjectReference, LObserver.LObserverCreate<CBulletin>(this, _lGuild.LGuildCatalogUpdate));
        _lGuild.LGuildPanel.LPanelObserverAttach(
            CSubject.CSubjectExample, LObserver.LObserverCreate<CBulletin>(this, _lGuild.LGuildCatalogUpdate));
        _lGuild.LGuildPanel.LPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(this, _lGuild.LGuildCatalogUpdate));
        QChoice.QChoiceOrderBuild(
            PEchelonList,
            "Echelon",
            PEchelonHandle,
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderWork,
                CCatalogOrder.CCatalogOrderUsage,
            ]);
        QChoice.QChoiceOrderApply(PEchelonDropdown, _lGuild.LGuildPanel.LPanelOrder);
        QChoice.QChoiceKindBuild(PLouverList, _lGuild.LGuildLouverRead(), PLouverHandle);
        PLouverUpdate();
        _lGuild.LGuildQuerySet(PMuster.Text);
        _lGuild.LGuildOeuvre.LOeuvreQuerySet(PComb.Text);
        _lGuild.LGuildPanel.LPanelRowsUpdate();
    }

    internal bool PGuildChangeCheck()
    {
        return _lGuild.LGuildChangeCheck();
    }

    internal bool PGuildDraftFinish(bool store)
    {
        return _lGuild.LGuildDraftFinish(store);
    }

    internal bool PGuildLeaveConfirm()
    {
        return _lGuild.LGuildLeaveConfirm();
    }

    internal void PGuildScribeRestore(bool editing)
    {
        _lGuild.LGuildScribeRestore(editing);
    }

    internal void PGuildClose()
    {
        PEchelonDropdown.IsOpen = false;
        PLouverDropdown.IsOpen = false;
    }

    private bool PGuildShownCheck()
    {
        return IsVisible;
    }

    private bool PGuildDiscardConfirm()
    {
        return _pGuildHost.PWindowDiscardConfirm(true, PGuildDraftFinish);
    }

    private bool PGuildRemovalConfirm(int works)
    {
        return _pGuildHost.PWindowRemovalConfirm(works, "Guild");
    }

    private void PRollUpdate()
    {
        LSplice.LSpliceApply(
            _pRollList,
            PRollItem.PRollItemBuild(_lGuild.LGuildRollRead()),
            PRollItem.PRollItemMatch,
            PRollItem.PRollItemSync);
        PRollEmpty.Visibility = QLook.QLookVisibleRead(_lGuild.LGuildEmpty);
        PVitaUpdate();
    }

    private void PVitaUpdate()
    {
        LVita vita = _lGuild.LGuildVitaRead();
        PVita.PVitaShow(vita, _lGuild.LGuildVitaHeld);
        PAutograph.PAutographTallyShow(vita);
    }

    private void POeuvreUpdate()
    {
        LSplice.LSpliceApply(
            _pOeuvreList,
            PShelfItem.PShelfItemBuild(_lGuild.LGuildOeuvre.LOeuvreRowsRead()),
            PShelfItem.PShelfItemMatch,
            PShelfItem.PShelfItemSync);
        POeuvreEmpty.SetResourceReference(TextBlock.TextProperty, _lGuild.LGuildOeuvre.LOeuvreEmptyKey);
        POeuvreEmpty.Visibility = QLook.QLookVisibleRead(_lGuild.LGuildOeuvre.LOeuvreEmpty);
        PColophon.PColophonTallyShow(_lGuild.LGuildOeuvre.LOeuvreTallyRead());
    }

    private void PGuildModeUpdate()
    {
        PAutograph.Visibility = QLook.QLookVisibleRead(_lGuild.LGuildAutographShown);
        PVita.Visibility = QLook.QLookVisibleRead(_lGuild.LGuildVitaShown);
        PColophon.Visibility = QLook.QLookVisibleRead(_lGuild.LGuildColophonShown);
        PGuildViewer.IsChecked = QLook.QLookCheckedRead(_lGuild.LGuildViewerChecked);
        PGuildScribe.IsChecked = QLook.QLookCheckedRead(_lGuild.LGuildScribeChecked);
        PGuildVoyage.Visibility = QLook.QLookVisibleRead(_lGuild.LGuildViewerChecked);
        PGuildChronicle.Visibility = QLook.QLookVisibleRead(_lGuild.LGuildScribeChecked);
        PGuildMode.IsEnabled = _lGuild.LGuildModeEnabled;
        PGuildBin.IsEnabled = _lGuild.LGuildBinEnabled;
        PGuildStore.IsEnabled = _lGuild.LGuildStoreEnabled;
        PAutograph.PAutographModeUpdate();
    }

    private void PLouverUpdate()
    {
        PLouverMark.Visibility = QLook.QLookVisibleRead(_lGuild.LGuildLouverActive);
    }

    private void PMusterHandle(object sender, TextChangedEventArgs e)
    {
        _lGuild.LGuildQuerySet(PMuster.Text);
    }

    private void PCombHandle(object sender, TextChangedEventArgs e)
    {
        _lGuild.LGuildOeuvre.LOeuvreQuerySet(PComb.Text);
    }

    private void PEchelonHandle(object sender, RoutedEventArgs e)
    {
        PEchelonDropper.IsChecked = false;
        _lGuild.LGuildOrderSet(QChoice.QChoiceOrderRead(sender));
    }

    private void PLouverHandle(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildLouverSet(QChoice.QChoiceFilterRead(PLouverList));
        PLouverUpdate();
    }

    private void PRollHandle(object sender, RoutedEventArgs e)
    {
        _pGuildHost.PVoyageRecord();
        _lGuild.LGuildRowSelect(QSender.QSenderSourceRead<PRollItem>(e)?.PRollItemId);
    }

    internal long PGuildVoyageRead()
    {
        return _lGuild.LGuildPanel.LPanelVoyageRead();
    }

    internal void PRollAuthorShow(long id)
    {
        _lGuild.LGuildRowShow(id);
    }

    private void POeuvreHandle(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildSourceSelect(QSender.QSenderSourceRead<PShelfItem>(e)?.PShelfItemId);
    }

    private void PGuildFreshHandle(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildFreshStart();
    }

    private void PGuildScribeHandle(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildScribeSet(ReferenceEquals(sender, PGuildScribe));
    }

    private void PGuildStoreHandle(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildSave();
    }

    private void PGuildBinHandle(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildDelete();
    }

    private void PGuildPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lGuild.LGuildPressAllowed;
    }

    private async void PGuildPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pGuildHost.PWindowPressRun(
            ticket => _lGuild.LGuildPortraitPrint(_pGuildHost.PWindowLegendRead("Source"), ticket));
    }

    internal void PGuildVoyageShow(bool past, bool future)
    {
        PGuildEarlier.IsEnabled = past;
        PGuildLater.IsEnabled = future;
    }

    private void PGuildRetreatHandle(object sender, RoutedEventArgs e)
    {
        _pGuildHost.PVoyageRetreatRun();
    }

    private void PGuildAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _pGuildHost.PVoyageAdvanceRun();
    }

    private void PGuildUndoHandle(object sender, RoutedEventArgs e)
    {
        PChronicle.PChronicleRun(_lGuild.LGuildAutograph.LDeskUndo);
    }

    private void PGuildRedoHandle(object sender, RoutedEventArgs e)
    {
        PChronicle.PChronicleRun(_lGuild.LGuildAutograph.LDeskRedo);
    }

    private void PGuildChronicleUpdate()
    {
        (bool undo, bool redo) = _lGuild.LGuildAutograph.LDeskChronicleRead();
        PGuildBackward.IsEnabled = undo;
        PGuildForward.IsEnabled = redo;
    }
}
