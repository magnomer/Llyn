using System.Windows;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

internal sealed class PMedia
{
    public static readonly DependencyProperty PMediaProperty = DependencyProperty.RegisterAttached(
        "PMedia",
        typeof(PMedia),
        typeof(PMedia),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

    private readonly LWindow _lWindow;

    private PMedia(LWindow window)
    {
        _lWindow = window;
    }

    internal static void PMediaAttach(DependencyObject root, LWindow window)
    {
        root.SetValue(PMediaProperty, new PMedia(window));
    }

    internal static PMedia? PMediaRead(DependencyObject element)
    {
        return element.GetValue(PMediaProperty) as PMedia;
    }

    internal PImage PMediaImageCreate(LImageDraft draft)
    {
        return new PImage(_lWindow, draft);
    }

    internal PVideo PMediaVideoCreate(LVideoDraft draft)
    {
        return new PVideo(_lWindow, draft);
    }
}
