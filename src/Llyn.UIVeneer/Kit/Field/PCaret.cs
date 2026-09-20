using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Llyn.UIVeneer;

internal sealed class PCaret : Adorner
{
    private const double PCaretWidth = 2;

    private const double PCaretCorner = 1;

    private const double PCaretSlide = 70;

    private const double PCaretReach = 240;

    private static readonly DependencyProperty PCaretHeldProperty = DependencyProperty.RegisterAttached(
        "PCaretHeld",
        typeof(PCaret),
        typeof(PCaret));

    private static readonly DependencyProperty PCaretOffsetProperty = DependencyProperty.Register(
        "PCaretOffset",
        typeof(double),
        typeof(PCaret),
        new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    private readonly TextBox _pCaretField;

    private Rect _pCaretShape = Rect.Empty;

    private bool _pCaretBlink;

    internal static void PCaretHook()
    {
        EventManager.RegisterClassHandler(
            typeof(TextBox),
            Keyboard.GotKeyboardFocusEvent,
            new KeyboardFocusChangedEventHandler(PCaretFieldHandle),
            true);
    }

    private PCaret(TextBox field)
        : base(field)
    {
        _pCaretField = field;
        IsHitTestVisible = false;
        Focusable = false;
        field.CaretBrush = Brushes.Transparent;

        field.TextChanged += PCaretTextHandle;
        field.SelectionChanged += PCaretTypeHandle;
        field.LayoutUpdated += PCaretLayoutHandle;
        field.LostKeyboardFocus += PCaretBlurHandle;
        field.Unloaded += PCaretUnloadHandle;
    }

    private static void PCaretFieldHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is not TextBox field || field.GetValue(PCaretHeldProperty) is PCaret)
        {
            return;
        }

        AdornerLayer? layer = AdornerLayer.GetAdornerLayer(field);
        if (layer is null)
        {
            return;
        }

        var caret = new PCaret(field);
        field.SetValue(PCaretHeldProperty, caret);
        layer.Add(caret);
        caret.PCaretUpdate(false);
    }

    private void PCaretTextHandle(object sender, TextChangedEventArgs e)
    {
        PCaretUpdate(true);
    }

    private void PCaretTypeHandle(object sender, RoutedEventArgs e)
    {
        PCaretUpdate(true);
    }

    private void PCaretLayoutHandle(object? sender, EventArgs e)
    {
        PCaretUpdate(false);
    }

    private void PCaretBlurHandle(object sender, RoutedEventArgs e)
    {
        PCaretUpdate(false);
    }

    private void PCaretUnloadHandle(object sender, RoutedEventArgs e)
    {
        _pCaretField.TextChanged -= PCaretTextHandle;
        _pCaretField.SelectionChanged -= PCaretTypeHandle;
        _pCaretField.LayoutUpdated -= PCaretLayoutHandle;
        _pCaretField.LostKeyboardFocus -= PCaretBlurHandle;
        _pCaretField.Unloaded -= PCaretUnloadHandle;

        _pCaretField.ClearValue(PCaretHeldProperty);
        _pCaretField.CaretBrush = null;
        PCaretBlinkStop();
        AdornerLayer.GetAdornerLayer(_pCaretField)?.Remove(this);
    }

    private void PCaretUpdate(bool typed)
    {
        Rect place = PCaretShapeRead();
        if (place.IsEmpty)
        {
            if (Visibility == Visibility.Visible)
            {
                PCaretBlinkStop();
                Visibility = Visibility.Collapsed;
            }

            _pCaretShape = place;
            return;
        }

        bool shown = Visibility == Visibility.Visible && !_pCaretShape.IsEmpty;
        bool moved = shown && !_pCaretShape.Equals(place);
        if (shown && !moved && !typed)
        {
            return;
        }

        bool lined = moved && _pCaretShape.Top.Equals(place.Top);
        double held = _pCaretShape.X;

        _pCaretShape = place;
        Visibility = Visibility.Visible;

        PCaretOffsetApply(lined && Math.Abs(place.X - held) <= PCaretReach ? held - place.X : 0);

        if (typed || moved || !_pCaretBlink)
        {
            PCaretBlinkStart();
        }

        InvalidateVisual();
    }

    private Rect PCaretShapeRead()
    {
        if (!_pCaretField.IsKeyboardFocused ||
            (_pCaretField.IsReadOnly && !_pCaretField.IsReadOnlyCaretVisible) ||
            _pCaretField.ActualWidth <= 0)
        {
            return Rect.Empty;
        }

        Rect place;
        try
        {
            place = _pCaretField.GetRectFromCharacterIndex(_pCaretField.CaretIndex);
        }
        catch (Exception)
        {
            return Rect.Empty;
        }

        if (place.IsEmpty || double.IsInfinity(place.X) || double.IsNaN(place.X) || place.Height <= 0)
        {
            return Rect.Empty;
        }

        if (place.Bottom < 0 || place.Top > _pCaretField.ActualHeight || place.X > _pCaretField.ActualWidth)
        {
            return Rect.Empty;
        }

        return place;
    }

    private void PCaretOffsetApply(double offset)
    {
        if (offset.Equals(0d))
        {
            BeginAnimation(PCaretOffsetProperty, null);
            SetValue(PCaretOffsetProperty, 0d);
            return;
        }

        var slide = new DoubleAnimation(offset, 0, new Duration(TimeSpan.FromMilliseconds(PCaretSlide)))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        BeginAnimation(PCaretOffsetProperty, slide);
    }

    private void PCaretBlinkStart()
    {
        var blink = new DoubleAnimationUsingKeyFrames { RepeatBehavior = RepeatBehavior.Forever };
        blink.KeyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.Zero)));
        blink.KeyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(520))));
        blink.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(660))));
        blink.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1000))));
        blink.KeyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1140))));

        _pCaretBlink = true;
        BeginAnimation(OpacityProperty, blink);
    }

    private void PCaretBlinkStop()
    {
        _pCaretBlink = false;
        BeginAnimation(OpacityProperty, null);
        Opacity = 1;
    }

    protected override void OnRender(DrawingContext drawing)
    {
        if (_pCaretShape.IsEmpty)
        {
            return;
        }

        double width = Math.Max(PCaretWidth, SystemParameters.CaretWidth);
        double left = _pCaretShape.X + (double)GetValue(PCaretOffsetProperty);
        Brush face = _pCaretField.TryFindResource("Theme.Accent") as Brush ?? Brushes.Black;

        drawing.DrawRoundedRectangle(
            face,
            null,
            new Rect(left, _pCaretShape.Top, width, _pCaretShape.Height),
            PCaretCorner,
            PCaretCorner);
    }
}
