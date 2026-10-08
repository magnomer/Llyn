using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QUnit
{
    private const string QUnitTitle = "Unit.Title";

    private readonly FrameworkElement _qUnitSurface;

    private readonly ObservableCollection<QUnitItem> _qUnitItem = [];

    private CEntry _cEntry = null!;

    internal QUnit(FrameworkElement surface)
    {
        _qUnitSurface = surface;
        QUnitList.ItemsSource = _qUnitItem;
        QLookItem.QLookItemAttach(QUnitList, QUnitApply);
        QChoice.QChoiceDropperAttach(QUnitDropper, QUnitDropdown, QUnitDropper);
    }

    private ToggleButton QUnitDropper => QContract.QContractFind<ToggleButton>(_qUnitSurface, "PUnitDropper");

    private Popup QUnitDropdown => QContract.QContractFind<Popup>(_qUnitSurface, "PUnitDropdown");

    private ItemsControl QUnitList => QContract.QContractFind<ItemsControl>(_qUnitSurface, "PUnitList");

    private TextBlock QUnitName => QContract.QContractFind<TextBlock>(_qUnitSurface, "PUnitName");

    internal void QUnitIntroduce(CEntry entry)
    {
        _cEntry = entry;
    }

    internal void QUnitRefine(IReadOnlyList<(string, bool)> units)
    {
        string name = QUnitTitle;
        _qUnitItem.Clear();
        foreach ((string key, bool taken) in units)
        {
            _qUnitItem.Add(new QUnitItem(key, taken));
            if (taken)
            {
                name = key;
            }
        }

        QUnitName.SetResourceReference(TextBlock.TextProperty, name);
    }

    private void QUnitApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QUnitItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PUnitText") is TextBlock text)
        {
            text.SetResourceReference(TextBlock.TextProperty, row.QUnitItemKey);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PUnitIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("check", 12);
            icon.Visibility = QLook.QLookVisibleRead(row.QUnitItemTaken);
        }

        if (QLook.QLookPartFind<Button>(container, "PUnitChoice") is Button choice)
        {
            choice.Click -= QUnitObserve;
            choice.Click += QUnitObserve;
        }
    }

    private void QUnitObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QUnitItem item })
        {
            QUnitDropper.IsChecked = false;
            _cEntry.CEntryUnitSet(item.QUnitItemKey);
        }
    }
}
