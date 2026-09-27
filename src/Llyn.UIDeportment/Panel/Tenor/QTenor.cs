using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Llyn.UIDeportment;

internal sealed partial class QTenor
{
    private readonly UserControl _qTenorSurface;

    private PWindow _qTenorHost = null!;

    private LTenor _lTenor = null!;

    private LEditor _lEditor = null!;

    internal QTenor(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qTenorSurface = surface;

        surface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QTenorPressHandle, QTenorPressCheck));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QTenorPortraitHandle, QTenorPressCheck));
        QTenorPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        QTenorPress.Command = ApplicationCommands.Print;

        QChoice.QChoiceDropperAttach(QDegreeDropper, QDegreeDropdown, QDegree);
        QChoice.QChoiceDropperAttach(QGrilleDropper, QGrilleDropdown, QGrilleDropper);

        QDegreeIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        QGrilleIcon.QIconSource = QIcon.QIconResolve("filter", 24);
        QTenorBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        QSounding.SetResourceReference(QField.QFieldHintProperty, "Tag.Search");
        QQuest.SetResourceReference(QField.QFieldHintProperty, "Quest.Search");
        QTenorFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        QTenorStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        QTenorEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        QTenorLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        QTenorBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        QTenorForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        QTenorPortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        QTenorPress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        QTenorViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        QTenorScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        QSounding.TextChanged += QSoundingHandle;
        QQuest.TextChanged += QQuestHandle;
        QTenorFresh.Click += QTenorFreshHandle;
        QTenorStore.Click += QTenorStoreHandle;
        QTenorEarlier.Click += QTenorRetreatHandle;
        QTenorLater.Click += QTenorAdvanceHandle;
        QTenorBackward.Click += QTenorUndoHandle;
        QTenorForward.Click += QTenorRedoHandle;
        QTenorViewer.Click += QTenorScribeHandle;
        QTenorScribe.Click += QTenorScribeHandle;
        QTenorBin.Click += QTenorBinHandle;
    }

    private Border QDegree => QContract.QContractFind<Border>(_qTenorSurface, "PDegree");

    private ToggleButton QDegreeDropper => QContract.QContractFind<ToggleButton>(_qTenorSurface, "PDegreeDropper");

    private QIconImage QDegreeIcon => QContract.QContractFind<QIconImage>(_qTenorSurface, "PDegreeIcon");

    private Popup QDegreeDropdown => QContract.QContractFind<Popup>(_qTenorSurface, "PDegreeDropdown");

    private StackPanel QDegreeList => QContract.QContractFind<StackPanel>(_qTenorSurface, "PDegreeList");

    private TextBox QSounding => QContract.QContractFind<TextBox>(_qTenorSurface, "PSounding");

    private ItemsControl QGamut => QContract.QContractFind<ItemsControl>(_qTenorSurface, "PGamut");

    private TextBlock QGamutEmpty => QContract.QContractFind<TextBlock>(_qTenorSurface, "PGamutEmpty");

    private ItemsControl QCohort => QContract.QContractFind<ItemsControl>(_qTenorSurface, "PCohort");

    private TextBlock QCohortEmpty => QContract.QContractFind<TextBlock>(_qTenorSurface, "PCohortEmpty");

    private TextBox QQuest => QContract.QContractFind<TextBox>(_qTenorSurface, "PQuest");

    private ToggleButton QGrilleDropper =>
        QContract.QContractFind<ToggleButton>(_qTenorSurface, "PGrilleDropper");

    private QIconImage QGrilleIcon => QContract.QContractFind<QIconImage>(_qTenorSurface, "PGrilleIcon");

    private FrameworkElement QGrilleMark =>
        QContract.QContractFind<FrameworkElement>(_qTenorSurface, "PGrilleMark");

    private Popup QGrilleDropdown => QContract.QContractFind<Popup>(_qTenorSurface, "PGrilleDropdown");

    private StackPanel QGrilleList => QContract.QContractFind<StackPanel>(_qTenorSurface, "PGrilleList");

    private Button QTenorFresh => QContract.QContractFind<Button>(_qTenorSurface, "PTenorFresh");

    private Button QTenorStore => QContract.QContractFind<Button>(_qTenorSurface, "PTenorStore");

    private StackPanel QTenorVoyage => QContract.QContractFind<StackPanel>(_qTenorSurface, "PTenorVoyage");

    private Button QTenorEarlier => QContract.QContractFind<Button>(_qTenorSurface, "PTenorEarlier");

    private Button QTenorLater => QContract.QContractFind<Button>(_qTenorSurface, "PTenorLater");

    private StackPanel QTenorChronicle =>
        QContract.QContractFind<StackPanel>(_qTenorSurface, "PTenorChronicle");

    private Button QTenorBackward => QContract.QContractFind<Button>(_qTenorSurface, "PTenorBackward");

    private Button QTenorForward => QContract.QContractFind<Button>(_qTenorSurface, "PTenorForward");

    private Button QTenorPortrait => QContract.QContractFind<Button>(_qTenorSurface, "PTenorPortrait");

    private Button QTenorPress => QContract.QContractFind<Button>(_qTenorSurface, "PTenorPress");

    private Border QTenorMode => QContract.QContractFind<Border>(_qTenorSurface, "PTenorMode");

    private RadioButton QTenorViewer => QContract.QContractFind<RadioButton>(_qTenorSurface, "PTenorViewer");

    private RadioButton QTenorScribe => QContract.QContractFind<RadioButton>(_qTenorSurface, "PTenorScribe");

    private PDisplay QTenorDisplay => QContract.QContractFind<PDisplay>(_qTenorSurface, "PDisplay");

    private PEditor QTenorEditor => QContract.QContractFind<PEditor>(_qTenorSurface, "PEditor");

    private Button QTenorBin => QContract.QContractFind<Button>(_qTenorSurface, "PTenorBin");

    private QIconImage QTenorBinIcon => QContract.QContractFind<QIconImage>(_qTenorSurface, "PTenorBinIcon");

    internal void QTenorAttach(PWindow host)
    {
        _qTenorHost = host;
        _lEditor = host.PWindowDeportment.LWindowForge.QForgeEditorCreate(host.PWindowUnreadableConfirm);
        LLectern lectern = new(_lEditor.LEditorDisplay);
        _lTenor = host.PWindowDeportment.LWindowForge.QForgeTenorCreate(
            _lEditor, lectern, QTenorShownCheck, QTenorDiscardConfirm, host.PWindowDeleteConfirm);
        LPanel panel = _lTenor.LTenorPanel;
        panel.LPanelChanged += QTenorModeUpdate;
        panel.LPanelRowsChanged += QCohortFind;
        panel.LPanelFailed += host.PWindowFailureShow;

        QGamut.ItemsSource = _qGamutList;
        QCohort.ItemsSource = _qCohortList;
        QLookItem.QLookItemAttach(QGamut, QGamutApply);
        QLookItem.QLookItemAttach(QCohort, QCohortApply);

        QTenorDisplay.PDisplayAttach(host, lectern);
        _lEditor.LEditorStateChanged += QTenorStoreUpdate;
        QTenorEditor.PEditorAttach(host, _lEditor, lectern);
        QTenorEditor.PEditorChronicleChanged += QTenorChronicleUpdate;
    }

    private void QTenorStoreUpdate()
    {
        QTenorStore.IsEnabled = _lEditor.LEditorStorable;
    }

    internal void QTenorReset()
    {
        _lTenor.LTenorPanel.LPanelClear();
        QGamutReset();
        QGamutFind();
    }

    internal bool QTenorDraftFinish(bool store)
    {
        return QTenorEditor.PEditorDraftFinish(store);
    }

    internal bool QTenorChangeCheck()
    {
        return _lTenor.LTenorPanel.LPanelChangeCheck();
    }

    internal void QTenorClose()
    {
        QTenorEditor.PEditorClose();
        QTenorDisplay.PDisplayClose();
    }

    private void QTenorPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lTenor?.LTenorPanel.LPanelPressAllowed ?? false;
    }

    private async void QTenorPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _qTenorHost.PWindowPressRun(_lTenor.LTenorPortraitPrint);
    }

    private async void QTenorPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _qTenorHost.PWindowPortraitExport(_lTenor.LTenorFileRead(), _lTenor.LTenorPortraitExport);
    }

    internal void QTenorVoyageShow(bool past, bool future)
    {
        QTenorEarlier.IsEnabled = past;
        QTenorLater.IsEnabled = future;
    }

    private void QTenorRetreatHandle(object sender, RoutedEventArgs e)
    {
        _qTenorHost.PVoyageRetreatRun();
    }

    private void QTenorAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _qTenorHost.PVoyageAdvanceRun();
    }

    private void QTenorUndoHandle(object sender, RoutedEventArgs e)
    {
        QTenorEditor.QChronicleUndo();
    }

    private void QTenorRedoHandle(object sender, RoutedEventArgs e)
    {
        QTenorEditor.QChronicleRedo();
    }

    private void QTenorChronicleUpdate()
    {
        (bool undo, bool redo) = QTenorEditor.PEditorChronicleRead();
        QTenorBackward.IsEnabled = undo;
        QTenorForward.IsEnabled = redo;
    }

    private bool QTenorShownCheck()
    {
        return _qTenorSurface.IsVisible;
    }

    private bool QTenorDiscardConfirm()
    {
        return _qTenorHost.PWindowDiscardConfirm(true, QTenorEditor.PEditorDraftFinish);
    }

    private void QTenorModeUpdate()
    {
        LPanel panel = _lTenor.LTenorPanel;
        QTenorEditor.Visibility = QLook.QLookVisibleRead(panel.LPanelEditing);
        QTenorDisplay.Visibility = QLook.QLookVisibleRead(panel.LPanelViewerChecked);
        QTenorViewer.IsChecked = QLook.QLookCheckedRead(panel.LPanelViewerChecked);
        QTenorScribe.IsChecked = QLook.QLookCheckedRead(panel.LPanelScribeChecked);
        QTenorVoyage.Visibility = QLook.QLookVisibleRead(panel.LPanelViewerChecked);
        QTenorChronicle.Visibility = QLook.QLookVisibleRead(panel.LPanelScribeChecked);
        QTenorMode.IsEnabled = panel.LPanelModeEnabled;
        QTenorBin.IsEnabled = panel.LPanelBinEnabled;
    }
}
