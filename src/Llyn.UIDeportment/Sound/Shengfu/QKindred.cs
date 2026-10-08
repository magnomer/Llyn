using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QKindred
{
    private readonly UserControl _qKindredSurface;

    private readonly ObservableCollection<QKindredItem> _qKindredList = [];

    private CXiesheng _cXiesheng = null!;

    internal QKindred(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qKindredSurface = surface;

        QSextant.SetResourceReference(QField.QFieldHintProperty, "Sextant.Search");

        QLookItem.QLookItemAttach(QKindredView, QKindredItem.QKindredItemRefine);
        QKindredView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QKindredObserve));

        QSextant.TextChanged += QSextantObserve;
    }

    private TextBox QSextant => QContract.QContractFind<TextBox>(_qKindredSurface, "PSextant");

    private ItemsControl QKindredView => QContract.QContractFind<ItemsControl>(_qKindredSurface, "PKindred");

    private TextBlock QKindredEmpty => QContract.QContractFind<TextBlock>(_qKindredSurface, "PKindredEmpty");

    internal void QKindredIntroduce(CXiesheng xiesheng)
    {
        ArgumentNullException.ThrowIfNull(xiesheng);

        _cXiesheng = xiesheng;
        _cXiesheng.CXieshengKindred.CEntryListPanel.CPanelAperture.CApertureRowsChanged += QKindredRefine;

        QKindredView.ItemsSource = _qKindredList;
    }

    internal void QKindredRefine()
    {
        QKindredRefine(_cXiesheng.CXieshengKindred.CEntryListRead());
    }

    internal void QKindredRefine(IReadOnlyList<CVistaRow> rows)
    {
        QSplice.QSpliceRefine(
            _qKindredList,
            QKindredItem.QKindredItemBuild(rows),
            QKindredItem.QKindredItemMatch,
            QKindredItem.QKindredItemSync);
        QKindredEmpty.SetResourceReference(TextBlock.TextProperty, _cXiesheng.CXieshengKindredKey);
        QKindredEmpty.Visibility = QLook.QLookVisibleRead(
            _cXiesheng.CXieshengKindred.CEntryListPanel.CPanelAperture.CApertureEmpty);
    }

    private void QSextantObserve(object sender, TextChangedEventArgs e)
    {
        _cXiesheng.CXieshengKindred.CEntryListPanel.CPanelAperture.CApertureQuerySet(QSextant.Text);
    }

    private void QKindredObserve(object sender, RoutedEventArgs e)
    {
        _cXiesheng.CXieshengKindred.CEntryListPanel.CPanelRowSelect(
            QSender.QSenderSourceRead<QKindredItem>(e)?.QKindredItemId);
    }
}
