using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PSCustoms : Window
{
    private readonly LWindow? _lWindow;
    private readonly ObservableCollection<PSCustomsItem> _psCustomsItem = [];
    private bool _psCustomsAccepted;

    private PSCustoms(Window owner, LWindow? window)
    {
        InitializeComponent();
        Owner = owner;
        _lWindow = window;
        PSCustomsList.ItemsSource = _psCustomsItem;
    }

    internal static IReadOnlyList<LMarkupIntake>? PSCustomsShow(
        Window owner, LWindow window, IReadOnlyList<LMarkupEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(entries);

        PSCustoms dialog = new(owner, window);
        for (int index = 0; index < entries.Count; index++)
        {
            PSCustomsItem item = new(index, entries[index], window.LWindowMarkupFind(entries[index]));
            item.PSCustomsItemLoss = dialog.PSCustomsLossFormat(item);
            dialog._psCustomsItem.Add(item);
        }

        dialog.PSCustomsAcceptUpdate();
        dialog.ShowDialog();
        if (!dialog._psCustomsAccepted)
        {
            return null;
        }

        List<LMarkupIntake> intakes = new(dialog._psCustomsItem.Count);
        foreach (PSCustomsItem item in dialog._psCustomsItem)
        {
            intakes.Add(LSCustoms.LSCustomsIntakeCreate(
                item.PSCustomsItemIndex,
                item.PSCustomsItemMode,
                item.PSCustomsItemTarget));
        }

        return intakes;
    }

    internal static void PSCustomsOmissionShow(Window owner, IReadOnlyList<LMarkupOmission> omissions)
    {
        ArgumentNullException.ThrowIfNull(omissions);

        if (omissions.Count == 0)
        {
            return;
        }

        PSCustoms dialog = new(owner, null);
        dialog.PSCustomsOmission.ItemsSource = omissions;
        dialog.PSCustomsDeclaration.Visibility = Visibility.Collapsed;
        dialog.PSCustomsReport.Visibility = Visibility.Visible;
        dialog.PSCustomsAdmit.IsDefault = false;
        dialog.PSCustomsQuit.IsCancel = false;
        dialog.ShowDialog();
    }

    private void PSCustomsModeHandle(object sender, SelectionChangedEventArgs e)
    {
        PSCustomsRowUpdate(sender);
    }

    private void PSCustomsTargetHandle(object sender, SelectionChangedEventArgs e)
    {
        PSCustomsRowUpdate(sender);
    }

    private void PSCustomsAcceptHandle(object sender, RoutedEventArgs e)
    {
        _psCustomsAccepted = true;
        DialogResult = true;
    }

    private void PSCustomsCancelHandle(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void PSCustomsCloseHandle(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void PSCustomsRowUpdate(object sender)
    {
        if (sender is not ComboBox { DataContext: PSCustomsItem item })
        {
            return;
        }

        item.PSCustomsItemLoss = PSCustomsLossFormat(item);
        PSCustomsAcceptUpdate();
    }

    private void PSCustomsAcceptUpdate()
    {
        bool ready = true;
        foreach (PSCustomsItem item in _psCustomsItem)
        {
            ready &= item.PSCustomsItemReady;
        }

        PSCustomsAdmit.IsEnabled = ready;
    }

    private string PSCustomsLossFormat(PSCustomsItem item)
    {
        return LSCustoms.LSCustomsLossResolve(
            _lWindow,
            item.PSCustomsItemMode,
            item.PSCustomsItemTarget,
            TryFindResource("Customs.Loss") as string ?? "Customs.Loss");
    }
}
