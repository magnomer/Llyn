using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIDeportment;

public class PXiesheng : UserControl
{
    private readonly ObservableCollection<PGroveItem> _pGroveList = [];

    private readonly ObservableCollection<PKindredItem> _pKindredList = [];

    private PWindow _pXieshengHost = null!;

    private LXiesheng _lXiesheng = null!;

    public PXiesheng()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Sound/Shengfu/PXiesheng.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        PXieshengPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        PXieshengPress.Command = ApplicationCommands.Print;

        PChoice.PChoiceDropperAttach(PRungDropper, PRungDropdown, PRungBar);

        PRungIcon.PIconSource = PIcon.PIconResolve("sort", 24);
        PXieshengBinIcon.PIconSource = PIcon.PIconResolve("delete", 24);
        PXieshengFresh.Tag = PIcon.PIconResolve("new", 24);
        PXieshengStore.Tag = PIcon.PIconResolve("save", 24);
        PXieshengEarlier.Tag = PIcon.PIconResolve("retreat", 24);
        PXieshengLater.Tag = PIcon.PIconResolve("advance", 24);
        PXieshengBackward.Tag = PIcon.PIconResolve("undo", 24);
        PXieshengForward.Tag = PIcon.PIconResolve("redo", 24);
        PXieshengPortrait.Tag = PIcon.PIconResolve("export", 24);
        PXieshengPress.Tag = PIcon.PIconResolve("print", 24);
        PXieshengViewer.Tag = PIcon.PIconResolve("view", 24);
        PXieshengScribe.Tag = PIcon.PIconResolve("edit", 24);

