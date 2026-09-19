using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Llyn.UIVeneer;

public sealed partial class PSwath
{
    private const double PSwathScrollStep = 24;

    private void PSwathPressHandle(object sender, MouseButtonEventArgs e)
    {
        PSwathClear();
        if (e.ClickCount != 1 || PSwathControlCheck(e.OriginalSource as DependencyObject))
        {
            return;
        }

        _pSwathPress = e.GetPosition(this);
    }

    private void PSwathMoveHandle(object sender, MouseEventArgs e)
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

            _pSwathList.Clear();
            PSwathScan(_pSwathViewer);
            _pSwathAnchor = PSwathFind(press);
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

    private void PSwathReleaseHandle(object sender, MouseButtonEventArgs e)
    {
        _pSwathPress = null;
        if (_pSwathViewer.IsMouseCaptured)
        {
            _pSwathViewer.ReleaseMouseCapture();
        }
    }

    private void PSwathLostHandle(object sender, MouseEventArgs e)
    {
        _pSwathPress = null;
    }

    private void PSwathCursorHandle(object sender, QueryCursorEventArgs e)
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
