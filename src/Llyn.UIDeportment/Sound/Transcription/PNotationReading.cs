using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Llyn.UIDeportment;

internal sealed class PNotationReading
{
    internal PNotationReading(
        string variety, string label, ImageSource? flag, string phonetic, string text, string opener, string closer)
    {
        PNotationReadingVariety = variety;
        PNotationReadingLabel = label;
        PNotationReadingFlag = flag;
        PNotationReadingPhonetic = phonetic;
        PNotationReadingText = text;
        PNotationReadingOpener = opener;
        PNotationReadingCloser = closer;
    }

    public string PNotationReadingVariety { get; }

    public string PNotationReadingLabel { get; }

    public ImageSource? PNotationReadingFlag { get; }

    public string PNotationReadingPhonetic { get; }

    public string PNotationReadingText { get; }

    public string PNotationReadingOpener { get; }

    public string PNotationReadingCloser { get; }

    internal static void PNotationReadingRefine(FrameworkElement container, object item, RoutedEventHandler select)
    {
        if (item is not PNotationReading reading)
        {
            return;
        }

        bool flagged = reading.PNotationReadingFlag is not null;

        if (QLook.QLookPartFind<Grid>(container, "PNotationReadingCell") is Grid cell)
        {
            cell.ColumnDefinitions[0].SharedSizeGroup = "PNotationColumn"
                + ItemsControl.GetAlternationIndex(container).ToString(CultureInfo.InvariantCulture);
        }

        if (QLook.QLookPartFind<Button>(container, "PNotationSelector") is Button selector)
        {
            selector.ToolTip = flagged ? reading.PNotationReadingLabel : null;
            selector.Click -= select;
            selector.Click += select;
        }

        if (QLook.QLookPartFind<Image>(container, "PNotationReadingFlag") is Image flag)
        {
            flag.Source = reading.PNotationReadingFlag;
            flag.Visibility = QLook.QLookVisibleRead(flagged);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PNotationReadingLabel") is TextBlock label)
        {
            label.Text = reading.PNotationReadingLabel;
            label.Visibility = QLook.QLookVisibleRead(!flagged && reading.PNotationReadingLabel.Length > 0);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PNotationReadingPhonetic") is TextBlock phonetic)
        {
            phonetic.Text = reading.PNotationReadingOpener + reading.PNotationReadingText
                + reading.PNotationReadingCloser;
        }
    }
}
