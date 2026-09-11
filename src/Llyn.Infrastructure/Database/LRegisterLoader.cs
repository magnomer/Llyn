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
            string? key = LRegisterTextRead(register, "id");
            if (key is null)
            {
                continue;
            }

            values.Add(new LRegister(
                0,
                LStateValue.LStateValueRead(LRegisterTextRead(register, "name") ?? key),
                language,
                true,
                key));
        }

        return values;
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
