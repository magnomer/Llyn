using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Llyn.UIDeportment;

public sealed class QSwath : FrameworkElement
{
    internal const double QSwathRowSlack = 4;

    private const double QSwathScrollStep = 24;

    private readonly QSwathText _qSwathText;

    private ScrollViewer _qSwathViewer = null!;

    private Point? _qSwathPress;

    private QSwathSeam? _qSwathAnchor;

    private QSwathSeam? _qSwathHead;

    private QSwathSeam? _qSwathTail;

    public QSwath()
    {
        _qSwathText = new QSwathText(this);
        IsHitTestVisible = false;
        Focusable = true;
        FocusVisualStyle = null;
        CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, QSwathCopyRefine, QSwathCopyCheck));
        CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, QSwathAllRefine));
    }

    internal void QSwathAttach(ScrollViewer viewer)
    {
        _qSwathViewer = viewer;
        viewer.PreviewMouseLeftButtonDown += QSwathPressRefine;
        viewer.PreviewMouseMove += QSwathMoveRefine;
        viewer.PreviewMouseLeftButtonUp += QSwathReleaseRefine;
        viewer.LostMouseCapture += QSwathLostRefine;
        viewer.AddHandler(QueryCursorEvent, new QueryCursorEventHandler(QSwathCursorRefine));
    }

    internal void QSwathClear()
    {
        _qSwathPress = null;
        _qSwathAnchor = null;
        _qSwathHead = null;
        _qSwathTail = null;
        _qSwathText.QSwathTextClear();
        InvalidateVisual();
    }

    private void QSwathCopyCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _qSwathHead is not null;
    }

    private void QSwathCopyRefine(object sender, ExecutedRoutedEventArgs e)
    {
        string text = _qSwathText.QSwathTextRead(_qSwathHead, _qSwathTail);
        if (text.Length > 0)
        {
            Clipboard.SetText(text);
        }
    }

    private void QSwathAllRefine(object sender, ExecutedRoutedEventArgs e)
    {
        _qSwathText.QSwathTextClear();
        _qSwathText.QSwathTextScan(_qSwathViewer);
        IReadOnlyList<FrameworkElement> list = _qSwathText.QSwathTextItem;
        if (list.Count == 0)
        {
            return;
        }

        _qSwathHead = new QSwathSeam(0, (list[0] as TextBlock)?.ContentStart);
        _qSwathTail = new QSwathSeam(list.Count - 1, (list[^1] as TextBlock)?.ContentEnd);
        InvalidateVisual();
    }

    private void QSwathAdjust(Point point)
    {
        if (_qSwathAnchor is not QSwathSeam anchor || _qSwathText.QSwathTextFind(point) is not QSwathSeam moving)
        {
            return;
        }

        bool forward = anchor.QSwathSeamIndex < moving.QSwathSeamIndex
            || (anchor.QSwathSeamIndex == moving.QSwathSeamIndex
                && QSwathCaretSort(anchor.QSwathSeamCaret, moving.QSwathSeamCaret) <= 0);
        _qSwathHead = forward ? anchor : moving;
        _qSwathTail = forward ? moving : anchor;
        InvalidateVisual();
    }

    private static int QSwathCaretSort(TextPointer? anchor, TextPointer? moving)
    {
        return anchor is null || moving is null ? 0 : anchor.CompareTo(moving);
    }

    protected override void OnRender(DrawingContext context)
    {
        base.OnRender(context);
        if (_qSwathHead is not QSwathSeam head || _qSwathTail is not QSwathSeam tail)
        {
            return;
        }

        Brush brush = new SolidColorBrush(SystemColors.HighlightColor) { Opacity = 0.35 };
        brush.Freeze();

        IReadOnlyList<FrameworkElement> list = _qSwathText.QSwathTextItem;
        for (int index = head.QSwathSeamIndex; index <= tail.QSwathSeamIndex && index < list.Count; index++)
        {
            FrameworkElement item = list[index];
            if (item is not TextBlock block)
            {
                context.DrawRectangle(brush, null, _qSwathText.QSwathTextPlace(item));
                continue;
            }

            GeneralTransform transform = block.TransformToVisual(this);
            TextPointer from = head.QSwathSeamStart(index, block);
            foreach (Rect band in QSwathBandScan(from, tail.QSwathSeamFinish(index, block)))
            {
                context.DrawRectangle(brush, null, transform.TransformBounds(band));
            }
        }
    }

    private static IEnumerable<Rect> QSwathBandScan(TextPointer from, TextPointer to)
    {
        Rect band = Rect.Empty;
        TextPointer? caret = from.GetInsertionPosition(LogicalDirection.Forward);
        while (caret is not null && caret.CompareTo(to) < 0)
        {
            Rect glyph = caret.GetCharacterRect(LogicalDirection.Forward);
            TextPointer? next = caret.GetNextInsertionPosition(LogicalDirection.Forward);
            if (next is not null)
            {
                Rect edge = next.GetCharacterRect(LogicalDirection.Backward);
                if (!edge.IsEmpty && Math.Abs(edge.Top - glyph.Top) < QSwathRowSlack)
                {
                    glyph.Union(edge);
                }
            }

            if (!glyph.IsEmpty)
            {
                if (band.IsEmpty || Math.Abs(band.Top - glyph.Top) >= QSwathRowSlack)
                {
                    if (!band.IsEmpty)
                    {
                        yield return band;
                    }

                    band = glyph;
                }
                else
                {
                    band.Union(glyph);
                }
            }

            caret = next;
        }

        if (!band.IsEmpty)
        {
            yield return band;
        }
    }

    private void QSwathPressRefine(object sender, MouseButtonEventArgs e)
    {
        QSwathClear();
        if (e.ClickCount != 1 || QSwathControlCheck(e.OriginalSource as DependencyObject))
        {
            return;
        }

        _qSwathPress = e.GetPosition(this);
    }

    private void QSwathMoveRefine(object sender, MouseEventArgs e)
    {
        if (_qSwathPress is not Point press || e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        Point point = e.GetPosition(this);
        if (_qSwathAnchor is null)
        {
            if (Math.Abs(point.X - press.X) <= SystemParameters.MinimumHorizontalDragDistance
                && Math.Abs(point.Y - press.Y) <= SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            _qSwathText.QSwathTextClear();
            _qSwathText.QSwathTextScan(_qSwathViewer);
            _qSwathAnchor = _qSwathText.QSwathTextFind(press);
            if (_qSwathAnchor is null)
            {
                _qSwathPress = null;
                return;
            }

            Focus();
            _qSwathViewer.CaptureMouse();
        }

        QSwathScroll(e.GetPosition(_qSwathViewer));
        QSwathAdjust(point);
    }

    private void QSwathReleaseRefine(object sender, MouseButtonEventArgs e)
    {
        _qSwathPress = null;
        if (_qSwathViewer.IsMouseCaptured)
        {
            _qSwathViewer.ReleaseMouseCapture();
        }
    }

    private void QSwathLostRefine(object sender, MouseEventArgs e)
    {
        _qSwathPress = null;
    }

    private void QSwathCursorRefine(object sender, QueryCursorEventArgs e)
    {
        bool dragging = _qSwathAnchor is not null && _qSwathPress is not null;
        if (dragging || QSwathTextCheck(e.OriginalSource as DependencyObject))
        {
            e.Cursor = Cursors.IBeam;
            e.Handled = true;
        }
    }

    private static bool QSwathControlCheck(DependencyObject? node)
    {
        while (node is not null)
        {
            if (node is ButtonBase or Slider or ScrollBar or QGrasp or TextBoxBase or Hyperlink)
            {
                return true;
            }

            node = QSwathParentRead(node);
        }

        return false;
    }

    private static bool QSwathTextCheck(DependencyObject? node)
    {
        bool text = false;
        while (node is not null)
        {
            if (node is ButtonBase or Slider or ScrollBar or QGrasp or TextBoxBase or Hyperlink)
            {
                return false;
            }

            text |= node is TextBlock;
            node = QSwathParentRead(node);
        }

        return text;
    }

    private static DependencyObject? QSwathParentRead(DependencyObject node)
    {
        return node is Visual or System.Windows.Media.Media3D.Visual3D
            ? VisualTreeHelper.GetParent(node)
            : LogicalTreeHelper.GetParent(node);
    }

    private void QSwathScroll(Point point)
    {
        if (point.Y < 0)
        {
            _qSwathViewer.ScrollToVerticalOffset(_qSwathViewer.VerticalOffset - QSwathScrollStep);
        }
        else if (point.Y > _qSwathViewer.ViewportHeight)
        {
            _qSwathViewer.ScrollToVerticalOffset(_qSwathViewer.VerticalOffset + QSwathScrollStep);
        }
    }
}
