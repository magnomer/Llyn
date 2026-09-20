using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LEntryClerk
{
    private readonly LVault _lEntryClerkVault;
    private readonly LEntryVault _lEntryClerkEntries;
    private readonly LFrequencyVault _lEntryClerkFrequencies;
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
        LRecordingClerk recordings)
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
        _lEntryClerkVault = rig.LRigVault;
        _lEntryClerkEntries = rig.LRigEntries;
        _lEntryClerkFrequencies = rig.LRigFrequencies;
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

    public LEntry LEntryClerkCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)
    {
        ArgumentNullException.ThrowIfNull(entry);
        return _lEntryClerkEntries.LEntryCreate(entry, forms, speeches);
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

    public IReadOnlyList<LEntry> LEntryClerkFind(LTag tag, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return filter.LCatalogFilterApply(LEntryClerkFind(tag), entry => entry.LEntryLanguage);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(LTag tag, string query, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(query);
        return LEntryClerkMatch(LEntryClerkFind(tag, filter), query);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(LRegister register)
    {
        ArgumentNullException.ThrowIfNull(register);
        return _lEntryClerkEntries.LEntryRegisterFind(register.LRegisterId);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(LRegister register, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return filter.LCatalogFilterApply(LEntryClerkFind(register), entry => entry.LEntryLanguage);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(LRegister register, string query, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(query);
        return LEntryClerkMatch(LEntryClerkFind(register, filter), query);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(LSituation situation)
    {
        ArgumentNullException.ThrowIfNull(situation);
        return _lEntryClerkEntries.LEntrySituationFind(situation.LSituationId);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(LSituation situation, string query, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(filter);
        return LEntryClerkMatch(
            filter.LCatalogFilterApply(LEntryClerkFind(situation), entry => entry.LEntryLanguage),
            query);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(LExample example)
    {
        ArgumentNullException.ThrowIfNull(example);
        return _lEntryClerkEntries.LEntryExampleFind(example.LExampleId);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(LExample example, string query, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(filter);
        return LEntryClerkMatch(
            filter.LCatalogFilterApply(LEntryClerkFind(example), entry => entry.LEntryLanguage),
            query);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(LReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        return _lEntryClerkEntries.LEntryReferenceFind(reference.LReferenceId);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(LReference reference, string query, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(filter);
        return LEntryClerkMatch(
            filter.LCatalogFilterApply(LEntryClerkFind(reference), entry => entry.LEntryLanguage),
            query);
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

        LRevisionChange change = new(0, id, "entry", "delete", deleted?.LEntryHeadword);
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

    public void LEntryEpithetSave(long entryId, string epithet)
    {
        ArgumentNullException.ThrowIfNull(epithet);
        _lEntryClerkEntries.LEntryEpithetSave(entryId, epithet);
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

    public LTombstone? LTombstoneRead(long entryId)
    {
        return _lEntryClerkTombstones.LTombstoneRead(entryId);
    }

    public LRevision? LRevisionRead()
    {
        long? id = _lEntryClerkWorkspaces.LWorkspaceStateRead().LWorkspaceStateRevision;
        return id is null ? null : _lEntryClerkRevisions.LRevisionRead(id.Value);
    }

    public IReadOnlyList<LRevisionChange> LRevisionChangeRead(long revisionId)
    {
        return _lEntryClerkRevisions.LRevisionChangeRead(revisionId);
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
            speeches: _lEntryClerkVocabulary.LSpeechResolve(0, language, draft.LEntryDraftSpeeches));

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
        _lEntryClerkParadigms.LParadigmClerkUpdate(entry);

        LRevisionRecord([new LRevisionChange(0, entry.LEntryId, "entry", "create", entry.LEntryHeadword)]);

        session.LVaultSessionCommit();
        return entry;
    }

    public LEntry LEntryClerkUpdate(long id, LEntryDraft draft, Dictionary<long, long> identity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(identity);

        using LVaultSession session = _lEntryClerkVault.LVaultSessionStart();

        List<LRevisionChange> changes = [];
        LEntry updated = LEntryClerkSave(id, draft, identity, changes);
        if (changes.Count > 0)
        {
            LRevisionRecord(changes);
        }

        session.LVaultSessionCommit();
        return updated;
    }

    public LEntry LEntryClerkSave(
        long id, LEntryDraft draft, Dictionary<long, long> identity, List<LRevisionChange> changes)
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
            _lEntryClerkFrequencies.LFrequencyClear(id);
            changes.Add(new LRevisionChange(0, id, "entry", "update", draft.LEntryDraftHeadword));
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

        LEntry updated = _lEntryClerkEntries.LEntryRead(id) ?? stored;
        _lEntryClerkParadigms.LParadigmClerkUpdate(updated);
        session.LVaultSessionCommit();
        return updated;
    }

    public LRevision LRevisionRecord(IReadOnlyList<LRevisionChange> changes)
    {
        ArgumentNullException.ThrowIfNull(changes);

        using LVaultSession session = _lEntryClerkVault.LVaultSessionStart();

        LRevision revision = _lEntryClerkRevisions.LRevisionRecord(changes);
        LWorkspaceState state = _lEntryClerkWorkspaces.LWorkspaceStateRead();
        _lEntryClerkWorkspaces.LWorkspaceStateSave(state with { LWorkspaceStateRevision = revision.LRevisionId });

        session.LVaultSessionCommit();
        return revision;
    }

    public void LEntryUpdatedSet(long entryId)
    {
        _lEntryClerkEntries.LEntryUpdatedSet(entryId);
    }

    private static void LHeadwordValidate(LEntryDraft draft)
    {
        if (string.IsNullOrWhiteSpace(draft.LEntryDraftHeadword))
        {
            throw new LRefusal(LRefusal.LRefusalHeadword);
        }
    }
}
