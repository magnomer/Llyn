using System.Collections.Generic;
using System.Text.Json;

namespace Llyn.Infrastructure;

internal static class LPack
{
    public static string? LPackTextRead(JsonElement element, string key)
    {
        return element.TryGetProperty(key, out JsonElement value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }

    public static int LPackNumberRead(JsonElement element, string key)
    {
        return element.TryGetProperty(key, out JsonElement value)
            && value.ValueKind == JsonValueKind.Number
            && value.TryGetInt32(out int number)
            ? number
            : 0;
    }

    public static double LPackMeasureRead(JsonElement element, string key)
    {
        return element.TryGetProperty(key, out JsonElement value)
            && value.ValueKind == JsonValueKind.Number
            && value.TryGetDouble(out double measure)
            && measure > 0
            ? measure
            : 0;
    }

    public static bool LPackBooleanRead(JsonElement element, string key)
    {
        return element.TryGetProperty(key, out JsonElement value) && value.ValueKind == JsonValueKind.True;
    }

    public static IReadOnlyList<string> LPackTextScan(JsonElement row, string key)
    {
        List<string> names = [];
        if (!row.TryGetProperty(key, out JsonElement value))
        {
            return names;
        }

        IEnumerable<JsonElement> elements = value.ValueKind switch
        {
            JsonValueKind.String => [value],
            JsonValueKind.Array => value.EnumerateArray(),
            _ => [],
        };
        foreach (JsonElement element in elements)
        {
            string trimmed = element.ValueKind == JsonValueKind.String ? element.GetString()!.Trim() : string.Empty;
            if (trimmed.Length > 0)
            {
                names.Add(trimmed);
            }
        }

        return names;
    }
}
