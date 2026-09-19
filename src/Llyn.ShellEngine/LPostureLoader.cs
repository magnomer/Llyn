using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.ShellEngine;

public static class LPostureLoader
{
    private const string LPostureLoaderWindow = "window";
    private const string LPostureLoaderVolume = "volume";
    private const string LPostureLoaderLayout = "layout";
    private const string LPostureLoaderLinked = "linked";
    private const string LPostureLoaderMode = "mode";
    private const string LPostureLoaderSplit = "split";
    private const string LPostureLoaderEditor = "Editor";
    private const string LPostureLoaderDisplay = "Display";
    private const string LPostureLoaderLeft = "left";
    private const string LPostureLoaderTop = "top";
    private const string LPostureLoaderWidth = "width";
    private const string LPostureLoaderHeight = "height";
    private const string LPostureLoaderMaximized = "maximized";
    private const string LPostureLoaderMiddle = "middle";
    private const string LPostureLoaderOrder = "order";
    private const string LPostureLoaderFilter = "filter";
    private const double LPostureLoaderLoudest = 1;

    public static LPostureState LPostureLoaderRead(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return new LPostureState();
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(text);
            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return new LPostureState();
            }

            LWindowState? window = root.TryGetProperty(LPostureLoaderWindow, out JsonElement block)
                ? LPostureWindowRead(block)
                : null;

            IReadOnlyList<LLayout>? layout = root.TryGetProperty(LPostureLoaderLayout, out JsonElement panels)
                ? LPostureLayoutRead(panels)
                : null;

            bool linked =
                !root.TryGetProperty(LPostureLoaderLinked, out JsonElement share) ||
                share.ValueKind != JsonValueKind.False;

            string? mode =
                root.TryGetProperty(LPostureLoaderMode, out JsonElement tab) &&
                tab.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(tab.GetString())
                    ? tab.GetString()
                    : null;

            bool split =
                root.TryGetProperty(LPostureLoaderSplit, out JsonElement side) &&
                side.ValueKind == JsonValueKind.String &&
                string.Equals(side.GetString(), LPostureLoaderEditor, StringComparison.Ordinal);

            double volume =
                root.TryGetProperty(LPostureLoaderVolume, out JsonElement level) &&
                level.ValueKind == JsonValueKind.Number
                    ? Math.Clamp(level.GetDouble(), 0, LPostureLoaderLoudest)
                    : LPostureLoaderLoudest;

