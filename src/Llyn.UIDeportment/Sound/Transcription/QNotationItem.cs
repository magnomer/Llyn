using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QNotationItem
{
    internal QNotationItem(CNotationItem row)
    {
        List<QNotationReading> readings = new(row.CNotationItemReading.Count);
        foreach (CNotationReading reading in row.CNotationItemReading)
        {
            readings.Add(new QNotationReading(
                reading.CNotationReadingVariety.CVarietyName,
                QAccentItem.QAccentLabelRefine(reading.CNotationReadingVariety),
                QAccentItem.QAccentEnsignRefine(reading.CNotationReadingVariety, reading.CNotationReadingFlagged),
                reading.CNotationReadingPhonetic,
                reading.CNotationReadingText,
                reading.CNotationReadingMark.CRespellingMarkOpener,
                reading.CNotationReadingMark.CRespellingMarkCloser));
        }

        QNotationItemSource = row.CNotationItemSource;
        QNotationItemReading = readings;
        QNotationItemNotice = QLocalizationCatalog.QLocalizationTextRead(row.CNotationItemNotice);
        QNotationItemReady = row.CNotationItemReady;
    }

    public string QNotationItemSource { get; }

    public IReadOnlyList<QNotationReading> QNotationItemReading { get; }

    public string QNotationItemNotice { get; }

    public bool QNotationItemReady { get; }

    internal static void QNotationItemRefine(FrameworkElement container, object item, RoutedEventHandler select)
    {
        if (item is not QNotationItem notation)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PNotationSource") is TextBlock source)
        {
            source.Text = notation.QNotationItemSource;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "QNotationReadingList") is ItemsControl list)
        {
            list.Visibility = QLook.QLookVisibleRead(notation.QNotationItemReady);
            if (!ReferenceEquals(list.ItemsSource, notation.QNotationItemReading))
            {
                list.ItemsSource = notation.QNotationItemReading;
            }

            QLookItem.QLookItemAttach(
                list, (reading, row, _) => QNotationReading.QNotationReadingRefine(reading, row, select));
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PNotationNote") is TextBlock note)
        {
            note.Text = notation.QNotationItemNotice;
            note.Visibility = QLook.QLookVisibleRead(!notation.QNotationItemReady);
        }
    }
}