        PLookItem.PLookItemAttach(PGrove, PGroveItem.PGroveItemApply);
        PLookItem.PLookItemAttach(PKindred, PKindredItem.PKindredItemApply);
        PGrove.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PGroveHandle));
        PKindred.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PKindredHandle));

        PLodestar.TextChanged += PLodestarHandle;
        PSextant.TextChanged += PSextantHandle;
        PXieshengFresh.Click += PXieshengFreshHandle;
        PXieshengStore.Click += PXieshengStoreHandle;
        PXieshengEarlier.Click += PXieshengRetreatHandle;
        PXieshengLater.Click += PXieshengAdvanceHandle;
        PXieshengBackward.Click += PXieshengUndoHandle;
        PXieshengForward.Click += PXieshengRedoHandle;
        PXieshengViewer.Click += PXieshengScribeHandle;
        PXieshengScribe.Click += PXieshengScribeHandle;
        PXieshengBin.Click += PXieshengBinHandle;
    }

    private Border PRungBar => (Border)FindName(nameof(PRungBar));

    private ToggleButton PRungDropper => (ToggleButton)FindName(nameof(PRungDropper));

    private PIconImage PRungIcon => (PIconImage)FindName(nameof(PRungIcon));

    private TextBox PLodestar => (TextBox)FindName(nameof(PLodestar));

    private Popup PRungDropdown => (Popup)FindName(nameof(PRungDropdown));

    private StackPanel PRungList => (StackPanel)FindName(nameof(PRungList));

    private ItemsControl PGrove => (ItemsControl)FindName(nameof(PGrove));

    private TextBlock PGroveEmpty => (TextBlock)FindName(nameof(PGroveEmpty));

    private TextBox PSextant => (TextBox)FindName(nameof(PSextant));

    private ItemsControl PKindred => (ItemsControl)FindName(nameof(PKindred));

    private TextBlock PKindredEmpty => (TextBlock)FindName(nameof(PKindredEmpty));

    private Button PXieshengFresh => (Button)FindName(nameof(PXieshengFresh));

    private Button PXieshengStore => (Button)FindName(nameof(PXieshengStore));

    private StackPanel PXieshengVoyage => (StackPanel)FindName(nameof(PXieshengVoyage));

    private Button PXieshengEarlier => (Button)FindName(nameof(PXieshengEarlier));

    private Button PXieshengLater => (Button)FindName(nameof(PXieshengLater));

    private StackPanel PXieshengChronicle => (StackPanel)FindName(nameof(PXieshengChronicle));

    private Button PXieshengBackward => (Button)FindName(nameof(PXieshengBackward));

    private Button PXieshengForward => (Button)FindName(nameof(PXieshengForward));

    private Button PXieshengPortrait => (Button)FindName(nameof(PXieshengPortrait));

    private Button PXieshengPress => (Button)FindName(nameof(PXieshengPress));

    private Border PXieshengMode => (Border)FindName(nameof(PXieshengMode));

    private RadioButton PXieshengViewer => (RadioButton)FindName(nameof(PXieshengViewer));

    private RadioButton PXieshengScribe => (RadioButton)FindName(nameof(PXieshengScribe));

    private PDisplay PDisplay => (PDisplay)FindName(nameof(PDisplay));

    private PStem PXieshengStem => (PStem)FindName(nameof(PXieshengStem));

    private PEditor PEditor => (PEditor)FindName(nameof(PEditor));

    private Button PXieshengBin => (Button)FindName(nameof(PXieshengBin));

    private PIconImage PXieshengBinIcon => (PIconImage)FindName(nameof(PXieshengBinIcon));

    internal void PXieshengAttach(PWindow host)
    {
        _pXieshengHost = host;
        _lXiesheng = host.PWindowDeportment.LWindowXieshengCreate(
            host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm),
            PXieshengShownCheck,
            PXieshengDiscardConfirm,
            host.PWindowDeleteConfirm);
        _lXiesheng.LXieshengEditor.LEditorStateChanged += PXieshengStoreUpdate;
        _lXiesheng.LXieshengChanged += PXieshengColumnUpdate;
        _lXiesheng.LXieshengGlyphChosen += host.PWindowGlyphShow;
        _lXiesheng.LXieshengPanel.LPanelChanged += PXieshengModeUpdate;
        _lXiesheng.LXieshengPanel.LPanelRowsChanged += PKindredUpdate;
        _lXiesheng.LXieshengPanel.LPanelCleared += PXieshengClearUpdate;
        _lXiesheng.LXieshengPanel.LPanelFailed += host.PWindowFailureShow;

        PGrove.ItemsSource = _pGroveList;
        PKindred.ItemsSource = _pKindredList;

        PDisplay.PDisplayAttach(host, _lXiesheng.LXieshengEditor.LEditorLectern);
        PXieshengStem.PStemAttach(host.PWindowDeportment);
        PXieshengStem.PStemEntryNotice = _lXiesheng.LXieshengGlyphSelect;

        PEditor.PEditorAttach(host, _lXiesheng.LXieshengEditor);

        PEditor.PEditorChronicleChanged += PXieshengChronicleUpdate;

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PXieshengPressHandle, PXieshengPressCheck));
        CommandBindings.Add(
            new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, PXieshengPortraitHandle, PXieshengPressCheck));
    }

    private void PXieshengStoreUpdate()
    {
        PXieshengStore.IsEnabled = _lXiesheng.LXieshengEditor.LEditorStorable;
    }

    internal bool PXieshengCheck()
    {
        return _lXiesheng.LXieshengAllowed;
    }

    internal void PXieshengVistaRestore()
    {
        _lXiesheng.LXieshengVistaRestore(_pXieshengHost.PWindowDeportment);
        _lXiesheng.LXieshengGroveAttach(
            LSubject.LSubjectVista, LObserver.LObserverCreate(this, _lXiesheng.LXieshengRowsUpdate));
        _lXiesheng.LXieshengGroveAttach(
            LSubject.LSubjectWorkspace, LObserver.LObserverCreate(this, PXieshengWorkspaceUpdate));
        _lXiesheng.LXieshengGroveAttach(
            LSubject.LSubjectFanqie, LObserver.LObserverCreate(this, _lXiesheng.LXieshengRowsUpdate));
        _lXiesheng.LXieshengGroveAttach(
            LSubject.LSubjectSettings, LObserver.LObserverCreate(this, _lXiesheng.LXieshengRowsUpdate));
        _lXiesheng.LXieshengPanel.LPanelObserverAttach(
            LSubject.LSubjectVista, LObserver.LObserverCreate(this, _lXiesheng.LXieshengPanel.LPanelRowsUpdate));
        _lXiesheng.LXieshengPanel.LPanelObserverAttach(
            LSubject.LSubjectEntry, LObserver.LObserverCreate(this, _lXiesheng.LXieshengEntryHandle));
        _lXiesheng.LXieshengPanel.LPanelChosenAttach(
            LSubject.LSubjectEntry, LObserver.LObserverCreate(this, _lXiesheng.LXieshengPanel.LPanelDraftUpdate));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderBuild(
            PRungList,
            "Rung",
            PRungHandle,
            [
                LCatalogOrder.LCatalogOrderName,
                LCatalogOrder.LCatalogOrderReverse,
                LCatalogOrder.LCatalogOrderUsage,
            ]);
        PChoice.PChoiceOrderApply(PRungDropdown, _lXiesheng.LXieshengRung);
        _lXiesheng.LXieshengLodestarSet(PLodestar.Text);
        _lXiesheng.LXieshengSextantSet(PSextant.Text);
        _lXiesheng.LXieshengRowsUpdate();
    }

    private async void PXieshengWorkspaceUpdate()
    {
        await LEnsignImage.LEnsignLoad(_pXieshengHost.PWindowDeportment);
        _lXiesheng.LXieshengReset();
    }

    internal bool PXieshengDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PXieshengChangeCheck()
    {
        return _lXiesheng.LXieshengPanel.LPanelChangeCheck();
    }

    internal bool PXieshengLeaveConfirm()
    {
        return _lXiesheng.LXieshengPanel.LPanelLeaveConfirm();
    }

    private bool PXieshengDiscardConfirm()
    {
        return _pXieshengHost.PWindowDiscardConfirm(true, PEditor.PEditorDraftFinish);
    }

    internal void PXieshengStemShow(string language, string? key)
    {
        PLodestar.Text = string.Empty;
        _lXiesheng.LXieshengStemShow(language, key);
    }

    internal void PXieshengScribeRestore(bool editing)
    {
        _lXiesheng.LXieshengPanel.LPanelScribeRestore(editing);
    }

    internal void PXieshengClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }

    private bool PXieshengShownCheck()
    {
        return IsVisible;
    }

    private void PXieshengColumnUpdate()
    {
        LSplice.LSpliceApply(
            _pGroveList,
            PGroveItem.PGroveItemBuild(_lXiesheng.LXieshengGroveRead()),
            PGroveItem.PGroveItemMatch,
            PGroveItem.PGroveItemSync);
        PGroveEmpty.SetResourceReference(TextBlock.TextProperty, _lXiesheng.LXieshengGroveKey);
        PGroveEmpty.Visibility = PLook.PLookVisibleRead(_lXiesheng.LXieshengGroveEmpty);
        PStemUpdate();
        PXieshengModeUpdate();
    }

    private void PKindredUpdate()
    {
        LSplice.LSpliceApply(
            _pKindredList,
            PKindredItem.PKindredItemBuild(_lXiesheng.LXieshengKindredRead()),
            PKindredItem.PKindredItemMatch,
            PKindredItem.PKindredItemSync);
        PKindredEmpty.SetResourceReference(TextBlock.TextProperty, _lXiesheng.LXieshengKindredKey);
        PKindredEmpty.Visibility = PLook.PLookVisibleRead(_lXiesheng.LXieshengKindredEmpty);
    }

    private void PStemUpdate()
    {
        PXieshengStem.PStemShow(_lXiesheng.LXieshengStemRead());
    }

    private void PXieshengModeUpdate()
    {
        PEditor.Visibility = PLook.PLookVisibleRead(_lXiesheng.LXieshengEditorShown);
        PDisplay.Visibility = PLook.PLookVisibleRead(_lXiesheng.LXieshengDisplayShown);
        PXieshengStem.Visibility = PLook.PLookVisibleRead(_lXiesheng.LXieshengStemShown);
        PXieshengViewer.IsChecked = PLook.PLookCheckedRead(_lXiesheng.LXieshengPanel.LPanelViewerChecked);
        PXieshengScribe.IsChecked = PLook.PLookCheckedRead(_lXiesheng.LXieshengPanel.LPanelScribeChecked);
        PXieshengVoyage.Visibility = PLook.PLookVisibleRead(_lXiesheng.LXieshengPanel.LPanelViewerChecked);
        PXieshengChronicle.Visibility = PLook.PLookVisibleRead(_lXiesheng.LXieshengPanel.LPanelScribeChecked);
        PXieshengMode.IsEnabled = _lXiesheng.LXieshengPanel.LPanelModeEnabled;
        PXieshengBin.IsEnabled = _lXiesheng.LXieshengPanel.LPanelBinEnabled;
    }

    private void PXieshengClearUpdate()
    {
        PStemUpdate();
    }

    private void PLodestarHandle(object sender, TextChangedEventArgs e)
    {
        _lXiesheng.LXieshengLodestarSet(PLodestar.Text);
    }

    private void PSextantHandle(object sender, TextChangedEventArgs e)
    {
        _lXiesheng.LXieshengSextantSet(PSextant.Text);
    }

    private void PRungHandle(object sender, RoutedEventArgs e)
    {
        PRungDropper.IsChecked = false;
        _lXiesheng.LXieshengRungSet(LChoice.LChoiceOrderRead(sender));
    }

    private void PGroveHandle(object sender, RoutedEventArgs e)
    {
        _lXiesheng.LXieshengStemSelect(PSender.PSenderSourceRead<PGroveItem>(e)?.PGroveItemId);
    }

    private void PKindredHandle(object sender, RoutedEventArgs e)
    {
        _pXieshengHost.PVoyageRecord();
        _lXiesheng.LXieshengPanel.LPanelRowSelect(PSender.PSenderSourceRead<PKindredItem>(e)?.PKindredItemId);
    }

    internal long PXieshengVoyageRead()
    {
        return _lXiesheng.LXieshengPanel.LPanelVoyageRead();
    }

    internal void PKindredEntryShow(long id)
    {
        _lXiesheng.LXieshengPanel.LPanelRowShow(id);
    }

    private void PXieshengFreshHandle(object sender, RoutedEventArgs e)
    {
        _lXiesheng.LXieshengPanel.LPanelFreshStart();
    }

    private void PXieshengScribeHandle(object sender, RoutedEventArgs e)
    {
        _lXiesheng.LXieshengPanel.LPanelScribeSet(ReferenceEquals(sender, PXieshengScribe));
    }

    private void PXieshengStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PXieshengBinHandle(object sender, RoutedEventArgs e)
    {
        _lXiesheng.LXieshengPanel.LPanelDelete();
    }

    private void PXieshengPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lXiesheng.LXieshengPanel.LPanelPressAllowed;
    }

    private async void PXieshengPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pXieshengHost.PWindowPressRun(_lXiesheng.LXieshengPortraitPrint);
    }

    private async void PXieshengPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pXieshengHost.PWindowPortraitExport(
            _lXiesheng.LXieshengFileRead(), _lXiesheng.LXieshengPortraitExport);
    }

    internal void PXieshengVoyageShow(bool past, bool future)
    {
        PXieshengEarlier.IsEnabled = past;
        PXieshengLater.IsEnabled = future;
    }

    private void PXieshengRetreatHandle(object sender, RoutedEventArgs e)
    {
        _pXieshengHost.PVoyageRetreatRun();
    }

    private void PXieshengAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _pXieshengHost.PVoyageAdvanceRun();
    }

    private void PXieshengUndoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleUndo();
    }

    private void PXieshengRedoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleRedo();
    }

    private void PXieshengChronicleUpdate()
    {
        (bool undo, bool redo) = PEditor.PEditorChronicleRead();
        PXieshengBackward.IsEnabled = undo;
        PXieshengForward.IsEnabled = redo;
    }
}
