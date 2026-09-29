using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QSCustoms
{
    private static readonly IReadOnlyDictionary<string, CSCustomsMode> QSCustomsChoice =
        new Dictionary<string, CSCustomsMode>(StringComparer.Ordinal)
        {
            ["PSCustomsFresh"] = CSCustomsMode.CSCustomsModeFresh,
            ["PSCustomsMerge"] = CSCustomsMode.CSCustomsModeMerge,
            ["PSCustomsReplace"] = CSCustomsMode.CSCustomsModeReplace,
        };

    private readonly Window _qsCustomsSurface;
    private readonly CSCustoms _csCustoms;

    private QSCustoms(PWindow host, CSCustoms customs)
    {
        _qsCustomsSurface = QContract.QContractSheetFind<Window>("PSCustoms");
        _qsCustomsSurface.Owner = host.PWindowSurface;
        _csCustoms = customs;
        QSCustomsList.ItemsSource = new ObservableCollection<QSCustomsItem>(customs.CSCustomsEntry.Select(
            static (entry, index) => new QSCustomsItem(
                index,
                entry.CMarkupEntryName,
                entry.CMarkupEntryLanguage,
                entry.CMarkupEntryTarget)));
        QLookItem.QLookItemAttach(QSCustomsList, QSCustomsRowRefine);
        QSCustomsAdmit.Click += QSCustomsAcceptObserve;
        QSCustomsQuit.Click += QSCustomsCancelObserve;
        QSCustomsAdmit.IsEnabled = customs.CSCustomsReadyCheck();
    }

    private ItemsControl QSCustomsList => QContract.QContractFind<ItemsControl>(_qsCustomsSurface, "PSCustomsList");

    private Button QSCustomsAdmit => QContract.QContractFind<Button>(_qsCustomsSurface, "PSCustomsAdmit");

    private Button QSCustomsQuit => QContract.QContractFind<Button>(_qsCustomsSurface, "PSCustomsQuit");

    internal static bool QSCustomsConsult(PWindow host, CSCustoms customs)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(customs);

        return new QSCustoms(host, customs)._qsCustomsSurface.ShowDialog() == true;
    }

    internal static void QSCustomsOmissionConsult(PWindow host, IReadOnlyList<CMarkupOmission> omissions)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(omissions);

        Window surface = QContract.QContractSheetFind<Window>("PSCustoms");
        surface.Owner = host.PWindowSurface;
        ItemsControl omission = QContract.QContractFind<ItemsControl>(surface, "PSCustomsOmission");
        QLookItem.QLookItemAttach(omission, QSCustomsOmissionApply);
        omission.ItemsSource = omissions
            .Select(static line => new KeyValuePair<string, string>(line.CMarkupOmissionLine, line.CMarkupOmissionText))
            .ToList();
        QContract.QContractFind<Grid>(surface, "PSCustomsDeclaration").Visibility = Visibility.Collapsed;
        QContract.QContractFind<Grid>(surface, "PSCustomsReport").Visibility = Visibility.Visible;
        QContract.QContractFind<Button>(surface, "PSCustomsAdmit").IsDefault = false;
        QContract.QContractFind<Button>(surface, "PSCustomsQuit").IsCancel = false;
        QContract.QContractFind<Button>(surface, "PSCustomsDismiss").Click += QSCustomsCloseObserve;
        surface.ShowDialog();
    }

    private void QSCustomsRowRefine(FrameworkElement container, object item, string? _)
    {
        if (item is QSCustomsItem row)
        {
            QSCustomsRowRefine(container, row, _csCustoms.CSCustomsRowRead(row.QSCustomsItemIndex));
        }
    }

    private void QSCustomsRowRefine(FrameworkElement container, QSCustomsItem row, CSCustomsRow state)
    {
        if (QLook.QLookPartFind<TextBlock>(container, "PSCustomsNumber") is TextBlock number)
        {
            number.Text = row.QSCustomsItemNumber;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PSCustomsHeadword") is TextBlock headword)
        {
            headword.Text = row.QSCustomsItemHeadword;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PSCustomsLanguage") is TextBlock language)
        {
            language.Text = row.QSCustomsItemLanguage;
        }

        if (QLook.QLookPartFind<ComboBox>(container, "PSCustomsMode") is ComboBox mode)
        {
            QSCustomsModeApply(container, mode, state.CSCustomsRowMode);
        }

        if (QLook.QLookPartFind<ComboBox>(container, "PSCustomsTarget") is ComboBox target)
        {
            QSCustomsTargetApply(target, row, state);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PSCustomsLoss") is TextBlock loss)
        {
            loss.Text = state.CSCustomsRowLoss is string key
                ? string.Format(
                    CultureInfo.CurrentCulture,
                    QLocalizationCatalog.QLocalizationTextRead(key),
                    state.CSCustomsRowMeaning,
                    state.CSCustomsRowCollocation)
                : string.Empty;
        }
    }

    private void QSCustomsModeApply(FrameworkElement container, ComboBox mode, CSCustomsMode chosen)
    {
        mode.SelectionChanged -= QSCustomsModeObserve;
        foreach (KeyValuePair<string, CSCustomsMode> pair in QSCustomsChoice)
        {
            if (QLook.QLookPartFind<ComboBoxItem>(container, pair.Key) is ComboBoxItem choice && pair.Value == chosen)
            {
                mode.SelectedItem = choice;
            }
        }

        mode.SelectionChanged += QSCustomsModeObserve;
    }

    private void QSCustomsTargetApply(ComboBox target, QSCustomsItem row, CSCustomsRow state)
    {
        target.SelectionChanged -= QSCustomsTargetObserve;
        target.IsEnabled = state.CSCustomsRowTargeted;
        if (!ReferenceEquals(target.ItemsSource, row.QSCustomsItemCandidate))
        {
            target.ItemsSource = row.QSCustomsItemCandidate;
            QLookItem.QLookItemAttach(
                target, (container, item, _) => QSCustomsCandidateApply(container, item, row.QSCustomsItemHeadword));
        }

        target.SelectedItem = state.CSCustomsRowTarget;
        target.SelectionChanged += QSCustomsTargetObserve;
        target.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, () =>
        {
            if (target.SelectedItem is long shown)
            {
                QSCustomsCandidateApply(target, shown, row.QSCustomsItemHeadword);
            }
        });
    }

    private static void QSCustomsCandidateApply(FrameworkElement container, object item, string headword)
    {
        if (item is not long id)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PSCustomsCandidateHeadword") is TextBlock shown)
        {
            shown.Text = headword;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PSCustomsCandidateId") is TextBlock number)
        {
            number.Text = string.Concat("#", id.ToString(CultureInfo.CurrentCulture));
        }
    }

    private static void QSCustomsOmissionApply(FrameworkElement container, object item, string? _)
    {
        if (item is not KeyValuePair<string, string> omission)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PSCustomsOmissionLine") is TextBlock line)
        {
            line.Text = omission.Key;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PSCustomsOmissionText") is TextBlock text)
        {
            text.Text = omission.Value;
        }
    }

    private void QSCustomsModeObserve(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox { DataContext: QSCustomsItem item, SelectedItem: ComboBoxItem choice })
        {
            return;
        }

        QSCustomsRefine(_csCustoms.CSCustomsModeSet(item.QSCustomsItemIndex, QSCustomsChoice[choice.Name]));
    }

    private void QSCustomsTargetObserve(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox { DataContext: QSCustomsItem item, SelectedItem: long target })
        {
            return;
        }

        QSCustomsRefine(_csCustoms.CSCustomsTargetSet(item.QSCustomsItemIndex, target));
    }

    private void QSCustomsRefine(bool ready)
    {
        QSCustomsAdmit.IsEnabled = ready;
        QLookItem.QLookItemApply(QSCustomsList);
    }

    private void QSCustomsAcceptObserve(object sender, RoutedEventArgs e)
    {
        _qsCustomsSurface.DialogResult = true;
    }

    private void QSCustomsCancelObserve(object sender, RoutedEventArgs e)
    {
        _qsCustomsSurface.DialogResult = false;
    }

    private static void QSCustomsCloseObserve(object sender, RoutedEventArgs e)
    {
        Window.GetWindow((DependencyObject)sender).DialogResult = true;
    }
}
