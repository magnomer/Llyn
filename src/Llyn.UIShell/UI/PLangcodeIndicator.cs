using System;
using System.IO;
using System.Windows.Media;
using System.Xml;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace Llyn.UIShell;

/// <summary>
/// Resolves a language pack's cached SVG flag into the frozen drawing used by both the editor's
/// language picker and the read-only entry display. A malformed or unreadable flag becomes no image,
/// which lets each surface show its neutral globe fallback.
/// </summary>
internal static class PLangcodeIndicator
{
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
}
