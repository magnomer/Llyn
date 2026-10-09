using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Llyn.UIDeportment;

public sealed class QSwathText
{
    private readonly PSwath _qSwathTextOwner;

    private readonly List<FrameworkElement> _qSwathTextList = [];

    public QSwathText(PSwath owner)
    {
        ArgumentNullException.ThrowIfNull(owner);
        _qSwathTextOwner = owner;
    }

    public IReadOnlyList<FrameworkElement> QSwathTextItem => _qSwathTextList;

    public void QSwathTextClear()
    {
        _qSwathTextList.Clear();
    }

    public void QSwathTextScan(DependencyObject node)
    {
        if (node is UIElement { IsVisible: false })
        {
            return;
        }

        if (node is TextBlock or Image or Rectangle or PContour || QScreen.QScreenFind(node) is not null)
        {
            FrameworkElement item = (FrameworkElement)node;
            if (item.ActualWidth > 0 && item.ActualHeight > 0)
            {
                _qSwathTextList.Add(item);
            }

            return;
        }

        int count = VisualTreeHelper.GetChildrenCount(node);
        for (int index = 0; index < count; index++)
        {
            QSwathTextScan(VisualTreeHelper.GetChild(node, index));
        }
    }

    public Rect QSwathTextPlace(FrameworkElement item)
    {
        return item.TransformToVisual(_qSwathTextOwner)
            .TransformBounds(new Rect(0, 0, item.ActualWidth, item.ActualHeight));
    }

    public PSwathSeam? QSwathTextFind(Point point)
    {
        int best = -1;
        double nearest = double.MaxValue;
        Rect bound = Rect.Empty;

        for (int index = 0; index < _qSwathTextList.Count; index++)
        {
            Rect rect = QSwathTextPlace(_qSwathTextList[index]);
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

        if (_qSwathTextList[best] is not TextBlock block)
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
            Point local = _qSwathTextOwner.TransformToVisual(block).Transform(point);
            TextPointer? caret = block.GetPositionFromPoint(local, true);
            return new PSwathSeam(best, caret ?? block.ContentStart);
        }
        catch (InvalidOperationException)
        {
            return new PSwathSeam(best, block.ContentStart);
        }
    }

    public string QSwathTextRead(PSwathSeam? start, PSwathSeam? finish)
    {
        if (start is not PSwathSeam head || finish is not PSwathSeam tail)
        {
            return string.Empty;
        }

        StringBuilder text = new();
        Rect previous = Rect.Empty;
        for (int index = head.PSwathSeamIndex;
            index <= tail.PSwathSeamIndex && index < _qSwathTextList.Count;
            index++)
        {
            if (_qSwathTextList[index] is not TextBlock block)
            {
                continue;
            }

            string piece = new TextRange(
                head.PSwathSeamStart(index, block), tail.PSwathSeamFinish(index, block)).Text;
            if (piece.Length == 0)
            {
                continue;
            }

            Rect bound = QSwathTextPlace(block);
            if (text.Length > 0)
            {
                text.Append(QSwathTextFormat(previous, bound));
            }

            text.Append(piece);
            previous = bound;
        }

        return text.ToString();
    }

    private static string QSwathTextFormat(Rect previous, Rect bound)
    {
        if (previous.IsEmpty || bound.Top >= previous.Bottom - PSwath.PSwathRowSlack)
        {
            return Environment.NewLine;
        }

        return bound.Left - previous.Right <= PSwath.PSwathRowSlack ? string.Empty : " ";
    }
}
