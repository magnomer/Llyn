using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QXiaoyun
{
    private readonly UserControl _qXiaoyunSurface;

    private readonly ObservableCollection<QXiaoyunItem> _qXiaoyunList = [];

    private CYunjing _cYunjing = null!;

    internal QXiaoyun(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qXiaoyunSurface = surface;

        QBeacon.SetResourceReference(QField.QFieldHintProperty, "Beacon.Search");

        QLookItem.QLookItemAttach(QXiaoyunView, QXiaoyunItem.QXiaoyunItemRefine);
        QXiaoyunView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QXiaoyunObserve));

        QBeacon.TextChanged += QBeaconObserve;
    }

    private TextBox QBeacon => QContract.QContractFind<TextBox>(_qXiaoyunSurface, "PBeacon");

    private ItemsControl QXiaoyunView => QContract.QContractFind<ItemsControl>(_qXiaoyunSurface, "PXiaoyun");

    private TextBlock QXiaoyunEmpty => QContract.QContractFind<TextBlock>(_qXiaoyunSurface, "PXiaoyunEmpty");

    internal void QXiaoyunIntroduce(CYunjing yunjing)
    {
        ArgumentNullException.ThrowIfNull(yunjing);

        _cYunjing = yunjing;
        _cYunjing.CYunjingXiaoyun.CEntryListPanel.CPanelAperture.CApertureRowsChanged += QXiaoyunRefine;

        QXiaoyunView.ItemsSource = _qXiaoyunList;
    }

    internal void QXiaoyunRefine()
    {
        QXiaoyunRefine(_cYunjing.CYunjingXiaoyun.CEntryListRead());
    }

    internal void QXiaoyunRefine(IReadOnlyList<CVistaRow> rows)
    {
        QSplice.QSpliceRefine(
            _qXiaoyunList,
            QXiaoyunItem.QXiaoyunItemBuild(rows),
            QXiaoyunItem.QXiaoyunItemMatch,
            QXiaoyunItem.QXiaoyunItemSync);
        QXiaoyunEmpty.SetResourceReference(TextBlock.TextProperty, _cYunjing.CYunjingXiaoyunKey);
        QXiaoyunEmpty.Visibility = QLook.QLookVisibleRead(
            _cYunjing.CYunjingXiaoyun.CEntryListPanel.CPanelAperture.CApertureEmpty);
    }

    private void QBeaconObserve(object sender, TextChangedEventArgs e)
    {
        _cYunjing.CYunjingXiaoyun.CEntryListPanel.CPanelAperture.CApertureQuerySet(QBeacon.Text);
    }

    private void QXiaoyunObserve(object sender, RoutedEventArgs e)
    {
        _cYunjing.CYunjingXiaoyun.CEntryListPanel.CPanelRowSelect(
            QSender.QSenderSourceRead<QXiaoyunItem>(e)?.QXiaoyunItemId);
    }
}
