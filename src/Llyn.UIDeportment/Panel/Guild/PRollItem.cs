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
        PRollItemMark = PIcon.PIconResolve(
            PLook.PLookFirstRead(row.LCatalogAuthorStored.LAuthorStored, "guild", "unlink"), 16);
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
                LVita.LVitaWorkFormat(row.LCatalogAuthorWork, PLocalizationCatalog.PLocalizationTextRead),
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

        if (PLook.PLookPartFind<Button>(container, "PRollRow") is Button row)
        {
            if (roll.PRollItemChosen)
            {
                row.Tag = "Chosen";
            }
            else
            {
                row.ClearValue(FrameworkElement.TagProperty);
            }
        }

        if (PLook.PLookPartFind<PIconImage>(container, "PRollMark") is PIconImage mark)
        {
            mark.PIconSource = roll.PRollItemMark;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PRollName") is TextBlock name)
        {
            name.Text = roll.PRollItemName;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PRollWork") is TextBlock work)
        {
            work.Text = roll.PRollItemWork;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PRollCount") is TextBlock count)
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
