using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QOeuvre
{
    private readonly UserControl _qOeuvreSurface;

    private readonly ObservableCollection<QShelfItem> _qOeuvreList = [];

    private CGuild _cGuild = null!;

    internal QOeuvre(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qOeuvreSurface = surface;

        QComb.SetResourceReference(QField.QFieldHintProperty, "Comb.Search");
        QComb.TextChanged += QCombObserve;
    }

    private ItemsControl QOeuvreView => QContract.QContractFind<ItemsControl>(_qOeuvreSurface, "POeuvre");

    private TextBlock QOeuvreEmpty => QContract.QContractFind<TextBlock>(_qOeuvreSurface, "POeuvreEmpty");

    private TextBox QComb => QContract.QContractFind<TextBox>(_qOeuvreSurface, "PComb");

    internal void QOeuvreIntroduce(CGuild guild, CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(guild);
        ArgumentNullException.ThrowIfNull(atelier);

        _cGuild = guild;
        _cGuild.CGuildOeuvre.COeuvrePanel.CPanelAperture.CApertureRowsChanged += QOeuvreRefine;
        atelier.CAtelierWorkspace.CWorkspaceOpened += QOeuvreRefine;

        QOeuvreView.ItemsSource = _qOeuvreList;
        QOeuvreView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QOeuvreObserve));
        QLookItem.QLookItemAttach(QOeuvreView, QShelfItem.QShelfItemRefine);
    }

    private void QOeuvreRefine()
    {
        QSplice.QSpliceRefine(
            _qOeuvreList,
            QShelfItem.QShelfItemBuild(_cGuild.CGuildOeuvre.COeuvreRowsRead()),
            QShelfItem.QShelfItemMatch,
            QShelfItem.QShelfItemSync);
        QOeuvreEmpty.SetResourceReference(TextBlock.TextProperty, _cGuild.CGuildOeuvre.COeuvreEmptyKey);
        QOeuvreEmpty.Visibility = QLook.QLookVisibleRead(
            _cGuild.CGuildOeuvre.COeuvrePanel.CPanelAperture.CApertureEmpty);
    }

    private void QCombObserve(object sender, TextChangedEventArgs e)
    {
        _cGuild.CGuildOeuvre.COeuvrePanel.CPanelAperture.CApertureQuerySet(QComb.Text);
    }

    private void QOeuvreObserve(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildSourceSelect(QSender.QSenderSourceRead<QShelfItem>(e)?.QShelfItemId);
    }
}
