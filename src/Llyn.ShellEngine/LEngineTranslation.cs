using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LTranslation> LEngineTranslationRead(string ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LTranslationArchive translations = new(_lEngineDatabase);
            return LEngineOwnerCheck(owner)
                ? translations.LTranslationCollocationRead(ownerId)
                : translations.LTranslationMeaningRead(ownerId);
        }
    }

    public void LEngineTranslationSave(string ownerId, IReadOnlyList<string> ids, LOwner owner)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(ids);
            LEngineTranslationSave(ownerId, ids, LEngineOwnerCheck(owner));
        }
    }

    public IReadOnlyList<LEntry> LEngineTranslationFind(string query, string? entryId)
    {
        lock (_lEngineGate)
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
    }

    public LEntry? LEngineTranslationResolve(string word, string? entryId)
    {
        lock (_lEngineGate)
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
    }

    public LEntry LEngineTranslationCreate(string headword, string language)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(headword);
            ArgumentException.ThrowIfNullOrWhiteSpace(language);

            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

            LEntry entry = new LEntryArchive(_lEngineDatabase).LEntryCreate(
                new LEntry(string.Empty, headword.Trim(), language, null, null, null, null),
                forms: [],
                speeches: []);

            LRevisionChange change = new(0, entry.LEntryId, "entry", "create", entry.LEntryHeadword);
            LRevision revision = new LRevisionArchive(_lEngineDatabase).LRevisionRecord([change]);

            LWorkspaceArchive workspace = new(_lEngineDatabase);
            LWorkspaceState state = workspace.LWorkspaceStateRead();
            workspace.LWorkspaceStateSave(state with { LWorkspaceStateRevision = revision.LRevisionId });

            session.LDatabaseSessionCommit();
            return entry;
        }
    }

    public void LEngineTranslationDelete(string id)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);

            if (new LTranslationArchive(_lEngineDatabase).LTranslationIncomingRead(id).Count > 0)
            {
                return;
            }

            LEngineEntryDelete(id);
        }
    }

    public IReadOnlyList<LTranslationTarget> LEngineTargetRead(IReadOnlyList<string> ids)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(ids);
            return new LTranslationArchive(_lEngineDatabase).LTranslationTargetRead(ids);
        }
    }

    public IReadOnlyList<LUsage> LEngineIncomingRead(string entryId)
    {
        lock (_lEngineGate)
        {
            return new LTranslationArchive(_lEngineDatabase).LTranslationIncomingRead(entryId);
        }
    }
}
