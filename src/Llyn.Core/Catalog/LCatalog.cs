using System;

namespace Llyn.Core;

public static class LCatalog
{
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
            _ => fallback,
        };
    }

    public static bool LCatalogTextMatch(string? text, string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        return query.Length == 0
            || (text is not null
                && text.Length > 0
                && text.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0);
    }
}
