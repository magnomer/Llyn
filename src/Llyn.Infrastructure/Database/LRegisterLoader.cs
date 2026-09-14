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
        if (!LLanguageLoader.LLanguageNameValidate(language) || !File.Exists(path))
        {
            return [];
        }

        try
        {
            using FileStream stream = File.OpenRead(path);
            using JsonDocument document = JsonDocument.Parse(stream);
            return LRegisterPackRead(document.RootElement);
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return [];
        }
    }

    private static IReadOnlyList<LRegister> LRegisterPackRead(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object
            || !root.TryGetProperty(LRegisterLoaderSection, out JsonElement registers)
            || registers.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        List<LRegister> values = [];
        HashSet<string> named = new(StringComparer.Ordinal);
        foreach (JsonElement register in registers.EnumerateArray())
        {
            string? name = LRegisterTextRead(register);
            if (name is null || !named.Add(name))
            {
                continue;
            }

            values.Add(new LRegister(0, LStateValue.LStateValueRead(name), true));
        }

        return values;
    }

    private static string? LRegisterTextRead(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        string text = element.GetString()?.Trim() ?? string.Empty;
        return text.Length == 0 ? null : text;
    }
}
