using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QGuild
{
    private readonly ObservableCollection<QRollItem> _qRollList = [];

    private readonly ObservableCollection<QShelfItem> _qOeuvreList = [];

    private readonly UserControl _qGuildSurface;

    private readonly QVita _qVita;

    private readonly QAutograph _qAutograph;

    private readonly QColophon _qColophon;

    private PWindow _qGuildHost = null!;

    private CGuild _cGuild = null!;

    internal QGuild(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qGuildSurface = surface;
        _qVita = new QVita(QContract.QContractFind<UserControl>(surface, "PVita"));
        _qAutograph = new QAutograph(QContract.QContractFind<UserControl>(surface, "PAutograph"));
        _qColophon = new QColophon(QGuildColophon);

        QGuildPress.Command = ApplicationCommands.Print;

        QChoice.QChoiceDropperAttach(QEchelonDropper, QEchelonDropdown, QEchelon);
        QChoice.QChoiceDropperAttach(QLouverDropper, QLouverDropdown, QLouverDropper);

        QEchelonIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        QLouverIcon.QIconSource = QIcon.QIconResolve("filter", 24);
        QGuildBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        QMuster.SetResourceReference(QField.QFieldHintProperty, "Muster.Search");
        QComb.SetResourceReference(QField.QFieldHintProperty, "Comb.Search");
        QGuildFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        QGuildStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        QGuildEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        QGuildLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        QGuildBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        QGuildForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        QGuildPress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        QGuildViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        QGuildScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        QMuster.TextChanged += QMusterHandle;
        QComb.TextChanged += QCombHandle;
        QGuildFresh.Click += QGuildFreshHandle;
        QGuildStore.Click += QGuildStoreHandle;
        QGuildEarlier.Click += QGuildRetreatHandle;
        QGuildLater.Click += QGuildAdvanceHandle;
        QGuildBackward.Click += QGuildUndoHandle;
        QGuildForward.Click += QGuildRedoHandle;
        QGuildViewer.Click += QGuildScribeHandle;
        QGuildScribe.Click += QGuildScribeHandle;
        QGuildBin.Click += QGuildBinHandle;

        QRoll.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QRollHandle));
        QOeuvre.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QOeuvreHandle));

        QLookItem.QLookItemAttach(QRoll, QRollItem.QRollItemApply);
        QLookItem.QLookItemAttach(QOeuvre, QShelfItem.QShelfItemApply);
    }

    private Border QEchelon => QContract.QContractFind<Border>(_qGuildSurface, "PEchelon");

    private ToggleButton QEchelonDropper => QContract.QContractFind<ToggleButton>(_qGuildSurface, "PEchelonDropper");

    private QIconImage QEchelonIcon => QContract.QContractFind<QIconImage>(_qGuildSurface, "PEchelonIcon");

    private TextBox QMuster => QContract.QContractFind<TextBox>(_qGuildSurface, "PMuster");

    private Popup QEchelonDropdown => QContract.QContractFind<Popup>(_qGuildSurface, "PEchelonDropdown");

    private StackPanel QEchelonList => QContract.QContractFind<StackPanel>(_qGuildSurface, "PEchelonList");

    private TextBox QComb => QContract.QContractFind<TextBox>(_qGuildSurface, "PComb");

    private ToggleButton QLouverDropper => QContract.QContractFind<ToggleButton>(_qGuildSurface, "PLouverDropper");

    private QIconImage QLouverIcon => QContract.QContractFind<QIconImage>(_qGuildSurface, "PLouverIcon");

    private FrameworkElement QLouverMark => QContract.QContractFind<FrameworkElement>(_qGuildSurface, "PLouverMark");

    private Popup QLouverDropdown => QContract.QContractFind<Popup>(_qGuildSurface, "PLouverDropdown");

    private StackPanel QLouverList => QContract.QContractFind<StackPanel>(_qGuildSurface, "PLouverList");

    private Button QGuildFresh => QContract.QContractFind<Button>(_qGuildSurface, "PGuildFresh");

    private Button QGuildStore => QContract.QContractFind<Button>(_qGuildSurface, "PGuildStore");

    private StackPanel QGuildVoyage => QContract.QContractFind<StackPanel>(_qGuildSurface, "PGuildVoyage");

    private Button QGuildEarlier => QContract.QContractFind<Button>(_qGuildSurface, "PGuildEarlier");

    private Button QGuildLater => QContract.QContractFind<Button>(_qGuildSurface, "PGuildLater");

    private StackPanel QGuildChronicle => QContract.QContractFind<StackPanel>(_qGuildSurface, "PGuildChronicle");

    private Button QGuildBackward => QContract.QContractFind<Button>(_qGuildSurface, "PGuildBackward");

    private Button QGuildForward => QContract.QContractFind<Button>(_qGuildSurface, "PGuildForward");

    private Button QGuildPress => QContract.QContractFind<Button>(_qGuildSurface, "PGuildPress");

    private Border QGuildMode => QContract.QContractFind<Border>(_qGuildSurface, "PGuildMode");

    private RadioButton QGuildViewer => QContract.QContractFind<RadioButton>(_qGuildSurface, "PGuildViewer");

    private RadioButton QGuildScribe => QContract.QContractFind<RadioButton>(_qGuildSurface, "PGuildScribe");

    private ItemsControl QRoll => QContract.QContractFind<ItemsControl>(_qGuildSurface, "PRoll");

    private TextBlock QRollEmpty => QContract.QContractFind<TextBlock>(_qGuildSurface, "PRollEmpty");

    private ItemsControl QOeuvre => QContract.QContractFind<ItemsControl>(_qGuildSurface, "POeuvre");

    private TextBlock QOeuvreEmpty => QContract.QContractFind<TextBlock>(_qGuildSurface, "POeuvreEmpty");

    private UserControl QGuildColophon => QContract.QContractFind<UserControl>(_qGuildSurface, "PColophon");

    private Button QGuildBin => QContract.QContractFind<Button>(_qGuildSurface, "PGuildBin");

    private QIconImage QGuildBinIcon => QContract.QContractFind<QIconImage>(_qGuildSurface, "PGuildBinIcon");

    internal void QGuildAttach(PWindow host)
    {
        _qGuildHost = host;
        _cGuild = host.PWindowForge.QForgeGuildCreate(QGuildShownCheck, host.PWindowEnvoy);
        _cGuild.CGuildChanged += QGuildModeUpdate;
        _cGuild.CGuildPanel.CPanelChanged += QGuildModeUpdate;
        _cGuild.CGuildPanel.CPanelRowsChanged += QRollUpdate;
        _cGuild.CGuildOeuvre.COeuvrePanel.CPanelChanged += QGuildModeUpdate;
        _cGuild.CGuildOeuvre.COeuvrePanel.CPanelRowsChanged += QOeuvreUpdate;
        _cGuild.CGuildOeuvre.COeuvrePanel.CPanelRowsChanged += QTallyRefine;
        _cGuild.CGuildOeuvre.COeuvrePanel.CPanelCleared += _qColophon.QColophonClear;
        _cGuild.CGuildOeuvre.COeuvreColophonChanged += _qColophon.QColophonShow;
        _qVita.QVitaAttach(host, _cGuild);
        _qAutograph.QAutographAttach(_cGuild);
        _cGuild.CGuildAutograph.CDeskFailed += host.PWindowFailureShow;
        _cGuild.CGuildAutograph.CDeskStateChanged += QGuildChronicleUpdate;

        QRoll.ItemsSource = _qRollList;
        QOeuvre.ItemsSource = _qOeuvreList;

        _qGuildSurface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QGuildPressHandle, QGuildPressCheck));
    }

    internal void QGuildVistaRestore()
    {
        _cGuild.CGuildPanel.CPanelObserverAttach(
            CSubject.CSubjectVista,
            LObserver.LObserverCreate<CBulletin>(_qGuildSurface, _cGuild.CGuildPanel.CPanelRowsResonate));
        _cGuild.CGuildOeuvre.COeuvrePanel.CPanelObserverAttach(
            CSubject.CSubjectVista,
            LObserver.LObserverCreate<CBulletin>(_qGuildSurface, _cGuild.CGuildOeuvre.COeuvrePanel.CPanelRowsResonate));
        _cGuild.CGuildPanel.CPanelObserverAttach(
            CSubject.CSubjectWorkspace,
            LObserver.LObserverCreate<CBulletin>(_qGuildSurface, _cGuild.CGuildAuthorClose));
        _cGuild.CGuildPanel.CPanelObserverAttach(
            CSubject.CSubjectAuthor,
            LObserver.LObserverCreate<CBulletin>(_qGuildSurface, _cGuild.CGuildCatalogResonate));
        _cGuild.CGuildPanel.CPanelObserverAttach(
            CSubject.CSubjectReference,
            LObserver.LObserverCreate<CBulletin>(_qGuildSurface, _cGuild.CGuildCatalogResonate));
        _cGuild.CGuildPanel.CPanelObserverAttach(
            CSubject.CSubjectExample,
            LObserver.LObserverCreate<CBulletin>(_qGuildSurface, _cGuild.CGuildCatalogResonate));
        _cGuild.CGuildPanel.CPanelObserverAttach(
            CSubject.CSubjectEntry,
            LObserver.LObserverCreate<CBulletin>(_qGuildSurface, _cGuild.CGuildCatalogResonate));
        QChoice.QChoiceOrderBuild(
            QEchelonList,
            "Echelon",
            QEchelonHandle,
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderWork,
                CCatalogOrder.CCatalogOrderUsage,
            ]);
        QChoice.QChoiceOrderApply(QEchelonDropdown, _cGuild.CGuildPanel.CPanelOrder);
        QChoice.QChoiceKindBuild(QLouverList, _cGuild.CGuildPanel.CPanelFilter, QLouverHandle);
        QLouverUpdate();
        _cGuild.CGuildQuerySet(QMuster.Text);
        _cGuild.CGuildPanel.CPanelRowsResonate();
    }

    internal bool QGuildChangeCheck()
    {
        return _cGuild.CGuildSession.CSessionChangeCheck();
    }

    internal bool QGuildDraftFinish(bool store)
    {
        return _cGuild.CGuildSession.CSessionFinish(store);
    }

    internal bool QGuildLeaveConfirm()
    {
        return _cGuild.CGuildLeaveConfirm();
    }

    internal void QGuildScribeRestore(bool editing)
    {
        _cGuild.CGuildScribeRestore(editing);
    }

    internal void QGuildClose()
    {
        QEchelonDropdown.IsOpen = false;
        QLouverDropdown.IsOpen = false;
    }

    private bool QGuildShownCheck()
    {
        return _qGuildSurface.IsVisible;
    }

    private void QRollUpdate()
    {
        LSplice.LSpliceApply(
            _qRollList,
            QRollItem.QRollItemBuild(_cGuild.CGuildRollRead()),
            QRollItem.QRollItemMatch,
            QRollItem.QRollItemSync);
        QRollEmpty.Visibility = QLook.QLookVisibleRead(_cGuild.CGuildEmpty);
        QVitaApply(_cGuild.CGuildVitaRead());
    }

    private void QVitaApply(CVita vita)
    {
        _qVita.QVitaShow(vita, _cGuild.CGuildVitaHeld);
        _qAutograph.QAutographTallyShow(vita);
    }

    private void QOeuvreUpdate()
    {
        LSplice.LSpliceApply(
            _qOeuvreList,
            QShelfItem.QShelfItemBuild(_cGuild.CGuildOeuvre.COeuvreRowsRead()),
            QShelfItem.QShelfItemMatch,
            QShelfItem.QShelfItemSync);
        QOeuvreEmpty.SetResourceReference(TextBlock.TextProperty, _cGuild.CGuildOeuvre.COeuvreEmptyKey);
        QOeuvreEmpty.Visibility = QLook.QLookVisibleRead(_cGuild.CGuildOeuvre.COeuvreEmpty);
    }

    private void QTallyRefine()
    {
        _qColophon.QColophonTallyShow(_cGuild.CGuildOeuvre.COeuvreTallyRead());
    }

    private void QGuildModeUpdate()
    {
        QContract.QContractFind<UserControl>(_qGuildSurface, "PAutograph").Visibility =
            QLook.QLookVisibleRead(_cGuild.CGuildAutographShown);
        QContract.QContractFind<UserControl>(_qGuildSurface, "PVita").Visibility =
            QLook.QLookVisibleRead(_cGuild.CGuildVitaShown);
        QGuildColophon.Visibility = QLook.QLookVisibleRead(_cGuild.CGuildColophonShown);
        QGuildViewer.IsChecked = QLook.QLookCheckedRead(_cGuild.CGuildViewerChecked);
        QGuildScribe.IsChecked = QLook.QLookCheckedRead(_cGuild.CGuildScribeChecked);
        QGuildVoyage.Visibility = QLook.QLookVisibleRead(_cGuild.CGuildViewerChecked);
        QGuildChronicle.Visibility = QLook.QLookVisibleRead(_cGuild.CGuildScribeChecked);
        QGuildMode.IsEnabled = _cGuild.CGuildModeEnabled;
        QGuildBin.IsEnabled = _cGuild.CGuildBinEnabled;
        QGuildStore.IsEnabled = _cGuild.CGuildStoreEnabled;
        _qAutograph.QAutographModeUpdate();
    }

    private void QLouverUpdate()
    {
        QLouverMark.Visibility = QLook.QLookVisibleRead(_cGuild.CGuildFiltered);
    }

    private void QMusterHandle(object sender, TextChangedEventArgs e)
    {
        _cGuild.CGuildQuerySet(QMuster.Text);
    }

    private void QCombHandle(object sender, TextChangedEventArgs e)
    {
        _cGuild.CGuildOeuvre.COeuvreQuerySet(QComb.Text);
    }

    private void QEchelonHandle(object sender, RoutedEventArgs e)
    {
        QEchelonDropper.IsChecked = false;
        _cGuild.CGuildOrderSet(QChoice.QChoiceOrderRead(sender));
    }

    private void QLouverHandle(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildFilterSet(QChoice.QChoiceFilterRead(sender));
        QLouverUpdate();
    }

    private void QRollHandle(object sender, RoutedEventArgs e)
    {
        _qGuildHost.PVoyageRecord();
        _cGuild.CGuildAuthorSelect(QSender.QSenderSourceRead<QRollItem>(e)?.QRollItemId);
    }

    internal long QGuildVoyageRead()
    {
        return _cGuild.CGuildPanel.CPanelChosenRead();
    }

    internal void QRollAuthorShow(long id)
    {
        _cGuild.CGuildAuthorOpen(id);
    }

    private void QOeuvreHandle(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildSourceSelect(QSender.QSenderSourceRead<QShelfItem>(e)?.QShelfItemId);
    }

    private void QGuildFreshHandle(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildAuthorCreate();
    }

    private void QGuildScribeHandle(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildScribeToggle(ReferenceEquals(sender, QGuildScribe));
    }

    private void QGuildStoreHandle(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildSession.CSessionSave();
    }

    private void QGuildBinHandle(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildAuthorDelete();
    }

    private void QGuildPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cGuild.CGuildPressAllowed;
    }

    private async void QGuildPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _cGuild.CGuildPortraitPrint();
    }

    internal void QGuildVoyageShow(bool past, bool future)
    {
        QGuildEarlier.IsEnabled = past;
        QGuildLater.IsEnabled = future;
    }

    private void QGuildRetreatHandle(object sender, RoutedEventArgs e)
    {
        _qGuildHost.PVoyageRetreatRun();
    }

    private void QGuildAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _qGuildHost.PVoyageAdvanceRun();
    }

    private void QGuildUndoHandle(object sender, RoutedEventArgs e)
    {
        QChronicle.QChronicleRun(_cGuild.CGuildSession.CSessionUndo);
    }

    private void QGuildRedoHandle(object sender, RoutedEventArgs e)
    {
        QChronicle.QChronicleRun(_cGuild.CGuildSession.CSessionRedo);
    }

    private void QGuildChronicleUpdate()
    {
        (bool undo, bool redo) = _cGuild.CGuildSession.CSessionChronicleRead();
        QGuildBackward.IsEnabled = undo;
        QGuildForward.IsEnabled = redo;
    }
}
