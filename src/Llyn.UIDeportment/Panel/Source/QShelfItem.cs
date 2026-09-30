using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QShelfItem : INotifyPropertyChanged
{
    private bool _qShelfItemChosen;

    internal QShelfItem(CCatalogReference row, bool chosen)
    {
        _qShelfItemChosen = chosen;

        QShelfItemId = row.CCatalogReferenceId;
        QShelfItemName = row.CCatalogReferenceName;
        QShelfItemAuthor = row.CCatalogReferenceCredit.CStateWordingKey is string author
            ? QLocalizationCatalog.QLocalizationTextRead(author)
            : row.CCatalogReferenceCredit.CStateWordingText;
        QShelfItemYear = row.CCatalogReferenceYear.CStateWordingKey is string year
            ? QLocalizationCatalog.QLocalizationTextRead(year)
            : row.CCatalogReferenceYear.CStateWordingText;
        QShelfItemCount = row.CCatalogReferenceUsage.ToString(CultureInfo.CurrentCulture);
    }

    public long QShelfItemId { get; }

    public string QShelfItemName { get; }

    public string QShelfItemAuthor { get; }

    public string QShelfItemYear { get; }

    public string QShelfItemCount { get; }

    internal static IReadOnlyList<QShelfItem> QShelfItemBuild(IReadOnlyList<CCatalogReference> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<QShelfItem> built = new(rows.Count);
        foreach (CCatalogReference row in rows)
        {
            built.Add(new QShelfItem(row, row.CCatalogReferenceChosen));
        }

        return built;
    }

    internal static bool QShelfItemMatch(QShelfItem held, QShelfItem fresh)
    {
        return held.QShelfItemId == fresh.QShelfItemId
            && string.Equals(held.QShelfItemName, fresh.QShelfItemName, StringComparison.Ordinal)
            && string.Equals(held.QShelfItemAuthor, fresh.QShelfItemAuthor, StringComparison.Ordinal)
            && string.Equals(held.QShelfItemYear, fresh.QShelfItemYear, StringComparison.Ordinal)
            && string.Equals(held.QShelfItemCount, fresh.QShelfItemCount, StringComparison.Ordinal);
    }

    internal static void QShelfItemSync(QShelfItem held, QShelfItem fresh)
    {
        held.QShelfItemChosen = fresh.QShelfItemChosen;
    }

    internal static void QShelfItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QShelfItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PShelfRow") is Button button)
        {
            if (row.QShelfItemChosen)
            {
                button.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                button.ClearValue(QLook.QLookCueProperty);
            }
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PShelfIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("source", 16);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PShelfName") is TextBlock name)
        {
            name.Text = row.QShelfItemName;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PShelfAuthor") is TextBlock author)
        {
            author.Text = row.QShelfItemAuthor;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PShelfYear") is TextBlock year)
        {
            year.Text = row.QShelfItemYear;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PShelfCount") is TextBlock count)
        {
            count.Text = row.QShelfItemCount;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool QShelfItemChosen
    {
        get => _qShelfItemChosen;

        set
        {
            if (_qShelfItemChosen == value)
            {
                return;
            }

            _qShelfItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QShelfItemChosen)));
        }
    }
}
