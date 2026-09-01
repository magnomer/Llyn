using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Llyn.UIShell;

/// <summary>
/// Dragging a card to another place in its list. The card is dragged by its header: a ghost of it
/// follows the pointer while the card itself stays on screen, and the list rearranges under that
/// ghost as the pointer crosses the middle of a neighbour, so the order is already the new order
/// when the button comes up. The list order is the saved order, so nothing else has to be told.
/// </summary>
public partial class PEditor
{
    private ObservableCollection<PCard>? _pCardDragList;
    private PCard? _pCardDragCard;
    private ItemsControl? _pCardDragHost;
    private PCardGhost? _pCardDragGhost;

    // Where the pointer went down, and how far below the card's top that was: the ghost is held at
    // that same distance from the pointer, so the card does not jump under the hand that took it.
    private double _pCardDragOrigin;
    private double _pCardDragGrab;

    // How tall the dragged card is. The order is decided from where the ghost is, not from where the
    // pointer is, and the ghost is this tall.
    private double _pCardDragHeight;

    internal void PCardDragHandle(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PCard card })
        {
            return;
        }

        ObservableCollection<PCard>? list = PCardListFind(card);

        // A list of one has nowhere else to put its card, so no drag begins on it.
        if (list is null || list.Count <= 1)
        {
            return;
        }

        ItemsControl host = ReferenceEquals(list, _pSenseList) ? PSenseList : PCollocationList;

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

        // Captured on the list and not on the card: the card slides out from under the pointer as
        // soon as the order changes, and it is the list that has to keep hearing the pointer until
        // the button comes back up.
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

        // A press is not a drag: nothing is shown until the pointer has travelled far enough that the
        // card was meant to be moved rather than its header clicked.
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

    // Capture can be taken away without the button ever coming up — another window, a menu — and a
    // ghost left painted over the list would outlive the drag it belongs to.
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

    // Which place the ghost is now over. The card is judged by its own edges and never by the pointer:
    // where inside the header it was taken hold of would otherwise decide how far it has to travel,
    // and going up would need a different distance from going down. Its leading edge is what crosses
    // a neighbour — the top of it on the way up, the bottom of it on the way down — so either
    // direction swaps at the same half-card, and it is the same half-card whatever the grip was.
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

            // Upward: the first card whose middle the ghost's top has risen above. Downward: the last
            // card whose middle its bottom has fallen past, so one long drag passes every card it crosses.
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

        list.Move(current, target);
        PCardOrderUpdate(list);
    }

    private void PCardDragReset()
    {
        if (_pCardDragHost is null)
        {
            return;
        }

        ItemsControl host = _pCardDragHost;
        PCardGhost? ghost = _pCardDragGhost;

        // Cleared before the capture is released: letting go raises the lost-capture notice, which
        // comes straight back here and must find a drag that is already over.
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
