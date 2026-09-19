using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;

namespace Llyn.UIVeneer;

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

    internal static IReadOnlyList<PShelfItem> PShelfItemBuild(IReadOnlyList<LCatalogReference> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        string unknown = PLocalizationCatalog.PLocalizationTextRead("Display.Unknown");
        string unset = PLocalizationCatalog.PLocalizationTextRead("Source.Unset");

        List<PShelfItem> built = new(rows.Count);
        foreach (LCatalogReference row in rows)
        {
            built.Add(new PShelfItem(row, unknown, unset, row.LCatalogReferenceChosen));
        }

        return built;
    }

    internal static string PShelfKindRead(LReferenceKind kind)
    {
        return kind switch
        {
            LReferenceKind.LReferenceKindUnknown => "Source.KindUnknown",
            LReferenceKind.LReferenceKindBook => "Source.KindBook",
            LReferenceKind.LReferenceKindJournal => "Source.KindJournal",
            LReferenceKind.LReferenceKindArticle => "Source.KindArticle",
            LReferenceKind.LReferenceKindWeb => "Source.KindWeb",
            LReferenceKind.LReferenceKindVideo => "Source.KindVideo",
            LReferenceKind.LReferenceKindAudio => "Source.KindAudio",
            LReferenceKind.LReferenceKindPicture => "Source.KindPicture",
            LReferenceKind.LReferenceKindOther => "Source.KindOther",
            _ => "Source.KindUnspecified",
        };
    }

    internal static string PShelfCreditRead(
        LReference reference,
        IReadOnlyList<LAuthor> credits,
        string unknown,
        string unset)
    {
        return reference.LReferenceCreditRead(credits)
            ?? (reference.LReferenceAuthorState.LStateMarkUncertain ? unknown : unset);
    }

    private static string? PShelfValueRead(LStateValue value, string unknown)
    {
        return value.LStateValueUncertain ? unknown : value.LStateValueShown;
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
