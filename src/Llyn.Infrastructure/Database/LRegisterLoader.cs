using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LRegisterLoader
{
    private const string LRegisterLoaderFolder = "languages";
    private const string LRegisterLoaderFile = "vocabulary.json";
    private const string LRegisterLoaderSection = "registers";

    public static IReadOnlyList<LRegister> LRegisterLoaderLoad(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        string path = Path.Combine(
            AppContext.BaseDirectory, LRegisterLoaderFolder, language, LRegisterLoaderFile);
        if (!File.Exists(path))
        {
            return [];
        }

        try
        {
            using FileStream stream = File.OpenRead(path);
            using JsonDocument document = JsonDocument.Parse(stream);
            return LRegisterPackRead(language, document.RootElement);
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return [];
        }
    }

    private static IReadOnlyList<LRegister> LRegisterPackRead(string language, JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object
            || !root.TryGetProperty(LRegisterLoaderSection, out JsonElement registers)
            || registers.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        List<LRegister> values = [];
        foreach (JsonElement register in registers.EnumerateArray())
        {
            long? id = LRegisterNumberRead(register, "id");
            string? name = LRegisterTextRead(register, "name");
            if (id is null || name is null)
            {
                continue;
            }

            values.Add(new LRegister(0, LStateValue.LStateValueRead(name), language, id));
        }

        return values;
    }

    private static long? LRegisterNumberRead(JsonElement element, string property)
    {
        if (element.ValueKind != JsonValueKind.Object
            || !element.TryGetProperty(property, out JsonElement value)
            || value.ValueKind != JsonValueKind.Number
            || !value.TryGetInt64(out long number)
            || number <= 0)
        {
            return null;
        }

        return number;
    }

    private static string? LRegisterTextRead(JsonElement element, string property)
    {
        if (element.ValueKind != JsonValueKind.Object
            || !element.TryGetProperty(property, out JsonElement value)
            || value.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        string text = value.GetString() ?? string.Empty;
        return string.IsNullOrWhiteSpace(text) ? null : text;
    }
}
