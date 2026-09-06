using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PShelfItem
{
    internal PShelfItem(
        LReference reference,
        IReadOnlyList<LAuthor> credits,
        int usage,
        string unreadable,
        string unset)
    {
        PShelfItemId = reference.LReferenceId;
        PShelfItemName = PCitationItem.PCitationItemCreate(reference).PCitationItemName;
        PShelfItemAuthor = PShelfCreditRead(reference, credits, unreadable, unset);
        PShelfItemYear = PShelfValueRead(reference.LReferenceYear, unreadable) ?? unset;
        PShelfItemCount = usage.ToString(CultureInfo.CurrentCulture);
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
