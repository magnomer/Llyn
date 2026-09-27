using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QKindredItem : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs QKindredItemMark = new(nameof(QKindredItemChosen));

    private QKindredItem(CVistaRow row, bool chosen)
    {
        QKindredItemId = row.CVistaRowId;
        QKindredItemHeadword = row.CVistaRowHeadword;
        QKindredItemName = row.CVistaRowName;
        QKindredItemEpithet = row.CVistaRowEpithet;
        QKindredItemLanguage = row.CVistaRowLanguage;
        QKindredItemFlag = LEnsignImage.LEnsignFind(row.CVistaRowLanguage);
        QKindredItemChosen = chosen;
    }

    public long QKindredItemId { get; }

    public string QKindredItemHeadword { get; }

    public string QKindredItemEpithet { get; }

    public string QKindredItemName { get; }

    public string QKindredItemLanguage { get; }

    public ImageSource? QKindredItemFlag { get; }

    public bool QKindredItemChosen { get; private set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal static IReadOnlyList<QKindredItem> QKindredItemBuild(IReadOnlyList<CVistaRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return LSplice.LSpliceBuild(rows, row => new QKindredItem(row, row.CVistaRowChosen));
    }

    internal static bool QKindredItemMatch(QKindredItem held, QKindredItem fresh)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(fresh);

        return held.QKindredItemId == fresh.QKindredItemId
            && string.Equals(held.QKindredItemHeadword, fresh.QKindredItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.QKindredItemEpithet, fresh.QKindredItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.QKindredItemName, fresh.QKindredItemName, StringComparison.Ordinal)
            && string.Equals(held.QKindredItemLanguage, fresh.QKindredItemLanguage, StringComparison.Ordinal);
    }

    internal static void QKindredItemSync(QKindredItem held, QKindredItem fresh)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(fresh);

        held.QKindredItemChosen = fresh.QKindredItemChosen;
        held.PropertyChanged?.Invoke(held, QKindredItemMark);
    }

    internal static void QKindredItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QKindredItem kindred)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PKindredRow") is Button row)
        {
            if (kindred.QKindredItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }
        }

        if (QLook.QLookPartFind<Image>(container, "PKindredFlag") is Image flag)
        {
            flag.Source = kindred.QKindredItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "PKindredName") is Run name)
        {
            name.Text = kindred.QKindredItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PKindredEpithet") is Run epithet)
        {
            epithet.Text = " " + kindred.QKindredItemEpithet;
        }
    }
}
