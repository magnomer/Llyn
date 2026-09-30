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
        new FrameworkPropertyMetadata(null, QIconSourceRefine));

    public QIconImage()
    {
        IsEnabledChanged += QIconEnabledRefine;
    }

    public ImageSource? QIconSource
    {
        get => (ImageSource?)GetValue(QIconSourceProperty);
        set => SetValue(QIconSourceProperty, value);
    }

    private void QIconEnabledRefine(object sender, DependencyPropertyChangedEventArgs e)
    {
        QIconImageApply();
    }

    private static void QIconSourceRefine(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ((QIconImage)sender).QIconImageApply();
    }

    private void QIconImageApply()
    {
        Source = QIcon.QIconResolve(null, 0, QIconSource, IsEnabled);
    }
}
