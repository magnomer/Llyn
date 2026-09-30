using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QClipItem
{
    private QClipItem(CClipItem row)
    {
        List<QClipReading> readings = new(row.CClipItemReading.Count);
        foreach (CClipReading reading in row.CClipItemReading)
        {
            readings.Add(new QClipReading(
                reading,
                QAccentItem.QAccentLabelRefine(reading.CClipReadingVariety),
                QAccentItem.QAccentEnsignRefine(reading.CClipReadingVariety, reading.CClipReadingFlagged),
                QLocalizationCatalog.QLocalizationTextRead(reading.CClipReadingAction)));
        }

        QClipItemSource = row.CClipItemSource;
        QClipItemReading = readings;
        QClipItemNotice = QLocalizationCatalog.QLocalizationTextRead(row.CClipItemNotice);
        QClipItemReady = row.CClipItemReady;
    }

    public string QClipItemSource { get; }

    public IReadOnlyList<QClipReading> QClipItemReading { get; }

    public string QClipItemNotice { get; }

    public bool QClipItemReady { get; }

    internal static IReadOnlyList<QClipItem> QClipItemBuild(IReadOnlyList<CClipItem> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<QClipItem> built = new(rows.Count);
        foreach (CClipItem row in rows)
        {
            built.Add(new QClipItem(row));
        }

        return built;
    }
}
