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
