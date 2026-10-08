using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QShelf
{
    private readonly UserControl _qShelfSurface;

    private readonly ObservableCollection<QShelfItem> _qShelfList = [];

    private CShelf _cShelf = null!;

    private QColophon _qShelfColophon = null!;

    internal QShelf(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qShelfSurface = surface;

        QSurvey.SetResourceReference(QField.QFieldHintProperty, "Source.Search");
        QSurvey.TextChanged += QSurveyObserve;
    }

    private ItemsControl QShelfView => QContract.QContractFind<ItemsControl>(_qShelfSurface, "PShelf");

    private TextBlock QShelfEmpty => QContract.QContractFind<TextBlock>(_qShelfSurface, "PShelfEmpty");

    private TextBox QSurvey => QContract.QContractFind<TextBox>(_qShelfSurface, "PSurvey");

    internal void QShelfIntroduce(CShelf shelf, QColophon colophon)
    {
        ArgumentNullException.ThrowIfNull(shelf);
        ArgumentNullException.ThrowIfNull(colophon);

        _cShelf = shelf;
        _qShelfColophon = colophon;
        _cShelf.CShelfPanel.CPanelAperture.CApertureRowsChanged += QShelfRefine;

        QShelfView.ItemsSource = _qShelfList;
        QShelfView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QShelfObserve));
        QLookItem.QLookItemAttach(QShelfView, QShelfItem.QShelfItemRefine);
    }

    internal void QShelfRefine(CShelfRoll roll)
    {
        QSplice.QSpliceRefine(
            _qShelfList,
            QShelfItem.QShelfItemBuild(roll.CShelfRollRows),
            QShelfItem.QShelfItemMatch,
            QShelfItem.QShelfItemSync);
        QShelfEmpty.Visibility = QLook.QLookVisibleRead(roll.CShelfRollEmpty);
        _qShelfColophon.QColophonTallyRefine(roll.CShelfRollTally);
    }

    private void QShelfRefine()
    {
        QShelfRefine(_cShelf.CShelfRollRead());
    }

    private void QSurveyObserve(object sender, TextChangedEventArgs e)
    {
        _cShelf.CShelfPanel.CPanelAperture.CApertureQuerySet(QSurvey.Text);
    }

    private void QShelfObserve(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfReferenceSelect(QSender.QSenderSourceRead<QShelfItem>(e)?.QShelfItemId);
    }
}
