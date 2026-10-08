using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Llyn.UIDeportment;

internal sealed class QCardGesture
{
    private readonly ItemsControl _qCardGestureHost;

    private readonly double _qCardGestureOrigin;

    private readonly double _qCardGestureGrab;

    private readonly double _qCardGestureHeight;

    private PCardGhost? _qCardGestureGhost;

    internal QCardGesture(object item, long id, ItemsControl host, FrameworkElement container, double origin)
    {
        QCardGestureItem = item;
        QCardGestureId = id;
        _qCardGestureHost = host;
        _qCardGestureOrigin = origin;
        _qCardGestureGrab = origin - container.TranslatePoint(new Point(0, 0), host).Y;
        _qCardGestureHeight = container.ActualHeight;
    }

    internal object QCardGestureItem { get; }

    internal long QCardGestureId { get; }

    internal bool QCardGestureShow(MouseEventArgs e)
    {
        double pointer = e.GetPosition(_qCardGestureHost).Y;

        if (_qCardGestureGhost is null)
        {
            if (Math.Abs(pointer - _qCardGestureOrigin) < SystemParameters.MinimumVerticalDragDistance)
            {
                return true;
            }

            _qCardGestureGhost = QCardGestureBuild();

            if (_qCardGestureGhost is null)
            {
                return false;
            }
        }

        _qCardGestureGhost.PCardGhostTop = pointer - _qCardGestureGrab;
        return true;
    }

    private PCardGhost? QCardGestureBuild()
    {
        ItemsControl host = _qCardGestureHost;

        if (host.ItemContainerGenerator.ContainerFromItem(QCardGestureItem) is not FrameworkElement container)
        {
            return null;
        }

        AdornerLayer? layer = AdornerLayer.GetAdornerLayer(host);

        if (layer is null)
        {
            return null;
        }

        var ghost = new PCardGhost(host, container, container.TranslatePoint(new Point(0, 0), host).X);
        layer.Add(ghost);

        return ghost;
    }

    internal int QCardGestureResolve(MouseEventArgs e)
    {
        ItemsControl host = _qCardGestureHost;
        double top = e.GetPosition(host).Y - _qCardGestureGrab;
        int current = host.Items.IndexOf(QCardGestureItem);
        double bottom = top + _qCardGestureHeight;
        int target = current;

        for (int index = 0; index < host.Items.Count; index++)
        {
            if (host.ItemContainerGenerator.ContainerFromIndex(index) is not FrameworkElement container)
            {
                continue;
            }

            double middle = container.TranslatePoint(new Point(0, 0), host).Y + (container.ActualHeight / 2);

            if (index < current && top < middle)
            {
                target = index;
                break;
            }

            if (index > current && bottom > middle)
            {
                target = index;
            }
        }

        return target;
    }

    internal void QCardGestureClear()
    {
        if (_qCardGestureGhost is not null)
        {
            AdornerLayer.GetAdornerLayer(_qCardGestureHost)?.Remove(_qCardGestureGhost);
        }

        if (_qCardGestureHost.IsMouseCaptured)
        {
            _qCardGestureHost.ReleaseMouseCapture();
        }
    }
}
