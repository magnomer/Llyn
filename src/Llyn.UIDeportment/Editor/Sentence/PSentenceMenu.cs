using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PSentenceTemplate _pSentenceTemplate;

    private readonly ObservableCollection<QCitationItem> _pEditorCitation = [];

    private readonly ObservableCollection<string> _pEditorParticle = [];

    private readonly ObservableCollection<string> _pEditorDependence = [];

    private CSentenceOrder? _pSentenceOrder;

    internal void PSentenceLoad()
    {
        _pEditorCitation.Clear();

        IReadOnlyList<CCatalogReference> references;
        try
        {
            references = _qEditor.QEditorArea.CEditorCard.CCardReferenceFind();
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureRefine("Reference.LoadFailed", exception);
            references = [];
        }

        foreach (CCatalogReference row in references)
        {
            _pEditorCitation.Add(QCitationItem.QCitationItemCreate(row));
        }

        PSentenceCitationShow();
    }

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
                PMentionCommand.PMentionCommandLink,
                _pSentenceTemplate.PSentenceLinkHandle,
                _pSentenceTemplate.PSentenceLinkCheck));
            reach.CommandBindings.Add(new CommandBinding(
                PMentionCommand.PMentionCommandChoose,
                _pSentenceTemplate.PSentenceSenseHandle,
                _pSentenceTemplate.PSentenceSenseCheck));
            reach.CommandBindings.Add(new CommandBinding(
                PMentionCommand.PMentionCommandSilence,
                _pSentenceTemplate.PSentenceSilenceHandle,
                _pSentenceTemplate.PSentenceLinkCheck));
            reach.CommandBindings.Add(new CommandBinding(
                PMentionCommand.PMentionCommandUnlink,
                _pSentenceTemplate.PSentenceUnlinkHandle,
                _pSentenceTemplate.PSentenceUnlinkCheck));
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
            citation.PreviewKeyDown -= _pSentenceTemplate.PCitationKeyHandle;
            citation.PreviewKeyDown += PProfferKeyRefine;
            citation.PreviewKeyDown += PProfferKeyObserve;
            citation.PreviewKeyDown += _pSentenceTemplate.PCitationKeyHandle;
            citation.LostKeyboardFocus -= _pSentenceTemplate.PCitationLeaveHandle;
            citation.LostKeyboardFocus += _pSentenceTemplate.PCitationLeaveHandle;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PSentenceMentionLine") is ItemsControl mention)
        {
            mention.ItemsSource = row.PSentenceChip.PMentionLineChip;
            QLookItem.QLookItemAttach(mention, PMentionChip.PMentionChipApply);
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PSentenceGlossLine") is ItemsControl glosses)
        {
            glosses.ItemsSource = row.PSentenceGloss;
            QLookItem.QLookItemAttach(glosses, PGloss.PGlossRowApply);
        }

        if (ItemsControl.ItemsControlFromItemContainer(container) is ItemsControl list)
        {
            PSentenceRevealApply(list);
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

    internal void PSentenceLinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence row } box || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        (int offset, int length) = PMentionSelection.PMentionSelectionRead(box, _pEditorHost.PWindowAtelier);
        if (length == 0)
        {
            return;
        }

        long cardId = card.PCardId;
        long rowId = row.PSentenceRow;
        PProspectShow(
            box,
            PMentionSelection.PMentionSelectionPlace(box),
            box.SelectedText.Trim(),
            _qEditor.QEditorArea.CEditorLanguage,
            entryId => PEditorRequestSend(
                new LRequestMentionAddition(PEditorDraft, cardId, rowId, offset, length, entryId, 0)));
    }

    internal void PSentenceSenseHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence row } box
            || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        PSentenceSenseShow(box, card, row, PSentenceMentionFind(box, card, row, true));
    }

    private void PSentenceSenseShow(TextBox box, PCard card, PSentence row, CMentionDraft? mention)
    {
        if (mention is not { CMentionDraftLinked: true })
        {
            return;
        }

        long cardId = card.PCardId;
        long rowId = row.PSentenceRow;
        _pEditorHost.PWindowSenseRefine(
            box,
            PMentionSelection.PMentionSelectionPlace(box),
            mention.CMentionDraftEntry,
            senseId => PEditorRequestSend(
                new LRequestMentionSense(PEditorDraft, cardId, rowId, mention.CMentionDraftId, senseId)));
    }

    internal void PSentenceSilenceHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence row } box || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        (int offset, int length) = PMentionSelection.PMentionSelectionRead(box, _pEditorHost.PWindowAtelier);
        if (length == 0)
        {
            return;
        }

        PEditorRequestSend(
            new LRequestMentionAddition(PEditorDraft, card.PCardId, row.PSentenceRow, offset, length, 0, 0));
    }

    internal void PSentenceUnlinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSentence row } || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        long? mentionId = e.Parameter is PMentionChip chip
            ? chip.PMentionChipId
            : e.Source is TextBox box
                ? PSentenceMentionFind(box, card, row, true)?.CMentionDraftId
                : null;
        if (mentionId is not long id)
        {
            return;
        }

        PEditorRequestSend(new LRequestMentionRemoval(PEditorDraft, card.PCardId, row.PSentenceRow, id));
    }

    internal void PSentenceLinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox box
            && PMentionSelection.PMentionSelectionRead(box, _pEditorHost.PWindowAtelier).PMentionSelectionLength > 0;
    }

    internal void PSentenceSenseCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox { DataContext: PSentence row } box
            && PCardSentenceFind(row) is PCard card
            && PSentenceMentionFind(box, card, row, false) is { CMentionDraftEntry: not 0 };
    }

    internal void PSentenceUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Parameter is PMentionChip
            || (e.Source is TextBox { DataContext: PSentence row } box
                && PCardSentenceFind(row) is PCard card
                && PSentenceMentionFind(box, card, row, false) is not null);
    }

    internal void PSentenceMentionShow(PCard card)
    {
        string silent = QLocalizationCatalog.QLocalizationTextRead("Mention.Silent");
        foreach (PSentence row in card.PCardSentence)
        {
            try
            {
                row.PSentenceMentionShow(_pEditorHost.PWindowAtelier, silent);
            }
            catch (Exception exception)
            {
                _pEditorHost.PWindowFailureRefine("Mention.FindFailed", exception);
                return;
            }
        }
    }

    private CMentionDraft? PSentenceMentionFind(TextBox box, PCard card, PSentence row, bool settled)
    {
        return _pEditorHost.PWindowAtelier.CAtelierMention.CMentionFind(
            _qEditor.QEditorArea.CEditorDesk, card.PCardId, row.PSentenceRow,
            box.Text, box.SelectionStart, box.SelectionLength, settled);
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
