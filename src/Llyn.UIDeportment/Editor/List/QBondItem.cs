using System;
using System.Collections.Generic;
using System.Windows;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QBondItem
{
    private const double QBondItemStep = 16;

    internal QBondItem(long card, CMeaning sense)
    {
        ArgumentNullException.ThrowIfNull(sense);

        QBondItemCard = card;
        QBondItemSense = sense.CMeaningId;
        QBondItemPath = sense.CMeaningPath;
        QBondItemName = sense.CMeaningName;
        QBondItemIndent = new Thickness(sense.CMeaningDepth * QBondItemStep, 0, 0, 0);
    }

    public long QBondItemCard { get; }

    public long QBondItemSense { get; }

    public string QBondItemPath { get; }

    public string QBondItemName { get; }

    public Thickness QBondItemIndent { get; }

    internal static IReadOnlyList<QBondItem> QBondItemCreate(long card, IReadOnlyList<CMeaning> senses)
    {
        List<QBondItem> rows = [];
        foreach (CMeaning sense in senses)
        {
            rows.Add(new QBondItem(card, sense));
        }

        return rows;
    }
}
