using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LLayoutLoader
{
    private const string LLayoutLoaderLeft = "left";
    private const string LLayoutLoaderMiddle = "middle";
    private const string LLayoutLoaderOrder = "order";
    private const string LLayoutLoaderFilter = "filter";

    public static IReadOnlyList<LLayout> LLayoutLoaderRead(JsonElement layout)
    {
        if (layout.ValueKind != JsonValueKind.Object)
        {
            return [];
        }

        List<LLayout> list = [];

        foreach (JsonProperty tab in layout.EnumerateObject())
        {
            if (string.IsNullOrWhiteSpace(tab.Name) || tab.Value.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            double? left = LLayoutLoaderResolve(tab.Value, LLayoutLoaderLeft);
            double? middle = LLayoutLoaderResolve(tab.Value, LLayoutLoaderMiddle);
            LCatalogOrder? order = LLayoutOrderResolve(tab.Value);
            LCatalogFilter? filter = LLayoutFilterResolve(tab.Value);

            if (left is null && middle is null && order is null && filter is null)
            {
                continue;
            }

            list.Add(new LLayout(tab.Name, left, middle, order, filter));
        }

        return list;
    }

    public static Dictionary<string, object> LLayoutLoaderCreate(IReadOnlyList<LLayout> layout)
    {
        ArgumentNullException.ThrowIfNull(layout);

        Dictionary<string, object> block = new(StringComparer.Ordinal);

        foreach (LLayout tab in layout)
        {
            Dictionary<string, object> widths = new(StringComparer.Ordinal);

            if (tab.LLayoutLeft is double left)
            {
                widths[LLayoutLoaderLeft] = left;
            }

            if (tab.LLayoutMiddle is double middle)
            {
                widths[LLayoutLoaderMiddle] = middle;
            }

            if (tab.LLayoutOrder is LCatalogOrder order)
            {
                widths[LLayoutLoaderOrder] = LCatalog.LCatalogOrderFormat(order);
            }

            if (tab.LLayoutFilter is LCatalogFilter filter)
            {
                widths[LLayoutLoaderFilter] = LCatalog.LCatalogFilterFormat(filter);
            }

            if (widths.Count > 0)
            {
                block[tab.LLayoutTab] = widths;
            }
        }

        return block;
    }

    private static double? LLayoutLoaderResolve(JsonElement tab, string name)
    {
        if (!tab.TryGetProperty(name, out JsonElement value))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out double number))
        {
            return double.IsFinite(number) && number >= 0 ? number : null;
        }

        if (value.ValueKind == JsonValueKind.String &&
            double.TryParse(value.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed))
        {
            return double.IsFinite(parsed) && parsed >= 0 ? parsed : null;
        }

        return null;
    }

    private static LCatalogOrder? LLayoutOrderResolve(JsonElement tab)
    {
        if (!tab.TryGetProperty(LLayoutLoaderOrder, out JsonElement value) || value.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        string? text = value.GetString();
        LCatalogOrder parsed = LCatalog.LCatalogOrderParse(text, LCatalogOrder.LCatalogOrderName);

        return string.Equals(LCatalog.LCatalogOrderFormat(parsed), text, StringComparison.Ordinal) ? parsed : null;
    }

    private static LCatalogFilter? LLayoutFilterResolve(JsonElement tab)
    {
        if (!tab.TryGetProperty(LLayoutLoaderFilter, out JsonElement value) || value.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        return LCatalog.LCatalogFilterParse(value.GetString());
    }
}
