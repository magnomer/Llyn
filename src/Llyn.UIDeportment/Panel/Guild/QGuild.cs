using System;
using System.Collections.Generic;
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

    private QWindow _qGuildHost = null!;

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

        QMuster.TextChanged += QMusterObserve;
        QComb.TextChanged += QCombObserve;
        QGuildFresh.Click += QGuildFreshObserve;
        QGuildStore.Click += QGuildStoreObserve;
        QGuildEarlier.Click += QGuildRetreatObserve;
        QGuildLater.Click += QGuildAdvanceObserve;
        QGuildBackward.Click += QGuildUndoObserve;
        QGuildForward.Click += QGuildRedoObserve;
        QGuildViewer.Click += QGuildViewerObserve;
        QGuildScribe.Click += QGuildScribeObserve;
        QGuildBin.Click += QGuildBinObserve;

        QRoll.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QRollObserve));
        QOeuvre.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QOeuvreObserve));

        QLookItem.QLookItemAttach(QRoll, QRollItem.QRollItemApply);
        QLookItem.QLookItemAttach(QOeuvre, QShelfItem.QShelfItemRefine);
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

    internal void QGuildIntroduce(QWindow host)
    {
        _qGuildHost = host;
        _cGuild = CGuild.CGuildCreate(
            host.QWindowAtelier,
            QGuildShownCheck,
            host.QWindowEnvoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        _cGuild.CGuildChanged += QGuildModeUpdate;
        _cGuild.CGuildPanel.CPanelChanged += QGuildModeUpdate;
        _cGuild.CGuildPanel.CPanelRowsChanged += QRollRefine;
        _cGuild.CGuildOeuvre.COeuvrePanel.CPanelChanged += QGuildModeUpdate;
        _cGuild.CGuildOeuvre.COeuvrePanel.CPanelRowsChanged += QOeuvreRefine;
        _cGuild.CGuildOeuvre.COeuvrePanel.CPanelRowsChanged += QTallyRefine;
        _cGuild.CGuildOeuvre.COeuvrePanel.CPanelCleared += _qColophon.QColophonClearRefine;
        _cGuild.CGuildOeuvre.COeuvreColophonChanged += _qColophon.QColophonRefine;
        _qVita.QVitaAttach(host, _cGuild);
        _qAutograph.QAutographIntroduce(_cGuild);
        _cGuild.CGuildAutograph.CDeskStateChanged += QGuildChronicleUpdate;

        QChoice.QChoiceOrderBuild(QEchelonList, "Echelon", QEchelonObserve, CGuild.CGuildOrderRead());

        QRoll.ItemsSource = _qRollList;
        QOeuvre.ItemsSource = _qOeuvreList;

        _qGuildSurface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QGuildPressObserve, QGuildPressCheck));
    }

    internal void QGuildVistaRefine()
    {
        CGuildRoll roll = _cGuild.CGuildRollRead();
        QChoice.QChoiceOrderApply(QEchelonDropdown, _cGuild.CGuildPanel.CPanelOrder);
        QLouverBuild(roll.CGuildRollKind);
        QLouverRefine();
        QRollShow(roll);
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

    private void QRollRefine()
    {
        QRollShow(_cGuild.CGuildRollRead());
    }

    private void QRollShow(CGuildRoll roll)
    {
        QSplice.QSpliceRefine(
            _qRollList,
            QRollItem.QRollItemBuild(roll.CGuildRollRows),
            QRollItem.QRollItemMatch,
            QRollItem.QRollItemSync);
        QRollEmpty.Visibility = QLook.QLookVisibleRead(roll.CGuildRollEmpty);
        _qVita.QVitaShow(roll.CGuildRollVita, _cGuild.CGuildVitaHeld);
        _qAutograph.QAutographTallyShow(roll.CGuildRollVita);
    }

    internal void QOeuvreRefine()
    {
        QSplice.QSpliceRefine(
            _qOeuvreList,
            QShelfItem.QShelfItemBuild(_cGuild.CGuildOeuvre.COeuvreRowsRead()),
            QShelfItem.QShelfItemMatch,
            QShelfItem.QShelfItemSync);
        QOeuvreEmpty.SetResourceReference(TextBlock.TextProperty, _cGuild.CGuildOeuvre.COeuvreEmptyKey);
        QOeuvreEmpty.Visibility = QLook.QLookVisibleRead(_cGuild.CGuildOeuvre.COeuvreEmpty);
    }

    internal void QTallyRefine()
    {
        _qColophon.QColophonTallyRefine(_cGuild.CGuildOeuvre.COeuvreTallyRead());
    }

    private void QGuildModeUpdate()
    {
        QContract.QContractFind<UserControl>(_qGuildSurface, "PAutograph").Visibility =
            QLook.QLookVisibleRead(_cGuild.CGuildAutographShown);
        QContract.QContractFind<UserControl>(_qGuildSurface, "PVita").Visibility =
            QLook.QLookVisibleRead(_cGuild.CGuildVitaShown);
        QGuildColophon.Visibility = QLook.QLookVisibleRead(_cGuild.CGuildColophonShown);
        QGuildViewer.IsChecked = _cGuild.CGuildViewerChecked;
        QGuildScribe.IsChecked = _cGuild.CGuildScribeChecked;
        QGuildVoyage.Visibility = QLook.QLookVisibleRead(_cGuild.CGuildViewerChecked);
        QGuildChronicle.Visibility = QLook.QLookVisibleRead(_cGuild.CGuildScribeChecked);
        QGuildMode.IsEnabled = _cGuild.CGuildModeEnabled;
        QGuildBin.IsEnabled = _cGuild.CGuildBinEnabled;
        QGuildStore.IsEnabled = _cGuild.CGuildStoreEnabled;
        _qAutograph.QAutographModeUpdate();
    }

    private void QLouverBuild(IReadOnlyList<CReferenceKind> kinds)
    {
        QChoice.QChoiceKindRefine(QLouverList, _cGuild.CGuildPanel.CPanelFilter, QLouverObserve, kinds);
    }

    private void QLouverRefine()
    {
        QLouverMark.Visibility = QLook.QLookVisibleRead(_cGuild.CGuildFiltered);
    }

    private void QMusterObserve(object sender, TextChangedEventArgs e)
    {
        _cGuild.CGuildQuerySet(QMuster.Text);
    }

    private void QCombObserve(object sender, TextChangedEventArgs e)
    {
        _cGuild.CGuildOeuvre.COeuvreQuerySet(QComb.Text);
    }

    private void QEchelonObserve(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildOrderSet(QChoice.QChoiceOrderRead(sender));
        QEchelonDropperRefine();
    }

    private void QEchelonDropperRefine()
    {
        QEchelonDropper.IsChecked = false;
    }

    private void QLouverObserve(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildFilterSet(QChoice.QChoiceFilterRead(sender));
        QLouverRefine();
    }

    private void QRollObserve(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildAuthorSelect(QSender.QSenderSourceRead<QRollItem>(e)?.QRollItemId);
    }

    private void QOeuvreObserve(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildSourceSelect(QSender.QSenderSourceRead<QShelfItem>(e)?.QShelfItemId);
    }

    private void QGuildFreshObserve(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildAuthorCreate();
    }

    private void QGuildViewerObserve(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildScribeToggle(false);
    }

    private void QGuildScribeObserve(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildScribeToggle(true);
    }

    private void QGuildStoreObserve(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildSession.CSessionSave();
    }

    private void QGuildBinObserve(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildAuthorDelete();
    }

    private void QGuildPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cGuild.CGuildPressAllowed;
    }

    private async void QGuildPressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cGuild.CGuildPortraitPrint();
    }

    internal void QGuildVoyageShow(bool past, bool future)
    {
        QGuildEarlier.IsEnabled = past;
        QGuildLater.IsEnabled = future;
    }

    private void QGuildRetreatObserve(object sender, RoutedEventArgs e)
    {
        _qGuildHost.QWindowAtelier.CAtelierNavigation.CNavigationStationUndo();
    }

    private void QGuildAdvanceObserve(object sender, RoutedEventArgs e)
    {
        _qGuildHost.QWindowAtelier.CAtelierNavigation.CNavigationStationRedo();
    }

    private void QGuildUndoObserve(object sender, RoutedEventArgs e)
    {
        QChronicle.QChronicleCaretRefine(_cGuild.CGuildSession.CSessionUndo);
    }

    private void QGuildRedoObserve(object sender, RoutedEventArgs e)
    {
        QChronicle.QChronicleCaretRefine(_cGuild.CGuildSession.CSessionRedo);
    }

    private void QGuildChronicleUpdate()
    {
        (bool undo, bool redo) = _cGuild.CGuildSession.CSessionChronicleRead();
        QGuildBackward.IsEnabled = undo;
        QGuildForward.IsEnabled = redo;
    }
}
