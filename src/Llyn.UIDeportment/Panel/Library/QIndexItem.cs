using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QIndexItem : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs QIndexItemMark = new(nameof(QIndexItemChosen));

    private QIndexItem(CVistaRow row, ImageSource? flag, bool chosen)
    {
        QIndexItemRow = row;
        QIndexItemId = row.CVistaRowId;
        QIndexItemName = row.CVistaRowName;
        QIndexItemEpithet = row.CVistaRowEpithet;
        QIndexItemLanguage = row.CVistaRowLanguage;
        QIndexItemFlag = flag;
        QIndexItemChosen = chosen;
    }

    public long QIndexItemId { get; }

    public string QIndexItemName { get; }

    public string QIndexItemEpithet { get; }

    public string QIndexItemLanguage { get; }

    public ImageSource? QIndexItemFlag { get; }

    public bool QIndexItemChosen { get; private set; }

    private CVistaRow QIndexItemRow { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal static IReadOnlyList<QIndexItem> QIndexItemBuild(IReadOnlyList<CVistaRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return QSplice.QSpliceBuild(
            rows,
            row => new QIndexItem(row, QEnsignImage.QEnsignRead(row.CVistaRowLanguage), row.CVistaRowChosen));
    }

    internal static bool QIndexItemMatch(QIndexItem held, QIndexItem fresh)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(fresh);

        CVistaRow kept = held.QIndexItemRow;
        CVistaRow read = fresh.QIndexItemRow;
        return kept.CVistaRowId == read.CVistaRowId
            && string.Equals(kept.CVistaRowHeadword, read.CVistaRowHeadword, StringComparison.Ordinal)
            && string.Equals(kept.CVistaRowEpithet, read.CVistaRowEpithet, StringComparison.Ordinal)
            && string.Equals(kept.CVistaRowName, read.CVistaRowName, StringComparison.Ordinal)
            && string.Equals(kept.CVistaRowLanguage, read.CVistaRowLanguage, StringComparison.Ordinal);
    }

    internal static void QIndexItemSync(QIndexItem held, QIndexItem fresh)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(fresh);

        if (held.QIndexItemChosen == fresh.QIndexItemChosen)
        {
            return;
        }

        held.QIndexItemChosen = fresh.QIndexItemChosen;
        held.PropertyChanged?.Invoke(held, QIndexItemMark);
    }

    internal static void QIndexItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QIndexItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PIndexRow") is Button button)
        {
            if (row.QIndexItemChosen)
            {
                button.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                button.ClearValue(QLook.QLookCueProperty);
            }
        }

        if (QLook.QLookPartFind<Image>(container, "PIndexFlag") is Image flag)
        {
            flag.Source = row.QIndexItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "PIndexName") is Run name)
        {
            name.Text = row.QIndexItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PIndexEpithet") is Run epithet)
        {
            epithet.Text = QLook.QLookEpithetRead(row.QIndexItemEpithet);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PIndexLanguage") is TextBlock language)
        {
            language.Text = row.QIndexItemLanguage;
        }
    }
}
