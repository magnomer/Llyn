using System;
using System.IO;
using System.Reflection;

namespace Llyn.Infrastructure;

public static class LPortraitIcon
{
    private const string LPortraitIconPrefix = "Llyn.Infrastructure.Icons.";

    public static LPortraitAsset LPortraitIconRead(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Assembly assembly = typeof(LPortraitIcon).Assembly;
        using Stream stream = assembly.GetManifestResourceStream(LPortraitIconPrefix + name + ".svg")
            ?? throw new ArgumentException("No icon is embedded under this name.", nameof(name));
        using MemoryStream data = new MemoryStream();
        stream.CopyTo(data);

        return new LPortraitAsset(data.ToArray(), "image/svg+xml", ".svg");
    }
}
