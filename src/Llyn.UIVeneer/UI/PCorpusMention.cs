using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PCorpus
{
    private readonly PMentionLine _pTranscriptChip = new();

    private IReadOnlyList<LMentionDraft> _pTranscriptMention = [];

    private void PTranscriptLinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        (int offset, int length) = PMentionSelection.PMentionSelectionRead(PTranscriptText);
        if (length == 0 || PTranscriptDraft == 0)
        {
            return;
        }

        _pCorpusHost.PWindowProspectShow(
            PTranscriptText,
            PMentionSelection.PMentionSelectionPlace(PTranscriptText),
            PTranscriptText.SelectedText.Trim(),
            _pTranscriptLanguage,
            entryId => PTranscriptRequestSend(
                new LRequestMentionAddition(PTranscriptDraft, 0, 0, offset, length, entryId, 0)));
    }

    private void PTranscriptSenseHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (PTranscriptDraft == 0)
        {
            return;
        }

        if (PTranscriptMentionFind() is not LMentionDraft mention)
        {
            return;
        }

        if (!mention.LMentionDraftLinked)
        {
            return;
        }

        _pCorpusHost.PWindowSenseShow(
            PTranscriptText,
            PMentionSelection.PMentionSelectionPlace(PTranscriptText),
            mention.LMentionDraftEntry,
            senseId => PTranscriptRequestSend(
                new LRequestMentionSense(PTranscriptDraft, 0, 0, mention.LMentionDraftId, senseId)));
    }

    private void PTranscriptSilenceHandle(object sender, ExecutedRoutedEventArgs e)
    {
        (int offset, int length) = PMentionSelection.PMentionSelectionRead(PTranscriptText);
        if (length == 0 || PTranscriptDraft == 0)
        {
            return;
        }

        PTranscriptRequestSend(new LRequestMentionAddition(PTranscriptDraft, 0, 0, offset, length, 0, 0));
    }

    private void PTranscriptUnlinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        long? mentionId = e.Parameter is PMentionChip chip
            ? chip.PMentionChipId
            : PTranscriptMentionFind()?.LMentionDraftId;
        if (mentionId is not long id || PTranscriptDraft == 0)
        {
            return;
        }

        PTranscriptRequestSend(new LRequestMentionRemoval(PTranscriptDraft, 0, 0, id));
    }

    private void PTranscriptLinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox box
            && PMentionSelection.PMentionSelectionRead(box).PMentionSelectionLength > 0;
    }

    private void PTranscriptSenseCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox && PTranscriptMentionFind() is { LMentionDraftEntry: not 0 };
    }

    private void PTranscriptUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Parameter is PMentionChip || (e.Source is TextBox && PTranscriptMentionFind() is not null);
    }

    private LMentionDraft? PTranscriptMentionFind()
    {
        (int offset, int length) = PMentionSelection.PMentionSelectionRead(PTranscriptText);
        return PMentionSelection.PMentionSelectionFind(_pTranscriptMention, offset, length);
    }

    private void PTranscriptMentionShow(LExample? example)
    {
        List<LMentionDraft> mentions = [];
        foreach (LMention mention in example?.LExampleMention ?? [])
        {
            mentions.Add(LMentionDraft.LMentionDraftCreate(mention));
        }

        _pTranscriptMention = mentions;

        if (example is null)
        {
            _pTranscriptChip.PMentionLineClear();
            return;
        }

        try
        {
            _pTranscriptChip.PMentionLineShow(
                _lEngine,
                example.LExampleText.LStateValueShow(),
                mentions,
                PLocalizationCatalog.PLocalizationTextRead("Mention.Silent"));
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Mention.FindFailed", exception);
        }
    }
}
