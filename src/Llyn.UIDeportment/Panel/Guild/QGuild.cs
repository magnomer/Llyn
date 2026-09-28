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

    private LGuild _lGuild = null!;

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
        _lGuild = host.PWindowForge.QForgeGuildCreate(
            QGuildShownCheck,
            QGuildDiscardConfirm,
            host.PWindowUnionConfirm,
            host.PWindowEnvoy);
        _lGuild.LGuildChanged += QGuildModeUpdate;
        _lGuild.LGuildRefused += host.PWindowEnvoy.CEnvoyFailureShow;
        _lGuild.LGuildFailed += host.PWindowFailureShow;
        _lGuild.LGuildPanel.CPanelChanged += QGuildModeUpdate;
        _lGuild.LGuildPanel.CPanelRowsChanged += QRollUpdate;
        _lGuild.LGuildOeuvre.COeuvrePanel.CPanelChanged += QGuildModeUpdate;
        _lGuild.LGuildOeuvre.COeuvrePanel.CPanelRowsChanged += QOeuvreUpdate;
        _lGuild.LGuildOeuvre.COeuvrePanel.CPanelRowsChanged += QTallyRefine;
        _lGuild.LGuildOeuvre.COeuvrePanel.CPanelCleared += _qColophon.QColophonClear;
        _lGuild.LGuildOeuvre.COeuvreColophonChanged += _qColophon.QColophonShow;
        _qVita.QVitaAttach(host, _lGuild);
        _qAutograph.QAutographAttach(_lGuild);
        _lGuild.LGuildAutograph.CDeskFailed += host.PWindowFailureShow;
        _lGuild.LGuildAutograph.CDeskStateChanged += QGuildChronicleUpdate;

        QRoll.ItemsSource = _qRollList;
        QOeuvre.ItemsSource = _qOeuvreList;

        _qGuildSurface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QGuildPressHandle, QGuildPressCheck));
    }

    internal void QGuildVistaRestore()
    {
        _lGuild.LGuildPanel.CPanelObserverAttach(
            CSubject.CSubjectVista,
            LObserver.LObserverCreate<CBulletin>(_qGuildSurface, _lGuild.LGuildPanel.CPanelRowsUpdate));
        _lGuild.LGuildOeuvre.COeuvrePanel.CPanelObserverAttach(
            CSubject.CSubjectVista,
            LObserver.LObserverCreate<CBulletin>(_qGuildSurface, _lGuild.LGuildOeuvre.COeuvrePanel.CPanelRowsUpdate));
        _lGuild.LGuildPanel.CPanelObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(_qGuildSurface, _lGuild.LGuildReset));
        _lGuild.LGuildPanel.CPanelObserverAttach(
            CSubject.CSubjectAuthor,
            LObserver.LObserverCreate<CBulletin>(_qGuildSurface, _lGuild.LGuildCatalogUpdate));
        _lGuild.LGuildPanel.CPanelObserverAttach(
            CSubject.CSubjectReference,
            LObserver.LObserverCreate<CBulletin>(_qGuildSurface, _lGuild.LGuildCatalogUpdate));
        _lGuild.LGuildPanel.CPanelObserverAttach(
            CSubject.CSubjectExample,
            LObserver.LObserverCreate<CBulletin>(_qGuildSurface, _lGuild.LGuildCatalogUpdate));
        _lGuild.LGuildPanel.CPanelObserverAttach(
            CSubject.CSubjectEntry,
            LObserver.LObserverCreate<CBulletin>(_qGuildSurface, _lGuild.LGuildCatalogUpdate));
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
        QChoice.QChoiceOrderApply(QEchelonDropdown, _lGuild.LGuildPanel.CPanelOrder);
        QChoice.QChoiceKindBuild(QLouverList, _lGuild.LGuildLouverRead(), QLouverHandle);
        QLouverUpdate();
        _lGuild.LGuildQuerySet(QMuster.Text);
        _lGuild.LGuildPanel.CPanelRowsUpdate();
    }

    internal bool QGuildChangeCheck()
    {
        return _lGuild.LGuildSession.CSessionChangeCheck();
    }

    internal bool QGuildDraftFinish(bool store)
    {
        return _lGuild.LGuildSession.CSessionFinish(store);
    }

    internal bool QGuildLeaveConfirm()
    {
        return _lGuild.LGuildLeaveConfirm();
    }

    internal void QGuildScribeRestore(bool editing)
    {
        _lGuild.LGuildScribeRestore(editing);
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

    private bool QGuildDiscardConfirm()
    {
        return _qGuildHost.PWindowDiscardConfirm(true, QGuildDraftFinish);
    }

    private void QRollUpdate()
    {
        LSplice.LSpliceApply(
            _qRollList,
            QRollItem.QRollItemBuild(_lGuild.LGuildRollRead()),
            QRollItem.QRollItemMatch,
            QRollItem.QRollItemSync);
        QRollEmpty.Visibility = QLook.QLookVisibleRead(_lGuild.LGuildEmpty);
        QVitaApply(_lGuild.LGuildVitaRead());
    }

    private void QVitaApply(CVita vita)
    {
        _qVita.QVitaShow(vita, _lGuild.LGuildVitaHeld);
        _qAutograph.QAutographTallyShow(vita);
    }

    private void QOeuvreUpdate()
    {
        LSplice.LSpliceApply(
            _qOeuvreList,
            QShelfItem.QShelfItemBuild(_lGuild.LGuildOeuvre.COeuvreRowsRead()),
            QShelfItem.QShelfItemMatch,
            QShelfItem.QShelfItemSync);
        QOeuvreEmpty.SetResourceReference(TextBlock.TextProperty, _lGuild.LGuildOeuvre.COeuvreEmptyKey);
        QOeuvreEmpty.Visibility = QLook.QLookVisibleRead(_lGuild.LGuildOeuvre.COeuvreEmpty);
    }

    private void QTallyRefine()
    {
        _qColophon.QColophonTallyShow(_lGuild.LGuildOeuvre.COeuvreTallyRead());
    }

    private void QGuildModeUpdate()
    {
        QContract.QContractFind<UserControl>(_qGuildSurface, "PAutograph").Visibility =
            QLook.QLookVisibleRead(_lGuild.LGuildAutographShown);
        QContract.QContractFind<UserControl>(_qGuildSurface, "PVita").Visibility =
            QLook.QLookVisibleRead(_lGuild.LGuildVitaShown);
        QGuildColophon.Visibility = QLook.QLookVisibleRead(_lGuild.LGuildColophonShown);
        QGuildViewer.IsChecked = QLook.QLookCheckedRead(_lGuild.LGuildViewerChecked);
        QGuildScribe.IsChecked = QLook.QLookCheckedRead(_lGuild.LGuildScribeChecked);
        QGuildVoyage.Visibility = QLook.QLookVisibleRead(_lGuild.LGuildViewerChecked);
        QGuildChronicle.Visibility = QLook.QLookVisibleRead(_lGuild.LGuildScribeChecked);
        QGuildMode.IsEnabled = _lGuild.LGuildModeEnabled;
        QGuildBin.IsEnabled = _lGuild.LGuildBinEnabled;
        QGuildStore.IsEnabled = _lGuild.LGuildStoreEnabled;
        _qAutograph.QAutographModeUpdate();
    }

    private void QLouverUpdate()
    {
        QLouverMark.Visibility = QLook.QLookVisibleRead(_lGuild.LGuildLouverActive);
    }

    private void QMusterHandle(object sender, TextChangedEventArgs e)
    {
        _lGuild.LGuildQuerySet(QMuster.Text);
    }

    private void QCombHandle(object sender, TextChangedEventArgs e)
    {
        _lGuild.LGuildOeuvre.COeuvreQuerySet(QComb.Text);
    }

    private void QEchelonHandle(object sender, RoutedEventArgs e)
    {
        QEchelonDropper.IsChecked = false;
        _lGuild.LGuildOrderSet(QChoice.QChoiceOrderRead(sender));
    }

    private void QLouverHandle(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildLouverSet(QChoice.QChoiceFilterRead(sender));
        QLouverUpdate();
    }

    private void QRollHandle(object sender, RoutedEventArgs e)
    {
        _qGuildHost.PVoyageRecord();
        _lGuild.LGuildRowSelect(QSender.QSenderSourceRead<QRollItem>(e)?.QRollItemId);
    }

    internal long QGuildVoyageRead()
    {
        return _lGuild.LGuildPanel.CPanelChosenRead();
    }

    internal void QRollAuthorShow(long id)
    {
        _lGuild.LGuildRowShow(id);
    }

    private void QOeuvreHandle(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildSourceSelect(QSender.QSenderSourceRead<QShelfItem>(e)?.QShelfItemId);
    }

    private void QGuildFreshHandle(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildFreshStart();
    }

    private void QGuildScribeHandle(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildScribeSet(ReferenceEquals(sender, QGuildScribe));
    }

    private void QGuildStoreHandle(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildSession.CSessionSave();
    }

    private void QGuildBinHandle(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildDelete();
    }

    private void QGuildPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lGuild.LGuildPressAllowed;
    }

    private async void QGuildPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _qGuildHost.PWindowPressRun("Source", _lGuild.LGuildPortraitPrint);
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
        QChronicle.QChronicleRun(_lGuild.LGuildSession.CSessionUndo);
    }

    private void QGuildRedoHandle(object sender, RoutedEventArgs e)
    {
        QChronicle.QChronicleRun(_lGuild.LGuildSession.CSessionRedo);
    }

    private void QGuildChronicleUpdate()
    {
        (bool undo, bool redo) = _lGuild.LGuildSession.CSessionChronicleRead();
        QGuildBackward.IsEnabled = undo;
        QGuildForward.IsEnabled = redo;
    }
}
