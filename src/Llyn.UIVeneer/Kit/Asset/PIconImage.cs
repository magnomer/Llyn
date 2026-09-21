using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Llyn.UIVeneer;

public sealed class PIconImage : Image
{
    public static readonly DependencyProperty PIconSourceProperty = DependencyProperty.Register(
        nameof(PIconSource),
        typeof(ImageSource),
        typeof(PIconImage),
        new FrameworkPropertyMetadata(null, PIconSourceHandle));

    public PIconImage()
    {
        IsEnabledChanged += PIconEnabledHandle;
    }

    public ImageSource? PIconSource
    {
        get => (ImageSource?)GetValue(PIconSourceProperty);
        set => SetValue(PIconSourceProperty, value);
    }

    private void PIconEnabledHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        PIconImageApply();
    }

    private static void PIconSourceHandle(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ((PIconImage)sender).PIconImageApply();
    }

    private void PIconImageApply()
    {
        Source = PIcon.PIconResolve(null, 0, PIconSource, IsEnabled);
    }
}
