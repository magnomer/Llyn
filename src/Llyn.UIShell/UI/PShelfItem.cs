using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PShelfItem : INotifyPropertyChanged
{
    private bool _pShelfItemChosen;

    internal PShelfItem(LCatalogReference row, string unknown, string unset, bool chosen)
    {
        _pShelfItemChosen = chosen;
        LReference reference = row.LCatalogReferenceStored;

        PShelfItemId = reference.LReferenceId;
        PShelfItemName = row.LCatalogReferenceName;
        PShelfItemAuthor = PShelfCreditRead(reference, row.LCatalogReferenceCredit, unknown, unset);
        PShelfItemYear = PShelfValueRead(reference.LReferenceYear, unknown) ?? unset;
        PShelfItemCount = row.LCatalogReferenceUsage.ToString(CultureInfo.CurrentCulture);
    }

    public long PShelfItemId { get; }

    public string PShelfItemName { get; }

    public string PShelfItemAuthor { get; }

    public string PShelfItemYear { get; }

    public string PShelfItemCount { get; }

    internal static string PShelfCreditRead(
        LReference reference,
        IReadOnlyList<LAuthor> credits,
        string unknown,
        string unset)
    {
        if (credits.Count > 0)
        {
            string[] names = new string[credits.Count];
            for (int index = 0; index < credits.Count; index++)
            {
                names[index] = credits[index].LAuthorName;
            }

            return string.Join(", ", names);
        }

        return PStateConverter.PStateConverterCheck(reference.LReferenceAuthorState) ? unknown : unset;
    }

    private static string? PShelfValueRead(LStateValue value, string unknown)
    {
        return PStateConverter.PStateConverterCheck(value)
            ? unknown
            : value.LStateValueShow() is { Length: > 0 } shown ? shown : null;
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
