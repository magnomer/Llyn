using System;
using System.Collections.Generic;
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
                    [new LPortraitLink(usage.LUsageEntry, usage.LUsageHeadword, usage.LUsageLanguage)],
                LPortraitSectionRole: LPortraitRole.LPortraitRoleUsage));
        }

        sections.Add(LPortraitSection.LPortraitSectionCreate(label.LPortraitLabelIncoming, rows));
    }

    public static void LPortraitEtymologyAdd(
        List<LPortraitSection> sections,
        LEntryDraft draft,
        IReadOnlyDictionary<long, LPortraitLink> targets,
        LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(sections);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(targets);
        ArgumentNullException.ThrowIfNull(label);

        LEtymologyDraft etymology = draft.LEntryDraftEtymology;
        List<LPortraitLine> lines = [];
        List<LPortraitLink> links = [];

        if (etymology.LEtymologyDraftNarrated)
        {
            lines.Add(new LPortraitLine(string.Empty, etymology.LEtymologyDraftText));
            foreach (LMentionDraft mention in etymology.LEtymologyDraftMentions)
            {
                LPortraitEtymologyAdd(links, targets, mention.LMentionDraftEntry);
            }
        }
        else
        {
            foreach (long id in etymology.LEtymologyDraftEtymons)
            {
                LPortraitEtymologyAdd(links, targets, id);
            }
        }

        if (lines.Count == 0 && links.Count == 0)
        {
            return;
        }

        sections.Add(new LPortraitSection(
            label.LPortraitLabelEtymology, lines, string.Empty, [], [], LPortraitSectionLink: links));
    }

    private static void LPortraitEtymologyAdd(
        List<LPortraitLink> links, IReadOnlyDictionary<long, LPortraitLink> targets, long id)
    {
        if (targets.TryGetValue(id, out LPortraitLink? link) && !links.Contains(link))
        {
            links.Add(link);
        }
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
