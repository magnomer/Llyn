using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PCitationItem> _pEditorCitation = [];

    private readonly ObservableCollection<string> _pEditorParticle = [];

    private readonly ObservableCollection<string> _pEditorDependence = [];

    private LSentenceOrder _pEditorSentenceOrder = LSentenceOrder.LSentenceOrderDefault;

    internal void PSentenceLoad()
    {
        _pEditorCitation.Clear();

        IReadOnlyList<LReference> references;
        try
        {
            references = _lEngine.LEngineReferenceRead();
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow("Reference.LoadFailed", exception);
            references = [];
        }

        foreach (LReference reference in references)
        {
            _pEditorCitation.Add(PCitationItem.PCitationItemCreate(reference));
        }

        PSentenceCitationShow();
    }

    internal void PSentenceFrameLoad(string language)
    {
        string chosen = string.IsNullOrWhiteSpace(language) ? _pSpeakerChoice : language;

        _pEditorSentenceOrder = _lEngine.LEngineOrderRead(chosen);
        PSentenceFrameShow(_pEditorParticle, PSentenceParticleRead(chosen));
        PSentenceFrameShow(_pEditorDependence, PSentenceDependenceRead(chosen));

        foreach (PCard card in _pMeaningList)
        {
            card.PCardSentenceApply(_pEditorSentenceOrder);
        }

        foreach (PCard card in _pCollocationList)
        {
            card.PCardSentenceApply(_pEditorSentenceOrder);
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

        PEditorRequestSend(new LRequestSentenceAddition(_pEditorDraft, card.PCardId, card.PCardSentenceFind(row) + 1));
    }

    internal void PSentenceRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSentence row } || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        PEditorRequestSend(new LRequestSentenceRemoval(_pEditorDraft, card.PCardId, row.PSentenceRow));
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
            _pSpeakerChoice,
            entryId => PEditorRequestSend(
                new LRequestMentionAddition(_pEditorDraft, cardId, rowId, offset, length, entryId, 0)));
    }

    internal void PSentenceSenseHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence row } box
            || PCardSentenceFind(row) is not PCard card
            || PSentenceMentionFind(box, row) is not LMentionDraft mention
            || mention.LMentionDraftEntry == 0)
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
                new LRequestMentionSense(_pEditorDraft, cardId, rowId, mention.LMentionDraftId, senseId)));
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
            new LRequestMentionAddition(_pEditorDraft, card.PCardId, row.PSentenceRow, offset, length, 0, 0));
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

        PEditorRequestSend(new LRequestMentionRemoval(_pEditorDraft, card.PCardId, row.PSentenceRow, id));
    }

    internal void PSentenceLinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox box && PMentionSelection.PMentionSelectionRead(box).PMentionSelectionLength > 0;
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
        string silent = _pEditorHost.PLocalizationTextRead("Mention.Silent");
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

    internal void PSentenceCitationClear(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PSentence row })
        {
            row.PSentenceCitationId = 0;
        }
    }

    private void PSentenceChangeHandle(PCard card, PSentence row, string field)
    {
        if (row.PSentenceGlossFind(field, out string name) is PGloss gloss)
        {
            PGlossChangeHandle(card, row, gloss, name);
            return;
        }

        switch (field)
        {
            case nameof(PSentence.PSentenceText):
                PEditorRequestDefer(
                    PEditorRequestFormat(card, row.PSentenceRow, field),
                    new LRequestSentenceText(_pEditorDraft, card.PCardId, row.PSentenceRow, row.PSentenceTextRead()));
                break;
            case nameof(PSentence.PSentenceParticle):
                PEditorRequestDefer(
                    PEditorRequestFormat(card, row.PSentenceRow, field),
                    new LRequestSentenceParticle(
                        _pEditorDraft, card.PCardId, row.PSentenceRow, row.PSentenceParticleRead()));
                break;
            case nameof(PSentence.PSentenceDependence):
                PEditorRequestDefer(
                    PEditorRequestFormat(card, row.PSentenceRow, field),
                    new LRequestSentenceDependence(
                        _pEditorDraft, card.PCardId, row.PSentenceRow, row.PSentenceDependenceRead()));
                break;
            case nameof(PSentence.PSentenceCitationId):
                PEditorRequestSend(new LRequestSentenceReference(
                    _pEditorDraft, card.PCardId, row.PSentenceRow, row.PSentenceCitationId));
                break;
        }
    }

    private bool PSentencePendingCheck(PCard card, PSentence row, string field)
    {
        return PEditorRequestCheck(PEditorRequestFormat(card, row.PSentenceRow, field));
    }

    private void PSentencePrepare()
    {
        List<PCard> bare = [];
        PSentencePrepare(_pMeaningList, bare);
        PSentencePrepare(_pCollocationList, bare);

        foreach (PCard card in bare)
        {
            if (card.PCardSentence.Count == 0)
            {
                PEditorRequestSend(new LRequestSentenceAddition(_pEditorDraft, card.PCardId, 0));
            }
        }
    }

    private static void PSentencePrepare(IReadOnlyList<PCard> cards, List<PCard> bare)
    {
        foreach (PCard card in cards)
        {
            if (card.PCardSentence.Count == 0)
            {
                bare.Add(card);
            }
        }
    }

    private void PSentenceCitationShow()
    {
        foreach (PCard card in _pMeaningList)
        {
            PSentenceCitationShow(card);
        }

        foreach (PCard card in _pCollocationList)
        {
            PSentenceCitationShow(card);
        }
    }

    private static void PSentenceCitationShow(PCard card)
    {
        foreach (PSentence row in card.PCardSentence)
        {
            row.PSentenceCitationShow();
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
