using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QSGalleyBar
{
    private readonly Window _qsGalleySurface;

    private readonly CSLens _csLens;

    internal QSGalleyBar(Window surface, CSLens lens)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(lens);

        _qsGalleySurface = surface;
        _csLens = lens;
        QContract.QContractFind<QIconImage>(surface, "PSGalleyMinusIcon").QIconSource =
            QIcon.QIconResolve("remove", 24);
        QContract.QContractFind<QIconImage>(surface, "PSGalleyPlusIcon").QIconSource =
            QIcon.QIconResolve("add", 24);
        QSGalleyLens = lens.CSLensRead();
        QSGalleyLensRefine(QSGalleyLens);
        QSGalleyCanvas.SizeChanged += QSGalleySizeObserve;
        QSGalleyCanvas.PreviewMouseWheel += QSGalleyWheelObserve;
        QSGalleyWidth.Click += QSGalleyWidthObserve;
        QSGalleyPage.Click += QSGalleyPageObserve;
        QSGalleyMinus.Click += QSGalleyMinusObserve;
        QSGalleyPlus.Click += QSGalleyPlusObserve;
        QSGalleyPercent.KeyDown += QSGalleyPercentObserve;
        surface.PreviewKeyDown += QSGalleyKeyObserve;
    }

    internal event Action<double>? QSGalleyFactorChanged;

    internal CSLensState QSGalleyLens { get; private set; }

    private ScrollViewer QSGalleyCanvas =>
        QContract.QContractFind<ScrollViewer>(_qsGalleySurface, "PSGalleyCanvas");

    private ItemsControl QSGalleyList =>
        QContract.QContractFind<ItemsControl>(_qsGalleySurface, "PSGalleyList");

    private RadioButton QSGalleyWidth =>
        QContract.QContractFind<RadioButton>(_qsGalleySurface, "PSGalleyWidth");

    private RadioButton QSGalleyPage =>
        QContract.QContractFind<RadioButton>(_qsGalleySurface, "PSGalleyPage");

    private Button QSGalleyMinus => QContract.QContractFind<Button>(_qsGalleySurface, "PSGalleyMinus");

    private TextBox QSGalleyPercent => QContract.QContractFind<TextBox>(_qsGalleySurface, "PSGalleyPercent");

    private Button QSGalleyPlus => QContract.QContractFind<Button>(_qsGalleySurface, "PSGalleyPlus");

    internal void QSGalleyLensRefine(CSLensState lens)
    {
        ArgumentNullException.ThrowIfNull(lens);

        double before = QSGalleyLens.CSLensStateFactor;
        QSGalleyLens = lens;
        QSGalleyWidth.IsChecked = lens.CSLensStateMode == CSLensMode.CSLensModeWidth;
        QSGalleyPage.IsChecked = lens.CSLensStateMode == CSLensMode.CSLensModePage;
        QSGalleyPercent.Text = lens.CSLensStateText;
        QSGalleyFactorChanged?.Invoke(before);
    }

    private (double, double) QSGalleyAreaDraw()
    {
        ScrollViewer canvas = QSGalleyCanvas;
        Thickness margin = QSGalleyList.Margin;
        return (canvas.ViewportWidth - margin.Left - margin.Right, canvas.ViewportHeight - (margin.Top * 2));
    }

    private void QSGalleyWidthObserve(object sender, RoutedEventArgs e)
    {
        QSGalleyLensRefine(_csLens.CSLensModeSet(CSLensMode.CSLensModeWidth));
    }

    private void QSGalleyPageObserve(object sender, RoutedEventArgs e)
    {
        QSGalleyLensRefine(_csLens.CSLensModeSet(CSLensMode.CSLensModePage));
    }

    private void QSGalleyMinusObserve(object sender, RoutedEventArgs e)
    {
        QSGalleyLensRefine(_csLens.CSLensLadderMove(-1));
    }

    private void QSGalleyPlusObserve(object sender, RoutedEventArgs e)
    {
        QSGalleyLensRefine(_csLens.CSLensLadderMove(1));
    }

    private void QSGalleyPercentObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        e.Handled = true;
        QSGalleyLensRefine(_csLens.CSLensTextSet(QSGalleyPercent.Text));
    }

    private void QSGalleyWheelObserve(object sender, MouseWheelEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            e.Handled = true;
            QSGalleyLensRefine(_csLens.CSLensLadderMove(e.Delta));
        }
    }

    private void QSGalleySizeObserve(object sender, SizeChangedEventArgs e)
    {
        (double width, double height) = QSGalleyAreaDraw();
        QSGalleyLensRefine(_csLens.CSLensAreaSet(width, height));
    }

    private void QSGalleyKeyObserve(object sender, KeyEventArgs e)
    {
        if (Keyboard.Modifiers != ModifierKeys.Control
            || Keyboard.FocusedElement is TextBoxBase or ComboBox or ComboBoxItem)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.OemPlus or Key.Add:
                QSGalleyLensRefine(_csLens.CSLensLadderMove(1));
                break;
            case Key.OemMinus or Key.Subtract:
                QSGalleyLensRefine(_csLens.CSLensLadderMove(-1));
                break;
            case Key.D0 or Key.NumPad0:
                QSGalleyLensRefine(_csLens.CSLensModeSet(CSLensMode.CSLensModePage));
                break;
            default:
                return;
        }

        e.Handled = true;
    }
}
