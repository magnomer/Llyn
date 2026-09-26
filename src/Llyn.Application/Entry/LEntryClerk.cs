using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LEntryClerk
{
    private readonly LVault _lEntryClerkVault;
    private readonly LEntryVault _lEntryClerkEntries;
    private readonly LEtymologyVault _lEntryClerkEtymologies;
    private readonly LFrequencyClerk _lEntryClerkFrequencies;
    private readonly LNoteVault _lEntryClerkNotes;
    private readonly LRevisionVault _lEntryClerkRevisions;
    private readonly LTombstoneVault _lEntryClerkTombstones;
    private readonly LWorkspaceVault _lEntryClerkWorkspaces;
    private readonly LCardClerk _lEntryClerkCards;
    private readonly LMeaningClerk _lEntryClerkMeanings;
    private readonly LVocabularyClerk _lEntryClerkVocabulary;
    private readonly LInflectionClerk _lEntryClerkInflections;
    private readonly LParadigmClerk _lEntryClerkParadigms;
    private readonly LPronunciationClerk _lEntryClerkPronunciations;
    private readonly LTranscriptionClerk _lEntryClerkTranscriptions;
    private readonly LReflexClerk _lEntryClerkReflexes;
    private readonly LRecordingClerk _lEntryClerkRecordings;

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
        LFrequencyClerk frequencies)
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
        _lEntryClerkVault = rig.LRigVault;
        _lEntryClerkEntries = rig.LRigEntries;
        _lEntryClerkEtymologies = rig.LRigEtymologies;
        _lEntryClerkFrequencies = frequencies;
        _lEntryClerkNotes = rig.LRigNotes;
        _lEntryClerkRevisions = rig.LRigRevisions;
        _lEntryClerkTombstones = rig.LRigTombstones;
        _lEntryClerkWorkspaces = rig.LRigWorkspaces;
        _lEntryClerkCards = cards;
        _lEntryClerkMeanings = meanings;
        _lEntryClerkVocabulary = vocabulary;
        _lEntryClerkInflections = inflections;
        _lEntryClerkParadigms = paradigms;
        _lEntryClerkPronunciations = pronunciations;
        _lEntryClerkTranscriptions = transcriptions;
        _lEntryClerkReflexes = reflexes;
        _lEntryClerkRecordings = recordings;
    }

    public LEntry? LEntryClerkRead(long id)
    {
        return _lEntryClerkEntries.LEntryRead(id);
    }

    public LEntryDraft? LEntryClerkLoad(long id)
    {
        LEntryDraft? draft = _lEntryClerkEntries.LEntryLoad(id);
        return draft is null ? null : _lEntryClerkRecordings.LRecordingClerkResolve(draft);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(string query)
    {
        return _lEntryClerkEntries.LEntryFind(query);
    }

    public IReadOnlyList<LEntry> LEntryHeadwordFind(string headword, string language)
    {
        ArgumentNullException.ThrowIfNull(headword);
        ArgumentNullException.ThrowIfNull(language);
        return _lEntryClerkEntries.LEntryHeadwordFind(language.Trim(), headword.Trim());
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(string query, LCatalogOrder order)
    {
        return LCatalogEntry.LCatalogEntrySort(_lEntryClerkEntries.LEntryFind(query), order);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(string query, LCatalogOrder order, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return filter.LCatalogFilterApply(LEntryClerkFind(query, order), entry => entry.LEntryLanguage);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(LTag tag)
    {
        ArgumentNullException.ThrowIfNull(tag);
        return _lEntryClerkEntries.LEntryTagFind(tag.LTagId);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(LRegister register)
    {
        ArgumentNullException.ThrowIfNull(register);
        return _lEntryClerkEntries.LEntryRegisterFind(register.LRegisterId);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(LSituation situation)
    {
        ArgumentNullException.ThrowIfNull(situation);
        return _lEntryClerkEntries.LEntrySituationFind(situation.LSituationId);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(LExample example)
    {
        ArgumentNullException.ThrowIfNull(example);
        return _lEntryClerkEntries.LEntryExampleFind(example.LExampleId);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(LReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        return _lEntryClerkEntries.LEntryReferenceFind(reference.LReferenceId);
    }

    public static IReadOnlyList<LEntry> LEntryClerkMatch(IReadOnlyList<LEntry> entries, string query)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(query);

        string trimmed = query.Trim();
        return trimmed.Length == 0
            ? entries
            : [.. entries.Where(entry => LCatalog.LCatalogTextMatch(entry.LEntryHeadword, trimmed))];
    }

    public LRevision LEntryClerkDelete(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LVaultSession session = _lEntryClerkVault.LVaultSessionStart();

        LEntry? deleted = _lEntryClerkEntries.LEntryRead(id);
        _lEntryClerkEntries.LEntryDelete(id);

        LRevisionDelta change = new(id, "entry", "delete", deleted?.LEntryHeadword);
        LRevision revision = _lEntryClerkRevisions.LRevisionRecord([change]);
        _lEntryClerkTombstones.LTombstoneRecord(id, revision.LRevisionId);
        LWorkspaceState state = _lEntryClerkWorkspaces.LWorkspaceStateRead();
        _lEntryClerkWorkspaces.LWorkspaceStateSave(state with { LWorkspaceStateRevision = revision.LRevisionId });

        session.LVaultSessionCommit();
        return revision;
    }

    public static int LEntryGraspStep => LGrasp.LGraspStep;

    public static string LEntryGraspFormat(int step)
    {
        return LLocalization.LLocalizationTextRead(LGrasp.LGraspKeyRead(step));
    }

    public void LEntryGraspSet(long entryId, int grasp)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        if (!LGrasp.LGraspCheck(grasp))
        {
            throw new ArgumentOutOfRangeException(nameof(grasp));
        }

        _lEntryClerkEntries.LEntryGraspSet(entryId, grasp);
    }

    public string LEntryEpithetRead(long entryId)
    {
        return entryId <= 0 ? string.Empty : _lEntryClerkEntries.LEntryEpithetRead(entryId);
    }

    public IReadOnlyDictionary<long, string> LEntryEpithetScan(IReadOnlyList<long> ids)
    {
        ArgumentNullException.ThrowIfNull(ids);
        return _lEntryClerkEntries.LEntryEpithetScan(ids);
    }

    public long LEntryCountRead()
    {
        return _lEntryClerkEntries.LEntryCountRead();
    }

    public long LWorkspaceSizeRead()
    {
        return _lEntryClerkWorkspaces.LWorkspaceSizeRead();
    }

    public LEntry LEntryClerkSave(LEntryDraft draft, Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(identity);
        LHeadwordValidate(draft);

        using LVaultSession session = _lEntryClerkVault.LVaultSessionStart();

        string language = draft.LEntryDraftLanguage;
        LEntry entry = _lEntryClerkEntries.LEntryCreate(
            new LEntry(0, draft.LEntryDraftHeadword, language, 0, null, null),
            forms: draft.LEntryDraftForms,
            speeches: _lEntryClerkVocabulary.LSpeechResolve(language, draft.LEntryDraftSpeeches));

        if (draft.LEntryDraftInflections.Count > 0)
        {
            _lEntryClerkInflections.LInflectionClerkValidate(draft.LEntryDraftInflections);
            _lEntryClerkInflections.LInflectionClerkSet(entry.LEntryId, draft.LEntryDraftInflections);
        }

        LCardClerkField.LCardValidate(draft.LEntryDraftMeanings, collocation: false);
        LCardClerkField.LCardValidate(draft.LEntryDraftCollocations, collocation: true);

        foreach (LCardDraft card in LCardClerkField.LCardRead(draft.LEntryDraftMeanings))
        {
            _lEntryClerkMeanings.LMeaningClerkCreate(entry.LEntryId, null, card, language, identity);
        }

        foreach (LCardDraft card in LCardClerkField.LCardRead(draft.LEntryDraftCollocations))
        {
            long rowId = _lEntryClerkCards.LCollocationInsert(entry.LEntryId, card, identity);
            _lEntryClerkCards.LCardClerkSync(rowId, card, language, true, identity);
        }

        string note = LMarkdown.LMarkdownNormalize(draft.LEntryDraftNote);
        if (note.Length > 0)
        {
            _lEntryClerkNotes.LNoteSave(new LNote(entry.LEntryId, note));
        }

        _lEntryClerkPronunciations.LPronunciationClerkSync(
            entry.LEntryId, LEntryClerkField.LPronunciationReset(draft.LEntryDraftPronunciations), null, identity);
        _lEntryClerkTranscriptions.LTranscriptionClerkSync(
            entry.LEntryId,
            LTranscriptionClerk.LTranscriptionClerkReset(draft.LEntryDraftTranscriptions),
            null,
            identity);
        _lEntryClerkReflexes.LReflexClerkSync(
            entry.LEntryId, language, LReflexClerk.LReflexClerkReset(draft.LEntryDraftReflexes), null, identity);
        LEntryClerkEtymology.LEtymologyUpdate(_lEntryClerkEtymologies, entry.LEntryId, draft, null);
        _lEntryClerkParadigms.LParadigmClerkUpdate(entry);

        LRevisionRecord([new LRevisionDelta(entry.LEntryId, "entry", "create", entry.LEntryHeadword)]);

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
            LRevisionRecord(changes);
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

    public LRevision LRevisionRecord(IReadOnlyList<LRevisionDelta> changes)
    {
        ArgumentNullException.ThrowIfNull(changes);

        using LVaultSession session = _lEntryClerkVault.LVaultSessionStart();

        LRevision revision = _lEntryClerkRevisions.LRevisionRecord(changes);
        LWorkspaceState state = _lEntryClerkWorkspaces.LWorkspaceStateRead();
        _lEntryClerkWorkspaces.LWorkspaceStateSave(state with { LWorkspaceStateRevision = revision.LRevisionId });

        session.LVaultSessionCommit();
        return revision;
    }

    private static void LHeadwordValidate(LEntryDraft draft)
    {
        if (string.IsNullOrWhiteSpace(draft.LEntryDraftHeadword))
        {
            throw new LRefusal(LRefusal.LRefusalHeadword);
        }
    }
}
