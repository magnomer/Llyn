using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public partial class PTenor : UserControl
{
    private PWindow _pTenorHost = null!;

    private LTenor _lTenor = null!;

    private LEditor _lEditor = null!;

    public PTenor()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Tenor/PTenor.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PTenorPressHandle, PTenorPressCheck));
        CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, PTenorPortraitHandle, PTenorPressCheck));
        PTenorPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        PTenorPress.Command = ApplicationCommands.Print;

        QChoice.QChoiceDropperAttach(PDegreeDropper, PDegreeDropdown, PDegree);
        QChoice.QChoiceDropperAttach(PGrilleDropper, PGrilleDropdown, PGrilleDropper);

        PDegreeIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        PGrilleIcon.QIconSource = QIcon.QIconResolve("filter", 24);
        PTenorBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        PSounding.SetResourceReference(QField.QFieldHintProperty, "Tag.Search");
        PQuest.SetResourceReference(QField.QFieldHintProperty, "Quest.Search");
        PTenorFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        PTenorStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        PTenorEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        PTenorLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        PTenorBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        PTenorForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        PTenorPortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        PTenorPress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        PTenorViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        PTenorScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        PSounding.TextChanged += PSoundingHandle;
        PQuest.TextChanged += PQuestHandle;
        PTenorFresh.Click += PTenorFreshHandle;
        PTenorStore.Click += PTenorStoreHandle;
        PTenorEarlier.Click += PTenorRetreatHandle;
        PTenorLater.Click += PTenorAdvanceHandle;
        PTenorBackward.Click += PTenorUndoHandle;
        PTenorForward.Click += PTenorRedoHandle;
        PTenorViewer.Click += PTenorScribeHandle;
        PTenorScribe.Click += PTenorScribeHandle;
        PTenorBin.Click += PTenorBinHandle;
    }

    private Border PDegree => (Border)FindName(nameof(PDegree));

    private ToggleButton PDegreeDropper => (ToggleButton)FindName(nameof(PDegreeDropper));

    private QIconImage PDegreeIcon => (QIconImage)FindName(nameof(PDegreeIcon));

    private Popup PDegreeDropdown => (Popup)FindName(nameof(PDegreeDropdown));

    private StackPanel PDegreeList => (StackPanel)FindName(nameof(PDegreeList));

    private TextBox PSounding => (TextBox)FindName(nameof(PSounding));

    private ItemsControl PGamut => (ItemsControl)FindName(nameof(PGamut));

    private TextBlock PGamutEmpty => (TextBlock)FindName(nameof(PGamutEmpty));

    private ItemsControl PCohort => (ItemsControl)FindName(nameof(PCohort));

    private TextBlock PCohortEmpty => (TextBlock)FindName(nameof(PCohortEmpty));

    private TextBox PQuest => (TextBox)FindName(nameof(PQuest));

    private ToggleButton PGrilleDropper => (ToggleButton)FindName(nameof(PGrilleDropper));

    private QIconImage PGrilleIcon => (QIconImage)FindName(nameof(PGrilleIcon));

    private FrameworkElement PGrilleMark => (FrameworkElement)FindName(nameof(PGrilleMark));

    private Popup PGrilleDropdown => (Popup)FindName(nameof(PGrilleDropdown));

    private StackPanel PGrilleList => (StackPanel)FindName(nameof(PGrilleList));

    private Button PTenorFresh => (Button)FindName(nameof(PTenorFresh));

    private Button PTenorStore => (Button)FindName(nameof(PTenorStore));

    private StackPanel PTenorVoyage => (StackPanel)FindName(nameof(PTenorVoyage));

    private Button PTenorEarlier => (Button)FindName(nameof(PTenorEarlier));

    private Button PTenorLater => (Button)FindName(nameof(PTenorLater));

    private StackPanel PTenorChronicle => (StackPanel)FindName(nameof(PTenorChronicle));

    private Button PTenorBackward => (Button)FindName(nameof(PTenorBackward));

    private Button PTenorForward => (Button)FindName(nameof(PTenorForward));

    private Button PTenorPortrait => (Button)FindName(nameof(PTenorPortrait));

    private Button PTenorPress => (Button)FindName(nameof(PTenorPress));

    private Border PTenorMode => (Border)FindName(nameof(PTenorMode));

    private RadioButton PTenorViewer => (RadioButton)FindName(nameof(PTenorViewer));

    private RadioButton PTenorScribe => (RadioButton)FindName(nameof(PTenorScribe));

    private PDisplay PDisplay => (PDisplay)FindName(nameof(PDisplay));

    private PEditor PEditor => (PEditor)FindName(nameof(PEditor));

    private Button PTenorBin => (Button)FindName(nameof(PTenorBin));

    private QIconImage PTenorBinIcon => (QIconImage)FindName(nameof(PTenorBinIcon));

    internal void PTenorAttach(PWindow host)
    {
        _pTenorHost = host;
        _lEditor = host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm);
        _lTenor = host.PWindowDeportment.LWindowTenorCreate(
            _lEditor, PTenorShownCheck, PTenorDiscardConfirm, host.PWindowDeleteConfirm);
        LPanel panel = _lTenor.LTenorPanel;
        panel.LPanelChanged += PTenorModeUpdate;
        panel.LPanelRowsChanged += PCohortFind;
        panel.LPanelFailed += host.PWindowFailureShow;

        PGamut.ItemsSource = _pGamutList;
        PCohort.ItemsSource = _pCohortList;
        QLookItem.QLookItemAttach(PGamut, PGamutApply);
        QLookItem.QLookItemAttach(PCohort, PCohortApply);

        PDisplay.PDisplayAttach(host, _lEditor.LEditorLectern);
        _lEditor.LEditorStateChanged += PTenorStoreUpdate;
        PEditor.PEditorAttach(host, _lEditor);
        PEditor.PEditorChronicleChanged += PTenorChronicleUpdate;
    }

    private void PTenorStoreUpdate()
    {
        PTenorStore.IsEnabled = _lEditor.LEditorStorable;
    }

    internal void PTenorReset()
    {
        _lTenor.LTenorPanel.LPanelClear();
        PGamutReset();
        PGamutFind();
    }

    internal bool PTenorDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PTenorChangeCheck()
    {
        return _lTenor.LTenorPanel.LPanelChangeCheck();
    }

    internal void PTenorClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }

    private void PTenorPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lTenor?.LTenorPanel.LPanelPressAllowed ?? false;
    }

    private async void PTenorPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pTenorHost.PWindowPressRun(_lTenor.LTenorPortraitPrint);
    }

    private async void PTenorPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pTenorHost.PWindowPortraitExport(_lTenor.LTenorFileRead(), _lTenor.LTenorPortraitExport);
    }

    internal void PTenorVoyageShow(bool past, bool future)
    {
        PTenorEarlier.IsEnabled = past;
        PTenorLater.IsEnabled = future;
    }

    private void PTenorRetreatHandle(object sender, RoutedEventArgs e)
    {
        _pTenorHost.PVoyageRetreatRun();
    }

    private void PTenorAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _pTenorHost.PVoyageAdvanceRun();
    }

    private void PTenorUndoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleUndo();
    }

    private void PTenorRedoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleRedo();
    }

    private void PTenorChronicleUpdate()
    {
        (bool undo, bool redo) = PEditor.PEditorChronicleRead();
        PTenorBackward.IsEnabled = undo;
        PTenorForward.IsEnabled = redo;
    }

    private bool PTenorShownCheck()
    {
        return IsVisible;
    }

    private bool PTenorDiscardConfirm()
    {
        return _pTenorHost.PWindowDiscardConfirm(true, PEditor.PEditorDraftFinish);
    }

    private void PTenorModeUpdate()
    {
        LPanel panel = _lTenor.LTenorPanel;
        PEditor.Visibility = QLook.QLookVisibleRead(panel.LPanelEditing);
        PDisplay.Visibility = QLook.QLookVisibleRead(panel.LPanelViewerChecked);
        PTenorViewer.IsChecked = QLook.QLookCheckedRead(panel.LPanelViewerChecked);
        PTenorScribe.IsChecked = QLook.QLookCheckedRead(panel.LPanelScribeChecked);
        PTenorVoyage.Visibility = QLook.QLookVisibleRead(panel.LPanelViewerChecked);
        PTenorChronicle.Visibility = QLook.QLookVisibleRead(panel.LPanelScribeChecked);
        PTenorMode.IsEnabled = panel.LPanelModeEnabled;
        PTenorBin.IsEnabled = panel.LPanelBinEnabled;
    }
}
