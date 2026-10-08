using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LMarkupClerkEntry
{
    private readonly LEntryVault _lMarkupEntryRows;
    private readonly LSpeechVault _lMarkupEntrySpeeches;
    private readonly LMorphologyVault _lMarkupEntryMorphologies;
    private readonly LReflexClerk _lMarkupEntryReflexes;
    private readonly LMarkupClerkCard _lMarkupEntryCard;
    private readonly LMarkupClerkExample _lMarkupEntryExample;

    public LMarkupClerkEntry(LRig rig, LReflexClerk reflexes, LMarkupClerkCard card, LMarkupClerkExample example)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(reflexes);
        ArgumentNullException.ThrowIfNull(card);
        ArgumentNullException.ThrowIfNull(example);
        _lMarkupEntryRows = rig.LRigEntries;
        _lMarkupEntrySpeeches = rig.LRigSpeeches;
        _lMarkupEntryMorphologies = rig.LRigMorphologies;
        _lMarkupEntryReflexes = reflexes;
        _lMarkupEntryCard = card;
        _lMarkupEntryExample = example;
    }

    public LMarkupEntry? LMarkupLoad(long id)
    {
        LEntryDraft? draft = _lMarkupEntryRows.LEntryLoad(id);
        if (draft is null)
        {
            return null;
        }

        List<string> speeches = new(draft.LEntryDraftSpeeches.Count);
        foreach (LSpeechDraft speech in draft.LEntryDraftSpeeches)
        {
            speeches.Add(speech.LSpeechDraftName);
        }

        List<LMarkupInflection> inflections = new(draft.LEntryDraftInflections.Count);
        foreach (LInflection inflection in draft.LEntryDraftInflections)
        {
            inflections.Add(LMarkupInflectionCreate(inflection));
        }

        List<LPronunciationDraft> pronunciations = new(draft.LEntryDraftPronunciations.Count);
        foreach (LPronunciationDraft pronunciation in draft.LEntryDraftPronunciations)
        {
            pronunciations.Add(pronunciation with { LPronunciationDraftId = 0, LPronunciationDraftSeeded = false });
        }

        List<LTranscriptionDraft> transcriptions = new(draft.LEntryDraftTranscriptions.Count);
        foreach (LTranscriptionDraft transcription in draft.LEntryDraftTranscriptions)
        {
            transcriptions.Add(transcription with { LTranscriptionDraftId = 0, LTranscriptionDraftSeeded = false });
        }

        IReadOnlyList<LReflexDraft> sorted =
            _lMarkupEntryReflexes.LReflexClerkSort(draft.LEntryDraftLanguage, draft.LEntryDraftReflexes);
        List<LReflexDraft> reflexes = new(sorted.Count);
        foreach (LReflexDraft reflex in sorted)
        {
            reflexes.Add(reflex with
            {
                LReflexDraftId = 0,
                LReflexDraftAnatomy = LAnatomy.LAnatomyEmpty,
                LReflexDraftAnchors = [],
            });
        }

        return new LMarkupEntry(
            draft.LEntryDraftHeadword,
            draft.LEntryDraftLanguage,
            speeches,
            draft.LEntryDraftForms,
            inflections,
            pronunciations,
            transcriptions,
            reflexes,
            _lMarkupEntryCard.LMarkupCardCreate(draft.LEntryDraftMeanings),
            _lMarkupEntryCard.LMarkupCardCreate(draft.LEntryDraftCollocations),
            draft.LEntryDraftNote,
            0,
            LMarkupEtymologyCreate(draft.LEntryDraftEtymology),
            LMarkupEtymonCreate(draft.LEntryDraftEtymology),
            draft.LEntryDraftUnit);
    }

    private LMarkupEtymology? LMarkupEtymologyCreate(LEtymologyDraft etymology)
    {
        if (!etymology.LEtymologyDraftNarrated)
        {
            return null;
        }

        List<LMarkupMention> mentions = new(etymology.LEtymologyDraftMentions.Count);
        foreach (LMentionDraft mention in etymology.LEtymologyDraftMentions)
        {
            LMarkupMention written = _lMarkupEntryExample.LMarkupMentionCreate(mention with { LMentionDraftSense = 0 });
            if (written.LMarkupMentionHeadword.Length > 0)
            {
                mentions.Add(written);
            }
        }

        return new LMarkupEtymology(etymology.LEtymologyDraftText, mentions);
    }

    private IReadOnlyList<LMarkupEtymon> LMarkupEtymonCreate(LEtymologyDraft etymology)
    {
        List<LMarkupEtymon> etymons = new(etymology.LEtymologyDraftEtymons.Count);
        foreach (long id in etymology.LEtymologyDraftEtymons)
        {
            if (_lMarkupEntryRows.LEntryRead(id) is LEntry entry)
            {
                etymons.Add(new LMarkupEtymon(entry.LEntryHeadword, entry.LEntryLanguage));
            }
        }

        return etymons;
    }

    private LMarkupInflection LMarkupInflectionCreate(LInflection inflection)
    {
        string speech = inflection.LInflectionSpeechId is long speechId
            ? _lMarkupEntrySpeeches.LSpeechValueRead(speechId)?.LSpeechValueName ?? string.Empty
            : string.Empty;

        List<string> names = new(inflection.LInflectionMorphology.Count);
        foreach (long morphologyId in inflection.LInflectionMorphology)
        {
            if (_lMarkupEntryMorphologies.LMorphologyRead(morphologyId) is LMorphology morphology)
            {
                names.Add(morphology.LMorphologyName);
            }
        }

        return new LMarkupInflection(inflection.LInflectionText, inflection.LInflectionLocal, speech, names);
    }
}
