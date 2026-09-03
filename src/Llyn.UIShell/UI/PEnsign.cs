using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;
using Llyn.ShellEngine;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace Llyn.UIShell;

internal static class PEnsign
{
    private static readonly Dictionary<string, ImageSource?> PEnsignStore = [];

    internal static DrawingImage? PEnsignResolve(string path)
    {
        try
        {
            FileSvgReader reader = new(new WpfDrawingSettings { IncludeRuntime = false, TextAsGeometry = true });
            DrawingGroup drawing = reader.Read(path);
            if (drawing is null)
            {
                return null;
            }

            DrawingImage image = new(drawing);
            image.Freeze();
            return image;
        }
        catch (Exception exception) when (exception is IOException or XmlException or NotSupportedException)
        {
            return null;
        }
    }

    internal static async Task PEnsignLoad(LEngine engine)
    {
        string[] missing = engine.LEngineLanguageRead()
            .Where(language => !PEnsignStore.ContainsKey(language))
            .ToArray();

        if (missing.Length == 0)
        {
            return;
        }

        string?[] paths = await Task.WhenAll(missing.Select(language => PEnsignRead(engine, language)));

        for (int index = 0; index < missing.Length; index++)
        {
            string? path = paths[index];
            PEnsignStore[missing[index]] = path is not null && File.Exists(path)
                ? PEnsignResolve(path)
                : null;
        }
    }

    private static async Task<string?> PEnsignRead(LEngine engine, string language)
    {
        try
        {
            return await engine.LEngineFlagRead(language, CancellationToken.None);
        }
        catch (Exception)
        {
            return null;
        }
    }

    internal static ImageSource? PEnsignFind(string language)
    {
        return PEnsignStore.TryGetValue(language, out ImageSource? flag) ? flag : null;
    }
}
