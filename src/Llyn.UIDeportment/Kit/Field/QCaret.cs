using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Llyn.UIDeportment;

internal sealed class QCaret : Adorner
{
    private const double QCaretWidth = 2;

    private const double QCaretCorner = 1;

    private const double QCaretSlide = 70;

    private const double QCaretReach = 240;

    private static readonly DependencyProperty QCaretHeldProperty = DependencyProperty.RegisterAttached(
        "QCaretHeld",
        typeof(QCaret),
        typeof(QCaret));

    private static readonly DependencyProperty QCaretOffsetProperty = DependencyProperty.Register(
        "QCaretOffset",
        typeof(double),
        typeof(QCaret),
        new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    private readonly TextBox _qCaretField;

    private Rect _qCaretShape = Rect.Empty;

    private bool _qCaretBlink;

    internal static void QCaretHook()
    {
        EventManager.RegisterClassHandler(
            typeof(TextBox),
            Keyboard.GotKeyboardFocusEvent,
            new KeyboardFocusChangedEventHandler(QCaretFieldHandle),
            true);
    }

    internal static bool QCaretKeyApply(
        string key, int caret, int length, int selection, Action<int> remove, Func<int, bool> move, Action place)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(remove);
        ArgumentNullException.ThrowIfNull(move);
        ArgumentNullException.ThrowIfNull(place);

        if (selection != 0)
        {
            return false;
        }

        if (key == "Back" && caret == 0)
        {
            remove(-1);
            return true;
        }

        if (key == "Delete" && caret == length)
        {
            remove(1);
            return true;
        }

        int step = key == "Left" ? -1 : key == "Right" ? 1 : 0;
        if (step == 0 || length > 0 || !move(step))
        {
            return false;
        }

        place();
        return true;
    }

    private QCaret(TextBox field)
        : base(field)
    {
        _qCaretField = field;
        IsHitTestVisible = false;
        Focusable = false;
        field.CaretBrush = Brushes.Transparent;

        field.TextChanged += QCaretTextHandle;
        field.SelectionChanged += QCaretTypeHandle;
        field.LayoutUpdated += QCaretLayoutHandle;
        field.LostKeyboardFocus += QCaretBlurHandle;
        field.Unloaded += QCaretUnloadHandle;
    }

    private static void QCaretFieldHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is not TextBox field || field.GetValue(QCaretHeldProperty) is QCaret)
        {
            return;
        }

        AdornerLayer? layer = AdornerLayer.GetAdornerLayer(field);
        if (layer is null)
        {
            return;
        }

        var caret = new QCaret(field);
        field.SetValue(QCaretHeldProperty, caret);
        layer.Add(caret);
        caret.QCaretUpdate(false);
    }

    private void QCaretTextHandle(object sender, TextChangedEventArgs e)
    {
        QCaretUpdate(true);
    }

    private void QCaretTypeHandle(object sender, RoutedEventArgs e)
    {
        QCaretUpdate(true);
    }

    private void QCaretLayoutHandle(object? sender, EventArgs e)
    {
        QCaretUpdate(false);
    }

    private void QCaretBlurHandle(object sender, RoutedEventArgs e)
    {
        QCaretUpdate(false);
    }

    private void QCaretUnloadHandle(object sender, RoutedEventArgs e)
    {
        _qCaretField.TextChanged -= QCaretTextHandle;
        _qCaretField.SelectionChanged -= QCaretTypeHandle;
        _qCaretField.LayoutUpdated -= QCaretLayoutHandle;
        _qCaretField.LostKeyboardFocus -= QCaretBlurHandle;
        _qCaretField.Unloaded -= QCaretUnloadHandle;

        _qCaretField.ClearValue(QCaretHeldProperty);
        _qCaretField.CaretBrush = null;
        QCaretBlinkStop();
        AdornerLayer.GetAdornerLayer(_qCaretField)?.Remove(this);
    }

    private void QCaretUpdate(bool typed)
    {
        Rect place = QCaretShapeRead();
        if (place.IsEmpty)
        {
            if (Visibility == Visibility.Visible)
            {
                QCaretBlinkStop();
                Visibility = Visibility.Collapsed;
            }

            _qCaretShape = place;
            return;
        }

        bool shown = Visibility == Visibility.Visible && !_qCaretShape.IsEmpty;
        bool moved = shown && !_qCaretShape.Equals(place);
        if (shown && !moved && !typed)
        {
            return;
        }

        bool lined = moved && _qCaretShape.Top.Equals(place.Top);
        double held = _qCaretShape.X;

        _qCaretShape = place;
        Visibility = Visibility.Visible;

        QCaretOffsetApply(lined && Math.Abs(place.X - held) <= QCaretReach ? held - place.X : 0);

        if (typed || moved || !_qCaretBlink)
        {
            QCaretBlinkStart();
        }

        InvalidateVisual();
    }

    private Rect QCaretShapeRead()
    {
        if (!_qCaretField.IsKeyboardFocused ||
            (_qCaretField.IsReadOnly && !_qCaretField.IsReadOnlyCaretVisible) ||
            _qCaretField.ActualWidth <= 0)
        {
            return Rect.Empty;
        }

        Rect place;
        try
        {
            place = _qCaretField.GetRectFromCharacterIndex(_qCaretField.CaretIndex);
        }
        catch (Exception)
        {
            return Rect.Empty;
        }

        if (place.IsEmpty || double.IsInfinity(place.X) || double.IsNaN(place.X) || place.Height <= 0)
        {
            return Rect.Empty;
        }

        if (place.Bottom < 0 || place.Top > _qCaretField.ActualHeight || place.X > _qCaretField.ActualWidth)
        {
            return Rect.Empty;
        }

        return place;
    }

    private void QCaretOffsetApply(double offset)
    {
        if (offset.Equals(0d))
        {
            BeginAnimation(QCaretOffsetProperty, null);
            SetValue(QCaretOffsetProperty, 0d);
            return;
        }

        var slide = new DoubleAnimation(offset, 0, new Duration(TimeSpan.FromMilliseconds(QCaretSlide)))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        BeginAnimation(QCaretOffsetProperty, slide);
    }

    private void QCaretBlinkStart()
    {
        var blink = new DoubleAnimationUsingKeyFrames { RepeatBehavior = RepeatBehavior.Forever };
        blink.KeyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.Zero)));
        blink.KeyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(520))));
        blink.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(660))));
        blink.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1000))));
        blink.KeyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1140))));

        _qCaretBlink = true;
        BeginAnimation(OpacityProperty, blink);
    }

    private void QCaretBlinkStop()
    {
        _qCaretBlink = false;
        BeginAnimation(OpacityProperty, null);
        Opacity = 1;
    }

    protected override void OnRender(DrawingContext drawing)
    {
        if (_qCaretShape.IsEmpty)
        {
            return;
        }

        double width = Math.Max(QCaretWidth, SystemParameters.CaretWidth);
        double left = _qCaretShape.X + (double)GetValue(QCaretOffsetProperty);
        Brush face = _qCaretField.TryFindResource("Theme.Accent") as Brush ?? Brushes.Black;

        drawing.DrawRoundedRectangle(
            face,
            null,
            new Rect(left, _qCaretShape.Top, width, _qCaretShape.Height),
            QCaretCorner,
            QCaretCorner);
    }
}
