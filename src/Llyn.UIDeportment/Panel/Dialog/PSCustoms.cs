using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Llyn.Core;

namespace Llyn.UIDeportment;

public class PSCustoms
{
    private readonly Window _psCustomsSurface;
    private readonly LWindow? _lWindow;
    private readonly ObservableCollection<PSCustomsItem> _psCustomsItem = [];
    private bool _psCustomsAccepted;

    private PSCustoms(Window owner, LWindow? window)
    {
        _psCustomsSurface = (Window)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Dialog/PSCustoms.xaml", UriKind.Relative));
        _psCustomsSurface.Owner = owner;
        _lWindow = window;
        PSCustomsList.ItemsSource = _psCustomsItem;
        PLookItem.PLookItemAttach(PSCustomsList, PSCustomsRowApply);
        PLookItem.PLookItemAttach(PSCustomsOmission, PSCustomsOmissionApply);
        PSCustomsAdmit.Click += PSCustomsAcceptHandle;
        PSCustomsQuit.Click += PSCustomsCancelHandle;
        PSCustomsDismiss.Click += PSCustomsCloseHandle;
    }

    private ItemsControl PSCustomsList => (ItemsControl)_psCustomsSurface.FindName(nameof(PSCustomsList));

    private ItemsControl PSCustomsOmission => (ItemsControl)_psCustomsSurface.FindName(nameof(PSCustomsOmission));

    private Grid PSCustomsDeclaration => (Grid)_psCustomsSurface.FindName(nameof(PSCustomsDeclaration));

    private Grid PSCustomsReport => (Grid)_psCustomsSurface.FindName(nameof(PSCustomsReport));

    private Button PSCustomsAdmit => (Button)_psCustomsSurface.FindName(nameof(PSCustomsAdmit));

    private Button PSCustomsQuit => (Button)_psCustomsSurface.FindName(nameof(PSCustomsQuit));

    private Button PSCustomsDismiss => (Button)_psCustomsSurface.FindName(nameof(PSCustomsDismiss));

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
        dialog._psCustomsSurface.ShowDialog();
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
        dialog._psCustomsSurface.ShowDialog();
    }

    private void PSCustomsRowApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PSCustomsItem row)
        {
            return;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PSCustomsNumber") is TextBlock number)
        {
            number.Text = row.PSCustomsItemNumber;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PSCustomsHeadword") is TextBlock headword)
        {
            headword.Text = row.PSCustomsItemHeadword;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PSCustomsLanguage") is TextBlock language)
        {
            language.Text = row.PSCustomsItemLanguage;
        }

        if (PLook.PLookPartFind<ComboBox>(container, "PSCustomsMode") is ComboBox mode)
        {
            PSCustomsModeApply(container, mode, row);
        }

        if (PLook.PLookPartFind<ComboBox>(container, "PSCustomsTarget") is ComboBox target)
        {
            PSCustomsTargetApply(target, row);
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PSCustomsLoss") is TextBlock loss)
        {
            loss.Text = row.PSCustomsItemLoss;
        }
    }

    private void PSCustomsModeApply(FrameworkElement container, ComboBox mode, PSCustomsItem row)
    {
        (string, LMarkupMode)[] choices =
        [
            ("PSCustomsFresh", LMarkupMode.LMarkupModeNew),
            ("PSCustomsMerge", LMarkupMode.LMarkupModeMerge),
            ("PSCustomsReplace", LMarkupMode.LMarkupModeReplace),
        ];
        foreach ((string name, LMarkupMode value) in choices)
        {
            if (PLook.PLookPartFind<ComboBoxItem>(container, name) is ComboBoxItem choice)
            {
                choice.Tag = value;
                if (value == row.PSCustomsItemMode && !ReferenceEquals(mode.SelectedItem, choice))
                {
                    mode.SelectedItem = choice;
                }
            }
        }

        mode.SelectionChanged -= PSCustomsModeHandle;
        mode.SelectionChanged += PSCustomsModeHandle;
    }

    private void PSCustomsTargetApply(ComboBox target, PSCustomsItem row)
    {
        target.IsEnabled = row.PSCustomsItemTargeted;
        if (!ReferenceEquals(target.ItemsSource, row.PSCustomsItemCandidate))
        {
            target.ItemsSource = row.PSCustomsItemCandidate;
            PLookItem.PLookItemAttach(target, PSCustomsCandidateApply);
        }

        LEntry? chosen = row.PSCustomsItemCandidate.FirstOrDefault(entry => entry.LEntryId == row.PSCustomsItemTarget);
        if (chosen is not null && !ReferenceEquals(target.SelectedItem, chosen))
        {
            target.SelectedItem = chosen;
        }

        target.SelectionChanged -= PSCustomsTargetHandle;
        target.SelectionChanged += PSCustomsTargetHandle;
        target.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, () =>
        {
            if (target.SelectedItem is LEntry shown)
            {
                PSCustomsCandidateApply(target, shown, null);
            }
        });
    }

    private static void PSCustomsCandidateApply(FrameworkElement container, object item, string? _)
    {
        if (item is not LEntry entry)
        {
            return;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PSCustomsCandidateHeadword") is TextBlock headword)
        {
            headword.Text = entry.LEntryHeadword;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PSCustomsCandidateId") is TextBlock id)
        {
            id.Text = string.Concat("#", entry.LEntryId.ToString(CultureInfo.CurrentCulture));
        }
    }

    private static void PSCustomsOmissionApply(FrameworkElement container, object item, string? _)
    {
        if (item is not LMarkupOmission omission)
        {
            return;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PSCustomsOmissionLine") is TextBlock line)
        {
            line.Text = omission.LMarkupOmissionLine.ToString(CultureInfo.CurrentCulture);
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PSCustomsOmissionText") is TextBlock text)
        {
            text.Text = omission.LMarkupOmissionText;
        }
    }

    private void PSCustomsModeHandle(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox
            {
                DataContext: PSCustomsItem item, SelectedItem: ComboBoxItem { Tag: LMarkupMode mode },
            })
        {
            item.PSCustomsItemMode = mode;
        }

        PSCustomsRowUpdate(sender);
    }

    private void PSCustomsTargetHandle(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox { DataContext: PSCustomsItem item, SelectedItem: LEntry entry })
        {
            item.PSCustomsItemTarget = entry.LEntryId;
        }

        PSCustomsRowUpdate(sender);
    }

    private void PSCustomsAcceptHandle(object sender, RoutedEventArgs e)
    {
        _psCustomsAccepted = true;
        _psCustomsSurface.DialogResult = true;
    }

    private void PSCustomsCancelHandle(object sender, RoutedEventArgs e)
    {
        _psCustomsSurface.DialogResult = false;
    }

    private void PSCustomsCloseHandle(object sender, RoutedEventArgs e)
    {
        _psCustomsSurface.DialogResult = true;
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
            _psCustomsSurface.TryFindResource("Customs.Loss") as string ?? "Customs.Loss");
    }
}
