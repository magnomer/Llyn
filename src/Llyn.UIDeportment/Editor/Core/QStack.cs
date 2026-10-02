using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Llyn.UIDeportment;

internal sealed class QStack
{
    private const QLookCue QStackSelected = QLookCue.QLookCueSelected;

    private const QLookCue QStackIdle = QLookCue.QLookCueIdle;

    private const double QStackSlide = 180;

    private readonly FrameworkElement _qStackSurface;

    internal QStack(FrameworkElement surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qStackSurface = surface;
        QStackMeaning.SetValue(QLook.QLookCueProperty, QStackSelected);
        QStackCollocation.SetValue(QLook.QLookCueProperty, QStackIdle);
        QStackEtymology.SetValue(QLook.QLookCueProperty, QStackIdle);
        QStackNote.SetValue(QLook.QLookCueProperty, QStackIdle);
        QStackFrame.Loaded += QStackPillRefine;
        QStackTabs.SizeChanged += QStackPillRefine;
        QStackMeaning.Click += QStackRefine;
        QStackCollocation.Click += QStackRefine;
        QStackEtymology.Click += QStackRefine;
        QStackNote.Click += QStackRefine;
    }

    private Grid QStackFrame => QContract.QContractFind<Grid>(_qStackSurface, "PStack");

    private Border QStackPill => QContract.QContractFind<Border>(_qStackSurface, "PStackPill");

    private TranslateTransform QStackPillOffset =>
        QContract.QContractFind<TranslateTransform>(_qStackSurface, "PStackPillOffset");

    private StackPanel QStackTabs => QContract.QContractFind<StackPanel>(_qStackSurface, "PStackTabs");

    private Button QStackMeaning => QContract.QContractFind<Button>(_qStackSurface, "PStackMeaning");

    private Button QStackCollocation => QContract.QContractFind<Button>(_qStackSurface, "PStackCollocation");

    private Button QStackEtymology => QContract.QContractFind<Button>(_qStackSurface, "PStackEtymology");

    private Button QStackNote => QContract.QContractFind<Button>(_qStackSurface, "PStackNote");

    private StackPanel QMeaning => QContract.QContractFind<StackPanel>(_qStackSurface, "PMeaning");

    private StackPanel QCollocation => QContract.QContractFind<StackPanel>(_qStackSurface, "PCollocation");

    private FrameworkElement QEtymologyField =>
        QContract.QContractFind<FrameworkElement>(_qStackSurface, "PEtymologyField");

    private Border QNote => QContract.QContractFind<Border>(_qStackSurface, "PNote");

    private void QStackRefine(object sender, RoutedEventArgs e)
    {
        if (sender is not Button selectedButton)
        {
            return;
        }

        (Button QStackButton, FrameworkElement QStackContents)[] tabs =
        [
            (QStackMeaning, QMeaning),
            (QStackCollocation, QCollocation),
            (QStackEtymology, QEtymologyField),
            (QStackNote, QNote)
        ];

        foreach ((Button button, FrameworkElement contents) in tabs)
        {
            bool isSelected = button == selectedButton;
            button.SetValue(QLook.QLookCueProperty, isSelected ? QStackSelected : QStackIdle);
            contents.Visibility = isSelected ? Visibility.Visible : Visibility.Collapsed;
        }

        QStackPillPlace(true);
    }

    private void QStackPillRefine(object sender, RoutedEventArgs e)
    {
        QStackPillPlace(false);
    }

    private void QStackPillPlace(bool glide)
    {
        Button? selected = QStackTabs.Children
            .OfType<Button>()
            .FirstOrDefault(button => Equals(button.GetValue(QLook.QLookCueProperty), QStackSelected));

        if (selected is null || selected.ActualWidth <= 0)
        {
            return;
        }

        double reach = selected.TranslatePoint(new Point(0, 0), QStackTabs).X;

        if (!glide)
        {
            QStackPill.BeginAnimation(FrameworkElement.WidthProperty, null);
            QStackPillOffset.BeginAnimation(TranslateTransform.XProperty, null);
            QStackPill.Width = selected.ActualWidth;
            QStackPillOffset.X = reach;
            return;
        }

        var span = new Duration(TimeSpan.FromMilliseconds(QStackSlide));
        var ease = new CubicEase { EasingMode = EasingMode.EaseOut };

        QStackPill.BeginAnimation(
            FrameworkElement.WidthProperty,
            new DoubleAnimation(selected.ActualWidth, span) { EasingFunction = ease });

        QStackPillOffset.BeginAnimation(
            TranslateTransform.XProperty,
            new DoubleAnimation(reach, span) { EasingFunction = ease });
    }
}
