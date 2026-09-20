using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Application;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PGuild : UserControl
{
    private readonly ObservableCollection<PRollItem> _pRollList = [];

    private readonly ObservableCollection<PShelfItem> _pOeuvreList = [];

    private PWindow _pGuildHost = null!;

    private LGuild _lGuild = null!;

    public PGuild()
    {
        InitializeComponent();
    }

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
        _lGuild.LGuildOeuvre.LOeuvrePanel.LPanelDraftChanged += PGuildSourceUpdate;
        _lGuild.LGuildOeuvre.LOeuvrePanel.LPanelFailed += host.PWindowFailureShow;
        _lGuild.LGuildAutograph.LDeskStarted += PAutographStartUpdate;
        PGuildObserverAttach();
        _lGuild.LGuildAutograph.LDeskDraftChanged += PAutographDraftUpdate;
        _lGuild.LGuildAutograph.LDeskFailed += host.PWindowFailureShow;

        PRoll.ItemsSource = _pRollList;
        POeuvre.ItemsSource = _pOeuvreList;

        PColophon.PColophonAttach(host);

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PGuildPressHandle, PGuildPressCheck));
    }

    internal void PGuildVistaRestore()
    {
        _lGuild.LGuildVistaRestore(_pGuildHost.PWindowDeportment);
        _lGuild.LGuildPanel.LPanelObserverAttach(
            LSubject.LSubjectVista, PObserver.PObserverCreate(this, _lGuild.LGuildPanel.LPanelRowsUpdate));
        _lGuild.LGuildOeuvre.LOeuvrePanel.LPanelObserverAttach(
            LSubject.LSubjectVista,
            PObserver.PObserverCreate(this, _lGuild.LGuildOeuvre.LOeuvrePanel.LPanelRowsUpdate));
        _lGuild.LGuildPanel.LPanelObserverAttach(
            LSubject.LSubjectWorkspace, PObserver.PObserverCreate(this, _lGuild.LGuildReset));
        _lGuild.LGuildPanel.LPanelObserverAttach(
            LSubject.LSubjectAuthor, PObserver.PObserverCreate(this, _lGuild.LGuildCatalogUpdate));
        _lGuild.LGuildPanel.LPanelObserverAttach(
            LSubject.LSubjectReference, PObserver.PObserverCreate(this, _lGuild.LGuildCatalogUpdate));
        _lGuild.LGuildPanel.LPanelObserverAttach(
            LSubject.LSubjectExample, PObserver.PObserverCreate(this, _lGuild.LGuildCatalogUpdate));
        _lGuild.LGuildPanel.LPanelObserverAttach(
            LSubject.LSubjectEntry, PObserver.PObserverCreate(this, _lGuild.LGuildCatalogUpdate));
        PChoice.PChoiceOrderBuild(
            PEchelonList,
            "Echelon",
            PEchelonHandle,
            [
                LCatalogOrder.LCatalogOrderName,
                LCatalogOrder.LCatalogOrderReverse,
                LCatalogOrder.LCatalogOrderWork,
                LCatalogOrder.LCatalogOrderUsage,
            ]);
        PChoice.PChoiceOrderApply(PEchelonDropdown, _lGuild.LGuildPanel.LPanelOrder);
        PChoice.PChoiceKindBuild(PLouverList, _lGuild.LGuildLouverRead(), PLouverHandle);
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
        PSplice.PSpliceApply(
            _pRollList,
            PRollItem.PRollItemBuild(_lGuild.LGuildRollRead()),
            PRollItem.PRollItemMatch,
            PRollItem.PRollItemSync);
        PRollEmpty.Visibility = PLook.PLookVisibleRead(_lGuild.LGuildEmpty);
        PVitaUpdate();
    }

    private void PVitaUpdate()
    {
        LVita vita = _lGuild.LGuildVitaRead();
        PVitaName.Text = vita.LVitaName;
        PField.PFieldPlaceholderShow(PVitaName, !vita.LVitaNamed);
        PVitaWork.Text = vita.LVitaWork;
        PVitaTally.Text = vita.LVitaTally;
        PAutographWork.Text = vita.LVitaWork;
        PAutographTally.Text = vita.LVitaTally;
        PFellow.ItemsSource = PFellowItem.PFellowItemBuild(vita.LVitaFellows);
        PVitaCitation.ItemsSource = PUsageItem.PUsageItemBuild(vita.LVitaUsages);
        PVitaFellowSection.Visibility = PLook.PLookVisibleRead(vita.LVitaFellowShown);
        PVitaCitationSection.Visibility = PLook.PLookVisibleRead(vita.LVitaUsageShown);
        PVitaBody.Visibility = PLook.PLookVisibleRead(_lGuild.LGuildVitaHeld);
        PVitaUnselected.Visibility = PLook.PLookVisibleRead(!_lGuild.LGuildVitaHeld);
    }

    private void POeuvreUpdate()
    {
        PSplice.PSpliceApply(
            _pOeuvreList,
            PShelfItem.PShelfItemBuild(_lGuild.LGuildOeuvre.LOeuvreRowsRead()),
            PShelfItem.PShelfItemMatch,
            PShelfItem.PShelfItemSync);
        POeuvreEmpty.SetResourceReference(TextBlock.TextProperty, _lGuild.LGuildOeuvre.LOeuvreEmptyKey);
        POeuvreEmpty.Visibility = PLook.PLookVisibleRead(_lGuild.LGuildOeuvre.LOeuvreEmpty);
        PColophon.PColophonTallyShow(_lGuild.LGuildOeuvre.LOeuvreTallyRead());
    }

    private void PGuildSourceUpdate(LDraft draft)
    {
        PColophon.PColophonShow(_lGuild.LGuildOeuvre.LOeuvreColophonRead(draft));
    }

    private void PGuildModeUpdate()
    {
        PAutograph.Visibility = PLook.PLookVisibleRead(_lGuild.LGuildAutographShown);
        PVita.Visibility = PLook.PLookVisibleRead(_lGuild.LGuildVitaShown);
        PColophon.Visibility = PLook.PLookVisibleRead(_lGuild.LGuildColophonShown);
        PGuildViewer.IsChecked = PLook.PLookCheckedRead(_lGuild.LGuildViewerChecked);
        PGuildScribe.IsChecked = PLook.PLookCheckedRead(_lGuild.LGuildScribeChecked);
        PGuildMode.IsEnabled = _lGuild.LGuildModeEnabled;
        PGuildBin.IsEnabled = _lGuild.LGuildBinEnabled;
        PGuildStore.IsEnabled = _lGuild.LGuildStoreEnabled;
        PAutographUnionBody.Visibility = PLook.PLookVisibleRead(_lGuild.LGuildUnionShown);
        PAutographUnionNotice.Visibility = PLook.PLookVisibleRead(!_lGuild.LGuildUnionShown);
    }

    private void PGuildObserverAttach()
    {
        PAutographObserverAttach(_lGuild.LGuildAutograph);
    }

    private void PAutographObserverAttach(LDesk desk)
    {
        desk.LDeskDraftAttach(LSubject.LSubjectDraft, PObserver.PObserverCreate(this, desk.LDeskDraftUpdate));
        desk.LDeskDraftAttach(LSubject.LSubjectTenure, PObserver.PObserverCreate(this, desk.LDeskStateUpdate));
    }

    private void PAutographStartUpdate()
    {
        PAutographUnion.Text = string.Empty;
        PAutographUnionList.ItemsSource = null;
        PAutographName.Focus();
    }

    private void PAutographDraftUpdate(LDraft draft)
    {
        PAutographName.Text = draft.LDraftAuthorName;
    }

    private void PLouverUpdate()
    {
        PLouverMark.Visibility = PLook.PLookVisibleRead(_lGuild.LGuildLouverActive);
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
        _lGuild.LGuildOrderSet(PSender.PSenderOrderRead(sender));
    }

    private void PLouverHandle(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildLouverSet(PChoice.PChoiceFilterRead(PLouverList));
        PLouverUpdate();
    }

    private void PRollHandle(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildRowSelect(PSender.PSenderSourceRead<PRollItem>(e)?.PRollItemId);
    }

    private void POeuvreHandle(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildSourceSelect(PSender.PSenderSourceRead<PShelfItem>(e)?.PShelfItemId);
    }

    private void PFellowHandle(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildRowSelect(PSender.PSenderSourceRead<PFellowItem>(e)?.PFellowItemId);
    }

    private void PVitaCitationHandle(object sender, RoutedEventArgs e)
    {
        PSender.PSenderSourceRead<PUsageItem>(e)?.PUsageItemShow(_pGuildHost);
    }

    private void PAutographNameHandle(object sender, TextChangedEventArgs e)
    {
        _lGuild.LGuildAutograph.LDeskDefer(
            new LRequestAuthorName(_lGuild.LGuildAutograph.LDeskId, PAutographName.Text));
    }

    private void PAutographUnionHandle(object sender, TextChangedEventArgs e)
    {
        PAutographUnionList.ItemsSource = PRollItem.PRollItemBuild(_lGuild.LGuildUnionRead(PAutographUnion.Text));
    }

    private void PAutographUnionSelect(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildUnionSelect(PSender.PSenderSourceRead<PRollItem>(e)?.PRollItemId);
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
}
