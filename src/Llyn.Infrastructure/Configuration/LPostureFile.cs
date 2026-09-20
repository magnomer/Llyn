using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LPostureFile : LPostureVault
{
    private const string LPostureFileWindow = "window";
    private const string LPostureFileVolume = "volume";
    private const string LPostureFileLayout = "layout";
    private const string LPostureFileLinked = "linked";
    private const string LPostureFileMode = "mode";
    private const string LPostureFileSplit = "split";
    private const string LPostureFileEditor = "Editor";
    private const string LPostureFileDisplay = "Display";
    private const string LPostureFileLeft = "left";
    private const string LPostureFileTop = "top";
    private const string LPostureFileWidth = "width";
    private const string LPostureFileHeight = "height";
    private const string LPostureFileMaximized = "maximized";
    private const string LPostureFileMiddle = "middle";
    private const string LPostureFileOrder = "order";
    private const string LPostureFileFilter = "filter";
    private const double LPostureFileLoudest = 1;

    private readonly LKeep _lPostureFileKeep;

    public LPostureFile(LKeep keep)
    {
        ArgumentNullException.ThrowIfNull(keep);
        _lPostureFileKeep = keep;
    }

    public LPostureState? LPostureRead(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        string? text;
        try
        {
            text = _lPostureFileKeep.LKeepRead(name);
        }
        catch (IOException exception)
        {
            throw new LVaultFault(exception);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw new LVaultFault(exception);
        }

        return text is null ? null : LPostureFileParse(text);
    }

    public void LPostureSave(string name, LPostureState state)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(state);

        try
        {
            _lPostureFileKeep.LKeepSave(name, LPostureFileFormat(state));
        }
        catch (IOException exception)
        {
            throw new LVaultFault(exception);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw new LVaultFault(exception);
        }
    }

    public static LPostureState LPostureFileParse(string? text)
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

            LWindowState? window = root.TryGetProperty(LPostureFileWindow, out JsonElement block)
                ? LPostureWindowRead(block)
                : null;

            IReadOnlyList<LLayout>? layout = root.TryGetProperty(LPostureFileLayout, out JsonElement panels)
                ? LPostureLayoutRead(panels)
                : null;

            bool linked =
                !root.TryGetProperty(LPostureFileLinked, out JsonElement share) ||
                share.ValueKind != JsonValueKind.False;

            string? mode =
                root.TryGetProperty(LPostureFileMode, out JsonElement tab) &&
                tab.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(tab.GetString())
                    ? tab.GetString()
                    : null;

            bool split =
                root.TryGetProperty(LPostureFileSplit, out JsonElement side) &&
                side.ValueKind == JsonValueKind.String &&
                string.Equals(side.GetString(), LPostureFileEditor, StringComparison.Ordinal);

            double volume =
                root.TryGetProperty(LPostureFileVolume, out JsonElement level) &&
                level.ValueKind == JsonValueKind.Number
                    ? Math.Clamp(level.GetDouble(), 0, LPostureFileLoudest)
                    : LPostureFileLoudest;

            return new LPostureState(window, layout, linked, mode, split, volume);
        }
        catch (JsonException)
        {
            return new LPostureState();
        }
    }

    public static string LPostureFileFormat(LPostureState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        Dictionary<string, object> payload = new(StringComparer.Ordinal)
        {
            [LPostureFileVolume] = Math.Clamp(state.LPostureStateVolume, 0, LPostureFileLoudest),
            [LPostureFileLinked] = state.LPostureStateLinked,
            [LPostureFileSplit] = state.LPostureStateSplit ? LPostureFileEditor : LPostureFileDisplay
        };

        if (!string.IsNullOrWhiteSpace(state.LPostureStateMode))
        {
            payload[LPostureFileMode] = state.LPostureStateMode;
        }

        if (state.LPostureStateLayout is { Count: > 0 } layout)
        {
            payload[LPostureFileLayout] = LPostureLayoutCreate(layout);
        }

        if (state.LPostureStateWindow is LWindowState window)
        {
            payload[LPostureFileWindow] = LPostureWindowCreate(window);
        }

        return JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
    }

    private static LWindowState? LPostureWindowRead(JsonElement window)
    {
        if (window.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        double? left = LPostureEdgeResolve(window, LPostureFileLeft);
        double? top = LPostureEdgeResolve(window, LPostureFileTop);
        double? width = LPostureEdgeResolve(window, LPostureFileWidth);
        double? height = LPostureEdgeResolve(window, LPostureFileHeight);

        if (left is null || top is null || width is null || height is null)
        {
            return null;
        }

        bool maximized =
            window.TryGetProperty(LPostureFileMaximized, out JsonElement flag) &&
            flag.ValueKind == JsonValueKind.True;

        return new LWindowState(left.Value, top.Value, width.Value, height.Value, maximized);
    }

    private static Dictionary<string, object> LPostureWindowCreate(LWindowState window)
    {
        return new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [LPostureFileLeft] = window.LWindowStateLeft,
            [LPostureFileTop] = window.LWindowStateTop,
            [LPostureFileWidth] = window.LWindowStateWidth,
            [LPostureFileHeight] = window.LWindowStateHeight,
            [LPostureFileMaximized] = window.LWindowStateMaximized
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

            double? left = LPostureWidthResolve(tab.Value, LPostureFileLeft);
            double? middle = LPostureWidthResolve(tab.Value, LPostureFileMiddle);
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
                widths[LPostureFileLeft] = left;
            }

            if (tab.LLayoutMiddle is double middle)
            {
                widths[LPostureFileMiddle] = middle;
            }

            if (tab.LLayoutOrder is LCatalogOrder order)
            {
                widths[LPostureFileOrder] = LCatalog.LCatalogOrderFormat(order);
            }

            if (tab.LLayoutFilter is LCatalogFilter filter)
            {
                widths[LPostureFileFilter] = LCatalog.LCatalogFilterFormat(filter);
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
        if (!tab.TryGetProperty(LPostureFileOrder, out JsonElement value) || value.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        string? text = value.GetString();
        LCatalogOrder parsed = LCatalog.LCatalogOrderParse(text, LCatalogOrder.LCatalogOrderName);

        return string.Equals(LCatalog.LCatalogOrderFormat(parsed), text, StringComparison.Ordinal) ? parsed : null;
    }

    private static LCatalogFilter? LPostureFilterResolve(JsonElement tab)
    {
        if (!tab.TryGetProperty(LPostureFileFilter, out JsonElement value) || value.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        return LCatalog.LCatalogFilterParse(value.GetString());
    }
}
