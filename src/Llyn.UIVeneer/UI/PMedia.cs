using System.Windows;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

internal sealed class PMedia
{
    public static readonly DependencyProperty PMediaProperty = DependencyProperty.RegisterAttached(
        "PMedia",
        typeof(PMedia),
        typeof(PMedia),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

    private readonly LEngine _lEngine;

    private PMedia(LEngine engine)
    {
        _lEngine = engine;
    }

    internal static void PMediaAttach(DependencyObject root, LEngine engine)
    {
        root.SetValue(PMediaProperty, new PMedia(engine));
    }

    internal static PMedia? PMediaRead(DependencyObject element)
    {
        return element.GetValue(PMediaProperty) as PMedia;
    }

    internal PImage PMediaImageCreate(LImageDraft draft)
    {
        return new PImage(_lEngine, draft);
    }

    internal PVideo PMediaVideoCreate(LVideoDraft draft)
    {
        return new PVideo(_lEngine, draft);
    }
}
