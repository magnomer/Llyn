using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal static class QFrequencyLabel
{
    private const string QFrequencyEmpty = "Empty";

    private const char QFrequencyStar = '✦';

    internal static void QFrequencyChipRefine(
        UIElement section, FrameworkElement chip, TextBlock name, TextBlock band, CFrequency? frequency)
    {
        if (frequency is null)
        {
            name.Text = string.Empty;
            chip.ToolTip = null;
            section.Visibility = Visibility.Collapsed;
            return;
        }

        QFrequencyLabelRefine(name, band, frequency);
        chip.ToolTip = frequency.CFrequencySource;
        section.Visibility = Visibility.Visible;
    }

    private static void QFrequencyLabelRefine(TextBlock name, TextBlock band, CFrequency frequency)
    {
        string brush = "Theme.Frequency." + frequency.CFrequencyRank;
        name.Text = QLocalizationCatalog.QLocalizationTextRead(frequency.CFrequencyKey);
        name.SetResourceReference(TextBlock.ForegroundProperty, brush);

        band.Inlines.Clear();
        Run filled = new(new string(QFrequencyStar, frequency.CFrequencyBand));
        filled.SetResourceReference(TextElement.ForegroundProperty, brush);
        band.Inlines.Add(filled);

        Run empty = new(new string(QFrequencyStar, frequency.CFrequencySpare));
        empty.SetResourceReference(TextElement.ForegroundProperty, "Theme.Frequency." + QFrequencyEmpty);
        band.Inlines.Add(empty);

        band.Visibility = QLook.QLookVisibleRead(frequency.CFrequencyRanked);
    }
}
