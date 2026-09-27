using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PShelfItem : INotifyPropertyChanged
{
    private bool _pShelfItemChosen;

    internal PShelfItem(CCatalogReference row, string unknown, string unset, bool chosen)
    {
        _pShelfItemChosen = chosen;

        PShelfItemId = row.CCatalogReferenceId;
        PShelfItemName = row.CCatalogReferenceName;
        PShelfItemAuthor = PShelfValueRead(row.CCatalogReferenceCredit, unknown) ?? unset;
        PShelfItemYear = PShelfValueRead(row.CCatalogReferenceYear, unknown) ?? unset;
        PShelfItemCount = row.CCatalogReferenceUsage.ToString(CultureInfo.CurrentCulture);
    }

    public long PShelfItemId { get; }

    public string PShelfItemName { get; }

    public string PShelfItemAuthor { get; }

    public string PShelfItemYear { get; }

    public string PShelfItemCount { get; }

    internal static IReadOnlyList<PShelfItem> PShelfItemBuild(IReadOnlyList<CCatalogReference> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        string unknown = QLocalizationCatalog.QLocalizationTextRead("Display.Unknown");
        string unset = QLocalizationCatalog.QLocalizationTextRead("Source.Unset");

        List<PShelfItem> built = new(rows.Count);
        foreach (CCatalogReference row in rows)
        {
            built.Add(new PShelfItem(row, unknown, unset, row.CCatalogReferenceChosen));
        }

        return built;
    }

    private static string? PShelfValueRead(CStateValue value, string unknown)
    {
        return value.CStateValueUncertain ? unknown : value.CStateValueShown;
    }

    internal static bool PShelfItemMatch(PShelfItem held, PShelfItem fresh)
    {
        return held.PShelfItemId == fresh.PShelfItemId
            && string.Equals(held.PShelfItemName, fresh.PShelfItemName, StringComparison.Ordinal)
            && string.Equals(held.PShelfItemAuthor, fresh.PShelfItemAuthor, StringComparison.Ordinal)
            && string.Equals(held.PShelfItemYear, fresh.PShelfItemYear, StringComparison.Ordinal)
            && string.Equals(held.PShelfItemCount, fresh.PShelfItemCount, StringComparison.Ordinal);
    }

    internal static void PShelfItemSync(PShelfItem held, PShelfItem fresh)
    {
        held.PShelfItemChosen = fresh.PShelfItemChosen;
    }

    internal static void PShelfItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PShelfItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PShelfRow") is Button button)
        {
            if (row.PShelfItemChosen)
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
            name.Text = row.PShelfItemName;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PShelfAuthor") is TextBlock author)
        {
            author.Text = row.PShelfItemAuthor;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PShelfYear") is TextBlock year)
        {
            year.Text = row.PShelfItemYear;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PShelfCount") is TextBlock count)
        {
            count.Text = row.PShelfItemCount;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PShelfItemChosen
    {
        get => _pShelfItemChosen;

        set
        {
            if (_pShelfItemChosen == value)
            {
                return;
            }

            _pShelfItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PShelfItemChosen)));
        }
    }
}
