using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QProspect
{
    private readonly FrameworkElement _qProspectSurface;

    private readonly ObservableCollection<PProspectItem> _qProspectItem = [];

    private readonly QSentence _qProspectSentence;

    private CEditor _cEditor = null!;

    private QLink _qProspectLink = null!;

    internal QProspect(FrameworkElement surface, QSentence sentence)
    {
        _qProspectSurface = surface;
        _qProspectSentence = sentence;
        QProspectList.ItemsSource = _qProspectItem;
        QLookItem.QLookItemAttach(QProspectList, QProspectApply);
        QProspectPopup.Closed += QProspectCloseRefine;
    }

    private Popup QProspectPopup => QContract.QContractFind<Popup>(_qProspectSurface, "PProspect");

    private ListBox QProspectList => QContract.QContractFind<ListBox>(_qProspectSurface, "PProspectList");

    private Border QProspectContents => QContract.QContractFind<Border>(_qProspectSurface, "PContents");

    internal void QProspectIntroduce(CEditor editor, QLink link)
    {
        _cEditor = editor;
        _qProspectLink = link;
    }

    private void QProspectApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PProspectItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PProspectMark") is TextBlock mark)
        {
            mark.Visibility = QLook.QLookVisibleRead(row.PProspectItemFresh);
        }

        if (QLook.QLookPartFind<Run>(container, "PProspectName") is Run name)
        {
            name.Text = row.PProspectItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PProspectEpithet") is Run epithet)
        {
            epithet.Text = QLook.QLookEpithetRead(row.PProspectItemEpithet);
        }

        if (QLook.QLookPartFind<Image>(container, "PProspectFlag") is Image flag)
        {
            flag.Source = row.PProspectItemFlag;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PProspectLanguage") is TextBlock language)
        {
            language.Text = row.PProspectItemLanguage;
        }

        if (QLook.QLookPartFind<Grid>(container, "PProspectRow") is Grid surface)
        {
            surface.PreviewMouseLeftButtonDown -= QProspectMissRefine;
            surface.PreviewMouseLeftButtonDown -= QProspectPickObserve;
            surface.PreviewMouseLeftButtonDown += QProspectMissRefine;
            surface.PreviewMouseLeftButtonDown += QProspectPickObserve;
        }
    }

    private void QProspectMissRefine(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PProspectItem })
        {
            QProspectShutRefine();
        }
    }

    private void QProspectPickObserve(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PProspectItem item })
        {
            return;
        }

        e.Handled = true;
        switch (QProspectPopup.PlacementTarget)
        {
            case TextBox { DataContext: PLinkCaret caret }:
                if (_qProspectLink.QLinkCardFind(caret) is PCard owner)
                {
                    _cEditor.CEditorCard.CCardTranslationInsert(
                        owner.PCardId,
                        item.PProspectItemId,
                        item.PProspectItemHeadword,
                        item.PProspectItemLanguage,
                        owner.PCardLinkPosition);
                    QProspectShutRefine();
                    owner.PCardLinkClear();
                }

                return;
            case TextBox { DataContext: PEtymon caret }:
                _cEditor.CEditorCard.CCardEtymonAdd(item.PProspectItemId);
                QEtymologyEditor.QEtymologyCaretRefine(caret);
                break;
            case TextBox { DataContext: PSentence row } box:
                if (_qProspectSentence.QSentenceCardFind(row) is PCard card)
                {
                    _cEditor.CEditorSentence.CSentenceMentionAdd(
                        card.PCardId,
                        row.PSentenceRow,
                        box.Text,
                        box.SelectionStart,
                        box.SelectionLength,
                        item.PProspectItemId);
                }

                break;
            case TextBox box:
                PMentionCommand.PMentionCommandPick.Execute(item.PProspectItemId, box);
                break;
        }

        QProspectShutRefine();
    }

    internal void QProspectKeyRefine(object sender, KeyEventArgs e)
    {
        int? lit = e.Key switch
        {
            Key.Down => CLantern.CLanternMove(QProspectList.SelectedIndex, QProspectList.Items.Count, 1),
            Key.Up => CLantern.CLanternMove(QProspectList.SelectedIndex, QProspectList.Items.Count, -1),
            _ => null,
        };
        if (lit is int chosen)
        {
            QProspectList.SelectedIndex = chosen;
            QProspectList.ScrollIntoView(QProspectList.SelectedItem);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape && QProspectPopup.IsOpen)
        {
            QProspectShutRefine();
            e.Handled = true;
        }
    }

    private void QProspectCloseRefine(object? sender, EventArgs e)
    {
        QProspectList.SelectedIndex = -1;
        _qProspectItem.Clear();
    }

    internal void QProspectKeyObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not FrameworkElement { DataContext: PLinkCaret row }
            || _qProspectLink.QLinkCardFind(row) is not PCard card)
        {
            return;
        }

        if (QProspectList.SelectedItem is not PProspectItem item)
        {
            return;
        }

        _cEditor.CEditorCard.CCardTranslationInsert(
            card.PCardId,
            item.PProspectItemId,
            item.PProspectItemHeadword,
            item.PProspectItemLanguage,
            card.PCardLinkPosition);
        e.Handled = true;
        QProspectShutRefine();
        card.PCardLinkClear();
    }

    internal void QProspectPlaceRefine(FrameworkElement anchor, Rect place)
    {
        ArgumentNullException.ThrowIfNull(anchor);

        QProspectShutRefine();
        QProspectPopup.PlacementTarget = anchor;
        QProspectPopup.HorizontalOffset = place.X;
        QProspectPopup.VerticalOffset = place.Bottom - anchor.ActualHeight;
    }

    internal void QProspectOpenRefine(CProspect prospect)
    {
        ArgumentNullException.ThrowIfNull(prospect);

        _qProspectItem.Clear();
        if (!prospect.CProspectShown)
        {
            return;
        }

        foreach (CVistaRow entry in prospect.CProspectRows)
        {
            _qProspectItem.Add(new PProspectItem(
                entry.CVistaRowId,
                entry.CVistaRowHeadword,
                entry.CVistaRowLanguage,
                entry.CVistaRowEpithet, entry.CVistaRowName));
        }

        foreach (string language in prospect.CProspectLanguages)
        {
            _qProspectItem.Add(new PProspectItem(null, prospect.CProspectWord, language));
        }

        QProspectPopup.IsOpen = true;
        QProspectList.SelectedIndex = prospect.CProspectChosen ? 0 : -1;
    }

    internal void QProspectTranslationRefine(PCard card, CProspect prospect)
    {
        QProspectPopup.HorizontalOffset = 0;
        QProspectPopup.VerticalOffset = 0;
        QProspectPopup.PlacementTarget = _qProspectLink.QLinkBoxFind(card) ?? (UIElement)QProspectContents;
        QProspectOpenRefine(prospect);
    }

    internal void QProspectShutRefine()
    {
        QProspectPopup.IsOpen = false;
        QProspectPopup.HorizontalOffset = 0;
        QProspectPopup.VerticalOffset = 0;
        QProspectList.SelectedIndex = -1;
        _qProspectItem.Clear();
    }
}
