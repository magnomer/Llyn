using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PSentenceTemplate _pSentenceTemplate;

    private readonly ObservableCollection<PCitationItem> _pEditorCitation = [];

    private readonly ObservableCollection<string> _pEditorParticle = [];

    private readonly ObservableCollection<string> _pEditorDependence = [];

    internal void PSentenceLoad()
    {
        _pEditorCitation.Clear();

        IReadOnlyList<LCatalogReference> references;
        try
        {
            references = _lEditor.LEditorCard.LCardReferenceFind();
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow("Reference.LoadFailed", exception);
            references = [];
        }

        foreach (LCatalogReference row in references)
        {
            _pEditorCitation.Add(PCitationItem.PCitationItemCreate(row));
        }

        PSentenceCitationShow();
    }

    internal void PSentenceFrameLoad(string language)
    {
        string chosen = string.IsNullOrWhiteSpace(language) ? _lEditor.LEditorLanguage : language;

        LSentenceOrder order = _lEditor.LEditorCard.LCardOrderRead(chosen);
        PSentenceFrameShow(_pEditorParticle, PSentenceParticleRead(chosen));
        PSentenceFrameShow(_pEditorDependence, PSentenceDependenceRead(chosen));

        foreach (PCard card in _pMeaningList)
        {
            card.PCardSentenceApply(order);
        }

        foreach (PCard card in _pCollocationList)
        {
            card.PCardSentenceApply(order);
        }
    }

    private IReadOnlyList<string> PSentenceParticleRead(string language)
    {
        try
        {
            return _lEditor.LEditorCard.LCardParticleRead(language);
        }
        catch (Exception)
        {
            return [];
        }
    }

    private IReadOnlyList<string> PSentenceDependenceRead(string language)
    {
        try
        {
            return _lEditor.LEditorCard.LCardDependenceRead(language);
        }
        catch (Exception)
        {
            return [];
        }
    }

    private static void PSentenceFrameShow(ObservableCollection<string> catalog, IReadOnlyList<string> values)
    {
        catalog.Clear();
        foreach (string value in values)
        {
            catalog.Add(value);
        }
    }

    internal void PSentenceAttach(PCard card)
    {
        card.PCardSentenceNotice = (row, field) => PSentenceChangeHandle(card, row, field);
    }

    private void PSentenceApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PSentence row)
        {
            return;
        }

        PSentence.PSentenceRowApply(container, row);
        if (PLook.PLookPartFind<Grid>(container, "PSentenceReach") is Grid reach && reach.CommandBindings.Count == 0)
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
                PGlossCommand.PGlossCommandRemoval, _pSentenceTemplate.PGlossRemoveHandle));
        }

        if (PLook.PLookPartFind<ToggleButton>(container, "PSentenceOpening") is ToggleButton opening)
        {
            opening.Click -= PSentenceFrameHandle;
            opening.Click += PSentenceFrameHandle;
        }

        if (PLook.PLookPartFind<Button>(container, "PSentenceAdder") is Button adder)
        {
            adder.Click -= _pSentenceTemplate.PSentenceAddHandle;
            adder.Click += _pSentenceTemplate.PSentenceAddHandle;
        }

        if (PLook.PLookPartFind<Button>(container, "PSentenceEraser") is Button eraser)
        {
            eraser.Click -= _pSentenceTemplate.PSentenceRemoveHandle;
            eraser.Click += _pSentenceTemplate.PSentenceRemoveHandle;
        }

        if (PLook.PLookPartFind<Button>(container, "PSentenceGlossChooser") is Button gloss)
        {
            gloss.Click -= _pSentenceTemplate.PGlossAddHandle;
            gloss.Click += _pSentenceTemplate.PGlossAddHandle;
        }

        if (PLook.PLookPartFind<TextBox>(container, "PSentenceCitation") is TextBox citation)
        {
            citation.PreviewKeyDown -= _pSentenceTemplate.PCitationKeyHandle;
            citation.PreviewKeyDown += _pSentenceTemplate.PCitationKeyHandle;
            citation.LostKeyboardFocus -= _pSentenceTemplate.PCitationLeaveHandle;
            citation.LostKeyboardFocus += _pSentenceTemplate.PCitationLeaveHandle;
        }

        if (PLook.PLookPartFind<ItemsControl>(container, "PSentenceMentionLine") is ItemsControl mention)
        {
            mention.ItemsSource = row.PSentenceChip.PMentionLineChip;
            PLookItem.PLookItemAttach(mention, PMentionChip.PMentionChipApply);
        }

        if (PLook.PLookPartFind<ItemsControl>(container, "PSentenceGlossLine") is ItemsControl glosses)
        {
            glosses.ItemsSource = row.PSentenceGloss;
            PLookItem.PLookItemAttach(glosses, PGloss.PGlossRowApply);
        }

        if (ItemsControl.ItemsControlFromItemContainer(container) is ItemsControl list)
        {
            PSentenceRevealApply(list);
        }
    }

    private void PSentenceFrameHandle(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton { DataContext: PSentence row } opening)
        {
            row.PSentenceFrameVisible = PLook.PLookCheckedRead(opening.IsChecked);
        }
    }

    internal void PSentenceAddHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSentence row } || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        PEditorRequestSend(new LRequestSentenceAddition(PEditorDraft, card.PCardId, card.PCardSentenceFind(row) + 1));
    }

    internal void PSentenceRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSentence row } || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        PEditorRequestSend(new LRequestSentenceRemoval(PEditorDraft, card.PCardId, row.PSentenceRow));
    }

    internal void PSentenceLinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence row } box || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        (int offset, int length) = PMentionSelection.PMentionSelectionRead(box, _pEditorHost.PWindowDeportment);
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
            _lEditor.LEditorLanguage,
            entryId => PEditorRequestSend(
                new LRequestMentionAddition(PEditorDraft, cardId, rowId, offset, length, entryId, 0)));
    }

    internal void PSentenceSenseHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _lEditor.LEditorDesk.LDeskPersist();
        if (e.Source is not TextBox { DataContext: PSentence row } box
            || PCardSentenceFind(row) is not PCard card
            || PSentenceMentionFind(box, card, row) is not LMentionDraft mention)
        {
            return;
        }

        if (!mention.LMentionDraftLinked)
        {
            return;
        }

        long cardId = card.PCardId;
        long rowId = row.PSentenceRow;
        _pEditorHost.PWindowSenseShow(
            box,
            PMentionSelection.PMentionSelectionPlace(box),
            mention.LMentionDraftEntry,
            senseId => PEditorRequestSend(
                new LRequestMentionSense(PEditorDraft, cardId, rowId, mention.LMentionDraftId, senseId)));
    }

    internal void PSentenceSilenceHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence row } box || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        (int offset, int length) = PMentionSelection.PMentionSelectionRead(box, _pEditorHost.PWindowDeportment);
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

        _lEditor.LEditorDesk.LDeskPersist();
        long? mentionId = e.Parameter is PMentionChip chip
            ? chip.PMentionChipId
            : e.Source is TextBox box
                ? PSentenceMentionFind(box, card, row)?.LMentionDraftId
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
            && PMentionSelection.PMentionSelectionRead(box, _pEditorHost.PWindowDeportment).PMentionSelectionLength > 0;
    }

    internal void PSentenceSenseCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox { DataContext: PSentence row } box
            && PCardSentenceFind(row) is PCard card
            && PSentenceMentionFind(box, card, row) is { LMentionDraftEntry: not 0 };
    }

    internal void PSentenceUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Parameter is PMentionChip
            || (e.Source is TextBox { DataContext: PSentence row } box
                && PCardSentenceFind(row) is PCard card
                && PSentenceMentionFind(box, card, row) is not null);
    }

    internal void PSentenceMentionShow(PCard card)
    {
        string silent = PLocalizationCatalog.PLocalizationTextRead("Mention.Silent");
        foreach (PSentence row in card.PCardSentence)
        {
            try
            {
                row.PSentenceMentionShow(_pEditorHost.PWindowDeportment, silent);
            }
            catch (Exception exception)
            {
                _pEditorHost.PWindowFailureShow("Mention.FindFailed", exception);
                return;
            }
        }
    }

    private LMentionDraft? PSentenceMentionFind(TextBox box, PCard card, PSentence row)
    {
        return _lEditor.LEditorDesk.LDeskMentionFind(
            card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength);
    }

    private void PSentenceChangeHandle(PCard card, PSentence row, string field)
    {
        if (row.PSentenceGlossFind(field, out string name) is PGloss gloss)
        {
            PGlossChangeHandle(card, row, gloss, name);
        }
    }

    private void PSentenceChangeHandle(PCard card, PSentence row, string field, TextBox box)
    {
        LStateWritten written = new(box.Text);
        switch (field)
        {
            case nameof(PSentence.PSentenceText):
                PEditorRequestDefer(new LRequestSentenceText(PEditorDraft, card.PCardId, row.PSentenceRow, written));
                break;
            case nameof(PSentence.PSentenceParticle):
                PEditorRequestDefer(
                    new LRequestSentenceParticle(PEditorDraft, card.PCardId, row.PSentenceRow, written));
                break;
            case nameof(PSentence.PSentenceDependence):
                PEditorRequestDefer(
                    new LRequestSentenceDependence(PEditorDraft, card.PCardId, row.PSentenceRow, written));
                break;
            case nameof(PSentence.PSentenceCitation):
                PCandidateCitationShow(card, row, box);
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
