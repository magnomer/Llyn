using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public const long LEngineMarkupCeiling = 64L * 1024 * 1024;

    public LMarkupCargo LEngineMarkupRead(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        IReadOnlyList<LMarkupEntry> parsed = LMarkup.LMarkupParse(
            LEngineMarkupLoad(path), out IReadOnlyList<LMarkupOmission> skipped);

        List<LMarkupOmission> omissions = [.. skipped];
        List<LMarkupEntry> entries = new(parsed.Count);
        foreach (LMarkupEntry entry in parsed)
        {
            string language = entry.LMarkupEntryLanguage;
            if (language.Length == 0 || LLanguageLoader.LLanguageNameValidate(language))
            {
                entries.Add(entry);
                continue;
            }

            omissions.Add(new LMarkupOmission(entry.LMarkupEntryLine, $"language \"{language}\""));
            entries.Add(entry with { LMarkupEntryLanguage = string.Empty });
        }

        string[] names = LEngineTwinRead(entries, entry => entry.LMarkupEntryHeadword, entry => entry.LMarkupEntryLine);
        for (int index = 0; index < entries.Count; index++)
        {
            entries[index] = entries[index] with { LMarkupEntryName = names[index] };
        }

        return new LMarkupCargo(entries, omissions);
    }

    public Task<LMarkupCargo> LEngineMarkupStart(string path)
    {
        return Task.Run(() => LEngineMarkupRead(path));
    }

    private static string LEngineMarkupLoad(string path)
    {
        using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (stream.Length > LEngineMarkupCeiling)
        {
            throw new LRefusal(LRefusal.LRefusalMarkup);
        }

        using StreamReader reader = new(stream, Encoding.UTF8, true);
        return reader.ReadToEnd();
    }

    public IReadOnlyList<LEntry> LEngineMarkupFind(LMarkupEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        lock (_lEngineGate)
        {
            return LEngineMarkupFind(entry.LMarkupEntryHeadword, entry.LMarkupEntryLanguage);
        }
    }

    public Task<LMarkupOutcome> LEngineMarkupStart(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)
    {
        return Task.Run(() => LEngineMarkupImport(cargo, intakes));
    }

    public LMarkupOutcome LEngineMarkupImport(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)
    {
        ArgumentNullException.ThrowIfNull(cargo);
        ArgumentNullException.ThrowIfNull(intakes);

        IReadOnlyList<LMarkupEntry> entries = cargo.LMarkupCargoEntry;
        if (intakes.Count != entries.Count)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LMarkupIntake?[] ordered = new LMarkupIntake?[entries.Count];
        foreach (LMarkupIntake intake in intakes)
        {
            if (intake.LMarkupIntakeIndex < 0
                || intake.LMarkupIntakeIndex >= ordered.Length
                || ordered[intake.LMarkupIntakeIndex] is not null)
            {
                throw new LRefusal(LRefusal.LRefusalItem);
            }

            ordered[intake.LMarkupIntakeIndex] = intake;
        }

        List<LMarkupOmission> omissions = [.. cargo.LMarkupCargoOmission];
        List<LEntry> stored = new(entries.Count);
        lock (_lEngineGate)
        {
            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

            LEngineMarkupValidate(ordered!);
            IReadOnlyDictionary<int, long> prepared = LEngineMarkupPrepare(entries, ordered!);

            Dictionary<(string, string), long> named = [];
            for (int index = 0; index < entries.Count; index++)
            {
                (string, string) key = (
                    LCatalog.LCatalogTextNormalize(entries[index].LMarkupEntryHeadword),
                    LCatalog.LCatalogTextNormalize(entries[index].LMarkupEntryLanguage));
                named[key] = named.ContainsKey(key) ? 0 : prepared[index];
            }

            LEntryArchive archive = new(_lEngineDatabase);
            List<LRevisionChange> changes = [];
            List<(LMarkupMention, int)> held = [];
            Dictionary<long, long> identity = [];
            for (int index = 0; index < entries.Count; index++)
            {
                long id = prepared[index];
                LMarkupMode mode = ordered[index]!.LMarkupIntakeMode;
                if (mode == LMarkupMode.LMarkupModeReplace)
                {
                    LEngineMarkupDetach(id, omissions);
                }

                LMarkupEntry entry = entries[index];
                if (mode != LMarkupMode.LMarkupModeNew && string.IsNullOrWhiteSpace(entry.LMarkupEntryLanguage))
                {
                    entry = entry with
                    {
                        LMarkupEntryLanguage = archive.LEntryRead(id)?.LEntryLanguage ?? string.Empty,
                    };
                }

                int noted = omissions.Count;
                LEntryDraft draft = LEngineMarkupResolve(entry, named, omissions, held);
                LEngineLineSet(omissions, noted, entry.LMarkupEntryLine);

                if (mode == LMarkupMode.LMarkupModeMerge)
                {
                    draft = LEngineMarkupAppend(
                        LEngineEntryLoad(id) ?? throw new LRefusal(LRefusal.LRefusalEntry), draft);
                }

                changes.Add(new LRevisionChange(
                    0,
                    id,
                    "entry",
                    mode == LMarkupMode.LMarkupModeNew ? "create" : "update",
                    draft.LEntryDraftHeadword));
                stored.Add(LEngineEntryUpdate(id, draft, identity, changes));
            }

            LEngineMarkupSettle(held, identity, named, omissions);

            LRevision revision = new LRevisionArchive(_lEngineDatabase).LRevisionRecord(changes);

            LWorkspaceArchive workspace = new(_lEngineDatabase);
            LWorkspaceState state = workspace.LWorkspaceStateRead();
            workspace.LWorkspaceStateSave(state with { LWorkspaceStateRevision = revision.LRevisionId });

            session.LDatabaseSessionCommit();
        }

        foreach (LEntry entry in stored)
        {
            LEngineFrequencyStart(entry.LEntryId);
        }

        return new LMarkupOutcome(stored, omissions);
    }

    private static void LEngineLineSet(List<LMarkupOmission> omissions, int noted, int line)
    {
        for (int index = noted; index < omissions.Count; index++)
        {
            if (omissions[index].LMarkupOmissionLine == 0)
            {
                omissions[index] = omissions[index] with { LMarkupOmissionLine = line };
            }
        }
    }

    private void LEngineMarkupValidate(IReadOnlyList<LMarkupIntake> intakes)
    {
        LEntryArchive archive = new(_lEngineDatabase);
        IReadOnlyList<LDraft>? drafts = null;
        HashSet<long> targets = [];
        foreach (LMarkupIntake intake in intakes)
        {
            if (intake.LMarkupIntakeMode == LMarkupMode.LMarkupModeNew)
            {
                continue;
            }

            long target = intake.LMarkupIntakeTarget;
            if (target <= 0 || archive.LEntryRead(target) is null)
            {
                throw new LRefusal(LRefusal.LRefusalEntry);
            }

            if (!targets.Add(target))
            {
                throw new LRefusal(LRefusal.LRefusalItem);
            }

            drafts ??= LDraftArchive.LDraftArchiveScan(_lEngineWorkspace);
            foreach (LDraft draft in drafts)
            {
                if (draft.LDraftEntryId == target
                    && draft.LDraftExample is null
                    && draft.LDraftSituation is null
                    && draft.LDraftReference is null
                    && draft.LDraftAuthorHeld is null
                    && _lEngineDraftHeld.Contains(draft.LDraftId))
                {
                    throw new LRefusal(LRefusal.LRefusalStale);
                }
            }
        }
    }

    private IReadOnlyDictionary<int, long> LEngineMarkupPrepare(
        IReadOnlyList<LMarkupEntry> entries, IReadOnlyList<LMarkupIntake> intakes)
    {
        LEntryArchive archive = new(_lEngineDatabase);
        Dictionary<int, long> prepared = new(entries.Count);
        foreach (LMarkupIntake intake in intakes)
        {
            LMarkupEntry entry = entries[intake.LMarkupIntakeIndex];
            if (intake.LMarkupIntakeMode != LMarkupMode.LMarkupModeNew)
            {
                prepared[intake.LMarkupIntakeIndex] = intake.LMarkupIntakeTarget;
                continue;
            }

            if (string.IsNullOrWhiteSpace(entry.LMarkupEntryHeadword))
            {
                throw new LRefusal(LRefusal.LRefusalHeadword);
            }

            LEntry created = archive.LEntryCreate(
                new LEntry(0, entry.LMarkupEntryHeadword, entry.LMarkupEntryLanguage, 0, null, null),
                [],
                []);
            prepared[intake.LMarkupIntakeIndex] = created.LEntryId;
        }

        return prepared;
    }
}
