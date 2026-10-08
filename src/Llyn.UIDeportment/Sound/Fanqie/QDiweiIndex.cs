using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QDiweiIndex
{
    private readonly UserControl _qDiweiIndexSurface;

    private readonly ObservableCollection<PYunjingItem> _qShengmuList = [];

    private readonly ObservableCollection<PYunjingItem> _qYunmuList = [];

    private CYunjing _cYunjing = null!;

    internal QDiweiIndex(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qDiweiIndexSurface = surface;

        QPlumb.SetResourceReference(QField.QFieldHintProperty, "Plumb.Search");
        QFathom.SetResourceReference(QField.QFieldHintProperty, "Fathom.Search");

        QLookItem.QLookItemAttach(QShengmu, PYunjingItem.PYunjingItemRefine);
        QLookItem.QLookItemAttach(QYunmu, PYunjingItem.PYunjingItemRefine);
        QShengmu.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QDiweiIndexObserve));
        QYunmu.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QDiweiIndexObserve));

        QPlumb.TextChanged += QPlumbObserve;
        QFathom.TextChanged += QFathomObserve;
    }

    private TextBox QPlumb => QContract.QContractFind<TextBox>(_qDiweiIndexSurface, "PPlumb");

    private ItemsControl QShengmu => QContract.QContractFind<ItemsControl>(_qDiweiIndexSurface, "PShengmu");

    private TextBlock QShengmuEmpty => QContract.QContractFind<TextBlock>(_qDiweiIndexSurface, "PShengmuEmpty");

    private TextBox QFathom => QContract.QContractFind<TextBox>(_qDiweiIndexSurface, "PFathom");

    private ItemsControl QYunmu => QContract.QContractFind<ItemsControl>(_qDiweiIndexSurface, "PYunmu");

    private TextBlock QYunmuEmpty => QContract.QContractFind<TextBlock>(_qDiweiIndexSurface, "PYunmuEmpty");

    internal void QDiweiIndexIntroduce(CYunjing yunjing, CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(yunjing);
        ArgumentNullException.ThrowIfNull(atelier);

        _cYunjing = yunjing;
        _cYunjing.CYunjingDiweiOpened += QPlumbRefine;
        _cYunjing.CYunjingDiweiOpened += QFathomRefine;
        _cYunjing.CYunjingChanged += QShengmuRefine;
        _cYunjing.CYunjingChanged += QYunmuRefine;
        atelier.CAtelierWorkspace.CWorkspaceOpened += QShengmuRefine;
        atelier.CAtelierWorkspace.CWorkspaceOpened += QYunmuRefine;

        QShengmu.ItemsSource = _qShengmuList;
        QYunmu.ItemsSource = _qYunmuList;
    }

    private void QShengmuRefine()
    {
        QSplice.QSpliceRefine(
            _qShengmuList,
            PYunjingItem.PYunjingItemBuild(_cYunjing.CYunjingShengmuRead()),
            PYunjingItem.PYunjingItemMatch,
            PYunjingItem.PYunjingItemSync);
        QShengmuEmpty.SetResourceReference(TextBlock.TextProperty, _cYunjing.CYunjingShengmu.CApertureKey);
        QShengmuEmpty.Visibility = QLook.QLookVisibleRead(_cYunjing.CYunjingShengmu.CApertureEmpty);
    }

    private void QYunmuRefine()
    {
        QSplice.QSpliceRefine(
            _qYunmuList,
            PYunjingItem.PYunjingItemBuild(_cYunjing.CYunjingYunmuRead()),
            PYunjingItem.PYunjingItemMatch,
            PYunjingItem.PYunjingItemSync);
        QYunmuEmpty.SetResourceReference(TextBlock.TextProperty, _cYunjing.CYunjingYunmu.CApertureKey);
        QYunmuEmpty.Visibility = QLook.QLookVisibleRead(_cYunjing.CYunjingYunmu.CApertureEmpty);
    }

    private void QPlumbRefine()
    {
        QPlumb.Text = string.Empty;
    }

    private void QFathomRefine()
    {
        QFathom.Text = string.Empty;
    }

    private void QPlumbObserve(object sender, TextChangedEventArgs e)
    {
        _cYunjing.CYunjingShengmu.CApertureQuerySet(QPlumb.Text);
    }

    private void QFathomObserve(object sender, TextChangedEventArgs e)
    {
        _cYunjing.CYunjingYunmu.CApertureQuerySet(QFathom.Text);
    }

    private void QDiweiIndexObserve(object sender, RoutedEventArgs e)
    {
        _cYunjing.CYunjingDiweiSelect(
            QSender.QSenderSourceRead<PYunjingItem>(e)?.PYunjingItemId,
            QSender.QSenderSourceRead<PYunjingItem>(e)?.PYunjingItemFinal);
    }
}
