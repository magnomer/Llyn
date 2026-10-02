using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCardDrag
{
    private readonly ItemsControl _qCardDragMeaning;

    private readonly ItemsControl _qCardDragCollocation;

    private CEditor _cEditor = null!;

    private PCard? _qCardDragCard;
    private ItemsControl? _qCardDragHost;
    private PCardGhost? _qCardDragGhost;

    private double _qCardDragOrigin;
    private double _qCardDragGrab;

    private double _qCardDragHeight;

    internal QCardDrag(FrameworkElement surface)
    {
        _qCardDragMeaning = QContract.QContractFind<ItemsControl>(surface, "PMeaningList");
        _qCardDragCollocation = QContract.QContractFind<ItemsControl>(surface, "PCollocationList");
        _qCardDragMeaning.LostMouseCapture += QCardDragReset;
        _qCardDragMeaning.MouseLeftButtonUp += QCardDragReset;
        _qCardDragMeaning.MouseMove += QCardDragUpdate;
        _qCardDragCollocation.LostMouseCapture += QCardDragReset;
        _qCardDragCollocation.MouseLeftButtonUp += QCardDragReset;
        _qCardDragCollocation.MouseMove += QCardDragUpdate;
    }

    internal void QCardDragIntroduce(CEditor editor)
    {
        _cEditor = editor;
    }

    internal void QCardDragRefine(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PCard card })
        {
            return;
        }

        ItemsControl? host = _qCardDragMeaning.Items.Contains(card) ? _qCardDragMeaning
            : _qCardDragCollocation.Items.Contains(card) ? _qCardDragCollocation
            : null;

        if (host is null || host.Items.Count <= 1)
        {
            return;
        }

        if (host.ItemContainerGenerator.ContainerFromItem(card) is not FrameworkElement container)
        {
            return;
        }

        _qCardDragCard = card;
        _qCardDragHost = host;
        _qCardDragOrigin = e.GetPosition(host).Y;
        _qCardDragGrab = _qCardDragOrigin - container.TranslatePoint(new Point(0, 0), host).Y;
        _qCardDragHeight = container.ActualHeight;

        host.CaptureMouse();
    }

    private void QCardDragUpdate(object sender, MouseEventArgs e)
    {
        if (_qCardDragHost is null || _qCardDragCard is null)
        {
            return;
        }

        if (e.LeftButton is not MouseButtonState.Pressed)
        {
            QCardDragReset(sender, e);
            return;
        }

        double pointer = e.GetPosition(_qCardDragHost).Y;
        double top = pointer - _qCardDragGrab;

        QCardDragShow(sender, e, pointer, top);

        if (_qCardDragHost is null)
        {
            return;
        }

        QCardDragMove(top);
    }

    private void QCardDragShow(object sender, MouseEventArgs e, double pointer, double top)
    {
        if (_qCardDragGhost is null)
        {
            if (Math.Abs(pointer - _qCardDragOrigin) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            _qCardDragGhost = QCardDragBuild();

            if (_qCardDragGhost is null)
            {
                QCardDragReset(sender, e);
                return;
            }
        }

        _qCardDragGhost.PCardGhostTop = top;
    }

    private PCardGhost? QCardDragBuild()
    {
        ItemsControl host = _qCardDragHost!;

        if (host.ItemContainerGenerator.ContainerFromItem(_qCardDragCard) is not FrameworkElement container)
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

    private void QCardDragMove(double top)
    {
        ItemsControl host = _qCardDragHost!;
        int current = host.Items.IndexOf(_qCardDragCard);
        double bottom = top + _qCardDragHeight;
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

        _cEditor.CEditorList.CCardMove(_qCardDragCard!.PCardId, target);
    }

    private void QCardDragReset(object sender, MouseEventArgs e)
    {
        if (_qCardDragHost is null)
        {
            return;
        }

        ItemsControl host = _qCardDragHost;
        PCardGhost? ghost = _qCardDragGhost;

        _qCardDragHost = null;
        _qCardDragGhost = null;
        _qCardDragCard = null;

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
