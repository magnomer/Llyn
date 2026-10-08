using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QAtlas
{
    private readonly UserControl _qAtlasScope;

    private readonly ObservableCollection<QAtlasItem> _qAtlasList = [];

    private CRepertoire _cRepertoire = null!;

    internal QAtlas(UserControl scope)
    {
        ArgumentNullException.ThrowIfNull(scope);

        _qAtlasScope = scope;

        QInquest.SetResourceReference(QField.QFieldHintProperty, "Situation.Search");
        QInquest.TextChanged += QInquestObserve;
    }

    private TextBox QInquest => QContract.QContractFind<TextBox>(_qAtlasScope, "PInquest");

    private ItemsControl QAtlasView => QContract.QContractFind<ItemsControl>(_qAtlasScope, "PAtlas");

    private TextBlock QAtlasEmpty => QContract.QContractFind<TextBlock>(_qAtlasScope, "PAtlasEmpty");

    internal void QAtlasIntroduce(CRepertoire repertoire)
    {
        ArgumentNullException.ThrowIfNull(repertoire);

        _cRepertoire = repertoire;
        _cRepertoire.CRepertoireAtlas.CAtlasPanel.CPanelAperture.CApertureRowsChanged += QAtlasRefine;
        _cRepertoire.CRepertoireQueryCleared += QInquestRefine;

        QAtlasView.ItemsSource = _qAtlasList;
        QLookItem.QLookItemAttach(QAtlasView, QAtlasItemRefine);
    }

    private void QInquestObserve(object sender, TextChangedEventArgs e)
    {
        _cRepertoire.CRepertoireAtlas.CAtlasPanel.CPanelAperture.CApertureQuerySet(QInquest.Text ?? string.Empty);
    }

    private void QInquestRefine()
    {
        QInquest.Clear();
    }

    private void QAtlasRefine()
    {
        QAtlasRefine(_cRepertoire.CRepertoireRowsRead());
    }

    internal void QAtlasRefine(IReadOnlyList<CCatalogSituation> rows)
    {
        List<QAtlasItem> fresh = [];
        foreach (CCatalogSituation row in rows)
        {
            fresh.Add(new QAtlasItem(row, row.CCatalogSituationChosen));
        }

        QSplice.QSpliceRefine(_qAtlasList, fresh, QAtlasItem.QAtlasItemMatch, QAtlasItem.QAtlasItemSync);

        QAtlasEmpty.Visibility = QLook.QLookVisibleRead(_qAtlasList.Count == 0);
    }

    private void QAtlasObserve(object sender, RoutedEventArgs e)
    {
        _cRepertoire.CRepertoireDiptych.CDiptychParentSelect(
            QSender.QSenderSourceRead<QAtlasItem>(e)?.QAtlasItemId);
    }

    private void QAtlasItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QAtlasItem atlas)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PAtlasRow") is Button row)
        {
            if (atlas.QAtlasItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QAtlasObserve;
            row.Click += QAtlasObserve;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PAtlasIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("situation", 16);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAtlasTitle") is TextBlock title)
        {
            title.Text = atlas.QAtlasItemTitle;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAtlasKind") is TextBlock kind)
        {
            kind.Text = atlas.QAtlasItemKind;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAtlasTally") is TextBlock tally)
        {
            tally.Text = atlas.QAtlasItemCount;
        }
    }
}
