using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PNotationTemplate _pNotationTemplate;

    private Popup PNotation => (Popup)FindName(nameof(PNotation));

    private Border PNotationProgress => (Border)FindName(nameof(PNotationProgress));

    private TextBlock PNotationNotice => (TextBlock)FindName(nameof(PNotationNotice));

    private ItemsControl PNotationList => (ItemsControl)FindName(nameof(PNotationList));

    private Button PPhonetician => (Button)FindName(nameof(PPhonetician));

    private void PNotationAttach()
    {
        QLookItem.QLookItemAttach(
            PNotationList,
            (container, item, _) => PNotationItem.PNotationItemRefine(
                container, item, _pNotationTemplate.PNotationSelectorHandle));
        PNotation.Closed += PNotationClosedObserve;
        PPhonetician.Click += PPhoneticianRefine;
        PPhonetician.Click += PPhoneticianObserve;
        PPhonetician.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("lookup", 24));
    }

    private void PPhoneticianRefine(object sender, RoutedEventArgs e)
    {
        PNotationOpenRefine(PPhonetician);
    }

    private void PPhoneticianObserve(object sender, RoutedEventArgs e)
    {
        PNotationStartRefine(_qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandTranscriptionStart(0, string.Empty));
    }

    private void PNotationClosedObserve(object? sender, EventArgs e)
    {
        _qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandCancel();
    }

    internal void PNotationSelectorObserve(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PNotationReading reading })
        {
            return;
        }

        _qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandReadingSet(
            reading.PNotationReadingPhonetic, reading.PNotationReadingVariety);
        PNotationCloseRefine();
    }

    private void PNotationOpenRefine(UIElement anchor)
    {
        PNotation.IsOpen = false;
        PNotation.PlacementTarget = anchor;
        PNotation.IsOpen = true;
    }

    private void PNotationCloseRefine()
    {
        PNotation.IsOpen = false;
    }

    private async void PNotationStartRefine(CNotationRoll roll)
    {
        PNotationRefine(roll);
        PNotationRefine(await LEnsignImage.LEnsignLoad(_qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandFlagLoad));
    }

    internal void PNotationRefine(CNotationRoll roll)
    {
        PNotationList.ItemsSource = LSplice.LSpliceBuild(roll.CNotationRollRows, static row => new PNotationItem(row));
        PNotationList.Visibility = QLook.QLookVisibleRead(!roll.CNotationRollEmpty);
        PNotationProgress.Visibility = QLook.QLookVisibleRead(roll.CNotationRollSearching);
        PNotationNotice.Text = QLocalizationCatalog.QLocalizationTextRead(roll.CNotationRollNotice);
        PNotationNotice.Visibility = QLook.QLookVisibleRead(roll.CNotationRollEmpty);
    }
}
