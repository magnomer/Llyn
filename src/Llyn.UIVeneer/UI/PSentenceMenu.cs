using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private readonly ObservableCollection<PCitationItem> _pEditorCitation = [];

    private readonly ObservableCollection<string> _pEditorParticle = [];

    private readonly ObservableCollection<string> _pEditorDependence = [];

    internal void PSentenceLoad()
    {
        _pEditorCitation.Clear();

        IReadOnlyList<LCatalogReference> references;
        try
        {
            references = _lEngine.LEngineReferenceFind(string.Empty, LCatalogOrder.LCatalogOrderAuthor);
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

        LSentenceOrder order = _lEngine.LEngineOrderRead(chosen);
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
            return _lEngine.LEngineParticleRead(language);
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
            return _lEngine.LEngineDependenceRead(language);
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

        (int offset, int length) = PMentionSelection.PMentionSelectionRead(box);
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
        if (e.Source is not TextBox { DataContext: PSentence row } box
            || PCardSentenceFind(row) is not PCard card
            || PSentenceMentionFind(box, row) is not LMentionDraft mention)
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

        (int offset, int length) = PMentionSelection.PMentionSelectionRead(box);
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
                ? PSentenceMentionFind(box, row)?.LMentionDraftId
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
            && PMentionSelection.PMentionSelectionRead(box).PMentionSelectionLength > 0;
    }

    internal void PSentenceSenseCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox { DataContext: PSentence row } box
            && PSentenceMentionFind(box, row) is { LMentionDraftEntry: not 0 };
    }

    internal void PSentenceUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Parameter is PMentionChip
            || (e.Source is TextBox { DataContext: PSentence row } box && PSentenceMentionFind(box, row) is not null);
    }

    internal void PSentenceMentionShow(PCard card)
    {
        string silent = PLocalizationCatalog.PLocalizationTextRead("Mention.Silent");
        foreach (PSentence row in card.PCardSentence)
        {
            try
            {
                row.PSentenceMentionShow(_lEngine, silent);
            }
            catch (Exception exception)
            {
                _pEditorHost.PWindowFailureShow("Mention.FindFailed", exception);
                return;
            }
        }
    }

    private static LMentionDraft? PSentenceMentionFind(TextBox box, PSentence row)
    {
        (int offset, int length) = PMentionSelection.PMentionSelectionRead(box);
        return PMentionSelection.PMentionSelectionFind(row.PSentenceMention, offset, length);
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
