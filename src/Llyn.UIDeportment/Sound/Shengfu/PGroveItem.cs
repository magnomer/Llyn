using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PGroveItem : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs PGroveItemMark = new(nameof(PGroveItemChosen));

    private PGroveItem(LStem row, bool chosen)
    {
        PGroveItemRow = row;
        PGroveItemId = row.LStemId;
        PGroveItemKey = row.LStemKey;
        PGroveItemCount = row.LStemCount;
        PGroveItemChosen = chosen;
    }

    public long PGroveItemId { get; }

    public string PGroveItemKey { get; }

    public int PGroveItemCount { get; }

    public bool PGroveItemChosen { get; private set; }

    internal LStem PGroveItemRow { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal static IReadOnlyList<PGroveItem> PGroveItemBuild(IReadOnlyList<LStem> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return LSplice.LSpliceBuild(rows, row => new PGroveItem(row, row.LStemChosen));
    }

    internal static bool PGroveItemMatch(PGroveItem held, PGroveItem fresh)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(fresh);

        return held.PGroveItemRow.LStemMatch(fresh.PGroveItemRow);
    }

    internal static void PGroveItemSync(PGroveItem held, PGroveItem fresh)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(fresh);

        held.PGroveItemChosen = fresh.PGroveItemChosen;
        held.PropertyChanged?.Invoke(held, PGroveItemMark);
    }

    internal static void PGroveItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PGroveItem grove)
        {
            return;
        }

        if (PLook.PLookPartFind<Button>(container, "PGroveRow") is Button row)
        {
            if (grove.PGroveItemChosen)
            {
                row.Tag = "Chosen";
            }
            else
            {
                row.ClearValue(FrameworkElement.TagProperty);
            }
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PGroveKey") is TextBlock key)
        {
            key.Text = grove.PGroveItemKey;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PGroveCount") is TextBlock count)
        {
            count.Text = grove.PGroveItemCount.ToString(CultureInfo.InvariantCulture);
        }
    }
}
