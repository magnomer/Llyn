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

    private readonly ObservableCollection<QProspectItem> _qProspectItem = [];

    private readonly QSentence _qProspectSentence;

    private CCaret _cCaret = null!;

    private CSentence _cSentence = null!;

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

    internal void QProspectIntroduce(CCaret caret, CSentence sentence, QLink link)
    {
        _cCaret = caret;
        _cSentence = sentence;
        _qProspectLink = link;
    }

    private void QProspectApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QProspectItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PProspectMark") is TextBlock mark)
        {
            mark.Visibility = QLook.QLookVisibleRead(row.QProspectItemFresh);
        }

        if (QLook.QLookPartFind<Run>(container, "PProspectName") is Run name)
        {
            name.Text = row.QProspectItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PProspectEpithet") is Run epithet)
        {
            epithet.Text = QLook.QLookEpithetRead(row.QProspectItemEpithet);
        }

        if (QLook.QLookPartFind<Image>(container, "PProspectFlag") is Image flag)
        {
            flag.Source = row.QProspectItemFlag;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PProspectLanguage") is TextBlock language)
        {
            language.Text = row.QProspectItemLanguage;
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
        if (sender is not FrameworkElement { DataContext: QProspectItem })
        {
            QProspectShutRefine();
        }
    }

    private void QProspectPickObserve(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: QProspectItem item })
        {
            return;
        }

        e.Handled = true;
        switch (QProspectPopup.PlacementTarget)
        {
            case TextBox { DataContext: QInlet<QLinkChip> caret }:
                if (_qProspectLink.QLinkCardFind(caret) is QPlacard owner)
                {
                    _cCaret.CCaretTranslationInsert(
                        owner.QPlacardCollocated,
                        owner.QPlacardId,
                        item.QProspectItemId,
                        item.QProspectItemHeadword,
                        item.QProspectItemLanguage);
                    QProspectShutRefine();
                    caret.QInletClear();
                }

                return;
            case TextBox { DataContext: QPassage row } box:
                if (_qProspectSentence.QSentenceCardFind(row) is QPlacard card)
                {
                    _cSentence.CSentenceMentionAdd(
                        card.QPlacardCollocated,
                        card.QPlacardId,
                        row.QPassageRow,
                        box.Text,
                        box.SelectionStart,
                        box.SelectionLength,
                        item.QProspectItemId);
                }

                break;
            case TextBox box:
                QMentionCommand.QMentionCommandPick.Execute(item.QProspectItemId, box);
                break;
        }

        QProspectShutRefine();
    }

    internal void QProspectKeyRefine(object sender, KeyEventArgs e)
    {
        int way = e.Key switch
        {
            Key.Down => 1,
            Key.Up => -1,
            _ => 0,
        };
        bool translation = QProspectTranslationCheck();
        int? lit = way == 0 || _qProspectItem.Count == 0 ? null
            : translation ? _cCaret.CCaretLanternMove(way)
            : CLantern.CLanternMove(QProspectList.SelectedIndex, QProspectList.Items.Count, way);
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

    private bool QProspectTranslationCheck()
    {
        return QProspectPopup.PlacementTarget is TextBox { DataContext: QInlet<QLinkChip> };
    }

    private void QProspectCloseRefine(object? sender, EventArgs e)
    {
        QProspectList.SelectedIndex = -1;
        _qProspectItem.Clear();
    }

    internal void QProspectKeyObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        if (QProspectList.SelectedItem is not QProspectItem item)
        {
            return;
        }

        switch (sender)
        {
            case FrameworkElement { DataContext: QInlet<QLinkChip> row }:
                if (_qProspectLink.QLinkCardFind(row) is QPlacard card)
                {
                    _cCaret.CCaretTranslationInsert(
                        card.QPlacardCollocated,
                        card.QPlacardId,
                        item.QProspectItemId,
                        item.QProspectItemHeadword,
                        item.QProspectItemLanguage);
                    e.Handled = true;
                    QProspectShutRefine();
                    row.QInletClear();
                }

                return;
            case TextBox box:
                QMentionCommand.QMentionCommandPick.Execute(item.QProspectItemId, box);
                e.Handled = true;
                QProspectShutRefine();
                return;
        }
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
            _qProspectItem.Add(new QProspectItem(
                entry.CVistaRowId,
                entry.CVistaRowHeadword,
                entry.CVistaRowLanguage,
                entry.CVistaRowEpithet, entry.CVistaRowName));
        }

        foreach (string language in prospect.CProspectLanguages)
        {
            _qProspectItem.Add(new QProspectItem(null, prospect.CProspectWord, language));
        }

        QProspectPopup.IsOpen = true;
        QProspectList.SelectedIndex = prospect.CProspectChosen ? 0 : -1;
    }

    internal void QProspectTranslationRefine(QPlacard card, CProspect prospect)
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
