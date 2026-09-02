using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;
using Llyn.ShellEngine;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace Llyn.UIShell;

internal static class PLangcodeIndicator
{
    private static readonly Dictionary<string, ImageSource?> PLangcodeIndicatorStore = [];

    internal static DrawingImage? PLangcodeIndicatorResolve(string path)
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

    internal static async Task PLangcodeIndicatorLoad(LEngine engine)
    {
        foreach (string language in engine.LEngineLanguageRead())
        {
            if (PLangcodeIndicatorStore.ContainsKey(language))
            {
                continue;
            }

            string? path;
            try
            {
                path = await engine.LEngineFlagRead(language, CancellationToken.None);
            }
            catch (Exception)
            {
                path = null;
            }

            PLangcodeIndicatorStore[language] = path is not null && File.Exists(path)
                ? PLangcodeIndicatorResolve(path)
                : null;
        }
    }

    internal static ImageSource? PLangcodeIndicatorFind(string language)
    {
        return PLangcodeIndicatorStore.TryGetValue(language, out ImageSource? flag) ? flag : null;
    }
}
