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
        _qDrawerView.SelectedIndex = -1;
    }

    internal void QDrawerHide()
    {
        _qDrawerPopup.IsOpen = false;
        _qDrawerView.SelectedIndex = -1;
        _qDrawerList.Clear();
    }

    internal void QDrawerMove(bool down)
    {
        int? lit = CLantern.CLanternMove(_qDrawerView.SelectedIndex, _qDrawerView.Items.Count, down ? 1 : -1);
        if (lit is not int chosen)
        {
            return;
        }

        _qDrawerView.SelectedIndex = chosen;
        _qDrawerView.ScrollIntoView(_qDrawerView.SelectedItem);
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
