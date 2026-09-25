using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LTranslationClerk
{
    private readonly LVault _lTranslationClerkVault;
    private readonly LEntryVault _lTranslationClerkEntries;
    private readonly LRevisionVault _lTranslationClerkRevisions;
    private readonly LWorkspaceVault _lTranslationClerkWorkspaces;
    private readonly LTranslationVault _lTranslationClerkTranslations;

    public LTranslationClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lTranslationClerkVault = rig.LRigVault;
        _lTranslationClerkEntries = rig.LRigEntries;
        _lTranslationClerkRevisions = rig.LRigRevisions;
        _lTranslationClerkWorkspaces = rig.LRigWorkspaces;
        _lTranslationClerkTranslations = rig.LRigTranslations;
    }

    public IReadOnlyList<LTranslation> LTranslationClerkRead(long ownerId, bool collocation)
    {
        return collocation
            ? _lTranslationClerkTranslations.LTranslationCollocationRead(ownerId)
            : _lTranslationClerkTranslations.LTranslationMeaningRead(ownerId);
    }

    public void LTranslationClerkSave(long ownerId, IReadOnlyList<long> ids, bool collocation)
    {
        ArgumentNullException.ThrowIfNull(ids);

        List<LTranslation> written = new(ids.Count);
        foreach (long id in ids)
        {
            if (id > 0 && _lTranslationClerkEntries.LEntryRead(id) is not null)
            {
                written.Add(new LTranslation(id));
            }
        }

        if (collocation)
        {
            _lTranslationClerkTranslations.LTranslationCollocationSave(ownerId, written);
            return;
        }

        _lTranslationClerkTranslations.LTranslationMeaningSave(ownerId, written);
    }

    public IReadOnlyList<LEntry> LTranslationClerkFind(string query, long? entryId)
    {
        ArgumentNullException.ThrowIfNull(query);

        string written = LCatalog.LCatalogTextNormalize(query);
        List<LEntry> exact = [];
        List<LEntry> partial = [];
        foreach (LEntry entry in _lTranslationClerkEntries.LEntryFind(query))
        {
            if (entryId > 0 && entry.LEntryId == entryId)
            {
                continue;
            }

            if (string.Equals(
                LCatalog.LCatalogTextNormalize(entry.LEntryHeadword), written, StringComparison.Ordinal))
            {
                exact.Add(entry);
            }
            else
            {
                partial.Add(entry);
            }
        }

        exact.AddRange(partial);
        return exact;
    }

    public LEntry? LTranslationClerkResolve(string word, long? entryId)
    {
        ArgumentNullException.ThrowIfNull(word);

        string written = word.Trim();
        if (written.Length == 0)
        {
            return null;
        }

        string folded = LCatalog.LCatalogTextNormalize(written);
        LEntry? single = null;
        foreach (LEntry entry in LTranslationClerkFind(written, entryId))
        {
            if (!string.Equals(
                LCatalog.LCatalogTextNormalize(entry.LEntryHeadword), folded, StringComparison.Ordinal))
            {
                break;
            }

            if (single is not null)
            {
                return null;
            }

            single = entry;
        }

        return single;
    }

    public LEntry LTranslationClerkCreate(string headword, string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(headword);
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        using LVaultSession session = _lTranslationClerkVault.LVaultSessionStart();

        LEntry entry = _lTranslationClerkEntries.LEntryCreate(
            new LEntry(0, headword.Trim(), language, 0, null, null),
            forms: [],
            speeches: []);

        LRevisionChange change = new(entry.LEntryId, "entry", "create", entry.LEntryHeadword);
        LRevision revision = _lTranslationClerkRevisions.LRevisionRecord([change]);
        LWorkspaceState state = _lTranslationClerkWorkspaces.LWorkspaceStateRead();
        _lTranslationClerkWorkspaces.LWorkspaceStateSave(state with { LWorkspaceStateRevision = revision.LRevisionId });

        session.LVaultSessionCommit();
        return entry;
    }

    public IReadOnlyList<LTranslationTarget> LTranslationTargetRead(IReadOnlyList<long> ids)
    {
        ArgumentNullException.ThrowIfNull(ids);
        return _lTranslationClerkTranslations.LTranslationTargetRead(ids);
    }

    public IReadOnlyList<LTranslationTarget> LTranslationTargetRead(
        long ownerId, IReadOnlyList<long> ids, IReadOnlyList<LCourt> links)
    {
        ArgumentNullException.ThrowIfNull(ids);
        ArgumentNullException.ThrowIfNull(links);
        ArgumentOutOfRangeException.ThrowIfZero(ownerId);

        List<LTranslationTarget> targets = new(
            _lTranslationClerkTranslations.LTranslationTargetRead(ids));
        HashSet<long> wanted = [.. ids];
        foreach (LTranslationTarget target in targets)
        {
            wanted.Remove(target.LTranslationTargetId);
        }

        foreach (LCourt link in links)
        {
            if (wanted.Remove(link.LCourtTargetId))
            {
                targets.Add(new LTranslationTarget(
                    link.LCourtTargetId, link.LCourtHeadword, link.LCourtLanguage));
            }
        }

        return targets;
    }

    public IReadOnlyList<LUsage> LTranslationIncomingRead(long entryId, bool epithet)
    {
        return LUsageClerk.LUsageClerkResolve(
            _lTranslationClerkTranslations.LTranslationIncomingRead(entryId),
            _lTranslationClerkEntries,
            epithet);
    }
}
