using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCardDrag
{
    private readonly ItemsControl _qCardDragMeaning;

    private readonly ItemsControl _qCardDragCollocation;

    private CEditor _cEditor = null!;

    private QCardGesture? _qCardDragGesture;

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

        _qCardDragGesture = new QCardGesture(card, card.PCardId, host, container, e.GetPosition(host).Y);

        host.CaptureMouse();
    }

    private void QCardDragUpdate(object sender, MouseEventArgs e)
    {
        if (_qCardDragGesture is not QCardGesture gesture)
        {
            return;
        }

        if (e.LeftButton is not MouseButtonState.Pressed)
        {
            QCardDragReset(sender, e);
            return;
        }

        if (!gesture.QCardGestureShow(e))
        {
            QCardDragReset(sender, e);
            return;
        }

        int target = gesture.QCardGestureResolve(e);

        _cEditor.CEditorList.CCardMove(gesture.QCardGestureId, target);
    }

    private void QCardDragReset(object sender, MouseEventArgs e)
    {
        if (_qCardDragGesture is not QCardGesture gesture)
        {
            return;
        }

        _qCardDragGesture = null;

        gesture.QCardGestureClear();
    }
}
