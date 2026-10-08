using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LEntryClerk
{
    private readonly LVault _lEntryClerkVault;
    private readonly LEntryVault _lEntryClerkEntries;
    private readonly LEtymologyVault _lEntryClerkEtymologies;
    private readonly LFrequencyClerk _lEntryClerkFrequencies;
    private readonly LNoteVault _lEntryClerkNotes;
    private readonly LTombstoneVault _lEntryClerkTombstones;
    private readonly LCardClerk _lEntryClerkCards;
    private readonly LMeaningClerk _lEntryClerkMeanings;
    private readonly LVocabularyClerk _lEntryClerkVocabulary;
    private readonly LInflectionClerk _lEntryClerkInflections;
    private readonly LParadigmClerk _lEntryClerkParadigms;
    private readonly LPronunciationClerk _lEntryClerkPronunciations;
    private readonly LTranscriptionClerk _lEntryClerkTranscriptions;
    private readonly LReflexClerk _lEntryClerkReflexes;
    private readonly LRecordingClerk _lEntryClerkRecordings;
    private readonly LRevisionClerk _lEntryClerkRevisions;

    public LEntryClerk(
        LRig rig,
        LCardClerk cards,
        LMeaningClerk meanings,
        LVocabularyClerk vocabulary,
        LInflectionClerk inflections,
        LParadigmClerk paradigms,
        LPronunciationClerk pronunciations,
        LTranscriptionClerk transcriptions,
        LReflexClerk reflexes,
        LRecordingClerk recordings,
        LFrequencyClerk frequencies,
        LRevisionClerk revisions)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentNullException.ThrowIfNull(meanings);
        ArgumentNullException.ThrowIfNull(vocabulary);
        ArgumentNullException.ThrowIfNull(inflections);
        ArgumentNullException.ThrowIfNull(paradigms);
        ArgumentNullException.ThrowIfNull(pronunciations);
        ArgumentNullException.ThrowIfNull(transcriptions);
        ArgumentNullException.ThrowIfNull(reflexes);
        ArgumentNullException.ThrowIfNull(recordings);
        ArgumentNullException.ThrowIfNull(frequencies);
        ArgumentNullException.ThrowIfNull(revisions);
        _lEntryClerkVault = rig.LRigVault;
        _lEntryClerkEntries = rig.LRigEntries;
        _lEntryClerkEtymologies = rig.LRigEtymologies;
        _lEntryClerkFrequencies = frequencies;
        _lEntryClerkNotes = rig.LRigNotes;
        _lEntryClerkTombstones = rig.LRigTombstones;
        _lEntryClerkCards = cards;
        _lEntryClerkMeanings = meanings;
        _lEntryClerkVocabulary = vocabulary;
        _lEntryClerkInflections = inflections;
        _lEntryClerkParadigms = paradigms;
        _lEntryClerkPronunciations = pronunciations;
        _lEntryClerkTranscriptions = transcriptions;
        _lEntryClerkReflexes = reflexes;
        _lEntryClerkRecordings = recordings;
        _lEntryClerkRevisions = revisions;
    }

    public LEntry? LEntryClerkRead(long id)
    {
        return _lEntryClerkEntries.LEntryRead(id);
    }

    public LEntryDraft? LEntryClerkLoad(long id)
    {
        LEntryDraft? draft = _lEntryClerkEntries.LEntryLoad(id);
        return draft is null
            ? null
            : _lEntryClerkRecordings.LRecordingClerkResolve(draft with
            {
                LEntryDraftReflexes = _lEntryClerkReflexes.LReflexClerkSort(
                    draft.LEntryDraftLanguage, draft.LEntryDraftReflexes),
            });
    }

    public LRevision LEntryClerkDelete(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LVaultSession session = _lEntryClerkVault.LVaultSessionStart();

        LEntry? deleted = _lEntryClerkEntries.LEntryRead(id);
        _lEntryClerkEntries.LEntryDelete(id);

        LRevisionDelta change = new(id, "entry", "delete", deleted?.LEntryHeadword);
        LRevision revision = _lEntryClerkRevisions.LRevisionClerkRecord([change]);
        _lEntryClerkTombstones.LTombstoneRecord(id, revision.LRevisionId);

        session.LVaultSessionCommit();
        return revision;
    }

    public LEntry LEntryClerkSave(LEntryDraft draft, Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(identity);
        LHeadwordValidate(draft);

        using LVaultSession session = _lEntryClerkVault.LVaultSessionStart();

        string language = draft.LEntryDraftLanguage;
        LEntry entry = _lEntryClerkEntries.LEntryCreate(
            new LEntry(0, draft.LEntryDraftHeadword, language, 0, null, null, draft.LEntryDraftUnit),
            forms: [],
            speeches: []);
        List<LRevisionDelta> changes = [new LRevisionDelta(entry.LEntryId, "entry", "create", entry.LEntryHeadword)];

        LCardClerkField.LCardValidate(draft.LEntryDraftMeanings, collocation: false);
        LCardClerkField.LCardValidate(draft.LEntryDraftCollocations, collocation: true);

        if (draft.LEntryDraftMeanings.Count > 0)
        {
            _lEntryClerkMeanings.LMeaningClerkSave(
                entry.LEntryId, draft.LEntryDraftMeanings, language, changes, identity);
        }

        if (draft.LEntryDraftCollocations.Count > 0)
        {
            _lEntryClerkCards.LCollocationSave(
                entry.LEntryId, draft.LEntryDraftCollocations, language, changes, identity);
        }

        _lEntryClerkVocabulary.LSpeechUpdate(entry.LEntryId, draft, changes);
        LEntryClerkField.LFormUpdate(_lEntryClerkEntries, entry.LEntryId, draft, changes);
        if (draft.LEntryDraftInflections.Count > 0)
        {
            _lEntryClerkInflections.LInflectionClerkUpdate(entry.LEntryId, draft, changes);
        }

        if (!string.IsNullOrWhiteSpace(draft.LEntryDraftNote))
        {
            LEntryClerkField.LNoteUpdate(_lEntryClerkNotes, entry.LEntryId, draft, changes);
        }

        _lEntryClerkPronunciations.LPronunciationClerkSync(
            entry.LEntryId, LEntryClerkField.LPronunciationReset(draft.LEntryDraftPronunciations), changes, identity);
        _lEntryClerkTranscriptions.LTranscriptionClerkSync(
            entry.LEntryId,
            LTranscriptionClerk.LTranscriptionClerkReset(draft.LEntryDraftTranscriptions),
            changes,
            identity);
        _lEntryClerkReflexes.LReflexClerkSync(
            entry.LEntryId, language, LReflexClerk.LReflexClerkReset(draft.LEntryDraftReflexes), changes, identity);
        LEntryClerkEtymology.LEtymologyUpdate(_lEntryClerkEtymologies, entry.LEntryId, draft, changes);
        _lEntryClerkParadigms.LParadigmClerkUpdate(entry);

        _lEntryClerkRevisions.LRevisionClerkRecord(changes);

        session.LVaultSessionCommit();
        return entry;
    }

    public LEntry LEntryClerkUpdate(long id, LEntryDraft draft, Dictionary<long, long> identity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(identity);

        using LVaultSession session = _lEntryClerkVault.LVaultSessionStart();

        List<LRevisionDelta> changes = [];
        LEntry updated = LEntryClerkSave(id, draft, identity, changes);
        if (changes.Count > 0)
        {
            _lEntryClerkRevisions.LRevisionClerkRecord(changes);
        }

        session.LVaultSessionCommit();
        return updated;
    }

    public LEntry LEntryClerkSave(
        long id, LEntryDraft draft, Dictionary<long, long> identity, List<LRevisionDelta> changes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(changes);
        LHeadwordValidate(draft);

        draft = draft with { LEntryDraftHeadword = draft.LEntryDraftHeadword.Trim() };

        using LVaultSession session = _lEntryClerkVault.LVaultSessionStart();

        LEntry stored = _lEntryClerkEntries.LEntryRead(id) ?? throw new LRefusal(LRefusal.LRefusalEntry);
        LEntryDraft? origin = LEntryClerkLoad(id);
        bool changed = origin is null || !LDraftClerkEquality.LDraftMatch(origin, draft);
        string language = draft.LEntryDraftLanguage;

        if (changed)
        {
            _lEntryClerkEntries.LEntryUpdate(stored with
            {
                LEntryHeadword = draft.LEntryDraftHeadword,
                LEntryLanguage = language,
                LEntryUnit = draft.LEntryDraftUnit,
            });
        }

        bool renamed =
            !string.Equals(stored.LEntryHeadword, draft.LEntryDraftHeadword, StringComparison.Ordinal) ||
            !string.Equals(stored.LEntryLanguage, language, StringComparison.Ordinal);
        if (renamed)
        {
            _lEntryClerkFrequencies.LFrequencyClerkClear(id);
            changes.Add(new LRevisionDelta(id, "entry", "update", draft.LEntryDraftHeadword));
        }

        _lEntryClerkMeanings.LMeaningClerkSave(id, draft.LEntryDraftMeanings, language, changes, identity);
        _lEntryClerkCards.LCollocationSave(id, draft.LEntryDraftCollocations, language, changes, identity);

        _lEntryClerkVocabulary.LSpeechUpdate(id, draft, changes);
        LEntryClerkField.LFormUpdate(_lEntryClerkEntries, id, draft, changes);
        _lEntryClerkInflections.LInflectionClerkUpdate(id, draft, changes);
        _lEntryClerkInflections.LInflectionClerkReset(id);
        LEntryClerkField.LNoteUpdate(_lEntryClerkNotes, id, draft, changes);
        _lEntryClerkPronunciations.LPronunciationClerkSync(id, draft.LEntryDraftPronunciations, changes, identity);
        _lEntryClerkTranscriptions.LTranscriptionClerkSync(id, draft.LEntryDraftTranscriptions, changes, identity);
        _lEntryClerkReflexes.LReflexClerkSync(id, language, draft.LEntryDraftReflexes, changes, identity);
        LEntryClerkEtymology.LEtymologyUpdate(_lEntryClerkEtymologies, id, draft, changes);

        LEntry updated = _lEntryClerkEntries.LEntryRead(id) ?? stored;
        _lEntryClerkParadigms.LParadigmClerkUpdate(updated);
        session.LVaultSessionCommit();
        return updated;
    }

    private static void LHeadwordValidate(LEntryDraft draft)
    {
        if (string.IsNullOrWhiteSpace(draft.LEntryDraftHeadword))
        {
            throw new LRefusal(LRefusal.LRefusalHeadword);
        }
    }
}
