using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Llyn.UIDeportment;

public sealed class QIconImage : Image
{
    public static readonly DependencyProperty QIconSourceProperty = DependencyProperty.Register(
        nameof(QIconSource),
        typeof(ImageSource),
        typeof(QIconImage),
        new FrameworkPropertyMetadata(null, QIconSourceHandle));

    public QIconImage()
    {
        IsEnabledChanged += QIconEnabledHandle;
    }

    public ImageSource? QIconSource
    {
        get => (ImageSource?)GetValue(QIconSourceProperty);
        set => SetValue(QIconSourceProperty, value);
    }

    private void QIconEnabledHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        QIconImageApply();
    }

    private static void QIconSourceHandle(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ((QIconImage)sender).QIconImageApply();
    }

    private void QIconImageApply()
    {
        Source = QIcon.QIconResolve(null, 0, QIconSource, IsEnabled);
    }
}
