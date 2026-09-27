using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    private readonly PWindow _qsCustomsHost;
    private readonly CSCustoms _csCustoms;

    private QSCustoms(PWindow host, CSCustoms customs, IReadOnlyList<QSCustomsItem> items)
    {
        _qsCustomsSurface = QContract.QContractSheetFind<Window>("PSCustoms");
        _qsCustomsSurface.Owner = host.PWindowSurface;
        _qsCustomsHost = host;
        _csCustoms = customs;
        QSCustomsList.ItemsSource = new ObservableCollection<QSCustomsItem>(items);
        QLookItem.QLookItemAttach(QSCustomsList, QSCustomsRowApply);
        QLookItem.QLookItemAttach(QSCustomsOmission, QSCustomsOmissionApply);
        QSCustomsAdmit.Click += QSCustomsAcceptHandle;
        QSCustomsQuit.Click += QSCustomsCancelHandle;
        QSCustomsDismiss.Click += QSCustomsCloseHandle;
        QSCustomsAdmit.IsEnabled = customs.CSCustomsReadyCheck();
    }

    private ItemsControl QSCustomsList => QContract.QContractFind<ItemsControl>(_qsCustomsSurface, "PSCustomsList");

    private ItemsControl QSCustomsOmission =>
        QContract.QContractFind<ItemsControl>(_qsCustomsSurface, "PSCustomsOmission");

    private Grid QSCustomsDeclaration => QContract.QContractFind<Grid>(_qsCustomsSurface, "PSCustomsDeclaration");

    private Grid QSCustomsReport => QContract.QContractFind<Grid>(_qsCustomsSurface, "PSCustomsReport");

    private Button QSCustomsAdmit => QContract.QContractFind<Button>(_qsCustomsSurface, "PSCustomsAdmit");

    private Button QSCustomsQuit => QContract.QContractFind<Button>(_qsCustomsSurface, "PSCustomsQuit");

    private Button QSCustomsDismiss => QContract.QContractFind<Button>(_qsCustomsSurface, "PSCustomsDismiss");

    internal static IReadOnlyList<CSCustomsRow>? QSCustomsShow(PWindow host, IReadOnlyList<CMarkupEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(entries);

        List<IReadOnlyList<long>> candidates = [];
        List<QSCustomsItem> items = [];
        int index = 0;
        foreach (CMarkupEntry entry in entries)
        {
            IReadOnlyList<long> found = host.PWindowDeportment.LWindowWorkspace.QWorkspaceMarkupFind(
                entry.CMarkupEntryHeadword, entry.CMarkupEntryLanguage);
            candidates.Add(found);
            items.Add(new QSCustomsItem(index, entry.CMarkupEntryName, entry.CMarkupEntryLanguage, found));
            index++;
        }

        return QSCustomsIntakeRead(host, new CSCustoms(candidates), items);
    }

    internal static void QSCustomsOmissionShow(PWindow host, IReadOnlyList<CMarkupOmission> omissions)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(omissions);

        List<KeyValuePair<string, string>> lines = [];
        foreach (CMarkupOmission omission in omissions)
        {
            lines.Add(new KeyValuePair<string, string>(
                omission.CMarkupOmissionLine.ToString(CultureInfo.CurrentCulture), omission.CMarkupOmissionText));
        }

        if (lines.Count == 0)
        {
            return;
        }

        QSCustoms dialog = new(host, new CSCustoms([]), []);
        dialog.QSCustomsOmission.ItemsSource = lines;
        dialog.QSCustomsDeclaration.Visibility = Visibility.Collapsed;
        dialog.QSCustomsReport.Visibility = Visibility.Visible;
        dialog.QSCustomsAdmit.IsDefault = false;
        dialog.QSCustomsQuit.IsCancel = false;
        dialog._qsCustomsSurface.ShowDialog();
    }

    private static IReadOnlyList<CSCustomsRow>? QSCustomsIntakeRead(
        PWindow host, CSCustoms customs, IReadOnlyList<QSCustomsItem> items)
    {
        bool accepted = new QSCustoms(host, customs, items)._qsCustomsSurface.ShowDialog() == true;
        List<CSCustomsRow> rows = new(items.Count);
        foreach (QSCustomsItem item in items)
        {
            rows.Add(customs.CSCustomsRowRead(item.QSCustomsItemIndex));
        }

        return accepted ? rows : null;
    }

    private static IReadOnlyList<CCardDraft> QSCustomsChildRead(CCardDraft card)
    {
        return card.CCardDraftChild;
    }

    private void QSCustomsRowApply(FrameworkElement container, object item, string? _)
    {
        if (item is QSCustomsItem row)
        {
            QSCustomsRowApply(container, row, _csCustoms.CSCustomsRowRead(row.QSCustomsItemIndex));
        }
    }

    private void QSCustomsRowApply(FrameworkElement container, QSCustomsItem row, CSCustomsRow state)
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
            loss.Text = QSCustomsLossFormat(
                _qsCustomsHost.PWindowDeportment.LWindowWorkspace.QWorkspaceEntryLoad(state.CSCustomsRowLoss));
        }
    }

    private void QSCustomsModeApply(FrameworkElement container, ComboBox mode, CSCustomsMode chosen)
    {
        mode.SelectionChanged -= QSCustomsModeHandle;
        foreach (KeyValuePair<string, CSCustomsMode> pair in QSCustomsChoice)
        {
            if (QLook.QLookPartFind<ComboBoxItem>(container, pair.Key) is ComboBoxItem choice && pair.Value == chosen)
            {
                mode.SelectedItem = choice;
            }
        }

        mode.SelectionChanged += QSCustomsModeHandle;
    }

    private void QSCustomsTargetApply(ComboBox target, QSCustomsItem row, CSCustomsRow state)
    {
        target.SelectionChanged -= QSCustomsTargetHandle;
        target.IsEnabled = state.CSCustomsRowTargeted;
        if (!ReferenceEquals(target.ItemsSource, row.QSCustomsItemCandidate))
        {
            target.ItemsSource = row.QSCustomsItemCandidate;
            QLookItem.QLookItemAttach(
                target, (container, item, _) => QSCustomsCandidateApply(container, item, row.QSCustomsItemHeadword));
        }

        target.SelectedItem = state.CSCustomsRowTarget;
        target.SelectionChanged += QSCustomsTargetHandle;
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

    private void QSCustomsModeHandle(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox { DataContext: QSCustomsItem item, SelectedItem: ComboBoxItem choice })
        {
            return;
        }

        QSCustomsAdmit.IsEnabled = _csCustoms.CSCustomsModeSet(item.QSCustomsItemIndex, QSCustomsChoice[choice.Name]);
        QLookItem.QLookItemApply(QSCustomsList);
    }

    private void QSCustomsTargetHandle(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox { DataContext: QSCustomsItem item, SelectedItem: long target })
        {
            return;
        }

        QSCustomsAdmit.IsEnabled = _csCustoms.CSCustomsTargetSet(item.QSCustomsItemIndex, target);
        QLookItem.QLookItemApply(QSCustomsList);
    }

    private void QSCustomsAcceptHandle(object sender, RoutedEventArgs e)
    {
        _qsCustomsSurface.DialogResult = true;
    }

    private void QSCustomsCancelHandle(object sender, RoutedEventArgs e)
    {
        _qsCustomsSurface.DialogResult = false;
    }

    private void QSCustomsCloseHandle(object sender, RoutedEventArgs e)
    {
        _qsCustomsSurface.DialogResult = true;
    }

    private string QSCustomsLossFormat(CEntryDraft? stored)
    {
        if (stored is null)
        {
            return string.Empty;
        }

        return string.Format(
            CultureInfo.CurrentCulture,
            _qsCustomsSurface.TryFindResource("Customs.Loss") as string ?? "Customs.Loss",
            CSCustoms.CSCustomsCardScan(stored.CEntryDraftMeanings, QSCustomsChildRead),
            CSCustoms.CSCustomsCardScan(stored.CEntryDraftCollocations, QSCustomsChildRead));
    }
}
