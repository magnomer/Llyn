using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PYunjingItem : INotifyPropertyChanged
{
    private bool _pYunjingItemChosen;

    internal PYunjingItem(CDiwei row, bool chosen)
    {
        _pYunjingItemChosen = chosen;
        PYunjingItemId = row.CDiweiId;
        PYunjingItemKey = row.CDiweiKey;
        PYunjingItemCount = row.CDiweiCount;
        PYunjingItemFinal = row.CDiweiFinal;
    }

    public long PYunjingItemId { get; }

    public string PYunjingItemKey { get; }

    public int PYunjingItemCount { get; }

    public bool PYunjingItemFinal { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PYunjingItemChosen
    {
        get => _pYunjingItemChosen;

        set
        {
            if (_pYunjingItemChosen == value)
            {
                return;
            }

            _pYunjingItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PYunjingItemChosen)));
        }
    }

    internal static IReadOnlyList<PYunjingItem> PYunjingItemBuild(IReadOnlyList<CDiwei> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<PYunjingItem> built = new(rows.Count);
        foreach (CDiwei row in rows)
        {
            built.Add(new PYunjingItem(row, row.CDiweiChosen));
        }

        return built;
    }

    internal static bool PYunjingItemMatch(PYunjingItem held, PYunjingItem fresh)
    {
        return held.PYunjingItemId == fresh.PYunjingItemId
            && string.Equals(held.PYunjingItemKey, fresh.PYunjingItemKey, StringComparison.Ordinal)
            && held.PYunjingItemCount == fresh.PYunjingItemCount
            && held.PYunjingItemFinal == fresh.PYunjingItemFinal;
    }

    internal static void PYunjingItemSync(PYunjingItem held, PYunjingItem fresh)
    {
        held.PYunjingItemChosen = fresh.PYunjingItemChosen;
    }

    internal static void PYunjingItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PYunjingItem yunjing)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PYunjingRow") is Button row)
        {
            if (yunjing.PYunjingItemChosen)
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
            key.Text = yunjing.PYunjingItemKey;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PYunjingCount") is TextBlock count)
        {
            count.Text = yunjing.PYunjingItemCount.ToString(CultureInfo.InvariantCulture);
        }
    }
}
