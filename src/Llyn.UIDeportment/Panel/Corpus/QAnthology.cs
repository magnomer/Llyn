using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QAnthology
{
    private readonly UserControl _qAnthologyScope;

    private readonly ObservableCollection<QAnthologyItem> _qAnthologyList = [];

    private CCorpus _cCorpus = null!;

    internal QAnthology(UserControl scope)
    {
        ArgumentNullException.ThrowIfNull(scope);

        _qAnthologyScope = scope;

        QQuery.SetResourceReference(QField.QFieldHintProperty, "Example.Search");
        QQuery.TextChanged += QQueryObserve;
    }

    private TextBox QQuery => QContract.QContractFind<TextBox>(_qAnthologyScope, "PQuery");

    private ItemsControl QAnthologyView => QContract.QContractFind<ItemsControl>(_qAnthologyScope, "PAnthology");

    private TextBlock QAnthologyEmpty => QContract.QContractFind<TextBlock>(_qAnthologyScope, "PAnthologyEmpty");

    internal void QAnthologyIntroduce(CCorpus corpus)
    {
        ArgumentNullException.ThrowIfNull(corpus);

        _cCorpus = corpus;
        _cCorpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureRowsChanged += QAnthologyRefine;
        _cCorpus.CCorpusQueryCleared += QQueryClear;

        QAnthologyView.ItemsSource = _qAnthologyList;
        QLookItem.QLookItemAttach(QAnthologyView, QAnthologyApply);
    }

    private void QQueryObserve(object sender, TextChangedEventArgs e)
    {
        _cCorpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureQuerySet(QQuery.Text ?? string.Empty);
    }

    private void QQueryClear()
    {
        QQuery.Clear();
    }

    private void QAnthologyRefine()
    {
        QAnthologyRefine(_cCorpus.CCorpusRowsRead());
    }

    internal void QAnthologyRefine(IReadOnlyList<CCatalogExample> rows)
    {
        List<QAnthologyItem> fresh = [];
        foreach (CCatalogExample row in rows)
        {
            fresh.Add(new QAnthologyItem(row, row.CCatalogExampleChosen));
        }

        QSplice.QSpliceRefine(
            _qAnthologyList, fresh, QAnthologyItem.QAnthologyItemMatch, QAnthologyItem.QAnthologyItemSync);

        QAnthologyEmpty.Visibility = QLook.QLookVisibleRead(_qAnthologyList.Count == 0);
    }

    private void QAnthologyObserve(object sender, RoutedEventArgs e)
    {
        _cCorpus.CCorpusDiptych.CDiptychParentSelect(
            QSender.QSenderSourceRead<QAnthologyItem>(e)?.QAnthologyItemId);
    }

    private void QAnthologyApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QAnthologyItem anthology)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PAnthologyRow") is Button row)
        {
            if (anthology.QAnthologyItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QAnthologyObserve;
            row.Click += QAnthologyObserve;
        }

        if (QLook.QLookPartFind<Image>(container, "PAnthologyFlag") is Image flag)
        {
            flag.Source = anthology.QAnthologyItemFlag;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAnthologyName") is TextBlock name)
        {
            name.Text = anthology.QAnthologyItemName;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAnthologyLanguage") is TextBlock language)
        {
            language.Text = anthology.QAnthologyItemLanguage;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAnthologyCount") is TextBlock count)
        {
            count.Text = anthology.QAnthologyItemCount;
        }
    }
}
