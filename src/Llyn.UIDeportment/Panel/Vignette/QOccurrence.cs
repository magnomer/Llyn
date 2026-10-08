using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QOccurrence
{
    private readonly UserControl _qOccurrenceScope;

    private readonly ObservableCollection<QOccurrenceItem> _qOccurrenceList = [];

    private CRepertoire _cRepertoire = null!;

    internal QOccurrence(UserControl scope)
    {
        ArgumentNullException.ThrowIfNull(scope);

        _qOccurrenceScope = scope;

        QSortie.SetResourceReference(QField.QFieldHintProperty, "Sortie.Search");
        QSortie.TextChanged += QSortieObserve;
    }

    private TextBox QSortie => QContract.QContractFind<TextBox>(_qOccurrenceScope, "PSortie");

    private ItemsControl QOccurrenceView => QContract.QContractFind<ItemsControl>(_qOccurrenceScope, "POccurrence");

    private TextBlock QOccurrenceEmpty =>
        QContract.QContractFind<TextBlock>(_qOccurrenceScope, "POccurrenceEmpty");

    internal void QOccurrenceIntroduce(CRepertoire repertoire, CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(repertoire);
        ArgumentNullException.ThrowIfNull(atelier);

        _cRepertoire = repertoire;
        _cRepertoire.CRepertoireOccurrence.COccurrencePanel.CPanelAperture.CApertureRowsChanged += QOccurrenceRefine;
        atelier.CAtelierWorkspace.CWorkspaceOpened += QOccurrenceVistaRefine;

        QOccurrenceView.ItemsSource = _qOccurrenceList;
        QLookItem.QLookItemAttach(QOccurrenceView, QOccurrenceItemRefine);
    }

    private async void QOccurrenceVistaRefine()
    {
        CEnsignSheet<IReadOnlyList<CVistaRow>> sheet =
            await _cRepertoire.CRepertoireOccurrence.COccurrenceRowsLoad(QEnsignImage.QEnsignDraw);
        QOccurrenceRefine(sheet.CEnsignSheetRows);
    }

    private void QSortieObserve(object sender, TextChangedEventArgs e)
    {
        _cRepertoire.CRepertoireOccurrence.COccurrencePanel.CPanelAperture.CApertureQuerySet(QSortie.Text);
    }

    private void QOccurrenceRefine()
    {
        QOccurrenceRefine(_cRepertoire.CRepertoireOccurrence.COccurrenceRowsRead());
    }

    private void QOccurrenceRefine(IReadOnlyList<CVistaRow> rows)
    {
        List<QOccurrenceItem> fresh = [];
        foreach (CVistaRow entry in rows)
        {
            fresh.Add(new QOccurrenceItem(entry, entry.CVistaRowChosen));
        }

        QSplice.QSpliceRefine(
            _qOccurrenceList, fresh, QOccurrenceItem.QOccurrenceItemMatch, QOccurrenceItem.QOccurrenceItemSync);

        QOccurrenceEmpty.SetResourceReference(
            TextBlock.TextProperty, _cRepertoire.CRepertoireOccurrence.COccurrencePanel.CPanelAperture.CApertureKey);
        QOccurrenceEmpty.Visibility = QLook.QLookVisibleRead(_qOccurrenceList.Count == 0);
    }

    private void QOccurrenceObserve(object sender, RoutedEventArgs e)
    {
        _cRepertoire.CRepertoireDiptych.CDiptychChildSelect(
            QSender.QSenderSourceRead<QOccurrenceItem>(e)?.QOccurrenceItemId);
    }

    private void QOccurrenceItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QOccurrenceItem occurrence)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "POccurrenceRow") is Button row)
        {
            if (occurrence.QOccurrenceItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QOccurrenceObserve;
            row.Click += QOccurrenceObserve;
        }

        if (QLook.QLookPartFind<Image>(container, "POccurrenceFlag") is Image flag)
        {
            flag.Source = occurrence.QOccurrenceItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "POccurrenceName") is Run name)
        {
            name.Text = occurrence.QOccurrenceItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "POccurrenceEpithet") is Run epithet)
        {
            epithet.Text = QLook.QLookEpithetRead(occurrence.QOccurrenceItemEpithet);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "POccurrenceLanguage") is TextBlock language)
        {
            language.Text = occurrence.QOccurrenceItemLanguage;
        }
    }
}
