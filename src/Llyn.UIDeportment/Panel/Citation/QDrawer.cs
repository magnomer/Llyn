using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QDrawer
{
    private const double QDrawerShade = 10;
    private const double QDrawerGap = 6;

    private readonly ObservableCollection<QDrawerItem> _qDrawerList = [];

    private readonly Popup _qDrawerPopup;

    private readonly Border _qDrawerSheet;

    private readonly ListBox _qDrawerView;

    private int _qDrawerChosen = -1;

    internal QDrawer(Popup popup, Border sheet, ListBox view, MouseButtonEventHandler press)
    {
        ArgumentNullException.ThrowIfNull(popup);
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(press);

        _qDrawerPopup = popup;
        _qDrawerSheet = sheet;
        _qDrawerView = view;
        view.ItemsSource = _qDrawerList;
        popup.CustomPopupPlacementCallback = QDrawerPlace;
        QLookItem.QLookItemAttach(view, (container, item, _) => QDrawerItem.QDrawerItemApply(container, item, press));
    }

    internal bool QDrawerShown => _qDrawerPopup.IsOpen;

    internal void QDrawerShow(CProffer offer, TextBox field)
    {
        ArgumentNullException.ThrowIfNull(offer);
        ArgumentNullException.ThrowIfNull(field);

        _qDrawerList.Clear();
        foreach (CProfferRow row in offer.CProfferRows)
        {
            _qDrawerList.Add(new QDrawerItem(row));
        }

        if (!offer.CProfferShown)
        {
            QDrawerHide();
            return;
        }

        FrameworkElement anchor = QDrawerFrameFind(field) ?? field;
        _qDrawerPopup.PlacementTarget = anchor;
        _qDrawerSheet.MinWidth = anchor.ActualWidth;
        _qDrawerPopup.IsOpen = true;
        QDrawerChosenSet(-1);
    }

    internal void QDrawerHide()
    {
        _qDrawerPopup.IsOpen = false;
        QDrawerChosenSet(-1);
        _qDrawerList.Clear();
    }

    internal void QDrawerMove(bool down)
    {
        int count = _qDrawerList.Count;
        if (count == 0)
        {
            return;
        }

        int chosen = _qDrawerChosen < 0 ? (down ? count - 1 : 0) : _qDrawerChosen;
        QDrawerChosenSet((chosen + (down ? 1 : count - 1)) % count);
        _qDrawerView.ScrollIntoView(_qDrawerList[_qDrawerChosen]);
    }

    private void QDrawerChosenSet(int chosen)
    {
        _qDrawerChosen = chosen;
        _qDrawerView.SelectedIndex = chosen;
    }

    private static CustomPopupPlacement[] QDrawerPlace(Size popup, Size target, Point offset)
    {
        return
        [
            new CustomPopupPlacement(
                new Point(-QDrawerShade, target.Height + QDrawerGap - QDrawerShade), PopupPrimaryAxis.Vertical),
            new CustomPopupPlacement(
                new Point(-QDrawerShade, QDrawerShade - QDrawerGap - popup.Height), PopupPrimaryAxis.Vertical),
        ];
    }

    private static FrameworkElement? QDrawerFrameFind(TextBox box)
    {
        box.ApplyTemplate();
        return box.Template?.FindName(QField.QFieldSurfaceName, box) as FrameworkElement;
    }
}
