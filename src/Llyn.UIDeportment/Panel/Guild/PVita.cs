using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIDeportment;

public class PVita : UserControl
{
    private PWindow _pVitaHost = null!;

    private LGuild _lGuild = null!;

    public PVita()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Guild/PVita.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        PFellow.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PFellowHandle));
        PVitaCitation.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PVitaCitationHandle));

        PLookItem.PLookItemAttach(PFellow, PFellowItem.PFellowItemApply);
        PLookItem.PLookItemAttach(PVitaCitation, LUsageItem.LUsageItemApply);
    }

    private StackPanel PVitaBody => (StackPanel)FindName(nameof(PVitaBody));

    private TextBlock PVitaName => (TextBlock)FindName(nameof(PVitaName));

    private TextBlock PVitaWork => (TextBlock)FindName(nameof(PVitaWork));

    private TextBlock PVitaTally => (TextBlock)FindName(nameof(PVitaTally));

    private StackPanel PVitaFellowSection => (StackPanel)FindName(nameof(PVitaFellowSection));

    private ItemsControl PFellow => (ItemsControl)FindName(nameof(PFellow));

    private StackPanel PVitaCitationSection => (StackPanel)FindName(nameof(PVitaCitationSection));

    private ItemsControl PVitaCitation => (ItemsControl)FindName(nameof(PVitaCitation));

    private TextBlock PVitaUnselected => (TextBlock)FindName(nameof(PVitaUnselected));

    internal void PVitaAttach(PWindow host, LGuild guild)
    {
        _pVitaHost = host;
        _lGuild = guild;
    }

    internal void PVitaShow(LVita vita, bool held)
    {
        ArgumentNullException.ThrowIfNull(vita);

        PVitaName.Text = vita.LVitaName;
        PField.PFieldPlaceholderShow(PVitaName, !vita.LVitaNamed);
        PVitaWork.Text = vita.LVitaWork;
        PVitaTally.Text = vita.LVitaTally;
        PFellow.ItemsSource = PFellowItem.PFellowItemBuild(vita.LVitaFellows);
        PVitaCitation.ItemsSource = LUsageItem.LUsageItemBuild(vita.LVitaUsages);
        PVitaFellowSection.Visibility = PLook.PLookVisibleRead(vita.LVitaFellowShown);
        PVitaCitationSection.Visibility = PLook.PLookVisibleRead(vita.LVitaUsageShown);
        PVitaBody.Visibility = PLook.PLookVisibleRead(held);
        PVitaUnselected.Visibility = PLook.PLookVisibleRead(!held);
    }

    private void PFellowHandle(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildRowSelect(PSender.PSenderSourceRead<PFellowItem>(e)?.PFellowItemId);
    }

    private void PVitaCitationHandle(object sender, RoutedEventArgs e)
    {
        PSender.PSenderSourceRead<LUsageItem>(e)?.LUsageItemShow(
            _pVitaHost.PWindowExampleShow,
            _pVitaHost.PWindowEntryShow);
    }
}
