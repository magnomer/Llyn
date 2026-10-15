using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QYunjingItem : INotifyPropertyChanged
{
    private bool _qYunjingItemChosen;

    internal QYunjingItem(CDiwei row, bool chosen)
    {
        _qYunjingItemChosen = chosen;
        QYunjingItemId = row.CDiweiId;
        QYunjingItemKey = row.CDiweiKey;
        QYunjingItemCount = row.CDiweiCount;
        QYunjingItemFinal = row.CDiweiFinal;
    }

    public long QYunjingItemId { get; }

    public string QYunjingItemKey { get; }

    public int QYunjingItemCount { get; }

    public bool QYunjingItemFinal { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool QYunjingItemChosen
    {
        get => _qYunjingItemChosen;

        set
        {
            if (_qYunjingItemChosen == value)
            {
                return;
            }

            _qYunjingItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QYunjingItemChosen)));
        }
    }

    internal static IReadOnlyList<QYunjingItem> QYunjingItemBuild(IReadOnlyList<CDiwei> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<QYunjingItem> built = new(rows.Count);
        foreach (CDiwei row in rows)
        {
            built.Add(new QYunjingItem(row, row.CDiweiChosen));
        }

        return built;
    }

    internal static bool QYunjingItemMatch(QYunjingItem held, QYunjingItem fresh)
    {
        return held.QYunjingItemId == fresh.QYunjingItemId
            && string.Equals(held.QYunjingItemKey, fresh.QYunjingItemKey, StringComparison.Ordinal)
            && held.QYunjingItemCount == fresh.QYunjingItemCount
            && held.QYunjingItemFinal == fresh.QYunjingItemFinal;
    }

    internal static void QYunjingItemSync(QYunjingItem held, QYunjingItem fresh)
    {
        held.QYunjingItemChosen = fresh.QYunjingItemChosen;
    }

    internal static void QYunjingItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QYunjingItem yunjing)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PYunjingRow") is Button row)
        {
            if (yunjing.QYunjingItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PYunjingKey") is TextBlock key)
        {
            key.Text = yunjing.QYunjingItemKey;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PYunjingCount") is TextBlock count)
        {
            count.Text = yunjing.QYunjingItemCount.ToString(CultureInfo.InvariantCulture);
        }
    }
}
