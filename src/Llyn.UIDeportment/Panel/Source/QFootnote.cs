using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QFootnote
{
    private readonly UserControl _qFootnoteSurface;

    private readonly ObservableCollection<QFootnoteItem> _qFootnoteList = [];

    private CShelf _cShelf = null!;

    internal QFootnote(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qFootnoteSurface = surface;

        QRummage.SetResourceReference(QField.QFieldHintProperty, "Rummage.Search");
        QRummage.TextChanged += QRummageObserve;
    }

    private ItemsControl QFootnoteView => QContract.QContractFind<ItemsControl>(_qFootnoteSurface, "PFootnote");

    private TextBlock QFootnoteEmpty => QContract.QContractFind<TextBlock>(_qFootnoteSurface, "PFootnoteEmpty");

    private TextBox QRummage => QContract.QContractFind<TextBox>(_qFootnoteSurface, "PRummage");

    internal void QFootnoteIntroduce(CShelf shelf, CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(shelf);
        ArgumentNullException.ThrowIfNull(atelier);

        _cShelf = shelf;
        _cShelf.CShelfFootnote.CFootnotePanel.CPanelAperture.CApertureRowsChanged += QFootnoteRefine;
        atelier.CAtelierWorkspace.CWorkspaceOpened += QFootnoteVistaRefine;

        QFootnoteView.ItemsSource = _qFootnoteList;
        QFootnoteView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QFootnoteObserve));
        QLookItem.QLookItemAttach(QFootnoteView, QFootnoteItem.QFootnoteItemRefine);
    }

    private async void QFootnoteVistaRefine()
    {
        QFootnoteRefine(
            (await _cShelf.CShelfFootnote.CFootnoteRowsLoad(QEnsignImage.QEnsignDraw)).CEnsignSheetRows);
    }

    private void QFootnoteRefine()
    {
        QFootnoteRefine(_cShelf.CShelfFootnote.CFootnoteRowsRead());
    }

    private void QFootnoteRefine(IReadOnlyList<CVistaRow> rows)
    {
        QSplice.QSpliceRefine(
            _qFootnoteList,
            QFootnoteItem.QFootnoteItemBuild(rows),
            QFootnoteItem.QFootnoteItemMatch,
            QFootnoteItem.QFootnoteItemSync);
        QFootnoteEmpty.SetResourceReference(
            TextBlock.TextProperty, _cShelf.CShelfFootnote.CFootnotePanel.CPanelAperture.CApertureKey);
        QFootnoteEmpty.Visibility = QLook.QLookVisibleRead(_qFootnoteList.Count == 0);
    }

    private void QRummageObserve(object sender, TextChangedEventArgs e)
    {
        _cShelf.CShelfFootnote.CFootnotePanel.CPanelAperture.CApertureQuerySet(QRummage.Text);
    }

    private void QFootnoteObserve(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfEntrySelect(QSender.QSenderSourceRead<QFootnoteItem>(e)?.QFootnoteItemId);
    }
}
