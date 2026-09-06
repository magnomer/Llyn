using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PShelfItem
{
    internal PShelfItem(LCatalogReference row, string unreadable, string unset)
    {
        LReference reference = row.LCatalogReferenceStored;

        PShelfItemId = reference.LReferenceId;
        PShelfItemName = row.LCatalogReferenceName;
        PShelfItemAuthor = PShelfCreditRead(reference, row.LCatalogReferenceCredit, unreadable, unset);
        PShelfItemYear = PShelfValueRead(reference.LReferenceYear, unreadable) ?? unset;
        PShelfItemCount = row.LCatalogReferenceUsage.ToString(CultureInfo.CurrentCulture);
    }

    public string PShelfItemId { get; }

    public string PShelfItemName { get; }

    public string PShelfItemAuthor { get; }

    public string PShelfItemYear { get; }

    public string PShelfItemCount { get; }

    internal static string PShelfCreditRead(
        LReference reference,
        IReadOnlyList<LAuthor> credits,
        string unreadable,
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

        return reference.LReferenceAuthorState == LState.LStateUnknown ? unreadable : unset;
    }

    private static string? PShelfValueRead(LStateValue value, string unreadable)
    {
        return value.LStateValueState switch
        {
            LState.LStateSpecified => value.LStateValueShow(),
            LState.LStateUnknown => unreadable,
            _ => null,
        };
    }
}
