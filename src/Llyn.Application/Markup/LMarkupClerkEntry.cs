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
    private readonly LMarkupClerkAncestor _lMarkupEntryAncestor;

    public LMarkupClerkEntry(LRig rig, LReflexClerk reflexes, LMarkupClerkCard card, LMarkupClerkAncestor ancestor)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(reflexes);
        ArgumentNullException.ThrowIfNull(card);
        ArgumentNullException.ThrowIfNull(ancestor);
        _lMarkupEntryRows = rig.LRigEntries;
        _lMarkupEntrySpeeches = rig.LRigLexicon.LRigLexiconSpeeches;
        _lMarkupEntryMorphologies = rig.LRigLexicon.LRigLexiconMorphologies;
        _lMarkupEntryReflexes = reflexes;
        _lMarkupEntryCard = card;
        _lMarkupEntryAncestor = ancestor;
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
            _lMarkupEntryCard.LMarkupCardCreate(draft.LEntryDraftMeanings, draft.LEntryDraftMeanings),
            _lMarkupEntryCard.LMarkupCardCreate(draft.LEntryDraftCollocations, draft.LEntryDraftMeanings),
            draft.LEntryDraftNote,
            0,
            draft.LEntryDraftUnit,
            draft.LEntryDraftFreedom,
            _lMarkupEntryAncestor.LMarkupAncestorCreate(draft.LEntryDraftAncestors));
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
