using System;
using System.IO;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LMarkupFile : LMarkupVault
{
    public const long LMarkupFileCeiling = 64L * 1024 * 1024;

    public string LMarkupRead(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (stream.Length > LMarkupFileCeiling)
        {
            throw new LRefusal(LRefusal.LRefusalMarkup);
        }

        using StreamReader reader = new(stream, Encoding.UTF8, true);
        return reader.ReadToEnd();
    }

    public void LMarkupSave(string path, string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(text);

        File.WriteAllText(path, text, new UTF8Encoding(false));
    }
}
