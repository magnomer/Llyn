using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LPostureFile : LPostureVault
{
    private const string LPostureFileVolume = "volume";
    private const string LPostureFileLayout = "layout";
    private const string LPostureFileMode = "mode";
    private const string LPostureFileSplit = "split";
    private const string LPostureFileEditor = "Editor";
    private const string LPostureFileDisplay = "Display";
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

            IReadOnlyList<LLayout>? layout = root.TryGetProperty(LPostureFileLayout, out JsonElement panels)
                ? LPostureLayoutRead(panels)
                : null;

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

            return new LPostureState(layout, mode, split, volume);
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

        return JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
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

            LCatalogOrder? order = LPostureOrderResolve(tab.Value);
            LCatalogFilter? filter = LPostureFilterResolve(tab.Value);

            if (order is null && filter is null)
            {
                continue;
            }

            list.Add(new LLayout(tab.Name, order, filter));
        }

        return list;
    }

    private static Dictionary<string, object> LPostureLayoutCreate(IReadOnlyList<LLayout> layout)
    {
        Dictionary<string, object> block = new(StringComparer.Ordinal);

        foreach (LLayout tab in layout)
        {
            Dictionary<string, object> view = new(StringComparer.Ordinal);

            if (tab.LLayoutOrder is LCatalogOrder order)
            {
                view[LPostureFileOrder] = LCatalog.LCatalogOrderFormat(order);
            }

            if (tab.LLayoutFilter is LCatalogFilter filter)
            {
                view[LPostureFileFilter] = LCatalog.LCatalogFilterFormat(filter);
            }

            if (view.Count > 0)
            {
                block[tab.LLayoutTab] = view;
            }
        }

        return block;
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
