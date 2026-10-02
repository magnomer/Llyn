using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Llyn.Infrastructure;

internal static class LEnvelope
{
    private const string LEnvelopePayload = "form";
    private const string LEnvelopeHeader = "headers";

    public static IReadOnlyDictionary<string, string> LEnvelopePayloadRead(JsonElement row)
    {
        Dictionary<string, string> form = new(StringComparer.Ordinal);
        if (!row.TryGetProperty(LEnvelopePayload, out JsonElement fields)
            || fields.ValueKind != JsonValueKind.Object)
        {
            return form;
        }

        foreach (JsonProperty field in fields.EnumerateObject())
        {
            if (field.Value.ValueKind == JsonValueKind.String)
            {
                form[field.Name] = field.Value.GetString()!;
            }
        }

        return form;
    }

    public static IReadOnlyDictionary<string, string>? LEnvelopeHeaderRead(JsonElement row)
    {
        if (!row.TryGetProperty(LEnvelopeHeader, out JsonElement headers) || headers.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        Dictionary<string, string> map = new(StringComparer.Ordinal);
        foreach (JsonProperty header in headers.EnumerateObject())
        {
            if (header.Value.ValueKind == JsonValueKind.String)
            {
                map[header.Name] = header.Value.GetString()!;
            }
        }

        return map.Count == 0 ? null : map;
    }
}
