using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LEntryClerk
{
    private readonly LVault _lEntryClerkVault;
    private readonly LEntryVault _lEntryClerkEntries;
    private readonly LRevisionVault _lEntryClerkRevisions;
    private readonly LTombstoneVault _lEntryClerkTombstones;
    private readonly LWorkspaceVault _lEntryClerkWorkspaces;

    public LEntryClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lEntryClerkVault = rig.LRigVault;
        _lEntryClerkEntries = rig.LRigEntries;
        _lEntryClerkRevisions = rig.LRigRevisions;
        _lEntryClerkTombstones = rig.LRigTombstones;
        _lEntryClerkWorkspaces = rig.LRigWorkspaces;
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
}
