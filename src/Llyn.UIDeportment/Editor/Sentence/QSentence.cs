using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QSentence
{
    private readonly ObservableCollection<QPlacard> _qSentenceMeaning;

    private readonly ObservableCollection<QPlacard> _qSentenceCollocation;

    private CMention _cMention = null!;

    private QMentionMenu _qMentionMenu = null!;

    private CCard _cCard = null!;

    private CSentence _cSentence = null!;

    private QProspect _qSentenceProspect = null!;

    internal QSentence(ObservableCollection<QPlacard> meaning, ObservableCollection<QPlacard> collocation)
    {
        _qSentenceMeaning = meaning;
        _qSentenceCollocation = collocation;
    }

    internal ObservableCollection<string> QSentenceParticle { get; } = [];

    internal ObservableCollection<string> QSentenceDependence { get; } = [];

    internal CSentenceOrder? QSentenceOrder { get; private set; }

    internal void QSentenceIntroduce(
        CCard card, CSentence sentence, CMention mention, QMentionMenu mentionMenu, QProspect prospect)
    {
        _cCard = card;
        _cSentence = sentence;
        _cMention = mention;
        _qMentionMenu = mentionMenu;
        _qSentenceProspect = prospect;
    }

    internal QPlacard? QSentenceCardFind(QPassage row)
    {
        foreach (QPlacard card in _qSentenceMeaning)
        {
            if (card.QCardSentence.QCardSentenceRow.Contains(row))
            {
                return card;
            }
        }

        foreach (QPlacard card in _qSentenceCollocation)
        {
            if (card.QCardSentence.QCardSentenceRow.Contains(row))
            {
                return card;
            }
        }

        return null;
    }

    internal void QSentenceFrameRefine(CEntryDraft _)
    {
        CSentenceFrame frame = _cSentence.CSentenceFrameRead();
        QSentenceOrder = frame.CSentenceFrameOrder;
        QSentenceListRefine(QSentenceParticle, frame.CSentenceFrameParticle);
        QSentenceListRefine(QSentenceDependence, frame.CSentenceFrameDependence);

        foreach (QPlacard card in _qSentenceMeaning)
        {
            card.QCardSentence.QCardSentenceApply(frame.CSentenceFrameOrder);
        }

        foreach (QPlacard card in _qSentenceCollocation)
        {
            card.QCardSentence.QCardSentenceApply(frame.CSentenceFrameOrder);
        }
    }

    internal void QSentenceMentionRefine(CEntryDraft _)
    {
        IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> lines =
            _cSentence.CSentenceMentionRead();
        QSentenceChipRefine(_qSentenceMeaning, lines);
        QSentenceChipRefine(_qSentenceCollocation, lines);
    }

    internal void QSentenceGlossObserve(QPassage row, QRendition gloss, string language)
    {
        if (QSentenceCardFind(row) is QPlacard card)
        {
            _cSentence.CSentenceLanguageSet(card.QPlacardId, row.QPassageRow, gloss.QRenditionId, language);
        }
    }

    internal void QSentenceTextObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: QPassage row } box
            && QSentenceCardFind(row) is QPlacard card)
        {
            _cSentence.CSentenceTextSet(card.QPlacardId, row.QPassageRow, box.Text);
        }
    }

    internal void QSentenceParticleObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is ComboBox { IsKeyboardFocusWithin: true, DataContext: QPassage row }
            && e.OriginalSource is TextBox box
            && QSentenceCardFind(row) is QPlacard card)
        {
            _cSentence.CSentenceParticleSet(card.QPlacardId, row.QPassageRow, box.Text);
        }
    }

    internal void QSentenceDependenceObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is ComboBox { IsKeyboardFocusWithin: true, DataContext: QPassage row }
            && e.OriginalSource is TextBox box
            && QSentenceCardFind(row) is QPlacard card)
        {
            _cSentence.CSentenceDependenceSet(card.QPlacardId, row.QPassageRow, box.Text);
        }
    }

    private static void QSentenceListRefine(ObservableCollection<string> catalog, IReadOnlyList<string> values)
    {
        catalog.Clear();
        foreach (string value in values)
        {
            catalog.Add(value);
        }
    }

    private static void QSentenceChipRefine(
        IReadOnlyList<QPlacard> cards, IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> lines)
    {
        foreach (QPlacard card in cards)
        {
            foreach (QPassage row in card.QCardSentence.QCardSentenceRow)
            {
                row.QPassageChip.QMentionLineRefine(QMentionChip.QMentionChipCreate(
                    lines.TryGetValue(row.QPassageRow, out IReadOnlyList<CMentionLabel>? labels) ? labels : []));
            }
        }
    }

    internal void QSentenceAddObserve(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: QPassage row }
            || QSentenceCardFind(row) is not QPlacard card)
        {
            return;
        }

        _cSentence.CSentenceAdd(card.QPlacardId, card.QCardSentence.QCardSentenceFind(row));
    }

    internal void QSentenceRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: QPassage row }
            || QSentenceCardFind(row) is not QPlacard card)
        {
            return;
        }

        _cSentence.CSentenceRemove(card.QPlacardId, row.QPassageRow);
    }

    internal void QSentenceLinkRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: QPassage } box)
        {
            return;
        }

        _qSentenceProspect.QProspectPlaceRefine(box, QMentionSelection.QMentionSelectionPlace(box));
        _qSentenceProspect.QProspectOpenRefine(_cCard.CCardMentionRead(box.SelectedText));
    }

    internal void QSentenceMeaningRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: QPassage row } box
            || QSentenceCardFind(row) is not QPlacard card)
        {
            return;
        }

        if (_cSentence.CSentenceSenseRead(
                card.QPlacardId, row.QPassageRow, box.Text, box.SelectionStart, box.SelectionLength)
            is CMentionSense sense)
        {
            QMentionAsk ask = _qMentionMenu.QMentionMeaningRefine(
                box, QMentionSelection.QMentionSelectionPlace(box), sense);
            ask.QMentionAskChosen += QSentenceSenseObserve;
        }
    }

    private void QSentenceSenseObserve(FrameworkElement anchor, long sense)
    {
        if (anchor is not TextBox { DataContext: QPassage row } box
            || QSentenceCardFind(row) is not QPlacard card)
        {
            return;
        }

        _cSentence.CSentenceSenseSet(
            card.QPlacardId, row.QPassageRow, box.Text, box.SelectionStart, box.SelectionLength, sense);
    }

    internal void QSentenceSilenceObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: QPassage row } box
            || QSentenceCardFind(row) is not QPlacard card)
        {
            return;
        }

        _cSentence.CSentenceSilenceSet(
            card.QPlacardId, row.QPassageRow, box.Text, box.SelectionStart, box.SelectionLength);
    }

    internal void QSentenceUnlinkObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: QPassage row }
            || QSentenceCardFind(row) is not QPlacard card)
        {
            return;
        }

        if (e.Parameter is QMentionChip chip)
        {
            _cSentence.CSentenceMentionRemove(card.QPlacardId, row.QPassageRow, chip.QMentionChipId);
        }
        else if (e.Source is TextBox box)
        {
            _cSentence.CSentenceMentionRemove(
                card.QPlacardId, row.QPassageRow, box.Text, box.SelectionStart, box.SelectionLength);
        }
    }

    internal void QSentenceSpanRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox box
            && _cMention.CMentionSpanCheck(
                box.Text, box.SelectionStart, box.SelectionLength);
    }

    internal void QSentenceSenseRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox { DataContext: QPassage row } box
            && QSentenceCardFind(row) is QPlacard card
            && _cSentence.CSentenceSenseCheck(
                card.QPlacardId, row.QPassageRow, box.Text, box.SelectionStart, box.SelectionLength);
    }

    internal void QSentenceUnlinkRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Parameter is QMentionChip
            || (e.Source is TextBox { DataContext: QPassage row } box
                && QSentenceCardFind(row) is QPlacard card
                && _cSentence.CSentenceMentionCheck(
                    card.QPlacardId, row.QPassageRow, box.Text, box.SelectionStart, box.SelectionLength));
    }
}
