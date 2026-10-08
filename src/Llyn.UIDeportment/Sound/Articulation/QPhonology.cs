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

    private readonly QPanelRail _qPhonologyRail;

    private readonly QChoiceOrder _qPhonologyOrder;

    private readonly QChoiceFilter _qPhonologyFilter;

    private CPhonology _cPhonology = null!;

    internal QPhonology(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qPhonologySurface = surface;
        _qPhonologyEditor = new QEditor(QContract.QContractFind<FrameworkElement>(surface, "PEditor"));
        _qPhonologyDisplay = new QDisplay(QContract.QContractFind<FrameworkElement>(surface, "PDisplay"));
        _qArticulation = new QArticulation(
            QPhonologyArticulation,
            QContract.QContractFind<ToggleButton>(surface, "PArticulationHelper"),
            QContract.QContractFind<Rectangle>(surface, "PArticulationSeam"));

        surface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QPhonologyPressObserve, QPhonologyPressRefine));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QPhonologyPortraitObserve, QPhonologyPressRefine));
        _qPhonologyRail = new QPanelRail(
            QContract.QContractFind<UserControl>(surface, "PPhonologyRail"),
            QPhonologyBin,
            QPhonologyBinIcon,
            true,
            true);
        _qPhonologyOrder = new QChoiceOrder(
            QContract.QContractFind<UserControl>(surface, "PPhonologyOrder"), QSequence);
        _qPhonologyFilter = new QChoiceFilter(QContract.QContractFind<UserControl>(surface, "PPhonologyFilter"));

        QProbe.SetResourceReference(QField.QFieldHintProperty, "Sound.Search");

        QLookItem.QLookItemAttach(QInventory, QInventoryItem.QInventoryItemRefine);
        QInventory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QInventoryObserve));

        QProbe.TextChanged += QProbeObserve;
        _qPhonologyRail.QPanelRailCreated += QPhonologyFreshObserve;
        _qPhonologyRail.QPanelRailStored += QPhonologyStoreObserve;
        _qPhonologyRail.QPanelRailToggled += QPhonologyScribeObserve;
        _qPhonologyRail.QPanelRailDeleted += QPhonologyBinObserve;
    }

    private Border QSequence => QContract.QContractFind<Border>(_qPhonologySurface, "PSequence");

    private TextBox QProbe => QContract.QContractFind<TextBox>(_qPhonologySurface, "PProbe");

    private UserControl QPhonologyArticulation =>
        QContract.QContractFind<UserControl>(_qPhonologySurface, "PArticulation");

    private ItemsControl QInventory => QContract.QContractFind<ItemsControl>(_qPhonologySurface, "PInventory");

    private TextBlock QInventoryEmpty => QContract.QContractFind<TextBlock>(_qPhonologySurface, "PInventoryEmpty");

    private Button QPhonologyBin => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyBin");

    private QIconImage QPhonologyBinIcon =>
        QContract.QContractFind<QIconImage>(_qPhonologySurface, "PPhonologyBinIcon");

    internal void QPhonologyIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cPhonology = CPhonology.CPhonologyCreate(
            atelier,
            QPhonologyShownCheck,
            envoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        CPanel panel = _cPhonology.CPhonologyPanel;
        _qPhonologyOrder.QChoiceOrderIntroduce(panel.CPanelAperture, "Sequence", CPhonology.CPhonologyOrderRead());
        _qPhonologyFilter.QChoiceFilterIntroduce(panel.CPanelAperture, "Lens");
        panel.CPanelChanged += QPhonologyModeRefine;
        panel.CPanelAperture.CApertureRowsChanged += QInventoryRefine;
        _cPhonology.CPhonologyWorkspaceChanged += QPhonologyWorkspaceRefine;
        _cPhonology.CPhonologyEditor.CEditorDesk.CDeskStateChanged += QPhonologyStoreRefine;

        QInventory.ItemsSource = _qInventoryList;

        _qPhonologyDisplay.QDisplayIntroduce(
            atelier, envoy, volume, mentionMenu, _cPhonology.CPhonologyEditor.CEditorDisplay);

        _qPhonologyEditor.QEditorIntroduce(atelier, envoy, volume, mentionMenu, _cPhonology.CPhonologyEditor);

        _qPhonologyEditor.QEditorChronicleChanged += QPhonologyChronicleRefine;
        _qPhonologyRail.QPanelRailIntroduce(atelier.CAtelierNavigation, _qPhonologyEditor);

        _qArticulation.QArticulationIntroduce(
            atelier.CAtelierCatalog,
            QProbe,
            QContract.QContractFind<TextBox>(
                QContract.QContractFind<FrameworkElement>(_qPhonologySurface, "PEditor"), "PPronunciationField"));
    }

    private void QPhonologyStoreRefine()
    {
        _qPhonologyRail.QEntryStorableRefine(_cPhonology.CPhonologyEditor.CEditorDesk.CDeskDraft.CDeskDraftStorable);
    }

    internal async void QPhonologyVistaRefine()
    {
        _qPhonologyOrder.QChoiceOrderRefine();
        _qPhonologyFilter.QChoiceFilterRefine();
        CEnsignSheet<IReadOnlyList<CCatalogPronunciation>> sheet =
            await _cPhonology.CPhonologyRowsLoad(QEnsignImage.QEnsignDraw);
        _qPhonologyFilter.QChoiceFilterBuild(sheet.CEnsignSheetLanguages);
        QInventoryRefine(sheet.CEnsignSheetRows);
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
        QInventoryEmpty.Visibility = QLook.QLookVisibleRead(_cPhonology.CPhonologyPanel.CPanelAperture.CApertureEmpty);
    }

    private void QPhonologyModeRefine()
    {
        CPanel panel = _cPhonology.CPhonologyPanel;
        _qPhonologyEditor.QEditorVisibleRefine(QLook.QLookVisibleRead(panel.CPanelEditing));
        _qPhonologyDisplay.QDisplayVisibleRefine(QLook.QLookVisibleRead(panel.CPanelViewerChecked));
        _qPhonologyRail.QPanelRailRefine(panel.CPanelScribeChecked, panel.CPanelModeEnabled, panel.CPanelBinEnabled);
    }

    private void QProbeObserve(object sender, TextChangedEventArgs e)
    {
        _cPhonology.CPhonologyPanel.CPanelAperture.CApertureQuerySet(QProbe.Text);
    }

    private void QInventoryObserve(object sender, RoutedEventArgs e)
    {
        _cPhonology.CPhonologyPanel.CPanelRowSelect(
            QSender.QSenderSourceRead<QInventoryItem>(e)?.QInventoryItemId);
    }

    private void QPhonologyFreshObserve()
    {
        _cPhonology.CPhonologyPanel.CPanelEntryCreate();
    }

    private void QPhonologyScribeObserve(bool scribe)
    {
        _cPhonology.CPhonologyPanel.CPanelScribeToggle(scribe);
    }

    private void QPhonologyStoreObserve()
    {
        _cPhonology.CPhonologyEditor.CEditorEntrySave();
    }

    private void QPhonologyBinObserve()
    {
        _cPhonology.CPhonologyPanel.CPanelBin.CPanelBinDelete();
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

    private void QPhonologyChronicleRefine()
    {
        (bool undo, bool redo) = _cPhonology.CPhonologyEditor.CEditorDesk.CDeskChronicle.CDeskChronicleRead();
        _qPhonologyRail.QChronicleRefine(undo, redo);
    }
}
