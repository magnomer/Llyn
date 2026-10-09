using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Llyn.UIDeportment;

public sealed class PSwath : FrameworkElement
{
    internal const double PSwathRowSlack = 4;

    private const double PSwathScrollStep = 24;

    private readonly QSwathText _pSwathText;

    private ScrollViewer _pSwathViewer = null!;

    private Point? _pSwathPress;

    private PSwathSeam? _pSwathAnchor;

    private PSwathSeam? _pSwathHead;

    private PSwathSeam? _pSwathTail;

    public PSwath()
    {
        _pSwathText = new QSwathText(this);
        IsHitTestVisible = false;
        Focusable = true;
        FocusVisualStyle = null;
        CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, PSwathCopyRefine, PSwathCopyCheck));
        CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, PSwathAllRefine));
    }

    internal void PSwathAttach(ScrollViewer viewer)
    {
        _pSwathViewer = viewer;
        viewer.PreviewMouseLeftButtonDown += PSwathPressRefine;
        viewer.PreviewMouseMove += PSwathMoveRefine;
        viewer.PreviewMouseLeftButtonUp += PSwathReleaseRefine;
        viewer.LostMouseCapture += PSwathLostRefine;
        viewer.AddHandler(QueryCursorEvent, new QueryCursorEventHandler(PSwathCursorRefine));
    }

    internal void PSwathClear()
    {
        _pSwathPress = null;
        _pSwathAnchor = null;
        _pSwathHead = null;
        _pSwathTail = null;
        _pSwathText.QSwathTextClear();
        InvalidateVisual();
    }

    private void PSwathCopyCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _pSwathHead is not null;
    }

    private void PSwathCopyRefine(object sender, ExecutedRoutedEventArgs e)
    {
        string text = _pSwathText.QSwathTextRead(_pSwathHead, _pSwathTail);
        if (text.Length > 0)
        {
            Clipboard.SetText(text);
        }
    }

    private void PSwathAllRefine(object sender, ExecutedRoutedEventArgs e)
    {
        _pSwathText.QSwathTextClear();
        _pSwathText.QSwathTextScan(_pSwathViewer);
        IReadOnlyList<FrameworkElement> list = _pSwathText.QSwathTextItem;
        if (list.Count == 0)
        {
            return;
        }

        _pSwathHead = new PSwathSeam(0, (list[0] as TextBlock)?.ContentStart);
        _pSwathTail = new PSwathSeam(list.Count - 1, (list[^1] as TextBlock)?.ContentEnd);
        InvalidateVisual();
    }

    private void PSwathAdjust(Point point)
    {
        if (_pSwathAnchor is not PSwathSeam anchor || _pSwathText.QSwathTextFind(point) is not PSwathSeam moving)
        {
            return;
        }

        bool forward = anchor.PSwathSeamIndex < moving.PSwathSeamIndex
            || (anchor.PSwathSeamIndex == moving.PSwathSeamIndex
                && PSwathCaretSort(anchor.PSwathSeamCaret, moving.PSwathSeamCaret) <= 0);
        _pSwathHead = forward ? anchor : moving;
        _pSwathTail = forward ? moving : anchor;
        InvalidateVisual();
    }

    private static int PSwathCaretSort(TextPointer? anchor, TextPointer? moving)
    {
        return anchor is null || moving is null ? 0 : anchor.CompareTo(moving);
    }

    protected override void OnRender(DrawingContext context)
    {
        base.OnRender(context);
        if (_pSwathHead is not PSwathSeam head || _pSwathTail is not PSwathSeam tail)
        {
            return;
        }

        Brush brush = new SolidColorBrush(SystemColors.HighlightColor) { Opacity = 0.35 };
        brush.Freeze();

        IReadOnlyList<FrameworkElement> list = _pSwathText.QSwathTextItem;
        for (int index = head.PSwathSeamIndex; index <= tail.PSwathSeamIndex && index < list.Count; index++)
        {
            FrameworkElement item = list[index];
            if (item is not TextBlock block)
            {
                context.DrawRectangle(brush, null, _pSwathText.QSwathTextPlace(item));
                continue;
            }

            GeneralTransform transform = block.TransformToVisual(this);
            TextPointer from = head.PSwathSeamStart(index, block);
            foreach (Rect band in PSwathBandScan(from, tail.PSwathSeamFinish(index, block)))
            {
                context.DrawRectangle(brush, null, transform.TransformBounds(band));
            }
        }
    }

    private static IEnumerable<Rect> PSwathBandScan(TextPointer from, TextPointer to)
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
                if (!edge.IsEmpty && Math.Abs(edge.Top - glyph.Top) < PSwathRowSlack)
                {
                    glyph.Union(edge);
                }
            }

            if (!glyph.IsEmpty)
            {
                if (band.IsEmpty || Math.Abs(band.Top - glyph.Top) >= PSwathRowSlack)
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

    private void PSwathPressRefine(object sender, MouseButtonEventArgs e)
    {
        PSwathClear();
        if (e.ClickCount != 1 || PSwathControlCheck(e.OriginalSource as DependencyObject))
        {
            return;
        }

        _pSwathPress = e.GetPosition(this);
    }

    private void PSwathMoveRefine(object sender, MouseEventArgs e)
    {
        if (_pSwathPress is not Point press || e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        Point point = e.GetPosition(this);
        if (_pSwathAnchor is null)
        {
            if (Math.Abs(point.X - press.X) <= SystemParameters.MinimumHorizontalDragDistance
                && Math.Abs(point.Y - press.Y) <= SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            _pSwathText.QSwathTextClear();
            _pSwathText.QSwathTextScan(_pSwathViewer);
            _pSwathAnchor = _pSwathText.QSwathTextFind(press);
            if (_pSwathAnchor is null)
            {
                _pSwathPress = null;
                return;
            }

            Focus();
            _pSwathViewer.CaptureMouse();
        }

        PSwathScroll(e.GetPosition(_pSwathViewer));
        PSwathAdjust(point);
    }

    private void PSwathReleaseRefine(object sender, MouseButtonEventArgs e)
    {
        _pSwathPress = null;
        if (_pSwathViewer.IsMouseCaptured)
        {
            _pSwathViewer.ReleaseMouseCapture();
        }
    }

    private void PSwathLostRefine(object sender, MouseEventArgs e)
    {
        _pSwathPress = null;
    }

    private void PSwathCursorRefine(object sender, QueryCursorEventArgs e)
    {
        bool dragging = _pSwathAnchor is not null && _pSwathPress is not null;
        if (dragging || PSwathTextCheck(e.OriginalSource as DependencyObject))
        {
            e.Cursor = Cursors.IBeam;
            e.Handled = true;
        }
    }

    private static bool PSwathControlCheck(DependencyObject? node)
    {
        while (node is not null)
        {
            if (node is ButtonBase or Slider or ScrollBar or PGrasp or TextBoxBase or Hyperlink)
            {
                return true;
            }

            node = PSwathParentRead(node);
        }

        return false;
    }

    private static bool PSwathTextCheck(DependencyObject? node)
    {
        bool text = false;
        while (node is not null)
        {
            if (node is ButtonBase or Slider or ScrollBar or PGrasp or TextBoxBase or Hyperlink)
            {
                return false;
            }

            text |= node is TextBlock;
            node = PSwathParentRead(node);
        }

        return text;
    }

    private static DependencyObject? PSwathParentRead(DependencyObject node)
    {
        return node is Visual or System.Windows.Media.Media3D.Visual3D
            ? VisualTreeHelper.GetParent(node)
            : LogicalTreeHelper.GetParent(node);
    }

    private void PSwathScroll(Point point)
    {
        if (point.Y < 0)
        {
            _pSwathViewer.ScrollToVerticalOffset(_pSwathViewer.VerticalOffset - PSwathScrollStep);
        }
        else if (point.Y > _pSwathViewer.ViewportHeight)
        {
            _pSwathViewer.ScrollToVerticalOffset(_pSwathViewer.VerticalOffset + PSwathScrollStep);
        }
    }
}
