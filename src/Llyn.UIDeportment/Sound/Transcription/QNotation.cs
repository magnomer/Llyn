using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QNotation
{
    private readonly FrameworkElement _qNotationSurface;

    private CEditor _cEditor = null!;

    internal QNotation(FrameworkElement surface)
    {
        _qNotationSurface = surface;
        QLookItem.QLookItemAttach(
            QNotationList,
            (container, item, _) => QNotationItem.QNotationItemRefine(
                container, item, QNotationSelectorObserve));
        QNotationPopup.Closed += QNotationClosedObserve;
        QPhonetician.Click += QPhoneticianRefine;
        QPhonetician.Click += QPhoneticianObserve;
        QPhonetician.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("lookup", 24));
    }

    private Popup QNotationPopup => QContract.QContractFind<Popup>(_qNotationSurface, "PNotation");

    private Border QNotationProgress => QContract.QContractFind<Border>(_qNotationSurface, "PNotationProgress");

    private TextBlock QNotationNotice => QContract.QContractFind<TextBlock>(_qNotationSurface, "PNotationNotice");

    private ItemsControl QNotationList => QContract.QContractFind<ItemsControl>(_qNotationSurface, "PNotationList");

    private Button QPhonetician => QContract.QContractFind<Button>(_qNotationSurface, "PPhonetician");

    internal void QNotationIntroduce(CEditor editor)
    {
        _cEditor = editor;
        editor.CEditorDesk.CDeskErrand.CErrandNotationChanged += QNotationRefine;
    }

    private void QPhoneticianRefine(object sender, RoutedEventArgs e)
    {
        QNotationOpenRefine(QPhonetician);
    }

    private void QPhoneticianObserve(object sender, RoutedEventArgs e)
    {
        QNotationStartRefine(_cEditor.CEditorDesk.CDeskErrand.CErrandTranscriptionStart(null, string.Empty));
    }

    private void QNotationClosedObserve(object? sender, EventArgs e)
    {
        _cEditor.CEditorDesk.CDeskErrand.CErrandCancel();
    }

    private void QNotationSelectorObserve(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: QNotationReading reading })
        {
            return;
        }

        _cEditor.CEditorDesk.CDeskErrand.CErrandReadingSet(
            reading.QNotationReadingPhonetic, reading.QNotationReadingVariety);
        QNotationCloseRefine();
    }

    internal void QNotationOpenRefine(UIElement anchor)
    {
        QNotationPopup.IsOpen = false;
        QNotationPopup.PlacementTarget = anchor;
        QNotationPopup.IsOpen = true;
    }

    private void QNotationCloseRefine()
    {
        QNotationPopup.IsOpen = false;
    }

    internal async void QNotationStartRefine(CNotationRoll roll)
    {
        QNotationRefine(roll);
        QNotationRefine(await _cEditor.CEditorDesk.CDeskErrand.CErrandFlagLoad(QEnsignImage.QEnsignDraw));
    }

    private void QNotationRefine(CNotationRoll roll)
    {
        QNotationList.ItemsSource = QSplice.QSpliceBuild(roll.CNotationRollRows, static row => new QNotationItem(row));
        QNotationList.Visibility = QLook.QLookVisibleRead(!roll.CNotationRollEmpty);
        QNotationProgress.Visibility = QLook.QLookVisibleRead(roll.CNotationRollSearching);
        QNotationNotice.Text = QLocalizationCatalog.QLocalizationTextRead(roll.CNotationRollNotice);
        QNotationNotice.Visibility = QLook.QLookVisibleRead(roll.CNotationRollEmpty);
    }
}
