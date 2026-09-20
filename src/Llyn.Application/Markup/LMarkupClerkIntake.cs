using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LMarkupClerkIntake
{
    private readonly LVault _lMarkupIntakeVault;
    private readonly LEntryVault _lMarkupIntakeRows;
    private readonly LMeaningVault _lMarkupIntakeMeanings;
    private readonly LMentionVault _lMarkupIntakeMentions;
    private readonly LClaimClerk _lMarkupIntakeClaims;
    private readonly LEntryClerk _lMarkupIntakeEntries;
    private readonly LLacunaClerk _lMarkupIntakeLacunae;
    private readonly LFrequencyClerk _lMarkupIntakeFrequencies;
    private readonly LMarkupClerkLink _lMarkupIntakeLink;
    private readonly LMarkupClerkDraft _lMarkupIntakeDraft;

    public LMarkupClerkIntake(
        LRig rig,
        LClaimClerk claims,
        LEntryClerk entries,
        LLacunaClerk lacunae,
        LFrequencyClerk frequencies,
        LMarkupClerkLink link,
        LMarkupClerkDraft draft)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(lacunae);
        ArgumentNullException.ThrowIfNull(frequencies);
        ArgumentNullException.ThrowIfNull(link);
        ArgumentNullException.ThrowIfNull(draft);
        _lMarkupIntakeVault = rig.LRigVault;
        _lMarkupIntakeRows = rig.LRigEntries;
        _lMarkupIntakeMeanings = rig.LRigMeanings;
        _lMarkupIntakeMentions = rig.LRigMentions;
        _lMarkupIntakeClaims = claims;
        _lMarkupIntakeEntries = entries;
        _lMarkupIntakeLacunae = lacunae;
        _lMarkupIntakeFrequencies = frequencies;
        _lMarkupIntakeLink = link;
        _lMarkupIntakeDraft = draft;
    }

    public LMarkupOutcome LMarkupClerkImport(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)
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
        using (LVaultSession session = _lMarkupIntakeVault.LVaultSessionStart())
        {
            LMarkupIntakeValidate(ordered!);
            IReadOnlyDictionary<int, long> prepared = LMarkupIntakePrepare(entries, ordered!);

            Dictionary<(string, string), long> named = [];
            for (int index = 0; index < entries.Count; index++)
            {
                (string, string) key = (
                    LCatalog.LCatalogTextNormalize(entries[index].LMarkupEntryHeadword),
                    LCatalog.LCatalogTextNormalize(entries[index].LMarkupEntryLanguage));
                named[key] = named.ContainsKey(key) ? 0 : prepared[index];
            }

            List<LRevisionChange> changes = [];
            List<(LMarkupMention, int)> held = [];
            Dictionary<long, long> identity = [];
            for (int index = 0; index < entries.Count; index++)
            {
                long id = prepared[index];
                LMarkupMode mode = ordered[index]!.LMarkupIntakeMode;
                if (mode == LMarkupMode.LMarkupModeReplace)
                {
                    LMarkupSenseDetach(id, omissions);
                }

                LMarkupEntry entry = entries[index];
                if (mode != LMarkupMode.LMarkupModeNew && string.IsNullOrWhiteSpace(entry.LMarkupEntryLanguage))
                {
                    entry = entry with
                    {
                        LMarkupEntryLanguage = _lMarkupIntakeRows.LEntryRead(id)?.LEntryLanguage ?? string.Empty,
                    };
                }

                int noted = omissions.Count;
                LEntryDraft draft = _lMarkupIntakeDraft.LMarkupDraftResolve(entry, named, omissions, held);
                LMarkupLineSet(omissions, noted, entry.LMarkupEntryLine);

                if (mode == LMarkupMode.LMarkupModeMerge)
                {
                    draft = LMarkupClerkUnion.LMarkupUnionRead(
                        _lMarkupIntakeEntries.LEntryClerkLoad(id) ?? throw new LRefusal(LRefusal.LRefusalEntry),
                        draft);
                }

                changes.Add(new LRevisionChange(
                    0,
                    id,
                    "entry",
                    mode == LMarkupMode.LMarkupModeNew ? "create" : "update",
                    draft.LEntryDraftHeadword));
                _lMarkupIntakeLacunae.LLacunaClerkCancel(id);
                stored.Add(_lMarkupIntakeEntries.LEntryClerkSave(id, draft, identity, changes));
            }

            LMarkupMentionSettle(held, identity, named, omissions);
            _lMarkupIntakeEntries.LRevisionRecord(changes);

            session.LVaultSessionCommit();
        }

        foreach (LEntry entry in stored)
        {
            _lMarkupIntakeFrequencies.LFrequencyClerkStart(entry.LEntryId);
        }

        return new LMarkupOutcome(stored, omissions);
    }

    private static void LMarkupLineSet(List<LMarkupOmission> omissions, int noted, int line)
    {
        for (int index = noted; index < omissions.Count; index++)
        {
            if (omissions[index].LMarkupOmissionLine == 0)
            {
                omissions[index] = omissions[index] with { LMarkupOmissionLine = line };
            }
        }
    }

    private void LMarkupIntakeValidate(IReadOnlyList<LMarkupIntake> intakes)
    {
        IReadOnlyList<LDraft>? drafts = null;
        HashSet<long> targets = [];
        foreach (LMarkupIntake intake in intakes)
        {
            if (intake.LMarkupIntakeMode == LMarkupMode.LMarkupModeNew)
            {
                continue;
            }

            long target = intake.LMarkupIntakeTarget;
            if (target <= 0 || _lMarkupIntakeRows.LEntryRead(target) is null)
            {
                throw new LRefusal(LRefusal.LRefusalEntry);
            }

            if (!targets.Add(target))
            {
                throw new LRefusal(LRefusal.LRefusalItem);
            }

            drafts ??= _lMarkupIntakeClaims.LDraftScan();
            foreach (LDraft draft in drafts)
            {
                if (draft.LDraftEntryId == target
                    && draft.LDraftExample is null
                    && draft.LDraftSituation is null
                    && draft.LDraftReference is null
                    && draft.LDraftAuthorHeld is null
                    && _lMarkupIntakeClaims.LClaimClerkHeld.Contains(draft.LDraftId))
                {
                    throw new LRefusal(LRefusal.LRefusalStale);
                }
            }
        }
    }

    private IReadOnlyDictionary<int, long> LMarkupIntakePrepare(
        IReadOnlyList<LMarkupEntry> entries, IReadOnlyList<LMarkupIntake> intakes)
    {
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

            LEntry created = _lMarkupIntakeRows.LEntryCreate(
                new LEntry(0, entry.LMarkupEntryHeadword, entry.LMarkupEntryLanguage, 0, null, null),
                [],
                []);
            prepared[intake.LMarkupIntakeIndex] = created.LEntryId;
        }

        return prepared;
    }

    private void LMarkupSenseDetach(long entryId, List<LMarkupOmission> omissions)
    {
        LMentionVault mentions = _lMarkupIntakeMentions;
        HashSet<long> affected = [];
        foreach (LMeaning meaning in _lMarkupIntakeMeanings.LMeaningRead(entryId))
        {
            foreach (long exampleId in mentions.LMentionSenseClear(meaning.LMeaningId))
            {
                if (affected.Add(exampleId))
                {
                    omissions.Add(new LMarkupOmission(0, $"sense of example {exampleId}"));
                }
            }
        }
    }

    private void LMarkupMentionSettle(
        IReadOnlyList<(LMarkupMention, int)> held,
        IReadOnlyDictionary<long, long> identity,
        IReadOnlyDictionary<(string, string), long> prepared,
        List<LMarkupOmission> omissions)
    {
        LMentionVault mentions = _lMarkupIntakeMentions;
        LMarkupClerkLink link = _lMarkupIntakeLink;
        for (int index = 0; index < held.Count; index++)
        {
            if (!identity.TryGetValue(-(index + 1), out long mentionId))
            {
                continue;
            }

            (LMarkupMention mention, int line) = held[index];
            long target = link.LMarkupEntryResolve(
                mention.LMarkupMentionHeadword, mention.LMarkupMentionLanguage, prepared);
            long sense = target == 0 ? 0 : link.LMarkupSenseResolve(target, mention.LMarkupMentionSense);
            if (sense == 0)
            {
                omissions.Add(new LMarkupOmission(
                    line, $"sense \"{mention.LMarkupMentionSense}\" of \"{mention.LMarkupMentionHeadword}\""));
                continue;
            }

            mentions.LMentionSenseSet(mentionId, sense);
        }
    }
}
