using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QPhonology
{
    private readonly ObservableCollection<QInventoryItem> _qInventoryList = [];

    private readonly UserControl _qPhonologySurface;

    private readonly QEditor _qPhonologyEditor;

    private readonly QDisplay _qPhonologyDisplay;

    private readonly QArticulation _qArticulation;

    private QWindow _qPhonologyHost = null!;

    private CPhonology _cPhonology = null!;

    internal QPhonology(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qPhonologySurface = surface;
        _qPhonologyEditor = new QEditor(QContract.QContractFind<FrameworkElement>(surface, "PEditor"));
        _qPhonologyDisplay = new QDisplay(QContract.QContractFind<FrameworkElement>(surface, "PDisplay"));
        _qArticulation = new QArticulation(QPhonologyArticulation);

        surface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QPhonologyPressObserve, QPhonologyPressRefine));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QPhonologyPortraitObserve, QPhonologyPressRefine));
        QPhonologyPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        QPhonologyPress.Command = ApplicationCommands.Print;

        QChoice.QChoiceDropperAttach(QSequenceDropper, QSequenceDropdown, QSequence);
        QChoice.QChoiceDropperAttach(QLensDropper, QLensDropdown, QLensDropper);

        QSequenceIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        QLensIcon.QIconSource = QIcon.QIconResolve("filter", 24);
        QPhonologyBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        QProbe.SetResourceReference(QField.QFieldHintProperty, "Sound.Search");
        QArticulationHelper.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("articulation", 24));
        QPhonologyFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        QPhonologyStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        QPhonologyEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        QPhonologyLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        QPhonologyBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        QPhonologyForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        QPhonologyPortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        QPhonologyPress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        QPhonologyViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        QPhonologyScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        QLookItem.QLookItemAttach(QInventory, QInventoryItem.QInventoryItemRefine);
        QInventory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QInventoryObserve));

        QProbe.TextChanged += QProbeObserve;
        QArticulationHelper.Checked += QArticulationFoldRefine;
        QArticulationHelper.Unchecked += QArticulationFoldRefine;
        QPhonologyFresh.Click += QPhonologyFreshObserve;
        QPhonologyStore.Click += QPhonologyStoreObserve;
        QPhonologyEarlier.Click += QPhonologyRetreatObserve;
        QPhonologyLater.Click += QPhonologyAdvanceObserve;
        QPhonologyBackward.Click += QPhonologyUndoObserve;
        QPhonologyForward.Click += QPhonologyRedoObserve;
        QPhonologyViewer.Click += QPhonologyViewerObserve;
        QPhonologyScribe.Click += QPhonologyScribeObserve;
        QPhonologyBin.Click += QPhonologyBinObserve;
    }

    private Rectangle QArticulationSeam =>
        QContract.QContractFind<Rectangle>(_qPhonologySurface, "PArticulationSeam");

    private Border QSequence => QContract.QContractFind<Border>(_qPhonologySurface, "PSequence");

    private ToggleButton QSequenceDropper =>
        QContract.QContractFind<ToggleButton>(_qPhonologySurface, "PSequenceDropper");

    private QIconImage QSequenceIcon => QContract.QContractFind<QIconImage>(_qPhonologySurface, "PSequenceIcon");

    private TextBox QProbe => QContract.QContractFind<TextBox>(_qPhonologySurface, "PProbe");

    private ToggleButton QLensDropper => QContract.QContractFind<ToggleButton>(_qPhonologySurface, "PLensDropper");

    private QIconImage QLensIcon => QContract.QContractFind<QIconImage>(_qPhonologySurface, "PLensIcon");

    private FrameworkElement QLensMark => QContract.QContractFind<FrameworkElement>(_qPhonologySurface, "PLensMark");

    private Popup QSequenceDropdown => QContract.QContractFind<Popup>(_qPhonologySurface, "PSequenceDropdown");

    private StackPanel QSequenceList => QContract.QContractFind<StackPanel>(_qPhonologySurface, "PSequenceList");

    private Popup QLensDropdown => QContract.QContractFind<Popup>(_qPhonologySurface, "PLensDropdown");

    private StackPanel QLensList => QContract.QContractFind<StackPanel>(_qPhonologySurface, "PLensList");

    private ToggleButton QArticulationHelper =>
        QContract.QContractFind<ToggleButton>(_qPhonologySurface, "PArticulationHelper");

    private Button QPhonologyFresh => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyFresh");

    private Button QPhonologyStore => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyStore");

    private StackPanel QPhonologyVoyage =>
        QContract.QContractFind<StackPanel>(_qPhonologySurface, "PPhonologyVoyage");

    private Button QPhonologyEarlier => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyEarlier");

    private Button QPhonologyLater => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyLater");

    private StackPanel QPhonologyChronicle =>
        QContract.QContractFind<StackPanel>(_qPhonologySurface, "PPhonologyChronicle");

    private Button QPhonologyBackward => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyBackward");

    private Button QPhonologyForward => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyForward");

    private Button QPhonologyPortrait => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyPortrait");

    private Button QPhonologyPress => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyPress");

    private Border QPhonologyMode => QContract.QContractFind<Border>(_qPhonologySurface, "PPhonologyMode");

    private RadioButton QPhonologyViewer =>
        QContract.QContractFind<RadioButton>(_qPhonologySurface, "PPhonologyViewer");

    private RadioButton QPhonologyScribe =>
        QContract.QContractFind<RadioButton>(_qPhonologySurface, "PPhonologyScribe");

    private UserControl QPhonologyArticulation =>
        QContract.QContractFind<UserControl>(_qPhonologySurface, "PArticulation");

    private ItemsControl QInventory => QContract.QContractFind<ItemsControl>(_qPhonologySurface, "PInventory");

    private TextBlock QInventoryEmpty => QContract.QContractFind<TextBlock>(_qPhonologySurface, "PInventoryEmpty");

    private Button QPhonologyBin => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyBin");

    private QIconImage QPhonologyBinIcon =>
        QContract.QContractFind<QIconImage>(_qPhonologySurface, "PPhonologyBinIcon");

    internal void QPhonologyIntroduce(QWindow host)
    {
        _qPhonologyHost = host;
        _cPhonology = CPhonology.CPhonologyCreate(
            host.QWindowAtelier,
            QPhonologyShownCheck,
            host.QWindowEnvoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        CPanel panel = _cPhonology.CPhonologyPanel;
        QLectern lectern = new(_cPhonology.CPhonologyEditor.CEditorDisplay, panel);
        QChoice.QChoiceOrderBuild(QSequenceList, "Sequence", QSequenceObserve, CPhonology.CPhonologyOrderRead());
        panel.CPanelChanged += QPhonologyModeRefine;
        panel.CPanelRowsChanged += QInventoryRefine;
        panel.CPanelRowsChanged += QLensRefine;
        _cPhonology.CPhonologyWorkspaceChanged += QPhonologyWorkspaceRefine;
        _cPhonology.CPhonologyEditor.CEditorDesk.CDeskStateChanged += QPhonologyStoreRefine;

        QInventory.ItemsSource = _qInventoryList;

        _qPhonologyDisplay.QDisplayIntroduce(host, lectern);

        _qPhonologyEditor.QEditorIntroduce(host, _cPhonology.CPhonologyEditor);

        _qPhonologyEditor.QEditorChronicleChanged += QPhonologyChronicleRefine;

        _qArticulation.QArticulationIntroduce(
            QProbe,
            QContract.QContractFind<TextBox>(
                QContract.QContractFind<FrameworkElement>(_qPhonologySurface, "PEditor"), "PPronunciationField"));
    }

    private void QPhonologyStoreRefine()
    {
        QPhonologyStore.IsEnabled = _cPhonology.CPhonologyEditor.CEditorDesk.CDeskStorable;
    }

    internal async void QPhonologyVistaRefine()
    {
        QChoice.QChoiceOrderApply(QSequenceDropdown, _cPhonology.CPhonologyPanel.CPanelOrder);
        QLensRefine();
        CEnsignSheet<IReadOnlyList<CCatalogPronunciation>> sheet =
            await _cPhonology.CPhonologyRowsLoad(QEnsignImage.QEnsignDraw);
        QLensListRefine(sheet.CEnsignSheetLanguages);
        QInventoryRefine(sheet.CEnsignSheetRows);
    }

    private void QLensListRefine(IReadOnlyList<string> languages)
    {
        QChoice.QChoiceFilterBuild(QLensList, languages, _cPhonology.CPhonologyPanel.CPanelFilter, QLensObserve);
    }

    private async void QPhonologyWorkspaceRefine()
    {
        QInventoryRefine((await _cPhonology.CPhonologyRowsLoad(QEnsignImage.QEnsignDraw)).CEnsignSheetRows);
    }

    private bool QPhonologyShownCheck()
    {
        return _qPhonologySurface.IsVisible;
    }

    internal void QPhonologyExitRefine()
    {
        _qPhonologyEditor.QEditorPlayerRefine();
    }

    private void QInventoryRefine()
    {
        QInventoryRefine(_cPhonology.CPhonologyRowsRead());
    }

    private void QInventoryRefine(IReadOnlyList<CCatalogPronunciation> rows)
    {
        QSplice.QSpliceRefine(
            _qInventoryList,
            QInventoryItem.QInventoryItemBuild(rows),
            QInventoryItem.QInventoryItemMatch,
            QInventoryItem.QInventoryItemSync);
        QInventoryEmpty.Visibility = QLook.QLookVisibleRead(_cPhonology.CPhonologyEmpty);
    }

    private void QPhonologyModeRefine()
    {
        CPanel panel = _cPhonology.CPhonologyPanel;
        _qPhonologyEditor.QEditorVisibleRefine(QLook.QLookVisibleRead(panel.CPanelEditing));
        _qPhonologyDisplay.QDisplayVisibleRefine(QLook.QLookVisibleRead(panel.CPanelViewerChecked));
        QPhonologyViewer.IsChecked = panel.CPanelViewerChecked;
        QPhonologyScribe.IsChecked = panel.CPanelScribeChecked;
        QPhonologyVoyage.Visibility = QLook.QLookVisibleRead(panel.CPanelViewerChecked);
        QPhonologyChronicle.Visibility = QLook.QLookVisibleRead(panel.CPanelScribeChecked);
        QPhonologyMode.IsEnabled = panel.CPanelModeEnabled;
        QPhonologyBin.IsEnabled = panel.CPanelBinEnabled;
    }

    private void QLensRefine()
    {
        QLensMark.Visibility = QLook.QLookVisibleRead(_cPhonology.CPhonologyFiltered);
    }

    private void QArticulationFoldRefine(object sender, RoutedEventArgs e)
    {
        QPhonologyArticulation.Visibility =
            QLook.QLookVisibleRead(QLook.QLookCheckedRead(QArticulationHelper.IsChecked));
        QArticulationSeam.Visibility = QPhonologyArticulation.Visibility;
    }

    private void QProbeObserve(object sender, TextChangedEventArgs e)
    {
        _cPhonology.CPhonologyQuerySet(QProbe.Text);
    }

    private void QSequenceObserve(object sender, RoutedEventArgs e)
    {
        _cPhonology.CPhonologyOrderSet(QChoice.QChoiceOrderRead(sender));
        QSequenceRefine();
    }

    private void QSequenceRefine()
    {
        QSequenceDropper.IsChecked = false;
    }

    private void QLensObserve(object sender, RoutedEventArgs e)
    {
        _cPhonology.CPhonologyFilterSet(QChoice.QChoiceFilterRead(sender));
    }

    private void QInventoryObserve(object sender, RoutedEventArgs e)
    {
        _cPhonology.CPhonologyPanel.CPanelRowSelect(
            QSender.QSenderSourceRead<QInventoryItem>(e)?.QInventoryItemId);
    }

    private void QPhonologyFreshObserve(object sender, RoutedEventArgs e)
    {
        _cPhonology.CPhonologyPanel.CPanelEntryCreate();
    }

    private void QPhonologyViewerObserve(object sender, RoutedEventArgs e)
    {
        _cPhonology.CPhonologyPanel.CPanelScribeToggle(false);
    }

    private void QPhonologyScribeObserve(object sender, RoutedEventArgs e)
    {
        _cPhonology.CPhonologyPanel.CPanelScribeToggle(true);
    }

    private void QPhonologyStoreObserve(object sender, RoutedEventArgs e)
    {
        _cPhonology.CPhonologyEditor.CEditorEntrySave();
    }

    private void QPhonologyBinObserve(object sender, RoutedEventArgs e)
    {
        _cPhonology.CPhonologyPanel.CPanelEntryDelete();
    }

    private void QPhonologyPressRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cPhonology?.CPhonologyPanel.CPanelPressAllowed ?? false;
    }

    private async void QPhonologyPressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cPhonology.CPhonologyPortraitPrint();
    }

    private async void QPhonologyPortraitObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cPhonology.CPhonologyPortraitExport();
    }

    internal void QPhonologyVoyageRefine(bool past, bool future)
    {
        QPhonologyEarlier.IsEnabled = past;
        QPhonologyLater.IsEnabled = future;
    }

    private void QPhonologyRetreatObserve(object sender, RoutedEventArgs e)
    {
        _qPhonologyHost.QWindowAtelier.CAtelierNavigation.CNavigationStationUndo();
    }

    private void QPhonologyAdvanceObserve(object sender, RoutedEventArgs e)
    {
        _qPhonologyHost.QWindowAtelier.CAtelierNavigation.CNavigationStationRedo();
    }

    private void QPhonologyUndoObserve(object sender, RoutedEventArgs e)
    {
        _qPhonologyEditor.QChronicleUndoObserve();
    }

    private void QPhonologyRedoObserve(object sender, RoutedEventArgs e)
    {
        _qPhonologyEditor.QChronicleRedoObserve();
    }

    private void QPhonologyChronicleRefine()
    {
        (bool undo, bool redo) = _cPhonology.CPhonologyEditor.CEditorDesk.CDeskChronicleRead();
        QPhonologyBackward.IsEnabled = undo;
        QPhonologyForward.IsEnabled = redo;
    }
}
