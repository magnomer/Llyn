using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private const string PStackSelected = "Selected";

    private const string PStackIdle = "Idle";

    private const double PStackSlide = 180;

    private Grid PStack => (Grid)FindName(nameof(PStack));

    private Border PStackPill => (Border)FindName(nameof(PStackPill));

    private TranslateTransform PStackPillOffset => (TranslateTransform)FindName(nameof(PStackPillOffset));

    private StackPanel PStackTabs => (StackPanel)FindName(nameof(PStackTabs));

    private Button PStackMeaning => (Button)FindName(nameof(PStackMeaning));

    private Button PStackCollocation => (Button)FindName(nameof(PStackCollocation));

    private Button PStackEtymology => (Button)FindName(nameof(PStackEtymology));

    private Button PStackNote => (Button)FindName(nameof(PStackNote));

    private StackPanel PMeaning => (StackPanel)FindName(nameof(PMeaning));

    private StackPanel PCollocation => (StackPanel)FindName(nameof(PCollocation));

    private Border PNote => (Border)FindName(nameof(PNote));

    private void PStackAttach()
    {
        PStackMeaning.Tag = PStackSelected;
        PStackCollocation.Tag = PStackIdle;
        PStackEtymology.Tag = PStackIdle;
        PStackNote.Tag = PStackIdle;
        PStack.Loaded += PStackPillHandle;
        PStackTabs.SizeChanged += PStackSizeHandle;
        PStackMeaning.Click += PStackHandle;
        PStackCollocation.Click += PStackHandle;
        PStackEtymology.Click += PStackHandle;
        PStackNote.Click += PStackHandle;
    }

    private void PStackHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not Button selectedButton)
        {
            return;
        }

        (Button PStackButton, FrameworkElement PStackContents)[] tabs =
        [
            (PStackMeaning, PMeaning),
            (PStackCollocation, PCollocation),
            (PStackEtymology, PEtymologyField),
            (PStackNote, PNote)
        ];

        foreach ((Button button, FrameworkElement contents) in tabs)
        {
            bool isSelected = button == selectedButton;
            button.Tag = isSelected ? PStackSelected : PStackIdle;
            contents.Visibility = isSelected ? Visibility.Visible : Visibility.Collapsed;
        }

        PStackPillPlace(true);
    }

    private void PStackPillHandle(object sender, RoutedEventArgs e)
    {
        PStackPillPlace(false);
    }

    private void PStackSizeHandle(object sender, SizeChangedEventArgs e)
    {
        PStackPillPlace(false);
    }

    private void PStackPillPlace(bool glide)
    {
        Button? selected = PStackTabs.Children
            .OfType<Button>()
            .FirstOrDefault(button => Equals(button.Tag, PStackSelected));

        if (selected is null || selected.ActualWidth <= 0)
        {
            return;
        }

        double reach = selected.TranslatePoint(new Point(0, 0), PStackTabs).X;

        if (!glide)
        {
            PStackPill.BeginAnimation(WidthProperty, null);
            PStackPillOffset.BeginAnimation(TranslateTransform.XProperty, null);
            PStackPill.Width = selected.ActualWidth;
            PStackPillOffset.X = reach;
            return;
        }

        var span = new Duration(TimeSpan.FromMilliseconds(PStackSlide));
        var ease = new CubicEase { EasingMode = EasingMode.EaseOut };

        PStackPill.BeginAnimation(
            WidthProperty,
            new DoubleAnimation(selected.ActualWidth, span) { EasingFunction = ease });

        PStackPillOffset.BeginAnimation(
            TranslateTransform.XProperty,
            new DoubleAnimation(reach, span) { EasingFunction = ease });
    }
}
