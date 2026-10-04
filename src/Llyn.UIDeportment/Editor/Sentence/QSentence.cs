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

    private QWindow _pSentenceHost = null!;

    private CEditor _cEditor = null!;

    private QProspect _qSentenceProspect = null!;

    private QCitation _qSentenceCitation = null!;

    internal QSentence(ObservableCollection<PCard> meaning, ObservableCollection<PCard> collocation)
    {
        _qSentenceMeaning = meaning;
        _qSentenceCollocation = collocation;
    }

    internal ObservableCollection<string> QSentenceParticle { get; } = [];

    internal ObservableCollection<string> QSentenceDependence { get; } = [];

    internal CSentenceOrder? QSentenceOrder { get; private set; }

    internal QExample QSentenceExample { get; private set; } = null!;

    internal void QSentenceIntroduce(
        CEditor editor, QWindow host, QProspect prospect, QGloss gloss, QCitation citation)
    {
        _cEditor = editor;
        _pSentenceHost = host;
        _qSentenceProspect = prospect;
        _qSentenceCitation = citation;
        QSentenceExample = new QExample(this, gloss, citation);
    }

    internal PCard? QSentenceCardFind(PSentence row)
    {
        foreach (PCard card in _qSentenceMeaning)
        {
            if (card.PCardSentence.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _qSentenceCollocation)
        {
            if (card.PCardSentence.Contains(row))
            {
                return card;
            }
        }

        return null;
    }

    internal void QSentenceFrameRefine(CEntryDraft _)
    {
        CSentenceFrame frame = _cEditor.CEditorSentence.CSentenceFrameRead();
        QSentenceOrder = frame.CSentenceFrameOrder;
        QSentenceListRefine(QSentenceParticle, frame.CSentenceFrameParticle);
        QSentenceListRefine(QSentenceDependence, frame.CSentenceFrameDependence);

        foreach (PCard card in _qSentenceMeaning)
        {
            card.PCardSentenceApply(frame.CSentenceFrameOrder);
        }

        foreach (PCard card in _qSentenceCollocation)
        {
            card.PCardSentenceApply(frame.CSentenceFrameOrder);
        }
    }

    internal void QSentenceMentionRefine(CEntryDraft _)
    {
        IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> lines =
            _cEditor.CEditorSentence.CSentenceMentionRead();
        QSentenceChipRefine(_qSentenceMeaning, lines);
        QSentenceChipRefine(_qSentenceCollocation, lines);
    }

    internal void QSentenceGlossObserve(PCard card, PSentence row, PGloss gloss, string language)
    {
        _cEditor.CEditorSentence.CSentenceLanguageSet(card.PCardId, row.PSentenceRow, gloss.PGlossId, language);
    }

    internal void QSentenceTextObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: PSentence row } box
            && QSentenceCardFind(row) is PCard card)
        {
            _cEditor.CEditorSentence.CSentenceTextSet(card.PCardId, row.PSentenceRow, box.Text);
        }
    }

    internal void QSentenceParticleObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is ComboBox { IsKeyboardFocusWithin: true, DataContext: PSentence row }
            && e.OriginalSource is TextBox box
            && QSentenceCardFind(row) is PCard card)
        {
            _cEditor.CEditorSentence.CSentenceParticleSet(card.PCardId, row.PSentenceRow, box.Text);
        }
    }

    internal void QSentenceDependenceObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is ComboBox { IsKeyboardFocusWithin: true, DataContext: PSentence row }
            && e.OriginalSource is TextBox box
            && QSentenceCardFind(row) is PCard card)
        {
            _cEditor.CEditorSentence.CSentenceDependenceSet(card.PCardId, row.PSentenceRow, box.Text);
        }
    }

    internal void QSentenceCitationRefine(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: PSentence row } box
            && QSentenceCardFind(row) is PCard card)
        {
            _qSentenceCitation.QCitationFieldRefine(card, row, box);
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
            foreach (PSentence row in card.PCardSentence)
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

        _cEditor.CEditorSentence.CSentenceAdd(card.PCardId, card.PCardSentenceFind(row));
    }

    internal void QSentenceRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSentence row }
            || QSentenceCardFind(row) is not PCard card)
        {
            return;
        }

        _cEditor.CEditorSentence.CSentenceRemove(card.PCardId, row.PSentenceRow);
    }

    internal void QSentenceLinkRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence } box)
        {
            return;
        }

        _qSentenceProspect.QProspectPlaceRefine(box, PMentionSelection.PMentionSelectionPlace(box));
        _qSentenceProspect.QProspectOpenRefine(_cEditor.CEditorCard.CCardMentionRead(box.SelectedText));
    }

    internal void QSentenceMeaningRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence row } box
            || QSentenceCardFind(row) is not PCard card)
        {
            return;
        }

        if (_cEditor.CEditorSentence.CSentenceSenseRead(
                card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength)
            is CMentionSense sense)
        {
            _pSentenceHost.QMentionMeaningRefine(
                box, PMentionSelection.PMentionSelectionPlace(box), sense, QSentenceSenseObserve);
        }
    }

    private void QSentenceSenseObserve(FrameworkElement anchor, long sense)
    {
        if (anchor is not TextBox { DataContext: PSentence row } box
            || QSentenceCardFind(row) is not PCard card)
        {
            return;
        }

        _cEditor.CEditorSentence.CSentenceSenseSet(
            card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength, sense);
    }

    internal void QSentenceSilenceObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence row } box
            || QSentenceCardFind(row) is not PCard card)
        {
            return;
        }

        _cEditor.CEditorSentence.CSentenceSilenceSet(
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
            _cEditor.CEditorSentence.CSentenceMentionRemove(card.PCardId, row.PSentenceRow, chip.PMentionChipId);
        }
        else if (e.Source is TextBox box)
        {
            _cEditor.CEditorSentence.CSentenceMentionRemove(
                card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength);
        }
    }

    internal void QSentenceSpanRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox box
            && _pSentenceHost.QWindowAtelier.CAtelierMention.CMentionSpanCheck(
                box.Text, box.SelectionStart, box.SelectionLength);
    }

    internal void QSentenceSenseRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox { DataContext: PSentence row } box
            && QSentenceCardFind(row) is PCard card
            && _cEditor.CEditorSentence.CSentenceSenseCheck(
                card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength);
    }

    internal void QSentenceUnlinkRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Parameter is PMentionChip
            || (e.Source is TextBox { DataContext: PSentence row } box
                && QSentenceCardFind(row) is PCard card
                && _cEditor.CEditorSentence.CSentenceMentionCheck(
                    card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength));
    }
}
