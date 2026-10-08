using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LTranslationClerk
{
    private readonly LVault _lTranslationClerkVault;
    private readonly LEntryVault _lTranslationClerkEntries;
    private readonly LRevisionClerk _lTranslationClerkRevisions;
    private readonly LTranslationVault _lTranslationClerkTranslations;

    public LTranslationClerk(LRig rig, LRevisionClerk revisions)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(revisions);
        _lTranslationClerkVault = rig.LRigVault;
        _lTranslationClerkEntries = rig.LRigEntries;
        _lTranslationClerkRevisions = revisions;
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
        if (LTranslationWordRead(word) is not string written)
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

    public IReadOnlyList<LEntry> LTranslationMentionFind(string word, LDraft? draft)
    {
        ArgumentNullException.ThrowIfNull(word);

        string language = draft?.LDraftMentionLanguage ?? string.Empty;
        List<LEntry> found = [];
        foreach (LEntry entry in LTranslationClerkFind(word, null))
        {
            if (language.Length == 0 || string.Equals(entry.LEntryLanguage, language, StringComparison.Ordinal))
            {
                found.Add(entry);
            }
        }

        return found;
    }

    public static string? LTranslationWordRead(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        string written = text.Trim();
        return written.Length == 0 ? null : written;
    }

    public static IReadOnlyList<string> LTranslationLanguageRead(IReadOnlyList<string> languages, string language)
    {
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(language);

        List<string> offered = [];
        foreach (string held in languages)
        {
            if (!string.Equals(held, language, StringComparison.Ordinal))
            {
                offered.Add(held);
            }
        }

        offered.Add(language);
        return offered;
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

        LRevisionDelta change = new(entry.LEntryId, "entry", "create", entry.LEntryHeadword);
        _lTranslationClerkRevisions.LRevisionClerkRecord([change]);

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
