using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternIncoming
{
    private readonly CDisplayCard _qLecternIncomingArea;

    private readonly CNavigation _qLecternIncomingNavigation;

    private readonly ObservableCollection<QUsageItem> _qLecternCardUsage = [];

    private readonly UIElement _qLecternCardReferral;

    public QLecternIncoming(FrameworkElement surface, CDisplayCard area, CNavigation navigation)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(area);
        ArgumentNullException.ThrowIfNull(navigation);

        _qLecternIncomingArea = area;
        _qLecternIncomingNavigation = navigation;
        ItemsControl incoming = QContract.QContractFind<ItemsControl>(surface, "PDisplayIncoming");
        _qLecternCardReferral = QContract.QContractFind<StackPanel>(surface, "PDisplayIncomingSection");
        incoming.ItemsSource = _qLecternCardUsage;

        incoming.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QLecternIncomingObserve));
    }

    public void QLecternIncomingRefine()
    {
        _qLecternCardUsage.Clear();
        foreach (CUsage usage in _qLecternIncomingArea.CDisplayIncomingRead())
        {
            _qLecternCardUsage.Add(new QUsageItem(usage));
        }

        _qLecternCardReferral.Visibility = QLook.QLookVisibleRead(_qLecternCardUsage.Count > 0);
    }

    private void QLecternIncomingObserve(object sender, RoutedEventArgs e)
    {
        _qLecternIncomingNavigation.CNavigationUsageOpen(QSender.QSenderSourceRead<QUsageItem>(e)?.QUsageItemUsage);
    }
}
