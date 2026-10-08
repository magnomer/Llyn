using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QGuild : QChronicleHost
{
    private readonly ObservableCollection<QRollItem> _qRollList = [];

    private readonly UserControl _qGuildSurface;

    private readonly QOeuvre _qGuildOeuvre;

    private readonly QVita _qVita;

    private readonly QAutograph _qAutograph;

    private readonly QColophon _qColophon;

    private readonly QPanelRail _qGuildRail;

    private readonly QChoiceOrder _qGuildOrder;

    private readonly QChoiceFilter _qGuildFilter;

    private CGuild _cGuild = null!;

    internal QGuild(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qGuildSurface = surface;
        _qVita = new QVita(QContract.QContractFind<UserControl>(surface, "PVita"));
        _qAutograph = new QAutograph(QContract.QContractFind<UserControl>(surface, "PAutograph"));
        _qColophon = new QColophon(QGuildColophon);
        _qGuildRail = new QPanelRail(
            QContract.QContractFind<UserControl>(surface, "PGuildRail"), QGuildBin, QGuildBinIcon, true, false);
        _qGuildOrder = new QChoiceOrder(QContract.QContractFind<UserControl>(surface, "PGuildOrder"), QEchelon);
        _qGuildFilter = new QChoiceFilter(QContract.QContractFind<UserControl>(surface, "PGuildFilter"));
        _qGuildOeuvre = new QOeuvre(surface);

        QMuster.SetResourceReference(QField.QFieldHintProperty, "Muster.Search");

        QMuster.TextChanged += QMusterObserve;
        _qGuildRail.QPanelRailCreated += QGuildFreshObserve;
        _qGuildRail.QPanelRailStored += QGuildStoreObserve;
        _qGuildRail.QPanelRailToggled += QGuildScribeObserve;
        _qGuildRail.QPanelRailDeleted += QGuildBinObserve;

        QRoll.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QRollObserve));

        QLookItem.QLookItemAttach(QRoll, QRollItem.QRollItemApply);
    }

    private Border QEchelon => QContract.QContractFind<Border>(_qGuildSurface, "PEchelon");

    private TextBox QMuster => QContract.QContractFind<TextBox>(_qGuildSurface, "PMuster");

    private ItemsControl QRoll => QContract.QContractFind<ItemsControl>(_qGuildSurface, "PRoll");

    private TextBlock QRollEmpty => QContract.QContractFind<TextBlock>(_qGuildSurface, "PRollEmpty");

    private UserControl QGuildColophon => QContract.QContractFind<UserControl>(_qGuildSurface, "PColophon");

    private Button QGuildBin => QContract.QContractFind<Button>(_qGuildSurface, "PGuildBin");

    private QIconImage QGuildBinIcon => QContract.QContractFind<QIconImage>(_qGuildSurface, "PGuildBinIcon");

    internal void QGuildIntroduce(CAtelier atelier, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cGuild = CGuild.CGuildCreate(
            atelier,
            QGuildShownCheck,
            envoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        _cGuild.CGuildChanged += QGuildModeUpdate;
        _cGuild.CGuildPanel.CPanelChanged += QGuildModeUpdate;
        _cGuild.CGuildPanel.CPanelAperture.CApertureRowsChanged += QRollRefine;
        _cGuild.CGuildOeuvre.COeuvrePanel.CPanelChanged += QGuildModeUpdate;
        _cGuild.CGuildOeuvre.COeuvrePanel.CPanelAperture.CApertureRowsChanged += QTallyRefine;
        _cGuild.CGuildOeuvre.COeuvrePanel.CPanelCleared += _qColophon.QColophonClearRefine;
        _cGuild.CGuildOeuvre.COeuvreColophonChanged += _qColophon.QColophonRefine;
        _qVita.QVitaAttach(atelier.CAtelierNavigation, _cGuild);
        _qAutograph.QAutographIntroduce(_cGuild);
        _qGuildOeuvre.QOeuvreIntroduce(_cGuild, atelier);
        _cGuild.CGuildAutograph.CDeskStateChanged += QGuildChronicleUpdate;

        _qGuildRail.QPanelRailIntroduce(atelier.CAtelierNavigation, this);
        _qGuildOrder.QChoiceOrderIntroduce(_cGuild.CGuildPanel.CPanelAperture, "Echelon", CGuild.CGuildOrderRead());
        _qGuildFilter.QChoiceFilterIntroduce(_cGuild.CGuildPanel.CPanelAperture, "Louver");

        QRoll.ItemsSource = _qRollList;

        _qGuildSurface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QGuildPressObserve, QGuildPressCheck));
    }

    internal void QGuildVistaRefine()
    {
        CGuildRoll roll = _cGuild.CGuildRollRead();
        _qGuildOrder.QChoiceOrderRefine();
        _qGuildFilter.QChoiceFilterBuild(roll.CGuildRollKind);
        _qGuildFilter.QChoiceFilterRefine();
        QRollShow(roll);
    }

    internal void QGuildClose()
    {
        _qGuildOrder.QChoiceOrderClose();
        _qGuildFilter.QChoiceFilterClose();
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

    internal void QTallyRefine()
    {
        _qColophon.QColophonTallyRefine(_cGuild.CGuildOeuvre.COeuvreTallyRead());
    }

    private void QGuildModeUpdate()
    {
        QContract.QContractFind<UserControl>(_qGuildSurface, "PAutograph").Visibility =
            QLook.QLookVisibleRead(_cGuild.CGuildDiptych.CDiptychParentEditing);
        QContract.QContractFind<UserControl>(_qGuildSurface, "PVita").Visibility =
            QLook.QLookVisibleRead(_cGuild.CGuildDiptych.CDiptychParentShown);
        QGuildColophon.Visibility = QLook.QLookVisibleRead(_cGuild.CGuildDiptych.CDiptychChildSide);
        _qGuildRail.QPanelRailRefine(
            _cGuild.CGuildDiptych.CDiptychScribeChecked, _cGuild.CGuildModeEnabled, _cGuild.CGuildBinEnabled);
        _qGuildRail.QEntryStorableRefine(_cGuild.CGuildStoreEnabled);
        _qAutograph.QAutographModeUpdate();
    }

    private void QMusterObserve(object sender, TextChangedEventArgs e)
    {
        _cGuild.CGuildPanel.CPanelAperture.CApertureQuerySet(QMuster.Text);
    }

    private void QRollObserve(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildAuthorSelect(QSender.QSenderSourceRead<QRollItem>(e)?.QRollItemId);
    }

    private void QGuildFreshObserve()
    {
        _cGuild.CGuildDiptych.CDiptychEntryCreate();
    }

    private void QGuildScribeObserve(bool scribe)
    {
        _cGuild.CGuildScribeToggle(scribe);
    }

    private void QGuildStoreObserve()
    {
        _cGuild.CGuildSession.CSessionSave();
    }

    private void QGuildBinObserve()
    {
        _cGuild.CGuildDiptych.CDiptychEntryDelete();
    }

    private void QGuildPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cGuild.CGuildDiptych.CDiptychChildSide;
    }

    private async void QGuildPressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cGuild.CGuildPortraitPrint();
    }

    public void QChronicleUndoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cGuild.CGuildSession.CSessionUndo);
    }

    public void QChronicleRedoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cGuild.CGuildSession.CSessionRedo);
    }

    private void QGuildChronicleUpdate()
    {
        (bool undo, bool redo) = _cGuild.CGuildSession.CSessionChronicleRead();
        _qGuildRail.QChronicleRefine(undo, redo);
    }
}
