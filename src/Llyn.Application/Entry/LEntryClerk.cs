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

    public LEntryClerk(
        LRig rig,
        LCardClerk cards,
        LMeaningClerk meanings,
        LVocabularyClerk vocabulary,
        LInflectionClerk inflections,
        LParadigmClerk paradigms,
        LPronunciationClerk pronunciations)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentNullException.ThrowIfNull(meanings);
        ArgumentNullException.ThrowIfNull(vocabulary);
        ArgumentNullException.ThrowIfNull(inflections);
        ArgumentNullException.ThrowIfNull(paradigms);
        ArgumentNullException.ThrowIfNull(pronunciations);
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
        return _lEntryClerkEntries.LEntryLoad(id);
    }

    public IReadOnlyList<LEntry> LEntryClerkFind(string query)
    {
        return _lEntryClerkEntries.LEntryFind(query);
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
            entry.LEntryId, LPronunciationReset(draft.LEntryDraftPronunciations), null, identity);
        _lEntryClerkParadigms.LParadigmClerkUpdate(entry);

        LRevisionRecord([new LRevisionChange(0, entry.LEntryId, "entry", "create", entry.LEntryHeadword)]);

        session.LVaultSessionCommit();
        return entry;
    }

    public LEntry LEntryClerkSave(
        long id, LEntryDraft draft, bool changed, Dictionary<long, long> identity, List<LRevisionChange> changes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(changes);

        using LVaultSession session = _lEntryClerkVault.LVaultSessionStart();

        LEntry stored = _lEntryClerkEntries.LEntryRead(id) ?? throw new LRefusal(LRefusal.LRefusalEntry);
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
        LFormUpdate(_lEntryClerkEntries, id, draft, changes);
        _lEntryClerkInflections.LInflectionClerkUpdate(id, draft, changes);
        _lEntryClerkInflections.LInflectionClerkReset(id);
        LNoteUpdate(id, draft, changes);
        _lEntryClerkPronunciations.LPronunciationClerkSync(id, draft.LEntryDraftPronunciations, changes, identity);

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

    private static IReadOnlyList<LPronunciationDraft> LPronunciationReset(IReadOnlyList<LPronunciationDraft> drafts)
    {
        List<LPronunciationDraft> renewed = new(drafts.Count);
        foreach (LPronunciationDraft draft in drafts)
        {
            renewed.Add(draft.LPronunciationDraftId > 0 ? draft with { LPronunciationDraftId = 0 } : draft);
        }

        return renewed;
    }

    private static void LFormUpdate(LEntryVault entries, long entryId, LEntryDraft draft, List<LRevisionChange> changes)
    {
        IReadOnlyList<LForm> stored = entries.LEntryFormRead(entryId);
        IReadOnlyList<LForm> current = draft.LEntryDraftForms;

        if (LFormMatch(stored, current))
        {
            return;
        }

        entries.LEntryFormSet(entryId, current);
        changes.Add(new LRevisionChange(
            0,
            entryId,
            "form",
            current.Count == 0 ? "delete" : stored.Count == 0 ? "create" : "update",
            null));
    }

    public static bool LFormMatch(IReadOnlyList<LForm> stored, IReadOnlyList<LForm> current)
    {
        ArgumentNullException.ThrowIfNull(stored);
        ArgumentNullException.ThrowIfNull(current);

        if (stored.Count != current.Count)
        {
            return false;
        }

        for (int index = 0; index < stored.Count; index++)
        {
            if (!string.Equals(stored[index].LFormText, current[index].LFormText, StringComparison.Ordinal)
                || !string.Equals(stored[index].LFormRole, current[index].LFormRole, StringComparison.Ordinal)
                || !string.Equals(stored[index].LFormLocal, current[index].LFormLocal, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private void LNoteUpdate(long entryId, LEntryDraft draft, List<LRevisionChange> changes)
    {
        LNoteVault notes = _lEntryClerkNotes;
        LNote? stored = notes.LNoteRead(entryId);
        string text = LMarkdown.LMarkdownNormalize(draft.LEntryDraftNote);

        if (text.Length == 0)
        {
            if (stored is not null)
            {
                notes.LNoteDelete(entryId);
                changes.Add(new LRevisionChange(0, entryId, "note", "delete", null));
            }

            return;
        }

        if (string.Equals(stored?.LNoteText, text, StringComparison.Ordinal))
        {
            return;
        }

        notes.LNoteSave(new LNote(entryId, text));
        changes.Add(new LRevisionChange(0, entryId, "note", stored is null ? "create" : "update", null));
    }
}
