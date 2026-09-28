using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QVita
{
    private readonly UserControl _qVitaSurface;

    private PWindow _qVitaHost = null!;

    private CGuild _cGuild = null!;

    internal QVita(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qVitaSurface = surface;

        QFellow.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QFellowHandle));
        QVitaCitation.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QVitaCitationHandle));

        QLookItem.QLookItemAttach(QFellow, QFellowItem.QFellowItemApply);
        QLookItem.QLookItemAttach(QVitaCitation, QUsageItem.QUsageItemRefine);
    }

    private StackPanel QVitaBody => QContract.QContractFind<StackPanel>(_qVitaSurface, "PVitaBody");

    private TextBlock QVitaName => QContract.QContractFind<TextBlock>(_qVitaSurface, "PVitaName");

    private TextBlock QVitaWork => QContract.QContractFind<TextBlock>(_qVitaSurface, "PVitaWork");

    private TextBlock QVitaTally => QContract.QContractFind<TextBlock>(_qVitaSurface, "PVitaTally");

    private StackPanel QVitaFellowSection => QContract.QContractFind<StackPanel>(_qVitaSurface, "PVitaFellowSection");

    private ItemsControl QFellow => QContract.QContractFind<ItemsControl>(_qVitaSurface, "PFellow");

    private StackPanel QVitaCitationSection =>
        QContract.QContractFind<StackPanel>(_qVitaSurface, "PVitaCitationSection");

    private ItemsControl QVitaCitation => QContract.QContractFind<ItemsControl>(_qVitaSurface, "PVitaCitation");

    private TextBlock QVitaUnselected => QContract.QContractFind<TextBlock>(_qVitaSurface, "PVitaUnselected");

    internal void QVitaAttach(PWindow host, CGuild guild)
    {
        _qVitaHost = host;
        _cGuild = guild;
    }

    internal void QVitaShow(CVita vita, bool held)
    {
        ArgumentNullException.ThrowIfNull(vita);

        QVitaName.Text = vita.CVitaName;
        QField.QFieldPlaceholderShow(QVitaName, !vita.CVitaNamed);
        QVitaWork.Text = vita.CVitaWork;
        QVitaTally.Text = vita.CVitaTally;
        QFellow.ItemsSource = QFellowItem.QFellowItemBuild(vita.CVitaFellows);
        QVitaCitation.ItemsSource = QUsageItem.QUsageItemBuild(vita.CVitaUsages);
        QVitaFellowSection.Visibility = QLook.QLookVisibleRead(vita.CVitaFellowShown);
        QVitaCitationSection.Visibility = QLook.QLookVisibleRead(vita.CVitaUsageShown);
        QVitaBody.Visibility = QLook.QLookVisibleRead(held);
        QVitaUnselected.Visibility = QLook.QLookVisibleRead(!held);
    }

    private void QFellowHandle(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildAuthorSelect(QSender.QSenderSourceRead<QFellowItem>(e)?.QFellowItemId);
    }

    private void QVitaCitationHandle(object sender, RoutedEventArgs e)
    {
        _qVitaHost.PWindowAtelier.CAtelierNavigation.CNavigationUsageOpen(
            QSender.QSenderSourceRead<QUsageItem>(e)?.QUsageItemUsage);
    }
}
