using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public static class LEntryClerkEtymology
{
    public static void LEtymologyUpdate(
        LEtymologyVault etymologies, long entryId, LEntryDraft draft, List<LRevisionChange>? changes)
    {
        ArgumentNullException.ThrowIfNull(etymologies);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(draft);

        LEtymologyDraft stored = LEtymologyRead(etymologies, entryId);
        LEtymologyDraft current = draft.LEntryDraftEtymology;
        if (LEtymologyMatch(stored, current))
        {
            return;
        }

        if (current.LEtymologyDraftNarrated)
        {
            etymologies.LEtymologyEtymonSet(entryId, []);
            etymologies.LEtymologySave(entryId, current.LEtymologyDraftResolve());
        }
        else if (current.LEtymologyDraftLinked)
        {
            etymologies.LEtymologySave(entryId, null);
            etymologies.LEtymologyEtymonSet(entryId, current.LEtymologyDraftEtymons);
        }
        else
        {
            etymologies.LEtymologySave(entryId, null);
            etymologies.LEtymologyEtymonSet(entryId, []);
        }

        changes?.Add(new LRevisionChange(
            entryId,
            "etymology",
            current.LEtymologyDraftEmpty ? "delete" : stored.LEtymologyDraftEmpty ? "create" : "update",
            current.LEtymologyDraftNarrated ? current.LEtymologyDraftText : null));
    }

    public static LEtymologyDraft LEtymologyRead(LEtymologyVault etymologies, long entryId)
    {
        ArgumentNullException.ThrowIfNull(etymologies);

        List<long> targets = [];
        foreach (LEtymon etymon in etymologies.LEtymologyEtymonRead(entryId))
        {
            targets.Add(etymon.LEtymonTargetId);
        }

        return LEtymologyDraft.LEtymologyDraftCreate(etymologies.LEtymologyRead(entryId), targets);
    }

    public static bool LEtymologyMatch(LEtymologyDraft one, LEtymologyDraft other)
    {
        ArgumentNullException.ThrowIfNull(one);
        ArgumentNullException.ThrowIfNull(other);

        if (!string.Equals(
                one.LEtymologyDraftText.Trim(), other.LEtymologyDraftText.Trim(), StringComparison.Ordinal)
            || one.LEtymologyDraftMentions.Count != other.LEtymologyDraftMentions.Count
            || one.LEtymologyDraftEtymons.Count != other.LEtymologyDraftEtymons.Count)
        {
            return false;
        }

        for (int index = 0; index < one.LEtymologyDraftMentions.Count; index++)
        {
            LMentionDraft held = one.LEtymologyDraftMentions[index];
            LMentionDraft named = other.LEtymologyDraftMentions[index];
            if (held.LMentionDraftOffset != named.LMentionDraftOffset
                || held.LMentionDraftLength != named.LMentionDraftLength
                || held.LMentionDraftEntry != named.LMentionDraftEntry)
            {
                return false;
            }
        }

        for (int index = 0; index < one.LEtymologyDraftEtymons.Count; index++)
        {
            if (one.LEtymologyDraftEtymons[index] != other.LEtymologyDraftEtymons[index])
            {
                return false;
            }
        }

        return true;
    }
}
