using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Llyn.UIShell;

public sealed partial class PSwath : FrameworkElement
{
    private const double PSwathRowSlack = 4;

    private readonly record struct PSwathSeam(int PSwathSeamIndex, TextPointer? PSwathSeamCaret);

    private readonly List<FrameworkElement> _pSwathList = [];

    private ScrollViewer _pSwathViewer = null!;

    private Point? _pSwathPress;

    private PSwathSeam? _pSwathAnchor;

    private PSwathSeam? _pSwathHead;

    private PSwathSeam? _pSwathTail;

    public PSwath()
    {
        IsHitTestVisible = false;
        Focusable = true;
        FocusVisualStyle = null;
        CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, PSwathCopyHandle, PSwathCopyCheck));
        CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, PSwathAllHandle));
    }

    internal void PSwathAttach(ScrollViewer viewer)
    {
        _pSwathViewer = viewer;
        viewer.PreviewMouseLeftButtonDown += PSwathPressHandle;
        viewer.PreviewMouseMove += PSwathMoveHandle;
        viewer.PreviewMouseLeftButtonUp += PSwathReleaseHandle;
        viewer.LostMouseCapture += PSwathLostHandle;
        viewer.AddHandler(QueryCursorEvent, new QueryCursorEventHandler(PSwathCursorHandle));
    }

    internal void PSwathClear()
    {
        _pSwathPress = null;
        _pSwathAnchor = null;
        _pSwathHead = null;
        _pSwathTail = null;
        _pSwathList.Clear();
        InvalidateVisual();
    }

    private void PSwathCopyCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _pSwathHead is not null;
    }

    private void PSwathCopyHandle(object sender, ExecutedRoutedEventArgs e)
    {
        string text = PSwathTextRead();
        if (text.Length > 0)
        {
            Clipboard.SetText(text);
        }
    }

    private void PSwathAllHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _pSwathList.Clear();
        PSwathScan(_pSwathViewer);
        if (_pSwathList.Count == 0)
        {
            return;
        }

        _pSwathHead = new PSwathSeam(0, (_pSwathList[0] as TextBlock)?.ContentStart);
        _pSwathTail = new PSwathSeam(_pSwathList.Count - 1, (_pSwathList[^1] as TextBlock)?.ContentEnd);
        InvalidateVisual();
    }

    private void PSwathScan(DependencyObject node)
    {
        if (node is UIElement { IsVisible: false })
        {
            return;
        }

        if (node is TextBlock or Image or Rectangle or PScreen or PContour)
        {
            FrameworkElement item = (FrameworkElement)node;
            if (item.ActualWidth > 0 && item.ActualHeight > 0)
            {
                _pSwathList.Add(item);
            }

            return;
        }

        int count = VisualTreeHelper.GetChildrenCount(node);
        for (int index = 0; index < count; index++)
        {
            PSwathScan(VisualTreeHelper.GetChild(node, index));
        }
    }

    private Rect PSwathBoundRead(FrameworkElement item)
    {
        return item.TransformToVisual(this).TransformBounds(new Rect(0, 0, item.ActualWidth, item.ActualHeight));
    }

    private PSwathSeam? PSwathFind(Point point)
    {
        int best = -1;
        double nearest = double.MaxValue;
        Rect bound = Rect.Empty;

        for (int index = 0; index < _pSwathList.Count; index++)
        {
            Rect rect = PSwathBoundRead(_pSwathList[index]);
            double vertical = point.Y < rect.Top
                ? rect.Top - point.Y
                : point.Y > rect.Bottom ? point.Y - rect.Bottom : 0;
            double horizontal = point.X < rect.Left
                ? rect.Left - point.X
                : point.X > rect.Right ? point.X - rect.Right : 0;
            double distance = vertical * 1000 + horizontal;
            if (distance < nearest)
            {
                nearest = distance;
                best = index;
                bound = rect;
            }
        }

        if (best < 0)
        {
            return null;
        }

        if (_pSwathList[best] is not TextBlock block)
        {
            return new PSwathSeam(best, null);
        }

        if (point.Y < bound.Top || (point.X < bound.Left && point.Y <= bound.Bottom))
        {
            return new PSwathSeam(best, block.ContentStart);
        }

        if (point.Y > bound.Bottom || point.X > bound.Right)
        {
            return new PSwathSeam(best, block.ContentEnd);
        }

        try
        {
            Point local = TransformToVisual(block).Transform(point);
            TextPointer? caret = block.GetPositionFromPoint(local, true);
            return new PSwathSeam(best, caret ?? block.ContentStart);
        }
        catch (InvalidOperationException)
        {
            return new PSwathSeam(best, block.ContentStart);
        }
    }

    private void PSwathAdjust(Point point)
    {
        if (_pSwathAnchor is not PSwathSeam anchor || PSwathFind(point) is not PSwathSeam moving)
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

    private string PSwathTextRead()
    {
        if (_pSwathHead is not PSwathSeam head || _pSwathTail is not PSwathSeam tail)
        {
            return string.Empty;
        }

        StringBuilder text = new();
        Rect previous = Rect.Empty;
        for (int index = head.PSwathSeamIndex; index <= tail.PSwathSeamIndex && index < _pSwathList.Count; index++)
        {
            if (_pSwathList[index] is not TextBlock block)
            {
                continue;
            }

            string piece = new TextRange(PSwathFromRead(head, index, block), PSwathToRead(tail, index, block)).Text;
            if (piece.Length == 0)
            {
                continue;
            }

            Rect bound = PSwathBoundRead(block);
            if (text.Length > 0)
            {
                text.Append(PSwathSeparatorRead(previous, bound));
            }

            text.Append(piece);
            previous = bound;
        }

        return text.ToString();
    }

    private static string PSwathSeparatorRead(Rect previous, Rect bound)
    {
        if (previous.IsEmpty || bound.Top >= previous.Bottom - PSwathRowSlack)
        {
            return Environment.NewLine;
        }

        return bound.Left - previous.Right <= PSwathRowSlack ? string.Empty : " ";
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

        for (int index = head.PSwathSeamIndex; index <= tail.PSwathSeamIndex && index < _pSwathList.Count; index++)
        {
            FrameworkElement item = _pSwathList[index];
            if (item is not TextBlock block)
            {
                context.DrawRectangle(brush, null, PSwathBoundRead(item));
                continue;
            }

            GeneralTransform transform = block.TransformToVisual(this);
            foreach (Rect band in PSwathBandScan(PSwathFromRead(head, index, block), PSwathToRead(tail, index, block)))
            {
                context.DrawRectangle(brush, null, transform.TransformBounds(band));
            }
        }
    }

    private static TextPointer PSwathFromRead(PSwathSeam head, int index, TextBlock block)
    {
        return index == head.PSwathSeamIndex ? head.PSwathSeamCaret ?? block.ContentStart : block.ContentStart;
    }

    private static TextPointer PSwathToRead(PSwathSeam tail, int index, TextBlock block)
    {
        return index == tail.PSwathSeamIndex ? tail.PSwathSeamCaret ?? block.ContentEnd : block.ContentEnd;
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
}
