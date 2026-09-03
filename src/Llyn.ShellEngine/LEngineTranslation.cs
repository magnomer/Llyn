using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LTranslation> LEngineTranslationRead(string ownerId, LOwner owner)
    {
        LTranslationArchive translations = new(_lEngineDatabase);
        return LEngineOwnerCheck(owner)
            ? translations.LTranslationCollocationRead(ownerId)
            : translations.LTranslationSenseRead(ownerId);
    }

    public void LEngineTranslationSave(string ownerId, IReadOnlyList<string> ids, LOwner owner)
    {
        ArgumentNullException.ThrowIfNull(ids);
        LEngineTranslationSave(ownerId, ids, LEngineOwnerCheck(owner));
    }

    public IReadOnlyList<LEntry> LEngineTranslationFind(string query, string? entryId)
    {
        ArgumentNullException.ThrowIfNull(query);

        IReadOnlyList<LEntry> found = new LEntryArchive(_lEngineDatabase).LEntryFind(query);
        if (string.IsNullOrWhiteSpace(entryId))
        {
            return found;
        }

        List<LEntry> kept = new(found.Count);
        foreach (LEntry entry in found)
        {
            if (!string.Equals(entry.LEntryId, entryId, StringComparison.Ordinal))
            {
                kept.Add(entry);
            }
        }

        return kept;
    }

    public LEntry? LEngineTranslationResolve(string word, string? entryId)
    {
        ArgumentNullException.ThrowIfNull(word);

        string written = word.Trim();
        if (written.Length == 0)
        {
            return null;
        }

        LEntry? single = null;
        foreach (LEntry entry in LEngineTranslationFind(written, entryId))
        {
            if (!string.Equals(
                    entry.LEntryHeadword.Trim(), written, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            if (single is not null)
            {
                return null;
            }

            single = entry;
        }

        return single;
    }

    public LEntry LEngineTranslationCreate(string headword, string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(headword);
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

        LEntry entry = new LEntryArchive(_lEngineDatabase).LEntryCreate(
            new LEntry(string.Empty, headword.Trim(), language, null, null, null, null),
            forms: [],
            speeches: []);

        LRevisionChange change = new(0, entry.LEntryId, "entry", "create", entry.LEntryHeadword);
        new LRevisionArchive(_lEngineDatabase).LRevisionRecord([change]);

        session.LDatabaseSessionCommit();
        return entry;
    }

    public void LEngineTranslationDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        if (new LTranslationArchive(_lEngineDatabase).LTranslationIncomingRead(id).Count > 0)
        {
            return;
        }

        LEngineEntryDelete(id);
    }

    public IReadOnlyList<LTranslationTarget> LEngineTargetRead(IReadOnlyList<string> ids)
    {
        ArgumentNullException.ThrowIfNull(ids);
        return new LTranslationArchive(_lEngineDatabase).LTranslationTargetRead(ids);
    }

    public IReadOnlyList<LUsage> LEngineIncomingRead(string entryId)
    {
        return new LTranslationArchive(_lEngineDatabase).LTranslationIncomingRead(entryId);
    }
}
