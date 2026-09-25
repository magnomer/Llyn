using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private ObservableCollection<PCard>? _pCardDragList;
    private PCard? _pCardDragCard;
    private ItemsControl? _pCardDragHost;
    private PCardGhost? _pCardDragGhost;

    private double _pCardDragOrigin;
    private double _pCardDragGrab;

    private double _pCardDragHeight;

    internal void PCardDragHandle(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PCard card })
        {
            return;
        }

        ObservableCollection<PCard>? list = PCardListFind(card);

        if (list is null || list.Count <= 1)
        {
            return;
        }

        ItemsControl host = ReferenceEquals(list, _pMeaningList) ? PMeaningList : PCollocationList;

        if (host.ItemContainerGenerator.ContainerFromItem(card) is not FrameworkElement container)
        {
            return;
        }

        _pCardDragList = list;
        _pCardDragCard = card;
        _pCardDragHost = host;
        _pCardDragOrigin = e.GetPosition(host).Y;
        _pCardDragGrab = _pCardDragOrigin - container.TranslatePoint(new Point(0, 0), host).Y;
        _pCardDragHeight = container.ActualHeight;

        host.CaptureMouse();
    }

    private void PCardDragUpdate(object sender, MouseEventArgs e)
    {
        if (_pCardDragHost is null || _pCardDragCard is null || _pCardDragList is null)
        {
            return;
        }

        if (e.LeftButton is not MouseButtonState.Pressed)
        {
            PCardDragReset();
            return;
        }

        double pointer = e.GetPosition(_pCardDragHost).Y;

        if (_pCardDragGhost is null)
        {
            if (Math.Abs(pointer - _pCardDragOrigin) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            _pCardDragGhost = PCardGhostBuild();

            if (_pCardDragGhost is null)
            {
                PCardDragReset();
                return;
            }
        }

        double top = pointer - _pCardDragGrab;

        _pCardDragGhost.PCardGhostTop = top;
        PCardMove(top);
    }

    private void PCardDragFinish(object sender, MouseButtonEventArgs e)
    {
        PCardDragReset();
    }

    private void PCardDragCancel(object sender, MouseEventArgs e)
    {
        PCardDragReset();
    }

    private PCardGhost? PCardGhostBuild()
    {
        ItemsControl host = _pCardDragHost!;

        if (host.ItemContainerGenerator.ContainerFromItem(_pCardDragCard) is not FrameworkElement container)
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

    private void PCardMove(double top)
    {
        ObservableCollection<PCard> list = _pCardDragList!;
        ItemsControl host = _pCardDragHost!;
        int current = list.IndexOf(_pCardDragCard!);

        if (current < 0)
        {
            return;
        }

        double bottom = top + _pCardDragHeight;
        int target = current;

        for (int index = 0; index < list.Count; index++)
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

        if (target == current)
        {
            return;
        }

        PCardMove(_pCardDragCard!, target);
    }

    private void PCardDragReset()
    {
        if (_pCardDragHost is null)
        {
            return;
        }

        ItemsControl host = _pCardDragHost;
        PCardGhost? ghost = _pCardDragGhost;

        _pCardDragHost = null;
        _pCardDragGhost = null;
        _pCardDragCard = null;
        _pCardDragList = null;

        if (ghost is not null)
        {
            AdornerLayer.GetAdornerLayer(host)?.Remove(ghost);
        }

        if (host.IsMouseCaptured)
        {
            host.ReleaseMouseCapture();
        }
    }
}
