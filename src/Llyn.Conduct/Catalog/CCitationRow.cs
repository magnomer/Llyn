using System;
using System.Collections.Generic;
using System.Globalization;

namespace Llyn.Conduct;

public sealed record CCitationRow(
    long CCitationRowId,
    string CCitationRowLead,
    string CCitationRowMark,
    string CCitationRowTail,
    string CCitationRowCount)
{
    private const int CCitationRowLimit = 8;

    public static IReadOnlyList<CCitationRow> CCitationRowFind(IReadOnlyList<CCatalogReference> found, string word)
    {
        ArgumentNullException.ThrowIfNull(found);
        ArgumentNullException.ThrowIfNull(word);

        string marked = word.Trim();
        List<CCitationRow> rows = [];
        foreach (CCatalogReference row in found)
        {
            if (rows.Count == CCitationRowLimit)
            {
                break;
            }

            rows.Add(CCitationRowRead(row, marked));
        }

        return rows;
    }

    private static CCitationRow CCitationRowRead(CCatalogReference row, string word)
    {
        string title = row.CCatalogReferenceByline;
        string count = row.CCatalogReferenceUsage > 0
            ? row.CCatalogReferenceUsage.ToString(CultureInfo.CurrentCulture)
            : string.Empty;
        int found = CultureInfo.CurrentCulture.CompareInfo.IndexOf(
            title, word, CompareOptions.IgnoreCase, out int size);

        return found < 0
            ? new CCitationRow(row.CCatalogReferenceId, title, string.Empty, string.Empty, count)
            : new CCitationRow(
                row.CCatalogReferenceId, title[..found], title.Substring(found, size), title[(found + size)..], count);
    }
}
