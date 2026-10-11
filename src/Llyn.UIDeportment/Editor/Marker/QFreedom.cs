using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QFreedom
{
    private const string QFreedomTitle = "Freedom.Title";

    private readonly FrameworkElement _qFreedomSurface;

    private readonly ObservableCollection<QFreedomItem> _qFreedomItem = [];

    private CEntry _cEntry = null!;

    internal QFreedom(FrameworkElement surface)
    {
        _qFreedomSurface = surface;
        QFreedomList.ItemsSource = _qFreedomItem;
        QLookItem.QLookItemAttach(QFreedomList, QFreedomApply);
        QChoice.QChoiceDropperAttach(QFreedomDropper, QFreedomDropdown, QFreedomDropper);
    }

    private ToggleButton QFreedomDropper => QContract.QContractFind<ToggleButton>(_qFreedomSurface, "PFreedomDropper");

    private Popup QFreedomDropdown => QContract.QContractFind<Popup>(_qFreedomSurface, "PFreedomDropdown");

    private ItemsControl QFreedomList => QContract.QContractFind<ItemsControl>(_qFreedomSurface, "PFreedomList");

    private TextBlock QFreedomName => QContract.QContractFind<TextBlock>(_qFreedomSurface, "PFreedomName");

    internal void QFreedomIntroduce(CEntry entry)
    {
        _cEntry = entry;
    }

    internal void QFreedomRefine(IReadOnlyList<(string, bool)> freedoms)
    {
        string name = QFreedomTitle;
        _qFreedomItem.Clear();
        foreach ((string key, bool taken) in freedoms)
        {
            _qFreedomItem.Add(new QFreedomItem(key, taken));
            if (taken)
            {
                name = key;
            }
        }

        QFreedomName.SetResourceReference(TextBlock.TextProperty, name);
    }

    private void QFreedomApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QFreedomItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PFreedomText") is TextBlock text)
        {
            text.SetResourceReference(TextBlock.TextProperty, row.QFreedomItemKey);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PFreedomIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("check", 12);
            icon.Visibility = QLook.QLookVisibleRead(row.QFreedomItemTaken);
        }

        if (QLook.QLookPartFind<Button>(container, "PFreedomChoice") is Button choice)
        {
            choice.Click -= QFreedomObserve;
            choice.Click += QFreedomObserve;
        }
    }

    private void QFreedomObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QFreedomItem item })
        {
            QFreedomDropper.IsChecked = false;
            _cEntry.CEntryFreedomSet(item.QFreedomItemKey);
        }
    }
}
