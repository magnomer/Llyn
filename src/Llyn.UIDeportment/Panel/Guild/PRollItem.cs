using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PRollItem : INotifyPropertyChanged
{
    private bool _pRollItemChosen;

    internal PRollItem(LCatalogAuthor row, string work, bool chosen)
    {
        _pRollItemChosen = chosen;
        PRollItemId = row.LCatalogAuthorStored.LAuthorId;
        PRollItemName = row.LCatalogAuthorName;
        PRollItemWork = work;
        PRollItemCount = row.LCatalogAuthorUsage.ToString(CultureInfo.CurrentCulture);
        PRollItemMark = QIcon.QIconResolve(
            QLook.QLookFirstRead(row.LCatalogAuthorStored.LAuthorStored, "guild", "unlink"), 16);
    }

    public long PRollItemId { get; }

    public string PRollItemName { get; }

    public string PRollItemWork { get; }

    public string PRollItemCount { get; }

    public ImageSource PRollItemMark { get; }

    internal static IReadOnlyList<PRollItem> PRollItemBuild(IReadOnlyList<LCatalogAuthor> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<PRollItem> built = new(rows.Count);
        foreach (LCatalogAuthor row in rows)
        {
            built.Add(new PRollItem(
                row,
                LVita.LVitaWorkFormat(row.LCatalogAuthorWork, QLocalizationCatalog.QLocalizationTextRead),
                row.LCatalogAuthorChosen));
        }

        return built;
    }

    internal static bool PRollItemMatch(PRollItem held, PRollItem fresh)
    {
        return held.PRollItemId == fresh.PRollItemId
            && string.Equals(held.PRollItemName, fresh.PRollItemName, StringComparison.Ordinal)
            && string.Equals(held.PRollItemWork, fresh.PRollItemWork, StringComparison.Ordinal)
            && string.Equals(held.PRollItemCount, fresh.PRollItemCount, StringComparison.Ordinal);
    }

    internal static void PRollItemSync(PRollItem held, PRollItem fresh)
    {
        held.PRollItemChosen = fresh.PRollItemChosen;
    }

    internal static void PRollItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PRollItem roll)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PRollRow") is Button row)
        {
            if (roll.PRollItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PRollMark") is QIconImage mark)
        {
            mark.QIconSource = roll.PRollItemMark;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PRollName") is TextBlock name)
        {
            name.Text = roll.PRollItemName;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PRollWork") is TextBlock work)
        {
            work.Text = roll.PRollItemWork;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PRollCount") is TextBlock count)
        {
            count.Text = roll.PRollItemCount;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PRollItemChosen
    {
        get => _pRollItemChosen;

        set
        {
            if (_pRollItemChosen == value)
            {
                return;
            }

            _pRollItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PRollItemChosen)));
        }
    }
}
