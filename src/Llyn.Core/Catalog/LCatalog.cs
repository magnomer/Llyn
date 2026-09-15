using System;

namespace Llyn.Core;

public static class LCatalog
{
    private const char LCatalogFilterSeparator = ',';

    private static readonly char[] LCatalogTextWildcard = ['*', '?'];

    public static string LCatalogOrderFormat(LCatalogOrder order)
    {
        return order switch
        {
            LCatalogOrder.LCatalogOrderHeadword => "Headword",
            LCatalogOrder.LCatalogOrderReverse => "Reverse",
            LCatalogOrder.LCatalogOrderRecent => "Recent",
            LCatalogOrder.LCatalogOrderEarliest => "Earliest",
            LCatalogOrder.LCatalogOrderYear => "Year",
            LCatalogOrder.LCatalogOrderAuthor => "Author",
            LCatalogOrder.LCatalogOrderUsage => "Usage",
            LCatalogOrder.LCatalogOrderLanguage => "Language",
            LCatalogOrder.LCatalogOrderMarked => "Marked",
            LCatalogOrder.LCatalogOrderText => "Text",
            LCatalogOrder.LCatalogOrderSource => "Source",
            LCatalogOrder.LCatalogOrderKind => "Kind",
            LCatalogOrder.LCatalogOrderSound => "Sound",
            LCatalogOrder.LCatalogOrderPending => "Pending",
            LCatalogOrder.LCatalogOrderWork => "Work",
            _ => "Name",
        };
    }

    public static LCatalogOrder LCatalogOrderParse(string? text, LCatalogOrder fallback)
    {
        return text switch
        {
            "Name" => LCatalogOrder.LCatalogOrderName,
            "Headword" => LCatalogOrder.LCatalogOrderHeadword,
            "Reverse" => LCatalogOrder.LCatalogOrderReverse,
            "Recent" => LCatalogOrder.LCatalogOrderRecent,
            "Earliest" => LCatalogOrder.LCatalogOrderEarliest,
            "Year" => LCatalogOrder.LCatalogOrderYear,
            "Author" => LCatalogOrder.LCatalogOrderAuthor,
            "Usage" => LCatalogOrder.LCatalogOrderUsage,
            "Language" => LCatalogOrder.LCatalogOrderLanguage,
            "Marked" => LCatalogOrder.LCatalogOrderMarked,
            "Text" => LCatalogOrder.LCatalogOrderText,
            "Source" => LCatalogOrder.LCatalogOrderSource,
            "Kind" => LCatalogOrder.LCatalogOrderKind,
            "Sound" => LCatalogOrder.LCatalogOrderSound,
            "Pending" => LCatalogOrder.LCatalogOrderPending,
            "Work" => LCatalogOrder.LCatalogOrderWork,
            _ => fallback,
        };
    }

    public static string LCatalogFilterFormat(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        return string.Join(LCatalogFilterSeparator, filter.LCatalogFilterHidden);
    }

    public static LCatalogFilter LCatalogFilterParse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return LCatalogFilter.LCatalogFilterEmpty;
        }

        return new LCatalogFilter(
            text.Split(
                LCatalogFilterSeparator,
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }

    public static string LCatalogTextNormalize(string? text)
    {
        return (text ?? string.Empty).Trim().ToLowerInvariant();
    }

    public static bool LCatalogTextMatch(string? text, string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.Length == 0)
        {
            return true;
        }

        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        string folded = text.ToLowerInvariant();
        string pattern = query.ToLowerInvariant();
        if (pattern.IndexOfAny(LCatalogTextWildcard) < 0)
        {
            return folded.Contains(pattern, StringComparison.Ordinal);
        }

        int at = 0;
        int step = 0;
        int star = -1;
        int mark = 0;
        while (at < folded.Length)
        {
            if (step < pattern.Length && (pattern[step] == '?' || pattern[step] == folded[at]))
            {
                at++;
                step++;
            }
            else if (step < pattern.Length && pattern[step] == '*')
            {
                star = step++;
                mark = at;
            }
            else if (star >= 0)
            {
                step = star + 1;
                at = ++mark;
            }
            else
            {
                return false;
            }
        }

        while (step < pattern.Length && pattern[step] == '*')
        {
            step++;
        }

        return step == pattern.Length;
    }
}
