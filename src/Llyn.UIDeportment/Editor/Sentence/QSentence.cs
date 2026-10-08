using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QSentence
{
    private readonly ObservableCollection<PCard> _qSentenceMeaning;

    private readonly ObservableCollection<PCard> _qSentenceCollocation;

    private CMention _cMention = null!;

    private QMentionMenu _qMentionMenu = null!;

    private CCard _cCard = null!;

    private CSentence _cSentence = null!;

    private QProspect _qSentenceProspect = null!;

    internal QSentence(ObservableCollection<PCard> meaning, ObservableCollection<PCard> collocation)
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

    internal PCard? QSentenceCardFind(PSentence row)
    {
        foreach (PCard card in _qSentenceMeaning)
        {
            if (card.PCardSentence.PCardSentenceRow.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _qSentenceCollocation)
        {
            if (card.PCardSentence.PCardSentenceRow.Contains(row))
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

        foreach (PCard card in _qSentenceMeaning)
        {
            card.PCardSentence.PCardSentenceApply(frame.CSentenceFrameOrder);
        }

        foreach (PCard card in _qSentenceCollocation)
        {
            card.PCardSentence.PCardSentenceApply(frame.CSentenceFrameOrder);
        }
    }

    internal void QSentenceMentionRefine(CEntryDraft _)
    {
        IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> lines =
            _cSentence.CSentenceMentionRead();
        QSentenceChipRefine(_qSentenceMeaning, lines);
        QSentenceChipRefine(_qSentenceCollocation, lines);
    }

    internal void QSentenceGlossObserve(PSentence row, PGloss gloss, string language)
    {
        if (QSentenceCardFind(row) is PCard card)
        {
            _cSentence.CSentenceLanguageSet(card.PCardId, row.PSentenceRow, gloss.PGlossId, language);
        }
    }

    internal void QSentenceTextObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: PSentence row } box
            && QSentenceCardFind(row) is PCard card)
        {
            _cSentence.CSentenceTextSet(card.PCardId, row.PSentenceRow, box.Text);
        }
    }

    internal void QSentenceParticleObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is ComboBox { IsKeyboardFocusWithin: true, DataContext: PSentence row }
            && e.OriginalSource is TextBox box
            && QSentenceCardFind(row) is PCard card)
        {
            _cSentence.CSentenceParticleSet(card.PCardId, row.PSentenceRow, box.Text);
        }
    }

    internal void QSentenceDependenceObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is ComboBox { IsKeyboardFocusWithin: true, DataContext: PSentence row }
            && e.OriginalSource is TextBox box
            && QSentenceCardFind(row) is PCard card)
        {
            _cSentence.CSentenceDependenceSet(card.PCardId, row.PSentenceRow, box.Text);
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
        IReadOnlyList<PCard> cards, IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> lines)
    {
        foreach (PCard card in cards)
        {
            foreach (PSentence row in card.PCardSentence.PCardSentenceRow)
            {
                row.PSentenceChip.PMentionLineRefine(QMentionChip.QMentionChipCreate(
                    lines.TryGetValue(row.PSentenceRow, out IReadOnlyList<CMentionLabel>? labels) ? labels : []));
            }
        }
    }

    internal void QSentenceAddObserve(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSentence row }
            || QSentenceCardFind(row) is not PCard card)
        {
            return;
        }

        _cSentence.CSentenceAdd(card.PCardId, card.PCardSentence.PCardSentenceFind(row));
    }

    internal void QSentenceRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSentence row }
            || QSentenceCardFind(row) is not PCard card)
        {
            return;
        }

        _cSentence.CSentenceRemove(card.PCardId, row.PSentenceRow);
    }

    internal void QSentenceLinkRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence } box)
        {
            return;
        }

        _qSentenceProspect.QProspectPlaceRefine(box, PMentionSelection.PMentionSelectionPlace(box));
        _qSentenceProspect.QProspectOpenRefine(_cCard.CCardMentionRead(box.SelectedText));
    }

    internal void QSentenceMeaningRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence row } box
            || QSentenceCardFind(row) is not PCard card)
        {
            return;
        }

        if (_cSentence.CSentenceSenseRead(
                card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength)
            is CMentionSense sense)
        {
            QMentionAsk ask = _qMentionMenu.QMentionMeaningRefine(
                box, PMentionSelection.PMentionSelectionPlace(box), sense);
            ask.QMentionAskChosen += QSentenceSenseObserve;
        }
    }

    private void QSentenceSenseObserve(FrameworkElement anchor, long sense)
    {
        if (anchor is not TextBox { DataContext: PSentence row } box
            || QSentenceCardFind(row) is not PCard card)
        {
            return;
        }

        _cSentence.CSentenceSenseSet(
            card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength, sense);
    }

    internal void QSentenceSilenceObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence row } box
            || QSentenceCardFind(row) is not PCard card)
        {
            return;
        }

        _cSentence.CSentenceSilenceSet(
            card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength);
    }

    internal void QSentenceUnlinkObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSentence row }
            || QSentenceCardFind(row) is not PCard card)
        {
            return;
        }

        if (e.Parameter is PMentionChip chip)
        {
            _cSentence.CSentenceMentionRemove(card.PCardId, row.PSentenceRow, chip.PMentionChipId);
        }
        else if (e.Source is TextBox box)
        {
            _cSentence.CSentenceMentionRemove(
                card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength);
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
        e.CanExecute = e.Source is TextBox { DataContext: PSentence row } box
            && QSentenceCardFind(row) is PCard card
            && _cSentence.CSentenceSenseCheck(
                card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength);
    }

    internal void QSentenceUnlinkRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Parameter is PMentionChip
            || (e.Source is TextBox { DataContext: PSentence row } box
                && QSentenceCardFind(row) is PCard card
                && _cSentence.CSentenceMentionCheck(
                    card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength));
    }
}
