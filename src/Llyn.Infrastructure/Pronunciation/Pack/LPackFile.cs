using System;
using System.IO;
using System.Text.Json;

namespace Llyn.Infrastructure;

internal static class LPackFile
{
    public const string LPackFileFolder = "languages";

    public static JsonElement? LPackFileLoad(string language, string file)
    {
        string name = file.Trim();
        if (name.Length == 0 || name != Path.GetFileName(name))
        {
            return null;
        }

        string path = Path.Combine(AppContext.BaseDirectory, LPackFileFolder, language, name);
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            using FileStream stream = File.OpenRead(path);
            using JsonDocument document = JsonDocument.Parse(stream);
            return document.RootElement.Clone();
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return null;
        }
    }
}
