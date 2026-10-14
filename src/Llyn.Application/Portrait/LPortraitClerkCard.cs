using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public static class LPortraitClerkCard
{
    public static void LPortraitBandAdd(
        List<LPortraitSection> sections, string heading, IReadOnlyList<LPortraitSection> cards)
    {
        ArgumentNullException.ThrowIfNull(sections);
        ArgumentNullException.ThrowIfNull(cards);

        if (cards.Count > 0)
        {
            sections.Add(LPortraitSection.LPortraitSectionCreate(heading, cards));
        }
    }

    public static void LPortraitIncomingAdd(
        List<LPortraitSection> sections, IReadOnlyList<LUsage> incoming, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(sections);
        ArgumentNullException.ThrowIfNull(incoming);
        ArgumentNullException.ThrowIfNull(label);

        if (incoming.Count == 0)
        {
            return;
        }

        List<LPortraitSection> rows = [];
        foreach (LUsage usage in incoming)
        {
            string owner = usage.LUsageOwner == LOwner.LOwnerCollocation
                ? label.LPortraitLabelCollocation
                : label.LPortraitLabelMeaning;
            string title = LPortraitText.LPortraitTextRead(usage.LUsageTitle, label.LPortraitLabelUnknown);
            rows.Add(new LPortraitSection(
                string.Empty,
                [new LPortraitLine(owner, title)],
                string.Empty,
                [],
                [],
                LPortraitSectionLink:
                    [new LPortraitLink(usage.LUsageHeadword, usage.LUsageLanguage)],
                LPortraitSectionRole: LPortraitRole.LPortraitRoleUsage));
        }

        sections.Add(LPortraitSection.LPortraitSectionCreate(label.LPortraitLabelIncoming, rows));
    }

    public static void LPortraitEtymologyAdd(
        List<LPortraitSection> sections,
        string headword,
        IReadOnlyList<LLineage> lineage,
        IReadOnlyList<LAncestorRelation> relations,
        LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(sections);
        ArgumentNullException.ThrowIfNull(headword);
        ArgumentNullException.ThrowIfNull(lineage);
        ArgumentNullException.ThrowIfNull(relations);
        ArgumentNullException.ThrowIfNull(label);

        if (lineage.Count == 0)
        {
            return;
        }

        Dictionary<long, string> names = relations.ToDictionary(
            static relation => relation.LAncestorRelationId, static relation => relation.LAncestorRelationName);
        List<LPortraitLine> lines = [new LPortraitLine(string.Empty, headword)];
        List<LPortraitLink> links = [];
        Stack<(LLineage, int)> pending = new();
        for (int index = lineage.Count - 1; index >= 0; index--)
        {
            pending.Push((lineage[index], 0));
        }

        while (pending.TryPop(out (LLineage, int) held))
        {
            (LLineage box, int depth) = held;
            string relation = box.LLineageRelationId is long id
                ? names.GetValueOrDefault(id, string.Empty)
                : string.Empty;
            string gloss = box.LLineageGloss.Length > 0 ? "‘" + box.LLineageGloss + "’" : string.Empty;
            string text = string.Join(
                " ", new[] { box.LLineageLanguage, box.LLineageText, gloss }.Where(static part => part.Length > 0));
            string opener = string.Concat(Enumerable.Repeat("· ", depth)) + (box.LLineageBorrowed ? "⇡ " : "↑ ");
            lines.Add(new LPortraitLine(relation, text, opener));

            LPortraitLink link = new(box.LLineageText, box.LLineageLanguage);
            if (box.LLineageEntryId is not null && !links.Contains(link))
            {
                links.Add(link);
            }

            for (int index = box.LLineageSources.Count - 1; index >= 0; index--)
            {
                pending.Push((box.LLineageSources[index], depth + 1));
            }
        }

        LLineage root = new(0, null, headword, string.Empty, string.Empty, null, false, lineage);
        sections.Add(new LPortraitSection(
            label.LPortraitLabelEtymology,
            lines,
            string.Empty,
            [],
            [],
            LPortraitSectionLink: links,
            LPortraitSectionLineage: root));
    }

    public static void LPortraitNoteAdd(List<LPortraitSection> sections, LEntryDraft draft, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(sections);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(label);

        if (draft.LEntryDraftNote.Length > 0)
        {
            sections.Add(new LPortraitSection(label.LPortraitLabelNote, [], draft.LEntryDraftNote, [], []));
        }
    }
}