            return new LPostureState(window, layout, linked, mode, split, volume);
        }
        catch (JsonException)
        {
            return new LPostureState();
        }
    }

    public static string LPostureLoaderFormat(LPostureState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        Dictionary<string, object> payload = new(StringComparer.Ordinal)
        {
            [LPostureLoaderVolume] = Math.Clamp(state.LPostureStateVolume, 0, LPostureLoaderLoudest),
            [LPostureLoaderLinked] = state.LPostureStateLinked,
            [LPostureLoaderSplit] = state.LPostureStateSplit ? LPostureLoaderEditor : LPostureLoaderDisplay
        };

        if (!string.IsNullOrWhiteSpace(state.LPostureStateMode))
        {
            payload[LPostureLoaderMode] = state.LPostureStateMode;
        }

        if (state.LPostureStateLayout is { Count: > 0 } layout)
        {
            payload[LPostureLoaderLayout] = LPostureLayoutCreate(layout);
        }

        if (state.LPostureStateWindow is LWindowState window)
        {
            payload[LPostureLoaderWindow] = LPostureWindowCreate(window);
        }

        return JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
    }

    private static LWindowState? LPostureWindowRead(JsonElement window)
    {
        if (window.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        double? left = LPostureEdgeResolve(window, LPostureLoaderLeft);
        double? top = LPostureEdgeResolve(window, LPostureLoaderTop);
        double? width = LPostureEdgeResolve(window, LPostureLoaderWidth);
        double? height = LPostureEdgeResolve(window, LPostureLoaderHeight);

        if (left is null || top is null || width is null || height is null)
        {
            return null;
        }

        bool maximized =
            window.TryGetProperty(LPostureLoaderMaximized, out JsonElement flag) &&
            flag.ValueKind == JsonValueKind.True;

        return new LWindowState(left.Value, top.Value, width.Value, height.Value, maximized);
    }

    private static Dictionary<string, object> LPostureWindowCreate(LWindowState window)
    {
        return new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [LPostureLoaderLeft] = window.LWindowStateLeft,
            [LPostureLoaderTop] = window.LWindowStateTop,
            [LPostureLoaderWidth] = window.LWindowStateWidth,
            [LPostureLoaderHeight] = window.LWindowStateHeight,
            [LPostureLoaderMaximized] = window.LWindowStateMaximized
        };
    }

    private static IReadOnlyList<LLayout> LPostureLayoutRead(JsonElement layout)
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

            double? left = LPostureWidthResolve(tab.Value, LPostureLoaderLeft);
            double? middle = LPostureWidthResolve(tab.Value, LPostureLoaderMiddle);
            LCatalogOrder? order = LPostureOrderResolve(tab.Value);
            LCatalogFilter? filter = LPostureFilterResolve(tab.Value);

            if (left is null && middle is null && order is null && filter is null)
            {
                continue;
            }

            list.Add(new LLayout(tab.Name, left, middle, order, filter));
        }

        return list;
    }

    private static Dictionary<string, object> LPostureLayoutCreate(IReadOnlyList<LLayout> layout)
    {
        Dictionary<string, object> block = new(StringComparer.Ordinal);

        foreach (LLayout tab in layout)
        {
            Dictionary<string, object> widths = new(StringComparer.Ordinal);

            if (tab.LLayoutLeft is double left)
            {
                widths[LPostureLoaderLeft] = left;
            }

            if (tab.LLayoutMiddle is double middle)
            {
                widths[LPostureLoaderMiddle] = middle;
            }

            if (tab.LLayoutOrder is LCatalogOrder order)
            {
                widths[LPostureLoaderOrder] = LCatalog.LCatalogOrderFormat(order);
            }

            if (tab.LLayoutFilter is LCatalogFilter filter)
            {
                widths[LPostureLoaderFilter] = LCatalog.LCatalogFilterFormat(filter);
            }

            if (widths.Count > 0)
            {
                block[tab.LLayoutTab] = widths;
            }
        }

        return block;
    }

    private static double? LPostureEdgeResolve(JsonElement window, string name)
    {
        return LPostureNumberResolve(window, name) is double number && double.IsFinite(number) ? number : null;
    }

    private static double? LPostureWidthResolve(JsonElement tab, string name)
    {
        return LPostureNumberResolve(tab, name) is double number && double.IsFinite(number) && number >= 0
            ? number
            : null;
    }

    private static double? LPostureNumberResolve(JsonElement block, string name)
    {
        if (!block.TryGetProperty(name, out JsonElement value))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out double number))
        {
            return number;
        }

        if (value.ValueKind == JsonValueKind.String &&
            double.TryParse(value.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed))
        {
            return parsed;
        }

        return null;
    }

    private static LCatalogOrder? LPostureOrderResolve(JsonElement tab)
    {
        if (!tab.TryGetProperty(LPostureLoaderOrder, out JsonElement value) || value.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        string? text = value.GetString();
        LCatalogOrder parsed = LCatalog.LCatalogOrderParse(text, LCatalogOrder.LCatalogOrderName);

        return string.Equals(LCatalog.LCatalogOrderFormat(parsed), text, StringComparison.Ordinal) ? parsed : null;
    }

    private static LCatalogFilter? LPostureFilterResolve(JsonElement tab)
    {
        if (!tab.TryGetProperty(LPostureLoaderFilter, out JsonElement value) || value.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        return LCatalog.LCatalogFilterParse(value.GetString());
    }
}
