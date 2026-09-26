using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Llyn.UIDeportment;

public sealed class PTab : Button
{
    public static readonly DependencyProperty PTabIconProperty = DependencyProperty.Register(
        nameof(PTabIcon),
        typeof(ImageSource),
        typeof(PTab));

    public static readonly DependencyProperty PTabChosenProperty = DependencyProperty.Register(
        nameof(PTabChosen),
        typeof(bool),
        typeof(PTab),
        new FrameworkPropertyMetadata(false));

    public ImageSource? PTabIcon
    {
        get => (ImageSource?)GetValue(PTabIconProperty);
        set => SetValue(PTabIconProperty, value);
    }

    public bool PTabChosen
    {
        get => (bool)GetValue(PTabChosenProperty);
        set => SetValue(PTabChosenProperty, value);
    }
}
