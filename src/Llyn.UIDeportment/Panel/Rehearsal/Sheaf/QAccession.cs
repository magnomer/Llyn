using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QAccession
{
    private readonly UserControl _qAccessionSurface;

    private readonly ObservableCollection<QAccessionItem> _qAccessionList = [];

    private CAccession _cAccession = null!;

    internal QAccession(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qAccessionSurface = surface;
        QTrawl.SetResourceReference(QField.QFieldHintProperty, "Accession.Search");
        QTrawl.TextChanged += QAccessionTrawlObserve;
        QAccessionBulk.Click += QAccessionBulkObserve;
        QAccessionRows.ItemsSource = _qAccessionList;
        QLookItem.QLookItemAttach(QAccessionRows, QAccessionItemRefine);
    }

    private TextBox QTrawl => QContract.QContractFind<TextBox>(_qAccessionSurface, "PTrawl");

    private Button QAccessionBulk => QContract.QContractFind<Button>(_qAccessionSurface, "PAccessionBulk");

    private ItemsControl QAccessionRows => QContract.QContractFind<ItemsControl>(_qAccessionSurface, "PAccessionRows");

    private TextBlock QAccessionEmpty => QContract.QContractFind<TextBlock>(_qAccessionSurface, "PAccessionEmpty");

    private TextBlock QAccessionTally => QContract.QContractFind<TextBlock>(_qAccessionSurface, "PAccessionTally");

    internal void QAccessionIntroduce(CAccession accession)
    {
        ArgumentNullException.ThrowIfNull(accession);

        _cAccession = accession;
        _cAccession.CAccessionAperture.CApertureRowsChanged += QAccessionRefine;
        _cAccession.CAccessionAperture.CApertureRowsChanged += QAccessionTallyRefine;
        _cAccession.CAccessionChanged += QAccessionRefine;
    }

    internal void QAccessionRefine()
    {
        QSplice.QSpliceRefine(
            _qAccessionList,
            [.. _cAccession.CAccessionRead().Select(static row => new QAccessionItem(row))],
            QAccessionItem.QAccessionItemMatch,
            QAccessionItem.QAccessionItemSync);
        CAperture aperture = _cAccession.CAccessionAperture;
        QAccessionEmpty.SetResourceReference(TextBlock.TextProperty, aperture.CApertureKey);
        QAccessionEmpty.Visibility = QLook.QLookVisibleRead(aperture.CApertureEmpty);
    }

    internal void QAccessionTallyRefine()
    {
        QAccessionTally.Text = _cAccession.CAccessionAperture.CApertureTallyRead();
    }

    private void QAccessionItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QAccessionItem accession)
        {
            return;
        }

        CAccessionRow row = accession.QAccessionItemRow;
        if (QLook.QLookPartFind<TextBlock>(container, "PAccessionText") is TextBlock text)
        {
            text.Text = row.CAccessionRowHeadword;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAccessionEpithet") is TextBlock epithet)
        {
            epithet.Text = QLook.QLookEpithetRead(row.CAccessionRowEpithet);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAccessionBound") is TextBlock bound)
        {
            bound.Visibility = QLook.QLookVisibleRead(row.CAccessionRowBound);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PAccessionAdditionIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("add", 12);
        }

        if (QLook.QLookPartFind<Button>(container, "PAccessionAddition") is Button addition)
        {
            addition.Visibility = QLook.QLookVisibleRead(!row.CAccessionRowBound);
            addition.Click -= QAccessionAdditionObserve;
            addition.Click += QAccessionAdditionObserve;
        }
    }

    private void QAccessionTrawlObserve(object sender, TextChangedEventArgs e)
    {
        _cAccession.CAccessionAperture.CApertureQuerySet(QTrawl.Text);
    }

    private void QAccessionBulkObserve(object sender, RoutedEventArgs e)
    {
        _cAccession.CAccessionShownAdd();
    }

    private void QAccessionAdditionObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<QAccessionItem>(sender) is QAccessionItem accession)
        {
            _cAccession.CAccessionAdd(accession.QAccessionItemRow.CAccessionRowEntry);
        }
    }
}
