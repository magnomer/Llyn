using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QGroveItem : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs QGroveItemMark = new(nameof(QGroveItemChosen));

    private QGroveItem(CStem row, bool chosen)
    {
        QGroveItemId = row.CStemId;
        QGroveItemKey = row.CStemKey;
        QGroveItemCount = row.CStemCount;
        QGroveItemChosen = chosen;
    }

    public long QGroveItemId { get; }

    public string QGroveItemKey { get; }

    public int QGroveItemCount { get; }

    public bool QGroveItemChosen { get; private set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal static IReadOnlyList<QGroveItem> QGroveItemBuild(IReadOnlyList<CStem> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return QSplice.QSpliceBuild(rows, row => new QGroveItem(row, row.CStemChosen));
    }

    internal static bool QGroveItemMatch(QGroveItem held, QGroveItem fresh)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(fresh);

        return held.QGroveItemId == fresh.QGroveItemId
            && string.Equals(held.QGroveItemKey, fresh.QGroveItemKey, StringComparison.Ordinal)
            && held.QGroveItemCount == fresh.QGroveItemCount;
    }

    internal static void QGroveItemSync(QGroveItem held, QGroveItem fresh)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(fresh);

        held.QGroveItemChosen = fresh.QGroveItemChosen;
        held.PropertyChanged?.Invoke(held, QGroveItemMark);
    }

    internal static void QGroveItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QGroveItem grove)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PGroveRow") is Button row)
        {
            if (grove.QGroveItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PGroveKey") is TextBlock key)
        {
            key.Text = grove.QGroveItemKey;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PGroveCount") is TextBlock count)
        {
            count.Text = grove.QGroveItemCount.ToString(CultureInfo.InvariantCulture);
        }
    }
}
