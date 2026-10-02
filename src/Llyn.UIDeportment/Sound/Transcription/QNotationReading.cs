using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Llyn.UIDeportment;

internal sealed class QNotationReading
{
    internal QNotationReading(
        string variety, string label, ImageSource? flag, string phonetic, string text, string opener, string closer)
    {
        QNotationReadingVariety = variety;
        QNotationReadingLabel = label;
        QNotationReadingFlag = flag;
        QNotationReadingPhonetic = phonetic;
        QNotationReadingText = text;
        QNotationReadingOpener = opener;
        QNotationReadingCloser = closer;
    }

    public string QNotationReadingVariety { get; }

    public string QNotationReadingLabel { get; }

    public ImageSource? QNotationReadingFlag { get; }

    public string QNotationReadingPhonetic { get; }

    public string QNotationReadingText { get; }

    public string QNotationReadingOpener { get; }

    public string QNotationReadingCloser { get; }

    internal static void QNotationReadingRefine(FrameworkElement container, object item, RoutedEventHandler select)
    {
        if (item is not QNotationReading reading)
        {
            return;
        }

        bool flagged = reading.QNotationReadingFlag is not null;

        if (QLook.QLookPartFind<Grid>(container, "QNotationReadingCell") is Grid cell)
        {
            cell.ColumnDefinitions[0].SharedSizeGroup = "PNotationColumn"
                + ItemsControl.GetAlternationIndex(container).ToString(CultureInfo.InvariantCulture);
        }

        if (QLook.QLookPartFind<Button>(container, "PNotationSelector") is Button selector)
        {
            selector.ToolTip = flagged ? reading.QNotationReadingLabel : null;
            selector.Click -= select;
            selector.Click += select;
        }

        if (QLook.QLookPartFind<Image>(container, "QNotationReadingFlag") is Image flag)
        {
            flag.Source = reading.QNotationReadingFlag;
            flag.Visibility = QLook.QLookVisibleRead(flagged);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "QNotationReadingLabel") is TextBlock label)
        {
            label.Text = reading.QNotationReadingLabel;
            label.Visibility = QLook.QLookVisibleRead(!flagged && reading.QNotationReadingLabel.Length > 0);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "QNotationReadingPhonetic") is TextBlock phonetic)
        {
            phonetic.Text = reading.QNotationReadingOpener + reading.QNotationReadingText
                + reading.QNotationReadingCloser;
        }
    }
}
