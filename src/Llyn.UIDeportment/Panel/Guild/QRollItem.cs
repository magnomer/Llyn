using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QRollItem : INotifyPropertyChanged
{
    private bool _qRollItemChosen;

    internal QRollItem(CCatalogAuthor row, bool chosen)
    {
        _qRollItemChosen = chosen;
        QRollItemId = row.CCatalogAuthorId;
        QRollItemName = row.CCatalogAuthorName;
        QRollItemWork = row.CCatalogAuthorWork;
        QRollItemCount = row.CCatalogAuthorUsage.ToString(CultureInfo.CurrentCulture);
        QRollItemMark = QIcon.QIconResolve(QLook.QLookFirstRead(row.CCatalogAuthorStored, "guild", "unlink"), 16);
    }

    public long QRollItemId { get; }

    public string QRollItemName { get; }

    public string QRollItemWork { get; }

    public string QRollItemCount { get; }

    public ImageSource QRollItemMark { get; }

    internal static IReadOnlyList<QRollItem> QRollItemBuild(IReadOnlyList<CCatalogAuthor> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<QRollItem> built = new(rows.Count);
        foreach (CCatalogAuthor row in rows)
        {
            built.Add(new QRollItem(row, row.CCatalogAuthorChosen));
        }

        return built;
    }

    internal static bool QRollItemMatch(QRollItem held, QRollItem fresh)
    {
        return held.QRollItemId == fresh.QRollItemId
            && string.Equals(held.QRollItemName, fresh.QRollItemName, StringComparison.Ordinal)
            && string.Equals(held.QRollItemWork, fresh.QRollItemWork, StringComparison.Ordinal)
            && string.Equals(held.QRollItemCount, fresh.QRollItemCount, StringComparison.Ordinal);
    }

    internal static void QRollItemSync(QRollItem held, QRollItem fresh)
    {
        held.QRollItemChosen = fresh.QRollItemChosen;
    }

    internal static void QRollItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QRollItem roll)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PRollRow") is Button row)
        {
            if (roll.QRollItemChosen)
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
            mark.QIconSource = roll.QRollItemMark;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PRollName") is TextBlock name)
        {
            name.Text = roll.QRollItemName;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PRollWork") is TextBlock work)
        {
            work.Text = roll.QRollItemWork;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PRollCount") is TextBlock count)
        {
            count.Text = roll.QRollItemCount;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool QRollItemChosen
    {
        get => _qRollItemChosen;

        set
        {
            if (_qRollItemChosen == value)
            {
                return;
            }

            _qRollItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QRollItemChosen)));
        }
    }
}
