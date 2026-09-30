using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PNotationItem
{
    internal PNotationItem(CNotationItem row)
    {
        List<PNotationReading> readings = new(row.CNotationItemReading.Count);
        foreach (CNotationReading reading in row.CNotationItemReading)
        {
            readings.Add(new PNotationReading(
                reading.CNotationReadingVariety.CVarietyName,
                QAccentItem.QAccentLabelRefine(reading.CNotationReadingVariety),
                QAccentItem.QAccentEnsignRefine(reading.CNotationReadingVariety, reading.CNotationReadingFlagged),
                reading.CNotationReadingPhonetic,
                reading.CNotationReadingText,
                reading.CNotationReadingMark.CRespellingMarkOpener,
                reading.CNotationReadingMark.CRespellingMarkCloser));
        }

        PNotationItemSource = row.CNotationItemSource;
        PNotationItemReading = readings;
        PNotationItemNotice = QLocalizationCatalog.QLocalizationTextRead(row.CNotationItemNotice);
        PNotationItemReady = row.CNotationItemReady;
    }

    public string PNotationItemSource { get; }

    public IReadOnlyList<PNotationReading> PNotationItemReading { get; }

    public string PNotationItemNotice { get; }

    public bool PNotationItemReady { get; }

    internal static void PNotationItemRefine(FrameworkElement container, object item, RoutedEventHandler select)
    {
        if (item is not PNotationItem notation)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PNotationSource") is TextBlock source)
        {
            source.Text = notation.PNotationItemSource;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PNotationReadingList") is ItemsControl list)
        {
            list.Visibility = QLook.QLookVisibleRead(notation.PNotationItemReady);
            if (!ReferenceEquals(list.ItemsSource, notation.PNotationItemReading))
            {
                list.ItemsSource = notation.PNotationItemReading;
            }

            QLookItem.QLookItemAttach(
                list, (reading, row, _) => PNotationReading.PNotationReadingApply(reading, row, select));
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PNotationNote") is TextBlock note)
        {
            note.Text = notation.PNotationItemNotice;
            note.Visibility = QLook.QLookVisibleRead(!notation.PNotationItemReady);
        }
    }
}
