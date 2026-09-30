using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly ObservableCollection<string> _pEditorParticle = [];

    private readonly ObservableCollection<string> _pEditorDependence = [];

    private CSentenceOrder? _pSentenceOrder;

    internal void PSentenceFrameRefine(CEntryDraft _)
    {
        CSentenceFrame frame = _qEditor.QEditorArea.CEditorSentence.CSentenceFrameRead();
        _pSentenceOrder = frame.CSentenceFrameOrder;
        PSentenceListRefine(_pEditorParticle, frame.CSentenceFrameParticle);
        PSentenceListRefine(_pEditorDependence, frame.CSentenceFrameDependence);

        foreach (PCard card in _pMeaningList)
        {
            card.PCardSentenceApply(frame.CSentenceFrameOrder);
        }

        foreach (PCard card in _pCollocationList)
        {
            card.PCardSentenceApply(frame.CSentenceFrameOrder);
        }
    }

    private static void PSentenceListRefine(ObservableCollection<string> catalog, IReadOnlyList<string> values)
    {
        catalog.Clear();
        foreach (string value in values)
        {
            catalog.Add(value);
        }
    }

    private void PSentenceApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PSentence row)
        {
            return;
        }

        PSentence.PSentenceRowApply(container, row);
        if (QLook.QLookPartFind<Grid>(container, "PSentenceReach") is Grid reach && reach.CommandBindings.Count == 0)
        {
            reach.CommandBindings.Add(new CommandBinding(
                PMentionCommand.PMentionCommandLink, PSentenceLinkRefine, PSentenceSpanRefine));
            reach.CommandBindings.Add(new CommandBinding(
                PMentionCommand.PMentionCommandChoose, PSentenceMeaningRefine, PSentenceSenseRefine));
            reach.CommandBindings.Add(new CommandBinding(
                PMentionCommand.PMentionCommandSilence, PSentenceSilenceObserve, PSentenceSpanRefine));
            reach.CommandBindings.Add(new CommandBinding(
                PMentionCommand.PMentionCommandUnlink, PSentenceUnlinkObserve, PSentenceUnlinkRefine));
            reach.CommandBindings.Add(new CommandBinding(
                PGlossCommand.PGlossCommandRemoval, PGlossRemoveObserve));
        }

        if (QLook.QLookPartFind<ToggleButton>(container, "PSentenceOpening") is ToggleButton opening)
        {
            opening.Click -= PSentenceOpeningRefine;
            opening.Click += PSentenceOpeningRefine;
        }

        if (QLook.QLookPartFind<Button>(container, "PSentenceAdder") is Button adder)
        {
            adder.Click -= PSentenceAddObserve;
            adder.Click += PSentenceAddObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PSentenceEraser") is Button eraser)
        {
            eraser.Click -= PSentenceRemoveObserve;
            eraser.Click += PSentenceRemoveObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PSentenceGlossChooser") is Button gloss)
        {
            gloss.Click -= PGlossAddObserve;
            gloss.Click += PGlossAddObserve;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PSentenceCitation") is TextBox citation)
        {
            citation.PreviewKeyDown -= PProfferKeyRefine;
            citation.PreviewKeyDown -= PProfferKeyObserve;
            citation.PreviewKeyDown -= PCitationCommitObserve;
            citation.PreviewKeyDown -= PCitationEscapeRefine;
            citation.PreviewKeyDown += PProfferKeyRefine;
            citation.PreviewKeyDown += PProfferKeyObserve;
            citation.PreviewKeyDown += PCitationCommitObserve;
            citation.PreviewKeyDown += PCitationEscapeRefine;
            citation.LostKeyboardFocus -= PCitationLeaveRefine;
            citation.LostKeyboardFocus += PCitationLeaveRefine;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PSentenceMentionLine") is ItemsControl mention)
        {
            mention.ItemsSource = row.PSentenceChip.PMentionLineChip;
            QLookItem.QLookItemAttach(mention, PMentionChip.PMentionChipRefine);
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PSentenceGlossLine") is ItemsControl glosses)
        {
            glosses.ItemsSource = row.PSentenceGloss;
            QLookItem.QLookItemAttach(glosses, PGloss.PGlossRowApply);
        }

        if (ItemsControl.ItemsControlFromItemContainer(container) is ItemsControl list)
        {
            PSentenceRevealRefine(list);
        }
    }

    private void PSentenceOpeningRefine(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton { DataContext: PSentence row } opening)
        {
            row.PSentenceFrameVisible = QLook.QLookCheckedRead(opening.IsChecked);
        }
    }

    private void PSentenceAddObserve(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSentence row } || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        _qEditor.QEditorArea.CEditorSentence.CSentenceAdd(card.PCardId, card.PCardSentenceFind(row));
    }

    private void PSentenceRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSentence row } || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        _qEditor.QEditorArea.CEditorSentence.CSentenceRemove(card.PCardId, row.PSentenceRow);
    }

    private void PSentenceLinkRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence } box)
        {
            return;
        }

        PProspectPlaceRefine(box, PMentionSelection.PMentionSelectionPlace(box));
        PProspectOpenRefine(_qEditor.QEditorArea.CEditorCard.CCardMentionRead(box.SelectedText));
    }

    private void PSentenceMeaningRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence row } box || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        if (_qEditor.QEditorArea.CEditorSentence.CSentenceSenseRead(
                card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength)
            is CMentionSense sense)
        {
            _pEditorHost.PMentionMeaningRefine(
                box, PMentionSelection.PMentionSelectionPlace(box), sense, PSentenceSenseObserve);
        }
    }

    private void PSentenceSenseObserve(FrameworkElement anchor, long sense)
    {
        if (anchor is not TextBox { DataContext: PSentence row } box || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        _qEditor.QEditorArea.CEditorSentence.CSentenceSenseSet(
            card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength, sense);
    }

    private void PSentenceSilenceObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence row } box || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        _qEditor.QEditorArea.CEditorSentence.CSentenceMentionAdd(
            card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength, 0);
    }

    private void PSentenceUnlinkObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSentence row } || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        if (e.Parameter is PMentionChip chip)
        {
            _qEditor.QEditorArea.CEditorSentence.CSentenceMentionRemove(
                card.PCardId, row.PSentenceRow, chip.PMentionChipId);
        }
        else if (e.Source is TextBox box)
        {
            _qEditor.QEditorArea.CEditorSentence.CSentenceMentionRemove(
                card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength);
        }
    }

    private void PSentenceSpanRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox box
            && _pEditorHost.PWindowAtelier.CAtelierMention.CMentionSpanCheck(
                box.Text, box.SelectionStart, box.SelectionLength);
    }

    private void PSentenceSenseRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox { DataContext: PSentence row } box
            && PCardSentenceFind(row) is PCard card
            && _qEditor.QEditorArea.CEditorSentence.CSentenceSenseCheck(
                card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength);
    }

    private void PSentenceUnlinkRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Parameter is PMentionChip
            || (e.Source is TextBox { DataContext: PSentence row } box
                && PCardSentenceFind(row) is PCard card
                && _qEditor.QEditorArea.CEditorSentence.CSentenceMentionCheck(
                    card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength));
    }

    internal void PSentenceMentionRefine(CEntryDraft _)
    {
        IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> lines =
            _qEditor.QEditorArea.CEditorSentence.CSentenceMentionRead();
        PSentenceChipRefine(_pMeaningList, lines);
        PSentenceChipRefine(_pCollocationList, lines);
    }

    private static void PSentenceChipRefine(
        IReadOnlyList<PCard> cards, IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> lines)
    {
        foreach (PCard card in cards)
        {
            foreach (PSentence row in card.PCardSentence)
            {
                if (lines.TryGetValue(row.PSentenceRow, out IReadOnlyList<CMentionLabel>? labels))
                {
                    row.PSentenceChip.PMentionLineRefine(labels);
                }
            }
        }
    }

    private void PSentenceGlossObserve(PCard card, PSentence row, PGloss gloss, string language)
    {
        _qEditor.QEditorArea.CEditorSentence.CSentenceLanguageSet(
            card.PCardId, row.PSentenceRow, gloss.PGlossId, language);
    }

    private void PSentenceFieldObserve(PCard card, PSentence row, string field, TextBox box)
    {
        switch (field)
        {
            case nameof(PSentence.PSentenceText):
                _qEditor.QEditorArea.CEditorSentence.CSentenceTextSet(card.PCardId, row.PSentenceRow, box.Text);
                break;
            case nameof(PSentence.PSentenceParticle):
                _qEditor.QEditorArea.CEditorSentence.CSentenceParticleSet(card.PCardId, row.PSentenceRow, box.Text);
                break;
            case nameof(PSentence.PSentenceDependence):
                _qEditor.QEditorArea.CEditorSentence.CSentenceDependenceSet(card.PCardId, row.PSentenceRow, box.Text);
                break;
            case nameof(PSentence.PSentenceCitation):
                PProfferCitationRefine(
                    box,
                    _qEditor.QEditorArea.CEditorCard.CCardReferenceFind(card.PCardId, row.PSentenceRow, box.Text));
                break;
        }
    }

    private PCard? PCardSentenceFind(PSentence row)
    {
        foreach (PCard card in _pMeaningList)
        {
            if (card.PCardSentence.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _pCollocationList)
        {
            if (card.PCardSentence.Contains(row))
            {
                return card;
            }
        }

        return null;
    }
}
