using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Llyn.UIVeneer;

public sealed class PTab : Button
{
    public static readonly DependencyProperty PTabOutlineProperty = DependencyProperty.Register(
        nameof(PTabOutline),
        typeof(Geometry),
        typeof(PTab));

    public static readonly DependencyProperty PTabFilledProperty = DependencyProperty.Register(
        nameof(PTabFilled),
        typeof(Geometry),
        typeof(PTab));

    public static readonly DependencyProperty PTabChosenProperty = DependencyProperty.Register(
        nameof(PTabChosen),
        typeof(bool),
        typeof(PTab),
        new FrameworkPropertyMetadata(false));

    public Geometry? PTabOutline
    {
        get => (Geometry?)GetValue(PTabOutlineProperty);
        set => SetValue(PTabOutlineProperty, value);
    }

    public Geometry? PTabFilled
    {
        get => (Geometry?)GetValue(PTabFilledProperty);
        set => SetValue(PTabFilledProperty, value);
    }

    public bool PTabChosen
    {
        get => (bool)GetValue(PTabChosenProperty);
        set => SetValue(PTabChosenProperty, value);
    }
}
